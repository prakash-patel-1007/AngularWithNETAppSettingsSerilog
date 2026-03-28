# Story 3.3: Edit, delete, and complete with completion timestamp

Status: review

## Story

As a **signed-in user**,
I want **to edit, delete, mark complete, and see when completion happened**,
so that **I can trust task history (FR7, FR8, FR9, FR10)** (Epic 3).

## Acceptance Criteria

1. **Given** an owned incomplete task **when** I mark it complete via the API **then** `CompletedAt` is set to a UTC ISO-8601 timestamp and `isCompleted` is true.
2. **Given** an owned task **when** I edit allowed fields **then** changes persist and validation errors return Problem Details for bad input.
3. **Given** an owned task **when** I delete it **then** it no longer appears on subsequent list calls.
4. **Given** another user's task id **when** I attempt mutate/delete **then** I receive 404 or 403 per chosen consistent policy.

## Tasks / Subtasks

- [ ] **AC1 — Mark complete**
  - [ ] Add `PATCH /api/todos/{id}/complete` (or `PATCH /api/todos/{id}` with body `{ "isCompleted": true }`) — choose one pattern and document.
  - [ ] When completing: set `IsCompleted = true`, `CompletedAt = DateTime.UtcNow`, `UpdatedAt = DateTime.UtcNow`.
  - [ ] When un-completing (if supported): set `IsCompleted = false`, `CompletedAt = null`.
  - [ ] Return updated `TodoDto` with `completedAt` as ISO 8601 UTC string.
- [ ] **AC2 — Edit todo**
  - [ ] Add `PUT /api/todos/{id}` accepting `UpdateTodoDto` with `title`, `description` fields.
  - [ ] Validate: `title` required. Return `400` Problem Details on invalid input.
  - [ ] Set `UpdatedAt = DateTime.UtcNow`.
  - [ ] Verify ownership (user id from claims == todo's UserId); return 404 if not owned (don't reveal other users' data).
- [ ] **AC3 — Delete todo**
  - [ ] Add `DELETE /api/todos/{id}`.
  - [ ] Verify ownership; delete from DB. Return `204 No Content`.
  - [ ] If not found or not owned: return `404`.
- [ ] **AC4 — Cross-user protection**
  - [ ] All mutate/delete operations check `UserId` from JWT claims against the todo's `UserId`.
  - [ ] Consistently return `404` (preferred for security — don't reveal existence of other users' tasks).
- [ ] **Green build check**
  - [ ] `dotnet build -c Release` succeeds.

## Dev Notes

### Scope boundary (critical)

- **In scope:** PUT, DELETE, PATCH (complete) for todos, ownership checks.
- **Out of scope (Story 3.4):** Angular UI.

### Architecture compliance

- **REST:** Standard HTTP verbs on `/api/todos/{id}`. [Source: architecture.md]
- **Dates:** `CompletedAt` as ISO 8601 UTC. [Source: architecture.md — Format patterns]
- **Errors:** Problem Details for validation; 404 for cross-user access. [Source: architecture.md — API errors]
- **Status codes:** 200 update, 204 delete, 400 validation, 404 not found/not owned. [Source: architecture.md]

### References

- [Source: epics.md — Epic 3, Story 3.3]

## Dev Agent Record

### Agent Model Used
_(fill on implementation)_
### Debug Log References
### Completion Notes List
### File List
_(fill on implementation)_
