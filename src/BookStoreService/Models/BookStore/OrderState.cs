namespace BookStoreService.Models.BookStore {
    /// <summary>
    /// Status of order
    /// </summary>
    public enum OrderState {
        /// <summary>
        /// New order
        /// </summary>
        CheckingOut,
        /// <summary>
        /// Have been paid
        /// </summary>
        Placed,
        /// <summary>
        /// To ship
        /// </summary>
        ReadyToShip,
        /// <summary>
        /// In shipping
        /// </summary>
        Shipped,
        /// <summary>
        /// Received
        /// </summary>
        Delivered,
        /// <summary>
        /// Cancelled
        /// </summary>
        Cancelled
    }
}