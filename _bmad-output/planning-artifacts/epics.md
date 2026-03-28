---
stepsCompleted:
  - 1
  - 2
  - 3
  - 4
workflowType: epics-and-stories
epicsWorkflowStatus: complete
epicsWorkflowCompletedAt: 2026-03-28
inputDocuments:
  - _bmad-output/planning-artifacts/prd.md
  - _bmad-output/planning-artifacts/architecture.md
---

# AngularWithNETAppSettingsSerilog - Epic Breakdown

## Overview

This document provides the complete epic and story breakdown for AngularWithNETAppSettingsSerilog, decomposing the requirements from the PRD, UX Design if it exists, and Architecture requirements into implementable stories.

## Requirements Inventory

### Functional Requirements

FR1: A visitor can submit credentials to establish an authenticated session.

FR2: An authenticated user can end their session (sign out).

FR3: An authenticated user can continue using protected capabilities after a short break without re-entering credentials, within the product’s session policy.

FR4: A user receives clear feedback when authentication fails or the session cannot be renewed.

FR5: An authenticated user can create a task.

FR6: An authenticated user can view a list of tasks available to them in the current deployment.

FR7: An authenticated user can edit a task they are allowed to change.

FR8: An authenticated user can delete a task they are allowed to delete.

FR9: An authenticated user can mark a task complete.

FR10: An authenticated user can see when a task was completed (completion timestamp).

FR11: The product associates tasks with the correct user when the deployment is configured for per-user data.

FR12: An authenticated user can refresh or reload the task list to reflect the latest server state.

FR13: An authenticated user can access only features their account is permitted to use.

FR14: The system rejects unauthorized access to protected server capabilities.

FR15: The client application can obtain non-secret configuration values intended for display or behavior in the SPA (e.g. equivalent to prior “app settings” exposure).

FR16: When an instance is a shared or demonstration environment, the user can see an appropriate notice (in-product or via documented entry path).

FR17: A user permitted by the product can access optional demonstration experiences (e.g. sample data or toy features) without gaining access to unrelated protected capabilities.

FR18: An unauthenticated user can access routes intended for sign-in and other public entry.

FR19: An authenticated user can move between authorized areas of the application using the application shell.

FR20: A user sees actionable messages for common failures without internal implementation details in production-oriented configurations.

FR21: When the system provides a correlation or reference identifier with an error, the user can communicate it for troubleshooting alongside server logs.

FR22: Task changes made by an authenticated user in a per-user deployment persist across reloads for that user until changed or deleted.

### NonFunctional Requirements

NFR-P1: Primary interactive actions (sign-in, list tasks, save task) complete in **under 3 seconds** under normal dev/local conditions unless documented otherwise (not a cloud SLA).

NFR-P2: Application startup fails **fast** with a **clear log message** when persistence cannot be initialized (e.g. unwritable data path), rather than failing on first user write.

NFR-S1: Credentials and refresh artifacts are **never written to application logs**.

NFR-S2: Signing keys and secrets for production-like configurations come from **environment or secret store**, not committed defaults.

NFR-S3: Authentication endpoints are protected against **trivial abuse** (e.g. rate limiting or equivalent on credential submission).

NFR-S4: Data access uses **parameterized** database access patterns to mitigate injection risk.

NFR-S5: Refresh tokens are **stored and validated** such that **rotation** or **invalidation** can be applied without changing the FR contract (implementation may use one-time rotation).

NFR-S6: API responses in production-oriented configurations **do not expose stack traces** or internal paths to the client.

NFR-SC1: The system is sized for **small to moderate concurrent use** on a **single-node** deployment with **embedded database**; **large-scale multi-tenant SaaS** is **out of MVP scope**.

NFR-SC2: Documentation states **SQLite** operational limits and **backup** expectations for operators.

NFR-A1 (MVP): Core flows (sign-in, task list, create/edit task) are **keyboard reachable** and use **meaningful labels** for controls.

NFR-A2 (Growth): Target **WCAG 2.1 Level AA** on core flows with a documented audit approach.

NFR-I1: No **mandatory** third-party SaaS integrations for MVP; the product operates with **bundled** persistence and **documented** REST usage.

NFR-I2: External identity providers (e.g. OIDC) are **optional future** scope unless added via PRD change.

NFR-O1: Operators can use **structured or consistent** log output to **diagnose** auth, API errors, and startup, using **cross-platform** log destinations documented in the runbook.

NFR-O2: Server-side errors intended for support include a **correlation identifier** where feasible (aligned with FR21).

### Additional Requirements

