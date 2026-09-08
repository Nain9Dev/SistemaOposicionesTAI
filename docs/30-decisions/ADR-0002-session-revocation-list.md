# ADR-0002 — Session revocation list for logout

- **Status:** Approved
- **Date:** 2026-09-08

## Context

A JWT is self-contained: it is valid until it expires, and the server holds no state that
could say otherwise. Logging out deleted the cookies, which only affects a cooperating
client. An access token captured before logout — from a shared machine, a proxy log, a
browser extension — kept working for the rest of its lifetime.

The end-to-end verification caught this directly: after a successful logout,
`GET /api/auth/me` still returned `200` when replaying the old cookie.

Two options were on the table:

1. Shorten the access token lifetime until the window stops mattering. Cheap, but it trades
   the problem for constant rotation traffic and never fully closes it.
2. Keep a server-side revocation list.

## Decision

On logout, the session identifier (`jti`) is written to the distributed cache under
`session-revoked:{jti}`, with a time-to-live equal to the token's remaining lifetime plus a
five-minute margin for clock skew between instances.

`JwtBearerEvents.OnTokenValidated` checks that key and calls `context.Fail(...)` when it is
present, so a revoked session is rejected after signature and expiry validation succeed.

## Consequences

- Logout becomes effective immediately, even for a copied token.
- Every authenticated request costs one cache read. In a single instance that is an
  in-process dictionary lookup; with Redis it is one round trip, which is acceptable for
  the guarantee it buys.
- The list purges itself: entries never outlive the tokens they invalidate, so it cannot
  grow without bound.
- If the cache is wiped, revoked sessions become valid again until they expire. That is the
  known limit of this approach and the reason the access token lifetime stays at one hour
  rather than a day.
- Refresh-token replay revokes the refresh chain but not sibling access tokens. Those expire
  within the access-token lifetime, which is documented in `60-runbook.md` as an accepted
  residual window.
