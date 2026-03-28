# Angular + ASP.NET Core — Todo Sample with JWT Auth, Serilog & App Settings

Full-stack sample: **Angular 19 SPA** + **.NET 10 API** with JWT authentication, refresh-token rotation, role-based permissions, Serilog structured logging, SQLite persistence, and `appsettings.json` values surfaced to the Angular client.

> **Note on naming:** The repository folder may still carry a legacy name to preserve git history and existing links. The solution and project have been renamed to `AngularWithNET` to stay version-neutral.

---

## Table of Contents

- [Prerequisites](#prerequisites)
- [Quick Start](#quick-start)
- [Development Workflow](#development-workflow-two-process)
- [Running Tests](#running-tests)
- [Default Credentials](#default-credentials)
- [Ports](#ports)
- [Configuration](#configuration)
- [Project Structure](#project-structure)
- [Database](#database)
- [Security](#security)
- [API Endpoints](#api-endpoints)
- [Architecture & BMad Process](#architecture--bmad-process)
- [Supported Browsers](#supported-browsers)
- [Conventions](#conventions)
- [License](#license)

---

## Prerequisites

| Tool | Version |
|------|---------|
| .NET SDK | 10.0.100+ ([download](https://dotnet.microsoft.com/download)) |
| Node.js | 22.x or 24.x LTS ([download](https://nodejs.org/)) |
| npm | Ships with Node (11.x+) |

## Quick Start

```bash
# 1. Clone
git clone https://github.com/<owner>/AngularWithNET.git
cd AngularWithNET

# 2. Restore & build backend
dotnet build AngularWithNET.csproj -c Release

# 3. Install & build frontend
cd ClientApp
npm install
npx ng build --configuration production
cd ..

# 4. Copy Angular output to wwwroot
# (PowerShell)
Copy-Item -Path ClientApp/dist/* -Destination wwwroot/ -Recurse -Force
# (bash/zsh)
cp -r ClientApp/dist/* wwwroot/

# 5. Run
dotnet run --project AngularWithNET.csproj --launch-profile AngularWithNET
# Open http://localhost:5000
```

## Development Workflow (two-process)

Run the API and Angular dev server in separate terminals for hot-reload on both sides:

**Terminal 1 — API**
```bash
dotnet watch run --project AngularWithNET.csproj --launch-profile AngularWithNET
# API listens on http://localhost:5000
```

**Terminal 2 — Angular**
```bash
cd ClientApp
npm start
# Angular dev server on http://localhost:4200 (proxies /security/* and /api/* to :5000)
```

Open `http://localhost:4200` in your browser. API calls are automatically proxied to the backend via `proxy.conf.json`.

## Running Tests

### .NET Tests (xUnit)

```bash
dotnet test tests/AngularWithNET.Tests/AngularWithNET.Tests.csproj
```

Runs **30 tests** covering:
- **Integration tests** — Auth (login, refresh, logout, token rotation), Todos (CRUD, authorization, user isolation), Settings (non-secret values, correlation ID headers)
- **Unit tests** — PasswordService (hash/verify), DbSeeder (idempotent seeding, hashed passwords)

The test project uses `WebApplicationFactory<Program>` with in-memory SQLite, so no external database is needed.

### Angular Tests (Jasmine/Karma)

```bash
cd ClientApp
npx ng test --watch=false --browsers=ChromeHeadless
```

Runs **42 tests** covering:
- **Service tests** — AuthService, TodoService, SettingsService (HTTP calls, session management, error handling)
- **Guard tests** — authGuard (allow/deny based on token presence)
- **Component tests** — TodoListComponent (CRUD, progress, empty state), LoginComponent (form validation, error display), NavMenuComponent (toggle, logout), CounterComponent, AppSettingsComponent

## Default Credentials

| User ID | Password | Permissions |
|---------|----------|-------------|
| `admin` | `admin` | All (ViewCounter, ViewForecast, ViewAppSettings) |
| `user` | `user` | Limited (ViewCounter only) |

Demo users are seeded automatically in the `Development` environment.

## Ports

| Service | URL |
|---------|-----|
| API (Kestrel) | `http://localhost:5000` |
| Angular dev server | `http://localhost:4200` |

## Configuration

- **`appsettings.json`** — Serilog config, JWT settings, app settings surfaced to Angular.
- **`appsettings.Development.json`** — CORS origins for the Angular dev server (`http://localhost:4200`).
- **`Properties/launchSettings.json`** — Kestrel port and environment.

### Serilog Logging

Logs are written to:
- **Console** — always, for immediate visibility.
- **Rolling file** — `logs/app-YYYYMMDD.log` relative to the content root. Configure via the `Serilog:WriteTo:File` section in `appsettings.json`.

Each request is tagged with a `CorrelationId` (via middleware) for end-to-end tracing.

### Secrets

The JWT signing key is in `appsettings.json` (`AppSettings:Secret`). For production, override it via environment variable or [user-secrets](https://learn.microsoft.com/en-us/aspnet/core/security/app-secrets):

```bash
dotnet user-secrets set "AppSettings:Secret" "your-secure-random-key-here"
```

## Project Structure

```
├── Features/                 # Feature-folder organized API
│   ├── Auth/                 #   Login, refresh, logout + PasswordService
│   ├── Todos/                #   CRUD + completion timestamps
│   └── Settings/             #   Non-secret config for the SPA
├── Domain/                   # EF Core entities (User, RefreshToken, TodoItem)
├── Data/                     # AppDbContext, migrations, DbSeeder
├── Infrastructure/           # Middleware (CorrelationId, GlobalExceptionHandler)
├── Controllers/              # Legacy API controllers (SecurityController)
├── ViewModel/                # Legacy DTOs, AppSettings, middleware
├── ClientApp/                # Angular 19 SPA
│   ├── src/app/core/         #   AuthService, SettingsService, guards, interceptors
│   ├── src/app/features/     #   TodoService, TodoListComponent
│   ├── src/app/login/        #   Login component
│   ├── src/app/nav-menu/     #   Navigation with auth-aware links
│   ├── proxy.conf.json       #   Dev proxy (ng serve → API)
│   └── package.json          #   Angular dependencies & scripts
├── tests/                    # .NET xUnit test project
│   └── AngularWithNET.Tests/ #   Integration + unit tests
├── _bmad/                    # BMad Method framework (skills, agents, workflows)
├── _bmad-output/             # BMad planning & implementation artifacts
│   ├── planning-artifacts/   #   PRD, architecture, epics, readiness report
│   └── implementation-artifacts/ # Story specs, sprint status
├── wwwroot/                  # Production Angular build output
├── appsettings.json          # Serilog, JWT, app settings
├── Program.cs                # Host entry point (DB init, seeding, fail-fast)
├── Startup.cs                # Service registration & middleware pipeline
├── SECURITY.md               # Vulnerability reporting policy
└── logs/                     # Rolling log files (git-ignored)
```

## Database

The application uses **SQLite** via **EF Core** with checked-in migrations.

- The database file (`app.db`) is created automatically at startup via `Database.Migrate()`.
- If the configured path is not writable, the application **fails fast** with a clear error message.
- The database file and WAL/SHM files are git-ignored.
- For backup, copy `app.db` while the application is stopped.

**Entities:** `User`, `RefreshToken`, `TodoItem` — each with proper relationships and indexes.

## Security

- **Authentication:** JWT access tokens (configurable expiry) + opaque refresh tokens with rotation.
- **Password hashing:** ASP.NET Core Identity `PasswordHasher<T>` (PBKDF2).
- **Rate limiting:** Fixed-window rate limiter on auth endpoints (configurable via `RateLimiting:PermitLimit`).
- **Route guards:** Angular `authGuard` redirects unauthenticated users to `/login`.
- **Permissions:** Server returns a permission list per user; Angular `PermissionDirective` controls UI visibility.
- **Error handling:** RFC 7807 Problem Details with correlation IDs; no secrets or stack traces leaked in responses.
- **Refresh token security:** Tokens are stored as SHA-256 hashes in the database; old tokens are revoked on rotation.
- Passwords and tokens are **never logged** (NFR-S1 compliance).

For vulnerability reporting, see [SECURITY.md](SECURITY.md).

## API Endpoints

| Method | Route | Auth | Description |
|--------|-------|------|-------------|
| POST | `/api/auth/login` | No | Authenticate and receive tokens |
| POST | `/api/auth/refresh` | No | Rotate refresh token |
| POST | `/api/auth/logout` | Yes | Revoke all refresh tokens |
| GET | `/api/todos` | Yes | List user's todos |
| GET | `/api/todos/:id` | Yes | Get a single todo |
| POST | `/api/todos` | Yes | Create a todo |
| PUT | `/api/todos/:id` | Yes | Update title / completion |
| DELETE | `/api/todos/:id` | Yes | Delete a todo |
| GET | `/api/settings` | No | Non-secret app settings + demo mode flag |

## Architecture & BMad Process

This project was planned and implemented using the **[BMad Method](https://github.com/bmadcode/BMAD-METHOD)** — an AI-assisted software development workflow. The full planning and implementation artifacts are included in the repository:

### Planning Artifacts (`_bmad-output/planning-artifacts/`)

| File | Description |
|------|-------------|
| `prd.md` | Product Requirements Document — functional & non-functional requirements |
| `architecture.md` | Technical architecture decisions and constraints |
| `epics.md` | Full epic and story breakdown (6 epics, 30+ stories) with FR traceability |
| `implementation-readiness-report-*.md` | Pre-implementation validation checklist |

### Implementation Artifacts (`_bmad-output/implementation-artifacts/`)

| File | Description |
|------|-------------|
| `sprint-status.yaml` | Sprint tracking with story statuses |
| `*.md` (story files) | Individual story specifications with acceptance criteria |

### Epic Summary

| Epic | Name | Stories |
|------|------|---------|
| 1 | Modern platform, build path, and project honesty | 6 stories — .NET 10 + Angular 19 upgrade, Serilog, dev workflow, README |
| 2 | Sign-in, session continuity, and public entry | 6 stories — SQLite, auth, JWT, refresh tokens, Angular login |
| 3 | Full task lifecycle for the signed-in user | 6 stories — Todo CRUD, completion timestamps, persistence |
| 4 | App shell, permissions, settings, and trust | 5 stories — Settings API, navigation, permissions, demo mode |
| 5 | Actionable errors and support correlation | 3 stories — Correlation IDs, Problem Details, client error display |
| 6 | Test coverage for API and Angular client | 5 stories — xUnit integration/unit tests, Karma/Jasmine Angular tests |

### BMad Framework (`_bmad/`)

The `_bmad/` directory contains the BMad framework itself (agents, skills, workflows). The `.github/skills/` directory mirrors these for GitHub Copilot compatibility. These are part of the development tooling and can be safely ignored if you are not using the BMad workflow.

## Supported Browsers

Current major versions of Chrome, Edge, Firefox, and Safari on desktop and mobile.

## Conventions

- **API routes:** Feature-folder controllers (`/api/auth/*`, `/api/todos/*`, `/api/settings`) + legacy (`/Security/*`).
- **Angular:** NgModule pattern with `standalone: false` on all components (standalone conversion planned for a future epic).
- **Naming:** C# PascalCase, TypeScript/Angular camelCase, kebab-case filenames.
- **Logging:** Serilog with `FromLogContext` enricher, CorrelationId, UserId, and Path properties.
- **JSON:** camelCase property names, ISO 8601 UTC dates.
- **Error responses:** RFC 7807 Problem Details with `X-Correlation-Id` header.

## License

See [LICENSE](LICENSE) or the repository's license file.
