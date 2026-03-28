# Story 4.2: Authenticated shell navigation and route guards

Status: review

## Story

As a **signed-in user**,
I want **to move between authorized areas via the app shell**,
so that **I can reach TODO and other allowed pages (FR19)** (Epic 4).

## Acceptance Criteria

1. **Given** authenticated session **when** I use primary navigation **then** I can reach authorized routes without manual URL hacking.
2. **Given** signed-out state **when** I attempt a protected deep link **then** I am redirected to sign-in or shown an appropriate gate (FR18/FR19 interplay).

## Tasks / Subtasks

- [ ] **AC1 — App shell with navigation**
  - [ ] Create or migrate an app shell component (`app.component.ts` or `shell/`) with a navigation bar/menu.
  - [ ] Navigation links: Home, Todos, Settings (if applicable), Logout.
  - [ ] Conditionally show/hide nav items based on authentication state (use `AuthService.isAuthenticated$` or similar).
  - [ ] Apply existing styles (Bootstrap or project CSS) for consistency.
- [ ] **AC2 — Route guards**
  - [ ] Ensure auth guard from Story 2.5 is applied to all protected routes (`/todos`, `/settings`, etc.).
  - [ ] Protected deep links (e.g. `http://localhost:4200/todos`) redirect to `/login` if not authenticated.
  - [ ] After login, redirect back to the originally requested URL (return URL pattern).
- [ ] **Green build check**
  - [ ] `ng build --configuration production` succeeds.

## Dev Notes

### Scope boundary (critical)

- **In scope:** App shell with nav, route guard enforcement, return URL after login.
- **Out of scope (Story 4.3):** Permission-based visibility (just auth-based here).

### Architecture compliance

- **Shell:** `app.component`, nav, router-outlet; no TODO business logic in shell. [Source: architecture.md — Component boundaries]
- **Guards:** Angular Router + guards for auth. [Source: architecture.md — Frontend architecture]
- **Structure:** `core/guards/`, `features/` routes. [Source: architecture.md]

### References

- [Source: epics.md — Epic 4, Story 4.2]

## Dev Agent Record

### Agent Model Used
_(fill on implementation)_
### Debug Log References
### Completion Notes List
### File List
_(fill on implementation)_
