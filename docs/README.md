# Documentation

This project follows **Spec Driven Development (SDD)**: every change starts in a document,
not in the code. Code that contradicts an `Approved` document is a defect, not a decision.

## Reading order

Start here if you are new. Read `00-charter.md`, then `20-architecture.md`, then the
requirement you are about to touch in `10-requirements.md`.

| Lifecycle phase | Document | Contents |
| :--- | :--- | :--- |
| Planning | [`00-charter.md`](00-charter.md) | Goal, scope, non-goals, constraints, success criteria |
| Analysis | [`10-requirements.md`](10-requirements.md) | Requirements `REQ-###` in EARS notation |
| Analysis | [`11-open-questions.md`](11-open-questions.md) | Unresolved ambiguity. Blocks the requirement it references |
| Design | [`20-architecture.md`](20-architecture.md) | Layers, boundaries and the contract of each one |
| Design | [`21-data-model.md`](21-data-model.md) | Entities, invariants and persistence |
| Design | [`30-decisions/`](30-decisions/) | `ADR-####-*.md` with status `Proposed`, `Approved` or `Superseded` |
| Development | [`40-tasks.md`](40-tasks.md) | Tasks `T-###` with a "done when" criterion |
| Development | [`41-blockers.md`](41-blockers.md) | External dependencies, listed before writing code |
| Testing | [`50-traceability.md`](50-traceability.md) | Every requirement and the test that proves it |
| Deployment | [`60-runbook.md`](60-runbook.md) | Start-up, recovery, known limits |
| Maintenance | [`90-changelog.md`](90-changelog.md) | History of relevant changes |

## Feature workspaces

Work in progress lives in `specs/NNN-feature-name/` with three files:

- `spec.md` — requirements in EARS notation
- `plan.md` — design
- `tasks.md` — verifiable tasks

Once a feature is consolidated, its requirements move into `10-requirements.md` and the
folder stays as the historical record of how the decision was reached.

## Working rules

1. Every change starts in the specification. A request that is not in `10-requirements.md`
   gets a requirement added first.
2. One task at a time, with the test written before the implementation.
3. A requirement without a test that covers it is not done, whatever the code says.
4. Significant decisions are recorded as an ADR in `Proposed` status. Only the repository
   owner promotes an ADR to `Approved`.
5. Session handoff is mandatory: before running out of context, dump the state (done,
   verified, next, blocked) into the active `tasks.md`. The next session starts by reading
   the docs, never from memory.
6. Design documents carry their diagrams in Mermaid and are updated in the same commit as
   the code or migration they describe.

## Task labels

| Label | Meaning |
| :--- | :--- |
| `[A]` | Fully automatable. The agent completes it alone. |
| `[M]` | Mixed. The agent does its part but a human action is required to close it. |
| `[H]` | Human only: creating accounts, generating credentials, paying, accepting terms, business decisions. |
