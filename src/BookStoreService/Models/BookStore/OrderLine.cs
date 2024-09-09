namespace BookStoreService.Models.BookStore {
    public class OrderLine {
        /// <summary>
        /// Order Id
        /// </summary>
        public int OrderId { get; set; }
        /// <summary>
        /// Product Id
        /// </summary>
        public int ProductId { get; set; }
        public uint Quantity { get; set; }
        /// <summary>
        /// Product price when purchasing
        /// </summary>
        public decimal PricePerUnit { get; set; }
        /// <summary>
        /// Product name for displaying
        /// </summary>
        public string DisplayName { get; set; } = string.Empty;
        public Product? Product { get; set; } = null;
    }
}