- **Brownfield migration:** Modernize the existing **Angular + ASP.NET Core** solution **in place**; do **not** replace with greenfield `dotnet new angular`; keep solution-relative **`ClientApp`** unless an ADR moves it.
- **Runtime targets:** Prefer **.NET 10 (LTS)**; **.NET 8 LTS** acceptable if policy requires; **Angular** must move to a **currently supported major** with exact minor/patch pinned in `package.json` / lockfile at implementation time.
- **Persistence:** **SQLite** + **EF Core**; relational model with **Users**, **RefreshTokens**, **Todos** (or equivalent names per EF configuration); **migrations** checked in; **fail-fast** at startup if the database path is not writable.
- **Auth implementation:** **JWT** short-lived access tokens; **opaque refresh tokens** persisted in SQLite **hashed**, with **rotation on refresh**; use **official ASP.NET Core JWT Bearer** validation (not custom-only middleware); passwords hashed (e.g. PasswordHasher or BCrypt).
- **API shape:** **REST** JSON, **unversioned** `/api/...` for MVP; **plural** resource paths (e.g. `/api/todos`, `/api/auth/login`, `/api/auth/refresh`); **pick one** auth route root and retire legacy `/api/security/...` inconsistency.
- **Errors and observability:** **RFC 7807 Problem Details**-compatible JSON where practical; **correlation ID** middleware early in pipeline, propagated to **Serilog**; **no** logging of access or refresh tokens or passwords.
- **Serilog:** **Console** + **rolling file** (or agreed sinks); file path **configurable** and **cross-platform**; structured properties where feasible (`CorrelationId`, `UserId`, `Path`, `ElapsedMs`).
- **Security defaults:** **Rate limiting** (or equivalent) on **login and refresh** endpoints; **CORS** with **explicit** allowed origins (dev: Angular dev server); secrets from **environment / user-secrets**, not committed production values.
- **Hosting:** **Development:** `dotnet watch` + `ng serve` + **`proxy.conf`** to API; **Production:** API serves **built SPA** **or** reverse proxy serves static + forwards `/api`; **`index.html` fallback** for deep links aligned with Angular `base href`.
- **Client patterns:** **HTTP interceptor** adds **Bearer**; on **401**, **one** silent **refresh** attempt then retry; failure → clear session and route to login; **route guards** for authenticated areas; **permission** model for FR13 (claims/roles—single pattern, documented).
- **Code layout (target):** **`Features/Auth`**, **`Features/Todos`**, **`Features/Settings`** with controllers or minimal APIs + DTOs; **`Data/AppDbContext`**, **`Data/Migrations`**; **`Infrastructure/`** for correlation id, rate limiting helpers; migrate legacy **`Controllers/`** and **`ViewModel/`** into features then remove.
- **JSON conventions:** **camelCase** property names; dates **ISO 8601** UTC in JSON; validation on write DTOs server-side.
- **Refresh transport:** Choose **one** documented approach (**httpOnly Secure cookie** vs **JSON body** for refresh token) and implement consistently in API + README; never pass refresh token in query string.
- **Implementation sequence (architecture):** (1) Retarget .NET + Serilog + pipeline cleanup; (2) EF Core + SQLite + users + refresh + seed; (3) JWT + refresh endpoints; (4) Todo API + authorization; (5) Angular upgrade + interceptor + TODO UI; (6) README + rename + acceptance bundle.

### UX Design Requirements

_No standalone UX Design document was found under `planning-artifacts`. The following PRD-derived UI/UX expectations apply until a dedicated UX spec is added:_

- **UX-DR1 (PRD — responsive):** TODO and auth flows remain **usable on narrow viewports** (single column, touch-friendly targets); sample/extra pages may be desktop-first if labeled in docs.
- **UX-DR2 (PRD — browsers):** Support **current** Chrome, Edge, Firefox, Safari (desktop + recent mobile); document minimum versions in README.
- **UX-DR3 (PRD — MVP accessibility):** Core flows use **semantic HTML**, **keyboard** path for login + TODO, **visible focus**, and **meaningful control labels** (NFR-A1); fix obvious contrast issues.
- **UX-DR4 (PRD — trust copy):** Shared/demo deployments surface an **appropriate notice** (in-product or documented path) per FR16; errors show **actionable** copy without stack traces in production-oriented configs (FR20).
- **UX-DR5 (PRD — session UX):** When refresh fails, user sees a **clear “session expired — sign in again”** (or equivalent) message; unsaved form behavior **documented** (best-effort preserve vs clear).

### FR Coverage Map

