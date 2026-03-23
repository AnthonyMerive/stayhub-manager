# ============================================
# Iniciar StayHub Manager con inicialización automática de BD
# Script para levantar todos los servicios desde cero
# ============================================

param(
    [switch]$WithTools,     # Incluir herramientas adicionales (Adminer, Seq, etc.)
    [switch]$SkipInit,      # Omitir inicialización de BD (si ya está creada)
    [switch]$Clean,         # Limpiar contenedores existentes antes de iniciar
    [switch]$Verbose        # Mostrar información detallada
)

# Configuración
$ErrorActionPreference = "Stop"
$VerbosePreference = if ($Verbose) { "Continue" } else { "SilentlyContinue" }

# Funciones para colores
function Write-Header { param([string]$Message) Write-Host "🚀 $Message" -ForegroundColor Cyan }
function Write-Step { param([string]$Message) Write-Host "🔄 $Message" -ForegroundColor Yellow }
function Write-Success { param([string]$Message) Write-Host "✅ $Message" -ForegroundColor Green }
function Write-Error { param([string]$Message) Write-Host "❌ $Message" -ForegroundColor Red }
function Write-Info { param([string]$Message) Write-Host "💡 $Message" -ForegroundColor Blue }
function Write-Warning { param([string]$Message) Write-Host "⚠️ $Message" -ForegroundColor DarkYellow }

Write-Header "StayHub Manager - Inicialización Completa"
Write-Host "=============================================" -ForegroundColor Cyan
Write-Host ""

