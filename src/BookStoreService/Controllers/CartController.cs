using BookStoreService.DtoModels.BookStore;
using BookStoreService.DtoModels.BookStore.Extensions;
using BookStoreService.Models.BookStore;
using BookStoreService.Services.Identify.Extensions;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BookStoreService.Controllers {
    /// <summary>
    /// APIs of Cart
    /// </summary>
    [Authorize(Roles = "User")]
    [Route("api/[controller]")]
    [ApiController]
    public class CartController : ControllerBase {

        private readonly BookStoreContext _context;
        private readonly ILogger<CartController> _logger;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="context"></param>
        /// <param name="logger"></param>
        public CartController(BookStoreContext context, ILogger<CartController> logger) {
            this._context = context;
            this._logger = logger;
        }

        /// <summary>
        /// Get the cart data of authenticated user.
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<ActionResult<CartDto>> GetCart() {

            var customerId = this.User.GetUserId();
            if (customerId == null) {
                return this.BadRequest("Unknown customer.");
            }
            var items = await this._context.CartItems.Where(i => i.CustomerId == customerId).Include(i => i.Product).ToListAsync();

            return items.Convert(customerId);
        }

        /// <summary>
        /// Update quantity of the item in the cart.
        /// </summary>
        /// <param name="item">The Id of the product and the quantity.</param>
        /// <returns></returns>
        [HttpPut]
        public async Task<ActionResult<CartItemDto>> PutCartItem([FromBody] CartItemRequestDto item) {

            var customerId = this.User.GetUserId();
            if (customerId == null) {
                return this.BadRequest("Unknown customer");
            }

            if (item == null) {
                return this.BadRequest($"Request data cannot be null.");
            }

            var product = await this._context.Products.FindAsync(item.ProductId);
            if (product == null) {
                return this.BadRequest($"product cannot be found.");
            }

            var cartItem = new CartItem { CustomerId = customerId, ProductId = item.ProductId, Quantity = item.Quantity };

            this._context.Entry(cartItem).State = EntityState.Modified;

            try {
                await this._context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException) {
                if (await this.CartItemExistsAsync(customerId, item.ProductId)) {
                    throw;
                }
                else {
                    return this.NotFound();
                }
            }

            cartItem.Product = product;
            return cartItem.Convert();
        }

        /// <summary>
        /// Add new item to cart.
        /// </summary>
        /// <param name="item">The Id of the product and the quantity.</param>
        /// <returns></returns>
        [HttpPost]
        public async Task<ActionResult<CartItemDto>> PostCategory([FromBody] CartItemRequestDto item) {

            var customerId = this.User.GetUserId();
            if (customerId == null) {
                return this.BadRequest("Unknown customer.");
            }

            if (item == null) {
                return this.BadRequest($"Request data cannot be null.");
            }

            var product = await this._context.Products.FindAsync(item.ProductId);
            if (product == null) {
                return this.BadRequest($"product cannot be found.");
            }

            var cartItem = new CartItem { CustomerId = customerId, ProductId = item.ProductId, Quantity = item.Quantity };

            this._context.Entry(cartItem).State = EntityState.Added;

            await this._context.SaveChangesAsync();

            cartItem.Product = product;
            return cartItem.Convert();
        }

        /// <summary>
        /// Delete the item
        /// </summary>
        /// <param name="productId">The Id of the product.</param>
        /// <returns></returns>
        [HttpDelete]
        public async Task<IActionResult> DeleteCategory([FromForm(Name = "productId")] int productId) {

            var customerId = this.User.GetUserId();
            if (customerId == null) {
                return this.BadRequest("Unknow customer.");
            }

            this._context.Entry(new CartItem { CustomerId = customerId, ProductId = productId }).State = EntityState.Deleted;

            await this._context.SaveChangesAsync();

            return this.NoContent();
        }

        private async Task<bool> CartItemExistsAsync(string customerId, int productId) {
            return await this._context.CartItems.AnyAsync(i => i.CustomerId == customerId && i.ProductId == productId);
        }
    }
}
