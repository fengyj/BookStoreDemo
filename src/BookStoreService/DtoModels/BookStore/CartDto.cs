using System.ComponentModel.DataAnnotations;

using BookStoreService.Models.BookStore;

namespace BookStoreService.DtoModels.BookStore {
    /// <summary>
    /// Model for cart
    /// </summary>
    public class CartDto {
        /// <summary>
        /// Customer Id
        /// </summary>
        [Required]
        public string CustomerId { get; set; } = string.Empty;
        /// <summary>
        /// Products in the cart.
        /// </summary>
        public IEnumerable<CartItemDto> Items { get; set; } = [];
    }

    namespace Extensions {

        /// <summary>
        /// Extension functions of CartDto
        /// </summary>
        public static class CartDtoExtensions {

            /// <summary>
            /// Convert list of CartItem to CartDto
            /// </summary>
            /// <param name="items"></param>
            /// <param name="customerId"></param>
            /// <returns></returns>
            public static CartDto Convert(this List<CartItem> items, string customerId) {

                return new CartDto {
                    CustomerId = customerId,
                    Items = items.Select(i => i.Convert()).ToList()
                };
            }

            /// <summary>
            /// Convert CartDto to CartItem list.
            /// </summary>
            /// <param name="cart"></param>
            /// <returns></returns>
            public static List<CartItem> Convert(this CartDto cart) {

                return cart.Items.Select(i => i.Convert(cart.CustomerId)).ToList();
            }
        }
    }
}
