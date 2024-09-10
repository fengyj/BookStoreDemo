using BookStoreService.DtoModels;
using BookStoreService.DtoModels.BookStore;
using BookStoreService.DtoModels.BookStore.Extensions;
using BookStoreService.Models.BookStore;
using BookStoreService.Services.Identify.Extensions;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BookStoreService.Controllers {
    /// <summary>
    /// APIs of Orders.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "User,Admin")]
    public class OrdersController : ControllerBase {
        private readonly BookStoreContext _context;
        private readonly ILogger<OrdersController> _logger;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="context"></param>
        /// <param name="logger"></param>
        public OrdersController(BookStoreContext context, ILogger<OrdersController> logger) {
            this._context = context;
            this._logger = logger;
        }

        /// <summary>
        /// Get orders.
        /// </summary>
        /// <param name="customerId">The Id of the customer.</param>
        /// <param name="pageFilter">The pagination filter info.</param>
        /// <returns></returns>
        [HttpGet("{customer_id}")]
        public async Task<ActionResult<PaginationResult<IEnumerable<OrderDto>>>> GetOrders(
            [FromRoute(Name = "customer_id")] string customerId,
            [FromQuery] PaginationFilter? pageFilter = null) {

            if (this.User.IsInRole("User") && this.User.GetUserId() != customerId) {
                return this.Forbid("Illegal action: current user tried to access others data.");
            }

            if (pageFilter == null)
                pageFilter = new PaginationFilter();

            var orders = this._context.Orders.Where(i => i.CustomerId == customerId);

            var totalCount = await orders.CountAsync();

            orders.Skip(pageFilter.GetSkipCount(20)).Take(pageFilter.GetPageSize());

            switch (pageFilter.GetSortBy("createdtime").ToLower()) {
                case "createdtime":
                    orders = pageFilter.GetIsAscend(false) ? orders.OrderBy(i => i.CreatedTime) : orders.OrderByDescending(i => i.CreatedTime);
                    break;
                default:
                    break;
            }

            var data = (await orders.Include(i => i.Lines).ToListAsync()).Select(i => i.Convert()).ToList();

            return new PaginationResult<IEnumerable<OrderDto>>(data, new PaginationInfo(pageFilter, totalCount));
        }

        /// <summary>
        /// Get an order.
        /// </summary>
        /// <param name="customerId">The Id of the customer.</param>
        /// <param name="orderId">The Id of the order.</param>
        /// <returns></returns>
        [HttpGet("{customer_id}/{order_id}")]
        public async Task<ActionResult<OrderDto>> GetOrder(
            [FromRoute(Name = "customer_id")] string customerId,
            [FromRoute(Name = "order_id")] int orderId) {

            if (this.User.IsInRole("User") && this.User.GetUserId() != customerId) {
                return this.Forbid("Illegal action: current user tried to access others data.");
            }

            var order = await this._context.Orders.Where(i => i.OrderId == orderId).Include(i => i.Lines).FirstOrDefaultAsync();

            if (order == null || order.CustomerId != customerId) {
                return this.NotFound();
            }

            return order.Convert();
        }

        /// <summary>
        /// Create an order
        /// </summary>
        /// <param name="requstData">Only the ProductId and Quantity in the Lines are required.</param>
        /// <returns></returns>
        [HttpPost]
        [Authorize(Roles = "User")]
        public async Task<ActionResult<Order>> PostOrder([FromBody] CreateOrderRequestDto requstData) {

            var customerId = this.User.GetUserId();
            if (customerId == null) {
                return this.BadRequest("Unknown customer.");
            }

            if (requstData == null || requstData.Items == null || !requstData.Items.Any()) {
                return this.BadRequest("No order items.");
            }
            if (requstData.Items.Any(i => i.Quantity <= 0)) {
                return this.ValidationProblem($"{nameof(OrderLineDto.Quantity)} must be larger than zero.");
            }
            var productIds = requstData.Items.Select(i => i.ProductId).ToList();
            var products = await this._context.Products.Where(i => productIds.Contains(i.ProductId)).ToDictionaryAsync(i => i.ProductId);
            var order = new Order {
                CustomerId = customerId,
                Lines = requstData.Items.Select(i => products.TryGetValue(i.ProductId, out var product) ? new OrderLine {
                    ProductId = product.ProductId,
                    DisplayName = product.DisplayName,
                    PricePerUnit = product.Price,
                    Quantity = i.Quantity
                } : null)
                .Where(i => i != null)
                .Select(i => i!).ToList()
            };
            if (order.Lines.Count != productIds.Count) {
                return this.ValidationProblem("Product cannot be found.");
            }
            order.TotalPrice = order.CalcTotalPrice();

            this._context.Orders.Add(order);
            await this._context.SaveChangesAsync();

            return this.CreatedAtAction("GetOrder", new { customer_id = order.CustomerId, order_id = order.OrderId }, order.Convert());
        }
    }
}
