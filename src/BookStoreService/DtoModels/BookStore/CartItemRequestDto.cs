using System.ComponentModel.DataAnnotations;

namespace BookStoreService.DtoModels.BookStore {
    /// <summary>
    /// Model for adding/updating cart
    /// </summary>
    public class CartItemRequestDto {
        /// <summary>
        /// Quantity of the product in the cart
        /// </summary>
        [Range(1, 10000)]
        public uint Quantity { get; set; }
        /// <summary>
        /// Product info
        /// </summary>
        [Range(1, int.MaxValue)]
        public int ProductId { get; set; }
    }
}
