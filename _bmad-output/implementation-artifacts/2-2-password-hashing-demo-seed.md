# Story 2.2: Password hashing and demo user seed

Status: review

## Story

As a **developer**,
I want **passwords stored with a strong hasher and documented demo accounts for local use**,
so that **FR1 has real credentials without hard-coded checks in code** (Epic 2).

## Acceptance Criteria

1. **Given** a new user row **when** a password is saved **then** only a hash/stored secret suitable for verification is persisted (NFR-S4 via parameterized EF).
2. **Given** Development environment **when** seed runs **then** at least one demo user exists and credentials are documented in README, not committed as production defaults.

## Tasks / Subtasks

- [ ] **AC1 — Password hashing**
  - [ ] Implement password hashing using `Microsoft.AspNetCore.Identity.PasswordHasher<User>` or a vetted BCrypt library. Register as a service.
  - [ ] Create a service/helper (e.g. `Features/Auth/Services/PasswordService.cs`) that exposes `HashPassword(string)` and `VerifyPassword(string hash, string provided)`.
  - [ ] Verify: stored `PasswordHash` in the DB is NOT plaintext.
- [ ] **AC2 — Demo user seed**
  - [ ] Create seed logic that runs on startup in Development environment only (guard with `IWebHostEnvironment.IsDevelopment()` or equivalent).
  - [ ] Seed at least two demo users: `admin` / `admin` (with admin-level permissions) and `user` / `user` (standard permissions). Hash passwords before storing.
  - [ ] Document demo credentials in README under a "Demo Accounts" section.
  - [ ] Ensure seed is idempotent — does not create duplicates on restart.
- [ ] **Green build check**
  - [ ] `dotnet build -c Release` succeeds.

## Dev Notes

### Scope boundary (critical)

- **In scope:** Password hashing service, demo user seeding, README credential docs.
- **Out of scope (Story 2.3):** Login endpoint, JWT issuance, refresh tokens.
- **Out of scope:** User registration endpoint (not in MVP PRD).

### Previous story intelligence (Story 2.1)

- `AppDbContext` with `DbSet<User>` exists. `User` entity has `PasswordHash` field.
- SQLite DB initialized with `Database.Migrate()` at startup.

### Architecture compliance

- **Password storage:** Strong hash (PasswordHasher or BCrypt). [Source: architecture.md — Authentication & security]
- **Secrets:** Demo credentials documented in README, not committed as production defaults. [Source: NFR-S2]
- **Seed:** Development only, idempotent. [Source: architecture.md — Data flow]

### References

- [Source: epics.md — Epic 2, Story 2.2]
- [Source: architecture.md — Authentication & security]

## Dev Agent Record

### Agent Model Used
_(fill on implementation)_
### Debug Log References
### Completion Notes List
### File List
_(fill on implementation)_
