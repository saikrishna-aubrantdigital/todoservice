# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Project Location

**IMPORTANT**: The actual project is in the `Aubrant.todoservice.poc-main` subdirectory. Always `cd` into this directory before running any commands.

```bash
cd Aubrant.todoservice.poc-main
```

## Development Commands

### Build and Run
```bash
# Build the project
dotnet build

# Run the API (development mode with hot reload)
dotnet run

# Run in production mode
dotnet run --configuration Release

# The API will be available at:
# - HTTPS: https://localhost:5001
# - HTTP: http://localhost:5000
# - Swagger UI: https://localhost:5001 (root URL)
```

### Testing
```bash
# Run all tests
dotnet test

# Run tests with detailed output
dotnet test --verbosity normal

# Run a specific test
dotnet test --filter "FullyQualifiedName~TodoApiTests.GetTodoById_ReturnsNotFound_WhenTodoDoesNotExist"

# Run tests with code coverage
dotnet test --collect:"XPlat Code Coverage"
```

### Database
```bash
# The SQLite database (todo.db) is created automatically on first run
# No migrations are needed for this POC - using EnsureCreated()

# To reset the database, simply delete the file:
rm todo.db
```

### Solution Structure
```bash
# Work with the solution file when adding projects or references
dotnet sln list                    # List all projects in solution
dotnet sln add NewProject.csproj   # Add a project to solution
```

## Architecture

<!-- ARCHITECTURE_START -->
- **Framework**: .NET 9 Minimal APIs
- **Database**: SQLite via Entity Framework Core 9
- **API Style**: RESTful with Minimal API endpoints (no controllers)
- **Documentation**: Swagger/OpenAPI via Swashbuckle (served at root `/`)
- **Pattern**: Single-project structure with `Models/`, `Data/`, `DTOs/` folders

### Key Files
- `Program.cs` — All endpoint definitions and service configuration (220 lines)
- `Models/TodoItem.cs` — Entity model with Priority enum (Low, Medium, High)
- `Data/TodoDbContext.cs` — EF Core context with seed data
- `DTOs/TodoDTOs.cs` — Request/response DTOs (CreateTodoRequest, UpdateTodoRequest, TodoResponse, TodoSummary)
- `Aubrant.TodoService.Tests/TodoApiTests.cs` — Integration tests using WebApplicationFactory

### Test Architecture
- Uses `WebApplicationFactory<Program>` for integration testing
- In-memory SQLite database for test isolation (separate instance per test)
- Tests use FluentAssertions for readable assertions
- `Program` class made `public partial` to enable WebApplicationFactory access
<!-- ARCHITECTURE_END -->

## API Patterns

<!-- API_PATTERNS_START -->
- All API endpoints are prefixed with `/api/todos` (except `/health` and `/api/todos/summary`)
- Use DTOs for request/response — never expose entity models directly
- Return appropriate HTTP status codes: 200 (OK), 201 (Created), 404 (Not Found)
- Error responses use `{ "error": "message" }` format
- Use `Results.Ok()`, `Results.Created()`, `Results.NotFound()` helper methods
- Filter/search parameters are query string parameters, not path parameters
- All timestamps are UTC (`DateTime.UtcNow`)
- Always set `UpdatedAt` when modifying an existing record
- Use `.WithName()`, `.WithTags()`, `.WithDescription()` for Swagger documentation
<!-- API_PATTERNS_END -->

## Coding Standards

<!-- STANDARDS_START -->
- Use C# 12 features and .NET 9 conventions
- Use `required` keyword for mandatory properties
- Use nullable reference types (`string?`) for optional fields
- Use file-scoped namespaces
- Use XML doc comments (`/// <summary>`) on all public classes and complex methods
- Follow Microsoft naming conventions (PascalCase for public members)
- Use async/await for all database operations
- Use `var` for implicitly typed local variables
- Keep endpoint definitions clean — extract complex logic into separate methods or services
- Enum values in responses should be returned as strings (use `.ToString()` in DTOs)
<!-- STANDARDS_END -->

## Testing Requirements

<!-- TESTING_START -->
- Unit tests use xUnit framework with FluentAssertions
- Test project: `Aubrant.TodoService.Tests`
- Use `WebApplicationFactory<Program>` for integration tests
- Use in-memory SQLite database for test isolation:
  ```csharp
  options.UseSqlite("Data Source=:memory:");
  ```
- Minimum 80% code coverage target
- Every endpoint must have at least: happy path test, not-found test, validation test
- Test naming convention: `MethodName_ExpectedBehavior_WhenCondition`
- Always call `Database.EnsureCreated()` in test setup to initialize the schema
<!-- TESTING_END -->

## Security Rules

<!-- SECURITY_START -->
- **NEVER** modify `.env` files or connection strings containing production credentials
- **NEVER** commit database files (`*.db`) to the repository
- **NEVER** log sensitive data (passwords, tokens, connection strings)
- **NEVER** disable HTTPS redirection in production
- Always validate and sanitize user input
- Use parameterized queries (EF Core handles this automatically)
<!-- SECURITY_END -->

## Current Features

<!-- FEATURES_START -->
- **2026-02-11**: Initial CRUD API with search, filtering, completion toggle, and summary analytics
- 8 endpoints: health, list (with search/filter), get by ID, create, update, delete, complete, summary
- SQLite database with 5 seed records
- Swagger UI at root URL for interactive API testing
- Integration tests with xUnit and FluentAssertions
<!-- FEATURES_END -->
