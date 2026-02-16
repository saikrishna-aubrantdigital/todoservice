using Aubrant.TodoService.Data;
using Aubrant.TodoService.DTOs;
using Aubrant.TodoService.Models;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// ──────────────────────────────────────────────
// Service Registration
// ──────────────────────────────────────────────
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new()
    {
        Title = "Aubrant Todo Service API",
        Version = "v1",
        Description = "A simple Todo CRUD API built with .NET 9 Minimal APIs. " +
                      "Demonstrates Claude Code integration for AI-assisted development."
    });
});

// Use SQLite for simplicity — no external database setup required
builder.Services.AddDbContext<TodoDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")
        ?? "Data Source=todo.db"));

var app = builder.Build();

// ──────────────────────────────────────────────
// Database Initialization
// ──────────────────────────────────────────────
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<TodoDbContext>();
    db.Database.EnsureCreated();
}

// ──────────────────────────────────────────────
// Middleware Pipeline
// ──────────────────────────────────────────────
app.UseSwagger();
app.UseSwaggerUI(options =>
{
    options.SwaggerEndpoint("/swagger/v1/swagger.json", "Aubrant Todo Service v1");
    options.RoutePrefix = string.Empty; // Serve Swagger at root URL
});

// ──────────────────────────────────────────────
// Health Check
// ──────────────────────────────────────────────
app.MapGet("/health", () => Results.Ok(new { status = "healthy", timestamp = DateTime.UtcNow }))
    .WithName("HealthCheck")
    .WithTags("System")
    .WithDescription("Returns the health status of the API.");

// ──────────────────────────────────────────────
// GET /api/todos — List all todos (with optional filters)
// ──────────────────────────────────────────────
app.MapGet("/api/todos", async (
    TodoDbContext db,
    string? search,
    string? category,
    string? priority,
    bool? isCompleted) =>
{
    var query = db.TodoItems.AsQueryable();

    if (!string.IsNullOrWhiteSpace(search))
        query = query.Where(t => t.Title.Contains(search) || (t.Description != null && t.Description.Contains(search)));

    if (!string.IsNullOrWhiteSpace(category))
        query = query.Where(t => t.Category == category);

    if (!string.IsNullOrWhiteSpace(priority) && Enum.TryParse<Priority>(priority, true, out var p))
        query = query.Where(t => t.Priority == p);

    if (isCompleted.HasValue)
        query = query.Where(t => t.IsCompleted == isCompleted.Value);

    var items = await query.OrderByDescending(t => t.Priority).ThenBy(t => t.DueDate).ToListAsync();
    return Results.Ok(items.Select(TodoResponse.FromEntity));
})
.WithName("GetTodos")
.WithTags("Todos")
.WithDescription("Retrieve all todo items. Supports filtering by search text, category, priority, and completion status.");

// ──────────────────────────────────────────────
// GET /api/todos/{id} — Get a single todo by ID
// ──────────────────────────────────────────────
app.MapGet("/api/todos/{id:int}", async (TodoDbContext db, int id) =>
{
    var item = await db.TodoItems.FindAsync(id);
    return item is not null
        ? Results.Ok(TodoResponse.FromEntity(item))
        : Results.NotFound(new { error = $"Todo item with ID {id} not found." });
})
.WithName("GetTodoById")
.WithTags("Todos")
.WithDescription("Retrieve a specific todo item by its ID.");

// ──────────────────────────────────────────────
// POST /api/todos — Create a new todo
// ──────────────────────────────────────────────
app.MapPost("/api/todos", async (TodoDbContext db, CreateTodoRequest request) =>
{
    var item = new TodoItem
    {
        Title = request.Title,
        Description = request.Description,
        Priority = request.Priority,
        Category = request.Category,
        DueDate = request.DueDate,
        CreatedAt = DateTime.UtcNow
    };

    db.TodoItems.Add(item);
    await db.SaveChangesAsync();

    return Results.Created($"/api/todos/{item.Id}", TodoResponse.FromEntity(item));
})
.WithName("CreateTodo")
.WithTags("Todos")
.WithDescription("Create a new todo item.");