| FR | Epic | Brief trace |
|----|------|-------------|
| FR1 | Epic 2 | Submit credentials → authenticated session |
| FR2 | Epic 2 | Sign out |
| FR3 | Epic 2 | Session continuation (access + refresh policy) |
| FR4 | Epic 2 | Clear feedback on auth / renewal failures |
| FR5 | Epic 3 | Create task |
| FR6 | Epic 3 | View task list |
| FR7 | Epic 3 | Edit task |
| FR8 | Epic 3 | Delete task |
| FR9 | Epic 3 | Mark task complete |
| FR10 | Epic 3 | See completion timestamp |
| FR11 | Epic 3 | Tasks associated with correct user (per-user deployments) |
| FR12 | Epic 3 | Refresh / reload list for latest server state |
| FR13 | Epic 4 | Access only permitted features |
| FR14 | Epic 4 | Server rejects unauthorized access to protected capabilities |
| FR15 | Epic 4 | Non-secret SPA configuration |
| FR16 | Epic 4 | Shared / demo environment notice |
| FR17 | Epic 4 | Optional demonstration experiences (permission-gated) |
| FR18 | Epic 2 | Public routes for sign-in and entry |
| FR19 | Epic 4 | Move between authorized areas (app shell) |
| FR20 | Epic 5 | Actionable failure messages (no internal leakage in prod-oriented configs) |
| FR21 | Epic 5 | User can communicate correlation / reference id with support vs logs |

**PRD / architecture scope without a single FR id (upgrade & docs):** **Epic 1** — PRD MVP items **1** (stack upgrade + documented dev path) and **6** (README + rename); PRD **Technical Success** / **Measurable Outcomes** on clean checkout build; architecture **brownfield migration**, **Serilog** baseline, **proxy / static hosting** documentation, **version-neutral naming**.

_NFRs and architecture constraints apply across epics via acceptance criteria and shared infrastructure (e.g. NFR-S1/S5 on auth/TODO APIs, NFR-P2/O1-O2 on host startup and middleware)._

## Epic List

### Epic 1: Modern platform, build path, and project honesty

After this epic, a **developer (Alex)** or **operator (Morgan)** can follow the **README** to run a **supported .NET and Angular** codebase on a **clean checkout**: **documented** dev workflow (API + client + **proxy** or equivalent), **production static/API** story, **Serilog** to console + configurable file paths, **no obsolete version-specific branding** where avoidable, and **clear prerequisites / ports / security notes**. This epic delivers the **brownfield upgrade** and **documentation** track from the PRD and architecture (MVP scope items **1** and **6**, technical success signals).

**FRs covered:** _None directly — traceability via PRD MVP #1, #6, Technical Success, and Architecture “starter / implementation sequence” foundations._

**Implementation notes:** Retarget `TargetFramework` and SDK; Angular major migration per official guides; remove obsolete SPA hosting middleware where replaced by current patterns; `appsettings` + env/user-secrets for non-committed secrets; optional MSBuild/CI hook for `ng build` + publish — **one** documented path; CONTRIBUTING/SECURITY stubs if timeboxed in same epic.

### Epic 2: Sign-in, session continuity, and public entry

After this epic, a **visitor** can open public entry routes, **sign in** and **sign out**, and a **returning user** can keep using the app within the **session / refresh policy** without unnecessary re-prompts; **auth failures** are understandable.

**FRs covered:** FR1, FR2, FR3, FR4, FR18.

**Implementation notes (from architecture):** JWT access + opaque refresh with rotation; rate limiting on login/refresh; interceptor + single retry on client; pick one refresh transport (cookie vs body) and document; no token logging.

### Epic 3: Full task lifecycle for the signed-in user

After this epic, an **authenticated user** can **create, view, edit, delete**, and **complete** tasks; see **when** a task was completed; **reload** the list; and rely on **persistence** and **per-user scoping** where the deployment is configured that way.

**FRs covered:** FR5, FR6, FR7, FR8, FR9, FR10, FR11, FR12, FR22.

**Implementation notes:** REST `/api/todos` (or equivalent plural resource); user id from token claims for authorization; SQLite + EF Core; ISO dates in JSON; Problem Details for validation errors.

### Epic 4: App shell, permissions, settings, and trust

After this epic, users **navigate** the authorized app shell, obtain **non-secret** client configuration, see **shared/demo** transparency when applicable, access **optional demos** only when permitted, and the product **enforces feature permissions** consistently on client and server.

**FRs covered:** FR13, FR14, FR15, FR16, FR17, FR19.

**Implementation notes:** Align with existing permission directive / guards migrated to `core/`; settings endpoint vs static env; FR14 enforced on every protected API.

### Epic 5: Actionable errors and support correlation

After this epic, when things go wrong, users see **safe, actionable** messages in production-oriented configurations and can share a **correlation (or reference) identifier** that operators can match in **structured logs**.

**FRs covered:** FR20, FR21.

**Implementation notes:** Correlation id middleware + Serilog enrichment; Problem Details shape; no stack traces or internal paths in prod-oriented API responses.

---

## Epic 1: Modern platform, build path, and project honesty

After this epic, a **developer (Alex)** or **operator (Morgan)** can follow the **README** to run a **supported .NET and Angular** codebase on a **clean checkout**: **documented** dev workflow (API + client + **proxy** or equivalent), **production static/API** story, **Serilog** to console + configurable file paths, **no obsolete version-specific branding** where avoidable, and **clear prerequisites / ports / security notes**.

