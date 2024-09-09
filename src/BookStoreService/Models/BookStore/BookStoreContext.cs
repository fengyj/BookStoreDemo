using Microsoft.EntityFrameworkCore;

namespace BookStoreService.Models.BookStore {
    /// <summary>
    /// DbContext of BookStore DB
    /// </summary>
    public class BookStoreContext : DbContext {

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="options">If options is omitted, then uses an InMemoryDatabase.</param>
        public BookStoreContext(DbContextOptions<BookStoreContext>? options = null)
            : base(options ?? new DbContextOptionsBuilder<BookStoreContext>().UseInMemoryDatabase("BookStoreDB").Options) { }

        /// <summary>
        /// Products
        /// </summary>
        public DbSet<Product> Products { get; set; }
        /// <summary>
        /// Categories
        /// </summary>
        public DbSet<Category> Categories { get; set; }
        /// <summary>
        /// Items in cart
        /// </summary>
        public DbSet<CartItem> CartItems { get; set; }
        /// <summary>
        /// Orders
        /// </summary>
        public DbSet<Order> Orders { get; set; }

        /// <summary>
        /// Definitions of the DB
        /// </summary>
        /// <param name="modelBuilder"></param>
        protected override void OnModelCreating(ModelBuilder modelBuilder) {

            modelBuilder.Entity<Category>()
                .HasOne(c => c.ParentCategory)
                .WithMany(c => c.Children)
                .HasForeignKey(c => c.ParentCategoryId);

            modelBuilder.Entity<OrderLine>()
                .HasKey(ol => new { ol.OrderId, ol.ProductId });

            modelBuilder.Entity<CartItem>()
                .HasKey(ol => new { ol.CustomerId, ol.ProductId });
        }

        /// <summary>
        /// Overwrite of the function, invoke DetectChanges function before saving.
        /// </summary>
        /// <returns></returns>
        public override int SaveChanges() {

            this.ChangeTracker.DetectChanges();
            return base.SaveChanges();
        }
    }

    namespace Extensions {

        /// <summary>
        /// Extension functions of BookStoreContext
        /// </summary>
        public static class BookStoreContextExtensions {

            /// <summary>
            /// Craete seed data
            /// </summary>
            /// <param name="context"></param>
            public static void EnsureSeedData(this BookStoreContext context) {

                if (!context.Categories.Any()) {

                    context.Categories.AddRange(
                        new Category {
                            Name = "Book",
                            Children = [
                            new Category{
                                    Name = "Others"
                                }
                        ]
                        },
                        new Category {
                            Name = "GiftCard"
                        });

                    context.SaveChanges();
                }

                if (!context.Products.Any()) {

                    var categoryOthers = context.Categories.Where(c => c.Name == "Others").FirstOrDefault();
                    if (categoryOthers != null) {
                        context.Products.Add(new Product {
                            CategoryId = categoryOthers.CategoryId,
                            DisplayName = "Sample Book",
                            Description = "Sample data",
                            Price = 0
                        });
                    }
                    var categoryGiftCard = context.Categories.Where(c => c.Name == "GiftCard").FirstOrDefault();
                    if (categoryGiftCard != null) {
                        context.Products.AddRange(
                            new Product {
                                CategoryId = categoryGiftCard.CategoryId,
                                DisplayName = "GiftCard 100",
                                Description = "Sample data",
                                Price = 100
                            },
                            new Product {
                                CategoryId = categoryGiftCard.CategoryId,
                                DisplayName = "GiftCard 50",
                                Description = "Sample data",
                                Price = 50
                            });
                    }
                    context.SaveChanges();
                }
            }
        }
    }
}
