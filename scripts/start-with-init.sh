#!/bin/bash

# ============================================
# Iniciar StayHub Manager con inicialización automática de BD
# Script para levantar todos los servicios desde cero (macOS/Linux)
# ============================================

# Configuración de colores para output
RED='\033[0;31m'
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
BLUE='\033[0;34m'
CYAN='\033[0;36m'
GRAY='\033[0;37m'
NC='\033[0m' # No Color

# Variables para opciones
WITH_TOOLS=false
SKIP_INIT=false
CLEAN=false
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
    echo "StayHub Manager - Script de Inicialización (macOS/Linux)"
    echo ""
    echo "Uso: $0 [OPCIONES]"
    echo ""
    echo "Opciones:"
    echo "  --with-tools     Incluir herramientas adicionales (Adminer, Seq, etc.)"
    echo "  --skip-init      Omitir inicialización de BD (si ya está creada)"
    echo "  --clean          Limpiar contenedores existentes antes de iniciar"
    echo "  --verbose        Mostrar información detallada"
    echo "  --help           Mostrar esta ayuda"
    echo ""
    echo "Ejemplos:"
    echo "  $0                        # Inicialización básica"
    echo "  $0 --with-tools          # Con herramientas adicionales"
    echo "  $0 --clean --verbose     # Limpiar y modo detallado"
    echo ""
}

# Procesar argumentos de línea de comandos
while [[ $# -gt 0 ]]; do
    case $1 in
        --with-tools)
            WITH_TOOLS=true
            shift
            ;;
        --skip-init)
            SKIP_INIT=true
            shift
            ;;
        --clean)
            CLEAN=true
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

print_header "StayHub Manager - Inicialización Completa ($(uname))"
echo "============================================="
echo ""

# Función para logging verbose
log_verbose() {
    if [[ "$VERBOSE" == "true" ]]; then
        echo -e "${GRAY}[VERBOSE] $1${NC}"
    fi
}

# Función para manejo de errores
handle_error() {
    print_error "Error durante la inicialización: $1"
    echo ""
    print_info "💡 Soluciones comunes:"
    echo -e "${GRAY}1. 🐳 Asegúrate de que Docker esté ejecutándose${NC}"
    echo -e "${GRAY}2. 🔌 Verifica que los puertos 1433 y 8080 no estén ocupados${NC}"
    echo -e "${GRAY}3. 📁 Ejecuta desde el directorio raíz del proyecto${NC}"
    echo -e "${GRAY}4. 🧹 Intenta limpiar: $0 --clean${NC}"
    echo -e "${GRAY}5. 📋 Revisa logs: docker-compose logs${NC}"
    echo ""
    print_info "🆘 Para soporte adicional:"
    echo -e "${GRAY}   • Revisa la documentación en: docs/README.md${NC}"
    echo -e "${GRAY}   • Guía de Docker: docs/docker-testing-guide.md${NC}"
    echo ""
    exit 1
}

# Paso 1: Verificar prerrequisitos
print_step "Paso 1: Verificando prerrequisitos del sistema..."

# Verificar Docker
if ! command -v docker &> /dev/null; then
    print_error "Docker no está instalado"
    print_info "Instala Docker desde: https://docs.docker.com/get-docker/"
    exit 1
fi

DOCKER_VERSION=$(docker --version 2>/dev/null)
if [ $? -ne 0 ]; then
    print_error "Docker no se está ejecutando"
    print_info "Inicia Docker Desktop o el daemon de Docker"
    if [[ "$OSTYPE" == "darwin"* ]]; then
        print_info "macOS: open -a Docker"
    elif [[ "$OSTYPE" == "linux-gnu"* ]]; then
        print_info "Linux: sudo systemctl start docker"
    fi
    exit 1
fi
log_verbose "Docker detectado: $DOCKER_VERSION"

# Verificar Docker Compose
if ! command -v docker-compose &> /dev/null; then
    print_error "Docker Compose no está disponible"
    exit 1
fi

COMPOSE_VERSION=$(docker-compose --version 2>/dev/null)
log_verbose "Docker Compose detectado: $COMPOSE_VERSION"

# Verificar que estemos en el directorio correcto
if [ ! -f "docker-compose.yml" ]; then
    print_error "No se encontró docker-compose.yml"
    print_info "Ejecuta este script desde el directorio raíz del proyecto StayHub Manager"
    print_info "Directorio actual: $(pwd)"
    exit 1
fi

