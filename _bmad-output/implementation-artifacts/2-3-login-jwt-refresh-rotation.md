# Story 2.3: Login, JWT access, and refresh token rotation

Status: review

## Story

As a **visitor**,
I want **to sign in and receive short-lived access plus a rotated refresh token stored server-side**,
so that **I can use the API securely and renew my session (FR1, FR3, NFR-S5)** (Epic 2).

## Acceptance Criteria

1. **Given** valid demo credentials **when** I POST to the documented login endpoint **then** I receive an access token and refresh artifact per the chosen transport (cookie or JSON body) documented in README.
2. **Given** a valid refresh request **when** I POST to the refresh endpoint **then** a new refresh identifier is issued and the previous refresh is invalidated if rotation is enabled (NFR-S5).
3. **Given** login and refresh endpoints **when** abuse simulation (rapid repeated posts) **then** rate limiting or equivalent mitigates trivial brute force (NFR-S3).
4. **And** tokens never appear in logs (NFR-S1).

## Tasks / Subtasks

- [ ] **AC1 — Login endpoint**
  - [ ] Create `Features/Auth/AuthController.cs` (or `AuthEndpoints.cs`) with `POST /api/auth/login` accepting `{ "username": "...", "password": "..." }`.
  - [ ] Validate credentials against `User` table using the password hashing service from Story 2.2.
  - [ ] On success: generate a short-lived JWT access token (e.g. 15–30 min, configurable via `AppSettings.TokenExpiry`).
  - [ ] On success: generate an opaque refresh token, hash it, store in `RefreshTokens` table with `UserId`, `ExpiresAt`, `CreatedAt`.
  - [ ] Return access token + refresh token per chosen transport. Document the choice (JSON body or httpOnly cookie) in README.
  - [ ] On failure: return 401 with Problem Details, no credential info in response.
  - [ ] Register official `AddAuthentication().AddJwtBearer()` in DI to replace/supplement the legacy custom `JwtMiddleware`. Mark `JwtMiddleware` for removal once all endpoints use `[Authorize]`.
- [ ] **AC2 — Refresh endpoint with rotation**
  - [ ] Add `RefreshToken` entity to `Domain/`: `Id`, `TokenHash`, `UserId` (FK), `ExpiresAt`, `CreatedAt`, `RevokedAt` (nullable), `ReplacedByTokenId` (nullable).
  - [ ] Add EF migration for `RefreshTokens` table.
  - [ ] Create `POST /api/auth/refresh` endpoint accepting the refresh token.
  - [ ] Validate: token exists, not expired, not revoked. If valid: revoke current token (set `RevokedAt`), issue new refresh + new access token, link via `ReplacedByTokenId`.
  - [ ] If invalid/expired/revoked: return 401.
- [ ] **AC3 — Rate limiting**
  - [ ] Add rate limiting middleware or use ASP.NET Core built-in rate limiting (`Microsoft.AspNetCore.RateLimiting`) on `/api/auth/login` and `/api/auth/refresh`.
  - [ ] Configure sensible limits (e.g. 5 attempts per minute per IP for login).
  - [ ] Return 429 Too Many Requests when limit exceeded.
- [ ] **AC4 — No token logging**
  - [ ] Audit all log statements in auth code — ensure no access token, refresh token, or password values appear.
  - [ ] Log login attempts with username and result (success/failure) only.
- [ ] **Green build check**
  - [ ] `dotnet build -c Release` succeeds.

## Dev Notes

### Scope boundary (critical)

- **In scope:** Login endpoint, JWT issuance, refresh endpoint with rotation, rate limiting, official JWT Bearer auth registration.
- **Out of scope (Story 2.4):** Sign-out / refresh revocation endpoint.
- **Out of scope (Story 2.6):** Angular HTTP interceptor for Bearer attachment and silent refresh.
- **Out of scope:** User registration endpoint (not in MVP).

### Previous story intelligence

- Story 2.1: `AppDbContext` + `User` entity exist; `Database.Migrate()` at startup.
- Story 2.2: Password hashing service + demo seed users exist.
- Story 1.2: `UseAuthentication()` present without `AddAuthentication()` — this story adds official JWT Bearer setup.
- Legacy `SecurityController` still exists with hard-coded user checks — this story adds a NEW `AuthController` under `Features/Auth/` using the DB. Legacy controller can be deprecated but kept compiling until Story 1.6 cleanup.

### Architecture compliance

- **Auth:** Official `AddAuthentication().AddJwtBearer()`. [Source: architecture.md]
- **Refresh:** Opaque, hashed at rest, rotated. [Source: architecture.md — Authentication & security]
- **REST:** `/api/auth/login`, `/api/auth/refresh`. [Source: architecture.md — API patterns]
- **Rate limiting:** On login/refresh. [Source: NFR-S3]
- **Secrets:** JWT signing key from environment/user-secrets. [Source: NFR-S2]
- **Errors:** Problem Details shape. [Source: architecture.md — API errors]

### Testing requirements

- **Manual:** POST `/api/auth/login` with demo creds → get tokens; POST `/api/auth/refresh` → get new tokens, old refresh revoked; rapid-fire login → 429.
- **Automated:** Optional.

### References

- [Source: epics.md — Epic 2, Story 2.3]
- [Source: architecture.md — Authentication & security, API patterns, Rate limiting]

## Dev Agent Record

### Agent Model Used
_(fill on implementation)_
### Debug Log References
### Completion Notes List
### File List
_(fill on implementation)_
