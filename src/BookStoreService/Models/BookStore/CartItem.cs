namespace BookStoreService.Models.BookStore {
    /// <summary>
    /// Item in the cart
    /// </summary>
    public class CartItem {
        /// <summary>
        /// Customer Id
        /// </summary>
        public string CustomerId { get; set; } = string.Empty;
        /// <summary>
        /// Product Id
        /// </summary>
        public int ProductId { get; set; }
        /// <summary>
        /// Quantity to order
        /// </summary>
        public uint Quantity { get; set; }
        /// <summary>
        /// related product
        /// </summary>
        public Product? Product { get; set; }
    }
}
