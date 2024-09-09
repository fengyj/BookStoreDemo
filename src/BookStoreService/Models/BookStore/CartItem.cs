namespace BookStoreService.Models.BookStore {
    public class CartItem {
        public string CustomerId { get; set; } = string.Empty;
        public int ProductId { get; set; }
        public uint Quantity { get; set; }
        public Product? Product { get; set; }
    }
}
