namespace BookStoreService.Models.BookStore {
    /// <summary>
    /// Product category.
    /// </summary>
    public class Category {
        /// <summary>
        /// Id of the category.
        /// </summary>
        public int CategoryId { get; set; }
        /// <summary>
        /// For displaying
        /// </summary>
        public string Name { get; set; } = string.Empty;
        /// <summary>
        /// Parent category Id. It's optional.
        /// </summary>
        public int? ParentCategoryId { get; set; }
        /// <summary>
        /// Parent category. It's optional.
        /// </summary>
        public Category? ParentCategory { get; set; }
        /// <summary>
        /// Children categories. An empty list means no children.
        /// </summary>
        public List<Category> Children { get; set; } = [];
    }
}
