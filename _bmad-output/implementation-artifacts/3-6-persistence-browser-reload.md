# Story 3.6: Persistence across browser reload

Status: review

## Story

As a **signed-in user**,
I want **my task changes to survive a full page reload**,
so that **I trust durability (FR22)** (Epic 3).

## Acceptance Criteria

1. **Given** I created or completed tasks **when** I hard-refresh the browser **then** the same tasks appear with correct completion timestamps for my user in per-user mode.

## Tasks / Subtasks

- [ ] **AC1 — Verify persistence**
  - [ ] This is primarily a verification story. Ensure that:
    - Auth tokens survive page reload (stored in localStorage/sessionStorage or httpOnly cookies per chosen transport).
    - On reload, the auth interceptor attaches the token and `GET /api/todos` returns the user's tasks from SQLite.
    - `CompletedAt` timestamps display correctly after reload.
  - [ ] If token storage is not yet implemented: implement token persistence in `AuthService` (localStorage for access token if using JSON body transport, or rely on cookies if using httpOnly cookie transport).
  - [ ] Document the token storage approach in README or Dev Agent Record.
- [ ] **Green build check**
  - [ ] `ng build --configuration production` succeeds.

## Dev Notes

### Scope boundary (critical)

- **In scope:** Verifying end-to-end persistence across browser reload, implementing token storage if missing.
- **Out of scope:** Offline support, service workers.

### References

- [Source: epics.md — Epic 3, Story 3.6]

## Dev Agent Record

### Agent Model Used
_(fill on implementation)_
### Debug Log References
### Completion Notes List
### File List
_(fill on implementation)_
