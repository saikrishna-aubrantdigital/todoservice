# Changelog

All notable changes to the Aubrant Todo Service will be documented in this file.

## [1.0.0] - 2026-02-11

### Added
- Initial Todo CRUD API with .NET 9 Minimal APIs
- Endpoints: List (with search/filter), Get by ID, Create, Update, Delete
- Mark-as-complete endpoint (`PATCH /api/todos/{id}/complete`)
- Summary analytics endpoint (`GET /api/todos/summary`)
- SQLite database with Entity Framework Core 9
- Seed data with 5 demo todo items
- Swagger UI at root URL for interactive API testing
- Health check endpoint (`GET /health`)

### Claude Code Integration
- `CLAUDE.md` project memory file with architecture, coding standards, and security rules
- `.claude/settings.json` with permission rules (allow/deny) and hooks (auto-format)
- Slash commands: `/deploy-sandbox`, `/code-review`, `/generate-tests`, `/request-review`
- GitHub Actions: `quality-gates.yml` (build, test, format, security scan)
- GitHub Actions: `claude-review.yml` (AI-powered PR review — Pro or API key)
