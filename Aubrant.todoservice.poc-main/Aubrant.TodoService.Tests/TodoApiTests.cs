using System.Data.Common;
using System.Net;
using System.Net.Http.Json;
using Aubrant.TodoService.Data;
using Aubrant.TodoService.DTOs;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Aubrant.TodoService.Tests;

/// <summary>
/// Integration tests for the Todo API endpoints.
/// Uses WebApplicationFactory with shared in-memory SQLite connection for test isolation.
/// </summary>
public class TodoApiTests : IClassFixture<WebApplicationFactory<Program>>, IDisposable
{
    private readonly HttpClient _client;
    private readonly WebApplicationFactory<Program> _factory;
    private readonly DbConnection _connection;

    public TodoApiTests(WebApplicationFactory<Program> factory)
    {
        // Keep a shared connection open for the lifetime of the test
        _connection = new SqliteConnection("DataSource=:memory:");
        _connection.Open();

        _factory = factory.WithWebHostBuilder(builder =>
        {
            builder.ConfigureServices(services =>
            {
                // Remove the existing DbContext registration
                var descriptor = services.SingleOrDefault(
                    d => d.ServiceType == typeof(DbContextOptions<TodoDbContext>));
                if (descriptor != null) services.Remove(descriptor);

                // Use shared in-memory SQLite connection for tests
                services.AddDbContext<TodoDbContext>(options =>
                    options.UseSqlite(_connection));
            });
        });

        // Ensure database is created with seed data using the shared connection
        using var scope = _factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<TodoDbContext>();
        db.Database.EnsureCreated();

        _client = _factory.CreateClient();
    }

    public void Dispose()
    {
        _client.Dispose();
        _connection.Close();
        _connection.Dispose();
    }

    // ──────────────────────────────────────────────
    // Health Check
    // ──────────────────────────────────────────────

