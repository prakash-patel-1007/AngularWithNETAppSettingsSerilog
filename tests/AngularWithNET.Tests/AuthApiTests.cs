using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Threading.Tasks;
using AngularWithNET.Features.Auth;
using Xunit;

namespace AngularWithNET.Tests
{
    public class AuthApiTests : IClassFixture<CustomWebApplicationFactory>
    {
        private readonly HttpClient _client;

        public AuthApiTests(CustomWebApplicationFactory factory)
        {
            _client = factory.CreateClient();
        }

        [Fact]
        public async Task Login_WithValidCredentials_ReturnsTokens()
        {
            var response = await _client.PostAsJsonAsync("/api/auth/login",
                new { username = "admin", password = "admin" });

            response.EnsureSuccessStatusCode();
            var body = await response.Content.ReadFromJsonAsync<LoginResponse>();

            Assert.NotNull(body);
            Assert.False(string.IsNullOrEmpty(body.AccessToken));
            Assert.False(string.IsNullOrEmpty(body.RefreshToken));
            Assert.Equal("admin", body.Username);
            Assert.Contains("ViewCounter", body.Permissions);
            Assert.Contains("ViewForecast", body.Permissions);
        }

        [Fact]
        public async Task Login_WithInvalidPassword_Returns401()
        {
            var response = await _client.PostAsJsonAsync("/api/auth/login",
                new { username = "admin", password = "wrong" });

            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }

        [Fact]
        public async Task Login_WithMissingFields_Returns400()
        {
            var response = await _client.PostAsJsonAsync("/api/auth/login",
                new { username = "", password = "" });

            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Fact]
        public async Task Login_UserHasLimitedPermissions()
        {
            var response = await _client.PostAsJsonAsync("/api/auth/login",
                new { username = "user", password = "user" });

            response.EnsureSuccessStatusCode();
            var body = await response.Content.ReadFromJsonAsync<LoginResponse>();

            Assert.Contains("ViewCounter", body.Permissions);
            Assert.DoesNotContain("ViewForecast", body.Permissions);
        }

        [Fact]
        public async Task Refresh_WithValidToken_ReturnsNewTokens()
        {
            var loginResp = await _client.PostAsJsonAsync("/api/auth/login",
                new { username = "admin", password = "admin" });
            var login = await loginResp.Content.ReadFromJsonAsync<LoginResponse>();

            var refreshResp = await _client.PostAsJsonAsync("/api/auth/refresh",
                new { refreshToken = login.RefreshToken });

            refreshResp.EnsureSuccessStatusCode();
            var body = await refreshResp.Content.ReadFromJsonAsync<LoginResponse>();

            Assert.NotNull(body);
            Assert.False(string.IsNullOrEmpty(body.AccessToken));
            Assert.False(string.IsNullOrEmpty(body.RefreshToken));
            Assert.NotEqual(login.RefreshToken, body.RefreshToken);
            Assert.Equal("admin", body.Username);
        }

        [Fact]
        public async Task Refresh_WithInvalidToken_Returns401()
        {
            var response = await _client.PostAsJsonAsync("/api/auth/refresh",
                new { refreshToken = "invalid-token" });

            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }

        [Fact]
        public async Task Refresh_UsedTokenCannotBeReused()
        {
            var loginResp = await _client.PostAsJsonAsync("/api/auth/login",
                new { username = "admin", password = "admin" });
            var login = await loginResp.Content.ReadFromJsonAsync<LoginResponse>();

            await _client.PostAsJsonAsync("/api/auth/refresh",
                new { refreshToken = login.RefreshToken });

            var reuse = await _client.PostAsJsonAsync("/api/auth/refresh",
                new { refreshToken = login.RefreshToken });

            Assert.Equal(HttpStatusCode.Unauthorized, reuse.StatusCode);
        }

        [Fact]
        public async Task Logout_RevokesRefreshTokens()
        {
            var loginResp = await _client.PostAsJsonAsync("/api/auth/login",
                new { username = "admin", password = "admin" });
            var login = await loginResp.Content.ReadFromJsonAsync<LoginResponse>();

            var logoutReq = new HttpRequestMessage(HttpMethod.Post, "/api/auth/logout");
            logoutReq.Headers.Authorization = new AuthenticationHeaderValue("Bearer", login.AccessToken);
            var logoutResp = await _client.SendAsync(logoutReq);
            Assert.Equal(HttpStatusCode.NoContent, logoutResp.StatusCode);

            var refreshResp = await _client.PostAsJsonAsync("/api/auth/refresh",
                new { refreshToken = login.RefreshToken });
            Assert.Equal(HttpStatusCode.Unauthorized, refreshResp.StatusCode);
        }

        [Fact]
        public async Task Logout_WithoutToken_Returns401()
        {
            var response = await _client.PostAsync("/api/auth/logout", null);
            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }

        private async Task<string> GetAdminTokenAsync()
        {
            var resp = await _client.PostAsJsonAsync("/api/auth/login",
                new { username = "admin", password = "admin" });
            var login = await resp.Content.ReadFromJsonAsync<LoginResponse>();
            return login.AccessToken;
        }
    }
}
