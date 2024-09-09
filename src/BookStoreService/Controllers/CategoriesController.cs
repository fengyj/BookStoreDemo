using BookStoreService.DtoModels.BookStore;
using BookStoreService.DtoModels.BookStore.Extensions;
using BookStoreService.Models.BookStore;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BookStoreService.Controllers {
    /// <summary>
    /// APIs of Category.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Admin")]
    public class CategoriesController : ControllerBase {

        private readonly BookStoreContext _context;
        private readonly ILogger<CategoriesController> _logger;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="context"></param>
        /// <param name="logger"></param>
        public CategoriesController(BookStoreContext context, ILogger<CategoriesController> logger) {
            this._context = context;
            this._logger = logger;
        }

        /// <summary>
        /// Return all the categories in hirachy structure.
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [AllowAnonymous]
        public async Task<ActionResult<IEnumerable<CategoryDto>>> GetCategories([FromQuery(Name = "tree")] bool? inTreeStruct = false) {

            var categories = await this._context.Categories.ToListAsync();
            if (inTreeStruct ?? false)
                return categories.Where(i => i.ParentCategoryId == null)
                    .Select(i => i.Convert(containChildren: true))
                    .ToList();
            else
                return categories.Select(i => i.Convert(containChildren: false)).ToList();
        }

        /// <summary>
        /// Return the category by Id.
        /// </summary>
        /// <param name="id">The Id of the category.</param>
        /// <returns></returns>
        [HttpGet("{id}")]
        [AllowAnonymous]
        public async Task<ActionResult<CategoryDto>> GetCategory(int id) {
            var category = await this._context.Categories.FindAsync(id);

            if (category == null) {
                return this.NotFound();
            }

            return category.Convert();
        }

        /// <summary>
        /// Update the category info.
        /// </summary>
        /// <param name="id">The Id of the category.</param>
        /// <param name="category">The information to update.</param>
        /// <returns></returns>
        [HttpPut("{id}")]
        public async Task<IActionResult> PutCategory(int id, CategoryDto category) {

            if (!this.TryVerify(category, out var result) && result != null)
                return result;

            if (id != category.CategoryId) {
                return this.BadRequest("Category Id doesn't match.");
            }

            this._context.Entry(category.Convert()).State = EntityState.Modified;

            try {
                await this._context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException) {
                if (!await this.CategoryExistsAsync(id)) {
                    return this.NotFound();
                }
                else {
                    throw;
                }
            }

            return this.NoContent();
        }

        /// <summary>
        /// Create new category.
        /// </summary>
        /// <param name="category">The category to create.</param>
        /// <returns></returns>
        [HttpPost]
        public async Task<ActionResult<CategoryDto>> PostCategory(CategoryDto category) {

            if (!this.TryVerify(category, out var result) && result != null)
                return result;

            var data = category.Convert();
            this._context.Categories.Add(data);
            await this._context.SaveChangesAsync();

            return this.CreatedAtAction("GetCategory", new { id = category.CategoryId }, data.Convert());
        }

        /// <summary>
        /// Delete the category. If it's been used by any products, it cannot be deleted.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCategory(int id) {
            var category = await this._context.Categories.FindAsync(id);
            if (category == null) {
                return this.NotFound();
            }

            var hasBeenUsed = await this._context.Products.AnyAsync(i => i.CategoryId == id);
            if (hasBeenUsed) {
                return this.ValidationProblem("The category has been used.");
            }

            this._context.Categories.Remove(category);
            await this._context.SaveChangesAsync();

            return this.NoContent();
        }

        private async Task<bool> CategoryExistsAsync(int id) {
            return await this._context.Categories.AnyAsync(e => e.CategoryId == id);
        }

        private bool TryVerify(CategoryDto category, out ActionResult? result) {

            if (category == null || string.IsNullOrWhiteSpace(category.Name))
                result = this.ValidationProblem($"{nameof(CategoryDto.Name)} cannot be blank.");
            else
                result = null;

            return result == null;
        }
    }
}
