# Story 3.1: Todo persistence model and migration

Status: review

## Story

As a **signed-in user**,
I want **tasks stored in SQLite with a relationship to my user**,
so that **my data can be isolated per deployment configuration (FR11)** (Epic 3).

## Acceptance Criteria

1. **Given** Epic 2 user schema in place **when** a new migration is added **then** it introduces **only** the `Todos` table and FK to user as needed (no unrelated schema).
2. **Given** the database **when** I inspect the model **then** tasks include fields needed for title/body, completion flag, and `CompletedAt` nullable timestamp.

## Tasks / Subtasks

- [ ] **AC1 — Todo entity and migration**
  - [ ] Create `Domain/Todo.cs`: `Id` (int or Guid — match User Id type), `Title` (string, required, max length), `Description` (string, nullable), `IsCompleted` (bool, default false), `CompletedAt` (DateTime?, nullable, UTC), `UserId` (FK to User), `CreatedAt` (DateTime UTC), `UpdatedAt` (DateTime? UTC).
  - [ ] Add `DbSet<Todo>` to `AppDbContext`. Configure FK relationship and any indexes (e.g. `IX_Todos_UserId`).
  - [ ] Create migration: `dotnet ef migrations add AddTodos`.
  - [ ] Verify: migration creates only `Todos` table with correct columns and FK.
- [ ] **AC2 — Schema validation**
  - [ ] Start the API — `Database.Migrate()` applies the new migration.
  - [ ] Inspect `app.db`: `Todos` table has `Id`, `Title`, `Description`, `IsCompleted`, `CompletedAt`, `UserId`, `CreatedAt`, `UpdatedAt` columns.
  - [ ] No other new tables created beyond `Todos`.
- [ ] **Green build check**
  - [ ] `dotnet build -c Release` succeeds.

## Dev Notes

### Scope boundary (critical)

- **In scope:** Todo entity, EF configuration, migration.
- **Out of scope (Story 3.2):** API endpoints for CRUD.
- **Out of scope (Story 3.4):** Angular UI.

### Previous story intelligence

- Story 2.1: `AppDbContext` with `DbSet<User>` and `InitialCreate` migration exist.
- Story 2.3: `RefreshToken` entity and migration added.

### Architecture compliance

- **Tables:** `Todos` (PascalCase plural). **FKs:** `UserId`. [Source: architecture.md — Naming patterns]
- **Dates:** UTC DateTime. [Source: architecture.md — Format patterns]
- **EF migrations:** Checked in. [Source: architecture.md — Data architecture]

### References

- [Source: epics.md — Epic 3, Story 3.1]
- [Source: architecture.md — Data architecture, Naming patterns]

## Dev Agent Record

### Agent Model Used
_(fill on implementation)_
### Debug Log References
### Completion Notes List
### File List
_(fill on implementation)_
