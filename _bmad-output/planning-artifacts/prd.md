---
stepsCompleted:
  - step-01-init
  - step-02-discovery
  - step-02b-vision
  - step-02c-executive-summary
  - step-03-success
  - step-04-journeys
  - step-05-domain
  - step-06-innovation
  - step-07-project-type
  - step-08-scoping
  - step-09-functional
  - step-10-nonfunctional
  - step-11-polish
  - step-12-complete
inputDocuments: []
documentCounts:
  briefCount: 0
  researchCount: 0
  brainstormingCount: 0
  projectDocsCount: 0
workflowType: "prd"
prdWorkflowCompletedAt: "2026-03-28"
classification:
  projectType: "Web application (SPA + API) — reference implementation with a functional TODO vertical slice"
  domain: "General — dual use: productivity TODO UI and developer education / sample"
  complexityDomain: "low"
  complexityTechnical: "medium"
  projectContext: "Brownfield codebase evolving into a shipped, documented, publishable artifact"
  audiences:
    - "Developers: clone, run, learn patterns, fork"
    - "End users: TODO features via a deployed instance (publisher-hosted or self-hosted)"
  priorityRule: "Primary: credible Angular + .NET reference with a real TODO slice. Secondary: usable TODO for people using an instance. When UX simplicity conflicts with showing patterns, keep the TODO path simple and teach via README, layout, and documentation."
  publishingIntent: "Intended for public distribution so anyone who needs the app or the sample can obtain it; specific channels and packaging to be defined in later PRD sections"
  elicitationApplied:
    - "First Principles Analysis (Advanced Elicitation) — accepted by user"
technicalDecisions:
  persistence: "Use SQLite for application data (e.g. identity/users and TODO records) instead of ad hoc text/JSON files. Rationale: structured storage, transactions, simpler querying, and less casual tampering than editable plain files — while remaining serverless-friendly and easy to ship for a sample/public repo. Note: SQLite still requires secure practices (parameterized SQL, file permissions, migrations, no production secrets in repo)."
  refreshTokens: "Implement refresh tokens alongside short-lived access tokens (JWT). Goals: reduce mid-session 401 surprises for TODO users (Jordan journey); demonstrate current auth patterns for developers (Alex journey). Requirements to specify in functional/technical sections: (1) refresh token issuance at login; (2) refresh endpoint (e.g. POST /security/refresh or /auth/refresh) validating stored refresh token; (3) rotation on use (new refresh token per refresh) recommended for sample quality; (4) persistence of refresh token metadata in SQLite (user id, token hash or identifier, expiry, optional revocation); (5) client: interceptor or auth service retries failed API calls once after silent refresh, or redirects to login if refresh fails; (6) document threat model for public sample — prefer httpOnly Secure cookie for refresh token where feasible vs localStorage tradeoff; never log refresh or access tokens. Scope: treat as MVP-aligned with auth modernization unless explicitly deferred."
productVision:
  vision: "One published web app with two clear stories: developers get a modern, inspectable Angular + .NET codebase to run and learn from; users get a direct TODO experience (manage tasks and completion) without needing to care about the stack."
  whatMakesItSpecial: "The same codebase earns trust with developers through structure and docs and with task users through a simple, serious TODO path — not by pretending both audiences read the same screens the same way."
  coreInsight: "Separate the promises, not necessarily the code — one repo, two honest entry narratives, shared implementation."
  elicitationApplied:
    - "User Persona Focus Group (Advanced Elicitation) — accepted by user"
---

# Product Requirements Document - AngularWithNETAppSettingsSerilog

**Author:** Prakash
**Date:** 2026-03-28

*Traceability for downstream work: **Executive Summary** → **Success Criteria** → **User Journeys** → **Functional / Non-Functional Requirements**.*

## Executive Summary

This initiative delivers a **single published web application** with **two explicit user stories**: **(1)** developers obtain a **current, inspectable Angular + ASP.NET** codebase they can **clone, run, and learn from**; **(2)** end users, on a **hosted or self-hosted instance**, get a **direct TODO workflow** (create, update, delete, complete with completion tracking) **without** needing stack knowledge. The work **modernizes a brownfield** Angular + .NET sample into a **credible reference** and a **usable task app**, with **clear documentation**, **observable logging** suitable for contributors, **security-minded defaults** appropriate for a **public repository**, and a **defined developer onboarding path** (exact tooling choices such as optional orchestration remain requirements detail). **Persistence** targets **SQLite** for structured, transactional storage of **users/identity and TODO data**—stronger integrity and operability than ad hoc text files while avoiding a separate database server for local and sample deployment.

