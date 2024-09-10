using BookStoreService.DtoModels.Identify;
using BookStoreService.DtoModels.Identify.Extensions;
using BookStoreService.Models.Identify;
using BookStoreService.Services.Identify;
using BookStoreService.Services.Identify.Extensions;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BookStoreService.Controllers {
    /// <summary>
    /// APIs of Users
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase {

        private readonly IdentifyContext _context;
        private readonly ILogger<UsersController> _logger;
        private readonly JwtTokenService _jwtTokenSvc;
        private readonly UserManager<User> _userManager;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="userManager"></param>
        /// <param name="context"></param>
        /// <param name="jwtTokenSvc"></param>
        /// <param name="logger"></param>
        public UsersController(
            UserManager<User> userManager,
            IdentifyContext context,
            JwtTokenService jwtTokenSvc,
            ILogger<UsersController> logger) {

            this._userManager = userManager;
            this._context = context;
            this._jwtTokenSvc = jwtTokenSvc;
            this._logger = logger;
        }

        /// <summary>
        /// Get user info
        /// </summary>
        /// <param name="id">user id</param>
        /// <returns></returns>
        [HttpGet("{id}")]
        [HttpGet("me")]
        [Authorize(Roles = "User,Admin")]
        public async Task<ActionResult<UserInfoDto>> Get(string? id) {

            id ??= this.User.GetUserId()!;
            if (this.User.IsInRole("User") && this.User.GetUserId() != id) {
                return this.Forbid("Illegal action: current user tried to access others data.");
            }

            var user = await this._context.Users.FindAsync(id);
            if (user == null) {
                return this.NotFound("Cannot find the user.");
            }

            return user.Convert();
        }

        /// <summary>
        /// Register a new user.
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        [HttpPost("register")]
        public async Task<IActionResult> Register(RegistrationDto data) {

            if (!this.ModelState.IsValid) {
                return this.BadRequest(this.ModelState);
            }

            var user = new User { UserName = data.Username, Email = data.Email, Role = "User" };
            var result = await this._userManager.CreateAsync(user, data.Password!);

            data.Password = string.Empty;
            if (result.Succeeded) {
                return this.CreatedAtAction(nameof(Register), new { email = user.Email, role = user.Role }, data);
            }

            foreach (var error in result.Errors) {
                this.ModelState.AddModelError(error.Code, error.Description);
            }

            return this.BadRequest(this.ModelState);
        }

        /// <summary>
        /// Login
        /// </summary>
        /// <param name="request"></param>
        /// <returns>Return JwtToken if succeed.</returns>
        [HttpPost("login")]
        public async Task<ActionResult<LoginResponseDto>> Login([FromBody] LoginRequestDto request) {

            if (!this.ModelState.IsValid) {
                return this.BadRequest(this.ModelState);
            }
            var user = await this._context.Users.Where(i => i.Email == request.Email).FirstOrDefaultAsync();
            var isPasswordValid = await this._userManager.CheckPasswordAsync(user ?? new User { Email = request.Email }, request.Password!);
            if (!isPasswordValid || user == null) {
                return this.BadRequest("Bad credentials");
            }

            var jwtToken = this._jwtTokenSvc.CreateToken(user);

            return this.Ok(new LoginResponseDto {
                UserName = user.UserName!,
                Email = user.Email!,
                JwtToken = jwtToken,
            });
        }

        /// <summary>
        /// Update user's role.
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPut("role")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> PutUserRole([FromBody] UserRoleRequestDto request) {

            var user = await this._context.Users.FirstOrDefaultAsync(i => i.Email == request.Email);
            if (user == null) {
                return this.BadRequest();
            }
            user.Role = request.Role;
            await this._context.SaveChangesAsync();

            return this.NoContent();
        }

    }
}
