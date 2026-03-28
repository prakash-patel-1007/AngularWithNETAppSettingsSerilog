# Story 4.4: Shared or demonstration environment notice

Status: review

## Story

As a **visitor or signed-in user**,
I want **a clear notice when the instance is shared or demo**,
so that **I do not treat it as private storage (FR16, UX-DR4)** (Epic 4).

## Acceptance Criteria

1. **Given** configuration marks deployment as shared/demo **when** I use the app entry path **then** I see short, plain language warning to avoid sensitive data.

## Tasks / Subtasks

- [ ] **AC1 — Demo mode configuration**
  - [ ] Add a config flag in `appsettings.json`: `"DemoMode": true/false` (or `"AppSettings.IsDemoEnvironment"`).
  - [ ] Expose via the settings API endpoint (`GET /api/settings`) so the Angular client can read it.
- [ ] **AC1 — Demo notice UI**
  - [ ] When `demoMode` is true in settings: show a persistent banner at the top of the app (or on login page).
  - [ ] Banner text: "This is a demo environment. Do not store sensitive data." (or equivalent plain language).
  - [ ] Banner should be dismissible but re-appear on page reload.
  - [ ] Style: visually distinct (e.g. yellow/warning color) but not blocking.
- [ ] **Green build check**
  - [ ] Both builds succeed.

## Dev Notes

### Scope boundary (critical)

- **In scope:** Config flag, settings API exposure, Angular banner component.
- **Out of scope:** Data isolation, data retention policies.

### Architecture compliance

- **Trust copy:** Plain language warning per UX-DR4. [Source: PRD]
- **Settings:** Exposed via `/api/settings`. [Source: architecture.md]

### References

- [Source: epics.md — Epic 4, Story 4.4]

## Dev Agent Record

### Agent Model Used
_(fill on implementation)_
### Debug Log References
### Completion Notes List
### File List
_(fill on implementation)_
