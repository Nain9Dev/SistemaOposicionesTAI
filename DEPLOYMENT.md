# Deployment

Target architecture, all on free tiers:

| Component | Platform |
| :--- | :--- |
| Database | Neon (PostgreSQL 16) |
| API | Render (Docker web service) |
| Web client | Vercel or GitHub Pages, from `tai-study-system-js` |

## 1. Database

1. Create a Neon project on PostgreSQL 16 or later and copy the connection URI. It looks
   like `postgres://user:password@host.neon.tech/neondb?sslmode=require`.

2. **Apply every migration in `db/`, in filename order.** All eight of them:

   ```
   001_create_schema.sql
   002_add_refresh_tokens.sql
   002_seed_minimal.sql
   003_procedures.sql
   004_attempts_procedures.sql
   005_progress_schema.sql
   006_attempt_scoring.sql
   007_seed_questions.sql
   ```

   Paste each one into Neon's SQL Editor in that order, or run them with `psql`:

   ```bash
   for f in db/*.sql; do psql "$DATABASE_URL" -v ON_ERROR_STOP=1 -f "$f"; done
   ```

3. Verify. Expect 30 questions, 120 options and 4 blocks:

   ```sql
   SELECT (SELECT count(*) FROM questions)      AS questions,
          (SELECT count(*) FROM answeroptions)  AS options,
          (SELECT count(*) FROM syllabusblocks) AS blocks,
          (SELECT count(*) FROM intentosusuario) AS attempts;
   ```

   If `intentosusuario` does not exist, migration `005` was skipped and `/api/progreso`
   will fail at run time. Earlier revisions of this guide pointed at a single script that
   was never in the repository, which is exactly how that happens.

## 2. API on Render

1. **New → Web Service**, connect the `SistemaOposicionesTAI` repository, language
   **Docker**, branch `main`, instance type **Free**.

2. Environment variables:

   | Variable | Value |
   | :--- | :--- |
   | `ASPNETCORE_ENVIRONMENT` | `Production` |
   | `ConnectionStrings__DefaultConnection` | The Neon URI |
   | `Jwt__Key` | A secret of at least 32 characters |
   | `Jwt__Issuer` | `OposicionesTAI` |
   | `Jwt__Audience` | `OposicionesTAIUsers` |
   | `Cors__AllowedOrigins` | The client origins, comma separated |
   | `AuthCookies__CrossSite` | `true` when the client is on another domain |

   Generate the key with:

   ```bash
   openssl rand -base64 48
   ```

   The API refuses to start with a missing or short key. That is deliberate: failing at
   start-up is better than failing on the first sign-in.

   `Cors__AllowedOrigins` accepts either a comma-separated list or the indexed form
   (`Cors__AllowedOrigins__0`, `__1`, …). Each entry must be scheme, host and port only —
   `https://tai.naindev.com`, never `https://tai.naindev.com/`. A trailing slash makes the
   browser's origin comparison fail, and the only symptom is an opaque CORS error with a
   perfectly healthy-looking API. Rejected entries are named in the start-up log.

3. Render injects `PORT`; the container reads it at start-up. No extra configuration.

4. **Keep-alive.** The free tier sleeps after 15 minutes of inactivity, and a cold start
   takes 30–60 seconds. Point a scheduler such as [cron-job.org](https://cron-job.org/) at
   `https://<service>.onrender.com/api/health` every 10 minutes. That endpoint touches no
   dependencies, so it wakes the service without loading the database.

## 3. Web client

Set `VITE_API_BASE_URL` to the Render URL plus `/api`, then deploy. Vite inlines the value
at build time, so changing it requires a rebuild, not just a restart.

The client's origin has to appear in `Cors__AllowedOrigins`. `AllowCredentials` forbids the
wildcard, so it must be listed exactly.

## 4. Verify the deployment

Do not assume a green build means a working API. Run the end-to-end suite against it:

```bash
bash scripts/verify-api.sh https://<service>.onrender.com/api
```

48 assertions covering the syllabus, the question bank, registration, CSRF enforcement,
progress and the whole attempt lifecycle. It creates its own `@example.test` accounts, so
run it against a staging database rather than one holding real candidates' work.

A quick manual check of the endpoints most likely to reveal a stale deployment:

```bash
curl -s https://<service>.onrender.com/api/health
```

```bash
curl -s "https://<service>.onrender.com/api/preguntas/bloque/1?cantidad=3"
```

`/api/health` returning 404 means the deployed image predates the backend hardening
release. `/api/preguntas/*` returning 404 means the same thing, and it is what the web
client hits first.

## Deployment checklist

- [ ] All eight migrations applied, in order, and the counts verified
- [ ] `Jwt__Key` set, 32 characters or more, and not the placeholder
- [ ] `ConnectionStrings__DefaultConnection` set to the Neon URI
- [ ] `Cors__AllowedOrigins` lists the client origin with no trailing slash
- [ ] `AuthCookies__CrossSite` matches the topology
- [ ] Start-up log shows the allowed origins and no rejected ones
- [ ] `scripts/verify-api.sh` passes against the deployed URL
- [ ] Keep-alive scheduler hitting `/api/health`
- [ ] Client rebuilt with the correct `VITE_API_BASE_URL`

## Known limits of the free tier

- **Cold starts.** The first request after sleep takes 30–60 seconds. The client's 10-second
  timeout will fire and it falls back to the bundled catalogue, so the candidate sees the
  offline notice rather than a hang. The keep-alive scheduler is what avoids it.
- **No Redis.** Without `REDIS_URL` the cache is per-process, so CSRF tokens and session
  revocations do not survive a restart or reach a second instance. Fine for one free
  instance; not fine the moment you scale out.
- **Neon suspends idle projects**, which adds its own cold start on top of Render's.
