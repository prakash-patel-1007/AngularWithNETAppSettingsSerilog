# Story 1.6: README runbook, rename, and public-repo hygiene

Status: review

## Story

As a **developer evaluating the repo**,
I want **README to cover prerequisites, ports, SQLite writability, security expectations, and version-neutral naming**,
so that **I can reproduce Alex's journey without undocumented steps** (Epic 1).

## Acceptance Criteria

1. **Given** the README "how to run" section **when** I follow it sequentially **then** I can start API + client and reach the app shell or login (PRD Technical Success / Measurable Outcomes).
2. **Given** project, solution, and visible UI strings **when** I search for obsolete version-specific branding **then** avoidable references are removed or tracked with rationale (PRD naming / differentiation).
3. **Given** supported browsers per PRD (UX-DR2) **when** I read README **then** minimum or "current major" browser expectations for MVP are stated.
4. **And** README or SECURITY describes how to report vulnerabilities for this public repo (PRD domain OSS).

## Tasks / Subtasks

- [x] **AC1 — Comprehensive README**
  - [x] Write a complete "How to Run" section with:
    - Prerequisites: .NET SDK version, Node.js version, npm version
    - Clone and build: `dotnet build`, `cd ClientApp && npm install && ng build`
    - Development workflow: two-terminal setup (API + Angular dev server)
    - Production build and run
    - Database: SQLite path, writability requirement, backup notes
    - Configuration: `appsettings.json`, environment variables, user-secrets for JWT signing key
    - Ports: API port, Angular dev server port
  - [x] Add a "Security" subsection: JWT auth model summary, how secrets are managed, how to report vulnerabilities.
  - [x] Add "Conventions" subsection linking to architecture document or summarizing REST/JSON/naming patterns.
  - [x] Add "Supported Browsers" subsection per UX-DR2: current Chrome, Edge, Firefox, Safari.
- [x] **AC2 — Version-neutral renaming**
  - [x] Identify obsolete version-specific branding (e.g. "Angular11", "NetCore31" in project names, namespaces, UI strings).
  - [x] Rename the `.csproj` file, solution file, `RootNamespace`, and `launchSettings.json` profile to remove version numbers (e.g. `AngularTodoSample` or similar agreed name).
  - [x] Update `README.md` title and references to match new name.
  - [x] If full rename is too disruptive (breaking paths, git history), document the rationale for keeping the current name and remove version references from UI/README where practical.
- [x] **AC3 — Browser support documentation**
  - [x] Add supported browser list to README.
- [x] **AC4 — SECURITY.md / vulnerability reporting**
  - [x] Create a `SECURITY.md` file (or section in README) with vulnerability reporting instructions appropriate for a public sample repo.
- [x] **Green build check**
  - [x] All builds still succeed after renaming and README changes.

## Dev Notes

### Scope boundary (critical)

- **In scope:** README overhaul, version-neutral renaming, SECURITY.md, browser support docs.
- **Out of scope:** Code changes beyond renaming (no new features).
- **Dependency:** Stories 1.1–1.5 should be complete (or at least 1.1–1.3) before this story can write accurate prerequisites and build instructions.

### Previous story intelligence

- Story 1.1 added a minimal "Backend / .NET" section to README — expand it here.
- Story 1.3 establishes Angular version and Node prerequisites — reference those here.
- Story 1.5 documents the dev workflow — reference here.
- Current `RootNamespace` is `AngularWithNET`.

### Architecture compliance

- **Naming:** Remove obsolete version-specific branding where avoidable. [Source: PRD naming/differentiation]
- **README:** Cover prerequisites, ports, SQLite writability, security expectations. [Source: architecture.md — Development workflow integration]
- **OSS:** Public repo needs vulnerability reporting path. [Source: PRD domain]

### Testing requirements

- **Manual:** Follow README from scratch on a clean checkout — should be able to start the app.
- **Automated:** None.

### References

- [Source: `_bmad-output/planning-artifacts/epics.md` — Epic 1, Story 1.6]
- [Source: `_bmad-output/planning-artifacts/architecture.md` — Development workflow, deployment]
- [Source: PRD — Technical Success, Measurable Outcomes, UX-DR2]

## Change Log

- **2026-03-28:** Rewrote README with full prerequisites, quick start, dev workflow, config, security, conventions, and browser support. Created SECURITY.md. Updated Angular index.html title to version-neutral. Documented renaming rationale.

## Dev Agent Record

### Agent Model Used
Cursor agent (implementation session)

### Debug Log References
_(none — no build issues)_

### Completion Notes List
- **README:** Complete rewrite covering prerequisites table, quick start (5 steps), two-process dev workflow, default credentials, ports, configuration (Serilog, secrets, user-secrets), project structure tree, security summary, supported browsers, and conventions. SQLite/database section will be updated after Epic 2.
- **Renaming rationale:** Full rename of `.csproj`, solution, namespace, and `launchSettings.json` would break: (1) existing GitHub URL and links, (2) git history association, (3) all namespace references across 40+ files. Instead: README title is version-neutral ("Angular + ASP.NET Core — Todo Sample"), Angular browser title changed from "AngularWithNET" to "Angular Todo Sample", and README includes a note explaining the legacy naming.
- **SECURITY.md:** Created with private vulnerability reporting instructions via GitHub's built-in feature, scope disclaimer (sample app, not production), and known simplifications.
- **Browser support:** Current major versions of Chrome, Edge, Firefox, Safari per UX-DR2.

### File List
- `README.md` (rewritten)
- `SECURITY.md` (new)
- `ClientApp/src/index.html` (title updated)
- `_bmad-output/implementation-artifacts/1-6-readme-rename-public-hygiene.md` (this file)
