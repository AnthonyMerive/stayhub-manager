#!/bin/bash

# ============================================
# Script de validación local - StayHub Manager
# Ejecuta las mismas validaciones que GitHub Actions (macOS/Linux)
# ============================================

# Configuración de colores
RED='\033[0;31m'
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
BLUE='\033[0;34m'
CYAN='\033[0;36m'
GRAY='\033[0;37m'
NC='\033[0m' # No Color

# Variables para opciones
SKIP_TESTS=false
SKIP_FORMAT=false
VERBOSE=false

# Funciones para colores
print_header() { echo -e "${CYAN}🚀 $1${NC}"; }
print_step() { echo -e "${YELLOW}🔄 $1${NC}"; }
print_success() { echo -e "${GREEN}✅ $1${NC}"; }
print_error() { echo -e "${RED}❌ $1${NC}"; }
print_info() { echo -e "${BLUE}💡 $1${NC}"; }
print_warning() { echo -e "${YELLOW}⚠️ $1${NC}"; }

# Función para mostrar ayuda
show_help() {
    echo "StayHub Manager - Script de Validación Local (macOS/Linux)"
    echo ""
    echo "Uso: $0 [OPCIONES]"
    echo ""
    echo "Opciones:"
    echo "  --skip-tests     Omitir ejecución de pruebas"
    echo "  --skip-format    Omitir verificación de formato"
    echo "  --verbose        Mostrar información detallada"
    echo "  --help           Mostrar esta ayuda"
    echo ""
    echo "Ejemplos:"
    echo "  $0                        # Validación completa"
    echo "  $0 --skip-tests          # Sin pruebas"
    echo "  $0 --skip-format         # Sin verificación de formato"
    echo "  $0 --verbose             # Modo detallado"
    echo ""
}

# Procesar argumentos
while [[ $# -gt 0 ]]; do
    case $1 in
        --skip-tests)
            SKIP_TESTS=true
            shift
            ;;
        --skip-format)
            SKIP_FORMAT=true
            shift
            ;;
        --verbose)
            VERBOSE=true
            shift
            ;;
        --help)
            show_help
            exit 0
            ;;
        *)
            echo "Opción desconocida: $1"
            show_help
            exit 1
            ;;
    esac
done

print_header "StayHub Manager - Validación Local ($(uname))"
echo "============================================="
echo ""

# Función para logging verbose
log_verbose() {
    if [[ "$VERBOSE" == "true" ]]; then
        echo -e "${GRAY}[VERBOSE] $1${NC}"
    fi
}

# Verificar que estamos en el directorio correcto
if [ ! -f "StayHub.sln" ]; then
    print_error "No se encontró StayHub.sln. Ejecuta desde el directorio raíz del proyecto."
    exit 1
fi

# Paso 1: Limpiar solución
print_step "Limpiando solución..."
if dotnet clean --verbosity minimal >/dev/null 2>&1; then
    print_success "Solución limpiada"
else
    print_warning "Advertencia al limpiar la solución"
fi

# Paso 2: Restore de paquetes
print_step "Restaurando paquetes NuGet..."
if dotnet restore --verbosity minimal; then
    print_success "Paquetes restaurados"
else
    print_error "Error al restaurar paquetes"
    exit 1
fi

# Paso 3: Verificar formato de código (si no se omite)
if [[ "$SKIP_FORMAT" != "true" ]]; then
    print_step "Verificando formato de código..."
    if dotnet format --verify-no-changes --verbosity minimal >/dev/null 2>&1; then
        print_success "Formato de código correcto"
    else
        print_warning "El código no está formateado correctamente"
        print_info "Ejecuta 'dotnet format' para corregir automáticamente"

        read -p "¿Quieres formatear automáticamente? (y/N): " -n 1 -r
        echo
        if [[ $REPLY =~ ^[Yy]$ ]]; then
            dotnet format
            print_success "Código formateado automáticamente"
        fi
    fi
fi

