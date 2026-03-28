# Story 5.2: Problem Details and production-safe API errors

Status: review

## Story

As a **user**,
I want **consistent, actionable API errors without stack traces in production-oriented configs**,
so that **I am not exposed to internal details (FR20, NFR-S6)** (Epic 5).

## Acceptance Criteria

1. **Given** Production or production-like environment **when** an unhandled API error occurs **then** the client receives Problem Details (or compatible JSON) without stack traces or internal file paths.
2. **Given** validation failures **when** a write DTO is invalid **then** the response uses 400 with machine-readable validation detail.

## Tasks / Subtasks

- [ ] **AC1 — Global exception handling**
  - [ ] Replace or enhance `CustomErrorMiddleware` with a proper exception handler.
  - [ ] Use `IExceptionHandler` (.NET 8+) or `UseExceptionHandler` with a custom handler that returns Problem Details JSON.
  - [ ] In Production: return `{ "type": "...", "title": "An error occurred", "status": 500, "traceId": "<correlationId>" }` — NO stack trace, NO internal paths.
  - [ ] In Development: optionally include more detail for debugging.
  - [ ] Include `correlationId` (from Story 5.1) as an extension property in the Problem Details response.
- [ ] **AC2 — Validation error format**
  - [ ] Ensure ASP.NET model validation returns `400` with Problem Details format (this is default behavior with `[ApiController]` — verify it works).
  - [ ] Customize validation problem details if needed to include consistent `type` and `title` fields.
- [ ] **AC2 — Consistent error responses**
  - [ ] Audit existing error responses (401, 403, 404, 429) to ensure they follow Problem Details shape or are at minimum consistent JSON.
  - [ ] Remove legacy `CustomErrorMiddleware` if fully replaced.
- [ ] **Green build check**
  - [ ] `dotnet build -c Release` succeeds.

## Dev Notes

### Scope boundary (critical)

- **In scope:** Global exception handling, Problem Details for all error responses, production-safe (no stack traces).
- **Out of scope (Story 5.3):** Angular client display of errors.

### Previous story intelligence

- Story 5.1: Correlation ID middleware exists — reference it in error responses.
- Existing `CustomErrorMiddleware` — evaluate if it can be replaced entirely.

### Architecture compliance

- **Errors:** RFC 7807 Problem Details with `correlationId` extension. [Source: architecture.md — API errors]
- **NFR-S6:** No stack traces in production responses. [Source: PRD]
- **Status codes:** 400, 401, 403, 404, 429, 500. [Source: architecture.md]

### References

- [Source: epics.md — Epic 5, Story 5.2]
- [Source: architecture.md — API errors, Format patterns]

## Dev Agent Record

### Agent Model Used
_(fill on implementation)_
### Debug Log References
### Completion Notes List
### File List
_(fill on implementation)_
