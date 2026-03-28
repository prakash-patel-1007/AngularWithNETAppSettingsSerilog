---
stepsCompleted:
  - 1
  - 2
  - 3
  - 4
  - 5
  - 6
  - 7
  - 8
inputDocuments:
  - _bmad-output/planning-artifacts/prd.md
workflowType: architecture
lastStep: 8
status: complete
completedAt: 2026-03-28
project_name: AngularWithNETAppSettingsSerilog
user_name: Prakash
date: 2026-03-28
---

# Architecture Decision Document

_This document builds collaboratively through step-by-step discovery. Sections are appended as we work through each architectural decision together._

## Project Context Analysis

### Requirements overview

**Functional requirements (22, PRD):** Capabilities cover **visitor → authenticated session** (including **session continuation** per policy—FR3), **sign-out**, **TODO CRUD**, **complete + visible completion time**, **list refresh**, **per-user task association** when configured, **permission-gated features**, **non-secret SPA configuration**, **shared-demo transparency**, **optional demo experiences**, **public vs authenticated routing**, **safe errors** with optional **correlation IDs**, and **durable task state** for per-user deployments.

**Non-functional requirements (PRD):** Drive **sub-3s interactive feel** (non-SLA), **fail-fast** persistence setup, **no credential/token logging**, **secrets from environment**, **abuse resistance on auth**, **parameterized data access**, **refresh storage/validation/rotation** without coupling FR wording to one library, **SQLite sizing** and **operator backup** documentation, **MVP a11y basics** with **Growth WCAG 2.1 AA** path, **no mandatory external SaaS**, and **consistent diagnosable logs** with **correlation** where feasible.

**Scale & complexity**

- **Primary domain:** Full-stack **web** (SPA + REST API).
- **Complexity:** **Medium**—brownfield **platform upgrade**, new **persistence and auth** patterns, **TODO vertical slice**, **OSS/public-repo** constraints; **not** regulated high-compliance or massive scale-out in MVP.
- **Rough component areas (for later decisions):** **Client app**, **API host**, **auth/session**, **TODO domain**, **user/identity + refresh persistence**, **observability**, **build/deploy/docs**.

### Technical constraints & dependencies

- **PRD / frontmatter:** **SQLite** for application data; **refresh tokens** MVP-aligned with documented **storage tradeoffs** (e.g. httpOnly cookie vs client storage).
- **Brownfield:** Existing **Angular 11 + .NET Core 3.1** baseline to be **replaced or migrated** to agreed **target** stacks (exact versions are architecture/implementation decisions, not repeated here).
- **Deployment:** **Writable database path**; **environment-based** signing/secret configuration for non-dev environments.

### Cross-cutting concerns

- **Authentication & session:** Access vs refresh lifecycle, **rotation**, **interceptor/retry** behavior on client, **401/403** semantics.
- **Security:** **Rate limiting** (or equivalent) on credential endpoints, **safe error responses**, **no secret leakage** in logs or repo defaults.
- **Observability:** **Structured/consistent logging**, **correlation IDs**, **cross-platform log paths**.
- **Data:** **Migrations**, **seed data** for demos, **user-scoped vs shared** deployment semantics.
- **Developer experience:** **README map**, **proxy/dev** and **production static** serving, **CORS**, optional **CI** (Growth).

## Starter Template Evaluation

### Primary technology domain

**Full-stack web — brownfield:** **Angular** SPA (existing `ClientApp`) + **ASP.NET Core** HTTP API (existing host), upgraded in place per PRD. **Not** greenfield `ng new` + discard legacy tree.

### Starter options considered

| Option | Verdict |
|--------|--------|
| **Greenfield `ng new` + new API** | **Rejected** for this effort — unnecessary rewrite vs PRD “modernize brownfield.” |
| **Third-party full-stack starter (e.g. alternate stack + Angular)** | **Rejected** — wrong codebase and learning surface for this repo. |
| **Microsoft Angular SPA template via CLI** | **Not relied on** — SPA templates are **not** the default `dotnet new` story for current SDKs; **VS** or **manual** layout is typical. Use docs only for **pattern** comparison. |
| **In-place migration (`ng update` / major migration guides + .NET retarget)** | **Selected** — preserves structure, matches PRD acceptance and journeys. |

