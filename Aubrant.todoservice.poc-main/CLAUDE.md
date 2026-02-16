# Aubrant Todo Service

> Project memory file for Claude Code. Loaded at the start of every session.

## Architecture

<!-- ARCHITECTURE_START -->
- **Framework**: .NET 9 Minimal APIs
- **Database**: SQLite via Entity Framework Core 9
- **API Style**: RESTful with Minimal API endpoints (no controllers)
- **Documentation**: Swagger/OpenAPI via Swashbuckle (served at root `/`)
- **Pattern**: Single-project structure with `Models/`, `Data/`, `DTOs/` folders

### Key Files
- `Program.cs` — All endpoint definitions and service configuration
- `Models/TodoItem.cs` — Entity model with Priority enum
- `Data/TodoDbContext.cs` — EF Core context with seed data
- `DTOs/TodoDTOs.cs` — Request/response DTOs and TodoSummary
<!-- ARCHITECTURE_END -->

## API Patterns

<!-- API_PATTERNS_START -->
- All API endpoints are prefixed with `/api/todos`
- Use DTOs for request/response — never expose entity models directly
- Return appropriate HTTP status codes: 200 (OK), 201 (Created), 404 (Not Found)
- Error responses use `{ "error": "message" }` format
- Use `Results.Ok()`, `Results.Created()`, `Results.NotFound()` helper methods
- Filter/search parameters are query string parameters, not path parameters
- All timestamps are UTC (`DateTime.UtcNow`)
- Always set `UpdatedAt` when modifying an existing record
<!-- API_PATTERNS_END -->

## Coding Standards

<!-- STANDARDS_START -->
- Use C# 12 features and .NET 9 conventions
- Use `required` keyword for mandatory properties
- Use nullable reference types (`string?`) for optional fields
- Use file-scoped namespaces
- Use XML doc comments on all public classes and complex methods
- Follow Microsoft naming conventions (PascalCase for public members)
- Use async/await for all database operations
- Use `var` for implicitly typed local variables
- Keep endpoint definitions clean — extract complex logic into separate methods or services
<!-- STANDARDS_END -->

## Testing Requirements

<!-- TESTING_START -->
- Unit tests use xUnit framework with FluentAssertions
- Test project: `Aubrant.TodoService.Tests`
- Use `WebApplicationFactory<Program>` for integration tests
- Use in-memory SQLite database for test isolation
- Minimum 80% code coverage target
- Every endpoint must have at least: happy path test, not-found test, validation test
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
<!-- FEATURES_END -->
