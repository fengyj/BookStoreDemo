using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

using BookStoreService.Models.Identify;

using Microsoft.IdentityModel.Tokens;

namespace BookStoreService.Services.Identify {
    /// <summary>
    /// For handling JWT Token
    /// </summary>
    public class JwtTokenService {

        private readonly ILogger<JwtTokenService> _logger;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="logger"></param>
        public JwtTokenService(ILogger<JwtTokenService> logger) {
            this._logger = logger;
        }

        internal static int ExpirationMinutes { get; private set; }
        internal static string ValidIssuer { get; private set; } = string.Empty;
        internal static string ValidAudience { get; private set; } = string.Empty;
        internal static string RegisteredClaimNamesSub { get; private set; } = string.Empty;
        internal static string SecretKey { get; private set; } = string.Empty;
        internal static bool ValidateLifetime { get; private set; } = true;
        internal static bool ValidateIssuer { get; private set; } = false;
        internal static bool ValidateAudience { get; private set; } = false;

        /// <summary>
        /// Initialize Token settings.
        /// </summary>
        /// <param name="validIssuer"></param>
        /// <param name="validAudience"></param>
        /// <param name="sub"></param>
        /// <param name="secretKey"></param>
        /// <param name="expirationMinutes"></param>
        /// <param name="validateLifetime"></param>
        /// <param name="validateIssuer"></param>
        /// <param name="validateAudience"></param>
        public static void Initialize(
            string validIssuer,
            string validAudience,
            string sub,
            string secretKey,
            int expirationMinutes = 30,
            bool validateLifetime = true,
            bool validateIssuer = false,
            bool validateAudience = false) {

            ExpirationMinutes = expirationMinutes;
            ValidIssuer = validIssuer;
            ValidAudience = validAudience;
            RegisteredClaimNamesSub = sub;
            SecretKey = secretKey;
            ValidIssuer = validIssuer;
            ValidateAudience = validateAudience;
            ValidateLifetime = validateLifetime;
        }

        /// <summary>
        /// Create JWT token
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public string CreateToken(User user) {

            var expiration = DateTime.UtcNow.AddMinutes(ExpirationMinutes);
            var token = this.CreateJwtToken(
                this.CreateClaims(user),
                this.CreateSigningCredentials(),
                expiration
            );
            var tokenHandler = new JwtSecurityTokenHandler();

            this._logger.LogInformation("JWT Token created");

            return tokenHandler.WriteToken(token);
        }

        private JwtSecurityToken CreateJwtToken(List<Claim> claims, SigningCredentials credentials, DateTime expiration) =>
            new(ValidIssuer, ValidAudience, claims, expires: expiration, signingCredentials: credentials);

        private List<Claim> CreateClaims(User user) {

            try {
                var claims = new List<Claim>
                {
                new Claim(JwtRegisteredClaimNames.Sub, user.Id),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.Iat, DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString()),
                new Claim(ClaimTypes.NameIdentifier, user.Id),
                new Claim(ClaimTypes.Name, user.UserName ?? string.Empty),
                new Claim(ClaimTypes.Email, user.Email ?? string.Empty),
                new Claim(ClaimTypes.Role, user.Role)
            };

                return claims;
            }
            catch (Exception e) {
                Console.WriteLine(e);
                throw;
            }
        }

        private SigningCredentials CreateSigningCredentials() {

            return new SigningCredentials(
                new SymmetricSecurityKey(
                    Encoding.UTF8.GetBytes(SecretKey)
                ),
                SecurityAlgorithms.HmacSha256
            );
        }
    }

    namespace Extensions {
        /// <summary>
        /// Extension functions of ClaimsPrincipal
        /// </summary>
        public static class ClaimsPrincipalExtensions {

            /// <summary>
            /// Get user id from ClaimsPrincipal
            /// </summary>
            /// <param name="principal"></param>
            /// <returns></returns>
            public static string? GetUserId(this ClaimsPrincipal principal) {
                return principal.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            }

            /// <summary>
            /// Get user name from ClaimsPrincipal
            /// </summary>
            /// <param name="principal"></param>
            /// <returns></returns>
            public static string? GetUserName(this ClaimsPrincipal principal) {
                return principal.FindFirst(ClaimTypes.Name)?.Value;
            }

            /// <summary>
            /// Get user email from ClaimsPrincipal
            /// </summary>
            /// <param name="principal"></param>
            /// <returns></returns>
            public static string? GetEmail(this ClaimsPrincipal principal) {
                return principal.FindFirst(ClaimTypes.Email)?.Value;
            }

            /// <summary>
            /// get user role from ClaimsPrincipal
            /// </summary>
            /// <param name="principal"></param>
            /// <returns></returns>
            public static string? GetRole(this ClaimsPrincipal principal) {
                return principal.FindFirst(ClaimTypes.Role)?.Value;
            }
        }
    }
}
