# 10 — Requirements

Requirements use EARS notation: **when** `<trigger>`, the system **shall** `<response>`.
Each one is traced to a test in [`50-traceability.md`](50-traceability.md).

Status legend: `Implemented` · `Partial` · `Planned`.

## Syllabus

| Id | Requirement | Status |
| :--- | :--- | :--- |
| REQ-001 | When a client requests the syllabus blocks, the system shall return them in official curriculum order (I, II, III, IV), regardless of the identifiers the database assigned. | Implemented |
| REQ-002 | When a client requests topics without specifying a block, the system shall return the whole syllabus rather than an empty list. | Implemented |
| REQ-003 | When the syllabus is requested repeatedly, the system shall serve it from a distributed cache, and shall fall back to the database if the cache is unavailable. | Implemented |

## Question bank

| Id | Requirement | Status |
| :--- | :--- | :--- |
| REQ-010 | When a client requests questions, the system shall return them in random order together with their options and the index of the correct one. | Implemented |
| REQ-011 | When a block selector is supplied, the system shall accept the curriculum ordinal ("1".."4"), the roman code ("I".."IV") and the wildcard ("all"), resolving all three to the same block. | Implemented |
| REQ-012 | When a question has no correct option marked, the system shall exclude it from the response and log the anomaly, because it would corrupt marking. | Implemented |
| REQ-013 | When more questions are requested than the configured maximum, the system shall reject the request with `400`, rather than silently returning fewer. | Implemented |
| REQ-014 | When a client asks for availability, the system shall report how many questions exist for the filter and the maximum exam size. | Implemented |

## Authentication

| Id | Requirement | Status |
| :--- | :--- | :--- |
| REQ-020 | When a user registers with an email already in use, the system shall respond `409` and shall not create a second account, even under concurrent requests. | Implemented |
| REQ-021 | When authentication succeeds, the system shall deliver the access and refresh tokens exclusively in `HttpOnly` cookies, and shall never include the JWT in the response body. | Implemented |
| REQ-022 | When login fails, the system shall return the same message whether the email exists or the password is wrong, so accounts cannot be enumerated. | Implemented |
| REQ-023 | When a refresh token is presented, the system shall rotate it and revoke the previous one. | Implemented |
| REQ-024 | When an already-rotated refresh token is presented again, the system shall revoke every session of that user, because replay indicates the chain is compromised. | Implemented |
| REQ-025 | When a user logs out, the system shall reject any subsequent request carrying the same access token, even though the JWT has not yet expired. | Implemented |
| REQ-026 | When the client and the API are served from different domains, the system shall emit cookies with `SameSite=None; Secure`; in local development it shall emit `SameSite=Lax` without `Secure`, so the browser does not discard them. | Implemented |

## Authorisation and protection

| Id | Requirement | Status |
| :--- | :--- | :--- |
| REQ-030 | When a state-changing request arrives from an authenticated session, the system shall require a valid CSRF token in the `X-CSRF-Token` header. | Implemented |
| REQ-031 | When a CSRF token is issued, the system shall bind it to the session identifier (`jti`), so signing in on a second device does not invalidate the first. | Implemented |
| REQ-032 | When a session is rotated, the system shall issue a new CSRF token and shall stop accepting the previous one. | Implemented |
| REQ-033 | When repeated requests hit the authentication endpoints, the system shall rate-limit them per client origin, never as a single global quota shared by every user. | Implemented |
| REQ-034 | When an unhandled error occurs, the system shall respond with `ProblemDetails` carrying a correlation identifier, and shall not leak SQL, internal paths or stack traces outside development. | Implemented |

## Attempts

| Id | Requirement | Status |
| :--- | :--- | :--- |
| REQ-040 | When an attempt is started for a test that does not exist, the system shall respond `404`. | Implemented |
| REQ-041 | When a user operates on an attempt that belongs to someone else, the system shall respond `403`. | Implemented |
| REQ-042 | When an answer references a question outside the attempt's test, or an option outside that question, the system shall reject it with `400`. | Implemented |
| REQ-043 | When an attempt has already been finished, the system shall reject further answers and any second attempt to finish it with `409`. | Implemented |
| REQ-044 | When an attempt is finished, the system shall compute the grade under the official INAP scale (+1.00 correct, −0.33 wrong, 0.00 blank) and shall never return a negative grade. | Implemented |
| REQ-045 | When an attempt is finished, the system shall count every unanswered question as blank, taking the total from the questions actually assigned to the test. | Implemented |
| REQ-046 | When a test is served to the client, the system shall not disclose which option is correct. | Implemented |

## Progress

| Id | Requirement | Status |
| :--- | :--- | :--- |
| REQ-050 | When an attempt is submitted, the system shall recompute the grade and the blank count server side, ignoring any value the client declares. | Implemented |
| REQ-051 | When an attempt is submitted, the system shall bind it to the authenticated user, ignoring any identifier present in the payload. | Implemented |
| REQ-052 | When an attempt reports more correct plus wrong answers than the total, the system shall reject it with `400`. | Implemented |
| REQ-053 | When an attempt is stored, the system shall normalise the block label so statistics do not fragment across "1", "I" and "i". | Implemented |
| REQ-054 | When the history is paginated, the system shall clamp page and page size to a safe range on the server. | Implemented |
| REQ-055 | When statistics are requested, the system shall aggregate per block by weighted totals, not as an average of averages. | Implemented |
| REQ-056 | When identifying the weakest block, the system shall ignore blocks with too small a sample to be meaningful. | Implemented |
| REQ-057 | When a date arrives without a timezone, the system shall interpret it as UTC, because PostgreSQL rejects an unspecified kind on `timestamptz`. | Implemented |
| REQ-058 | When a user asks to clear their history, the system shall delete only their own attempts. | Implemented |

## Operations

| Id | Requirement | Status |
| :--- | :--- | :--- |
| REQ-060 | When the application starts without a valid JWT key or connection string, the system shall fail immediately rather than on the first request. | Implemented |
| REQ-061 | When the connection string is supplied as a `postgres://` URL, the system shall translate it to the driver format, honouring the `sslmode` in the query string and requiring TLS when absent. | Implemented |
| REQ-062 | When the readiness probe is called, the system shall verify the database actually responds and shall report latency without leaking connection details. | Implemented |
| REQ-063 | When no CORS origins are configured, the system shall log a warning at start-up, because the web client will be unable to reach the API. | Implemented |
