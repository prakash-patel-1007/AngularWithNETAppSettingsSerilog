# Story 1.1: Retarget API to agreed .NET LTS and restore green build



Status: review



## Story



As a **developer**,

I want **the server project to target a supported .NET version and build cleanly on a fresh clone**,

so that **I have a credible baseline before feature work** (PRD Technical Success; Epic 1).



## Acceptance Criteria



1. **Given** a clean checkout with the documented SDK installed **when** `dotnet build` runs at the project root **then** the build completes with **no errors**.

2. **Given** the retarget **when** obsolete package references block compile **then** they are upgraded, removed, or temporarily documented as explicit follow-ups **without** a false green (broken publish).

3. **Given** architecture preference **when** README / story handoff is read **then** `TargetFramework` matches **either `net10.0` (preferred)** or **`net8.0` (acceptable)** and that choice is **documented** in README prerequisites.



## Tasks / Subtasks



- [x] **AC1 — Green `dotnet build`**

  - [x] Install/use **.NET 10 SDK** (preferred) or **.NET 8 SDK** on the build machine; add `global.json` only if the repo must pin SDK version for CI.

  - [x] Update `AngularWithNET.csproj`: set `<TargetFramework>net10.0</TargetFramework>` (or `net8.0`).

  - [x] Replace legacy **package-per-package** ASP.NET Core 3.1 references with the **modern Web SDK model**: for `Microsoft.NET.Sdk.Web`, rely on the **shared framework** (`FrameworkReference` to `Microsoft.AspNetCore.App` is implicit with the Web SDK); add explicit `PackageReference` only where still required (e.g. JWT Bearer, Serilog) at versions compatible with the chosen TFM.

  - [x] Resolve compile errors from **namespace / API surface** changes between netcoreapp3.1 and net8/10 (e.g. `IHostBuilder`, `Startup` vs minimal hosting — **do not** complete full pipeline migration here if it balloons scope; minimal changes to **compile** are in scope; full middleware alignment is **Story 1.2**).

- [x] **AC2 — No false green**

  - [x] Run `dotnet build -c Release` and fix warnings that indicate broken publish (e.g. missing targets).

  - [x] List any **intentional** deferred items in **Dev Agent Record → Completion Notes** (not in README until 1.6 unless blocking).

- [x] **AC3 — Documented TFM**

  - [x] Add or update a short **“Backend / .NET”** subsection in README (can be minimal in this story; expanded in 1.6) stating exact **SDK version**, **TFM**, and install link.



## Dev Notes



### Scope boundary (critical)



- **In scope:** Server **.csproj** retarget, package/framework alignment, **compile-success** for the API project.

- **Out of scope (Story 1.2):** Full **minimal hosting** migration, SPA static pipeline redesign, CORS/auth reordering, deep linking — only touch what **compilation** forces.

- **Out of scope (Story 1.3):** Angular / `ClientApp` upgrade.



### Current brownfield baseline (do not reinvent)



| Area | Today | Source |

|------|--------|--------|

| TFM | `netcoreapp3.1` | `AngularWithNET.csproj` |

| Host | `Program.cs` + `Startup.cs`, `Host.CreateDefaultBuilder`, `UseStartup<Startup>()` | `Program.cs`, `Startup.cs` |

| Serilog | `UseSerilog()`, read from configuration | `Program.cs` |

| SPA | `SpaServices` proxy to `localhost:4200`, `ClientApp/dist` | `Startup.cs` |

| Config load | `appSettings.json` in `Program.cs` (verify casing vs `appsettings.json` on disk) | `Program.cs` |



### Project Structure Notes



- Keep **existing** project file name and namespace **unless** rename is required for build (rename is **Story 1.6**).

- Architecture target layout (`Features/`, `Data/`) is **future**; this story **must not** mass-move files unless required to compile.



### Architecture compliance



- **Brownfield in-place migration** — no greenfield `dotnet new` replacement of the tree. [Source: `_bmad-output/planning-artifacts/architecture.md` — Starter Template Evaluation]