### Story 1.1: Retarget API to agreed .NET LTS and restore green build

As a **developer**,
I want **the server project to target a supported .NET version and build cleanly on a fresh clone**,
So that **I have a credible baseline before feature work**.

**Acceptance Criteria:**

**Given** a clean checkout with the documented SDK installed  
**When** I run `dotnet build` at the solution/project root  
**Then** the build completes with no errors  
**And** obsolete package references introduced by the retarget are resolved or listed as follow-ups with no false green

**Given** the architecture preference for .NET 10 (or approved .NET 8)  
**When** I inspect the project file  
**Then** `TargetFramework` matches the agreed LTS choice documented in README

### Story 1.2: Align HTTP host pipeline with current ASP.NET patterns

As a **developer**,
I want **the API host to use the supported minimal-hosting / middleware pipeline for the chosen .NET version**,
So that **auth, CORS, and static files integrate the way current docs recommend**.

**Acceptance Criteria:**

**Given** the upgraded host  
**When** I start the API with Development settings  
**Then** Kestrel listens on the documented port(s) from launch settings  
**And** legacy SPA middleware that conflicts with the chosen dev/prod split is removed or replaced per architecture (no dead code paths that break deep linking)

**Given** a request to a non-API route in production configuration  
**When** static files and fallback are configured  
**Then** Angular deep links fall back to `index.html` where that pattern is selected

### Story 1.3: Upgrade Angular workspace to a supported major

As a **developer**,
I want **ClientApp on a currently supported Angular major with a successful production build**,
So that **security and tooling match present-day expectations**.

**Acceptance Criteria:**

**Given** documented Node/npm prerequisites  
**When** I run the client production build (`ng build` or npm script)  
**Then** the build completes without errors  
**And** `package.json` / lockfile pin the chosen Angular major (per architecture)

**Given** the brownfield constraint  
**When** I review the migration approach  
**Then** it follows official upgrade/migration guidance (major-by-major or approved jump), not a discarded greenfield scaffold

### Story 1.4: Add Serilog with console and configurable file logging

As an **operator**,
I want **structured logs to console and to rolling files on a configurable, cross-platform path**,
So that **I can diagnose startup and request issues without leaking secrets**.

**Acceptance Criteria:**

**Given** default Development configuration  
**When** the API starts  
**Then** Serilog writes human- or machine-readable output to console (NFR-O1)

**Given** configured file sink path  
**When** the API runs on Windows and on a Unix-like OS (documented as best-effort if CI only covers one)  
**Then** log files roll per configuration and paths are not hard-coded to a single developer machine

**Given** login or refresh requests occur  
**When** logs are written for those requests  
**Then** access tokens, refresh tokens, and passwords never appear in log payloads (NFR-S1)

### Story 1.5: Document and wire the two-process dev workflow with API proxy

As a **developer**,
I want **Angular dev server traffic proxied to the API and both commands documented**,
So that **I can run UI and API together without CORS friction during development**.

**Acceptance Criteria:**

**Given** `proxy.conf` (or equivalent) pointing to the API base  
**When** I run `dotnet watch` / API and `ng serve` / npm start as documented  
**Then** browser calls to `/api/*` reach the API successfully

**Given** PRD performance guidance  
**When** I exercise sign-in and list endpoints locally  
**Then** interactive actions remain within the documented “under 3 seconds” expectation under normal dev conditions unless documented exceptions apply (NFR-P1)

### Story 1.6: README runbook, rename, and public-repo hygiene

As a **developer evaluating the repo**,
I want **README to cover prerequisites, ports, SQLite writability, security expectations, and version-neutral naming**,
So that **I can reproduce Alex’s journey without undocumented steps**.

**Acceptance Criteria:**

**Given** the README “how to run” section  
**When** I follow it sequentially  
**Then** I can start API + client and reach the app shell or login (PRD Technical Success / Measurable Outcomes)

**Given** project, solution, and visible UI strings  
**When** I search for obsolete version-specific branding  
**Then** avoidable references are removed or tracked with rationale (PRD naming / differentiation)

**Given** supported browsers per PRD (UX-DR2)  
**When** I read README  
**Then** minimum or “current major” browser expectations for MVP are stated

**And** README or SECURITY describes how to report vulnerabilities for this public repo (PRD domain OSS)

---

## Epic 2: Sign-in, session continuity, and public entry

After this epic, a **visitor** can use **public entry routes**, **sign in** and **sign out**, and a **returning user** can stay signed in per **refresh policy** with **clear** auth errors.

### Story 2.1: Application database with users and fail-fast initialization

