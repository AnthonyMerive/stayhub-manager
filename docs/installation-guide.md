# ⚙️ Guía de Instalación y Configuración

## 📋 Prerrequisitos

### 🔧 Software Requerido

| Herramienta | Versión Mínima | Recomendada | Propósito |
|-------------|----------------|-------------|-----------|
| **.NET SDK** | 9.0 | 9.0.x | Framework principal |
| **Docker** | 20.10 | Latest | Containerización |
| **Docker Compose** | 2.0 | Latest | Orquestación |
| **SQL Server** | 2019 | 2022 | Base de datos |
| **Git** | 2.30 | Latest | Control de versiones |

### 🖥️ Sistemas Operativos Soportados

- ✅ **Windows 10/11** (x64, ARM64)
- ✅ **macOS 12+** (Intel, Apple Silicon)  
- ✅ **Linux** (Ubuntu 20.04+, RHEL 8+, Alpine)

## 🚀 Instalación Rápida con Docker

### ⚡ **Método Recomendado: Script Automático**

**La forma más fácil de instalar StayHub Manager:**

```powershell
# 1. Clonar el repositorio
git clone https://github.com/company/stayhub-manager.git
cd stayhub-manager

# 2. Ejecutar script de inicialización automática
.\start-with-init.ps1
```

**¡Eso es todo!** 🎉 El script automáticamente:
- ✅ Verifica prerrequisitos
- ✅ Configura variables de entorno
- ✅ Inicia todos los servicios
- ✅ Ejecuta scripts de inicialización de BD
- ✅ Verifica que todo funcione

👉 **Ver guía detallada:** [Guía de Inicio Rápido](./quick-start-guide.md)

### 📦 Método Manual: Docker Compose

Si prefieres más control sobre el proceso:

```bash
# 1. Clonar el repositorio
git clone https://github.com/company/stayhub-manager.git
cd stayhub-manager

# 2. Configurar variables de entorno
cp .env.example .env
# Editar .env según necesidades

# 3. Levantar todos los servicios
docker-compose up -d

# 4. Verificar estado
docker-compose ps

# 5. Ver logs
docker-compose logs -f stayhub-api
```

### 🔧 Estructura Docker Compose

```yaml
# docker-compose.yml
version: '3.8'

services:
  stayhub-api:
    build: 
      context: .
      dockerfile: src/StayHub.Host/Dockerfile
    ports:
      - "8080:8080"
    environment:
      - ASPNETCORE_ENVIRONMENT=Development
      - ConnectionStrings__DefaultConnection=${DB_CONNECTION_STRING}
    depends_on:
      - stayhub-db
    networks:
      - stayhub-network

  stayhub-db:
    image: mcr.microsoft.com/mssql/server:2022-latest
    ports:
      - "1433:1433"
    environment:
      ACCEPT_EULA: "Y"
      MSSQL_SA_PASSWORD: ${DB_SA_PASSWORD}
    volumes:
      - stayhub-db-data:/var/opt/mssql
    networks:
      - stayhub-network

networks:
  stayhub-network:
    driver: bridge

volumes:
  stayhub-db-data:
```

### 📄 Archivo de Variables (.env)

```bash
# .env.example
# Database Configuration
DB_SERVER=stayhub-db
DB_DATABASE=StayHubDB
DB_USER=sa
DB_SA_PASSWORD=YourStrong!Passw0rd
DB_CONNECTION_STRING=Server=${DB_SERVER};Database=${DB_DATABASE};User Id=${DB_USER};Password=${DB_SA_PASSWORD};TrustServerCertificate=true;

# API Configuration
API_PORT=8080
ASPNETCORE_ENVIRONMENT=Development

# Logging
LOG_LEVEL=Information
LOG_OUTPUT_TEMPLATE=[{Timestamp:yyyy-MM-dd HH:mm:ss} {Level:u3}] {Message:lj} {Properties:j}{NewLine}{Exception}

# Security
JWT_SECRET_KEY=your-super-secret-jwt-key-here-must-be-at-least-32-characters
JWT_ISSUER=StayHub
JWT_AUDIENCE=StayHub.API
JWT_EXPIRATION_MINUTES=60
```

