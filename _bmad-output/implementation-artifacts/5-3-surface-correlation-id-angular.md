# Story 5.3: Surface correlation ID in the Angular client

Status: review

## Story

As a **signed-in user**,
I want **to see or copy a reference id when the server returns one with an error**,
so that **I can give it to someone troubleshooting (FR21)** (Epic 5).

## Acceptance Criteria

1. **Given** an API error payload including correlation/trace extension **when** the client shows the error state **then** the reference id is visible or copyable in a non-technical way.
2. **And** sensitive headers are never logged to console in the client build (defense in depth).

## Tasks / Subtasks

- [ ] **AC1 — Extract correlation ID from error responses**
  - [ ] In the Angular HTTP error handling (interceptor or service-level), extract the `correlationId` (or `traceId`) from the Problem Details response body or response header (`X-Correlation-Id`).
  - [ ] Store it on the error object or pass it to the error display component.
- [ ] **AC1 — Display reference ID to user**
  - [ ] When showing an error message to the user, include a "Reference ID: {correlationId}" line that the user can copy.
  - [ ] Style it to be non-technical: small text below the main error message, with a copy button or selectable text.
  - [ ] If no correlation id is available (e.g. network error before reaching the server), omit the reference line.
- [ ] **AC2 — No sensitive header logging**
  - [ ] Ensure the Angular app does NOT log `Authorization` headers or token values to `console.log` in production builds.
  - [ ] Review existing code for any `console.log` statements that might leak sensitive data.
  - [ ] Consider using `environment.production` flag to disable verbose console logging in production builds.
- [ ] **Green build check**
  - [ ] `ng build --configuration production` succeeds.

## Dev Notes

### Scope boundary (critical)

- **In scope:** Angular error display with correlation ID, no sensitive console logging.
- **Out of scope:** Server-side changes (covered in 5.1 and 5.2).

### Previous story intelligence

- Story 5.1: Server returns `X-Correlation-Id` header and includes `correlationId` in Problem Details body.
- Story 5.2: All server errors return Problem Details format.

### Architecture compliance

- **FR21:** User can communicate reference id for troubleshooting. [Source: PRD]
- **Defense in depth:** No sensitive header logging in client. [Source: architecture.md]

### References

- [Source: epics.md — Epic 5, Story 5.3]

## Dev Agent Record

### Agent Model Used
_(fill on implementation)_
### Debug Log References
### Completion Notes List
### File List
_(fill on implementation)_
