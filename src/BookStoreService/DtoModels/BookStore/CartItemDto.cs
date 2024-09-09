using System.ComponentModel.DataAnnotations;

using BookStoreService.Models.BookStore;

namespace BookStoreService.DtoModels.BookStore {
    /// <summary>
    /// Model for cart item.
    /// </summary>
    public class CartItemDto {
        /// <summary>
        /// Quantity of the product in the cart
        /// </summary>
        [Range(1, 10000)]
        public uint Quantity { get; set; }
        /// <summary>
        /// Product info
        /// </summary>
        public ProductDto? Product { get; set; }
    }

    namespace Extensions {

        /// <summary>
        /// Extension functions of CartItemDto
        /// </summary>
        public static class CartItemDtoExtensions {

            /// <summary>
            /// Convert CartItem object to CartItemDto object
            /// </summary>
            /// <param name="item"></param>
            /// <returns></returns>
            public static CartItemDto Convert(this CartItem item) {

                return new CartItemDto {
                    Quantity = item.Quantity,
                    Product = item.Product?.Convert()
                };
            }

            /// <summary>
            /// Convert CartItemDto object to CartItem object
            /// </summary>
            /// <param name="item"></param>
            /// <param name="customerId"></param>
            /// <returns></returns>
            public static CartItem Convert(this CartItemDto item, string customerId) {

                return new CartItem {
                    Quantity = item.Quantity,
                    CustomerId = customerId,
                    ProductId = item.Product?.ProductId ?? 0
                };
            }
        }
    }
}
