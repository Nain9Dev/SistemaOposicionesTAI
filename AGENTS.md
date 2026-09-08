# AGENTS.md — Sistema Oposiciones TAI (backend)

Canonical instructions for any agent working in this repository. Read this before writing
code, then read [`docs/README.md`](docs/README.md).

## Working method

This repository follows **Spec Driven Development**. Every change starts in a document.

1. If a request is not covered by `docs/10-requirements.md`, add the requirement first.
2. One task at a time, from `docs/40-tasks.md`, with the test written before the code.
3. A requirement with no test in `docs/50-traceability.md` is not done, whatever the code
   says.
4. Record significant decisions as an ADR in `docs/30-decisions/` with status `Proposed`.
   Only the repository owner promotes one to `Approved`.
5. `docs/` is the source of truth. Code that contradicts an `Approved` document is a defect.
6. Before running out of context, dump the state (done, verified, next, blocked) into the
   handoff section of `docs/40-tasks.md`.

## Language

Everything that lands in the repository is written in **English**: code, identifiers, file
and folder names, branches, commits, comments, documentation, tests and configuration.

Two exceptions:

1. Text shown to the end user follows the product locale, which is Spanish. API error
   messages are user-facing, so they stay in Spanish.
2. Legal or administrative terms with no exact equivalent (`TAI`, `INAP`, `oposición`) are
   kept as values with an English comment explaining them.

Public contract identifiers already in Spanish (`/api/preguntas`, `/api/progreso`,
`PreguntaDto`) are **not** renamed for style. They are a published contract with a deployed
client.

## Architecture rules

Clean Architecture, four projects, dependencies pointing inwards. See
[`docs/20-architecture.md`](docs/20-architecture.md).

| Rule | Why |
| :--- | :--- |
| `Domain` has no package references | It must stay describable without any infrastructure |
| Business decisions live in `Application` | A rule stated in two layers will diverge |
| Controllers hold no business logic | They resolve identity, delegate, translate the result |
| Only the composition root touches `Infrastructure` | No controller imports a repository |
| Repositories return `IReadOnlyList<T>` | Forces materialisation before the connection closes |
| Every public async method takes a `CancellationToken` | An abandoned request must not keep a connection busy |

## Non-negotiables

These exist because each one was a real defect. Do not undo them.

- **The server computes results.** The client sends observations. Never bind a request body
  straight to a domain entity — ADR-0006.
- **The JWT never reaches JavaScript.** `HttpOnly` cookies plus a session-scoped CSRF token
  — ADR-0001.
- **Logout revokes the session**, it does not merely delete the cookie — ADR-0002.
- **Every timestamp column is `timestamptz`** and the application always writes UTC —
  ADR-0003.
- **The block selector is a curriculum ordinal, not a primary key** — ADR-0004.
- **PostgreSQL functions are called with `SELECT`**, never `CommandType.StoredProcedure` —
  ADR-0005.
- **Ownership is checked before acting**, not after.
- **Pagination is clamped server side.**

## Database

Migrations are plain `.sql` under `db/`, applied in filename order, forward only. Write them
idempotent where it is cheap. Never edit a migration that has already been applied to a
shared environment — add a new one.

Identifiers are unquoted, so PostgreSQL folds them to lower case. Dapper matches columns
case-insensitively, which is why C# keeps pascal casing.

## Verification

Nothing is done until both of these pass:

```bash
dotnet test tests/Oposiciones.UnitTests
bash scripts/verify-api.sh
```

The end-to-end script needs a running instance and a migrated database. Use a disposable
container on a non-default port — see [`docs/60-runbook.md`](docs/60-runbook.md). Never
point it at a database another project owns.

## Commits

Conventional Commits, subject in English: `<type>(<scope>): <description>`.

Types: `feat`, `fix`, `refactor`, `perf`, `docs`, `test`, `build`, `ci`, `chore`.
Contract breaks carry `!`. No dates in the message — git already records them.

Do not commit or push unless explicitly asked.

## Secrets

Never commit a connection string, a JWT key or any credential. `appsettings.json` holds
placeholders only; real values arrive through environment variables.
