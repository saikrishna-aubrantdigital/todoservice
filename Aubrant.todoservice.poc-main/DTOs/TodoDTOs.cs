using Aubrant.TodoService.Models;

namespace Aubrant.TodoService.DTOs;

/// <summary>
/// Request DTO for creating a new todo item.
/// </summary>
public class CreateTodoRequest
{
    public required string Title { get; set; }
    public string? Description { get; set; }
    public Priority Priority { get; set; } = Priority.Medium;
    public string? Category { get; set; }
    public DateTime? DueDate { get; set; }
}

/// <summary>
/// Request DTO for updating an existing todo item.
/// </summary>
public class UpdateTodoRequest
{
    public string? Title { get; set; }
    public string? Description { get; set; }
    public bool? IsCompleted { get; set; }
    public Priority? Priority { get; set; }
    public string? Category { get; set; }
    public DateTime? DueDate { get; set; }
}

/// <summary>
/// Response DTO for returning todo items to clients.
/// </summary>
public class TodoResponse
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public bool IsCompleted { get; set; }
    public string Priority { get; set; } = string.Empty;
    public string? Category { get; set; }
    public DateTime? DueDate { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }

    public static TodoResponse FromEntity(TodoItem item) => new()
    {
        Id = item.Id,
        Title = item.Title,
        Description = item.Description,
        IsCompleted = item.IsCompleted,
        Priority = item.Priority.ToString(),
        Category = item.Category,
        DueDate = item.DueDate,
        CreatedAt = item.CreatedAt,
        UpdatedAt = item.UpdatedAt
    };
}

/// <summary>
/// Summary statistics for the todo list.
/// </summary>
public class TodoSummary
{
    public int TotalItems { get; set; }
    public int CompletedItems { get; set; }
    public int PendingItems { get; set; }
    public int OverdueItems { get; set; }
    public Dictionary<string, int> ByPriority { get; set; } = new();
    public Dictionary<string, int> ByCategory { get; set; } = new();
}
