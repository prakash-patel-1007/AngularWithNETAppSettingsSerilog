# Story 1.4: Add Serilog with console and configurable file logging

Status: review

## Story

As an **operator**,
I want **structured logs to console and to rolling files on a configurable, cross-platform path**,
so that **I can diagnose startup and request issues without leaking secrets** (Epic 1).

## Acceptance Criteria

1. **Given** default Development configuration **when** the API starts **then** Serilog writes human- or machine-readable output to console (NFR-O1).
2. **Given** configured file sink path **when** the API runs on Windows and on a Unix-like OS (documented as best-effort if CI only covers one) **then** log files roll per configuration and paths are not hard-coded to a single developer machine.
3. **Given** login or refresh requests occur **when** logs are written for those requests **then** access tokens, refresh tokens, and passwords never appear in log payloads (NFR-S1).

## Tasks / Subtasks

- [x] **AC1 — Console sink**
  - [x] Add `Serilog.Sinks.Console` package (if not already present via `Serilog.AspNetCore`).
  - [x] Update `appsettings.json` Serilog `WriteTo` to include a Console sink alongside the File sink.
  - [x] Verify: `dotnet run` shows structured log output in terminal on startup.
- [x] **AC2 — Configurable cross-platform file path**
  - [x] Replace the current hard-coded Windows path `C:\Logs\Application-log-.txt` in `appsettings.json` with a relative or environment-variable-based path (e.g. `logs/app-.log` relative to content root, or `%APPDATA%`/`$HOME`-based).
  - [x] Document the default log path in `appsettings.json` comments or README.
  - [x] Add `appsettings.Development.json` Serilog overrides if the dev log path should differ.
  - [x] Ensure `logs/` directory is in `.gitignore`.
  - [x] Verify rolling file behavior: files roll daily per existing `rollingInterval: "Day"` config.
- [x] **AC3 — No secret leakage in logs**
  - [x] Audit `SecurityController.cs` login action: the existing `_logger.LogInformation($"User: {loginDetails.UserName}, Login successful")` is acceptable, but verify no password or token values are logged.
  - [x] Ensure the JWT `Secret` from `AppSettings` is never logged at startup or request time.
  - [x] Add a note in Dev Agent Record about which log statements were audited and confirmed safe.
- [x] **Green build check**
  - [x] `dotnet build -c Release` succeeds with 0 errors after changes.

## Dev Notes

### Scope boundary (critical)

- **In scope:** Serilog console sink, cross-platform configurable file path, no-secret-logging audit.
- **Out of scope (Story 5.1):** Correlation ID middleware and Serilog enrichment — that is Epic 5.
- **Out of scope (Story 1.6):** Full README logging documentation — just ensure the config is right.

### Previous story intelligence

- Serilog is already configured in `Program.cs` via `UseSerilog()` on `IHostBuilder`, reading from `Configuration`.
- Current `appsettings.json` Serilog config has `WriteTo` with only a File sink pointing to `C:\Logs\Application-log-.txt` — Windows-only, hard-coded, not cross-platform.
- `Serilog.AspNetCore` 9.0.0, `Serilog.Sinks.File` 6.0.0, `Serilog.Settings.Configuration` 9.0.0, `Serilog.Enrichers.Environment` 3.0.1 are already in `csproj`.
- Console sink may already be included transitively via `Serilog.AspNetCore` — verify.

### Architecture compliance

- **Serilog:** Console + rolling file; file path configurable and cross-platform. [Source: architecture.md — Infrastructure & deployment]
- **NFR-S1:** Never log tokens or passwords. [Source: architecture.md — Communication patterns]
- **NFR-O1:** Structured/consistent log output for diagnosing auth, API errors, and startup. [Source: PRD]

### Testing requirements

- **Manual:** Start API → see console output; check `logs/` folder for rolling log file; grep logs for password/token values (should find none).
- **Automated:** Optional.

### References

- [Source: `_bmad-output/planning-artifacts/epics.md` — Epic 1, Story 1.4]
- [Source: `appsettings.json` — current Serilog configuration]
- [Source: `Program.cs` — Serilog bootstrap]
- [Source: `Controllers/SecurityController.cs` — login logging]

## Change Log

- **2026-03-28:** Added Console sink to Serilog config. Replaced hard-coded `C:\Logs\Application-log-.txt` with relative `logs/app-.log`. Added `logs/` and `app.db` patterns to `.gitignore`. Audited log statements — no secrets logged.

## Dev Agent Record

### Agent Model Used
Cursor agent (implementation session)

### Debug Log References
_(none — no build issues)_

### Completion Notes List
- **Console sink:** Already included transitively via `Serilog.AspNetCore` 9.0.0; added `Serilog.Sinks.Console` to the `Using` array in config and a `Console` WriteTo entry with timestamp + level + message template.
- **File path:** Changed from `C:\Logs\Application-log-.txt` (Windows-only, absolute) to `logs/app-.log` (relative, cross-platform). Forward slashes work on both Windows and Unix.
- **Default minimum level:** Changed from `Debug` to `Information` for less noise in production; `Microsoft.Hosting.Lifetime` override at `Information` to show startup URLs.
- **Log audit:** Audited 3 log statements in `SecurityController.cs`: line 38 ("Get Forecast Called"), line 62 ("User: {userName}, Login successful"), line 68 ("User: {userName}, Login failed"). None log passwords, tokens, or the JWT Secret. The Secret is only used in `GenerateJwtToken()` to create signing credentials — never passed to a logger.
- **`.gitignore`:** Added `logs/`, `app.db`, `*.db-shm`, `*.db-wal` for future SQLite story.

### File List
- `appsettings.json`
- `.gitignore`
- `_bmad-output/implementation-artifacts/1-4-serilog-console-configurable-file.md` (this file)
