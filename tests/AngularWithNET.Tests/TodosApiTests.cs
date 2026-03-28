using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Threading.Tasks;
using AngularWithNET.Features.Auth;
using AngularWithNET.Features.Todos;
using Xunit;

namespace AngularWithNET.Tests
{
    public class TodosApiTests : IClassFixture<CustomWebApplicationFactory>
    {
        private readonly HttpClient _client;

        public TodosApiTests(CustomWebApplicationFactory factory)
        {
            _client = factory.CreateClient();
        }

        [Fact]
        public async Task GetAll_WithoutAuth_Returns401()
        {
            var response = await _client.GetAsync("/api/todos");
            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }

        [Fact]
        public async Task GetAll_Authenticated_ReturnsEmptyList()
        {
            var token = await LoginAsync("admin", "admin");
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var response = await _client.GetAsync("/api/todos");
            response.EnsureSuccessStatusCode();

            var todos = await response.Content.ReadFromJsonAsync<List<TodoDto>>();
            Assert.NotNull(todos);
        }

        [Fact]
        public async Task CreateAndGetTodo_WorksCorrectly()
        {
            var token = await LoginAsync("admin", "admin");
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var createResp = await _client.PostAsJsonAsync("/api/todos", new { title = "Test Todo" });
            Assert.Equal(HttpStatusCode.Created, createResp.StatusCode);

            var created = await createResp.Content.ReadFromJsonAsync<TodoDto>();
            Assert.Equal("Test Todo", created.Title);
            Assert.False(created.IsCompleted);
            Assert.Null(created.CompletedAt);
            Assert.True(created.Id > 0);

            var getResp = await _client.GetAsync($"/api/todos/{created.Id}");
            getResp.EnsureSuccessStatusCode();
            var fetched = await getResp.Content.ReadFromJsonAsync<TodoDto>();
            Assert.Equal(created.Id, fetched.Id);
        }

        [Fact]
        public async Task Create_WithBlankTitle_Returns400()
        {
            var token = await LoginAsync("admin", "admin");
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var resp = await _client.PostAsJsonAsync("/api/todos", new { title = "  " });
            Assert.Equal(HttpStatusCode.BadRequest, resp.StatusCode);
        }

        [Fact]
        public async Task CompleteTodo_SetsCompletedAt()
        {
            var token = await LoginAsync("admin", "admin");
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var createResp = await _client.PostAsJsonAsync("/api/todos", new { title = "Complete me" });
            var created = await createResp.Content.ReadFromJsonAsync<TodoDto>();

            var updateResp = await _client.PutAsJsonAsync($"/api/todos/{created.Id}",
                new { isCompleted = true });
            updateResp.EnsureSuccessStatusCode();

            var updated = await updateResp.Content.ReadFromJsonAsync<TodoDto>();
            Assert.True(updated.IsCompleted);
            Assert.NotNull(updated.CompletedAt);
        }

        [Fact]
        public async Task UncompleteTodo_ClearsCompletedAt()
        {
            var token = await LoginAsync("admin", "admin");
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var createResp = await _client.PostAsJsonAsync("/api/todos", new { title = "Toggle me" });
            var created = await createResp.Content.ReadFromJsonAsync<TodoDto>();

            await _client.PutAsJsonAsync($"/api/todos/{created.Id}", new { isCompleted = true });
            var uncheckResp = await _client.PutAsJsonAsync($"/api/todos/{created.Id}", new { isCompleted = false });
            var unchecked_ = await uncheckResp.Content.ReadFromJsonAsync<TodoDto>();

            Assert.False(unchecked_.IsCompleted);
            Assert.Null(unchecked_.CompletedAt);
        }

        [Fact]
        public async Task DeleteTodo_RemovesFromList()
        {
            var token = await LoginAsync("admin", "admin");
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var createResp = await _client.PostAsJsonAsync("/api/todos", new { title = "Delete me" });
            var created = await createResp.Content.ReadFromJsonAsync<TodoDto>();

            var deleteResp = await _client.DeleteAsync($"/api/todos/{created.Id}");
            Assert.Equal(HttpStatusCode.NoContent, deleteResp.StatusCode);

            var getResp = await _client.GetAsync($"/api/todos/{created.Id}");
            Assert.Equal(HttpStatusCode.NotFound, getResp.StatusCode);
        }

        [Fact]
        public async Task UserCannotAccessOtherUsersTodos()
        {
            var adminToken = await LoginAsync("admin", "admin");
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", adminToken);

            var createResp = await _client.PostAsJsonAsync("/api/todos", new { title = "Admin only" });
            var created = await createResp.Content.ReadFromJsonAsync<TodoDto>();

            _client.DefaultRequestHeaders.Clear();
            var userToken = await LoginAsync("user", "user");
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", userToken);

            var getResp = await _client.GetAsync($"/api/todos/{created.Id}");
            Assert.Equal(HttpStatusCode.NotFound, getResp.StatusCode);
        }

        [Fact]
        public async Task UpdateTitle_ChangesTitle()
        {
            var token = await LoginAsync("admin", "admin");
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var createResp = await _client.PostAsJsonAsync("/api/todos", new { title = "Original" });
            var created = await createResp.Content.ReadFromJsonAsync<TodoDto>();

            var updateResp = await _client.PutAsJsonAsync($"/api/todos/{created.Id}",
                new { title = "Updated" });
            var updated = await updateResp.Content.ReadFromJsonAsync<TodoDto>();

            Assert.Equal("Updated", updated.Title);
        }

        private async Task<string> LoginAsync(string username, string password)
        {
            var resp = await _client.PostAsJsonAsync("/api/auth/login",
                new { username, password });
            var login = await resp.Content.ReadFromJsonAsync<LoginResponse>();
            return login.AccessToken;
        }
    }
}
