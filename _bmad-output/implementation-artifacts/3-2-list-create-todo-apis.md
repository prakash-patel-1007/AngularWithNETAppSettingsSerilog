# Story 3.2: Authorized list and create todo APIs

Status: review

## Story

As a **signed-in user**,
I want **to list and create tasks via REST**,
so that **I see only my tasks when per-user mode is on (FR5, FR6, FR11, FR14)** (Epic 3).

## Acceptance Criteria

1. **Given** a valid access token **when** I `GET /api/todos` **then** I receive only tasks owned by my user id from claims, not from request body spoofing.
2. **Given** valid task input **when** I `POST /api/todos` **then** it persists and returns camelCase JSON with stable id (architecture JSON rules).
3. **Given** no token or invalid token **when** I call these endpoints **then** I receive 401/403 without leaking internal details (NFR-S6).

## Tasks / Subtasks

- [ ] **AC1 — List todos (GET)**
  - [ ] Create `Features/Todos/TodosController.cs` with `[ApiController]`, `[Route("api/todos")]`, `[Authorize]`.
  - [ ] `GET /api/todos` — extract user id from JWT claims; query `AppDbContext.Todos.Where(t => t.UserId == userId)`.
  - [ ] Return `200` with array of todo DTOs (camelCase JSON): `id`, `title`, `description`, `isCompleted`, `completedAt`, `createdAt`.
  - [ ] Create `Features/Todos/Dtos/TodoDto.cs` for the response shape.
- [ ] **AC2 — Create todo (POST)**
  - [ ] `POST /api/todos` — accept `CreateTodoDto` with `title` (required), `description` (optional).
  - [ ] Set `UserId` from JWT claims (never from request body).
  - [ ] Set `CreatedAt` to `DateTime.UtcNow`, `IsCompleted` to `false`.
  - [ ] Return `201 Created` with the created `TodoDto` and `Location` header.
  - [ ] Validate input: if `title` is missing/empty, return `400` with Problem Details.
- [ ] **AC3 — Unauthorized access**
  - [ ] Requests without a valid JWT return `401`.
  - [ ] Response body does not expose stack traces or internal paths.
- [ ] **Green build check**
  - [ ] `dotnet build -c Release` succeeds.

## Dev Notes

### Scope boundary (critical)

- **In scope:** GET and POST for todos, authorization via JWT claims.
- **Out of scope (Story 3.3):** Edit, delete, complete endpoints.
- **Out of scope (Story 3.4):** Angular UI.

### Architecture compliance

- **REST:** `GET /api/todos`, `POST /api/todos` (plural). [Source: architecture.md — API patterns]
- **JSON:** camelCase, ISO 8601 dates. [Source: architecture.md — Format patterns]
- **Auth:** User id from JWT claims. [Source: architecture.md — Data boundaries]
- **Errors:** Problem Details for validation. [Source: architecture.md — API errors]

### References

- [Source: epics.md — Epic 3, Story 3.2]
- [Source: architecture.md — API patterns, Format patterns, Data boundaries]

## Dev Agent Record

### Agent Model Used
_(fill on implementation)_
### Debug Log References
### Completion Notes List
### File List
_(fill on implementation)_
