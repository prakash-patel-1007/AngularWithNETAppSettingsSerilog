using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using AngularWithNET.Features.Settings;
using Xunit;

namespace AngularWithNET.Tests
{
    public class SettingsApiTests : IClassFixture<CustomWebApplicationFactory>
    {
        private readonly HttpClient _client;

        public SettingsApiTests(CustomWebApplicationFactory factory)
        {
            _client = factory.CreateClient();
        }

        [Fact]
        public async Task GetSettings_ReturnsNonSecretValues()
        {
            var response = await _client.GetAsync("/api/settings");
            response.EnsureSuccessStatusCode();

            var settings = await response.Content.ReadFromJsonAsync<AppSettingsDto>();
            Assert.NotNull(settings);
            Assert.NotNull(settings.AppSettings1);
        }

        [Fact]
        public async Task GetSettings_DoesNotExposeSecrets()
        {
            var response = await _client.GetAsync("/api/settings");
            var content = await response.Content.ReadAsStringAsync();

            Assert.DoesNotContain("Secret", content, System.StringComparison.OrdinalIgnoreCase);
            Assert.DoesNotContain("PkoYh0yr", content);
        }

        [Fact]
        public async Task GetSettings_IncludesDemoModeFlag()
        {
            var response = await _client.GetAsync("/api/settings");
            var settings = await response.Content.ReadFromJsonAsync<AppSettingsDto>();

            Assert.True(settings.IsDemoEnvironment);
        }

        [Fact]
        public async Task GetSettings_ReturnsCorrelationIdHeader()
        {
            var response = await _client.GetAsync("/api/settings");
            Assert.True(response.Headers.Contains("X-Correlation-Id"));
        }
    }
}