## 🛠️ Instalación Manual (Desarrollo)

### 1️⃣ Configurar Base de Datos

#### SQL Server Local
```bash
# Windows - SQL Server Express
# Descargar desde: https://www.microsoft.com/sql-server/sql-server-downloads

# macOS/Linux - Docker
docker run -e "ACCEPT_EULA=Y" -e "MSSQL_SA_PASSWORD=YourStrong!Passw0rd" \
   -p 1433:1433 --name stayhub-db \
   -d mcr.microsoft.com/mssql/server:2022-latest
```

#### Crear Base de Datos
```sql
-- Conectar a SQL Server y ejecutar:
CREATE DATABASE StayHubDB;
GO
```

### 2️⃣ Configurar Aplicación .NET

```bash
# 1. Clonar repositorio
git clone https://github.com/company/stayhub-manager.git
cd stayhub-manager

# 2. Restaurar paquetes NuGet
dotnet restore

# 3. Configurar connection string
# Editar src/StayHub.Host/appsettings.Development.json
```

#### appsettings.Development.json
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Database=StayHubDB;Integrated Security=true;TrustServerCertificate=true;"
  },
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning",
      "Microsoft.EntityFrameworkCore": "Information"
    }
  },
  "AllowedHosts": "*",
  "Serilog": {
    "Using": ["Serilog.Sinks.Console", "Serilog.Sinks.File"],
    "MinimumLevel": "Information",
    "WriteTo": [
      {
        "Name": "Console",
        "Args": {
          "outputTemplate": "[{Timestamp:yyyy-MM-dd HH:mm:ss} {Level:u3}] {Message:lj} {Properties:j}{NewLine}{Exception}"
        }
      },
      {
        "Name": "File",
        "Args": {
          "path": "logs/stayhub-.log",
          "rollingInterval": "Day",
          "retainedFileCountLimit": 7,
          "outputTemplate": "[{Timestamp:yyyy-MM-dd HH:mm:ss} {Level:u3}] {Message:lj} {Properties:j}{NewLine}{Exception}"
        }
      }
    ]
  }
}
```

### 3️⃣ Aplicar Migraciones

```bash
# Instalar herramienta EF Core (si no está instalada)
dotnet tool install --global dotnet-ef

# Ir al proyecto principal
cd src/StayHub.Host

# Aplicar migraciones
dotnet ef database update --context StayHubDbContext

# O crear nueva migración si es necesario
dotnet ef migrations add InitialMigration --context StayHubDbContext --output-dir ../StayHub.Infrastructure/Out/Database/EfCore/Migrations
```

### 4️⃣ Ejecutar Aplicación

```bash
# Modo desarrollo
dotnet run --project src/StayHub.Host

# O con hot reload
dotnet watch --project src/StayHub.Host

# La API estará disponible en:
# - HTTP: http://localhost:5000
# - HTTPS: https://localhost:5001
# - Swagger: http://localhost:5000/swagger
```

## 🔧 Configuración Avanzada

### 🐳 Docker Personalizado

#### Dockerfile Optimizado
```dockerfile
# src/StayHub.Host/Dockerfile
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copiar archivos de proyecto y restaurar dependencias
COPY ["src/StayHub.Host/StayHub.Host.csproj", "src/StayHub.Host/"]
COPY ["src/StayHub.Application/StayHub.Application.csproj", "src/StayHub.Application/"]
COPY ["src/StayHub.Domain/StayHub.Domain.csproj", "src/StayHub.Domain/"]
COPY ["src/StayHub.Infrastructure/StayHub.Infrastructure.csproj", "src/StayHub.Infrastructure/"]
COPY ["src/StayHub.Shared/StayHub.Shared.csproj", "src/StayHub.Shared/"]

RUN dotnet restore "src/StayHub.Host/StayHub.Host.csproj"

# Copiar código fuente y compilar
COPY . .
WORKDIR "/src/src/StayHub.Host"
RUN dotnet build "StayHub.Host.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "StayHub.Host.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .

