using System.Security.Claims;

using BookStoreService.Controllers;
using BookStoreService.DtoModels.Identify;
using BookStoreService.Models.Identify;
using BookStoreService.Services.Identify;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

using Moq;

namespace BookStoreService.Tests.Controllers {
    public class UsersControllerTests {

        private readonly Mock<ILogger<UsersController>> _logger;
        private readonly JwtTokenService _jwtTokenSvc;
        private readonly Mock<UserManager<User>> _userManager;

        public UsersControllerTests() {

            this._logger = new Mock<ILogger<UsersController>>();
            this._jwtTokenSvc = new JwtTokenService(new Mock<ILogger<JwtTokenService>>().Object);
            this._userManager = new Mock<UserManager<User>>(new Mock<IUserStore<User>>().Object, null, null, null, null, null, null, null, null);
        }

        [Fact]
        public async Task Test_Get() {

            using var ctx = new IdentifyContext();

            var admin = new User { UserName = "admin", Email = "admin@x.org", Role = "Admin" };
            var user = new User { UserName = "user", Email = "user@x.org", Role = "User" };
            ctx.Users.AddRange(admin, user);

            await ctx.SaveChangesAsync();

            var principal = new ClaimsPrincipal(new ClaimsIdentity(
                [
                    new Claim(ClaimTypes.NameIdentifier, admin.Id),
                    new Claim(ClaimTypes.Role, admin.Role)
                ], "mock"));

            var controller = new UsersController(this._userManager.Object, ctx, this._jwtTokenSvc, this._logger.Object);
            controller.ControllerContext = new ControllerContext {
                HttpContext = new DefaultHttpContext { User = principal }
            };

            var result = await controller.Get(null);

            Assert.NotNull(result);
            Assert.NotNull(result.Value);
            Assert.Equal(admin.Id, result.Value.UserId);
            Assert.Equal(admin.UserName, result.Value.UserName);
            Assert.Equal(admin.Email, result.Value.Email);
            Assert.Equal(admin.Role, result.Value.Role);

            result = await controller.Get(user.Id);

            Assert.NotNull(result);
            Assert.NotNull(result.Value);
            Assert.Equal(user.Id, result.Value.UserId);
            Assert.Equal(user.UserName, result.Value.UserName);
            Assert.Equal(user.Email, result.Value.Email);
            Assert.Equal(user.Role, result.Value.Role);

            principal = new ClaimsPrincipal(new ClaimsIdentity(
                [
                    new Claim(ClaimTypes.NameIdentifier, user.Id),
                    new Claim(ClaimTypes.Role, user.Role)
                ], "mock"));

            controller.ControllerContext = new ControllerContext {
                HttpContext = new DefaultHttpContext { User = principal }
            };

            result = await controller.Get(null);

            Assert.NotNull(result);
            Assert.NotNull(result.Value);
            Assert.Equal(user.Id, result.Value.UserId);
            Assert.Equal(user.UserName, result.Value.UserName);
            Assert.Equal(user.Email, result.Value.Email);
            Assert.Equal(user.Role, result.Value.Role);

            result = await controller.Get(admin.Id);

            Assert.IsType<ForbidResult>(result.Result);
        }

        [Fact]
        public async Task Test_Register() {

            using var ctx = new IdentifyContext();
            this._userManager.Setup(i => i.CreateAsync(It.IsAny<User>(), It.IsAny<string>())).ReturnsAsync(IdentityResult.Success);

            var controller = new UsersController(this._userManager.Object, ctx, this._jwtTokenSvc, this._logger.Object);
            var result = await controller.Register(new DtoModels.Identify.RegistrationDto { Email = "User2@x.org", Username = "User2", Password = "1@qwASzx" });

            Assert.IsType<CreatedAtActionResult>(result);
            var data = (result as CreatedAtActionResult)?.Value;
            Assert.NotNull(data);
            Assert.Equal("User2", (data as RegistrationDto)?.Username);
            Assert.Equal("User2@x.org", (data as RegistrationDto)?.Email);

        }
    }
}
