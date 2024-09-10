namespace BookStoreService.Models.BookStore {
    /// <summary>
    /// Item in the order
    /// </summary>
    public class OrderLine {
        /// <summary>
        /// Order Id
        /// </summary>
        public int OrderId { get; set; }
        /// <summary>
        /// Product Id
        /// </summary>
        public int ProductId { get; set; }
        /// <summary>
        /// Quantity
        /// </summary>
        public uint Quantity { get; set; }
        /// <summary>
        /// Product price when purchasing
        /// </summary>
        public decimal PricePerUnit { get; set; }
        /// <summary>
        /// Product name for displaying
        /// </summary>
        public string DisplayName { get; set; } = string.Empty;
    }
}