# Verificar script de inicialización
if [ ! -f "scripts/init-db/01-init.sql" ]; then
    print_error "No se encontró el script de inicialización: scripts/init-db/01-init.sql"
    exit 1
fi

print_success "Prerrequisitos verificados correctamente"

# Paso 2: Configurar variables de entorno
print_step "Paso 2: Configurando variables de entorno..."

if [ ! -f ".env" ]; then
    print_info "Creando archivo .env..."
    if [ -f ".env.example" ]; then
        cp ".env.example" ".env"
        print_success "Archivo .env creado desde .env.example"
    else
        print_info "Creando archivo .env con configuración por defecto..."
        cat > .env << 'EOF'
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
EOF
        print_success "Archivo .env creado con configuración por defecto"
    fi
else
    print_success "Archivo .env ya existe"
fi

# Cargar variables de entorno
if [ -f ".env" ]; then
    export $(cat .env | grep -v '^#' | grep -v '^$' | xargs)
fi

DB_PASSWORD=${DB_PASSWORD:-YourStrong@Passw0rd}
API_PORT=${API_PORT:-8080}

# Paso 3: Limpiar contenedores si se solicita
if [[ "$CLEAN" == "true" ]]; then
    print_step "Paso 3: Limpiando contenedores existentes..."
    docker-compose down -v >/dev/null 2>&1
    docker system prune -f >/dev/null 2>&1
    print_success "Contenedores limpiados"
fi

# Paso 4: Iniciar servicios principales
print_step "Paso 4: Iniciando servicios Docker..."
print_info "Esto puede tardar varios minutos en la primera ejecución..."

if [[ "$WITH_TOOLS" == "true" ]]; then
    print_info "Iniciando con herramientas adicionales (Adminer, Seq, etc.)..."
    if [ -f "docker-compose.tools.yml" ]; then
        docker-compose -f docker-compose.yml -f docker-compose.tools.yml up -d
    else
        print_warning "docker-compose.tools.yml no encontrado, iniciando servicios básicos"
        docker-compose up -d
    fi
else
    print_info "Iniciando servicios básicos (API + Base de Datos)..."
    docker-compose up -d
fi

if [ $? -ne 0 ]; then
    handle_error "Error al iniciar los servicios Docker"
fi

print_success "Contenedores Docker iniciados correctamente"

# Paso 5: Esperar que SQL Server esté listo
print_step "Paso 5: Esperando que SQL Server esté completamente listo..."
print_info "SQL Server puede tardar hasta 2-3 minutos en estar completamente operativo..."

max_attempts=40
attempts=0

while [ $attempts -lt $max_attempts ]; do
    attempts=$((attempts + 1))
    sleep 5

    log_verbose "Intento $attempts/$max_attempts: Probando conexión a SQL Server..."

    if docker exec stayhub-db /opt/mssql-tools18/bin/sqlcmd -S localhost -U sa -P "$DB_PASSWORD" -C -Q "SELECT 1" >/dev/null 2>&1; then
        echo ""
        print_success "SQL Server está listo y respondiendo!"
        break
    fi

    if [ $attempts -eq $max_attempts ]; then
        echo ""
        print_error "SQL Server no respondió después de $max_attempts intentos"
        print_info "Verifica los logs: docker-compose logs db"
        print_info "Puede que necesite más tiempo en máquinas más lentas"
        exit 1
    fi

    # Mostrar progreso visual
    if [ $((attempts % 3)) -eq 0 ]; then
        echo -e "  ${GRAY}⏳ Esperando SQL Server... ($attempts/$max_attempts)${NC}"
    else
        echo -n "."
    fi
done

# Paso 6: Ejecutar script de inicialización de BD (si no se omite)
if [[ "$SKIP_INIT" != "true" ]]; then
    print_step "Paso 6: Ejecutando script de inicialización de base de datos..."
    print_info "Archivo: scripts/init-db/01-init.sql"

    if [ -f "scripts/init-db/01-init.sql" ]; then
        log_verbose "Contenido del script cargado correctamente"

        # Ejecutar el script usando pipeline
        if cat "scripts/init-db/01-init.sql" | docker exec -i stayhub-db /opt/mssql-tools18/bin/sqlcmd -S localhost -U sa -P "$DB_PASSWORD" -C; then
            print_success "Script de inicialización ejecutado exitosamente!"

            # Verificar que la base de datos se creó correctamente
            print_step "Verificando estructura de la base de datos..."
            verification=$(docker exec stayhub-db /opt/mssql-tools18/bin/sqlcmd -S localhost -U sa -P "$DB_PASSWORD" -C -Q "USE StayHubDb; SELECT 'Tabla: ' + TABLE_NAME as TablesCreated FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_TYPE = 'BASE TABLE' ORDER BY TABLE_NAME" 2>/dev/null)

            if [ $? -eq 0 ]; then
                print_success "Base de datos StayHubDb verificada:"
                echo -e "${GRAY}$verification${NC}"
            else
                print_warning "La base de datos se creó pero hubo problemas al verificar las tablas"
            fi
        else
            print_error "Error al ejecutar el script de inicialización"
            print_info "Revisa el contenido del archivo: scripts/init-db/01-init.sql"
            print_info "Puedes ejecutar el script manualmente después"
        fi
    else
        print_error "No se pudo leer el contenido del script de inicialización"
    fi
