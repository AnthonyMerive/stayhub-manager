# ============================================
# Script de Validación Local - StayHub Manager
# Ejecuta las mismas validaciones que GitHub Actions
# ============================================

param(
    [switch]$SkipTests,
    [switch]$SkipFormat,
    [switch]$Verbose
)

# Configuración
$ErrorActionPreference = "Stop"
$VerbosePreference = if ($Verbose) { "Continue" } else { "SilentlyContinue" }

Write-Host "🚀 StayHub Manager - Validación Local" -ForegroundColor Cyan
Write-Host "=====================================" -ForegroundColor Cyan
Write-Host ""

# Función para mostrar paso
function Write-Step {
    param([string]$Message)
    Write-Host "🔄 $Message" -ForegroundColor Yellow
}

# Función para mostrar éxito
function Write-Success {
    param([string]$Message)
    Write-Host "✅ $Message" -ForegroundColor Green
}

# Función para mostrar error
function Write-Error {
    param([string]$Message)
    Write-Host "❌ $Message" -ForegroundColor Red
}

# Función para mostrar warning
function Write-Warning {
    param([string]$Message)
    Write-Host "⚠️ $Message" -ForegroundColor Yellow
}

try {
    # Verificar que estamos en el directorio correcto
    if (-not (Test-Path "StayHub.sln")) {
        Write-Error "No se encontró StayHub.sln. Ejecuta desde el directorio raíz del proyecto."
        exit 1
    }

    # Paso 1: Limpiar solución
    Write-Step "Limpiando solución..."
    dotnet clean --verbosity minimal
    Write-Success "Solución limpiada"

    # Paso 2: Restore de paquetes
    Write-Step "Restaurando paquetes NuGet..."
    dotnet restore --verbosity minimal
    Write-Success "Paquetes restaurados"

    # Paso 3: Verificar formato de código (si no se omite)
    if (-not $SkipFormat) {
        Write-Step "Verificando formato de código..."
        try {
            dotnet format --verify-no-changes --verbosity minimal
            Write-Success "Formato de código correcto"
        }
        catch {
            Write-Warning "El código no está formateado correctamente"
            Write-Host "💡 Ejecuta 'dotnet format' para corregir automáticamente" -ForegroundColor Blue

            $response = Read-Host "¿Quieres formatear automáticamente? (y/N)"
            if ($response -eq "y" -or $response -eq "Y") {
                dotnet format
                Write-Success "Código formateado automáticamente"
            }
        }
    }

    # Paso 4: Build de la solución
    Write-Step "Compilando solución..."
    $buildOutput = dotnet build --configuration Release --no-restore --verbosity minimal 2>&1

    # Verificar warnings
    $warnings = ($buildOutput | Select-String "warning").Count
    if ($warnings -gt 0) {
        Write-Warning "Se encontraron $warnings warning(s)"
        if ($warnings -gt 10) {
            Write-Error "Demasiados warnings ($warnings). Considera corregirlos antes del push."
        }
    }

    Write-Success "Solución compilada correctamente"

    # Paso 5: Ejecutar pruebas (si no se omite)
    if (-not $SkipTests) {
        Write-Step "Ejecutando pruebas unitarias..."

        $testResult = dotnet test tests/StayHub.UnitTests/StayHub.UnitTests.csproj `
            --no-build `
            --configuration Release `
            --verbosity minimal `
            --collect:"XPlat Code Coverage" `
            --results-directory ./temp-coverage `
            --logger "console;verbosity=normal"

        if ($LASTEXITCODE -eq 0) {
            Write-Success "Todas las pruebas pasaron"

            # Limpiar archivos temporales de cobertura
            if (Test-Path "./temp-coverage") {
                Remove-Item -Recurse -Force "./temp-coverage"
            }
        } else {
            Write-Error "Algunas pruebas fallaron"
            exit 1
        }
    }

    # Paso 6: Validación de archivos esenciales
    Write-Step "Validando archivos del proyecto..."

    $essentialFiles = @(
        "README.md",
        ".gitignore",
        "docs/README.md"
    )

    foreach ($file in $essentialFiles) {
        if (Test-Path $file) {
            Write-Verbose "✓ $file encontrado"
        } else {
            Write-Warning "Archivo recomendado no encontrado: $file"
        }
    }

    Write-Success "Validación de archivos completada"

    # Paso 7: Verificar estado de Git
    Write-Step "Verificando estado de Git..."

    $gitStatus = git status --porcelain
    if ($gitStatus) {
        Write-Host "📁 Archivos modificados:" -ForegroundColor Blue
        $gitStatus | ForEach-Object { Write-Host "   $_" -ForegroundColor Gray }
        Write-Host ""
    }

    # Verificar si hay commits sin push
    $unpushed = git log origin/main..HEAD --oneline 2>$null
    if ($unpushed) {
        Write-Host "📤 Commits pendientes de push:" -ForegroundColor Blue
        $unpushed | ForEach-Object { Write-Host "   $_" -ForegroundColor Gray }
        Write-Host ""
    }

    # Resumen final
    Write-Host ""
    Write-Host "🎉 ¡Validación completada exitosamente!" -ForegroundColor Green
    Write-Host "====================================" -ForegroundColor Green
    Write-Host ""
    Write-Host "✅ Solución compila correctamente" -ForegroundColor Green
    if (-not $SkipTests) {
        Write-Host "✅ Todas las pruebas pasan" -ForegroundColor Green
    }
    if (-not $SkipFormat) {
        Write-Host "✅ Formato de código verificado" -ForegroundColor Green
    }
    Write-Host "✅ Archivos del proyecto validados" -ForegroundColor Green
    Write-Host ""
    Write-Host "🚀 Tu código está listo para push a GitHub!" -ForegroundColor Cyan

    # Sugerir comandos Git
    if ($gitStatus) {
        Write-Host ""
        Write-Host "💡 Comandos sugeridos:" -ForegroundColor Blue
        Write-Host "   git add ." -ForegroundColor Gray
        Write-Host "   git commit -m ""feat: descripción de cambios""" -ForegroundColor Gray
        Write-Host "   git push origin main" -ForegroundColor Gray
    }

} catch {
    Write-Host ""
    Write-Error "Error durante la validación: $($_.Exception.Message)"
    Write-Host ""
    Write-Host "🔧 Sugerencias para solucionar:" -ForegroundColor Blue
    Write-Host "1. Verifica que .NET 9 esté instalado correctamente" -ForegroundColor Gray
    Write-Host "2. Asegúrate de estar en el directorio raíz del proyecto" -ForegroundColor Gray
    Write-Host "3. Ejecuta 'dotnet restore' manualmente" -ForegroundColor Gray
    Write-Host "4. Revisa los mensajes de error anteriores" -ForegroundColor Gray

    exit 1
}

Write-Host ""
