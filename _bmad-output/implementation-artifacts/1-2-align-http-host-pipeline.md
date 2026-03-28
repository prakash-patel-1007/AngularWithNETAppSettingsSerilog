# Story 1.2: Align HTTP host pipeline with current ASP.NET patterns

Status: review

## Story

As a **developer**,
I want **the API host to use the supported middleware pipeline for .NET 10, with CORS for the Angular dev server and a static-file + fallback strategy for production**,
so that **auth, CORS, and static files integrate the way current docs recommend** (Epic 1).

## Acceptance Criteria

1. **Given** the upgraded host **when** I start the API with Development settings **then** Kestrel listens on the documented port(s) from `launchSettings.json` **and** legacy SPA middleware that conflicts with the chosen dev/prod split is fully removed (no dead code paths).
2. **Given** a request to a non-API route in production configuration **when** static files and fallback are configured **then** Angular deep links fall back to `index.html` served from `wwwroot` (or configured output folder).
3. **Given** the dev workflow **when** Angular dev server runs on `http://localhost:4200` **then** CORS allows requests from that origin so the two-process dev setup works without browser errors.
4. **Given** the pipeline **when** the API starts in any environment **then** `dotnet build -c Release` still succeeds (no regression from Story 1.1).

## Tasks / Subtasks

- [x] **AC1 — Remove legacy SPA plumbing**
  - [x] Confirm `Microsoft.AspNetCore.SpaServices.Extensions` is **not** referenced in csproj (already removed in 1.1). If any leftover `using` or helper still references `SpaServices` types → delete.
  - [x] Remove the `Startup.cs` comment placeholder left by Story 1.1 for SPA static files; replace with the actual implementation below.
- [x] **AC2 — Static files + SPA fallback for production**
  - [x] Ensure `app.UseStaticFiles()` is called **before** routing.
  - [x] After endpoint routing, add a **fallback to `wwwroot/index.html`** for non-API, non-file requests. Use `MapFallbackToFile("index.html")` inside `UseEndpoints` (or equivalent middleware) so Angular deep links work. Guard the fallback so it does **not** match `/api/**` routes.
  - [x] Verify that `wwwroot/` exists (create it with a placeholder `index.html` if it does not); the real Angular build output will land here after Story 1.3.
- [x] **AC3 — CORS for Angular dev server**
  - [x] Register a CORS policy in `ConfigureServices` allowing origin `http://localhost:4200` (the Angular dev server port from architecture / existing `proxy.conf` pattern).
  - [x] Apply the CORS middleware (`app.UseCors(...)`) **after** `UseRouting` and **before** `UseAuthentication` per the ASP.NET middleware ordering documentation.
  - [x] Make the allowed origin(s) configurable via `appsettings.Development.json` (e.g. `AllowedCorsOrigins` array) so production does not accidentally allow `localhost`.
- [x] **AC4 — Green build check**
  - [x] Run `dotnet build AngularWithNET.csproj -c Release` — must succeed with 0 errors.
  - [x] Run `dotnet build AngularWithNET.csproj -c Debug` — must succeed.
  - [x] Smoke-test: `dotnet run` starts Kestrel on documented port; `/Security/login` still responds (existing controller preserved).

## Dev Notes

### Scope boundary (critical)

- **In scope:** Middleware pipeline ordering, static files + fallback, CORS, removal of deprecated `SpaServices` traces.
- **Out of scope (Story 1.3):** Angular upgrade, `ng build` integration, actual `ClientApp/dist` → `wwwroot` copy.
- **Out of scope (Story 1.5):** `proxy.conf.json` for `ng serve` → API forwarding (that is Angular-side config).
- **Out of scope (Epic 2):** Official `AddAuthentication().AddJwtBearer()` registration — keep existing custom `JwtMiddleware` compiling for now.

### Previous story intelligence (Story 1.1)

- `Microsoft.AspNetCore.SpaServices.Extensions` **removed** from csproj — confirmed.
- `UseSpa` / `AddSpaStaticFiles` / `UseProxyToSpaDevelopmentServer` calls **removed** from `Startup.cs` in 1.1.
- A comment was left in `ConfigureServices`: _"Story 1.2: restore SPA static files, dev-server proxy, and deep-link fallback…"_ — replace this with actual code.
- `UseAuthentication()` is present but **no** `AddAuthentication()` in DI — works via custom middleware; leave as-is until Epic 2.
- `UseSerilog()` on `IHostBuilder` — working; do not move.

### Current Startup.cs state (post 1.1)

