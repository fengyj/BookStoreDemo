namespace BookStoreService.Models.BookStore {
    /// <summary>
    /// Order
    /// </summary>
    public class Order {
        /// <summary>
        /// Id of the order
        /// </summary>
        public int OrderId { get; set; }
        /// <summary>
        /// Id of the customer
        /// </summary>
        public string CustomerId { get; set; } = string.Empty;
        /// <summary>
        /// Created time of the order
        /// </summary>
        public DateTime CreatedTime { get; set; } = DateTime.Now;
        /// <summary>
        /// Last updated time of the order
        /// </summary>
        public DateTime LastUpdatedTime { get; set; } = DateTime.Now;
        /// <summary>
        /// Total price of the order
        /// </summary>
        public decimal TotalPrice { get; set; }
        /// <summary>
        /// Status of the order
        /// </summary>
        public OrderState State { get; set; }
        /// <summary>
        /// Ordered items in the order
        /// </summary>

        public List<OrderLine> Lines { get; set; } = [];

        /// <summary>
        /// for calculate TotalPrice
        /// </summary>
        /// <returns></returns>
        public decimal CalcTotalPrice() {
            return this.Lines.Sum(i => i.PricePerUnit * i.Quantity);
        }
    }
}
