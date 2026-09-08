# ADR-0008 — The CORS allowlist accepts a comma-separated value as well as an indexed array

- **Status:** Approved
- **Date:** 2026-09-08

## Context

`Cors:AllowedOrigins` binds to `string[]`. From environment variables that requires the
indexed form — `Cors__AllowedOrigins__0`, `__1` — which is not obvious and which most
hosting dashboards make awkward to enter.

The deployment guide told operators to set `CORS__AllowedOrigins` to a comma-separated
string. That is wrong twice over: the wrong case, and a form that does not bind to an array.
The result is an empty allowlist.

An empty allowlist is a particularly unpleasant failure. The API is healthy, every endpoint
answers correctly to `curl`, and the only symptom is an opaque CORS error in the browser
console. There is nothing in the logs pointing at the cause.

A trailing slash causes the same class of failure: the browser compares origins exactly, so
`https://tai.naindev.com/` never matches `https://tai.naindev.com`.

## Decision

`CorsOrigins.Resolve` accepts either form. A scalar value is split on commas and semicolons;
an indexed array is also split, so a mixed configuration behaves.

Each entry is normalised — trimmed, trailing slash removed — and validated as a real origin:
an absolute `http` or `https` URI with no path and no query. Entries that fail are dropped
and **named in the start-up log**, so a malformed origin is a line in the log rather than a
morning spent in the browser console.

Development still defaults to the Vite dev and preview ports, so a fresh clone runs with no
configuration. Nothing is assumed in any other environment: a default allowlist in
production would be one nobody approved.

## Consequences

- The value most operators would naturally type now works.
- The most common mistake, a trailing slash, is corrected rather than silently ignored.
- Start-up logs the allowlist it ended up with, so a deployment can be checked without a
  browser.
- Eleven tests pin the behaviour, including the case that started this: a comma-separated
  string producing an empty list.
- Splitting on commas means an origin containing a comma is impossible. Origins cannot
  contain commas, so nothing is lost.
