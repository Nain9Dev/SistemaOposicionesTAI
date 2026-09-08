# 40 — Tasks

Label legend: `[A]` agent completes it alone · `[M]` mixed, needs a human action to close ·
`[H]` human only.

## Done

| Id | Task | Label | Done when |
| :--- | :--- | :--- | :--- |
| T-001 | Serve the question bank from the database (`PreguntasController`) | `[A]` | ✅ The client's study flow runs against the API, not the static catalogue |
| T-002 | Create the missing `IntentosUsuario` table | `[A]` | ✅ `/api/progreso` works; the table existed nowhere in `db/` |
| T-003 | Call PostgreSQL functions with `SELECT` instead of `CommandType.StoredProcedure` | `[A]` | ✅ Test generation and the attempt lifecycle work end to end |
| T-004 | Convert every timestamp column to `timestamptz` | `[A]` | ✅ Registration and refresh-token issuance stop throwing |
| T-005 | Make the server authoritative for marking | `[A]` | ✅ `a client-declared grade is ignored` passes |
| T-006 | Enforce attempt ownership | `[A]` | ✅ `another account cannot close the attempt` returns `403` |
| T-007 | Scope the CSRF token to the session (`jti`) | `[A]` | ✅ A second device does not invalidate the first |
| T-008 | Session revocation list so logout is effective | `[A]` | ✅ `the session stops working after logout` passes |
| T-009 | Partition the rate limiter per client | `[A]` | ✅ One caller can no longer exhaust the global quota |
| T-010 | Fix the block ordinal / identifier confusion | `[A]` | ✅ `/api/preguntas/bloque/1` returns block I |
| T-011 | `ProblemDetails` error handling with a correlation identifier | `[A]` | ✅ Business errors stop surfacing as `500` |
| T-012 | Single connection factory over `NpgsqlDataSource` | `[A]` | ✅ No repository builds its own connection string |
| T-013 | Unit test project and end-to-end verification script | `[A]` | ✅ 70 unit tests and 48 assertions, all green |
| T-016 | Seed the question bank from migrations | `[A]` | ✅ 30 questions, idempotent on replay |
| T-021 | Fix the container build, broken by the test project joining the solution | `[A]` | ✅ `docker build` succeeds; the unit suite gates the image |
| T-022 | Bind to the port the platform injects, at run time | `[A]` | ✅ `PORT=10000` produced `Now listening on: http://[::]:10000` |
| T-023 | Accept both CORS configuration forms and report rejected origins | `[A]` | ✅ 11 tests; the allowlist is logged at start-up |
| T-024 | Rewrite the deployment guide against the real migrations and variables | `[A]` | ✅ All eight migrations listed; a verification step that catches a stale deploy |

## Next

| Id | Task | Label | Done when |
| :--- | :--- | :--- | :--- |
| T-014 | Integration test project with `WebApplicationFactory` and Testcontainers | `[A]` | The requirements marked `— (manual)` in `50-traceability.md` have automated coverage |
| T-015 | CI workflow: build, unit tests, migrations against a service container, `verify-api.sh` | `[M]` | The workflow runs green on a pull request. Needs the repository owner to enable Actions |
| T-017 | Expand the question bank beyond the 30 seeded items | `[H]` | Content is a business decision, not an engineering one |
| T-018 | Populate `Questions.Explanation` | `[H]` | The column exists and the API already returns it; the text has to be written by a human |
| T-019 | `ForwardedHeaders` middleware for correct client IPs behind a proxy | `[M]` | Rate limiting partitions by real client IP in production. Needs to know the hosting topology |
| T-020 | Structured logging with an aggregator | `[M]` | Correlation identifiers are searchable outside the console |
| T-025 | Redeploy Render from `main` and apply the migrations to Neon | `[H]` | `scripts/verify-api.sh` passes against the production URL. Needs Render and Neon account access |
| T-026 | Automate migrations on deploy, or add a start-up schema check | `[A]` | A deployment cannot start against a database missing its tables |

## Session handoff

**State as of 2026-09-08**

- **Done:** the backend rewrite described above. Solution builds with zero warnings. Unit
  suite 70/70. End-to-end suite 48/48 against PostgreSQL 16 with all eight migrations
  applied from scratch.
- **Verified:** by running both suites against a disposable container, not by inspection.
  The three bugs the end-to-end run surfaced (block ordinal, stale CSRF after rotation,
  logout not revoking the token) are fixed and now covered.
- **Next:** T-025 is the one that matters — production is still serving a build that
  predates all of this, which is why the client sees 404 on every `/api/preguntas` call.
  It needs account access, so it is `[H]`. Then T-026, T-014, T-015.
- **Blocked:** nothing that can be done from the repository. See
  [`41-blockers.md`](41-blockers.md).