# Crear usuario no-root para seguridad
RUN adduser --disabled-password --home /app --gecos '' appuser && chown -R appuser /app
USER appuser

ENTRYPOINT ["dotnet", "StayHub.Host.dll"]
```

### ⚙️ Configuración de Producción

#### appsettings.Production.json
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "${CONNECTION_STRING}"
  },
  "Logging": {
    "LogLevel": {
      "Default": "Warning",
      "StayHub": "Information"
    }
  },
  "AllowedHosts": "*",
  "Serilog": {
    "Using": ["Serilog.Sinks.Console"],
    "MinimumLevel": "Warning",
    "WriteTo": [
      {
        "Name": "Console",
        "Args": {
          "formatter": "Serilog.Formatting.Json.JsonFormatter"
        }
      }
    ]
  },
  "HealthChecks": {
    "Enabled": true,
    "DatabaseTimeoutSeconds": 30
  }
}
```

## 🗄️ Configuración de Base de Datos

### 📊 Schema Inicial

El schema se crea automáticamente via Entity Framework migrations:

```bash
# Ver migraciones pendientes
dotnet ef migrations list --project src/StayHub.Infrastructure --startup-project src/StayHub.Host

# Generar script SQL
dotnet ef migrations script --project src/StayHub.Infrastructure --startup-project src/StayHub.Host --output database-setup.sql
```

### 📁 Datos de Prueba (Seed Data)

```csharp
// src/StayHub.Infrastructure/Out/Database/EfCore/SeedData.cs
public static class SeedData
{
    public static async Task SeedAsync(StayHubDbContext context)
    {
        if (!context.Hoteles.Any())
        {
            var hoteles = new[]
            {
                new Hotel { Nombre = "Hotel Plaza", Ciudad = "Madrid", Activo = true },
                new Hotel { Nombre = "Hotel Costa", Ciudad = "Barcelona", Activo = true }
            };

            context.Hoteles.AddRange(hoteles);
            await context.SaveChangesAsync();
        }
    }
}
```

## 🧪 Verificación de Instalación

### ✅ Health Checks

```bash
# Verificar estado de la API
curl http://localhost:8080/health

# Respuesta esperada:
{
  "status": "Healthy",
  "results": {
    "database": "Healthy",
    "self": "Healthy"
  },
  "totalDuration": "00:00:00.123"
}
```

### 🔍 Pruebas de Endpoints

```bash
# 1. Obtener hoteles
curl http://localhost:8080/api/v1/hoteles

# 2. Crear hotel
curl -X POST http://localhost:8080/api/v1/hoteles \
  -H "Content-Type: application/json" \
  -d '{"nombre":"Hotel Test","direccion":"Calle 123","ciudad":"Madrid","telefono":"123-456-789","email":"test@hotel.com"}'

# 3. Swagger UI
# Abrir en navegador: http://localhost:8080/swagger
```

## 🔧 Troubleshooting

### ❓ Problemas Comunes

#### Error de Conexión a Base de Datos
```bash
# Verificar conectividad
telnet localhost 1433

# Verificar logs
docker-compose logs stayhub-db

# Reiniciar servicios
docker-compose restart
```

#### Error de Migraciones
```bash
# Limpiar y recrear
dotnet ef database drop --force
dotnet ef database update
```

#### Puerto ya en uso
```bash
# Cambiar puerto en docker-compose.yml
ports:
  - "8081:8080"  # Usar 8081 en lugar de 8080
```

### 📋 Comandos Útiles

```bash
# Ver logs en tiempo real
docker-compose logs -f stayhub-api

# Ejecutar comandos dentro del contenedor
docker-compose exec stayhub-api bash

# Reiniciar solo la API
docker-compose restart stayhub-api

# Limpiar todo y empezar de nuevo
docker-compose down -v
docker-compose up -d --build
```

## 🚀 Despliegue

### 🌐 Producción

Para despliegue en producción, ver:
- [📄 Guía de Deployment](./deployment.md)
- [🔐 Configuración de Seguridad](./security.md)
- [📊 Monitoreo](./monitoring.md)

---
