# Story 3.4: Angular TODO feature UI (CRUD + complete)

Status: review

## Story

As a **signed-in user**,
I want **a focused UI to manage my tasks on desktop and narrow viewports**,
so that **I can use the app daily without framework knowledge (FR5–FR10, UX-DR1, UX-DR3)** (Epic 3).

## Acceptance Criteria

1. **Given** I am authenticated **when** I open the TODO area **then** I can create, edit, delete, and mark tasks complete with clear labels (NFR-A1, UX-DR3).
2. **Given** a narrow viewport **when** I use TODO flows **then** layout remains usable with touch-friendly targets (UX-DR1).
3. **When** tasks are completed **then** completion time is visible per FR10.

## Tasks / Subtasks

- [ ] **AC1 — Todo list component**
  - [ ] Create `features/todo/todo-list/todo-list.component.ts` (standalone if Angular 16+).
  - [ ] On load: call `GET /api/todos` via `TodoService`, display list with title, completion status, completion date.
  - [ ] Show `isLoading` spinner during fetch; show `errorMessage` on failure.
  - [ ] Each item has: edit button, delete button, complete/uncomplete toggle.
- [ ] **AC1 — Create todo**
  - [ ] Add a form (inline or modal) with title (required) and description (optional) fields.
  - [ ] On submit: `POST /api/todos`, refresh list on success, show error on failure.
- [ ] **AC1 — Edit todo**
  - [ ] On edit click: show editable fields (inline or navigate to detail).
  - [ ] On save: `PUT /api/todos/{id}`, refresh item on success.
- [ ] **AC1 — Delete todo**
  - [ ] On delete: confirm action, then `DELETE /api/todos/{id}`, remove from list on success.
- [ ] **AC1 — Complete/uncomplete**
  - [ ] Toggle: `PATCH /api/todos/{id}` (or chosen endpoint from Story 3.3).
  - [ ] Show `completedAt` timestamp when completed (formatted for readability).
- [ ] **AC2 — Responsive layout**
  - [ ] Use CSS (Bootstrap or utility classes) for single-column layout on narrow viewports.
  - [ ] Touch-friendly button sizes (min 44x44px tap targets).
- [ ] **AC3 — Accessibility**
  - [ ] All controls have visible labels or `aria-label`.
  - [ ] Tab order is logical.
  - [ ] Visible focus indicators.
- [ ] **Angular TodoService**
  - [ ] Create `features/todo/todo.service.ts` wrapping `HttpClient` calls to `/api/todos`.
- [ ] **Routing**
  - [ ] Add route `/todos` (protected by auth guard from Story 2.5).
- [ ] **Green build check**
  - [ ] `ng build --configuration production` succeeds.

## Dev Notes

### Scope boundary (critical)

- **In scope:** Angular todo UI — list, create, edit, delete, complete; responsive; accessible.
- **Out of scope (Story 3.5):** Explicit refresh button/mechanism (basic list reload on CRUD actions is fine here).
- **Out of scope (Story 3.6):** Persistence verification across reload.

### Architecture compliance

- **Structure:** `features/todo/todo-list/`, `features/todo/todo.service.ts`. [Source: architecture.md — Frontend architecture]
- **Standalone components:** Prefer standalone. [Source: architecture.md]
- **Loading/errors:** `isLoading`, `errorMessage`. [Source: architecture.md — Process patterns]
- **Accessibility:** Semantic HTML, keyboard, visible focus, labels. [Source: NFR-A1, UX-DR3]
- **Responsive:** Narrow viewport usable (UX-DR1).

### References

- [Source: epics.md — Epic 3, Story 3.4]

## Dev Agent Record

### Agent Model Used
_(fill on implementation)_
### Debug Log References
### Completion Notes List
### File List
_(fill on implementation)_