As an **operator**,
I want **SQLite + EF Core with a User store and fail-fast startup if the database path is unusable**,
So that **the app does not fail mysteriously on first write**.

**Acceptance Criteria:**

**Given** a configured SQLite connection string or path  
**When** the directory is not writable or cannot be created  
**Then** startup fails immediately with a clear log message (NFR-P2)

**Given** a valid writable path  
**When** the API starts  
**Then** migrations apply per documented approach (`Database.Migrate()` or `dotnet ef` documented)

**When** the first migration runs  
**Then** it creates only the schema needed for **users** (no upfront creation of unrelated tables for future epics)

### Story 2.2: Password hashing and demo user seed

As a **developer**,
I want **passwords stored with a strong hasher and documented demo accounts for local use**,
So that **FR1 has real credentials without hard-coded checks in code**.

**Acceptance Criteria:**

**Given** a new user row  
**When** a password is saved  
**Then** only a hash/stored secret suitable for verification is persisted (NFR-S4 via parameterized EF; aligns with NFR-S2 for secrets elsewhere)

**Given** Development environment  
**When** seed runs  
**Then** at least one demo user exists and credentials are documented in README, not committed as production defaults

### Story 2.3: Login, JWT access, and refresh token rotation

As a **visitor**,
I want **to sign in and receive short-lived access plus a rotated refresh token stored server-side**,
So that **I can use the API securely and renew my session (FR1, FR3, NFR-S5)**.

**Acceptance Criteria:**

**Given** valid demo credentials  
**When** I POST to the documented login endpoint  
**Then** I receive an access token and refresh artifact per the chosen transport (cookie or JSON body) documented in README

**Given** a valid refresh request  
**When** I POST to the refresh endpoint  
**Then** a new refresh identifier is issued and the previous refresh is invalidated if rotation is enabled (NFR-S5)

**Given** login and refresh endpoints  
**When** abuse simulation (rapid repeated posts)  
**Then** rate limiting or equivalent mitigates trivial brute force (NFR-S3)

**And** tokens never appear in logs (NFR-S1)

### Story 2.4: Sign-out and refresh revocation

As an **authenticated user**,
I want **to sign out and have my refresh session ended server-side**,
So that **shared devices do not stay logged in (FR2)**.

**Acceptance Criteria:**

**Given** an authenticated session with a valid refresh record  
**When** I invoke sign-out  
**Then** refresh tokens for that session are revoked or deleted and cannot be reused

**When** I attempt refresh after sign-out  
**Then** I receive an auth error suitable for returning the client to login (FR4)

### Story 2.5: Angular public routes, login screen, and auth error feedback

As a **visitor**,
I want **to reach sign-in without authentication and see clear errors when credentials or session renewal fail**,
So that **I understand what to do next (FR4, FR18, UX-DR3)**.

**Acceptance Criteria:**

**Given** I am not signed in  
**When** I navigate to public routes documented for sign-in  
**Then** I am not blocked by the authenticated shell guard

**Given** invalid credentials  
**When** I submit login  
**Then** I see a clear, non-technical error message (no stack trace)

**Given** the login form  
**When** I use keyboard only  
**Then** I can complete login with visible focus and labeled controls (UX-DR3, NFR-A1 for login)

### Story 2.6: Bearer attachment and silent refresh with single retry

As an **authenticated user**,
I want **API calls to send the access token and automatically refresh once when it expires**,
So that **I avoid unnecessary re-login (FR3) with a defined failure path (UX-DR5)**.

**Acceptance Criteria:**

**Given** a stored refresh capability and access token  
**When** I call a protected API  
**Then** `Authorization: Bearer` is attached per architecture

**Given** an expired access token and valid refresh  
**When** the API returns 401  
**Then** the client attempts **exactly one** refresh-and-retry cycle before giving up

**Given** refresh failure  
**When** retry is exhausted  
**Then** the client clears session state and shows “session expired — sign in again” (or equivalent) and behavior for unsaved form state is documented (UX-DR5)

---

## Epic 3: Full task lifecycle for the signed-in user

After this epic, an **authenticated user** can **manage tasks** with **completion timestamps**, **reload** the list, and rely on **persistence** for per-user deployments.

### Story 3.1: Todo persistence model and migration

As a **signed-in user**,
I want **tasks stored in SQLite with a relationship to my user**,
So that **my data can be isolated per deployment configuration (FR11)**.

**Acceptance Criteria:**

**Given** Epic 2 user schema in place  
**When** a new migration is added  
**Then** it introduces **only** the `Todo` (or equivalent) table and FK to user as needed (no unrelated schema)

**Given** the database  
**When** I inspect the model  
**Then** tasks include fields needed for title/body (as designed), completion flag, and `CompletedAt` nullable timestamp

### Story 3.2: Authorized list and create todo APIs