    [Fact]
    public async Task HealthCheck_ReturnsHealthy()
    {
        var response = await _client.GetAsync("/health");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    // ──────────────────────────────────────────────
    // GET /api/todos
    // ──────────────────────────────────────────────

    [Fact]
    public async Task GetTodos_ReturnsSeededItems()
    {
        var response = await _client.GetAsync("/api/todos");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var todos = await response.Content.ReadFromJsonAsync<List<TodoResponse>>();
        todos.Should().NotBeNull();
        todos!.Count.Should().BeGreaterThanOrEqualTo(5);
    }

    [Fact]
    public async Task GetTodos_FilterByCategory_ReturnsFilteredResults()
    {
        var response = await _client.GetAsync("/api/todos?category=DevOps");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var todos = await response.Content.ReadFromJsonAsync<List<TodoResponse>>();
        todos.Should().NotBeNull();
        todos!.Should().AllSatisfy(t => t.Category.Should().Be("DevOps"));
    }

    [Fact]
    public async Task GetTodos_FilterByPriority_ReturnsFilteredResults()
    {
        var response = await _client.GetAsync("/api/todos?priority=High");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var todos = await response.Content.ReadFromJsonAsync<List<TodoResponse>>();
        todos.Should().NotBeNull();
        todos!.Should().AllSatisfy(t => t.Priority.Should().Be("High"));
    }

    [Fact]
    public async Task GetTodos_SearchByTitle_ReturnsMatchingResults()
    {
        var response = await _client.GetAsync("/api/todos?search=pipeline");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var todos = await response.Content.ReadFromJsonAsync<List<TodoResponse>>();
        todos.Should().NotBeNull();
        todos!.Count.Should().BeGreaterThanOrEqualTo(1);
    }

    [Fact]
    public async Task GetTodos_FilterByCompletion_ReturnsPendingOnly()
    {
        var response = await _client.GetAsync("/api/todos?isCompleted=false");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var todos = await response.Content.ReadFromJsonAsync<List<TodoResponse>>();
        todos.Should().NotBeNull();
        todos!.Should().AllSatisfy(t => t.IsCompleted.Should().BeFalse());
    }

    // ──────────────────────────────────────────────
    // GET /api/todos/{id}
    // ──────────────────────────────────────────────

    [Fact]
    public async Task GetTodoById_ValidId_ReturnsTodo()
    {
        var response = await _client.GetAsync("/api/todos/1");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var todo = await response.Content.ReadFromJsonAsync<TodoResponse>();
        todo.Should().NotBeNull();
        todo!.Id.Should().Be(1);
        todo.Title.Should().NotBeNullOrEmpty();
    }

    [Fact]
    public async Task GetTodoById_InvalidId_ReturnsNotFound()
    {
        var response = await _client.GetAsync("/api/todos/9999");

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    // ──────────────────────────────────────────────
    // POST /api/todos
    // ──────────────────────────────────────────────

    [Fact]
    public async Task CreateTodo_ValidRequest_ReturnsCreated()
    {
        var request = new CreateTodoRequest
        {
            Title = "Test new todo",
            Description = "Created by integration test",
            Priority = Models.Priority.High,
            Category = "Testing"
        };

        var response = await _client.PostAsJsonAsync("/api/todos", request);

        response.StatusCode.Should().Be(HttpStatusCode.Created);
        var todo = await response.Content.ReadFromJsonAsync<TodoResponse>();
        todo.Should().NotBeNull();
        todo!.Title.Should().Be("Test new todo");
        todo.Priority.Should().Be("High");
        todo.IsCompleted.Should().BeFalse();
    }

    // ──────────────────────────────────────────────
    // PUT /api/todos/{id}
    // ──────────────────────────────────────────────

    [Fact]
    public async Task UpdateTodo_ValidId_ReturnsUpdatedTodo()
    {
        var request = new UpdateTodoRequest
        {
            Title = "Updated title",
            IsCompleted = true
        };

        var response = await _client.PutAsJsonAsync("/api/todos/1", request);

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var todo = await response.Content.ReadFromJsonAsync<TodoResponse>();
        todo.Should().NotBeNull();
        todo!.Title.Should().Be("Updated title");
        todo.IsCompleted.Should().BeTrue();
        todo.UpdatedAt.Should().NotBeNull();
    }

    [Fact]
    public async Task UpdateTodo_InvalidId_ReturnsNotFound()
    {
        var request = new UpdateTodoRequest { Title = "Doesn't matter" };

        var response = await _client.PutAsJsonAsync("/api/todos/9999", request);

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    // ──────────────────────────────────────────────
    // DELETE /api/todos/{id}
    // ──────────────────────────────────────────────

    [Fact]
    public async Task DeleteTodo_ValidId_ReturnsOk()
    {
        // Create a todo to delete
        var createRequest = new CreateTodoRequest { Title = "To be deleted", Category = "Temp" };
        var createResponse = await _client.PostAsJsonAsync("/api/todos", createRequest);
        var created = await createResponse.Content.ReadFromJsonAsync<TodoResponse>();

        var response = await _client.DeleteAsync($"/api/todos/{created!.Id}");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task DeleteTodo_InvalidId_ReturnsNotFound()
    {
        var response = await _client.DeleteAsync("/api/todos/9999");

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    // ──────────────────────────────────────────────
    // PATCH /api/todos/{id}/complete
    // ──────────────────────────────────────────────

    [Fact]
    public async Task CompleteTodo_ValidId_MarksAsCompleted()
    {
        var response = await _client.PatchAsync("/api/todos/2/complete", null);

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var todo = await response.Content.ReadFromJsonAsync<TodoResponse>();
        todo.Should().NotBeNull();
        todo!.IsCompleted.Should().BeTrue();
    }

    [Fact]
    public async Task CompleteTodo_InvalidId_ReturnsNotFound()
    {
        var response = await _client.PatchAsync("/api/todos/9999/complete", null);

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    // ──────────────────────────────────────────────
    // GET /api/todos/summary
    // ──────────────────────────────────────────────

    [Fact]
    public async Task GetSummary_ReturnsSummaryWithCorrectCounts()
    {
        var response = await _client.GetAsync("/api/todos/summary");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var summary = await response.Content.ReadFromJsonAsync<TodoSummary>();
        summary.Should().NotBeNull();
        summary!.TotalItems.Should().BeGreaterThanOrEqualTo(5);
        summary.CompletedItems.Should().BeGreaterThanOrEqualTo(0);
        summary.PendingItems.Should().BeGreaterThanOrEqualTo(0);
        (summary.CompletedItems + summary.PendingItems).Should().Be(summary.TotalItems);
        summary.ByPriority.Should().NotBeEmpty();
        summary.ByCategory.Should().NotBeEmpty();
    }
}
