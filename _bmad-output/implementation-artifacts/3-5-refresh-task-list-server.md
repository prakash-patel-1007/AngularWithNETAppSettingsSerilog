# Story 3.5: Refresh task list from server

Status: review

## Story

As a **signed-in user**,
I want **to reload the task list to pick up server changes**,
so that **I trust what I see matches the backend (FR12)** (Epic 3).

## Acceptance Criteria

1. **Given** tasks changed on the server (e.g. second tab or API) **when** I trigger the documented refresh action **then** the UI reflects the latest server state.

## Tasks / Subtasks

- [ ] **AC1 — Refresh mechanism**
  - [ ] Add a visible "Refresh" button (or pull-to-refresh on mobile) to the todo list UI.
  - [ ] On click: re-fetch `GET /api/todos` and replace the list.
  - [ ] Show loading indicator during refresh.
  - [ ] Show error message if refresh fails.
- [ ] **Green build check**
  - [ ] `ng build --configuration production` succeeds.

## Dev Notes

### Scope boundary (critical)

- **In scope:** Manual refresh action for the todo list.
- **Out of scope:** Auto-refresh, WebSocket, polling.

### References

- [Source: epics.md — Epic 3, Story 3.5]

## Dev Agent Record

### Agent Model Used
_(fill on implementation)_
### Debug Log References
### Completion Notes List
### File List
_(fill on implementation)_