As a **signed-in user**,
I want **to list and create tasks via REST**,
So that **I see only my tasks when per-user mode is on (FR5, FR6, FR11, FR14)**.

**Acceptance Criteria:**

**Given** a valid access token  
**When** I `GET` the todos collection  
**Then** I receive only tasks owned by my user id from claims, not from request body spoofing

**Given** valid task input  
**When** I `POST` a new todo  
**Then** it persists and returns camelCase JSON with stable id (architecture JSON rules)

**Given** no token or invalid token  
**When** I call these endpoints  
**Then** I receive 401/403 without leaking internal details (NFR-S6)

### Story 3.3: Edit, delete, and complete with completion timestamp

As a **signed-in user**,
I want **to edit, delete, mark complete, and see when completion happened**,
So that **I can trust task history (FR7, FR8, FR9, FR10)**.

**Acceptance Criteria:**

**Given** an owned incomplete task  
**When** I mark it complete via the API  
**Then** `CompletedAt` is set to a UTC ISO-8601 timestamp and `isCompleted` (or equivalent) is true

**Given** an owned task  
**When** I edit allowed fields  
**Then** changes persist and validation errors return Problem Details for bad input

**Given** an owned task  
**When** I delete it  
**Then** it no longer appears on subsequent list calls

**Given** another user’s task id  
**When** I attempt mutate/delete  
**Then** I receive 404 or 403 per chosen consistent policy

### Story 3.4: Angular TODO feature UI (CRUD + complete)

As a **signed-in user**,
I want **a focused UI to manage my tasks on desktop and narrow viewports**,
So that **I can use the app daily without framework knowledge (FR5–FR10, UX-DR1, UX-DR3)**.

**Acceptance Criteria:**

**Given** I am authenticated  
**When** I open the TODO area  
**Then** I can create, edit, delete, and mark tasks complete with clear labels (NFR-A1, UX-DR3)

**Given** a narrow viewport  
**When** I use TODO flows  
**Then** layout remains usable with touch-friendly targets (UX-DR1)

**When** tasks are completed  
**Then** completion time is visible per FR10

### Story 3.5: Refresh task list from server

As a **signed-in user**,
I want **to reload the task list to pick up server changes**,
So that **I trust what I see matches the backend (FR12)**.

**Acceptance Criteria:**

**Given** tasks changed on the server (e.g. second tab or API)  
**When** I trigger the documented refresh action  
**Then** the UI reflects the latest server state

### Story 3.6: Persistence across browser reload

As a **signed-in user**,
I want **my task changes to survive a full page reload**,
So that **I trust durability (FR22)**.

**Acceptance Criteria:**

**Given** I created or completed tasks  
**When** I hard-refresh the browser  
**Then** the same tasks appear with correct completion timestamps for my user in per-user mode

---

## Epic 4: App shell, permissions, settings, and trust

After this epic, users **navigate** the app, read **non-secret settings**, see **demo transparency**, and **permissions** are enforced consistently.

### Story 4.1: Non-secret settings for the SPA

As a **signed-in user**,
I want **the client to load non-secret configuration from the API**,
So that **behavior can differ by environment without leaking secrets (FR15)**.

**Acceptance Criteria:**

**Given** the settings endpoint  
**When** the client requests it with a valid token if required by design  
**Then** only documented non-secret keys are returned (no signing keys or connection secrets)

**When** settings fail to load  
**Then** the UI degrades gracefully with actionable messaging (FR20 partial)

### Story 4.2: Authenticated shell navigation and route guards

As a **signed-in user**,
I want **to move between authorized areas via the app shell**,
So that **I can reach TODO and other allowed pages (FR19)**.

**Acceptance Criteria:**

**Given** authenticated session  
**When** I use primary navigation  
**Then** I can reach authorized routes without manual URL hacking

**Given** signed-out state  
**When** I attempt a protected deep link  
**Then** I am redirected to sign-in or shown an appropriate gate (FR18/FR19 interplay)

### Story 4.3: Permission checks on client and server

As a **signed-in user**,
I want **features I am not allowed to use to be hidden or blocked with server enforcement**,
So that **I cannot bypass UI-only checks (FR13, FR14)**.

**Acceptance Criteria:**

**Given** a user lacking a permission claim required for a feature  
**When** they use the UI  
**Then** controls are disabled/hidden per directive or guard policy

**Given** the same user crafts a direct API request  
**When** they hit a protected capability  
**Then** the server returns 403 (or consistent policy) per FR14

### Story 4.4: Shared or demonstration environment notice

As a **visitor or signed-in user**,
I want **a clear notice when the instance is shared or demo**,
So that **I do not treat it as private storage (FR16, UX-DR4)**.

**Acceptance Criteria:**

**Given** configuration marks deployment as shared/demo  
**When** I use the app entry path  
**Then** I see short, plain language warning to avoid sensitive data

