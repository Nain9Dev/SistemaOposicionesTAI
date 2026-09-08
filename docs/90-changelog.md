# 90 — Changelog

## 2026-09-08 — Deployment fixes

Production was still serving a build that predated the backend hardening release, so every
`/api/preguntas` call from the web client returned 404. Investigating why a redeploy had
not happened turned up the reason it could not: the container build was broken.

### Fixed

- **The container build failed.** Adding the test project to the solution meant
  `dotnet restore` could no longer resolve the graph from the layer that copies only the
  four `src/` project files. Every build died with `MSB3202`, and the image had not been
  rebuilt since — ADR-0007.
- **The port was baked in at build time.** `ENV ASPNETCORE_URLS=http://+:$PORT` resolves
  when the image is built, where `PORT` is unset, producing `http://+:`. Docker had been
  warning about it. Now resolved at start-up, verified by running the image with
  `PORT=10000` — ADR-0007.
- **The CORS configuration in the deployment guide could never have worked.**
  `Cors:AllowedOrigins` binds to `string[]`, which from environment variables needs the
  indexed form; the guide specified a comma-separated value under the wrong key. Both forms
  are now accepted, entries are validated, and the resolved allowlist is logged — ADR-0008.
- **The deployment guide pointed at a migration script that is not in the repository**, and
  never mentioned the eight that are. A deployment following it would come up against a
  database with no `IntentosUsuario` table.

### Changed

- The unit suite runs inside the container build, so a failing business rule stops the image
  rather than reaching production.
- Added `.dockerignore`; `COPY . ./` was pulling host `bin/` and `obj/` into the image.
- The container runs as the runtime image's non-root account.
- `DEPLOYMENT.md` rewritten: every migration listed in order, a verification step that
  catches a stale deployment, and a checklist.

### Verification

- `docker build` succeeds and runs 81 unit tests as part of the build.
- The image, run in `Production` mode against a freshly migrated PostgreSQL 16, passes
  all 48 end-to-end assertions. The previous run had been in `Development`, so this is the
  first time the production cookie and CORS configuration has been exercised.

## 2026-09-08 — Backend rewrite

The study flow never reached the database. `/api/preguntas` had no controller, the table
behind `/api/progreso` was never created, and every PostgreSQL function was invoked in a way
that fails on Npgsql 7. The client therefore ran permanently on its static fallback, which
is why none of it showed up as a visible failure.

### Fixed — the system did not work

- **The question bank had no endpoint.** Added `PreguntasController`, `StudyQuestionService`
  and `StudyQuestionRepository`. The client's study flow now runs against the API.
- **`IntentosUsuario` did not exist.** `ProgresoRepository` queried a table absent from every
  migration. Created in `005` with its indexes and check constraints.
- **PostgreSQL functions were unreachable.** `CommandType.StoredProcedure` emits `CALL`,
  which only works for procedures; all four objects are functions. Replaced with explicit
  `SELECT` and typed arguments — ADR-0005.
- **Registration and refresh-token issuance threw.** Npgsql 6 rejects `Kind=Utc` on
  `timestamp without time zone`. Every timestamp column converted to `timestamptz` — ADR-0003.
- **`/api/preguntas/bloque/1` returned an empty array with `200 OK`.** The block ordinal was
  being treated as a primary key, and the seed left those keys in arbitrary order — ADR-0004.
- **`/api/syllabus/blocks` listed the syllabus as IV, I, III, II.** Ordering by identifier
  instead of curriculum position.

### Fixed — security

- **The client set its own grade.** `POST /api/progreso` bound straight to the domain entity,
  so `Nota`, `Id` and `UsuarioId` were all client-controlled. Replaced with a DTO that
  carries observations only; the server recomputes — ADR-0006.
- **Attempts were unauthenticated and unowned.** Anyone could answer or close anyone's
  attempt by guessing an identifier. `[Authorize]` plus ownership checks in `AttemptService`.
- **Answers were not validated against the attempt's test.** A single statement now checks
  that the question belongs to the test, the option belongs to the question and the attempt
  is still open.
- **Logout left the token valid.** Deleting the cookie does not invalidate a JWT. Added a
  session revocation list — ADR-0002.
- **A second sign-in broke the first device.** CSRF tokens were keyed by user, so each login
  overwrote the previous. Now keyed by session id — ADR-0001.
- **The rate limiter was one global quota.** Five login attempts per minute for the entire
  world; one attacker could lock everyone out. Partitioned per client.
- **`AttemptFinish` used raw percentage correct**, disagreeing with the client's INAP
  calculation. Rewritten to the official scale.
- **Business errors surfaced as `500`.** `ProblemDetails` with a correlation identifier, and
  no SQL or stack traces outside development.
- **Pagination was unbounded.** `page=0` produced a negative `OFFSET`; page size had no cap.

### Changed

- Single `IDbConnectionFactory` over a singleton `NpgsqlDataSource`; repositories no longer
  build their own connection strings.
- `CancellationToken` propagated from every controller through to every query.
- `SELECT *` replaced with explicit column lists.
- Email uniqueness resolved by the database rather than a check-then-insert race.
- Login runs a throwaway hash when the account does not exist, so failure timing does not
  reveal which emails are registered.
- Refresh-token rotation records its chain and revokes every session on replay.
- CORS origins and rate limits moved into configuration.
- Cookie attributes follow the environment: `SameSite=None; Secure` cross-site,
  `SameSite=Lax` locally.
- Syllabus caching survives a cache outage instead of failing the request.
- `Class1.cs` placeholders removed.

### Added

- `tests/Oposiciones.UnitTests` — 70 tests over the marking scale, block selector,
  connection-string translation, pagination, attempt ownership and progress rules.
- `scripts/verify-api.sh` — 48 end-to-end assertions against a live instance. Replaces
  `test-api.sh`.
- `db/005`, `db/006`, `db/007` — progress schema, INAP marking in SQL, and an idempotent
  seed of 30 questions.
- `GET /api/auth/me`, `DELETE /api/progreso/historial`,
  `GET /api/preguntas/disponibilidad`, `GET /api/health`.
- Full SDD documentation under `docs/`, including six ADRs.

### Verification

Solution builds with zero warnings. Unit suite 70/70. End-to-end suite 48/48 against
PostgreSQL 16 with all eight migrations applied to an empty database.

Three of those end-to-end assertions were written before the behaviour existed and failed
on first run: the block ordinal, the stale CSRF token after rotation, and logout not
revoking the access token. All three are now fixed and covered.
