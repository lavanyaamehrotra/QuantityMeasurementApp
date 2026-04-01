using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Google.Apis.Auth;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using QuantityMeasurementBusinessLayer.Interface;
using QuantityMeasurementModel.Dto;
using QuantityMeasurementModel.Entities;
using QuantityMeasurementRepository.Interface;

namespace QuantityMeasurementBusinessLayer.Service
{
    /// <summary>
    /// UC19: Google OAuth2 Authentication Service.
    ///
    /// FLOW:
    ///   1. Frontend shows Google Sign-In button → user clicks → Google issues an ID Token (JWT).
    ///   2. Frontend POSTs that ID Token to  POST /api/v1/users/google-login.
    ///   3. This service validates the token cryptographically using Google's public keys.
    ///   4. Find-or-create the user in SQL Server (first time = auto-register).
    ///   5. Issue our own JWT + refresh token — identical to normal login from this point on.
    /// </summary>
    public class GoogleAuthService : IGoogleAuthService
    {
        private readonly IUserRepository           _userRepo;
        private readonly IConfiguration            _config;
        private readonly ILogger<GoogleAuthService> _logger;

        public GoogleAuthService(
            IUserRepository userRepo,
            IConfiguration config,
            ILogger<GoogleAuthService> logger)
        {
            _userRepo = userRepo;
            _config   = config;
            _logger   = logger;
        }

        public async Task<AuthResponseDto> GoogleLoginAsync(GoogleAuthRequestDto request)
        {
            // Step 1: Validate with Google — throws UnauthorizedAccessException on failure
            var payload = await ValidateGoogleTokenAsync(request.IdToken);
            _logger.LogInformation("[GoogleAuthService] Google token valid for: {Email}", payload.Email);

            // Step 2: Find existing user OR auto-create (social login pattern)
            var user = await _userRepo.GetByEmailAsync(payload.Email)
                       ?? await CreateGoogleUserAsync(payload);

            // Step 3: Issue our own JWT — same as normal login
            return await IssueTokensAsync(user, "Google login successful.");
        }

        // ── Private helpers ───────────────────────────────────────────────

        private async Task<GoogleJsonWebSignature.Payload> ValidateGoogleTokenAsync(string idToken)
        {
            string clientId = _config["GoogleAuth:ClientId"]
                ?? throw new InvalidOperationException(
                    "GoogleAuth:ClientId missing. Add it to appsettings.json.");

            try
            {
                return await GoogleJsonWebSignature.ValidateAsync(
                    idToken,
                    new GoogleJsonWebSignature.ValidationSettings { Audience = new[] { clientId } });
            }
            catch (InvalidJwtException ex)
            {
                _logger.LogWarning("[GoogleAuthService] Invalid Google token: {Msg}", ex.Message);
                throw new UnauthorizedAccessException("Google token validation failed: " + ex.Message);
            }
        }

        private async Task<UserEntity> CreateGoogleUserAsync(GoogleJsonWebSignature.Payload payload)
        {
            // Build username from email prefix, e.g. "john.doe@gmail.com" → "john.doe_google"
            string baseUsername = payload.Email.Split('@')[0];
            string username     = baseUsername + "_google";

            if (await _userRepo.UsernameExistsAsync(username))
                username = baseUsername + "_g" + RandomNumberGenerator.GetInt32(1000, 9999);

            var user = new UserEntity
            {
                Username     = username,
                Email        = payload.Email,
                // Google users have no password — sentinel value satisfies NOT NULL constraint
                PasswordHash = "GOOGLE_AUTH_" + Guid.NewGuid().ToString("N"),
                Role         = "User",
                CreatedAt    = DateTime.UtcNow,
                IsActive     = true
            };

            var saved = await _userRepo.CreateUserAsync(user);
            _logger.LogInformation("[GoogleAuthService] Auto-created Google user Id={Id}", saved.Id);
            return saved;
        }

        private async Task<AuthResponseDto> IssueTokensAsync(UserEntity user, string message)
        {
            string accessToken  = GenerateJwt(user);
            string refreshToken = Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));
            var    expiry       = DateTime.UtcNow.AddDays(7);

            await _userRepo.UpdateRefreshTokenAsync(user.Id, refreshToken, expiry);

            return new AuthResponseDto
            {
                AccessToken  = accessToken,
                RefreshToken = refreshToken,
                ExpiresAt    = DateTime.UtcNow.AddMinutes(JwtExpireMinutes()),
                Username     = user.Username,
                Role         = user.Role,
                Message      = message
            };
        }

        private string GenerateJwt(UserEntity user)
        {
            string secret   = _config["JwtSettings:Secret"]   ?? throw new InvalidOperationException("JwtSettings:Secret missing.");
            string issuer   = _config["JwtSettings:Issuer"]   ?? "QuantityMeasurementAPI";
            string audience = _config["JwtSettings:Audience"] ?? "QuantityMeasurementAPI";

            var key   = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer:   issuer,
                audience: audience,
                claims: new Claim[]
                {
                    new(ClaimTypes.NameIdentifier, user.Id.ToString()),
                    new(ClaimTypes.Name,           user.Username),
                    new(ClaimTypes.Email,          user.Email),
                    new(ClaimTypes.Role,           user.Role),
                    new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
                },
                expires:            DateTime.UtcNow.AddMinutes(JwtExpireMinutes()),
                signingCredentials: creds);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        private int JwtExpireMinutes()
            => int.TryParse(_config["JwtSettings:ExpirationMinutes"], out int m) ? m : 60;
    }
}
