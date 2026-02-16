using Aubrant.TodoService.Models;
using Microsoft.EntityFrameworkCore;

namespace Aubrant.TodoService.Data;

/// <summary>
/// Entity Framework Core database context for the Todo Service.
/// Uses SQLite for simplicity and zero-configuration setup.
/// </summary>
public class TodoDbContext : DbContext
{
    public TodoDbContext(DbContextOptions<TodoDbContext> options) : base(options)
    {
    }

    public DbSet<TodoItem> TodoItems => Set<TodoItem>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<TodoItem>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Title).IsRequired().HasMaxLength(200);
            entity.Property(e => e.Description).HasMaxLength(1000);
            entity.Property(e => e.Category).HasMaxLength(50);
            entity.Property(e => e.Priority).HasConversion<string>();
        });

        // Seed some demo data
        modelBuilder.Entity<TodoItem>().HasData(
            new TodoItem
            {
                Id = 1,
                Title = "Set up development environment",
                Description = "Install .NET SDK, VS Code, and required extensions",
                IsCompleted = true,
                Priority = Priority.High,
                Category = "DevOps",
                CreatedAt = new DateTime(2026, 1, 15, 10, 0, 0, DateTimeKind.Utc)
            },
            new TodoItem
            {
                Id = 2,
                Title = "Configure CI/CD pipeline",
                Description = "Set up GitHub Actions for automated builds and deployments",
                IsCompleted = false,
                Priority = Priority.High,
                Category = "DevOps",
                DueDate = new DateTime(2026, 2, 20, 0, 0, 0, DateTimeKind.Utc),
                CreatedAt = new DateTime(2026, 1, 20, 14, 30, 0, DateTimeKind.Utc)
            },
            new TodoItem
            {
                Id = 3,
                Title = "Write API documentation",
                Description = "Document all endpoints using Swagger/OpenAPI annotations",
                IsCompleted = false,
                Priority = Priority.Medium,
                Category = "Documentation",
                DueDate = new DateTime(2026, 2, 28, 0, 0, 0, DateTimeKind.Utc),
                CreatedAt = new DateTime(2026, 2, 1, 9, 0, 0, DateTimeKind.Utc)
            },
            new TodoItem
            {
                Id = 4,
                Title = "Review security best practices",
                Description = "Ensure all endpoints follow OWASP guidelines",
                IsCompleted = false,
                Priority = Priority.High,
                Category = "Security",
                DueDate = new DateTime(2026, 3, 1, 0, 0, 0, DateTimeKind.Utc),
                CreatedAt = new DateTime(2026, 2, 5, 11, 15, 0, DateTimeKind.Utc)
            },
            new TodoItem
            {
                Id = 5,
                Title = "Add unit tests for Todo endpoints",
                Description = "Achieve at least 80% code coverage on service layer",
                IsCompleted = false,
                Priority = Priority.Medium,
                Category = "Testing",
                DueDate = new DateTime(2026, 2, 25, 0, 0, 0, DateTimeKind.Utc),
                CreatedAt = new DateTime(2026, 2, 8, 16, 0, 0, DateTimeKind.Utc)
            }
        );
    }
}
