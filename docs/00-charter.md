# 00 — Project charter

## Goal

Provide the backend for an exam-simulation platform for the Spanish **TAI** civil service
examination (Técnico Auxiliar de Informática, INAP). The system serves a question bank
drawn from the official syllabus, records each candidate's attempts and reports where their
performance is weakest, so study time goes where it actually pays off.

## Scope

- REST API over the official syllabus: blocks, topics and question bank.
- Randomised exam generation filtered by block, topic and difficulty.
- Attempt lifecycle: start, answer, finish, with the official INAP marking scale applied
  server side.
- Candidate accounts with session-cookie authentication and progress history.
- Aggregate statistics: accuracy, average grade, per-block breakdown and trend.

## Non-goals

- Content authoring. The question bank is loaded through migrations, not through the API.
- Payments, subscriptions or any commercial tier.
- Notifications, email delivery or scheduling.
- Multi-tenancy. A single instance serves a single candidate population.
- Serving the web client. The frontend is a separate repository
  (`tai-study-system-js`) deployed independently.

## Constraints

| Constraint | Reason |
| :--- | :--- |
| .NET 10 with Clean Architecture | Existing codebase and the technical showcase the project doubles as |
| PostgreSQL with Dapper, no ORM | Explicit SQL is part of the demonstrated skill set; migrations are plain `.sql` |
| The client may be hosted on a different domain | Cookies must work cross-site, which forces `SameSite=None` plus CSRF protection |
| Free hosting tiers | No dependency may be assumed always-on: Redis is optional and the API degrades to an in-process cache |
| Guest mode is a product feature | The question bank is readable without an account |

## Success criteria

1. A candidate can register, sit an exam and see their grade under the official scale
   (+1.00 correct, −0.33 wrong, 0.00 blank) without any client-side calculation.
2. The grade recorded in the history is the one computed by the server. A tampered client
   cannot declare its own result.
3. `scripts/verify-api.sh` passes end to end against a freshly migrated database.
4. Unit tests cover every business rule listed in `50-traceability.md`.
5. A cold start on a clean database requires only running `db/*.sql` in order.

## Definition of done for the project

The API is done when the web client can run the entire study flow — syllabus, exam,
marking, history and statistics — against the API alone, with the static JSON catalogue
acting purely as an offline fallback and never as the primary source of truth.
