using AuthService.Models;
using AuthService.Services;
using Microsoft.AspNetCore.Mvc;

namespace AuthService.Controllers
{
    [ApiController]
    [Route("api/auth")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthBusinessService _svc;

        public AuthController(IAuthBusinessService svc) => _svc = svc;

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequestDto req)
        {
            try { return Ok(await _svc.RegisterAsync(req)); }
            catch (InvalidOperationException ex) { return Conflict(new { error = ex.Message }); }
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequestDto req)
        {
            try { return Ok(await _svc.LoginAsync(req)); }
            catch (UnauthorizedAccessException ex) { return Unauthorized(new { error = ex.Message }); }
        }

        [HttpPost("refresh")]
        public async Task<IActionResult> Refresh([FromBody] RefreshTokenRequestDto req)
        {
            try { return Ok(await _svc.RefreshTokenAsync(req)); }
            catch (UnauthorizedAccessException ex) { return Unauthorized(new { error = ex.Message }); }
        }

        [HttpPost("google")]
        public async Task<IActionResult> Google([FromBody] GoogleAuthRequestDto req)
        {
            try { return Ok(await _svc.GoogleLoginAsync(req)); }
            catch (Exception ex) { return Unauthorized(new { error = ex.Message }); }
        }

        [HttpGet("health")]
        public IActionResult Health() => Ok(new { status = "healthy", service = "auth-service" });
    }
}
