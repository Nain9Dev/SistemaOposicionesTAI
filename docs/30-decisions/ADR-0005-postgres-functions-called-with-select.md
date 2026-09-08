# ADR-0005 — PostgreSQL functions are invoked with `SELECT`, not `CommandType.StoredProcedure`

- **Status:** Approved
- **Date:** 2026-09-08

## Context

The repositories called `TestGenerate`, `AttemptStart`, `AttemptAnswerUpsert` and
`AttemptFinish` through Dapper with `commandType: CommandType.StoredProcedure`. That style
is carried over from SQL Server, where it is idiomatic.

Since Npgsql 7, `CommandType.StoredProcedure` emits `CALL`, which in PostgreSQL only works
for a `PROCEDURE`. All four objects are declared as `FUNCTION`, so every call failed at
runtime. Parameter naming compounded it: Dapper sends `@TestId` while the function declares
`p_TestId`.

## Decision

Functions are called with explicit SQL and explicit argument casts:

```sql
SELECT TestGenerate(@Title::VARCHAR, @SyllabusTopicId::INT, @Difficulty::SMALLINT, @TotalQuestions::INT);
SELECT * FROM AttemptFinish(@AttemptId::BIGINT);
```

Casting each argument pins overload resolution rather than relying on the implicit
`text`-to-`varchar` coercion, which is binary-coercible but not guaranteed to pick the
intended overload once a second one exists.

`AttemptStart` and the answer upsert moved out of functions and into statements in the
repository, because both needed validation the original functions did not perform:
`AttemptStart` has to distinguish "test does not exist" from a foreign-key violation, and
the answer upsert has to reject a question outside the attempt's test. Expressing those as
`INSERT ... SELECT` with joins keeps the check atomic with the write.

`AttemptFinish` stays a function: it holds the marking logic and reading every answer over
the wire to add them up in C# would be strictly worse.

## Consequences

- The attempt endpoints work. They could not have worked before this change.
- Callers see the SQL they are actually running, which makes the parameter-name mismatch
  impossible to reintroduce silently.
- `AttemptAnswerUpsert` remains in `004_attempts_procedures.sql` but is no longer called.
  It is left in place so the migration folder stays replayable as written.
