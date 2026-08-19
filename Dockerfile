# ------------------------------------------------------------------------------------------------
# Imagen de la Api del Sistema de Oposiciones.
#
# Construccion en dos etapas: el SDK compila y publica, y la imagen final solo lleva el runtime de
# ASP.NET. La carpeta content/ viaja dentro de la imagen, de modo que el contenedor arranca y sirve
# tests sin necesidad de base de datos.
# ------------------------------------------------------------------------------------------------
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# Los ficheros de proyecto se copian primero para que la restauracion de paquetes quede cacheada y
# no se repita cada vez que cambia una linea de codigo.
COPY Directory.Build.props Directory.Packages.props global.json ./
COPY src/Oposiciones.Domain/Oposiciones.Domain.csproj src/Oposiciones.Domain/
COPY src/Oposiciones.Application/Oposiciones.Application.csproj src/Oposiciones.Application/
COPY src/Oposiciones.Infrastructure/Oposiciones.Infrastructure.csproj src/Oposiciones.Infrastructure/
COPY src/Oposiciones.Api/Oposiciones.Api.csproj src/Oposiciones.Api/
RUN dotnet restore src/Oposiciones.Api/Oposiciones.Api.csproj

COPY src/ src/
COPY content/ content/
RUN dotnet publish src/Oposiciones.Api/Oposiciones.Api.csproj \
        --configuration Release \
        --no-restore \
        --output /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
WORKDIR /app

# curl es la unica dependencia adicional, y esta solo para que HEALTHCHECK pueda consultar la
# sonda de disponibilidad de la propia Api.
RUN apt-get update \
    && apt-get install --no-install-recommends --yes curl \
    && rm -rf /var/lib/apt/lists/* \
    && useradd --uid 64198 --create-home --shell /usr/sbin/nologin oposiciones

USER 64198

COPY --from=build /app/publish .

ENV ASPNETCORE_URLS=http://+:8080 \
    ASPNETCORE_ENVIRONMENT=Production \
    DOTNET_gcServer=1

EXPOSE 8080

HEALTHCHECK --interval=30s --timeout=5s --start-period=20s --retries=3 \
    CMD curl --fail --silent http://localhost:8080/health/ready || exit 1

ENTRYPOINT ["dotnet", "Oposiciones.Api.dll"]
