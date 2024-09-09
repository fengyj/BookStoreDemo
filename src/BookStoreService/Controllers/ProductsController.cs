using BookStoreService.DtoModels;
using BookStoreService.DtoModels.BookStore;
using BookStoreService.DtoModels.BookStore.Extensions;
using BookStoreService.Models.BookStore;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BookStoreService.Controllers {
    /// <summary>
    /// APIs of Products
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Admin")]
    public class ProductsController : ControllerBase {

        private readonly BookStoreContext _context;
        private readonly ILogger<ProductsController> _logger;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="context"></param>
        /// <param name="logger"></param>
        public ProductsController(BookStoreContext context, ILogger<ProductsController> logger) {
            this._context = context;
            this._logger = logger;
        }

        /// <summary>
        /// Get product list.
        /// </summary>
        /// <param name="categoryId">If specified, return the products by category id.</param>
        /// <param name="keyWords">If specified, return the products which DisplayName contain the key words.</param>
        /// <param name="isDeactive">If specified, return the products by deactive. Default value is false.</param>
        /// <param name="pageFilter">The pagination filter info.</param>
        /// <returns></returns>
        [HttpGet]
        [AllowAnonymous]
        public async Task<ActionResult<PaginationResult<IEnumerable<ProductDto>>>> GetProducts(
            [FromQuery(Name = "category")] int? categoryId = null,
            [FromQuery(Name = "key_words")] string? keyWords = null,
            [FromQuery(Name = "is_deactive")] bool? isDeactive = false,
            [FromQuery] PaginationFilter? pageFilter = null) {

            if (pageFilter == null)
                pageFilter = new PaginationFilter();

            var products = (IQueryable<Product>)this._context.Products;
            if (categoryId.HasValue) products = products.Where(i => i.CategoryId == categoryId);
            if (!string.IsNullOrWhiteSpace(keyWords)) {
                var words = keyWords.Split(' ').Where(w => !string.IsNullOrWhiteSpace(w)).Select(w => $"%{w}%");
                foreach (var word in words)
                    products = products.Where(i => EF.Functions.Like(i.DisplayName, word));
            }
            if (isDeactive.HasValue)
                products = products.Where(i => i.IsDeactive == isDeactive);

            var totalCount = await products.CountAsync();

            products.Skip(pageFilter.GetSkipCount(20)).Take(pageFilter.GetPageSize());

            switch (pageFilter.GetSortBy("price").ToLower()) {
                case "price":
                    products = pageFilter.GetIsAscend(true) ? products.OrderBy(i => i.Price) : products.OrderByDescending(i => i.Price);
                    break;
                default:
                    break;
            }

            var data = (await products.ToListAsync()).Select(i => i.Convert()).ToList();

            return new PaginationResult<IEnumerable<ProductDto>>(data, new PaginationInfo(pageFilter, totalCount));
        }

        /// <summary>
        /// Get the product by Id, and with other filters if have.
        /// </summary>
        /// <param name="id">The id of the product.</param>
        /// <param name="isDeactive">If specified, if the product found is not matched, will return NotFound.</param>
        /// <returns></returns>
        [HttpGet("{id}")]
        [AllowAnonymous]
        public async Task<ActionResult<ProductDto>> GetProduct(
            int id,
            [FromQuery(Name = "is_deactive")] bool? isDeactive = false) {

            var product = await this._context.Products.FindAsync(id);

            if (product == null || (isDeactive != null && product.IsDeactive != isDeactive.Value)) {
                return this.NotFound();
            }

            return product.Convert();
        }

        /// <summary>
        /// Update the product.
        /// </summary>
        /// <param name="id">The id of the product.</param>
        /// <param name="product">The content of the product.</param>
        /// <returns></returns>
        [HttpPut("{id}")]
        public async Task<IActionResult> PutProduct(int id, ProductDto product) {

            if (!this.TryVerify(product, out var result) && result != null)
                return result;

            if (id != product.ProductId) {
                return this.BadRequest("ProductId doens't match.");
            }

            this._context.Entry(product.Convert()).State = EntityState.Modified;

            try {
                await this._context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException) {
                if (!await this.ProductExistsAsync(id)) {
                    return this.NotFound();
                }
                else {
                    throw;
                }
            }

            return this.NoContent();
        }

        /// <summary>
        /// Create a new product.
        /// </summary>
        /// <param name="product">The content of the product.</param>
        /// <returns></returns>
        [HttpPost]
        public async Task<ActionResult<ProductDto>> PostProduct(ProductDto product) {

            if (!this.TryVerify(product, out var result) && result != null)
                return result;

            var obj = product.Convert();
            this._context.Products.Add(obj);
            await this._context.SaveChangesAsync();

            return this.CreatedAtAction("GetProduct", new { id = product.ProductId }, obj.Convert());
        }

        /// <summary>
        /// Delete the product if it's not been used.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProduct(int id) {

            var product = await this._context.Products.FindAsync(id);
            if (product == null) {
                return this.NotFound();
            }

            var hasBeenUsed = await this._context.CartItems.AnyAsync(i => i.ProductId == id);
            hasBeenUsed = hasBeenUsed || await this._context.Orders.AnyAsync(i => i.Lines.Any(l => l.ProductId == id));
            if (hasBeenUsed) {
                return this.ValidationProblem("The book has been used.");
            }


            this._context.Products.Remove(product);
            await this._context.SaveChangesAsync();

            return this.NoContent();
        }

        private async Task<bool> ProductExistsAsync(int id) {
            return await this._context.Products.AnyAsync(e => e.ProductId == id);
        }

        private bool TryVerify(ProductDto product, out ActionResult? result) {

            if (product == null || string.IsNullOrWhiteSpace(product.DisplayName))
                result = this.ValidationProblem($"{nameof(ProductDto.DisplayName)} cannot be blank.");
            else if (product.Price < 0)
                result = this.ValidationProblem($"{nameof(ProductDto.Price)} cannot be less than 0.");
            else if (product.CategoryId <= 0)
                result = this.ValidationProblem($"{nameof(ProductDto.CategoryId)} hasn't been specified.");
            else
                result = null;

            return result == null;
        }
    }
}
