# Story 5.1: Correlation ID middleware and log enrichment

Status: review

## Story

As an **operator**,
I want **each request to carry a correlation id into Serilog**,
so that **I can trace failures across components (NFR-O2, FR21)** (Epic 5).

## Acceptance Criteria

1. **Given** an incoming request without correlation id **when** middleware runs early in the pipeline **then** a correlation id is assigned or propagated from an agreed header.
2. **When** an error is logged **then** the correlation id appears as a structured property in Serilog output.

## Tasks / Subtasks

- [ ] **AC1 — Correlation ID middleware**
  - [ ] Create `Infrastructure/CorrelationIdMiddleware.cs`.
  - [ ] On each request: check for an incoming header (e.g. `X-Correlation-Id`). If present, use it; if not, generate a new GUID.
  - [ ] Store the correlation id in `HttpContext.Items` and/or `HttpContext.TraceIdentifier`.
  - [ ] Optionally set the correlation id as a response header so the client can read it.
  - [ ] Register the middleware EARLY in the pipeline (before routing, auth, etc.) in `Startup.cs` / `Program.cs`.
- [ ] **AC2 — Serilog enrichment**
  - [ ] Push the correlation id into Serilog's `LogContext` using `LogContext.PushProperty("CorrelationId", correlationId)` within the middleware.
  - [ ] Update `appsettings.json` Serilog output template to include `{CorrelationId}` (or rely on structured JSON output where properties are automatic).
  - [ ] Verify: make a request → check console/file log → `CorrelationId` appears as a structured property.
- [ ] **Optional enrichments**
  - [ ] Add `UserId` enrichment (extract from JWT claims if authenticated) and `Path` property.
  - [ ] These are nice-to-have for this story; can be deferred if scope grows.
- [ ] **Green build check**
  - [ ] `dotnet build -c Release` succeeds.

## Dev Notes

### Scope boundary (critical)

- **In scope:** Correlation ID middleware, Serilog enrichment, response header.
- **Out of scope (Story 5.2):** Problem Details error responses.
- **Out of scope (Story 5.3):** Angular client display of correlation id.

### Architecture compliance

- **Correlation ID:** Middleware early in pipeline, propagated to Serilog. [Source: architecture.md — Infrastructure]
- **Structured properties:** `CorrelationId`, `UserId`, `Path`. [Source: architecture.md — Communication patterns]
- **Header:** `X-Correlation-Id` or `X-Request-Id` (pick one). [Source: architecture.md]

### References

- [Source: epics.md — Epic 5, Story 5.1]
- [Source: architecture.md — Infrastructure, Communication patterns]

## Dev Agent Record

### Agent Model Used
_(fill on implementation)_
### Debug Log References
### Completion Notes List
### File List
_(fill on implementation)_