// ──────────────────────────────────────────────
// PUT /api/todos/{id} — Update an existing todo
// ──────────────────────────────────────────────
app.MapPut("/api/todos/{id:int}", async (TodoDbContext db, int id, UpdateTodoRequest request) =>
{
    var item = await db.TodoItems.FindAsync(id);
    if (item is null)
        return Results.NotFound(new { error = $"Todo item with ID {id} not found." });

    if (request.Title is not null) item.Title = request.Title;
    if (request.Description is not null) item.Description = request.Description;
    if (request.IsCompleted.HasValue) item.IsCompleted = request.IsCompleted.Value;
    if (request.Priority.HasValue) item.Priority = request.Priority.Value;
    if (request.Category is not null) item.Category = request.Category;
    if (request.DueDate.HasValue) item.DueDate = request.DueDate.Value;

    item.UpdatedAt = DateTime.UtcNow;
    await db.SaveChangesAsync();

    return Results.Ok(TodoResponse.FromEntity(item));
})
.WithName("UpdateTodo")
.WithTags("Todos")
.WithDescription("Update an existing todo item. Only provided fields will be updated.");

// ──────────────────────────────────────────────
// DELETE /api/todos/{id} — Delete a todo
// ──────────────────────────────────────────────
app.MapDelete("/api/todos/{id:int}", async (TodoDbContext db, int id) =>
{
    var item = await db.TodoItems.FindAsync(id);
    if (item is null)
        return Results.NotFound(new { error = $"Todo item with ID {id} not found." });

    db.TodoItems.Remove(item);
    await db.SaveChangesAsync();

    return Results.Ok(new { message = $"Todo item '{item.Title}' deleted successfully." });
})
.WithName("DeleteTodo")
.WithTags("Todos")
.WithDescription("Delete a todo item by its ID.");

// ──────────────────────────────────────────────
// PATCH /api/todos/{id}/complete — Mark a todo as completed
// ──────────────────────────────────────────────
app.MapPatch("/api/todos/{id:int}/complete", async (TodoDbContext db, int id) =>
{
    var item = await db.TodoItems.FindAsync(id);
    if (item is null)
        return Results.NotFound(new { error = $"Todo item with ID {id} not found." });

    item.IsCompleted = true;
    item.UpdatedAt = DateTime.UtcNow;
    await db.SaveChangesAsync();

    return Results.Ok(TodoResponse.FromEntity(item));
})
.WithName("CompleteTodo")
.WithTags("Todos")
.WithDescription("Mark a specific todo item as completed.");

// ──────────────────────────────────────────────
// GET /api/todos/summary — Get summary statistics
// ──────────────────────────────────────────────
app.MapGet("/api/todos/summary", async (TodoDbContext db) =>
{
    var items = await db.TodoItems.ToListAsync();
    var now = DateTime.UtcNow;

    var summary = new TodoSummary
    {
        TotalItems = items.Count,
        CompletedItems = items.Count(i => i.IsCompleted),
        PendingItems = items.Count(i => !i.IsCompleted),
        OverdueItems = items.Count(i => !i.IsCompleted && i.DueDate.HasValue && i.DueDate < now),
        ByPriority = items.GroupBy(i => i.Priority.ToString())
                          .ToDictionary(g => g.Key, g => g.Count()),
        ByCategory = items.Where(i => i.Category != null)
                          .GroupBy(i => i.Category!)
                          .ToDictionary(g => g.Key, g => g.Count())
    };

    return Results.Ok(summary);
})
.WithName("GetTodoSummary")
.WithTags("Analytics")
.WithDescription("Get summary statistics including counts by priority, category, and completion status.");

// ──────────────────────────────────────────────
// Run the application
// ──────────────────────────────────────────────
app.Run();

// Make Program accessible for integration tests
public partial class Program { }
