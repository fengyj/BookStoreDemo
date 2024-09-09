using System.ComponentModel.DataAnnotations;

namespace BookStoreService.DtoModels.Identify {
    /// <summary>
    /// User information for registration
    /// </summary>
    public class RegistrationDto {
        /// <summary>
        /// Account name (email)
        /// </summary>
        [Required]
        [MinLength(5)]
        [MaxLength(200)]
        public string Email { get; set; } = string.Empty;
        /// <summary>
        /// Name
        /// </summary>
        [Required]
        [MinLength(5)]
        [MaxLength(200)]
        public string Username { get; set; } = string.Empty;
        /// <summary>
        /// Password
        /// </summary>
        [Required]
        [MinLength(8)]
        [MaxLength(32)]
        public string Password { get; set; } = string.Empty;
    }
}
