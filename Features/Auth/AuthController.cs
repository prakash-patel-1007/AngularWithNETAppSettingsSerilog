using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using AngularWithNET.Data;
using AngularWithNET.Domain;
using AngularWithNET.Features.Auth.Services;
using AngularWithNET.ViewModel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace AngularWithNET.Features.Auth
{
    [ApiController]
    [Route("api/auth")]
    [EnableRateLimiting("auth")]
    public class AuthController : ControllerBase
    {
        private readonly AppDbContext _db;
        private readonly PasswordService _passwordService;
        private readonly AppSettings _appSettings;
        private readonly ILogger<AuthController> _logger;

        public AuthController(
            AppDbContext db,
            PasswordService passwordService,
            IOptions<AppSettings> appSettings,
            ILogger<AuthController> logger)
        {
            _db = db;
            _passwordService = passwordService;
            _appSettings = appSettings.Value;
            _logger = logger;
        }

        [HttpPost("login")]
        public IActionResult Login([FromBody] LoginRequest request)
        {
            if (request == null || string.IsNullOrWhiteSpace(request.Username) || string.IsNullOrWhiteSpace(request.Password))
                return BadRequest(new ProblemDetails { Title = "Invalid request", Detail = "Username and password are required." });

            var user = _db.Users.FirstOrDefault(u => u.Username == request.Username);
            if (user == null || !_passwordService.VerifyPassword(user.PasswordHash, request.Password))
            {
                _logger.LogWarning("Login failed for user: {Username}", request.Username);
                return Unauthorized(new ProblemDetails { Title = "Authentication failed", Detail = "Invalid username or password." });
            }

            var accessToken = GenerateJwtToken(user);
            var refreshToken = GenerateRefreshToken(user.Id);

            _logger.LogInformation("Login successful for user: {Username}", user.Username);

            return Ok(new LoginResponse
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken,
                Username = user.Username,
                Permissions = GetPermissions(user.Username)
            });
        }

        [HttpPost("logout")]
        [Microsoft.AspNetCore.Authorization.Authorize]
        public IActionResult Logout()
        {
            var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (userIdClaim == null || !int.TryParse(userIdClaim, out var userId))
                return Unauthorized();

            var activeTokens = _db.RefreshTokens
                .Where(r => r.UserId == userId && r.RevokedAt == null)
                .ToList();

            foreach (var token in activeTokens)
                token.RevokedAt = DateTime.UtcNow;

            _db.SaveChanges();

            _logger.LogInformation("User logged out, {Count} refresh tokens revoked for userId: {UserId}", activeTokens.Count, userId);

            return NoContent();
        }

        [HttpPost("refresh")]
        public IActionResult Refresh([FromBody] RefreshRequest request)
        {
            if (request == null || string.IsNullOrWhiteSpace(request.RefreshToken))
                return BadRequest(new ProblemDetails { Title = "Invalid request", Detail = "Refresh token is required." });

            var tokenHash = HashToken(request.RefreshToken);
            var storedToken = _db.RefreshTokens.FirstOrDefault(r => r.TokenHash == tokenHash);

            if (storedToken == null || !storedToken.IsActive)
            {
                _logger.LogWarning("Refresh token validation failed");
                return Unauthorized(new ProblemDetails { Title = "Invalid token", Detail = "Refresh token is invalid or expired." });
            }

            storedToken.RevokedAt = DateTime.UtcNow;

            var user = _db.Users.Find(storedToken.UserId);
            if (user == null)
                return Unauthorized(new ProblemDetails { Title = "User not found" });

            var newAccessToken = GenerateJwtToken(user);
            var newRefreshToken = GenerateRefreshToken(user.Id);

            var newStoredToken = _db.RefreshTokens
                .OrderByDescending(r => r.Id)
                .First(r => r.UserId == user.Id && r.RevokedAt == null);
            storedToken.ReplacedByTokenId = newStoredToken.Id;

            _db.SaveChanges();

            _logger.LogInformation("Token refreshed for user: {Username}", user.Username);

            return Ok(new LoginResponse
            {
                AccessToken = newAccessToken,
                RefreshToken = newRefreshToken,
                Username = user.Username,
                Permissions = GetPermissions(user.Username)
            });
        }

        private string GenerateJwtToken(User user)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_appSettings.Secret);
            var expiryMinutes = _appSettings.TokenExpiry > 0 ? _appSettings.TokenExpiry : 15;
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                    new Claim(ClaimTypes.Name, user.Username),
                }),
                Expires = DateTime.UtcNow.AddMinutes(expiryMinutes),
                SigningCredentials = new SigningCredentials(
                    new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

        private string GenerateRefreshToken(int userId)
        {
            var rawToken = Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));
            var tokenHash = HashToken(rawToken);

            _db.RefreshTokens.Add(new RefreshToken
            {
                TokenHash = tokenHash,
                UserId = userId,
                ExpiresAt = DateTime.UtcNow.AddDays(7),
                CreatedAt = DateTime.UtcNow
            });
            _db.SaveChanges();

            return rawToken;
        }

        private static string HashToken(string token)
        {
            using var sha = SHA256.Create();
            var bytes = sha.ComputeHash(Encoding.UTF8.GetBytes(token));
            return Convert.ToBase64String(bytes);
        }

        private string[] GetPermissions(string username)
        {
            var permissions = new List<string> { "ViewCounter" };
            if (string.Equals(username, "admin", StringComparison.OrdinalIgnoreCase))
            {
                permissions.Add("ViewForecast");
                permissions.Add("ViewAppSettings");
            }
            return permissions.ToArray();
        }
    }

    public class LoginRequest
    {
        public string Username { get; set; }
        public string Password { get; set; }
    }

    public class RefreshRequest
    {
        public string RefreshToken { get; set; }
    }

    public class LoginResponse
    {
        public string AccessToken { get; set; }
        public string RefreshToken { get; set; }
        public string Username { get; set; }
        public string[] Permissions { get; set; }
    }
}
