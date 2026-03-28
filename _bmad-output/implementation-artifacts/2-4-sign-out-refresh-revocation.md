# Story 2.4: Sign-out and refresh revocation

Status: review

## Story

As an **authenticated user**,
I want **to sign out and have my refresh session ended server-side**,
so that **shared devices do not stay logged in (FR2)** (Epic 2).

## Acceptance Criteria

1. **Given** an authenticated session with a valid refresh record **when** I invoke sign-out **then** refresh tokens for that session are revoked or deleted and cannot be reused.
2. **When** I attempt refresh after sign-out **then** I receive an auth error suitable for returning the client to login (FR4).

## Tasks / Subtasks

- [ ] **AC1 — Sign-out endpoint**
  - [ ] Create `POST /api/auth/logout` endpoint (authenticated, requires valid access token).
  - [ ] Revoke all active refresh tokens for the user (or just the current session's token, per design choice — document which).
  - [ ] Return 204 No Content on success.
- [ ] **AC2 — Post-logout refresh fails**
  - [ ] Verify: after calling logout, attempting `POST /api/auth/refresh` with the previously valid refresh token returns 401.
  - [ ] Ensure no token values are logged during logout processing.
- [ ] **Green build check**
  - [ ] `dotnet build -c Release` succeeds.

## Dev Notes

### Scope boundary (critical)

- **In scope:** Logout endpoint, refresh token revocation.
- **Out of scope (Story 2.5):** Angular login UI.
- **Out of scope (Story 2.6):** Angular interceptor / session clearing.

### Previous story intelligence (Story 2.3)

- `AuthController` with login + refresh endpoints exists.
- `RefreshToken` entity with `RevokedAt` field supports revocation.
- Official JWT Bearer auth registered in DI.

### Architecture compliance

- **Sign-out:** Server-side revocation of refresh tokens. [Source: architecture.md]
- **REST:** `POST /api/auth/logout`. [Source: architecture.md — API patterns]

### References

- [Source: epics.md — Epic 2, Story 2.4]

## Dev Agent Record

### Agent Model Used
_(fill on implementation)_
### Debug Log References
### Completion Notes List
### File List
_(fill on implementation)_
