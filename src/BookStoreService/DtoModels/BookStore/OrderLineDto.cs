using System.ComponentModel.DataAnnotations;

using BookStoreService.Models.BookStore;

namespace BookStoreService.DtoModels.BookStore {
    /// <summary>
    /// Model of the item in order.
    /// </summary>
    public class OrderLineDto {
        /// <summary>
        /// Product Id
        /// </summary>
        [Required]
        public int ProductId { get; set; }
        /// <summary>
        /// Quantity of the product
        /// </summary>
        [Required]
        public uint Quantity { get; set; }
        /// <summary>
        /// Product price when purchasing
        /// </summary>
        [Required]
        public decimal PricePerUnit { get; set; }
        /// <summary>
        /// Product name for displaying
        /// </summary>
        [Required]
        public string DisplayName { get; set; } = string.Empty;
    }

    namespace Extensions {

        /// <summary>
        /// Extension functions of OrderLineDto
        /// </summary>
        public static class OrderLineDtoExtensions {

            /// <summary>
            /// Conver to OrderLineDto
            /// </summary>
            /// <param name="item"></param>
            /// <returns></returns>
            public static OrderLineDto Convert(this OrderLine item) {

                return new OrderLineDto {
                    ProductId = item.ProductId,
                    DisplayName = item.DisplayName,
                    PricePerUnit = item.PricePerUnit,
                    Quantity = item.Quantity
                };
            }

            /// <summary>
            /// Convert to OrderLine
            /// </summary>
            /// <param name="item"></param>
            /// <returns></returns>
            public static OrderLine Convert(this OrderLineDto item) {

                return new OrderLine {
                    Quantity = item.Quantity,
                    ProductId = item.ProductId,
                    PricePerUnit = item.PricePerUnit,
                    DisplayName = item.DisplayName
                };
            }
        }
    }
}
