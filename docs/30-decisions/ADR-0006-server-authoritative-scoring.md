# ADR-0006 — The server is authoritative for marking

- **Status:** Approved
- **Date:** 2026-09-08

## Context

`POST /api/progreso` bound its request body straight to the `IntentoUsuario` domain entity.
The client therefore controlled every field, including `Nota` (the grade), `Id` and
`UsuarioId`. The controller overwrote `UsuarioId` but nothing else, so a candidate could
post a perfect grade for an exam they never sat, and it would be stored verbatim and counted
in their statistics.

Three different marking implementations also existed: the client's `calculateINAPScore`,
whatever the client chose to send, and `AttemptFinish` in the database — which used raw
percentage correct rather than the official scale, so the same exam produced different
grades depending on which route recorded it.

## Decision

The request body is a dedicated `IntentoDto` that does not expose `Nota`, `Id`,
`UsuarioId` or `Blancos`. It carries only observations: correct, wrong, total, block and
date.

`ProgresoService` recomputes the grade and the blank count through `ScoringService`, and
binds the row to the authenticated user. `AttemptFinish` was rewritten to the same scale so
both routes agree.

Incoherent input — a total of zero, or correct plus wrong exceeding the total — is rejected
with `400` rather than clamped silently, because it indicates a client bug worth surfacing.

## Consequences

- A tampered client can lie about how many questions it got right, but not about what that
  is worth. Closing the remaining gap requires the server to own the answers too, which is
  exactly what the attempt endpoints do; `/api/progreso` remains the offline-sync path.
- The marking scale is stated once in C# and once in SQL, and both are pinned by tests to
  the same constants. See [ADR-0005](ADR-0005-postgres-functions-called-with-select.md) for
  why the SQL copy exists.
- The client can no longer choose its own row id, so `IntentoUsuario.Id` is trustworthy as
  a stable key for offline reconciliation.
