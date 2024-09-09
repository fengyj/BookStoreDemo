using System.ComponentModel.DataAnnotations;

namespace BookStoreService.DtoModels.Identify {
    /// <summary>
    /// User role for update request.
    /// </summary>
    public class UserRoleRequestDto {
        /// <summary>
        /// Account name (email)
        /// </summary>
        [Required]
        [MinLength(5)]
        [MaxLength(200)]
        public string Email { get; set; } = string.Empty;
        /// <summary>
        /// The new role of the user
        /// </summary>
        [Required]
        [MinLength(2)]
        [MaxLength(200)]
        public string Role { get; set; } = string.Empty;
    }
}
