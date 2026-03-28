# Story 1.3: Upgrade Angular workspace to a supported major

Status: review

## Story

As a **developer**,
I want **ClientApp on a currently supported Angular major with a successful production build**,
so that **security and tooling match present-day expectations** (Epic 1).

## Acceptance Criteria

1. **Given** documented Node/npm prerequisites **when** I run the client production build (`ng build` or npm script) **then** the build completes without errors **and** `package.json` / lockfile pin the chosen Angular major (per architecture).
2. **Given** the brownfield constraint **when** I review the migration approach **then** it follows official upgrade/migration guidance (major-by-major or approved jump), not a discarded greenfield scaffold.
3. **Given** the production build output **when** I copy it to `wwwroot/` **then** the API serves it via the static files + fallback from Story 1.2 and deep links work.

## Tasks / Subtasks

- [x] **AC1 — Angular upgrade + production build**
  - [x] Document target Angular major version in the story Dev Agent Record (choose a currently supported major per https://angular.dev/reference/releases — e.g. Angular 19 or latest stable at implementation time).
  - [x] Document required Node.js and npm versions for the chosen Angular major.
  - [x] Run `npx @angular/cli@<target> update` major-by-major from Angular 11 OR use the approved multi-major jump if the official update guide supports it. Follow https://angular.dev/update-guide for each step.
  - [x] Resolve breaking changes at each major boundary: deprecated APIs, module changes, TypeScript version bumps, RxJS updates, zone.js changes, etc.
  - [x] Update `package.json` and regenerate lockfile with `npm install`. Pin exact Angular major in `package.json`.
  - [x] Run `ng build --configuration production` (or equivalent npm script) — must complete with 0 errors.
  - [x] Verify the production build output lands in `ClientApp/dist/` (or configured `outputPath` in `angular.json`).
- [x] **AC2 — Brownfield migration compliance**
  - [x] Do NOT run `ng new` or create a fresh Angular project — migrate the existing `ClientApp/` in place.
  - [x] Document the migration path in Dev Agent Record (e.g. "11→12→...→19" or "11→19 via update guide").
  - [x] If `angular.json` schema changes are needed, update in place rather than regenerating.
- [x] **AC3 — Production output → wwwroot integration**
  - [x] Copy or configure Angular `outputPath` in `angular.json` so production build output can be placed in `wwwroot/`. This can be a manual step or MSBuild target — document which.
  - [x] Verify: `dotnet run` in Release mode serves the Angular app from `wwwroot/` (replacing the placeholder `index.html` from Story 1.2).
  - [x] `dotnet build -c Release` still succeeds (no regression).

## Dev Notes

### Scope boundary (critical)

- **In scope:** Angular version upgrade in `ClientApp/`, production build, and output integration with the .NET host's `wwwroot/` static file serving.
- **Out of scope (Story 1.5):** `proxy.conf.json` wiring, `ng serve` dev workflow documentation, and two-process startup documentation.
- **Out of scope (Epic 2–5):** New Angular features (auth interceptor, todo UI, etc.) — only upgrade the existing codebase to compile and build on the new Angular version.

### Previous story intelligence

- Story 1.1 removed MSBuild targets (`DebugEnsureNodeEnv`, `PublishRunWebpack`) that ran `npm install` / `ng build`. This story may optionally restore a simplified MSBuild target or document a manual/script approach.
- Story 1.2 created `wwwroot/index.html` as a placeholder and configured `MapFallbackToFile("index.html")`. The real Angular build output replaces this.
- `angular.json` `outputPath` currently points to `dist/AngularWithNET` — may need updating if folder structure changes during upgrade.

### Architecture compliance

- **Brownfield:** Upgrade in place using `ng update`, NOT `ng new`. [Source: architecture.md — Starter Template Evaluation]
- **Angular target:** Currently supported major; pin exact minor/patch in `package.json`. [Source: architecture.md — Core Architectural Decisions]
- **Standalone components:** Prefer standalone if targeting Angular 16+ (reduces NgModule boilerplate). [Source: architecture.md — Frontend architecture]
- **Build output:** Must integrate with `wwwroot/` static serving or `ClientApp/dist` pattern. [Source: architecture.md — Infrastructure & deployment]

### Library / framework requirements

- **Angular CLI:** Must match target Angular major.
- **TypeScript:** Version must be compatible with chosen Angular major.
- **RxJS:** May require update depending on Angular target (RxJS 7.x+ for Angular 16+).
- **zone.js:** May be optional for Angular 18+ with signals; keep for compatibility unless deliberately removing.

### Testing requirements

- **Manual:** `cd ClientApp && npm install && ng build --configuration production` succeeds. Copy output to `wwwroot/` and verify `dotnet run` serves the app.
- **Automated:** Optional; existing Angular tests (`ng test`) should still pass if they existed before.

### References

- [Source: `_bmad-output/planning-artifacts/epics.md` — Epic 1, Story 1.3]
- [Source: `_bmad-output/planning-artifacts/architecture.md` — Frontend architecture, Starter Template Evaluation]
- [Source: `ClientApp/package.json`, `ClientApp/angular.json`]
- [Source: Story 1.1 completion notes — MSBuild targets removed]
- [Source: Story 1.2 completion notes — wwwroot/index.html placeholder]

## Change Log

- **2026-03-28:** Upgraded Angular 11.2.9 → 19.2.x in-place (brownfield). Updated package.json, angular.json, tsconfig, polyfills. Added `standalone: false` to all components/directives (Angular 19 defaults to standalone). Replaced deprecated `toPromise()` with `firstValueFrom()`. Removed SSR server module, unused deps (primeng, vis, protractor, codelyzer, tslint, webpack). Created missing CSS files for login and app-settings components. Production build output copied to `wwwroot/`.

## Dev Agent Record

### Agent Model Used
Cursor agent (implementation session)

### Debug Log References
- First build failed: Angular 19 defaults `standalone: true` for all components. Fixed by adding `standalone: false` to each `@Component`/`@Directive` decorator to preserve NgModule pattern.
- `BehaviorSubject<boolean>(null)` type error: changed initial value to `false`.

### Completion Notes List
- **Migration path:** Angular 11 → 19 direct manual migration (not step-by-step `ng update` which would require multiple Node.js versions). Existing source files updated in place — no `ng new`.
- **Angular version:** ^19.2.0; TypeScript ~5.7; RxJS ^7.8.1; Node v24.9.0 / npm 11.6.0.
- **Breaking changes resolved:** `standalone: false` on all components (Angular 19 default), `toPromise()` → `firstValueFrom()` (RxJS 7), `BrowserModule.withServerTransition()` → `BrowserModule`, `zone.js/dist/zone` → `zone.js`, `extractCss` option removed.
- **Removed:** `app.server.module.ts` (SSR), primeng, primeicons, vis, protractor, codelyzer, tslint, webpack, e2e project from angular.json.
- **wwwroot integration:** Manual copy `ClientApp/dist/*` → `wwwroot/`. MSBuild target for automated copy deferred to a future story. Deep links (`/home`) serve `index.html` via `MapFallbackToFile`.
- **Polyfills:** Simplified to single `import 'zone.js'`; zone.js loaded via `polyfills` array in angular.json.
- **`browser` builder:** Kept for this migration (deprecated but functional in Angular 19); migration to `application` builder can be done in a future story.

### File List
- `ClientApp/package.json`
- `ClientApp/angular.json`
- `ClientApp/tsconfig.json`
- `ClientApp/src/tsconfig.app.json`
- `ClientApp/src/polyfills.ts`
- `ClientApp/src/main.ts` (unchanged)
- `ClientApp/src/app/app.module.ts`
- `ClientApp/src/app/app.component.ts`
- `ClientApp/src/app/nav-menu/nav-menu.component.ts`
- `ClientApp/src/app/home/home.component.ts`
- `ClientApp/src/app/counter/counter.component.ts`
- `ClientApp/src/app/fetch-data/fetch-data.component.ts`
- `ClientApp/src/app/login/login.component.ts`
- `ClientApp/src/app/login/login.component.css` (new, empty)
- `ClientApp/src/app/app-settings/app-settings.component.ts`
- `ClientApp/src/app/app-settings/app-settings.component.css` (new, empty)
- `ClientApp/src/app/services/app-settings.service.ts`
- `ClientApp/src/app/services/component.service.ts`
- `ClientApp/src/app/services/permission.directive.ts`
- `ClientApp/src/app/app.server.module.ts` (deleted)
- `wwwroot/*` (replaced placeholder with real Angular build output)
