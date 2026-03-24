# 🏨 StayHub Manager
[![.NET](https://img.shields.io/badge/.NET-9.0-purple.svg?style=for-the-badge&logo=dotnet)](https://dotnet.microsoft.com/)
[![C#](https://img.shields.io/badge/C%23-13.0-blue.svg?style=for-the-badge&logo=csharp)](https://docs.microsoft.com/en-us/dotnet/csharp/)
[![Docker](https://img.shields.io/badge/Docker-Ready-blue.svg?style=for-the-badge&logo=docker)](https://www.docker.com/)


Sistema de gestión hotelera moderno desarrollado con **.NET 9** siguiendo principios de **arquitectura limpia** y patrones modernos de desarrollo empresarial.

## 🎯 Características Principales

### 🏨 **Gestión Integral de Hoteles**
- ✅ **Administración de Hoteles**: Registro, actualización y consulta de establecimientos
- ✅ **Gestión de Habitaciones**: Control completo de inventario y disponibilidad  
- ✅ **Sistema de Reservas**: Proceso de reserva con validaciones de negocio robustas
- ✅ **Trazabilidad Completa**: Seguimiento unificado con `transactionId` en logs y respuestas

### 🏗️ **Arquitectura Moderna**
- 🔹 **Hexagonal Architecture** (Ports & Adapters)
- 🔹 **Domain-Driven Design** con entidades ricas
- 🔹 **CQRS Pattern** para separación de comandos y consultas
- 🔹 **Dependency Injection** nativo de .NET 9
- 🔹 **Repository Pattern** con Entity Framework Core

### 🚀 **Stack Tecnológico**
```
🔧 Backend:     .NET 9 + ASP.NET Core
🗄️ Base de Datos: Entity Framework Core + SQL Server
🧪 Testing:     xUnit + FluentAssertions + Moq  
📋 Logging:     Serilog con structured logging
📄 API Docs:    Swagger/OpenAPI 3.0
🐳 Deploy:      Docker + Docker Compose
```

### 🖥️ **Compatibilidad Multiplataforma**
- ✅ **Windows 10/11** - PowerShell + Docker Desktop
- ✅ **macOS 12+** (Intel/Apple Silicon) - Terminal + Docker Desktop  
- ✅ **Linux** (Ubuntu 20.04+, RHEL 8+) - Bash + Docker Engine

## 🚀 Inicio Rápido

### ⚡ **Opción 1: Un solo comando (Recomendado)**

#### **Windows (PowerShell)**
```powershell
# Desde el directorio raíz del proyecto
.\scripts\start-with-init.ps1
```

#### **macOS / Linux (Bash)**
```bash
# Desde el directorio raíz del proyecto
chmod +x scripts/start-with-init.sh
./scripts/start-with-init.sh
```

Este script único:
- ✅ Verifica que Docker esté funcionando
- ✅ Crea archivos de configuración necesarios
- ✅ Inicia todos los servicios (API + Base de Datos)
- ✅ Ejecuta automáticamente el script de inicialización de BD
- ✅ Verifica que todo esté funcionando correctamente
- ✅ Te muestra las URLs para acceder a los servicios

### 🛠️ **Opciones avanzadas del script:**

#### **Windows (PowerShell)**
```powershell
# Con herramientas adicionales (Adminer, Seq, etc.)
.\scripts\start-with-init.ps1 -WithTools

# Limpiar contenedores existentes y empezar desde cero
.\scripts\start-with-init.ps1 -Clean

# Omitir inicialización de BD (si ya está creada)
.\scripts\start-with-init.ps1 -SkipInit

# Modo detallado para debugging
.\scripts\start-with-init.ps1 -Verbose

# Combinación de opciones
.\scripts\start-with-init.ps1 -WithTools -Clean -Verbose
```

#### **macOS / Linux (Bash)**
```bash
# Con herramientas adicionales (Adminer, Seq, etc.)
./scripts/start-with-init.sh --with-tools

# Limpiar contenedores existentes y empezar desde cero
./scripts/start-with-init.sh --clean

# Omitir inicialización de BD (si ya está creada)
./scripts/start-with-init.sh --skip-init

# Modo detallado para debugging
./scripts/start-with-init.sh --verbose

# Combinación de opciones
./scripts/start-with-init.sh --with-tools --clean --verbose
```

### 🐳 **Opción 2: Docker tradicional (Multiplataforma)**
```bash
# Iniciar servicios
docker-compose up -d

# Esperar que SQL Server esté listo (2-3 minutos)
# Luego ejecutar script de BD manualmente:

# En Windows (PowerShell):
Get-Content "scripts\init-db\01-init.sql" | docker exec -i stayhub-db /opt/mssql-tools18/bin/sqlcmd -S localhost -U sa -P "YourStrong@Passw0rd" -C

# En macOS/Linux (Bash):
cat scripts/init-db/01-init.sql | docker exec -i stayhub-db /opt/mssql-tools18/bin/sqlcmd -S localhost -U sa -P "YourStrong@Passw0rd" -C
```

### 🎯 **Requisitos previos**
- ✅ **Docker Desktop** instalado y ejecutándose
  - **Windows**: Docker Desktop for Windows
  - **macOS**: Docker Desktop for Mac  
  - **Linux**: Docker Engine + Docker Compose
- ✅ **Terminal/Shell**
  - **Windows**: PowerShell (incluido por defecto)
  - **macOS**: Terminal (incluido por defecto)
  - **Linux**: Bash (incluido por defecto)
- ✅ **Puertos libres**: 1433 (BD) y 8080 (API)

### 📥 **Instalación desde cero**

#### **Windows**
```powershell
# 1. Clonar el repositorio
git clone https://github.com/anthonymerive/stayhub-manager.git
cd stayhub-manager

# 2. Ejecutar script de inicialización
.\start-with-init.ps1

# ¡Eso es todo! 🎉
```

#### **macOS / Linux**
```bash
# 1. Clonar el repositorio
git clone https://github.com/anthonymerive/stayhub-manager.git
cd stayhub-manager

# 2. Dar permisos de ejecución y ejecutar
chmod +x scripts/start-with-init.sh
./scripts/start-with-init.sh

# ¡Eso es todo! 🎉
```

## 📊 API Endpoints

**Base URL:** `http://localhost:8080`

### 🏨 **Hoteles**
```http
GET    /api/hoteles              # Listar hoteles
GET    /api/hoteles/{id}         # Obtener hotel específico
POST   /api/hoteles              # Crear nuevo hotel
PUT    /api/hoteles/{id}         # Actualizar hotel
DELETE /api/hoteles/{id}         # Eliminar hotel
```

### 🛏️ **Habitaciones**
```http
GET    /api/habitaciones                    # Listar habitaciones
GET    /api/habitaciones/{id}               # Obtener habitación específica
GET    /api/habitaciones/hotel/{hotelId}    # Habitaciones por hotel
POST   /api/habitaciones                    # Crear habitación
PUT    /api/habitaciones/{id}               # Actualizar habitación
DELETE /api/habitaciones/{id}               # Eliminar habitación
```

### 📅 **Reservas**
```http
GET    /api/reservas                # Listar reservas
GET    /api/reservas/{id}           # Obtener reserva específica
POST   /api/reservas                # Crear nueva reserva
PUT    /api/reservas/{id}           # Actualizar reserva
DELETE /api/reservas/{id}           # Cancelar reserva
```

### 🔍 **Explorar API interactivamente**
- **Swagger UI:** http://localhost:8080/swagger
- **Health Check:** http://localhost:8080/healthz/ready

## 🧪 Testing

### **Ejecutar Pruebas**
```bash
# Todas las pruebas (multiplataforma)
dotnet test

# Con cobertura de código (multiplataforma)
dotnet test --collect:"XPlat Code Coverage"
```

### **Validación Local**

#### **Windows (PowerShell)**
```powershell
# Validación local completa
.\scripts\validate-local.ps1

# Opciones disponibles
.\scripts\validate-local.ps1 -SkipTests    # Sin pruebas
.\scripts\validate-local.ps1 -SkipFormat   # Sin formato
.\scripts\validate-local.ps1 -Verbose      # Modo detallado
```

#### **macOS / Linux (Bash)**
```bash
# Validación local completa
chmod +x scripts/validate-local.sh
./scripts/validate-local.sh

# Opciones disponibles
./scripts/validate-local.sh --skip-tests    # Sin pruebas
./scripts/validate-local.sh --skip-format   # Sin formato
./scripts/validate-local.sh --verbose       # Modo detallado
```

### **Cobertura de Código**
- 🎯 **Target**: >80% cobertura general
- ✅ **Unit Tests**: 54/54 pruebas pasando
- 📊 **Reportes**: Generados automáticamente en CI/CD

## 📚 Documentación Completa

Consulta la **[documentación técnica completa](./docs/README.md)** que incluye:

- 📋 **[Project Overview](./docs/project-overview.md)** - Descripción detallada y objetivos
- 🏗️ **[Architecture Guide](./docs/architecture.md)** - Diagramas y patrones arquitecturales
- ⚙️ **[Installation Guide](./docs/installation-guide.md)** - Guía completa de instalación
- 📜 **[Business Rules](./docs/business-rules.md)** - Especificaciones BR-01 a BR-06
- 🔌 **[API Reference](./docs/api-reference.md)** - Documentación completa de endpoints
- 🔍 **[Traceability System](./docs/Trazabilidad-Unificada.md)** - Sistema de trazabilidad unificado

## 🛠️ Troubleshooting

### ❌ **"Docker Desktop no está ejecutándose"**

#### **Windows**
```powershell
# Solución: Iniciar Docker Desktop
# 1. Buscar "Docker Desktop" en el menú de inicio
# 2. O ejecutar: Start-Process "C:\Program Files\Docker\Docker\Docker Desktop.exe"
# 3. Esperar que aparezca el icono de Docker en la bandeja del sistema
# 4. Volver a ejecutar: .\start-with-init.ps1
```

#### **macOS**
```bash
# Solución: Iniciar Docker Desktop
# 1. Buscar "Docker" en Applications o Launchpad
# 2. O ejecutar: open -a Docker
# 3. Esperar que aparezca el icono de Docker en la barra de menú
# 4. Volver a ejecutar: ./scripts/start-with-init.sh
```

#### **Linux**
```bash
# Solución: Iniciar Docker daemon
sudo systemctl start docker
sudo systemctl enable docker

# Verificar que está funcionando
docker --version
docker-compose --version

# Volver a ejecutar: ./scripts/start-with-init.sh
```

### ❌ **"Puerto 1433 ocupado"**

#### **Windows (PowerShell)**
```powershell
# Ver qué proceso está usando el puerto
netstat -ano | findstr :1433

# Cambiar puerto en .env
# DB_PORT=1434  # Usar otro puerto
# Reiniciar: .\start-with-init.ps1 -Clean
```

#### **macOS / Linux (Terminal)**
```bash
# Ver qué proceso está usando el puerto
lsof -i :1433
# o
netstat -tulpn | grep :1433

# Cambiar puerto en .env
# DB_PORT=1434  # Usar otro puerto  
# Reiniciar: ./scripts/start-with-init.sh --clean
```

### ❌ **"Script no se puede ejecutar"**

#### **Windows (PowerShell)**
```powershell
# Habilitar ejecución de scripts de PowerShell
Set-ExecutionPolicy RemoteSigned -Scope CurrentUser -Force

# Luego ejecutar normalmente
.\start-with-init.ps1
```

#### **macOS / Linux (Bash)**
```bash
# Dar permisos de ejecución al script
chmod +x scripts/start-with-init.sh

# Luego ejecutar normalmente
./scripts/start-with-init.sh
```

### ❌ **"Error de conexión a la base de datos"**
```bash
# Verificar que SQL Server esté listo (multiplataforma)
docker-compose logs db

# Reiniciar servicios (multiplataforma)
docker-compose down

# Windows:
.\start-with-init.ps1

# macOS/Linux:
./scripts/start-with-init.sh
```

### ❌ **"La API no responde"**
```bash
# Ver logs de la API (multiplataforma)
docker-compose logs api
```

#### **Windows**
```powershell
# Verificar que la BD esté inicializada
.\start-with-init.ps1 -SkipInit
```

#### **macOS / Linux**
```bash
# Verificar que la BD esté inicializada
./scripts/start-with-init.sh --skip-init
```

### 🔧 **Comandos de diagnóstico (Multiplataforma)**
```bash
# Estado de contenedores
docker-compose ps

# Logs detallados
docker-compose logs -f

# Verificar conectividad BD
docker exec stayhub-db /opt/mssql-tools18/bin/sqlcmd -S localhost -U sa -P "YourStrong@Passw0rd" -C -Q "SELECT @@VERSION"
```

#### **Limpiar y empezar de nuevo**

**Windows:**
```powershell
.\start-with-init.ps1 -Clean -Verbose
```

**macOS / Linux:**
```bash
./scripts/start-with-init.sh --clean --verbose
```

## 🏆 Reglas de Negocio

El sistema implementa **6 reglas de negocio principales**:

- **BR-01**: Validación de disponibilidad de habitaciones
- **BR-02**: Gestión de estados de reserva 
- **BR-03**: Validación de fechas y períodos
- **BR-04**: Control de capacidad por habitación
- **BR-05**: Validación de datos de huéspedes
- **BR-06**: Políticas de cancelación

> Consulta [Business Rules](./docs/business-rules.md) para especificaciones completas.

## 🤝 Contribuir

1. **Fork** el repositorio
2. Crear **feature branch**: `git checkout -b feature/nueva-funcionalidad`
3. **Commit** cambios: `git commit -m 'feat: nueva funcionalidad'`
4. **Push** a la branch: `git push origin feature/nueva-funcionalidad`
5. Crear **Pull Request**

### **Convenciones de Commit**
```
feat:     Nueva funcionalidad
fix:      Corrección de bug  
docs:     Cambios en documentación
style:    Cambios de formato (no afectan lógica)
refactor: Refactoring de código
test:     Agregar o modificar tests
chore:    Cambios en build o herramientas
```

## 📄 Licencia

Este proyecto está licenciado bajo la **MIT License** - consulta el archivo [LICENSE](LICENSE) para más detalles.

## 🆘 Soporte

- 🐛 **Issues**: [GitHub Issues](https://github.com/anthonymerive/stayhub-manager/issues)
- 💬 **Discussions**: [GitHub Discussions](https://github.com/anthonymerive/stayhub-manager/discussions)  
- 📧 **Email**: stayhub-support@company.com

## 📈 Roadmap

### **v1.1** (Q1 2026)
- [ ] Sistema de notificaciones en tiempo real
- [ ] API para integraciones externas
- [ ] Dashboard administrativo avanzado

### **v2.0** (Q2 2026)
- [ ] Microservicios con message brokers
- [ ] Implementación de CQRS completo
- [ ] Sistema de métricas y analytics

---
