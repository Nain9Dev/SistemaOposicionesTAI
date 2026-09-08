# ADR-0003 — Every timestamp column is `timestamptz`

- **Status:** Approved
- **Date:** 2026-09-08

## Context

The original schema declared every timestamp as `TIMESTAMP` (without time zone). Every value
the application writes comes from `DateTime.UtcNow`, which produces `Kind = Utc`.

Since Npgsql 6, writing a `DateTime` with `Kind = Utc` into a
`timestamp without time zone` column throws `InvalidCastException`. The mapping is
deliberately strict: the driver refuses to guess whether the caller means an absolute
instant or a wall-clock reading.

The practical effect was that user registration and refresh-token issuance both failed at
runtime against a correctly migrated database. The failure never surfaced during
development because the affected paths had no coverage.

The mirror hazard also applies: a date arriving from the client without a zone has
`Kind = Unspecified`, which `timestamptz` rejects for the same reason.

## Decision

Every timestamp column is `timestamptz`. Migration `005_progress_schema.sql` converts the
existing ones with `USING <column> AT TIME ZONE 'UTC'`, which reads the stored naive values
as the UTC instants they always were.

At the boundary, `ProgresoService.NormalizeToUtc` interprets an unspecified kind as UTC and
converts a local one, so a client date can never reach the driver in a state it rejects.

## Consequences

- Instants round-trip as instants. A candidate in Madrid and one in the Canary Islands see
  their attempts on the same absolute timeline.
- The `AttemptFinish` function returns `timestamptz`, so its signature changed and the
  migration drops it before recreating it — `CREATE OR REPLACE` cannot change a return type.
- Existing rows are reinterpreted, not shifted, so no data is lost by the conversion.
- The rule is now simple enough to hold in one's head: the database stores instants, the
  application produces UTC, and the client is free to render whatever local time it wants.