### What Makes This Special

**One repository, two honest promises:** reference-quality **structure, patterns, and docs** for builders, and a **simple, serious TODO path** for task users—without pretending both audiences use the product the same way. **Core idea:** **separate the narratives, not necessarily the implementation**—shared code, **prioritized** so **sample credibility** and **reproducibility** lead, while the **TODO experience** stays **obvious and low-friction**. Compared with **disconnected tutorials** or **toy demos**, this aims for **runnable realism** (auth slice, data, logging) that still **ships** as something **others can publish, fork, or deploy**.

## Project Classification

| Dimension | Classification |
|-----------|----------------|
| **Project type** | **Web application** — SPA (Angular) + **REST API** (ASP.NET); **reference implementation** including a **functional TODO vertical slice**. |
| **Domain** | **General** — **dual use**: productivity **TODO** UI and **developer education** / sample. **Regulatory complexity: low.** |
| **Technical complexity** | **Medium** — major **framework upgrades**, **authentication and data** design, **logging and public-repo hygiene**, **developer experience**, optional **distribution/packaging** choices. |
| **Project context** | **Brownfield** codebase **evolving** toward **documented, publishable** artifacts for **broad access**. |
| **Key technical decision** | **SQLite** for application data (users, TODOs), with **parameterized access**, **migrations**, and **deployment-safe** handling of the database file. |

## Success Criteria

### User Success

- **Developer (sampler):** Following **only** the README (and linked prerequisites), a developer achieves **first successful API + UI load** and completes **sign-in plus one authenticated action** (e.g. load settings or TODO list) **without undocumented tribal steps**. A **~30 minute** target is **aspirational**; the **hard bar** is **repeatable pass/fail** on the documented checklist.
- **Developer:** Can locate **auth, TODO persistence, and logging** using README “map” pointers (folders/files), without reading the entire repository.
- **End user (TODO):** On a running instance, a signed-in user can **create, edit, delete, and mark tasks complete**; **completed items persist a completion timestamp**; flows remain understandable **without** framework knowledge.
- **Emotional bar:** Developers describe the repo as **runnable and honest**; task users describe the TODO path as **simple and predictable**.

### Business Success

- **Distribution:** Project is **publicly obtainable** (repository; optional releases/packages—exact channel in requirements). README covers **license, how to run, and security expectations** for a public sample.
- **Self-serve:** A **third party can run and explore the app without private hand-holding from the author**—README is the contract; optional **CONTRIBUTING** stub or **issue template** improves OSS hygiene.
- **Non-goal (MVP):** **Viral growth metrics** are **not** required for v1; focus is **credibility and reproducibility**.
- **Reputation:** No **known high-severity** pattern of **committed secrets** or **unsafe default public deploy** in shipped defaults and docs.

### Technical Success

- **Stack:** **Angular** and **.NET** upgraded to **agreed targets**; **build** (and **tests** where present) succeed on a **clean checkout** per documented environment; **optional CI green on main** is a **strong success signal** (see Growth).
- **Data:** **SQLite** stores **users/identity and TODOs** with an **agreed schema-change story**: **EF (or chosen) migrations** plus **documented commands** or **automated apply in dev**—so **clone → run** does not depend on undocumented DB setup.
- **Security (code-level):** **Secrets not committed**; auth uses **current patterns** (e.g. JWT configuration); **rate limiting or equivalent** on **login** considered; **production** error responses **do not expose stack traces**.
- **Logging:** **Serilog** (or agreed) → **console** for all contributors + **file** or agreed sinks with **cross-platform paths**; **no passwords or tokens** in logs.
- **Naming:** **Branding and project identity** avoid **obsolete version-specific** strings where avoidable.

### Measurable Outcomes

| Outcome | Signal |
|--------|--------|
| Reproducible dev | README checklist → **API + UI up** without extra steps |
| Acceptance bundle | **Seeded user** → login → **create TODO** → **complete** → **CompletedAt** set → **survives refresh** (manual or automated) |
| Public safety | Review/scan: **no production secrets** in default tree |
| Upgrade | **Clean checkout:** `dotnet build` + client build succeed |
| Schema discipline | Migrations (or documented apply) **documented and repeatable** |

