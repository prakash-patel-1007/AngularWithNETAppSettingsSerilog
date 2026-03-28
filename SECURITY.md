# Security Policy

## Supported Versions

This is a sample/demo repository. Only the latest commit on `main` is actively maintained.

## Reporting a Vulnerability

If you discover a security vulnerability in this project, please report it responsibly:

1. **Do not** open a public GitHub issue for security vulnerabilities.
2. Use [GitHub's private vulnerability reporting](https://docs.github.com/en/code-security/security-advisories/guidance-on-reporting-and-writing-information-about-vulnerabilities/privately-reporting-a-security-vulnerability) on this repository, or email the maintainer directly (see the repository's profile).
3. Include a description of the vulnerability, steps to reproduce, and any potential impact.

We aim to acknowledge reports within 7 days and provide a fix or mitigation within 30 days for confirmed issues.

## Scope

This is a **sample application** intended for learning and demonstration. It should not be deployed to production without a thorough security review. Known simplifications include:

- In-memory user store with plain-text passwords (migration to hashed passwords with SQLite is planned).
- JWT signing key stored in `appsettings.json` (use environment variables or user-secrets in production).
- No HTTPS enforcement in the default development configuration.
