# Aubrant Todo Service

A simple Todo CRUD API built with **.NET 9 Minimal APIs** — designed to demonstrate **Claude Code integration** for AI-assisted development workflows.

## 🚀 Quick Start

```bash
# Clone the repository
git clone https://github.com/Aubrant/Aubrant.todoservice.poc.git
cd Aubrant.todoservice.poc

# Run the API
dotnet run

# Open Swagger UI
# Navigate to https://localhost:5001 (or http://localhost:5000)
```

## 📋 API Endpoints

| Method | Endpoint | Description |
|--------|----------|-------------|
| `GET` | `/health` | Health check |
| `GET` | `/api/todos` | List all todos (supports search, filter by category/priority/status) |
| `GET` | `/api/todos/{id}` | Get a specific todo |
| `POST` | `/api/todos` | Create a new todo |
| `PUT` | `/api/todos/{id}` | Update a todo |
| `DELETE` | `/api/todos/{id}` | Delete a todo |
| `PATCH` | `/api/todos/{id}/complete` | Mark a todo as completed |
| `GET` | `/api/todos/summary` | Get summary statistics |

### Query Parameters for `GET /api/todos`

| Parameter | Type | Description |
|-----------|------|-------------|
| `search` | string | Search in title and description |
| `category` | string | Filter by category (e.g., "DevOps", "Testing") |
| `priority` | string | Filter by priority: Low, Medium, High |
| `isCompleted` | bool | Filter by completion status |

## 🏗️ Tech Stack

- **.NET 9** — Minimal APIs (no controllers)
- **Entity Framework Core 9** — SQLite database
- **Swashbuckle** — Swagger/OpenAPI documentation

## 🤖 Claude Code Integration

This project includes a full **Claude Code integration framework** for AI-assisted development:

### Project Memory
- [`CLAUDE.md`](CLAUDE.md) — Architecture, coding standards, API patterns, security rules

### Guardrails (`.claude/`)
- **Permissions** — Allow/deny rules for safe AI operations
- **Hooks** — Auto-format C# files after every edit

### Slash Commands (`.claude/commands/`)
| Command | Description |
|---------|-------------|
| `/deploy-sandbox` | Lint → Build → Test → Self-review → Confirm → Push |
| `/code-review` | Deep code review against project standards |
| `/generate-tests` | Generate unit/integration tests for all endpoints |
| `/request-review` | Create a PR with standardized template |

### CI/CD (`.github/workflows/`)
| Workflow | Trigger | What It Does |
|----------|---------|--------------|
| `quality-gates.yml` | PR to dev/uat/main, push to sandbox | Format check, build, test, security scan |
| `claude-review.yml` | PR to dev/uat/main | AI code review (via `@claude` or API key) |

## 📁 Project Structure

```
Aubrant.todoservice.poc/
├── .claude/
│   ├── settings.json              ← Permissions + hooks
│   └── commands/
│       ├── deploy-sandbox.md      ← /deploy-sandbox
│       ├── code-review.md         ← /code-review
│       ├── generate-tests.md      ← /generate-tests
│       └── request-review.md      ← /request-review
├── .github/workflows/
│   ├── quality-gates.yml          ← Build + test + security
│   └── claude-review.yml          ← AI-powered PR review
├── Data/
│   └── TodoDbContext.cs           ← EF Core context + seed data
├── DTOs/
│   └── TodoDTOs.cs                ← Request/response DTOs
├── Models/
│   └── TodoItem.cs                ← Entity model + Priority enum
├── CLAUDE.md                      ← Project memory for Claude Code
├── CHANGELOG.md                   ← Version history
├── Program.cs                     ← API endpoints + configuration
└── README.md                      ← This file
```

## 📄 License

This project is a proof-of-concept for internal demonstration purposes.
