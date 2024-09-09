using System.ComponentModel.DataAnnotations;

using BookStoreService.Models.BookStore;

using Microsoft.OpenApi.Extensions;

namespace BookStoreService.DtoModels.BookStore {
    /// <summary>
    /// Model of Order
    /// </summary>
    public class OrderDto {
        /// <summary>
        /// Order Id
        /// </summary>
        [Required]
        public int OrderId { get; set; }
        /// <summary>
        /// Customer Id
        /// </summary>
        [Required]
        public string CustomerId { get; set; } = string.Empty;
        /// <summary>
        /// Order crerated time
        /// </summary>
        public DateTime CreatedTime { get; set; } = DateTime.Now;
        /// <summary>
        /// Order last updated time
        /// </summary>
        public DateTime LastUpdatedTime { get; set; } = DateTime.Now;
        /// <summary>
        /// Total price of the order.
        /// </summary>
        [Range(0, int.MaxValue)]
        public decimal TotalPrice { get; set; }
        /// <summary>
        /// Order state
        /// </summary>
        [Required]
        public string State { get; set; } = default(OrderState).GetDisplayName();
        /// <summary>
        /// Items of the order
        /// </summary>
        public IEnumerable<OrderLineDto> Lines { get; set; } = [];
    }

    namespace Extensions {

        /// <summary>
        /// Extension functions of OrderDto
        /// </summary>
        public static class OrderDtoExtensions {

            /// <summary>
            /// Convert to Order
            /// </summary>
            /// <param name="order"></param>
            /// <returns></returns>
            public static OrderDto Convert(this Order order) {

                return new OrderDto {
                    OrderId = order.OrderId,
                    CustomerId = order.CustomerId,
                    CreatedTime = order.CreatedTime,
                    LastUpdatedTime = order.LastUpdatedTime,
                    TotalPrice = order.TotalPrice,
                    State = order.State.GetDisplayName(),
                    Lines = order.Lines.Select(i => i.Convert()).ToList()
                };
            }

            /// <summary>
            /// Convert to OrderDto
            /// </summary>
            /// <param name="order"></param>
            /// <returns></returns>
            public static Order Convert(this OrderDto order) {

                return new Order {
                    OrderId = order.OrderId,
                    CustomerId = order.CustomerId,
                    CreatedTime = order.CreatedTime,
                    LastUpdatedTime = order.LastUpdatedTime,
                    TotalPrice = order.TotalPrice,
                    State = order.State.GetEnumFromDisplayName<OrderState>(),
                    Lines = order.Lines.Select(i => i.Convert()).ToList()
                };
            }
        }
    }
}
