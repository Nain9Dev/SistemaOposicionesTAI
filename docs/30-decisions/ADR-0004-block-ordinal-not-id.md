# ADR-0004 — The block selector is a curriculum ordinal, not a database id

- **Status:** Approved
- **Date:** 2026-09-08

## Context

The client offers the four syllabus blocks as `"1"` to `"4"`, plus `"all"`. The database
codes them in roman numerals and gives them an `IDENTITY` primary key.

`002_seed_minimal.sql` inserted the blocks from a `VALUES` list with no `ORDER BY`.
PostgreSQL is free to materialise that in any order, and in practice it did: on a freshly
migrated database, block IV was assigned `Id = 1` and block I got `Id = 2`.

Interpreting the client's `"1"` as an id therefore returned block IV — and because block IV
has no questions seeded, `GET /api/preguntas/bloque/1` returned an empty array with a
`200 OK`. A silent wrong answer, which is worse than an error.

Ordering blocks by `Id` was broken for the same reason: `GET /api/syllabus/blocks` listed
them as IV, I, III, II.

## Decision

The selector is the block's **position in the syllabus**, never its primary key.
`BlockSelector.Parse` maps `1..4` to `I..IV` and filters by code. A number outside that
range is still treated as a literal id, which keeps a direct-id query available for API
consumers that genuinely know one.

Ordering is expressed as an explicit `CASE` over the code rather than by `Id` or
alphabetically — alphabetical ordering happens to be correct for I, II, III, IV but would
place IX before V.

The seed now carries an `ORDER BY` so identifiers are deterministic on a fresh database.
That is defence in depth: no code path depends on it any more.

## Consequences

- `"1"`, `"I"` and `"i"` all resolve to the same block, and `BlockSelector.Normalize`
  collapses them to `"I"` before an attempt is stored, so per-block statistics stop
  fragmenting across spellings.
- Renumbering or reseeding the syllabus cannot silently change what the client asks for.
- A block outside the four official ones is passed through as an uppercase code and simply
  matches nothing, rather than colliding with an unrelated identifier.