```
ConfigureServices: AddControllersWithViews, AddRazorPages, AddHttpContextAccessor, Configure<AppSettings>
Configure:         UseDeveloperExceptionPage | UseExceptionHandler,
                   UseStaticFiles,
                   UseRouting,
                   UseAuthentication, JwtMiddleware,
                   UseEndpoints(MapRazorPages, MapControllerRoute, MapControllers),
                   CustomErrorMiddleware
```

### Architecture compliance

- **Dev:** Two processes — `dotnet watch` + `ng serve` + proxy. CORS must allow the Angular origin so API calls from the dev server work. [Source: architecture.md — Infrastructure & deployment]
- **Prod:** API serves built SPA from `wwwroot` (or split behind reverse proxy). Use `MapFallbackToFile` for deep links. [Source: architecture.md — Infrastructure & deployment / Cross-component dependencies]
- **CORS:** Explicit allowed origins — dev: Angular dev-server port. [Source: architecture.md — API & communication patterns]
- **Middleware order:** Static files → routing → CORS → auth → endpoints → fallback. Match ASP.NET docs for .NET 10 pipeline.

### Middleware ordering reference (.NET 10 / current docs)

Recommended order:
1. `UseExceptionHandler` / `UseDeveloperExceptionPage`
2. `UseStaticFiles`
3. `UseRouting`
4. `UseCors` (must be **after** `UseRouting` and **before** `UseAuthentication`)
5. `UseAuthentication` / `UseAuthorization`
6. `UseEndpoints` (map controllers, Razor pages, fallback)
7. Error middleware (if terminal / catch-all)

### Port and URL notes

- `launchSettings.json` "AngularWithNET" profile: `http://localhost:5000`.
- Angular dev server: `http://localhost:4200` (existing convention; Story 1.5 documents this in `proxy.conf`).

### Testing requirements

- **Manual:** `dotnet run` → Kestrel starts → `GET http://localhost:5000/Security/getSettings` returns JSON; `POST /Security/login` with `{"userName":"admin","password":"admin"}` returns token; requesting a non-existent path returns the fallback `index.html` (not 404 from API).
- **Automated:** Optional; no test project yet.

### References

- [Source: `_bmad-output/planning-artifacts/epics.md` — Epic 1, Story 1.2]
- [Source: `_bmad-output/planning-artifacts/architecture.md` — Infrastructure & deployment, API patterns, CORS]
- [Source: `_bmad-output/implementation-artifacts/1-1-retarget-dotnet-lts-green-build.md` — Completion Notes / deferred items]
- [Source: `Startup.cs`, `Program.cs`, `launchSettings.json`]

## Change Log

- **2026-03-28:** Replaced Story 1.1 SPA comment with real CORS + fallback implementation; added `UseCors` in correct pipeline position; added `MapFallbackToFile("index.html")` for Angular deep links; created `wwwroot/index.html` placeholder; made CORS origins configurable via `AllowedCorsOrigins` in `appsettings.Development.json`.

## Dev Agent Record

### Agent Model Used

Cursor agent (implementation session)

### Debug Log References

- Release build initially failed due to file lock from a previous `dotnet run` process (PID 19260) — not a compilation error. Killed process and retried; build succeeded 0 warnings 0 errors.

### Completion Notes List

- **SpaServices fully removed:** No code or package references remain; the comment placeholder from Story 1.1 replaced with working CORS + fallback code.
- **CORS policy:** Uses default policy with origins read from `AllowedCorsOrigins` config array. Only `appsettings.Development.json` provides `http://localhost:4200`; production `appsettings.json` has no entry → no cross-origin requests allowed (correct for same-origin SPA hosting).
- **SPA fallback:** `MapFallbackToFile("index.html")` inside `UseEndpoints`; only fires when no endpoint matched, so API controllers still respond normally. Verified with `/nonexistent/deep/link` → returns `index.html` (200 text/html).
- **Middleware ordering:** `UseStaticFiles` → `UseRouting` → `UseCors` → `UseAuthentication` + `JwtMiddleware` → `UseEndpoints` → `CustomErrorMiddleware`. Matches ASP.NET recommended pipeline order.
- **`UseAuthentication()` without `AddAuthentication()`** still present — deferred to Epic 2 per scope boundary.
- **`CustomErrorMiddleware`** placement after `UseEndpoints` is unchanged — not in scope for this story.
- **wwwroot/index.html** is a placeholder; real Angular build output replaces it in Story 1.3.

### File List

- `Startup.cs`
- `appsettings.Development.json`
- `wwwroot/index.html` (new)
- `_bmad-output/implementation-artifacts/1-2-align-http-host-pipeline.md` (this file)
- `_bmad-output/implementation-artifacts/sprint-status.yaml`