try {
    # Paso 1: Verificar prerrequisitos
    Write-Step "Paso 1: Verificando prerrequisitos del sistema..."

    # Verificar Docker
    $dockerVersion = docker --version 2>$null
    if ($LASTEXITCODE -ne 0) {
        Write-Error "Docker Desktop no está instalado o no se está ejecutando"
        Write-Info "Descarga Docker Desktop desde: https://www.docker.com/products/docker-desktop"
        Write-Info "Después de instalarlo, reinicia este script"
        exit 1
    }
    Write-Verbose "Docker detectado: $dockerVersion"

    # Verificar Docker Compose
    $composeVersion = docker-compose --version 2>$null
    if ($LASTEXITCODE -ne 0) {
        Write-Error "Docker Compose no está disponible"
        exit 1
    }
    Write-Verbose "Docker Compose detectado: $composeVersion"

    # Verificar que estemos en el directorio correcto
    if (-not (Test-Path "docker-compose.yml")) {
        Write-Error "No se encontró docker-compose.yml"
        Write-Info "Ejecuta este script desde el directorio raíz del proyecto StayHub Manager"
        Write-Info "Directorio actual: $(Get-Location)"
        exit 1
    }

    # Verificar script de inicialización
    if (-not (Test-Path "scripts\init-db\01-init.sql")) {
        Write-Error "No se encontró el script de inicialización: scripts\init-db\01-init.sql"
        exit 1
    }

    Write-Success "Prerrequisitos verificados correctamente"

    # Paso 2: Configurar variables de entorno
    Write-Step "Paso 2: Configurando variables de entorno..."

    if (-not (Test-Path ".env")) {
        Write-Info "Creando archivo .env desde .env.example..."
        if (Test-Path ".env.example") {
            Copy-Item ".env.example" ".env"
            Write-Success "Archivo .env creado desde .env.example"
        } else {
            Write-Info "Creando archivo .env con configuración por defecto..."
            @"
# ============================================
# Variables de Entorno - StayHub Manager
# ============================================

# API Configuration
API_PORT=8080
ASPNETCORE_ENVIRONMENT=Development

# Database Configuration
DB_PORT=1433
DB_PASSWORD=YourStrong@Passw0rd

# Herramientas adicionales
ADMINER_PORT=8080
SQLPAD_PORT=3010
SEQ_PORT=5341
REDIS_PORT=6379
"@ | Out-File -FilePath ".env" -Encoding UTF8
            Write-Success "Archivo .env creado con configuración por defecto"
        }
    } else {
        Write-Success "Archivo .env ya existe"
    }

    # Paso 3: Limpiar contenedores si se solicita
    if ($Clean) {
        Write-Step "Paso 3: Limpiando contenedores existentes..."
        docker-compose down -v 2>$null
        docker system prune -f 2>$null
        Write-Success "Contenedores limpiados"
    }

    # Paso 4: Iniciar servicios principales
    Write-Step "Paso 4: Iniciando servicios Docker..."
    Write-Info "Esto puede tardar varios minutos en la primera ejecución..."

    if ($WithTools) {
        Write-Info "Iniciando con herramientas adicionales (Adminer, Seq, etc.)..."
        docker-compose -f docker-compose.yml -f docker-compose.tools.yml up -d
    } else {
        Write-Info "Iniciando servicios básicos (API + Base de Datos)..."
        docker-compose up -d
    }

    if ($LASTEXITCODE -ne 0) {
        Write-Error "Error al iniciar los servicios Docker"
        Write-Info "Revisa los logs: docker-compose logs"
        exit 1
    }

    Write-Success "Contenedores Docker iniciados correctamente"

    # Paso 5: Esperar que SQL Server esté listo
    Write-Step "Paso 5: Esperando que SQL Server esté completamente listo..."
    Write-Info "SQL Server puede tardar hasta 2-3 minutos en estar completamente operativo..."

    $maxAttempts = 40
    $attempts = 0
    $dbPassword = $env:DB_PASSWORD ?? "YourStrong@Passw0rd"

    do {
        $attempts++
        Start-Sleep -Seconds 5

        Write-Verbose "Intento $attempts/$maxAttempts: Probando conexión a SQL Server..."
        $testConnection = docker exec stayhub-db /opt/mssql-tools18/bin/sqlcmd -S localhost -U sa -P "$dbPassword" -C -Q "SELECT 1" 2>$null

        if ($LASTEXITCODE -eq 0) {
            Write-Host ""
            Write-Success "SQL Server está listo y respondiendo!"
            break
        }

        if ($attempts -eq $maxAttempts) {
            Write-Host ""
            Write-Error "SQL Server no respondió después de $maxAttempts intentos"
            Write-Info "Verifica los logs: docker-compose logs db"
            Write-Info "Puede que necesite más tiempo en máquinas más lentas"
            exit 1
        }

        # Mostrar progreso visual
        if ($attempts % 3 -eq 0) {
            Write-Host "  ⏳ Esperando SQL Server... ($attempts/$maxAttempts)" -ForegroundColor Gray
        } else {
            Write-Host "." -NoNewline -ForegroundColor Yellow
        }

    } while ($true)

    # Paso 6: Ejecutar script de inicialización de BD (si no se omite)
    if (-not $SkipInit) {
        Write-Step "Paso 6: Ejecutando script de inicialización de base de datos..."
        Write-Info "Archivo: scripts\init-db\01-init.sql"

        # Verificar que el script existe y leerlo
        $sqlScript = Get-Content "scripts\init-db\01-init.sql" -Raw -Encoding UTF8

        if ($sqlScript) {
            Write-Verbose "Contenido del script cargado correctamente"

            # Ejecutar el script usando pipeline
            $sqlScript | docker exec -i stayhub-db /opt/mssql-tools18/bin/sqlcmd -S localhost -U sa -P "$dbPassword" -C

            if ($LASTEXITCODE -eq 0) {
                Write-Success "Script de inicialización ejecutado exitosamente!"

                # Verificar que la base de datos se creó correctamente
                Write-Step "Verificando estructura de la base de datos..."
                $verification = docker exec stayhub-db /opt/mssql-tools18/bin/sqlcmd -S localhost -U sa -P "$dbPassword" -C -Q "USE StayHubDb; SELECT 'Tabla: ' + TABLE_NAME as TablesCreated FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_TYPE = 'BASE TABLE' ORDER BY TABLE_NAME"

                if ($LASTEXITCODE -eq 0) {
                    Write-Success "Base de datos StayHubDb verificada:"
                    Write-Host $verification -ForegroundColor Gray
                } else {
                    Write-Warning "La base de datos se creó pero hubo problemas al verificar las tablas"
                }

            } else {
                Write-Error "Error al ejecutar el script de inicialización"
                Write-Info "Revisa el contenido del archivo: scripts\init-db\01-init.sql"
                Write-Info "Puedes ejecutar el script manualmente después"
            }
        } else {
            Write-Error "No se pudo leer el contenido del script de inicialización"
        }
    } else {
        Write-Info "Paso 6: Omitiendo inicialización de BD (--SkipInit especificado)"
    }

    # Paso 7: Verificar que la API esté respondiendo
    Write-Step "Paso 7: Verificando que la API esté lista..."

    $apiMaxAttempts = 20
    $apiAttempts = 0
    $apiPort = $env:API_PORT ?? "8080"

    do {
        $apiAttempts++
        Start-Sleep -Seconds 3

        try {
            $response = Invoke-WebRequest -Uri "http://localhost:$apiPort/healthz/ready" -TimeoutSec 10 -UseBasicParsing 2>$null
            if ($response.StatusCode -eq 200) {
                Write-Success "API está respondiendo correctamente!"
                break
            }
        } catch {
            # Continuar intentando
        }

        if ($apiAttempts -eq $apiMaxAttempts) {
            Write-Warning "La API tardó más de lo esperado en estar lista"
            Write-Info "Puede que esté iniciando aún. Verifica manualmente: http://localhost:$apiPort"
            break
        }

        Write-Verbose "Intento $apiAttempts/$apiMaxAttempts: Esperando API..."
    } while ($true)

    # Paso 8: Mostrar resumen final
    Write-Host ""
    Write-Header "🎉 StayHub Manager iniciado exitosamente!"
    Write-Host "=========================================" -ForegroundColor Green
    Write-Host ""

    Write-Host "📊 Estado de los servicios:" -ForegroundColor Cyan
    docker-compose ps
    Write-Host ""

    Write-Host "🌐 Servicios disponibles:" -ForegroundColor Cyan
    Write-Host "   🔗 API Principal:      http://localhost:$apiPort" -ForegroundColor White
    Write-Host "   📋 Swagger UI:         http://localhost:$apiPort/swagger" -ForegroundColor White
    Write-Host "   🗄️ SQL Server:         localhost:1433" -ForegroundColor White
    Write-Host "       Database:         StayHubDb" -ForegroundColor Gray
    Write-Host "       Username:         sa" -ForegroundColor Gray
    Write-Host "       Password:         $dbPassword" -ForegroundColor Gray

    if ($WithTools) {
        Write-Host ""
        Write-Host "🛠️ Herramientas adicionales:" -ForegroundColor Cyan
        Write-Host "   🌐 Adminer (DB Web):   http://localhost:8080" -ForegroundColor White
        Write-Host "   📊 SQLPad (SQL Query): http://localhost:3010" -ForegroundColor White
        Write-Host "   📋 Seq (Logs):         http://localhost:5341" -ForegroundColor White
    }

    Write-Host ""
    Write-Host "🔧 Comandos útiles:" -ForegroundColor Cyan
    Write-Host "   Ver logs:              docker-compose logs" -ForegroundColor Gray
    Write-Host "   Detener servicios:     docker-compose down" -ForegroundColor Gray
    Write-Host "   Reiniciar:             .\start-with-init.ps1" -ForegroundColor Gray
    Write-Host "   Con herramientas:      .\start-with-init.ps1 -WithTools" -ForegroundColor Gray
    Write-Host "   Limpiar y reiniciar:   .\start-with-init.ps1 -Clean" -ForegroundColor Gray

    Write-Host ""
    Write-Host "📋 Para conectar desde herramientas externas:" -ForegroundColor Cyan
    Write-Host "   • Azure Data Studio / SSMS:" -ForegroundColor Gray
    Write-Host "     Server: localhost,1433" -ForegroundColor Gray
    Write-Host "     Auth: SQL Server Authentication" -ForegroundColor Gray
    Write-Host "     User: sa | Password: $dbPassword" -ForegroundColor Gray
    Write-Host ""
    Write-Host "   • DBeaver:" -ForegroundColor Gray
    Write-Host "     Host: localhost | Port: 1433" -ForegroundColor Gray
    Write-Host "     Database: StayHubDb | User: sa" -ForegroundColor Gray
    Write-Host "     Driver Properties: trustServerCertificate=true" -ForegroundColor Gray

    Write-Host ""
    Write-Success "¡Todo listo para desarrollar con StayHub Manager!"

} catch {
    Write-Host ""
    Write-Error "Error durante la inicialización: $($_.Exception.Message)"
    Write-Host ""
    Write-Info "💡 Soluciones comunes:"
    Write-Host "1. 🐳 Asegúrate de que Docker Desktop esté ejecutándose" -ForegroundColor Gray
    Write-Host "2. 🔌 Verifica que los puertos 1433 y 8080 no estén ocupados" -ForegroundColor Gray  
    Write-Host "3. 📁 Ejecuta desde el directorio raíz del proyecto" -ForegroundColor Gray
    Write-Host "4. 🧹 Intenta limpiar: .\start-with-init.ps1 -Clean" -ForegroundColor Gray
    Write-Host "5. 📋 Revisa logs: docker-compose logs" -ForegroundColor Gray
    Write-Host "6. 🔄 En Windows, habilita scripts: Set-ExecutionPolicy RemoteSigned -Scope CurrentUser" -ForegroundColor Gray

    Write-Host ""
    Write-Info "🆘 Para soporte adicional:"
    Write-Host "   • Revisa la documentación en: docs/README.md" -ForegroundColor Gray
    Write-Host "   • Guía de Docker: docs/docker-testing-guide.md" -ForegroundColor Gray

    exit 1
}

Write-Host ""
