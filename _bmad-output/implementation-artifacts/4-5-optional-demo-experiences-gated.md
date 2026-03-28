# Story 4.5: Optional demonstration experiences (permission-gated)

Status: review

## Story

As a **user permitted for demos**,
I want **optional sample/toy experiences without gaining unrelated access**,
so that **the sample stays honest (FR17)** (Epic 4).

## Acceptance Criteria

1. **Given** demo permission **when** I open the optional demo experience **then** I cannot access unrelated admin or user data.
2. **Given** user without demo permission **when** they attempt the same routes/APIs **then** access is denied per FR14.

## Tasks / Subtasks

- [ ] **AC1 — Demo feature area**
  - [ ] Create a demo feature area (e.g. `features/demo/` with a component showing sample data like weather forecast from existing `GetForecast()` endpoint or equivalent).
  - [ ] Gate access with a demo-specific permission (e.g. `ViewForecast` from the existing permission model).
  - [ ] Route: `/demo` or `/demo/forecast` — protected by auth guard + permission guard.
- [ ] **AC2 — Permission enforcement**
  - [ ] Users without the demo permission see the route hidden in nav and receive redirect or 403 if accessing directly.
  - [ ] Server-side: the demo API endpoint requires the relevant permission claim.
  - [ ] Demo access does NOT grant access to other users' todos or admin features.
- [ ] **Green build check**
  - [ ] Both builds succeed.

## Dev Notes

### Scope boundary (critical)

- **In scope:** Demo feature area, permission gating, isolation from other features.
- **Out of scope:** New demo content creation tools.

### Previous story intelligence

- Story 4.3 establishes the permission model — reuse it here.
- Legacy `SecurityController.GetForecast()` with `[Authorize]` and `ViewForecast` permission exists — can be migrated or wrapped.

### References

- [Source: epics.md — Epic 4, Story 4.5]

## Dev Agent Record

### Agent Model Used
_(fill on implementation)_
### Debug Log References
### Completion Notes List
### File List
_(fill on implementation)_
