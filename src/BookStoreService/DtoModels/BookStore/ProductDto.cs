using System.ComponentModel.DataAnnotations;

using BookStoreService.Models.BookStore;


namespace BookStoreService.DtoModels.BookStore {
    /// <summary>
    /// Model of Product
    /// </summary>
    public class ProductDto {
        /// <summary>
        /// Id of the product.
        /// </summary>
        [Required]
        public int ProductId { get; set; }
        /// <summary>
        /// Product name for displaying
        /// </summary>
        [Required]
        public string DisplayName { get; set; } = string.Empty;
        /// <summary>
        /// Description of the product
        /// </summary>
        public string Description { get; set; } = string.Empty;
        /// <summary>
        /// Price of the product
        /// </summary>
        [Required]
        [Range(0, int.MaxValue)]
        public decimal Price { get; set; }
        /// <summary>
        /// CategoryId of the product
        /// </summary>
        [Required]
        public int CategoryId { get; set; }
        /// <summary>
        /// Indicator of whether the product is available for sale.
        /// </summary>
        [Required]
        public bool IsDeactive { get; set; } = false;
        /// <summary>
        /// Customerized attributes
        /// </summary>
        public Dictionary<string, object> Attributes { get; set; } = [];
    }

    namespace Extensions {

        /// <summary>
        /// Extension functions of ProductDto
        /// </summary>
        public static class ProductDtoExtensions {

            /// <summary>
            /// Conver to ProductDto
            /// </summary>
            /// <param name="product"></param>
            /// <returns></returns>
            public static ProductDto Convert(this Product product) {

                return new ProductDto {
                    ProductId = product.ProductId,
                    DisplayName = product.DisplayName,
                    Description = product.Description,
                    Price = product.Price,
                    CategoryId = product.CategoryId,
                    IsDeactive = product.IsDeactive,
                    Attributes = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, object>>(product.Attributes) ?? []
                };
            }

            /// <summary>
            /// Conver to Produc
            /// </summary>
            /// <param name="product"></param>
            /// <returns></returns>
            public static Product Convert(this ProductDto product) {

                return new Product {
                    ProductId = product.ProductId,
                    DisplayName = product.DisplayName,
                    Description = product.Description,
                    IsDeactive = product.IsDeactive,
                    CategoryId = product.CategoryId,
                    Attributes = Newtonsoft.Json.JsonConvert.SerializeObject(product.Attributes)
                };
            }
        }
    }
}
