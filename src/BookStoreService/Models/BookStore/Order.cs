namespace BookStoreService.Models.BookStore {
    public class Order {
        public int OrderId { get; set; }
        public string CustomerId { get; set; } = string.Empty;
        public DateTime CreatedTime { get; set; } = DateTime.Now;
        public DateTime LastUpdatedTime { get; set; } = DateTime.Now;
        public decimal TotalPrice { get; set; }
        public OrderState State { get; set; }

        public List<OrderLine> Lines { get; set; } = [];

        public decimal CalcTotalPrice() {
            return this.Lines.Sum(i => i.PricePerUnit * i.Quantity);
        }
    }
}