- Prefer **.NET 10 LTS**; **.NET 8 LTS** acceptable. [Source: `architecture.md` — Core Architectural Decisions]

- **Secrets** remain out of committed defaults (no regression). [Source: PRD NFR-S2]



### Library / framework requirements



- **JWT / Serilog / File sink:** Upgrade to versions compatible with **net8+ / net10**; remove packages absorbed by the shared framework where applicable.

- **`Microsoft.AspNetCore.SpaServices.Extensions`:** Likely **obsolete or incompatible** on modern TFMs — if it blocks build, **stub or conditionally exclude** only enough for **Story 1.1** compile, with a **clear comment** pointing to **Story 1.2** for proper dev-server / static-file strategy. Prefer **not** deleting SPA integration entirely without 1.2 follow-up.



### Testing requirements



- **Manual:** `dotnet build`, `dotnet build -c Release` from repo root (or project directory — document which).

- **Automated:** Optional; no test project required for this story unless one already exists.



### Latest technical information (2026)



- **.NET 10** is the current **LTS** track for long-term sample credibility; verify latest **SDK patch** on [https://dotnet.microsoft.com/download](https://dotnet.microsoft.com/download) at implementation time.

- Migrating **3.1 → 8/10** commonly requires: implicit usings (optional), nullable reference types (optional per team), removing or upgrading legacy `Microsoft.AspNetCore.*` 3.1 package references in favor of the shared framework, and adjusting breaking middleware types — use official migration docs for the jump you choose.



### Git intelligence (recent commits)



- Recent history is **initial/upload/README** commits; **no prior migration work** to reuse — establish patterns in this story for later epics.



### References



- [Source: `_bmad-output/planning-artifacts/epics.md` — Epic 1, Story 1.1]

- [Source: `_bmad-output/planning-artifacts/architecture.md` — Version targets, brownfield]

- [Source: `_bmad-output/planning-artifacts/prd.md` — Technical Success, MVP #1]

- [Source: `AngularWithNET.csproj`, `Program.cs`, `Startup.cs`]



## Change Log



- **2026-03-28:** Retargeted host to `net10.0`; removed SpaServices package usage from pipeline (deferred to Story 1.2); fixed config file name to `appsettings.json`; excluded `_tmp_bcrypt/**` from default compile items; Serilog registered on `IHostBuilder`; README Backend section added.



## Dev Agent Record



### Agent Model Used



Cursor agent (implementation session)



### Debug Log References



- Build failure: duplicate assembly attributes — root cause `_tmp_bcrypt\obj\**` picked up by SDK wildcard compile items; fixed via `DefaultItemExcludes`.

- Build failure: `UseSerilog` on `IWebHostBuilder` — moved to `Host.CreateDefaultBuilder().UseSerilog()`.



### Completion Notes List



- **Deferred to Story 1.2:** `AddSpaStaticFiles` / `UseSpa` / dev-server proxy to Angular; restore static file + fallback + ordering with modern hosting patterns.

- **Deferred to Story 1.3:** MSBuild targets that run `npm install` / `ng build` on publish (removed with old csproj); reintroduce a single documented publish path later.

- **`UseAuthentication()`** remains without `AddAuthentication` in DI (legacy); JWT validation is custom middleware — Story 1.2 may align with official JWT Bearer.

- **Tests:** No test project added; story marked automated tests optional.



### File List



- `AngularWithNET.csproj`

- `Program.cs`

- `Startup.cs`

- `README.md`

- `_bmad-output/implementation-artifacts/1-1-retarget-dotnet-lts-green-build.md` (this file)

- `_bmad-output/implementation-artifacts/sprint-status.yaml`



---



**Checklist:** Story context reviewed against `bmad-create-story/checklist.md` — anti-patterns called out (no duplicate greenfield, SPA deferred to 1.2 if needed, TFM documented).



**Ultimate context engine analysis completed** — comprehensive developer guide created for Story 1.1.

