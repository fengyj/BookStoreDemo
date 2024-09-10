using BookStoreService.Models.Identify;

namespace BookStoreService.DtoModels.Identify {
    /// <summary>
    /// Information of user
    /// </summary>
    public class UserInfoDto {
        /// <summary>
        /// Id of the user
        /// </summary>
        public string UserId { get; set; } = string.Empty;
        /// <summary>
        /// User name
        /// </summary>
        public string UserName { get; set; } = string.Empty;
        /// <summary>
        /// Email
        /// </summary>
        public string Email { get; set; } = string.Empty;
        /// <summary>
        /// Role of the user
        /// </summary>
        public string Role { get; set; } = string.Empty;
    }

    namespace Extensions {

        /// <summary>
        /// Extension functions of UserInfoDto
        /// </summary>
        public static class UserInfoDtoExtensions {

            /// <summary>
            /// Conver to UserInfoDto
            /// </summary>
            /// <param name="user"></param>
            /// <returns></returns>
            public static UserInfoDto Convert(this User user) {

                return new UserInfoDto {
                    UserId = user.Id,
                    UserName = user.UserName!,
                    Email = user.Email!,
                    Role = user.Role
                };
            }
        }
    }
}
