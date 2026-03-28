# Story 2.1: Application database with users and fail-fast initialization

Status: review

## Story

As an **operator**,
I want **SQLite + EF Core with a User store and fail-fast startup if the database path is unusable**,
so that **the app does not fail mysteriously on first write** (Epic 2).

## Acceptance Criteria

1. **Given** a configured SQLite connection string or path **when** the directory is not writable or cannot be created **then** startup fails immediately with a clear log message (NFR-P2).
2. **Given** a valid writable path **when** the API starts **then** migrations apply per documented approach (`Database.Migrate()` or `dotnet ef` documented).
3. **When** the first migration runs **then** it creates only the schema needed for **users** (no upfront creation of unrelated tables for future epics).

## Tasks / Subtasks

- [ ] **AC1 — Fail-fast DB initialization**
  - [ ] Add `Microsoft.EntityFrameworkCore.Sqlite` and `Microsoft.EntityFrameworkCore.Design` packages to csproj.
  - [ ] Create `Data/AppDbContext.cs` with `DbSet<User>` (User entity only for this story).
  - [ ] Create `Domain/User.cs` entity: `Id` (int or Guid), `Username` (string, unique), `PasswordHash` (string), `CreatedAt` (DateTime UTC).
  - [ ] Register `AppDbContext` in DI with SQLite connection string from `appsettings.json` (e.g. `"ConnectionStrings": { "DefaultConnection": "Data Source=app.db" }`).
  - [ ] Add startup code that checks if the DB directory is writable BEFORE calling `Database.Migrate()`. If not writable, log a clear error via Serilog and throw / exit with non-zero code.
  - [ ] Add `app.db` and `*.db-*` to `.gitignore`.
- [ ] **AC2 — EF Core migration**
  - [ ] Create initial migration: `dotnet ef migrations add InitialCreate` (or equivalent).
  - [ ] Call `Database.Migrate()` at startup (in `Program.cs` or a startup filter) to apply pending migrations.
  - [ ] Verify: on first run, `app.db` is created with a `Users` table.
- [ ] **AC3 — Schema discipline**
  - [ ] The migration ONLY creates the `Users` table (and EF's `__EFMigrationsHistory`). No `Todos` or `RefreshTokens` tables in this migration.
  - [ ] Verify by inspecting the migration file or running `.schema` on the SQLite DB.
- [ ] **Green build check**
  - [ ] `dotnet build -c Release` succeeds.

## Dev Notes

### Scope boundary (critical)

- **In scope:** EF Core + SQLite setup, User entity, migration, fail-fast startup, DB connection config.
- **Out of scope (Story 2.2):** Password hashing, demo user seed.
- **Out of scope (Story 2.3):** RefreshToken entity and table, JWT endpoints.
- **Out of scope (Story 3.1):** Todo entity and table.

### Architecture compliance

- **SQLite + EF Core** with migrations checked in. [Source: architecture.md — Data architecture]
- **Tables:** `Users` (PascalCase plural). [Source: architecture.md — Naming patterns]
- **FKs:** `NavigationPropertyId` pattern. [Source: architecture.md]
- **Fail-fast:** Clear log message if DB path not writable (NFR-P2). [Source: PRD]
- **Feature folders:** `Data/AppDbContext.cs`, `Domain/User.cs`. [Source: architecture.md — Project structure]

### Testing requirements

- **Manual:** Start API → `app.db` created → `Users` table exists → no other domain tables.
- **Manual:** Set DB path to read-only directory → startup fails with clear log message.

### References

- [Source: epics.md — Epic 2, Story 2.1]
- [Source: architecture.md — Data architecture, Naming patterns, Project structure]

## Dev Agent Record

### Agent Model Used
_(fill on implementation)_
### Debug Log References
### Completion Notes List
### File List
_(fill on implementation)_
