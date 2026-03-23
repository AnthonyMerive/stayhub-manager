# ============================================
# Dockerfile para StayHub Manager API
# ============================================

# Build stage
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copiar archivos de proyecto y restaurar dependencias
COPY ["src/StayHub.Host/StayHub.Host.csproj", "StayHub.Host/"]
COPY ["src/StayHub.Application/StayHub.Application.csproj", "StayHub.Application/"]
COPY ["src/StayHub.Domain/StayHub.Domain.csproj", "StayHub.Domain/"]
COPY ["src/StayHub.Infrastructure/StayHub.Infrastructure.csproj", "StayHub.Infrastructure/"]
COPY ["src/StayHub.Shared/StayHub.Shared.csproj", "StayHub.Shared/"]

RUN dotnet restore "StayHub.Host/StayHub.Host.csproj"

# Copiar código fuente y compilar
COPY ["src/", "./"]
RUN dotnet build "StayHub.Host/StayHub.Host.csproj" -c Release -o /app/build

# Publish stage
FROM build AS publish
RUN dotnet publish "StayHub.Host/StayHub.Host.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS runtime
WORKDIR /app

# Crear usuario no-root para seguridad
RUN adduser --disabled-password --gecos "" appuser && chown -R appuser /app
USER appuser

COPY --from=publish /app/publish .

# Variables de entorno
ENV ASPNETCORE_URLS=http://+:8080
ENV ASPNETCORE_ENVIRONMENT=Production
ENV DOTNET_RUNNING_IN_CONTAINER=true

EXPOSE 8080

HEALTHCHECK --interval=30s --timeout=3s --start-period=5s --retries=3 \
    CMD curl -f http://localhost:8080/healthz/live || exit 1

ENTRYPOINT ["dotnet", "StayHub.Host.dll"]
