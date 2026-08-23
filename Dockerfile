FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /app

# Copiar la solución y restaurar dependencias
COPY src/*.sln ./src/
COPY src/Oposiciones.Api/*.csproj src/Oposiciones.Api/
COPY src/Oposiciones.Domain/*.csproj src/Oposiciones.Domain/
COPY src/Oposiciones.Infrastructure/*.csproj src/Oposiciones.Infrastructure/
RUN dotnet restore src/Oposiciones.sln

# Copiar todo el código fuente y compilar
COPY . ./
WORKDIR /app/src/Oposiciones.Api
RUN dotnet publish -c Release -o /out

# Runtime de ASP.NET
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
WORKDIR /app
COPY --from=build /out ./

# Render utiliza la variable PORT para asignar el puerto
ENV ASPNETCORE_URLS=http://+:$PORT

ENTRYPOINT ["dotnet", "Oposiciones.Api.dll"]
