using BookStoreService.Models.BookStore;

using Microsoft.EntityFrameworkCore;

namespace BookStoreService.Tests.Models.BookStore {

    public class BookStoreContextTests {

        [Fact]
        public void Test() {

            var builder = new DbContextOptionsBuilder<BookStoreContext>();
            builder.UseInMemoryDatabase("BookStore");
            var options = builder.Options;

            using var ctx = new BookStoreContext(options);

            ctx.AddRange(
                new Category {
                    Name = "Fiction",
                    Children = [
                    new Category { Name = "Sci-Fi" },
                    new Category { Name = "Detective story" }
                ]
                },
                new Category { Name = "History" });

            var count = ctx.SaveChanges();
            Assert.Equal(4, count);

            var categories = ctx.Categories.ToList();
            Assert.Equal(4, categories.Count);
        }
    }
}
