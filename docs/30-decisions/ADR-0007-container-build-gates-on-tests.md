# ADR-0007 — The container build runs the tests and resolves the port at run time

- **Status:** Approved
- **Date:** 2026-09-08

## Context

Two defects in the same Dockerfile, both found by trying to build it rather than by reading
it.

**The build was broken.** `dotnet restore src/Oposiciones.sln` runs against a layer that
copies only the four `src/` project files. Adding `tests/Oposiciones.UnitTests` to the
solution meant restore could no longer resolve the graph, and every build failed with
`MSB3202: The project file was not found`. The image had not been rebuilt since, so nobody
noticed — and the next deploy would have failed with a message that says nothing about the
actual cause.

**The port was resolved at the wrong time.** `ENV ASPNETCORE_URLS=http://+:$PORT` looks like
it reads the platform's injected port. It does not: Docker expands variables when the image
is built, where `PORT` is unset, baking in the malformed `http://+:`. Docker itself warns
about this — `UndefinedVar: Usage of undefined variable '$PORT'` — and the warning had been
scrolling past unread. The service worked only because Kestrel fell back to a default that
Render happened to detect.

## Decision

Copy every project file in the solution before restoring, including the test project.

Run `dotnet test` inside the build stage. The unit suite needs no database, so a broken
business rule stops the image from being produced rather than reaching production.

Resolve the port at start-up with a shell-form entrypoint and a default:

```dockerfile
ENV PORT=8080
ENTRYPOINT ["sh", "-c", "ASPNETCORE_URLS=http://+:${PORT} exec dotnet Oposiciones.Api.dll"]
```

`exec` keeps the application as PID 1, so it receives `SIGTERM` and shuts down cleanly
instead of being killed after the platform's grace period.

Add a `.dockerignore`. `COPY . ./` was pulling the host's `bin/` and `obj/` into the image,
which produces confusing failures when they were compiled against a different SDK.

Run as the non-root account the runtime image ships with.

## Consequences

- The image builds again, which is the difference between being able to deploy and not.
- A failing test blocks the deploy, so the marking scale cannot regress into production.
- The container honours whatever port the platform injects. Verified by running it with
  `PORT=10000` and confirming `Now listening on: http://[::]:10000`.
- The build is slower by the length of the test run — a few seconds, against the cost of
  discovering a broken rule in production.
- The published image no longer carries host build artifacts.
