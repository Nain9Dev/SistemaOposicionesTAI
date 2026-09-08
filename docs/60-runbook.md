# 60 — Runbook

## Local start-up

### 1. Database

Use a disposable container on a non-default port so it cannot collide with another
project's PostgreSQL:

```bash
docker run -d --name tai-db -e POSTGRES_PASSWORD=tai -e POSTGRES_USER=tai -e POSTGRES_DB=oposicionestai -p 55432:5432 postgres:16-alpine
```

### 2. Migrations

Apply them in filename order. They are idempotent, so replaying the folder is safe:

```bash
for f in db/*.sql; do docker exec -i tai-db psql -U tai -d oposicionestai -v ON_ERROR_STOP=1 -q < "$f"; done
```

Check it worked — expect 30 questions and 4 blocks:

```bash
docker exec tai-db psql -U tai -d oposicionestai -c "SELECT (SELECT count(*) FROM questions) AS questions, (SELECT count(*) FROM syllabusblocks) AS blocks;"
```

### 3. Configuration

`appsettings.Development.json` already points at port 55432 with a development key. For any
other environment, supply configuration through environment variables:

```bash
export ConnectionStrings__DefaultConnection='postgres://user:pass@host:5432/db?sslmode=require'
export Jwt__Key="$(openssl rand -base64 48)"
export Cors__AllowedOrigins__0='https://tai.naindev.com'
```

### 4. Run

```bash
dotnet run --project src/Oposiciones.Api/Oposiciones.Api.csproj
```

Swagger is available at `/swagger` in development only.

## Verification

```bash
dotnet test tests/Oposiciones.UnitTests
```

```bash
bash scripts/verify-api.sh http://localhost:5298/api
```

The end-to-end script exits non-zero on the first failed assertion group, so it works as a
deployment gate. It creates its own accounts with `@example.test` addresses; run it against
a disposable database, not production.

## Configuration reference

| Key | Environment variable | Default | Notes |
| :--- | :--- | :--- | :--- |
| `ConnectionStrings:DefaultConnection` | `ConnectionStrings__DefaultConnection` | — | Accepts `postgres://` URLs and key/value form |
| `Jwt:Key` | `Jwt__Key` | — | 32 characters minimum; start-up fails otherwise |
| `Jwt:AccessTokenMinutes` | `Jwt__AccessTokenMinutes` | 60 | Also bounds the revocation-list retention |
| `Jwt:RefreshTokenDays` | `Jwt__RefreshTokenDays` | 7 | |
| `Cors:AllowedOrigins` | `Cors__AllowedOrigins__0` | Localhost in development | `AllowCredentials` forbids a wildcard |
| `AuthCookies:CrossSite` | `AuthCookies__CrossSite` | `true` outside development | `false` locally, or the browser discards the cookies over plain HTTP |
| `RateLimiting:AuthPermitLimit` | `RateLimiting__AuthPermitLimit` | 10 per minute per client | |
| `RateLimiting:PublicPermitLimit` | `RateLimiting__PublicPermitLimit` | 120 per minute per client | |
| `REDIS_URL` | `REDIS_URL` | Unset | Without it the cache is per-process |

## Diagnosing failures

Every error response carries an `X-Correlation-Id` header, echoed in the body as
`correlationId`. Search the logs for that value to find the matching entry.

| Symptom | Likely cause | Action |
| :--- | :--- | :--- |
| `503` with "Base de datos no inicializada" | Migrations were never applied | Run `db/*.sql` in order |
| `500` at start-up mentioning the JWT key | `Jwt__Key` missing or under 32 characters | Set it. This check is deliberate: the application refuses to start rather than fail on the first login |
| The client authenticates but every write returns `403` | The client is not sending `X-CSRF-Token`, or is sending a token from before a session rotation | After `/api/auth/refresh` the client must adopt the new `csrfToken` |
| Cookies never appear in the browser | `AuthCookies:CrossSite` is `true` over plain HTTP | Set it to `false` locally, or serve over HTTPS |
| `401` right after logging in on a second device | A stale build without session-scoped CSRF | Confirm the deployed version includes ADR-0001 |
| Requests are rate-limited far too aggressively | Behind a proxy every client shares the load balancer's IP | Configure `ForwardedHeaders` — task T-019 |

## Known limits

- **Cache loss reopens revoked sessions.** The revocation list lives in the cache. If it is
  wiped, a session revoked by logout becomes valid again until its access token expires
  (one hour by default). Accepted, documented in ADR-0002.
- **A single instance without Redis cannot scale out.** CSRF tokens and revocations are
  per-process, so a second instance rejects sessions established on the first.
- **Refresh-token replay does not immediately revoke sibling access tokens.** It revokes the
  refresh chain; existing access tokens expire within their lifetime.
- **The question bank ships with 30 items.** Enough to exercise the system, not enough to
  study with. Tracked as T-017.

## Rollback

Application code has no state of its own: redeploy the previous image or tag.

Migrations are forward-only and have no down scripts. `005` converts timestamp columns and
`006` replaces `AttemptFinish`; reverting either requires a manual `ALTER`. Take a backup
before applying migrations to an environment holding real data.