## Product Scope

### MVP — Minimum viable product

**Order of execution (intent):**

1. **Upgrade** backend and frontend to agreed **.NET** and **Angular** targets; **one documented dev path** (e.g. API + client dev server with proxy or equivalent).
2. **SQLite + persistence** for **users** (hashed passwords) and **TODOs**; **migrations** and **documented** create/reset of dev DB.
3. **Auth:** Remove hard-coded credentials; **documented demo accounts** for local use.
4. **TODO vertical slice:** API + UI — **CRUD + complete** with **completion timestamp**.
5. **Logging:** Console + file (or agreed sinks), **public-repo-safe** defaults.
6. **README + rename:** Prerequisites, runbook, ports, **security notes**, **SQLite** path; **remove fixed version naming** from project/solution/UI where applicable.

### Growth features (post-MVP)

- **CI** (build/test on PR or main) — **recommended first Growth item** for fork/cloner confidence.
- **Optional orchestration** (e.g. Aspire or **one-command dev script**) if it **measurably** reduces setup friction.
- **Container** or **publish template** for one deployment target.
- **E2E** tests for **auth + TODO** acceptance bundle.

### Vision (future)

- **Hosted demo** with explicit **data/privacy** messaging.
- **Team TODOs**, **OIDC**, or **pluggable auth** only if aligned with dual **reference + product** story.

## User Journeys

### Journey 1 — Jordan: first productive afternoon (primary success)

**Opening:** Jordan needs a simple list for home projects. They receive a URL from **someone who deployed the app** (friend, IT, or self-hosted instructions)—README should state **who is responsible for hosting** and whether the instance is a **shared demo** or **per-user data**.

**Rising action:** Opens site; if **shared demo**, sees a **short, plain warning** (“Do not enter sensitive data”) when appropriate. **Signs in** with credentials from that environment’s documentation. Lands on **TODO** (or home then TODO). Adds “Buy paint,” marks another item **complete**, sees **completion date**.

**Climax:** Refreshes—**tasks and completion dates persist**. If data is **per-user**, Jordan only sees **their** items.

**Resolution:** Jordan uses the list daily without thinking about the stack.

**Failure / recovery:** Wrong password → **clear message** (no stack trace). **500** → safe user message; **correlation id** in logs for support.

### Journey 2 — Jordan: mistakes, session, and errors (edge cases)

**Wrong task completed (MVP):** Jordan **deletes** the mistaken item or **adds a replacement**—**completion is terminal** in MVP unless product explicitly adds **uncomplete**.  

**Access token expiry:** While editing, the **access JWT expires**. The client **silently refreshes** using the **refresh token** (policy in repository **frontmatter** under `technicalDecisions.refreshTokens`); API calls **retry once**. If refresh **fails** (revoked/expired), Jordan sees **“Session expired—sign in again”** and **unsaved form state** behavior is defined (best-effort preserve or clear—document choice).

**Environment / server errors:** Offline or server failure → **actionable copy**; logs capture **correlation id**.

### Journey 3 — Alex: Sunday afternoon clone (developer success)

**Opening:** Alex evaluates **Angular + .NET**; finds the **public repo**.

**Rising action:** Installs prerequisites per README, clones, runs **documented** API + client commands. Ensures **SQLite database path is writable** (local folder or mounted volume); **migrations** apply without undocumented steps. Signs in with **seeded** demo user, exercises **TODO** flow; may observe **access + refresh** behavior in DevTools.

**Climax:** **Documented checklist** completes: **builds, runs, authenticated TODO flow**—pass/fail bar (30 min aspirational).

**Resolution:** Alex treats the repo as **runnable and honest**.

**Failure / recovery:** **Read-only filesystem** or bad path → **fail-fast** at startup with log message; README **troubleshooting** for “database locked,” Node/.NET version mismatches.

### Journey 4 — Alex: how auth works (developer deep dive)

**Opening:** Alex must explain **auth** to a teammate.

**Rising action:** README **map** points to **login**, **JWT access token** issuance, **refresh token** issuance/storage (e.g. cookie vs client storage—**documented tradeoff**), **refresh endpoint**, **validation middleware**, **password hashing**, and **refresh rotation** if implemented. Confirms **no production secrets** in committed config.

**Climax:** Alex can **whiteboard the request path** (login → access + refresh → API bearer → refresh on expiry).

**Resolution:** Credible **sample** for security discussion—not enterprise IdP, but **clear and current**.