else
    print_info "Paso 6: Omitiendo inicialización de BD (--skip-init especificado)"
fi

# Paso 7: Verificar que la API esté respondiendo
print_step "Paso 7: Verificando que la API esté lista..."

api_max_attempts=20
api_attempts=0

while [ $api_attempts -lt $api_max_attempts ]; do
    api_attempts=$((api_attempts + 1))
    sleep 3

    if curl -s "http://localhost:$API_PORT/healthz/ready" >/dev/null 2>&1; then
        print_success "API está respondiendo correctamente!"
        break
    fi

    if [ $api_attempts -eq $api_max_attempts ]; then
        print_warning "La API tardó más de lo esperado en estar lista"
        print_info "Puede que esté iniciando aún. Verifica manualmente: http://localhost:$API_PORT"
        break
    fi

    log_verbose "Intento $api_attempts/$api_max_attempts: Esperando API..."
done

# Paso 8: Mostrar resumen final
echo ""
print_header "🎉 StayHub Manager iniciado exitosamente!"
echo "========================================="
echo ""

echo -e "${CYAN}📊 Estado de los servicios:${NC}"
docker-compose ps
echo ""

echo -e "${CYAN}🌐 Servicios disponibles:${NC}"
echo -e "   ${GREEN}🔗 API Principal:      http://localhost:$API_PORT${NC}"
echo -e "   ${GREEN}📋 Swagger UI:         http://localhost:$API_PORT/swagger${NC}"
echo -e "   ${GREEN}🗄️ SQL Server:         localhost:1433${NC}"
echo -e "       ${GRAY}Database:         StayHubDb${NC}"
echo -e "       ${GRAY}Username:         sa${NC}"
echo -e "       ${GRAY}Password:         $DB_PASSWORD${NC}"

if [[ "$WITH_TOOLS" == "true" ]]; then
    echo ""
    echo -e "${CYAN}🛠️ Herramientas adicionales:${NC}"
    echo -e "   ${GREEN}🌐 Adminer (DB Web):   http://localhost:8080${NC}"
    echo -e "   ${GREEN}📊 SQLPad (SQL Query): http://localhost:3010${NC}"
    echo -e "   ${GREEN}📋 Seq (Logs):         http://localhost:5341${NC}"
fi

echo ""
echo -e "${CYAN}🔧 Comandos útiles:${NC}"
echo -e "   ${GRAY}Ver logs:              docker-compose logs${NC}"
echo -e "   ${GRAY}Detener servicios:     docker-compose down${NC}"
echo -e "   ${GRAY}Reiniciar:             $0${NC}"
echo -e "   ${GRAY}Con herramientas:      $0 --with-tools${NC}"
echo -e "   ${GRAY}Limpiar y reiniciar:   $0 --clean${NC}"

echo ""
echo -e "${CYAN}📋 Para conectar desde herramientas externas:${NC}"
echo -e "   ${GRAY}• Azure Data Studio / SSMS:${NC}"
echo -e "     ${GRAY}Server: localhost,1433${NC}"
echo -e "     ${GRAY}Auth: SQL Server Authentication${NC}"
echo -e "     ${GRAY}User: sa | Password: $DB_PASSWORD${NC}"
echo ""
echo -e "   ${GRAY}• DBeaver:${NC}"
echo -e "     ${GRAY}Host: localhost | Port: 1433${NC}"
echo -e "     ${GRAY}Database: StayHubDb | User: sa${NC}"
echo -e "     ${GRAY}Driver Properties: trustServerCertificate=true${NC}"

echo ""
print_success "¡Todo listo para desarrollar con StayHub Manager!"
echo ""
