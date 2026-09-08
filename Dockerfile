# Build stage -----------------------------------------------------------------
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /app

# Project files first, so the restore layer is cached until a dependency changes.
# Every project in the solution has to be here: `dotnet restore` reads the whole
# solution graph and fails with MSB3202 on the first .csproj it cannot find.
COPY src/*.sln ./src/
COPY src/Oposiciones.Api/*.csproj src/Oposiciones.Api/
COPY src/Oposiciones.Domain/*.csproj src/Oposiciones.Domain/
COPY src/Oposiciones.Application/*.csproj src/Oposiciones.Application/
COPY src/Oposiciones.Infrastructure/*.csproj src/Oposiciones.Infrastructure/
COPY tests/Oposiciones.UnitTests/*.csproj tests/Oposiciones.UnitTests/
RUN dotnet restore src/Oposiciones.sln

COPY . ./

# The unit suite runs as part of the image build, so a failing business rule stops
# the deploy instead of reaching production. It needs no database.
RUN dotnet test tests/Oposiciones.UnitTests/Oposiciones.UnitTests.csproj \
    --no-restore \
    --configuration Release \
    --verbosity quiet

RUN dotnet publish src/Oposiciones.Api/Oposiciones.Api.csproj \
    --no-restore \
    --configuration Release \
    --output /out

# Runtime stage ---------------------------------------------------------------
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
WORKDIR /app
COPY --from=build /out ./

# The image ships with a non-root account; using it limits what a compromised
# process can reach.
USER $APP_UID

# Render assigns the port through PORT at run time. Baking it into an ENV does not
# work: Docker resolves the variable when the image is built, where PORT is unset,
# producing the malformed URL "http://+:". The shell form expands it on start-up,
# and `exec` keeps the application as PID 1 so it receives SIGTERM and shuts down
# cleanly rather than being killed.
ENV PORT=8080
EXPOSE 8080

ENTRYPOINT ["sh", "-c", "ASPNETCORE_URLS=http://+:${PORT} exec dotnet Oposiciones.Api.dll"]