# Paso 4: Build de la solución
print_step "Compilando solución..."

# Capturar output del build para contar warnings
build_output=$(dotnet build --configuration Release --no-restore --verbosity minimal 2>&1)
build_exit_code=$?

if [ $build_exit_code -eq 0 ]; then
    # Verificar warnings
    warning_count=$(echo "$build_output" | grep -c "warning" || echo "0")

    if [ "$warning_count" -gt 0 ]; then
        print_warning "Se encontraron $warning_count warning(s)"
        if [ "$warning_count" -gt 10 ]; then
            print_error "Demasiados warnings ($warning_count). Considera corregirlos antes del push."
        fi
    fi

    print_success "Solución compilada correctamente"
else
    print_error "Error al compilar la solución"
    echo "$build_output"
    exit 1
fi

# Paso 5: Ejecutar pruebas (si no se omite)
if [[ "$SKIP_TESTS" != "true" ]]; then
    print_step "Ejecutando pruebas unitarias..."

    # Crear directorio temporal para cobertura
    temp_coverage_dir="./temp-coverage"
    mkdir -p "$temp_coverage_dir"

    if dotnet test tests/StayHub.UnitTests/StayHub.UnitTests.csproj \
        --no-build \
        --configuration Release \
        --verbosity minimal \
        --collect:"XPlat Code Coverage" \
        --results-directory "$temp_coverage_dir" \
        --logger "console;verbosity=normal"; then

        print_success "Todas las pruebas pasaron"

        # Limpiar archivos temporales de cobertura
        rm -rf "$temp_coverage_dir"
    else
        print_error "Algunas pruebas fallaron"
        rm -rf "$temp_coverage_dir"
        exit 1
    fi
fi

# Paso 6: Validación de archivos esenciales
print_step "Validando archivos del proyecto..."

essential_files=("README.md" ".gitignore" "docs/README.md")

for file in "${essential_files[@]}"; do
    if [ -f "$file" ]; then
        log_verbose "✓ $file encontrado"
    else
        print_warning "Archivo recomendado no encontrado: $file"
    fi
done

print_success "Validación de archivos completada"

# Paso 7: Verificar estado de Git
print_step "Verificando estado de Git..."

if command -v git &> /dev/null; then
    # Verificar si hay archivos modificados
    git_status=$(git status --porcelain 2>/dev/null)
    if [ -n "$git_status" ]; then
        echo -e "${BLUE}📁 Archivos modificados:${NC}"
        echo "$git_status" | while read line; do
            echo -e "   ${GRAY}$line${NC}"
        done
        echo ""
    fi

    # Verificar si hay commits sin push
    if git rev-parse --verify origin/main >/dev/null 2>&1; then
        unpushed=$(git log origin/main..HEAD --oneline 2>/dev/null)
        if [ -n "$unpushed" ]; then
            echo -e "${BLUE}📤 Commits pendientes de push:${NC}"
            echo "$unpushed" | while read line; do
                echo -e "   ${GRAY}$line${NC}"
            done
            echo ""
        fi
    fi
else
    print_info "Git no está disponible para verificar el estado"
fi

# Resumen final
echo ""
print_success "¡Validación completada exitosamente!"
echo "===================================="
echo ""
print_success "Solución compila correctamente"
if [[ "$SKIP_TESTS" != "true" ]]; then
    print_success "Todas las pruebas pasan"
fi
if [[ "$SKIP_FORMAT" != "true" ]]; then
    print_success "Formato de código verificado"
fi
print_success "Archivos del proyecto validados"
echo ""
print_info "🚀 Tu código está listo para push a GitHub!"

# Sugerir comandos Git si hay cambios
if [ -n "$git_status" ]; then
    echo ""
    print_info "💡 Comandos sugeridos:"
    echo -e "   ${GRAY}git add .${NC}"
    echo -e "   ${GRAY}git commit -m \"feat: descripción de cambios\"${NC}"
    echo -e "   ${GRAY}git push origin main${NC}"
fi

echo ""
