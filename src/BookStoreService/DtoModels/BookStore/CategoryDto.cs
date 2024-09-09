using BookStoreService.Models.BookStore;

namespace BookStoreService.DtoModels.BookStore {
    /// <summary>
    /// Model of Category
    /// </summary>
    public class CategoryDto {
        /// <summary>
        /// Id of the category.
        /// </summary>
        public int CategoryId { get; set; }
        /// <summary>
        /// For displaying
        /// </summary>
        public string Name { get; set; } = string.Empty;
        /// <summary>
        /// ProductType
        /// </summary>
        public string ProductType { get; set; } = string.Empty;
        /// <summary>
        /// Parent category Id. It's optional.
        /// </summary>
        public int? ParentCategoryId { get; set; }
        /// <summary>
        /// Children categories. An empty list means no children.
        /// </summary>
        public IEnumerable<CategoryDto> Children { get; set; } = [];
    }

    namespace Extensions {

        /// <summary>
        /// Extension functions of CategoryDto
        /// </summary>
        public static class CategoryDtoExtensions {

            /// <summary>
            /// Convert to CategoryDto
            /// </summary>
            /// <param name="category"></param>
            /// <param name="containChildren"></param>
            /// <returns></returns>
            public static CategoryDto Convert(this Category category, bool containChildren = true) {

                return new CategoryDto {
                    CategoryId = category.CategoryId,
                    Name = category.Name,
                    ParentCategoryId = category.ParentCategoryId,
                    Children = containChildren ? category.Children.Select(i => Convert(i)).ToList() : []
                };
            }

            /// <summary>
            /// Convert to Category
            /// </summary>
            /// <param name="category"></param>
            /// <returns></returns>
            public static Category Convert(this CategoryDto category) {

                return new Category {
                    CategoryId = category.CategoryId,
                    Name = category.Name,
                    ParentCategoryId = category.ParentCategoryId,
                    Children = category.Children.Select(i => Convert(i)).ToList()
                };
            }
        }
    }
}