### Journey 5 — Morgan: deploy for a team (ops-light)

**Opening:** Morgan deploys to a **VM or PaaS** for internal demo.

**Rising action:** Configures **environment** for **JWT signing secret**, **refresh token secret** (if distinct), **writable data directory** for SQLite (**volume mount**). Runs or applies **migrations**. Watches **Serilog** (console + file) for startup health.

**Climax:** **No secrets** in git; **401/403** behave as expected; **read-only FS** caught at startup, not on first user write.

**Resolution:** Morgan shares URL with **Jordan** with clear **data/privacy** expectations (shared vs per-user).

### Journey requirements summary

| Area | Driven by journeys |
|------|---------------------|
| **Authentication** | Jordan (1–2), Alex (3–4), Morgan (5)—**short-lived access + refresh**, rotation, storage choice documented |
| **TODO domain** | Jordan (1–2)—CRUD, **CompletedAt**, **user-scoped vs shared-demo** clarity |
| **Developer experience** | Alex (3–4)—README map, migrations, ports, **writable DB path** |
| **Observability** | Morgan (5), Jordan (2)—logs, correlation id, safe client errors |
| **Public / OSS** | Alex (3), Morgan (5)—secrets hygiene, optional CONTRIBUTING/issues |

*Journeys incorporate **what-if** elicitation (hosting model, shared demo, read-only FS, terminal completion) and **refresh token** decision from frontmatter.*

## Domain-Specific Requirements

### General software & public OSS context

This product is **not** in a regulated vertical (e.g. healthcare, payments). Domain-specific regulation is **out of scope** for v1; requirements focus on **credible, responsible open-source practice** and **honest data handling** for a **dual-audience** (developers + TODO users).

- **Licensing:** Repository **LICENSE** file matches **actual distribution intent**; README states **license** and, where needed, **third-party / attribution** notes for dependencies and assets.
- **Security reporting:** **README** or **SECURITY.md** describes how to **report vulnerabilities** (e.g. private email or GitHub **Security Advisories**); no commitment to SLA, but **clear channel** for a public repo.
- **Data & privacy (demo / self-hosted):** Document **what** is stored in **SQLite** (accounts, refresh-token metadata, TODOs), **who** operates a deployment (operator responsibility), and how to **reset or delete** dev/demo data. If a **shared demo** exists, **in-product or README** warning: **no sensitive data**.
- **Retention:** No long-term **personal data** product promise beyond what the **operator** configures; **MVP** targets **technical clarity** over **legal policy** pages unless you add a hosted multi-tenant product later.
- **Accessibility & inclusion:** **WCAG** target (e.g. **2.1 AA** for key flows) is **recommended** for Growth; MVP may **track** obvious issues (labels, focus, contrast) without full audit.
- **Export / portability:** **Out of MVP** unless required; TODO data **SQLite** file is **operator-accessible** for backup/migration.

*This section intentionally avoids industry-specific compliance matrices; revisit if the product moves into a regulated domain or multi-tenant SaaS with real PII commitments.*

## Differentiation & what this sample proves

*Reframed from a generic “innovation” claim: this section states **credible differentiation** and **learning outcomes** for a public reference repo.*

| Audience | What this sample proves |
|----------|-------------------------|
| **Developers** | A **current** Angular + ASP.NET **SPA hosting model** (dev proxy + static/production hosting) with **real** persistence (**SQLite**), **JWT access tokens** plus **refresh rotation**, and **Serilog** suitable for **clone-and-run** diagnosis—not a disconnected snippet. |
| **Operators** | **Deployable** shape: **writable data directory**, **env-based secrets**, **migrations**, **safe client errors**, and **logs** that do not leak credentials. |
| **TODO users** | A **straight path** from sign-in to **task CRUD** and **completion timestamps** without needing to understand the stack—**honest** about shared vs per-user deployments via README/UX. |
| **OSS readers** | **Dual narrative** in one repo: **reference-first** priority with **documented** tradeoffs (auth storage, refresh vs session, demo data). |

**Trust signals (not “novel technology”):** documented **runbook**, **acceptance-style** checklist (login → TODO → complete → refresh), **no secrets in tree**, and **version-neutral naming**.

*Elicitation note: **Meta-prompting analysis** — user chose **Option B** (differentiation appendix) over a blank innovation section.*

## Web Application Specific Requirements

### Project-type overview

