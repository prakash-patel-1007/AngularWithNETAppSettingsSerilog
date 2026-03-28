# Story 2.5: Angular public routes, login screen, and auth error feedback

Status: review

## Story

As a **visitor**,
I want **to reach sign-in without authentication and see clear errors when credentials or session renewal fail**,
so that **I understand what to do next (FR4, FR18, UX-DR3)** (Epic 2).

## Acceptance Criteria

1. **Given** I am not signed in **when** I navigate to public routes documented for sign-in **then** I am not blocked by the authenticated shell guard.
2. **Given** invalid credentials **when** I submit login **then** I see a clear, non-technical error message (no stack trace).
3. **Given** the login form **when** I use keyboard only **then** I can complete login with visible focus and labeled controls (UX-DR3, NFR-A1 for login).

## Tasks / Subtasks

- [ ] **AC1 — Public routes**
  - [ ] Create or migrate Angular login component under `ClientApp/src/app/features/auth/login/`.
  - [ ] Configure Angular Router so `/login` (or `/auth/login`) is accessible without an auth guard.
  - [ ] Add an auth guard (`core/guards/auth.guard.ts`) that redirects unauthenticated users to `/login` for protected routes. Apply it to protected route groups.
- [ ] **AC2 — Login form with error feedback**
  - [ ] Build a login form with username and password fields, submit button.
  - [ ] On submit, call `POST /api/auth/login` via `AuthService`.
  - [ ] On success: store tokens (per chosen transport), navigate to the main authenticated area.
  - [ ] On failure: display a clear, user-friendly error message (e.g. "Invalid username or password") — no stack traces or technical details.
  - [ ] Show loading state (`isLoading`) during the API call.
- [ ] **AC3 — Keyboard accessibility**
  - [ ] All form controls have visible labels (not just placeholders).
  - [ ] Tab order is logical: username → password → submit.
  - [ ] Focus indicator is visible on all interactive elements.
  - [ ] Form can be submitted via Enter key.
- [ ] **Green build check**
  - [ ] `ng build --configuration production` succeeds.

## Dev Notes

### Scope boundary (critical)

- **In scope:** Angular login component, public route config, auth guard skeleton, error display.
- **Out of scope (Story 2.6):** HTTP interceptor for Bearer token attachment and silent refresh.
- **Out of scope (Story 4.2):** Full app shell navigation — just login and guard basics here.

### Architecture compliance

- **Angular structure:** `features/auth/login/`, `core/guards/`, `core/services/auth.service.ts`. [Source: architecture.md — Frontend architecture]
- **Standalone components:** Prefer standalone if Angular 16+. [Source: architecture.md]
- **Loading/errors:** `isLoading`, `errorMessage` flags. [Source: architecture.md — Process patterns]
- **Accessibility:** Semantic HTML, keyboard path, visible focus, meaningful labels (NFR-A1, UX-DR3).

### References

- [Source: epics.md — Epic 2, Story 2.5]
- [Source: architecture.md — Frontend architecture, Process patterns]

## Dev Agent Record

### Agent Model Used
_(fill on implementation)_
### Debug Log References
### Completion Notes List
### File List
_(fill on implementation)_
