namespace BookStoreService.Models.BookStore {
    /// <summary>
    /// Information of product
    /// </summary>
    public class Product {
        /// <summary>
        /// Id of the product.
        /// </summary>
        public int ProductId { get; set; }
        /// <summary>
        /// Product name for displaying
        /// </summary>
        public string DisplayName { get; set; } = string.Empty;
        /// <summary>
        /// Description of the product
        /// </summary>
        public string Description { get; set; } = string.Empty;
        /// <summary>
        /// Price of the product
        /// </summary>
        public decimal Price { get; set; }
        /// <summary>
        /// Category Id of the product
        /// </summary>
        public int CategoryId { get; set; }
        /// <summary>
        /// Indicator of whether the product is available for sale.
        /// </summary>
        public bool IsDeactive { get; set; } = false;
        /// <summary>
        /// Customerized attributes
        /// </summary>
        public string Attributes { get; set; } = "{}";
    }
}