### Story 4.5: Optional demonstration experiences (permission-gated)

As a **user permitted for demos**,
I want **optional sample/toy experiences without gaining unrelated access**,
So that **the sample stays honest (FR17)**.

**Acceptance Criteria:**

**Given** demo permission  
**When** I open the optional demo experience  
**Then** I cannot access unrelated admin or user data

**Given** user without demo permission  
**When** they attempt the same routes/APIs  
**Then** access is denied per FR14

---

## Epic 5: Actionable errors and support correlation

After this epic, users see **safe errors** and can share **correlation identifiers** aligned with logs.

### Story 5.1: Correlation ID middleware and log enrichment

As an **operator**,
I want **each request to carry a correlation id into Serilog**,
So that **I can trace failures across components (NFR-O2, FR21)**.

**Acceptance Criteria:**

**Given** an incoming request without correlation id  
**When** middleware runs early in the pipeline  
**Then** a correlation id is assigned or propagated from an agreed header

**When** an error is logged  
**Then** the correlation id appears as a structured property in Serilog output

### Story 5.2: Problem Details and production-safe API errors

As a **user**,
I want **consistent, actionable API errors without stack traces in production-oriented configs**,
So that **I am not exposed to internal details (FR20, NFR-S6)**.

**Acceptance Criteria:**

**Given** Production or production-like environment  
**When** an unhandled API error occurs  
**Then** the client receives Problem Details (or compatible JSON) without stack traces or internal file paths

**Given** validation failures  
**When** a write DTO is invalid  
**Then** the response uses 400 with machine-readable validation detail

### Story 5.3: Surface correlation ID in the Angular client

As a **signed-in user**,
I want **to see or copy a reference id when the server returns one with an error**,
So that **I can give it to someone troubleshooting (FR21)**.

**Acceptance Criteria:**

**Given** an API error payload including correlation/trace extension  
**When** the client shows the error state  
**Then** the reference id is visible or copyable in a non-technical way

**And** sensitive headers are never logged to console in the client build (defense in depth)

---

## Final validation report

**Date:** 2026-03-28  
**Validator:** Epics & stories workflow — Step 4

### 1. FR coverage

| FR | Covered by story evidence |
|----|---------------------------|
| FR1 | Epic 2: 2.2 (credentials), 2.3 (login issues session) |
| FR2 | Epic 2: 2.4 |
| FR3 | Epic 2: 2.3 (refresh API), 2.6 (client silent refresh) |
| FR4 | Epic 2: 2.4, 2.5 |
| FR5–FR10 | Epic 3: 3.2, 3.3, 3.4 |
| FR11 | Epic 3: 3.1, 3.2 |
| FR12 | Epic 3: 3.5 |
| FR13–FR14 | Epic 4: 4.3; FR14 also 3.2, 4.5 |
| FR15 | Epic 4: 4.1 |
| FR16 | Epic 4: 4.4 |
| FR17 | Epic 4: 4.5 |
| FR18 | Epic 2: 2.5; Epic 4: 4.2 (gate interplay) |
| FR19 | Epic 4: 4.2 |
| FR20 | Epic 4: 4.1 (degraded load); Epic 5: 5.2 |
| FR21 | Epic 5: 5.1, 5.2, 5.3 |
| FR22 | Epic 3: 3.6 |

**Result:** All **FR1–FR22** trace from the inventory through the **FR coverage map** into at least one story with testable acceptance criteria.

### 2. Architecture alignment

- **Starter template:** Architecture specifies **brownfield in-place migration**, not greenfield `dotnet new`. **Epic 1 Story 1** correctly **does not** claim “clone starter”; it is **retarget + green build** — **pass**.
- **Schema discipline:** **Users** appear in **2.1** migration scope; **Todos** in **3.1** only — **pass** (no “all tables in story 1”).
- **Auth, API, logging:** Reflected in Epics **2**, **3**, **5** and **Additional Requirements** inventory — **pass**.

### 3. Story quality

- Stories use **As a / I want / So that** and **Given / When / Then** (with **And** where needed).
- Sizing targets **single-agent** completion; epics decomposed into **26** stories total.
- **UX-DR1–UX-DR5** addressed in stories called out in Epic **1.6**, **2.5**, **2.6**, **3.4**, **4.4**, **5.2** — **pass**.

### 4. Epic structure

