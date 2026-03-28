# Story 4.3: Permission checks on client and server

Status: review

## Story

As a **signed-in user**,
I want **features I am not allowed to use to be hidden or blocked with server enforcement**,
so that **I cannot bypass UI-only checks (FR13, FR14)** (Epic 4).

## Acceptance Criteria

1. **Given** a user lacking a permission claim required for a feature **when** they use the UI **then** controls are disabled/hidden per directive or guard policy.
2. **Given** the same user crafts a direct API request **when** they hit a protected capability **then** the server returns 403 (or consistent policy) per FR14.

## Tasks / Subtasks

- [ ] **AC1 — Client-side permission checks**
  - [ ] Define a permission model: e.g. claims in JWT or a `/api/auth/profile` endpoint returning user permissions.
  - [ ] Create a permission directive or service (`shared/directives/has-permission.directive.ts` or `core/services/permission.service.ts`) that shows/hides/disables UI elements based on user permissions.
  - [ ] Apply to appropriate UI elements (e.g. admin-only features, settings access).
  - [ ] Existing permission model from legacy code (`ViewCounter`, `ViewForecast`, `ViewAppSettings`) can be migrated and extended.
- [ ] **AC2 — Server-side permission enforcement**
  - [ ] Add authorization policies or role checks on API endpoints that require specific permissions.
  - [ ] Return `403 Forbidden` (not 401) when authenticated but not authorized.
  - [ ] Ensure response does not leak internal details.
- [ ] **Green build check**
  - [ ] Both builds succeed.

## Dev Notes

### Scope boundary (critical)

- **In scope:** Permission model, client directive/service, server enforcement with 403.
- **Out of scope:** Role management UI, admin user CRUD.

### Architecture compliance

- **Permissions:** Claims/roles from token or profile endpoint — pick one pattern. [Source: architecture.md — Frontend architecture]
- **Server enforcement:** 403 for forbidden. [Source: architecture.md — API errors]

### References

- [Source: epics.md — Epic 4, Story 4.3]
- [Source: architecture.md — Frontend architecture, API errors]
- [Source: Controllers/SecurityController.cs — existing GetPermissions()]

## Dev Agent Record

### Agent Model Used
_(fill on implementation)_
### Debug Log References
### Completion Notes List
### File List
_(fill on implementation)_