The product is a **single-page application (SPA)** delivered by **Angular**, backed by an **ASP.NET Core REST API**. **MPA** is out of scope for MVP. **Native** and **CLI** client surfaces are **skipped** per project-type defaults.

### Technical architecture considerations

- **Hosting:** Development uses **API + dev server** (e.g. proxy to API); production serves **built static assets** from the API host or reverse proxy—documented in README.
- **API contract:** JSON over HTTPS; version path or header strategy **TBD** (MVP may use unversioned `/api` or controller routes with stable names).

### Browser matrix

| Tier | Browsers | Notes |
|------|-----------|--------|
| **Supported (MVP)** | **Current** Chrome, Edge, Firefox, Safari (desktop + mobile recent) | Best-effort; document **minimum** versions in README. |
| **Unsupported** | Legacy IE, unmaintained browsers | Not tested; may break. |

### Responsive design

- **TODO and auth** flows **usable** on **narrow viewports** (single column, touch-friendly targets). **Sample/extra** pages may be desktop-first if labeled in docs.

### Performance targets

- **MVP:** **Perceived** fast first load on broadband; **no strict SLA**. **Growth:** optional budgets (e.g. LCP, API p95) once baseline measured.
- **API:** Typical TODO CRUD **under 300 ms p95** on local/dev hardware—**indicative**, not contractual.

### SEO strategy

- **Authenticated** app: **SEO low priority**; **no** requirement for deep public indexing of TODO views. **Optional:** static `index.html` **title/meta** and **README** as primary discovery (GitHub, not Google for app shell).

### Real-time

- **Not required for MVP** (no WebSocket requirement for TODO list). **Future:** live collaboration optional.

### Accessibility level

- **MVP:** **Semantic HTML**, **keyboard** path for login + TODO, **visible focus**, **form labels**; fix **obvious** contrast issues. **Growth:** target **WCAG 2.1 AA** on **core flows** with audit checklist.

### Implementation considerations

- **CORS** explicitly configured for dev origins. **Content Security Policy** considered for production template. **Static asset** caching headers for `index.html` vs hashed bundles per Angular defaults.

## Project Scoping & Phased Development

### MVP strategy & philosophy

**MVP approach:** **Experience + platform MVP**—smallest **end-to-end** slice that is both **credible to developers** (inspectable, modern stack) and **useful to task users** (real TODO CRUD + completion). **Not** a revenue MVP; validation is **reproducibility**, **trust**, and **clarity**.

**Resource requirements:** **1–2 developers** comfortable with **.NET** and **Angular** (or one strong full-stack + docs discipline). **No** dedicated security/legal role for v1; follow **checklist-style** OSS practices.

### MVP feature set (Phase 1)

**Core user journeys supported:** **Jordan** (1–2) happy path + basic error/session refresh; **Alex** (3–4) clone + auth understanding; **Morgan** (5) minimal deploy notes optional in README if time allows (may be README-only in MVP).

**Capabilities:** The **ordered MVP list** lives in **Product Scope — MVP** above. **Must-haves** map to **Success Criteria**, **Measurable Outcomes**, and **FR1–FR22**; **quality** expectations map to **Non-Functional Requirements**.

### Post-MVP features

**Phase 2 (growth):** **CI** on PR/main; **E2E** for acceptance bundle; **Aspire** or **one-command dev** script if justified; **container** or single-cloud **publish** template; **WCAG** hardening on core flows; **CONTRIBUTING** / issue templates.

**Phase 3 (expansion):** **Hosted demo** with privacy copy; **team/shared lists**; **OIDC** / social login; **real-time** collaboration **if** product direction shifts from “reference + simple TODO.”

### Risk mitigation strategy

| Risk | Mitigation |
|------|------------|
| **Technical** — upgrade drag, auth bugs | **Phased order** in MVP (backend data first, then client); **acceptance bundle** as gate; **time-box** “extra” pages. |
| **Market / adoption** (OSS) | **README quality** and **CI** as trust; **no** dependency on vanity metrics for MVP success. |
| **Resource** | **Cut** demo pages and **defer** Aspire/containers before **cutting** TODO vertical slice or **refresh** story. |
| **Security (public repo)** | **Secret scanning**, **env-only secrets**, **documented** demo vs production; **SECURITY.md**. |

## Functional Requirements

*This list is the **capability contract** for UX, architecture, and epics: if a capability is not here, it is **not** in scope unless the PRD is amended.*

