using System.ComponentModel.DataAnnotations;

namespace BookStoreService.DtoModels.Identify {
    /// <summary>
    /// Log in request data
    /// </summary>
    public class LoginRequestDto {
        /// <summary>
        /// Account (email)
        /// </summary>
        [Required]
        [MinLength(5)]
        [MaxLength(200)]
        public string? Email { get; set; }
        /// <summary>
        /// Password
        /// </summary>
        [Required]
        [MinLength(8)]
        [MaxLength(32)]
        public string? Password { get; set; }
    }
}
