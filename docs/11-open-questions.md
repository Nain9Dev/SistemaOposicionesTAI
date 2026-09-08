# 11 — Open questions

Unresolved ambiguity. Each entry blocks the requirement it references until it is answered.

## OQ-001 — Should the question bank stay publicly readable?

**Blocks:** REQ-010, REQ-012

`GET /api/preguntas` is anonymous. That is deliberate today — guest mode is a product
feature and the same catalogue ships as a static JSON file in the web client — but it means
the full bank, correct answers included, can be scraped with a loop.

That is acceptable for 30 seeded questions. It stops being acceptable if the bank grows into
something with commercial value.

**Options:** leave it open · require an account · serve questions without the answer index
and mark server side only.

**Owner:** repository owner. **Answer by:** before T-017 grows the bank.

## OQ-002 — What is the intended relationship between `/api/progreso` and the attempt endpoints?

**Blocks:** REQ-050, REQ-044

Two paths record a result:

- `/api/attempts/*` — the server holds the answers and marks them. Not forgeable.
- `/api/progreso` — the client posts aggregate counts. The server recomputes the grade but
  has to trust the counts.

The web client uses the second because it also has to work offline. The consequence is that
a tampered client can still inflate how many answers it got right.

**Options:** accept it and document the offline path as trusted-client · have the offline
queue replay individual answers once connectivity returns · keep both and flag
client-reported attempts distinctly in statistics.

**Owner:** repository owner. **Answer by:** before statistics are presented as anything
other than a personal study aid.

## OQ-003 — How long should a session last?

**Blocks:** REQ-025 indirectly

The access token lasts one hour and the refresh token seven days. Those numbers were chosen
to bound the revocation-list window (ADR-0002), not from any usage evidence.

For a candidate who studies in two-hour blocks, a one-hour token means one silent refresh
mid-session, which works but is untested under a flaky connection.

**Options:** keep it · lengthen the access token and shorten the refresh chain · add sliding
expiry.

**Owner:** repository owner. **Answer by:** after the client implements automatic refresh.

## OQ-004 — Should difficulty be exposed to the candidate?

**Blocks:** REQ-010

`Questions.Difficulty` exists, is indexed, is filterable through the API and is returned in
every response — but every seeded question is difficulty 1, and the client never uses it.

Either the field earns its place through real content classification, or it is scaffolding
that should be marked as unused so nobody builds on it.

**Owner:** repository owner. **Answer by:** alongside T-017.

## Resolved

| Question | Resolution |
| :--- | :--- |
| Does the block selector mean an ordinal or an id? | Ordinal. [ADR-0004](30-decisions/ADR-0004-block-ordinal-not-id.md) |
| Where does the marking scale live? | `ScoringService` and `AttemptFinish`, pinned to the same constants. [ADR-0006](30-decisions/ADR-0006-server-authoritative-scoring.md) |
| Should the JWT reach JavaScript? | No. [ADR-0001](30-decisions/ADR-0001-jwt-in-httponly-cookie-with-csrf.md) |
| Is deleting the cookie enough to log out? | No. [ADR-0002](30-decisions/ADR-0002-session-revocation-list.md) |