- Epics are **user- and operator-outcome** oriented; **Epic 1** carries **platform/README** scope explicitly (PRD MVP **#1**, **#6**).
- **Cross-epic order:** **1 → 2 → 3 → 4 → 5** is the natural implementation sequence; **Epic 5** global error polish may **overlap late in Epic 2–3** but Epic **2** remains shippable with basic errors before **5.2** — **acceptable** (no hard reverse dependency).

### 5. Within-epic dependencies (spot check)

- **Epic 2:** 2.1 → 2.2 → 2.3 → 2.4 / 2.5 / 2.6 builds on prior (client stories assume APIs exist) — **pass**.
- **Epic 3:** 3.1 → 3.2 → 3.3 → 3.4–3.6 — **pass**.
- **Epic 4:** 4.1–4.5 order logical; 4.3 assumes authenticated shell from **4.2** — **pass**.
- **Epic 5:** 5.1 → 5.2 → 5.3 — **pass**.

### 6. Residual notes (non-blocking)

- **Rate limiting** (NFR-S3): Cited in **2.3**; verify implementation choice in dev vs prod configs during build.
- **“Production-oriented”** error shape: align **launch profile / environment** flags with **5.2** ACs so Development can still show richer diagnostics if desired.

**Overall:** **Ready for development** pending product owner spot-check of story order in the sprint backlog.

---

## Epic 6: Test coverage for API and Angular client

After this epic, the project has **automated test suites** for both **.NET API** (xUnit integration + unit tests) and **Angular client** (Jasmine/Karma unit + component tests), giving developers **confidence** that existing features work after changes and providing a **safety net** for future development.

**FRs covered:** Cross-cutting — validates FR1–FR22 implementations via automated tests.

**Implementation notes:** .NET tests use `WebApplicationFactory<T>` with in-memory SQLite for integration tests; Angular tests use Jasmine/Karma with `HttpClientTestingModule` for service tests and `TestBed` for component tests. Existing broken spec files are updated for Angular 19 patterns.

### Story 6.1: .NET test project setup and API integration tests

As a **developer**,
I want **integration tests for auth, todo CRUD, and settings API endpoints**,
So that **I can verify the full request pipeline works correctly after changes**.

**Acceptance Criteria:**

**Given** a new xUnit test project with `WebApplicationFactory`  
**When** I run `dotnet test`  
**Then** integration tests exercise login, refresh, logout, todo CRUD (create/list/update/delete/complete), and settings endpoints against an in-memory SQLite database

**Given** auth-protected endpoints  
**When** tests call them without a token  
**Then** tests verify 401 is returned

**Given** valid credentials  
**When** tests login and use the returned token  
**Then** subsequent API calls succeed with correct data

### Story 6.2: .NET unit tests for services and middleware

As a **developer**,
I want **unit tests for PasswordService, DbSeeder, and middleware components**,
So that **I can verify business logic in isolation without HTTP overhead**.

**Acceptance Criteria:**

**Given** PasswordService  
**When** I hash and verify a password  
**Then** tests confirm correct verification and rejection of wrong passwords

**Given** DbSeeder  
**When** run against an empty database  
**Then** tests confirm demo users are created with hashed passwords

**Given** DbSeeder run twice  
**When** demo users already exist  
**Then** tests confirm no duplicates are created (idempotency)

### Story 6.3: Angular test infrastructure modernization

As a **developer**,
I want **the Angular test runner to work with Angular 19 and current Karma/Jasmine**,
So that **I have a working foundation before writing new tests**.

**Acceptance Criteria:**

**Given** the existing `karma.conf.js` and `test.ts`  
**When** updated for Angular 19 zone.js import paths and coverage reporters  
**Then** `ng test --watch=false` completes without infrastructure errors

**Given** existing legacy spec files  
**When** they use deprecated `async()` from `@angular/core/testing`  
**Then** they are updated to `waitForAsync()` (Angular 19 pattern)

### Story 6.4: Angular service and guard unit tests

As a **developer**,
I want **unit tests for AuthService, TodoService, SettingsService, authGuard, and authInterceptor**,
So that **core client logic is verified independently of UI rendering**.

**Acceptance Criteria:**

**Given** AuthService  
**When** tested  
**Then** login/logout/refresh/session management behaviors are verified with mocked HTTP

**Given** TodoService  
**When** tested  
**Then** CRUD methods make correct HTTP calls with expected URLs and payloads

**Given** authGuard  
**When** tested with and without a token  
**Then** navigation is allowed/blocked correctly

**Given** authInterceptor  
**When** tested  
**Then** Bearer header attachment and 401 retry logic are verified

### Story 6.5: Angular component tests

As a **developer**,
I want **component tests for TodoListComponent, LoginComponent, NavMenuComponent, and AppComponent**,
So that **UI rendering and user interactions are verified**.

**Acceptance Criteria:**

**Given** TodoListComponent  
**When** tested  
**Then** it renders todos, handles add/delete/toggle actions, shows empty state and loading spinner

**Given** LoginComponent  
**When** tested  
**Then** it renders the login form, submits credentials, shows errors on failure

**Given** NavMenuComponent  
**When** tested  
**Then** it shows/hides authenticated links based on auth state

---

**Epics & stories workflow:** Steps **1–4** complete; document ready for backlog and implementation.
