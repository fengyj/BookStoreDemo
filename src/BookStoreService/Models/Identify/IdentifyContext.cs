using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace BookStoreService.Models.Identify {
    /// <summary>
    /// DbContext of Identify DB.
    /// </summary>
    public class IdentifyContext : IdentityDbContext<User> {

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="options">If options is omitted, then uses an InMemoryDatabase.</param>
        public IdentifyContext(DbContextOptions<IdentifyContext>? options = null)
            : base(options ?? new DbContextOptionsBuilder<IdentifyContext>().UseInMemoryDatabase("IdentifyDB").Options) { }


        /// <summary>
        /// Overwrite of the function, invoke DetectChanges function before saving.
        /// </summary>
        /// <returns></returns>
        public override int SaveChanges() {

            this.ChangeTracker.DetectChanges();
            return base.SaveChanges();
        }
    }
}
