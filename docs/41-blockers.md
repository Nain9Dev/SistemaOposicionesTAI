# 41 — Blockers

Everything external the work needs: what it is, where to get it, which variable holds it,
what it unblocks and how to check it is ready.

## Required to run

| Dependency | What it is | Where | Variable | Unblocks | Ready when |
| :--- | :--- | :--- | :--- | :--- | :--- |
| PostgreSQL 14+ | Primary datastore | Local, Docker, Neon, Render | `ConnectionStrings__DefaultConnection` | Everything | `GET /api/health/db` returns `{"status":"ok"}` |
| JWT signing key | 32+ character secret | Generated locally, never committed | `Jwt__Key` | Authentication | The application starts without throwing |
| CORS origins | Web client origins | Deployment topology | `Cors__AllowedOrigins__0`, `__1`, … | Browser access | No `No CORS origins configured` warning at start-up |

Generate a key with:

```bash
openssl rand -base64 48
```

## Optional

| Dependency | What it is | Variable | Consequence when absent |
| :--- | :--- | :--- | :--- |
| Redis | Distributed cache for CSRF tokens, session revocation and syllabus | `REDIS_URL` | Falls back to an in-process cache. Correct on a single instance; with more than one, a session established on instance A is rejected on instance B |

## Human-only actions

| Id | Action | Why it cannot be automated |
| :--- | :--- | :--- |
| `[H]` | Provision the production database | Requires an account and, beyond the free tier, a payment method |
| `[H]` | Generate and store the production `Jwt__Key` | A secret must never pass through an agent's context or a commit |
| `[H]` | Enable GitHub Actions on the repository | Repository setting, owner only |
| `[H]` | Author question content and explanations | Editorial and legal decision about the source material |

## Currently blocking

Nothing. Every `[A]` task in [`40-tasks.md`](40-tasks.md) can proceed with a local
PostgreSQL instance.

## Note on verification

The end-to-end suite needs a running instance and a migrated database. During development
that is a disposable container on a non-default port, so it never touches another project's
PostgreSQL:

```bash
docker run -d --name tai-verify-db \
  -e POSTGRES_PASSWORD=verify -e POSTGRES_USER=verify -e POSTGRES_DB=oposicionestai \
  -p 55432:5432 postgres:16-alpine
```

Full instructions are in [`60-runbook.md`](60-runbook.md).
