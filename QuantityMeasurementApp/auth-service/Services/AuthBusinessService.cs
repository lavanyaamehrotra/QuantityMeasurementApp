using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using AuthService.Data;
using AuthService.Models;
using BCrypt.Net;
using Google.Apis.Auth;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace AuthService.Services
{
    public interface IAuthBusinessService
    {
        Task<AuthResponseDto> RegisterAsync(RegisterRequestDto request);
        Task<AuthResponseDto> LoginAsync(LoginRequestDto request);
        Task<AuthResponseDto> RefreshTokenAsync(RefreshTokenRequestDto request);
        Task<AuthResponseDto> GoogleLoginAsync(GoogleAuthRequestDto request);
    }

    public class AuthBusinessService : IAuthBusinessService
    {
        private readonly AppDbContext _db;
        private readonly IConfiguration _config;
        private readonly ILogger<AuthBusinessService> _logger;

        public AuthBusinessService(AppDbContext db, IConfiguration config, ILogger<AuthBusinessService> logger)
        {
            _db = db;
            _config = config;
            _logger = logger;
        }

        public async Task<AuthResponseDto> RegisterAsync(RegisterRequestDto req)
        {
            if (await _db.Users.AnyAsync(u => u.Username == req.Username))
                throw new InvalidOperationException("Username is already taken.");
            if (await _db.Users.AnyAsync(u => u.Email == req.Email))
                throw new InvalidOperationException("Email is already registered.");

            var user = new UserEntity
            {
                Username = req.Username,
                Email = req.Email,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(req.Password, 12),
                Role = "User",
                CreatedAt = DateTime.UtcNow,
                IsActive = true
            };

            _db.Users.Add(user);
            await _db.SaveChangesAsync();
            _logger.LogInformation("Registered user {U}", user.Username);
            return await IssueTokensAsync(user, "Registration successful.");
        }

        public async Task<AuthResponseDto> LoginAsync(LoginRequestDto req)
        {
            var user = await _db.Users.FirstOrDefaultAsync(u => u.Username == req.Username);
            if (user is null || !BCrypt.Net.BCrypt.Verify(req.Password, user.PasswordHash))
            {
                _logger.LogWarning("Failed login: {U}", req.Username);
                throw new UnauthorizedAccessException("Invalid username or password.");
            }
            if (!user.IsActive) throw new UnauthorizedAccessException("Account is disabled.");
            _logger.LogInformation("Login success: UserId={Id}", user.Id);
            return await IssueTokensAsync(user, "Login successful.");
        }

        public async Task<AuthResponseDto> RefreshTokenAsync(RefreshTokenRequestDto req)
        {
            var principal = GetPrincipalFromExpiredToken(req.AccessToken);
            if (!int.TryParse(principal.FindFirstValue(ClaimTypes.NameIdentifier), out int userId))
                throw new UnauthorizedAccessException("Invalid token.");
            var user = await _db.Users.FindAsync(userId)
                ?? throw new UnauthorizedAccessException("User not found.");
            if (user.RefreshToken != req.RefreshToken || user.RefreshTokenExpiry <= DateTime.UtcNow)
                throw new UnauthorizedAccessException("Refresh token is invalid or expired.");
            return await IssueTokensAsync(user, "Token refreshed.");
        }

        public async Task<AuthResponseDto> GoogleLoginAsync(GoogleAuthRequestDto req)
        {
            var clientId = _config["Google:ClientId"];
            if (string.IsNullOrEmpty(clientId))
                throw new InvalidOperationException("Google ClientId not configured.");

            GoogleJsonWebSignature.Payload payload;
            try
            {
                payload = await GoogleJsonWebSignature.ValidateAsync(req.IdToken,
                    new GoogleJsonWebSignature.ValidationSettings { Audience = new[] { clientId } });
            }
            catch
            {
                throw new UnauthorizedAccessException("Invalid Google token.");
            }

            var user = await _db.Users.FirstOrDefaultAsync(u => u.GoogleId == payload.Subject);
            if (user is null)
            {
                user = new UserEntity
                {
                    Username = payload.Email.Split('@')[0],
                    Email = payload.Email,
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword(Guid.NewGuid().ToString()),
                    Role = "User",
                    IsActive = true,
                    IsGoogleUser = true,
                    GoogleId = payload.Subject,
                    CreatedAt = DateTime.UtcNow
                };
                _db.Users.Add(user);
                await _db.SaveChangesAsync();
            }
            return await IssueTokensAsync(user, "Google login successful.");
        }

        // ── Helpers ──────────────────────────────────────────────────────

        private async Task<AuthResponseDto> IssueTokensAsync(UserEntity user, string message)
        {
            string accessToken = GenerateJwt(user);
            string refreshToken = Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));
            var expiry = DateTime.UtcNow.AddDays(7);

            user.RefreshToken = refreshToken;
            user.RefreshTokenExpiry = expiry;
            await _db.SaveChangesAsync();

            return new AuthResponseDto
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken,
                ExpiresAt = DateTime.UtcNow.AddMinutes(JwtExpireMinutes()),
                Username = user.Username,
                Role = user.Role,
                Message = message
            };
        }

        private string GenerateJwt(UserEntity user)
        {
            var secret = _config["JwtSettings:Secret"]
                ?? throw new InvalidOperationException("JwtSettings:Secret not configured.");
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _config["JwtSettings:Issuer"] ?? "QuantityMeasurementAPI",
                audience: _config["JwtSettings:Audience"] ?? "QuantityMeasurementAPI",
                claims: new[]
                {
                    new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                    new Claim(ClaimTypes.Name, user.Username),
                    new Claim(ClaimTypes.Email, user.Email),
                    new Claim(ClaimTypes.Role, user.Role),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
                },
                expires: DateTime.UtcNow.AddMinutes(JwtExpireMinutes()),
                signingCredentials: creds);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        private ClaimsPrincipal GetPrincipalFromExpiredToken(string token)
        {
            var secret = _config["JwtSettings:Secret"]!;
            var p = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret)),
                ValidateIssuer = false,
                ValidateAudience = false,
                ValidateLifetime = false
            };
            return new JwtSecurityTokenHandler().ValidateToken(token, p, out _);
        }

        private int JwtExpireMinutes()
            => int.TryParse(_config["JwtSettings:ExpirationMinutes"], out int m) ? m : 60;
    }
}
