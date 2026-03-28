# Story 4.1: Non-secret settings for the SPA

Status: review

## Story

As a **signed-in user**,
I want **the client to load non-secret configuration from the API**,
so that **behavior can differ by environment without leaking secrets (FR15)** (Epic 4).

## Acceptance Criteria

1. **Given** the settings endpoint **when** the client requests it with a valid token if required by design **then** only documented non-secret keys are returned (no signing keys or connection secrets).
2. **When** settings fail to load **then** the UI degrades gracefully with actionable messaging (FR20 partial).

## Tasks / Subtasks

- [ ] **AC1 — Settings API endpoint**
  - [ ] Create `Features/Settings/SettingsController.cs` with `GET /api/settings`.
  - [ ] Return only non-secret configuration values (e.g. app name, feature flags, demo mode indicator). Explicitly exclude JWT secrets, connection strings, etc.
  - [ ] Decide: public (no auth required) or authenticated. Document the choice.
  - [ ] Create `Features/Settings/Dtos/AppSettingsDto.cs` for the response shape.
  - [ ] Migrate or deprecate legacy `SecurityController.GetSettings()`.
- [ ] **AC2 — Angular settings service**
  - [ ] Create `core/services/settings.service.ts` that calls `GET /api/settings`.
  - [ ] Load settings on app initialization (e.g. `APP_INITIALIZER` or on first route load).
  - [ ] If settings fail to load: show a user-friendly error (e.g. banner or toast), do not crash the app.
- [ ] **Green build check**
  - [ ] Both `dotnet build -c Release` and `ng build --configuration production` succeed.

## Dev Notes

### Scope boundary (critical)

- **In scope:** Settings API endpoint, Angular settings service, graceful degradation.
- **Out of scope:** Secret management UI, admin config.

### Architecture compliance

- **REST:** `GET /api/settings`. [Source: architecture.md — API patterns]
- **No secrets in response:** Only documented non-secret keys. [Source: NFR-S2]
- **Feature folder:** `Features/Settings/`. [Source: architecture.md — Project structure]
- **Angular:** `core/services/settings.service.ts`. [Source: architecture.md — Frontend]

### References

- [Source: epics.md — Epic 4, Story 4.1]
- [Source: architecture.md — Project structure, API patterns]
- [Source: Controllers/SecurityController.cs — existing GetSettings()]

## Dev Agent Record

### Agent Model Used
_(fill on implementation)_
### Debug Log References
### Completion Notes List
### File List
_(fill on implementation)_
