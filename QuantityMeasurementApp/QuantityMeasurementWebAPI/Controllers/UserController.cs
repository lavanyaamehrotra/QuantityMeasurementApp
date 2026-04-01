using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using QuantityMeasurementBusinessLayer.Interface;
using QuantityMeasurementModel.Dto;
using System.Security.Claims;

namespace QuantityMeasurementWebAPI.Controllers
{
    /// <summary>User authentication and profile — Register, Login, Refresh, Profile</summary>
    [ApiController]
    [Route("api/v1/users")]
    [Produces("application/json")]
    public class UserController : ControllerBase
    {
        private readonly IAuthService       _auth;
        private readonly IGoogleAuthService _googleAuth;  // UC19
        private readonly ILogger<UserController> _logger;

        public UserController(
            IAuthService auth,
            IGoogleAuthService googleAuth,
            ILogger<UserController> logger)
        {
            _auth       = auth;
            _googleAuth = googleAuth;
            _logger     = logger;
        }

        /// <summary>Register a new user. Password is BCrypt hashed (work factor 12) before saving to SQL Server.</summary>
        [HttpPost("register")]
        [AllowAnonymous]
        [ProducesResponseType(typeof(AuthResponseDto), 201)]
        [ProducesResponseType(typeof(ErrorResponseDto), 409)]
        public async Task<IActionResult> Register([FromBody] RegisterRequestDto req)
        {
            _logger.LogInformation("[UserController] Register: {U}", req.Username);
            var result = await _auth.RegisterAsync(req);
            return StatusCode(201, result);
        }

        /// <summary>Login. Returns JWT access token (60 min) + cryptographic refresh token (7 days).</summary>
        [HttpPost("login")]
        [AllowAnonymous]
        [ProducesResponseType(typeof(AuthResponseDto), 200)]
        [ProducesResponseType(typeof(ErrorResponseDto), 401)]
        public async Task<IActionResult> Login([FromBody] LoginRequestDto req)
        {
            _logger.LogInformation("[UserController] Login: {U}", req.Username);
            return Ok(await _auth.LoginAsync(req));
        }

        /// <summary>
        /// UC19: Google OAuth2 Sign-In.
        /// Frontend sends the Google ID Token after the user signs in with Google.
        /// Backend validates it, finds or auto-creates the user, returns our JWT.
        /// </summary>
        [HttpPost("google-login")]
        [AllowAnonymous]
        [ProducesResponseType(typeof(AuthResponseDto), 200)]
        [ProducesResponseType(typeof(ErrorResponseDto), 401)]
        public async Task<IActionResult> GoogleLogin([FromBody] GoogleAuthRequestDto req)
        {
            _logger.LogInformation("[UserController] Google login attempt");
            return Ok(await _googleAuth.GoogleLoginAsync(req));
        }

        /// <summary>Refresh expired JWT using a valid refresh token. Rotates the refresh token.</summary>
        [HttpPost("refresh")]
        [AllowAnonymous]
        [ProducesResponseType(typeof(AuthResponseDto), 200)]
        [ProducesResponseType(typeof(ErrorResponseDto), 401)]
        public async Task<IActionResult> Refresh([FromBody] RefreshTokenRequestDto req)
            => Ok(await _auth.RefreshTokenAsync(req));

        /// <summary>Get current user's profile from JWT claims. Requires Bearer token.</summary>
        [HttpGet("profile")]
        [Authorize]
        [ProducesResponseType(typeof(UserProfileDto), 200)]
        public IActionResult Profile() => Ok(new UserProfileDto
        {
            Id       = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "0"),
            Username = User.FindFirstValue(ClaimTypes.Name)  ?? "",
            Email    = User.FindFirstValue(ClaimTypes.Email) ?? "",
            Role     = User.FindFirstValue(ClaimTypes.Role)  ?? ""
        });
    }
}
