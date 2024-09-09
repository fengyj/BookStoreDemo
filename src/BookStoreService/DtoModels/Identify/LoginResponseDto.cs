namespace BookStoreService.DtoModels.Identify {
    /// <summary>
    /// Log in response data
    /// </summary>
    public class LoginResponseDto {
        /// <summary>
        /// Account name (email)
        /// </summary>
        public string Email { get; set; } = string.Empty;
        /// <summary>
        /// Name
        /// </summary>
        public string UserName { get; set; } = string.Empty;
        /// <summary>
        /// JWT token
        /// </summary>
        public string JwtToken { get; set; } = string.Empty;
    }
}