### Identity and session

- **FR1:** A visitor can submit credentials to establish an authenticated session.
- **FR2:** An authenticated user can end their session (sign out).
- **FR3:** An authenticated user can continue using protected capabilities after a short break without re-entering credentials, within the product’s session policy.
- **FR4:** A user receives clear feedback when authentication fails or the session cannot be renewed.

### Task (TODO) management

- **FR5:** An authenticated user can create a task.
- **FR6:** An authenticated user can view a list of tasks available to them in the current deployment.
- **FR7:** An authenticated user can edit a task they are allowed to change.
- **FR8:** An authenticated user can delete a task they are allowed to delete.
- **FR9:** An authenticated user can mark a task complete.
- **FR10:** An authenticated user can see when a task was completed (completion timestamp).
- **FR11:** The product associates tasks with the correct user when the deployment is configured for per-user data.
- **FR12:** An authenticated user can refresh or reload the task list to reflect the latest server state.

### Authorization (feature access)

- **FR13:** An authenticated user can access only features their account is permitted to use.
- **FR14:** The system rejects unauthorized access to protected server capabilities.

### Client-visible configuration

- **FR15:** The client application can obtain non-secret configuration values intended for display or behavior in the SPA (e.g. equivalent to prior “app settings” exposure).

### Transparency and trust

- **FR16:** When an instance is a shared or demonstration environment, the user can see an appropriate notice (in-product or via documented entry path).

### Demonstration experiences (if retained in MVP)

- **FR17:** A user permitted by the product can access optional demonstration experiences (e.g. sample data or toy features) without gaining access to unrelated protected capabilities.

### Navigation and routing

- **FR18:** An unauthenticated user can access routes intended for sign-in and other public entry.
- **FR19:** An authenticated user can move between authorized areas of the application using the application shell.

### Error handling and support correlation

- **FR20:** A user sees actionable messages for common failures without internal implementation details in production-oriented configurations.
- **FR21:** When the system provides a correlation or reference identifier with an error, the user can communicate it for troubleshooting alongside server logs.

### Persistence expectations

- **FR22:** Task changes made by an authenticated user in a per-user deployment persist across reloads for that user until changed or deleted.

## Non-Functional Requirements

### Performance

- **NFR-P1:** Primary interactive actions (sign-in, list tasks, save task) complete in **under 3 seconds** under normal dev/local conditions unless documented otherwise (not a cloud SLA).
- **NFR-P2:** Application startup fails **fast** with a **clear log message** when persistence cannot be initialized (e.g. unwritable data path), rather than failing on first user write.

### Security

- **NFR-S1:** Credentials and refresh artifacts are **never written to application logs**.
- **NFR-S2:** Signing keys and secrets for production-like configurations come from **environment or secret store**, not committed defaults.
- **NFR-S3:** Authentication endpoints are protected against **trivial abuse** (e.g. rate limiting or equivalent on credential submission).
- **NFR-S4:** Data access uses **parameterized** database access patterns to mitigate injection risk.
- **NFR-S5:** Refresh tokens are **stored and validated** such that **rotation** or **invalidation** can be applied without changing the FR contract (implementation may use one-time rotation).
- **NFR-S6:** API responses in production-oriented configurations **do not expose stack traces** or internal paths to the client.

### Scalability

- **NFR-SC1:** The system is sized for **small to moderate concurrent use** on a **single-node** deployment with **embedded database**; **large-scale multi-tenant SaaS** is **out of MVP scope**.
- **NFR-SC2:** Documentation states **SQLite** operational limits and **backup** expectations for operators.

### Accessibility

- **NFR-A1 (MVP):** Core flows (sign-in, task list, create/edit task) are **keyboard reachable** and use **meaningful labels** for controls.
- **NFR-A2 (Growth):** Target **WCAG 2.1 Level AA** on core flows with a documented audit approach.

### Integration

- **NFR-I1:** No **mandatory** third-party SaaS integrations for MVP; the product operates with **bundled** persistence and **documented** REST usage.
- **NFR-I2:** External identity providers (e.g. OIDC) are **optional future** scope unless added via PRD change.

### Observability

- **NFR-O1:** Operators can use **structured or consistent** log output to **diagnose** auth, API errors, and startup, using **cross-platform** log destinations documented in the runbook.
- **NFR-O2:** Server-side errors intended for support include a **correlation identifier** where feasible (aligned with FR21).