### Selected approach: brownfield migration baseline

**Rationale:** Delivers **PRD** outcomes (upgrade, SQLite, TODO, auth+refresh, Serilog, OSS) **without** throwing away the existing solution. Aligns with **Alex** (clone → run → understand) and **Morgan** (deploy with writable DB + env secrets).

**Version targets (verify at implementation time):**

- **.NET:** Prefer **.NET 10 (LTS)** for longest support window (confirm patch on [Microsoft .NET download / support](https://dotnet.microsoft.com/download)); **.NET 8 LTS** acceptable if policy requires it until **Nov 2026** EOS.
- **Angular:** Move toward a **currently supported** major (per [angular.dev/releases](https://angular.dev/reference/releases)); pin exact **minor/patch** in `package.json` / lockfile when executing.

**Initialization / first implementation stories (no single greenfield command):**

1. **Branch + baseline** — record current **green** build (or gap list) for API + `ClientApp`.
2. **Lock targets** — `TargetFramework` / SDK + Angular major in ADR or implementation plan.
3. **Backend** — retarget host, replace obsolete middleware with **minimal hosting + JWT Bearer**, add **EF Core + SQLite**, migrations, refresh persistence.
4. **Frontend** — follow **Angular update** / migration docs major-by-major (or approved jump), add **HTTP interceptor** (refresh), TODO feature module/routes.
5. **Verify** — PRD **acceptance bundle** (login → TODO → complete → survives refresh).

**Illustrative audit commands (run after targets are chosen):**

```bash
dotnet --version
node --version
cd ClientApp && npx ng version
```

**Architectural decisions this “starter” implies**

- **Language & runtime:** **C#** on chosen **.NET** LTS; **TypeScript** on chosen **Angular** toolchain.
- **Styling / UI:** **Existing** Bootstrap or successor chosen during migration (not fixed by an external starter).
- **Build:** **Angular CLI** application build; **.NET** SDK build/publish for API + static `wwwroot` or `ClientApp/dist` hosting pattern (decision in later steps).
- **Tests:** **Optional** in MVP; add **xUnit** / **Karma or Vitest/Jest** per Growth—not mandated by a starter.
- **Code organization:** **Keep** solution-relative `ClientApp` unless ADR moves it; **feature folders** for TODO and auth on both sides.

**Note:** The first implementation work is **migration and vertical slice**, not running a deprecated **`dotnet new angular`** scaffold.

## Core Architectural Decisions

### Decision priority analysis

**Critical (block implementation if unset):**

- **Runtime targets:** .NET **10 (LTS)** preferred; Angular **current supported major** (pin at implementation).
- **Persistence:** **SQLite** + **EF Core** with **migrations**; **fail-fast** if DB path not writable.
- **Auth:** **JWT access tokens** + **refresh tokens** stored server-side (hashed identifiers), **rotation on refresh**; passwords **hashed** (e.g. ASP.NET Core PasswordHasher or vetted library); **official JWT Bearer** validation on API—not custom middleware only.
- **Hosting split:** **Dev** = API + Angular dev server + **proxy**; **Prod** = API serves **built SPA** or reverse proxy serves static + forwards `/api` (exact layout ADR’d during implementation).

**Important (shape the system):**

- **REST** JSON API; **controllers** or **minimal APIs** grouped by feature (**Auth**, **Todos**, **Settings**).
- **Client:** **HTTP interceptor** for **Bearer** + **one retry after silent refresh**; **route guards** for authenticated routes; **permission** model aligned with FR13 (claims/roles from token or profile endpoint—pick one pattern and document).
- **Errors:** **Consistent JSON error shape** + **correlation id** in logs and optionally in response body/headers.
- **Logging:** **Serilog** sinks: **console** + **rolling file** under repo-relative or configurable path; enrichers as needed; **no token logging**.

**Deferred (post-MVP / Growth):**

- **CI/CD** provider and **container** image (PRD Growth).
- **OIDC / social login** (Vision).
- **Centralized metrics/tracing** (beyond logs).

### Data architecture

| Decision | Choice | Rationale |
|----------|--------|-----------|
| Database | **SQLite** file | PRD; single-node, no separate DB server for sample. |
| ORM | **EF Core** | Migrations, relational model for users, refresh sessions, todos. |
| Modeling | **Relational tables** with **FKs** | Clear sample; supports per-user TODO scope. |
| Migrations | **EF migrations** checked in; `Database.Migrate()` or documented `dotnet ef` | PRD schema discipline. |
| Validation | **Server-side** validation on write DTOs + **Problem Details** | Security and consistent API. |
| Caching | **None** in MVP | Simplicity; add if profiling demands. |

### Authentication & security

| Decision | Choice | Rationale |
|----------|--------|-----------|
| Credential login | **User/password** against SQLite | PRD MVP; demo users seeded. |
| Password storage | **Strong hash** (built-in hasher or BCrypt) | NFR-S4 / public repo hygiene. |
| Access token | **JWT** short-lived | Standard API auth. |
| Refresh token | **Opaque** stored in DB, **hashed**, **rotated** | PRD `technicalDecisions.refreshTokens`. |
| Transport | **HTTPS** in production configs | Baseline. |
| Abuse | **Rate limiting** on login/refresh | NFR-S3. |
| Secrets | **Environment / user secrets** for signing keys | NFR-S2; nothing production-grade in git. |

### API & communication patterns

| Decision | Choice | Rationale |
|----------|--------|-----------|
| Style | **REST** over HTTP/JSON | PRD web app; simple for sample. |
| Versioning | **Unversioned** URIs for MVP (`/api/...` or controller routes) | Small surface; version later if needed. |
| Documentation | **README + inline XML** optional; OpenAPI **Growth** | Time-box MVP. |
| Errors | **RFC 7807 Problem Details**-compatible JSON where practical + **correlation id** | FR20–FR21, NFR-S6. |
| CORS | **Explicit allowed origins** (dev: localhost Angular port) | PRD web requirements. |

### Frontend architecture

| Decision | Choice | Rationale |
|----------|--------|-----------|
| Structure | **Feature-oriented** folders (auth, todo, shell, shared) | Teachable layout. |
| Components | **Angular** components; prefer **standalone** if targeting modern Angular | Reduces NgModule boilerplate on new majors. |
| State | **Services + `HttpClient`** for MVP; **signals**/**RxJS** per team taste | Avoid heavy global store until needed. |
| Routing | **Angular Router** + **guards** for auth/permissions | FR18–FR19. |
| Styling | **Bootstrap** or migrate to **utility CSS** in same effort as Angular upgrade | Keep visual continuity unless UX spec says otherwise. |

### Infrastructure & deployment

| Decision | Choice | Rationale |
|----------|--------|-----------|
| Dev | **Two processes** (or script): `dotnet watch` + `ng serve` + **proxy.conf** | PRD developer journey. |
| Prod | **Single deployable** hosting API + static files **or** split static + API behind proxy | Morgan journey; document both. |
| Config | **appsettings** + **environment** + **secrets** | Standard ASP.NET. |
| Logs | **Serilog** file path **configurable** and **cross-platform** | NFR-O1, PRD. |
| Scale | **Single instance** + SQLite | PRD NFR-SC1. |

### Decision impact analysis

**Suggested implementation sequence**

1. Retarget **.NET** host; **Serilog** + config cleanup; **remove** obsolete SPA middleware patterns.  
2. Add **EF Core + SQLite**, **identity/user** + **refresh token** entities, **migrations**, **seed**.  
3. Implement **JWT + refresh** endpoints; **remove** hard-coded users.  
4. Add **Todo** API + authorization.  
5. Upgrade **Angular**; **interceptor**, **TODO** UI, **settings** client.  
6. **README**, **rename**, **acceptance bundle**.

**Cross-component dependencies**

- Refresh storage ↔ **interceptor** retry ↔ **login** response contract.  
- **Per-user TODOs** ↔ **user id** on Todo entity ↔ **token claims** mapping.  
- **Static hosting** ↔ **Angular `base href`** ↔ **API fallback** to `index.html` for deep links.

## Implementation Patterns & Consistency Rules

### Pattern categories defined

**Conflict risks addressed:** **~12** areas (DB/EF names, REST paths, JSON casing, dates, Angular files, errors, logging, auth headers, tests location).

### Naming patterns

**Database (EF Core / SQLite)**  

- **Tables:** PascalCase plural entity names mapped to convention—use **`Todos`**, **`Users`**, **`RefreshTokens`** (configure singular/plural in `DbContext` once; **do not** mix `todo` vs `Todo` ad hoc).  
- **Columns:** **PascalCase** in C# properties; DB columns follow EF defaults unless a single global snake_case convention is chosen—**default: PascalCase columns** for SQLite simplicity in this sample.  
- **FKs:** `NavigationPropertyId` pattern (e.g. `UserId` on `Todo`).  
- **Indexes:** `IX_Table_Column` via EF Fluent API when explicit.

**REST API**  

- **Resources:** **Plural** nouns: `/api/todos`, `/api/auth/login`, `/api/auth/refresh` (or `/api/security/...` if retaining legacy segment—**pick one root and keep**).  
- **Route params:** `{id}` (ASP.NET style).  
- **Query params:** **camelCase** (`isCompleted`), matching JSON.

**C# code**  

- Types, methods, public members: **PascalCase**. Private fields: `_camelCase`. Async methods end with **`Async`** when they await.

**Angular**  

- **Files:** **kebab-case** (`todo-list.component.ts`).  
- **Classes:** **PascalCase**. **Selectors:** `app-todo-list`. **Templates:** prefer **async pipe** or explicit `*ngIf` loading flags named **`isLoading`**, **`errorMessage`**.

### Structure patterns

**Backend**  

- **Feature folders** under `Features/` or `Modules/`: `Auth/`, `Todos/`, `Settings/`—each with **Endpoints/Controllers**, **DTOs**, **Validators** if needed.  
- **Data:** `Data/AppDbContext.cs`, `Migrations/`, `Configurations/` (optional).  
- **Shared:** `Infrastructure/` (Serilog, correlation id middleware), `Common/Exceptions` only if kept small.

**Frontend**  

- Under `ClientApp/src/app/`: **`features/todo`**, **`features/auth`**, **`core/`** (interceptors, guards, services), **`shared/`** (ui widgets).  
- **Tests:** co-located `*.spec.ts` next to source (Angular default) unless project switches to `__tests__`—**prefer default**.

### Format patterns

**JSON**  

- **camelCase** property names in API JSON (ASP.NET default `PropertyNamingPolicy.Json`).  
- **Dates:** **ISO 8601** UTC strings in JSON (e.g. `completedAt: "2026-03-28T12:00:00Z"`).  
- **Booleans:** JSON `true`/`false`.  
- **IDs:** **GUID strings** for Todo id unless int chosen—**document one type** in OpenAPI/README.

**API success body**  

- **Direct** DTO or array for MVP—**no** mandatory `{ data: ... }` wrapper unless added consistently everywhere.

**API errors**  

- **Problem Details** shape: `type`, `title`, `status`, `detail`, `instance`; include **`traceId`** or **`correlationId`** extension member when available.  
- **Status codes:** **401** unauthenticated, **403** forbidden, **404** missing resource, **400** validation, **429** rate limit, **500** unexpected (no stack in body in prod).

### Communication patterns

**Auth headers**  

- **`Authorization: Bearer <access_token>`** only for API calls; refresh via **dedicated POST** body or **httpOnly cookie**—**document chosen pattern** in README; **never** send refresh token in URL query.

**Client retry**  

- On **401** from API: **one** refresh attempt; if refresh fails, **clear session** and **navigate to login**—no infinite loops.

**Logging**  

- **Structured** properties: `CorrelationId`, `UserId` (if resolved), `Path`, `ElapsedMs`—**never** log tokens or passwords.

### Process patterns

**Validation**  

- **Server:** validate all write models; return **400** with validation problem details.  
- **Client:** minimal UX validation + trust server for security rules.

**Loading / errors (Angular)**  

- Per-feature **`isLoading`** / **`error`**; avoid silent failures—surface **actionable** message for network errors.

### Enforcement guidelines

**All implementers MUST:**  

- Follow **REST plural** paths and **camelCase** JSON unless an ADR changes it.  
- Use **EF migrations** for any schema change; **no** hand-edited production DB without migration.  
- Add **correlation id** middleware early in pipeline; propagate to Serilog.  
- Keep **secrets** out of source; use **env / user-secrets** for JWT signing.

**Verification:** PR review + optional **analyzer** / **eslint** rules; README **“conventions”** subsection links here.

### Examples

**Good:** `GET /api/todos` → `[{ "id": "…", "title": "x", "isCompleted": true, "completedAt": "…" }]`.  
**Avoid:** Mixed `/Todo` vs `/todos`; PascalCase JSON from API without global serializer config; logging `Authorization` header value.

**Good:** `POST /api/auth/refresh` with body `{ "refreshToken": "…" }` **only if** not using httpOnly cookie pattern—**pick one** and delete the other from docs.

## Project Structure & Boundaries

### Complete project directory structure

Target layout after migration (root = solution folder `AngularWithNETAppSettingsSerilog/`). **Existing** paths are noted; **new** paths are additions.

```text
AngularWithNETAppSettingsSerilog/
├── README.md                          # Dev/prod map, env vars, DB path, conventions link
├── .gitignore
├── .github/
│   └── workflows/
│       └── ci.yml                     # Growth: optional build/test
├── appsettings.json                   # existing — non-secret defaults
├── appsettings.Development.json
├── Properties/
│   └── launchSettings.json            # existing — URLs, env
├── {HostProject}.csproj               # rename from AngularWithNET.csproj when rebranding
├── Program.cs                         # host bootstrap, Serilog, pipeline
├── Startup.cs                         # remove when on minimal-host-only template, or fold into Program
│
├── Data/
│   ├── AppDbContext.cs
│   ├── Configurations/                # optional: IEntityTypeConfiguration<>
│   └── Migrations/                    # EF Core migrations (checked in)
│
├── Features/
│   ├── Auth/
│   │   ├── AuthController.cs          # or MinimalApis/AuthEndpoints.cs
│   │   ├── Dtos/
│   │   └── Services/                  # token issuance, refresh rotation
│   ├── Todos/
│   │   ├── TodosController.cs
│   │   ├── Dtos/
│   │   └── Services/
│   └── Settings/
│       ├── SettingsController.cs      # non-secret SPA config / health
│       └── Dtos/
│
├── Domain/                            # optional: entities only (Todo, User, RefreshToken)
│   ├── Todo.cs
│   ├── User.cs
│   └── RefreshToken.cs
│
├── Infrastructure/
│   ├── SerilogConfiguration.cs        # optional helper
│   ├── CorrelationIdMiddleware.cs
│   └── RateLimiting/                  # login/refresh policies
│
├── Controllers/                       # legacy — migrate into Features/* then remove
├── ViewModel/                         # legacy — replace with Features/*/Dtos
├── Pages/                             # existing Error.cshtml — keep or move to wwwroot/errors
├── wwwroot/                           # prod: Angular build output (or dist copy from ClientApp)
│
├── ClientApp/                         # existing Angular workspace
│   ├── angular.json
│   ├── package.json
│   ├── proxy.conf.json                # dev: forward /api to Kestrel
│   ├── src/
│   │   ├── main.ts
│   │   ├── index.html
│   │   ├── environments/
│   │   │   ├── environment.ts
│   │   │   └── environment.prod.ts
│   │   └── app/
│   │       ├── app.component.ts
│   │       ├── app.routes.ts          # or legacy app-routing.module.ts until migrated
│   │       ├── core/
│   │       │   ├── interceptors/
│   │       │   │   └── auth.interceptor.ts
│   │       │   ├── guards/
│   │       │   │   └── auth.guard.ts
│   │       │   └── services/
│   │       │       ├── auth.service.ts
│   │       │       └── api-base.service.ts
│   │       ├── features/
│   │       │   ├── auth/
│   │       │   │   └── login/
│   │       │   └── todo/
│   │       │       ├── todo-list/
│   │       │       └── todo-detail/
│   │       ├── shared/                # dumb UI, pipes
│   │       ├── nav-menu/              # existing — fold into shell or shared
│   │       ├── login/                 # existing — migrate into features/auth
│   │       └── app-settings/          # existing — align with Settings API
│   └── e2e/                           # Growth: Playwright/Cypress later
│
├── tests/                             # Growth: optional xUnit integration
│   └── AngularWithNET.Tests/
│       └── Integration/
│
├── logs/                              # .gitignore — Serilog rolling files (configurable path)
└── app.db                             # .gitignore — SQLite file (path configurable)
```

### Architectural boundaries

**API boundaries**

- **Public:** `POST /api/auth/login`, `POST /api/auth/register` (if added), `GET /api/settings` (non-secret only), health if exposed.
- **Authenticated:** `GET/POST/PATCH/DELETE /api/todos` (and sub-routes) — **JWT Bearer** required; **user id** from claims, never from client-supplied body for authorization.
- **Refresh:** `POST /api/auth/refresh` — **rate-limited**; accepts refresh token per chosen storage pattern (body vs httpOnly cookie); returns new pair; **rotates** server-side record.
- **Internal:** **EF Core** only inside feature services/repositories; **no** raw SQL except rare migrations/seed if documented.

**Component boundaries**

- **Angular shell** (`app.component`, nav): layout and router-outlet; **no** TODO business logic.
- **Feature components** talk to **`HttpClient` via feature or core services** only; **no** direct `fetch` scattered across app.
- **Auth state:** owned by **`AuthService`** + interceptor; routes use **guards**; **no** duplicate token parsing in every component.

**Service boundaries**

- **API host** is single process for MVP; **SQLite** file is **exclusive** to that process (single-instance assumption).
- **Angular dev server** proxies **`/api`** to API; **production** either API serves SPA static files or reverse proxy splits static vs `/api`.

**Data boundaries**

- **SQLite schema** owned by **EF migrations**; tables **`Users`**, **`RefreshTokens`**, **`Todos`** (names per implementation patterns).
- **Refresh tokens:** **opaque**, **hashed at rest**; **no** plaintext persistence.
- **Caching:** none MVP; **optional** later at API edge only with explicit invalidation story.

### Requirements to structure mapping

**Feature / FR mapping (PRD-aligned)**

| Area | Location |
|------|----------|
| Login, refresh, sign-out | `Features/Auth/*`, `ClientApp/src/app/features/auth/`, `core/interceptors`, `core/guards` |
| TODO CRUD + completion timestamp | `Features/Todos/*`, `ClientApp/src/app/features/todo/*` |
| Per-user tasks | `Domain/Todo.cs` (`UserId`), `AppDbContext` query filters / service checks |
| Non-secret SPA settings | `Features/Settings/*`, `ClientApp` `environment*.ts` + optional `/api/settings` |
| Permission-gated UI | `ClientApp` directive/guard (existing pattern migrates to `core/` or `shared/`) |
| Safe errors + correlation | `Infrastructure/CorrelationIdMiddleware.cs`, Problem Details in API, Serilog enrichers |
| Observability | `Program.cs` + Serilog config, `logs/` |
| Demo seed users | `Data/Seeding/` or `Program.cs` guarded seed |

**Cross-cutting**

| Concern | Location |
|---------|----------|
| JWT validation | `Program.cs` / `AddAuthentication().AddJwtBearer` |
| Rate limiting | `Infrastructure/RateLimiting` + endpoint metadata |
| CORS | `Program.cs` — explicit origins |
| Global exception → Problem Details | middleware or `IExceptionHandler` (.NET 8+) |

### Integration points

**Internal**

- Browser → **Angular** → **`/api/*`** (proxy dev, same origin or CORS prod).
- **Auth interceptor** attaches **Bearer**; on **401**, **one** refresh via **AuthService** then retry original request.
- **EF Core** invoked from **Auth/Todos** services; **DbContext** scoped per request.

**External**

- None required MVP (no mandatory SaaS). **Growth:** OIDC, OpenAPI UI, CI.

**Data flow**

1. Login → API validates credentials → issues **access + refresh** → client stores per ADR (memory + secure storage vs cookie).
2. TODO writes → API validates JWT → maps **user id** → EF **SaveChanges** → SQLite.
3. Logs + correlation id flow **request → middleware → Serilog**; never log secrets.

### File organization patterns

**Configuration**

- **ASP.NET:** `appsettings*.json`, environment variables, user secrets for dev signing keys.
- **Angular:** `environment.ts` / `environment.prod.ts` for **public** API base URL only.

**Source**

- **Backend:** feature-first under `Features/`; legacy `Controllers/` + `ViewModel/` retired after cutover.
- **Frontend:** `core` vs `features` vs `shared` as above; new work in `features/`; migrate `login`, `home`, `fetch-data` sample code out or replace.

**Tests**

- **Angular:** co-located `*.spec.ts` (default).
- **.NET:** optional `tests/*.Tests` project; integration tests hit **TestServer** + in-memory or temp SQLite.

**Assets**

- **Angular build** → `wwwroot/` or `ClientApp/dist` per chosen publish script; `angular.json` `outputPath` aligned with `.csproj` copy targets.

### Development workflow integration

**Development**

- Terminal 1: `dotnet watch run` from solution root.
- Terminal 2: `npm start` / `ng serve` in `ClientApp` with **proxy** to API port.
- SQLite path writable relative to content root or `%TEMP%` for local experiments (document in README).

**Build**

- `dotnet publish` runs **Angular production build** (MSBuild target) or CI runs `ng build` then `dotnet publish`—**one** documented path.

**Deployment**

- Single folder: **API + wwwroot** + **writable** path for `app.db` and `logs/`; environment sets **JWT key**, **connection string** or DB path, **allowed CORS origins**.

## Architecture validation results

### Coherence validation

**Decision compatibility**

- **.NET LTS + Angular (supported major) + EF Core SQLite + JWT + refresh** are a standard, compatible stack; no conflicting persistence or auth choices.
- **Dev proxy + prod static/API** matches SPA hosting decisions; **single SQLite file** aligns with single-instance scale (NFR-SC1).
- **Refresh rotation + interceptor single-retry** matches security and client patterns; **httpOnly vs body** is explicitly flagged as a single choice to lock at implementation.

**Pattern consistency**

- Implementation patterns (plural REST, camelCase JSON, Problem Details, EF table naming) align with core decisions and ASP.NET defaults.
- Structure patterns (`Features/`, `Data/`, `ClientApp` feature folders) match the documented migration path from legacy `Controllers/` / `ViewModel/`.

**Structure alignment**

- Project tree maps **Auth, Todos, Settings, infrastructure, and client features** to concrete paths; boundaries (public vs authenticated API, EF-only data access) support the decisions above.

### Requirements coverage validation

**Functional requirements (PRD)**

- **Auth/session/refresh/sign-out, TODO CRUD + completion time, per-user scope, settings, routing/guards, permission UI, errors/correlation, durable state** are all mapped to API + Angular locations in this document and the PRD.
- **Shared-demo / optional demo** themes are supported via seed data and settings endpoints as described in structure mapping.

**Non-functional requirements**

- **Performance feel, fail-fast DB, no secret logging, env secrets, auth abuse resistance, parameterized access, SQLite ops, a11y baseline, optional SaaS, structured logs** are addressed in decisions + patterns + infrastructure section.

### Implementation readiness validation

**Decision completeness**

- Critical paths (runtime targets, SQLite + migrations, JWT + refresh storage tradeoff, hosting) are documented; exact **minor/patch** pins remain for implementation-time lock (called out in starter evaluation).

**Structure completeness**

- Target directories, legacy vs new layout, build/deploy/dev workflow, and integration points are specified concretely for this repository—not a generic placeholder.

**Pattern completeness**

- Naming, JSON, errors, logging, auth headers, client retry, and validation process patterns are documented with examples and anti-patterns.

### Gap analysis results

**Critical gaps**

- None blocking: **refresh token transport** (cookie vs JSON body) must be chosen once and reflected in README + OpenAPI when added.

**Important gaps**

- **OpenAPI/Swagger** deferred to Growth—acceptable if README documents representative routes and DTOs.
- **Exact .NET SDK / Angular major.minor** to be pinned in `csproj` / `package.json` at kickoff.

**Nice-to-have**

- Optional **xUnit integration tests** and **e2e** paths are noted but not required for architecture completeness.

### Validation issues addressed

- **Legacy vs new API route root** (`/api/security/...` vs `/api/auth/...`): architecture requires **one** consistent root; implementation should migrate to plural REST under `/api/auth` and `/api/todos` per patterns.
- **Startup.cs vs minimal hosting**: tree allows either during migration; final state should be **one** clear entry (`Program.cs` only preferred on modern templates).

### Architecture completeness checklist

**Requirements analysis**

- [x] Project context analyzed
- [x] Scale and complexity assessed
- [x] Technical constraints identified
- [x] Cross-cutting concerns mapped

**Architectural decisions**

- [x] Critical decisions documented (with version strategy)
- [x] Technology stack specified
- [x] Integration patterns defined
- [x] Performance and security considerations addressed

**Implementation patterns**

- [x] Naming conventions established
- [x] Structure patterns defined
- [x] Communication patterns specified
- [x] Process patterns documented

**Project structure**

- [x] Directory structure defined for this repo
- [x] Component boundaries established
- [x] Integration points mapped
- [x] Requirements-to-structure mapping complete

### Architecture readiness assessment

**Overall status:** **Ready for implementation**

**Confidence level:** **High** — stack, boundaries, and folder mapping are coherent; remaining items are explicit pin/version and refresh-transport choice.

**Key strengths**

- Brownfield-aware tree with clear retirement path for legacy folders.
- Security and observability woven through API, data, and client layers.
- PRD FR/NFR coverage traced to concrete components.

**Areas for future enhancement**

- OIDC, centralized tracing/metrics, CI/CD, OpenAPI, fuller automated test pyramid (Growth / Vision).

### Implementation handoff

**AI agent guidelines**

- Follow architectural decisions and implementation patterns in this document and `_bmad-output/planning-artifacts/prd.md`.
- Keep **secrets** out of source; use environment and user secrets for signing keys.
- Use **EF migrations** for all schema changes; **do not** log tokens or passwords.

**First implementation priorities**

1. Retarget host and tooling; wire **Serilog** and correlation id middleware.
2. Add **EF Core + SQLite**, entities, migrations, seed users.
3. Implement **JWT + refresh** (with chosen storage pattern) and **Todos** API.
4. Upgrade **Angular**; add **interceptor**, **TODO** UI, README and rename as planned.

---

## Architecture workflow completion

This architecture workflow is **complete** as of **2026-03-28**. The document above is the technical single source of truth for implementation consistency.

**Suggested next steps**

- Execute implementation using **`bmad-dev-story`**, **`bmad-quick-dev`**, or your normal sprint flow, with **`prd.md` + this `architecture.md`** as inputs.
- If you want BMad to suggest the next skill or artifact, use **`bmad-help`** in the repo.

Questions about this architecture document or how it maps to code changes are welcome.
