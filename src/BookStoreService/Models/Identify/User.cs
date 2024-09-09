using Microsoft.AspNetCore.Identity;

namespace BookStoreService.Models.Identify {
    /// <summary>
    /// User
    /// </summary>
    public class User : IdentityUser {

        /// <summary>
        /// Constructor
        /// </summary>
        public User() : base() { }

        /// <summary>
        /// Constructor
        /// </summary>
        public User(string userName) : base(userName) { }

        /// <summary>
        /// The role of the user.
        /// </summary>
        public string Role { get; set; } = string.Empty;
    }
}
