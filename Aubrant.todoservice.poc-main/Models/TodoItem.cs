namespace Aubrant.TodoService.Models;

/// <summary>
/// Represents a single todo item in the system.
/// </summary>
public class TodoItem
{
    public int Id { get; set; }

    /// <summary>The title of the todo item.</summary>
    public required string Title { get; set; }

    /// <summary>Optional detailed description.</summary>
    public string? Description { get; set; }

    /// <summary>Whether this todo item has been completed.</summary>
    public bool IsCompleted { get; set; } = false;

    /// <summary>Priority level: Low, Medium, High.</summary>
    public Priority Priority { get; set; } = Priority.Medium;

    /// <summary>Optional category for grouping (e.g., "Work", "Personal").</summary>
    public string? Category { get; set; }

    /// <summary>Optional due date for the todo item.</summary>
    public DateTime? DueDate { get; set; }

    /// <summary>When the todo item was created (UTC).</summary>
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    /// <summary>When the todo item was last updated (UTC).</summary>
    public DateTime? UpdatedAt { get; set; }
}

/// <summary>
/// Priority levels for todo items.
/// </summary>
public enum Priority
{
    Low = 0,
    Medium = 1,
    High = 2
}
