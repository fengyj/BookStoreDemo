namespace BookStoreService.DtoModels.BookStore {
    /// <summary>
    /// Model for creating order
    /// </summary>
    public class CreateOrderRequestDto {
        /// <summary>
        /// Items to order.
        /// </summary>
        public IEnumerable<CartItemRequestDto> Items { get; set; } = [];
    }
}
