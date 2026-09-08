# ADR-0001 — Access token in an HttpOnly cookie, with a session-scoped CSRF token

- **Status:** Approved
- **Date:** 2026-08-24
- **Supersedes:** the original design, which returned the JWT in the response body

## Context

The client is a single-page application that may be served from a different domain than the
API. The first implementation returned the JWT in the login response and the client stored
it, which makes any cross-site scripting flaw a full account takeover: the token is readable
from JavaScript and long-lived.

Moving the token into an `HttpOnly` cookie removes that exposure, but introduces the mirror
problem: the browser attaches the cookie to cross-site requests on its own, so the cookie by
itself no longer proves the user intended the request.

## Decision

The access and refresh tokens travel exclusively in `HttpOnly` cookies. The login response
carries a CSRF token, which the client echoes in the `X-CSRF-Token` header on every
state-changing request.

The CSRF token is stored in the distributed cache under the **session identifier** (`jti`)
carried by the JWT, not under the user id.

Cookie attributes follow the deployment topology: `SameSite=None; Secure` when the client is
on another domain, `SameSite=Lax` without `Secure` in local development, because a browser
discards a `Secure` cookie served over plain `http://localhost`.

## Consequences

- An XSS flaw can still issue requests as the user, but cannot exfiltrate a reusable token.
- Keying by `jti` instead of user id means signing in on a second device no longer
  invalidates the first. The previous design overwrote a single `csrf_{userId}` entry, so
  the older device started receiving `403` on every write with no explanation.
- Rotating the session issues a new CSRF token, and the previous one stops working. The
  client must adopt the token returned by `/api/auth/refresh`.
- Without Redis the cache is per-process, so more than one instance requires
  `REDIS_URL`. The application logs a warning at start-up when that is not configured.
- Logout needs a revocation list; see [ADR-0002](ADR-0002-session-revocation-list.md).
