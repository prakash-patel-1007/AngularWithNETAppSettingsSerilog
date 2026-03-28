# Story 2.6: Bearer attachment and silent refresh with single retry

Status: review

## Story

As an **authenticated user**,
I want **API calls to send the access token and automatically refresh once when it expires**,
so that **I avoid unnecessary re-login (FR3) with a defined failure path (UX-DR5)** (Epic 2).

## Acceptance Criteria

1. **Given** a stored refresh capability and access token **when** I call a protected API **then** `Authorization: Bearer` is attached per architecture.
2. **Given** an expired access token and valid refresh **when** the API returns 401 **then** the client attempts **exactly one** refresh-and-retry cycle before giving up.
3. **Given** refresh failure **when** retry is exhausted **then** the client clears session state and shows "session expired — sign in again" (or equivalent) and behavior for unsaved form state is documented (UX-DR5).

## Tasks / Subtasks

- [ ] **AC1 — HTTP interceptor for Bearer**
  - [ ] Create `core/interceptors/auth.interceptor.ts` that attaches `Authorization: Bearer <accessToken>` to all API requests (or requests matching `/api/*`).
  - [ ] Register the interceptor in the Angular HTTP client configuration (standalone: `provideHttpClient(withInterceptors([...]))` or legacy module approach).
- [ ] **AC2 — Silent refresh on 401**
  - [ ] In the interceptor, catch 401 responses.
  - [ ] Attempt ONE refresh call to `POST /api/auth/refresh` using the stored refresh token.
  - [ ] If refresh succeeds: store new tokens, retry the original request with the new access token.
  - [ ] If refresh fails (401, network error, etc.): do NOT retry again — fall through to AC3.
  - [ ] Ensure concurrent 401s don't trigger multiple refresh calls (use a flag or subject to serialize).
- [ ] **AC3 — Session expiry UX**
  - [ ] On refresh failure: clear all stored tokens and auth state via `AuthService`.
  - [ ] Navigate to `/login` with a message: "Session expired — please sign in again" (or equivalent).
  - [ ] Document behavior for unsaved form state: best-effort — forms are NOT preserved across session expiry (document in README or inline comment).
- [ ] **Green build check**
  - [ ] `ng build --configuration production` succeeds.

## Dev Notes

### Scope boundary (critical)

- **In scope:** HTTP interceptor, Bearer attachment, single silent refresh retry, session expiry handling.
- **Out of scope (Story 2.5):** Login form UI (already done).
- **Out of scope (Story 4.2):** Full shell navigation — just the interceptor + expiry redirect here.

### Architecture compliance

- **Bearer:** `Authorization: Bearer <access_token>`. [Source: architecture.md — Communication patterns]
- **Retry:** On 401, one refresh attempt; failure → clear session → login. No infinite loops. [Source: architecture.md]
- **UX-DR5:** Clear "session expired" message; unsaved form behavior documented. [Source: PRD]

### References

- [Source: epics.md — Epic 2, Story 2.6]
- [Source: architecture.md — Communication patterns, Client retry]

## Dev Agent Record

### Agent Model Used
_(fill on implementation)_
### Debug Log References
### Completion Notes List
### File List
_(fill on implementation)_
