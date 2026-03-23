# 🏨 StayHub Manager
[![.NET 9](https://img.shields.io/badge/.NET-9.0-purple.svg)](https://dotnet.microsoft.com/download/dotnet/9.0)
[![License](https://img.shields.io/badge/license-MIT-blue.svg)](LICENSE)

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

## 🚀 Inicio Rápido

### 🐳 **Con Docker (Recomendado)**
```bash
# Clonar el repositorio
git clone https://github.com/usuario/stayhub-manager.git
cd stayhub-manager

# Ejecutar con Docker Compose
docker-compose up -d

# La API estará disponible en: http://localhost:5000
# Swagger UI en: http://localhost:5000/swagger
```

### 🛠️ **Instalación Manual**
```bash
# Requisitos: .NET 9 SDK
dotnet --version  # Debe ser 9.0.x

# Restaurar dependencias
dotnet restore

# Ejecutar migraciones
dotnet ef database update --project src/StayHub.Infrastructure

# Ejecutar la aplicación
dotnet run --project src/StayHub.Host
```

## 📊 API Endpoints

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

## 🧪 Testing

### **Ejecutar Pruebas**
```bash
# Todas las pruebas
dotnet test

# Con cobertura de código
dotnet test --collect:"XPlat Code Coverage"

# Validación local (script incluido)
.\scripts\validate-local.ps1
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

## 🔄 CI/CD Pipeline

### **GitHub Actions Workflows**
- ✅ **CI Pipeline**: Build, test y quality checks automáticos
- 🚀 **Production Pipeline**: Validación completa para main branch  
- 🔄 **PR Validation**: Validación rápida para Pull Requests
- 📊 **Coverage Reports**: Reportes automáticos de cobertura

### **Validación Local**
```powershell
# Ejecutar validaciones antes del push
.\scripts\validate-local.ps1

# Opciones disponibles
.\scripts\validate-local.ps1 -SkipTests    # Sin pruebas
.\scripts\validate-local.ps1 -SkipFormat   # Sin formato
.\scripts\validate-local.ps1 -Verbose      # Modo detallado
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

- 🐛 **Issues**: [GitHub Issues](https://github.com/usuario/stayhub-manager/issues)
- 💬 **Discussions**: [GitHub Discussions](https://github.com/usuario/stayhub-manager/discussions)  
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

**Desarrollado con .NET 9 y las mejores prácticas de desarrollo empresarial.**

[![.NET](https://img.shields.io/badge/.NET-9.0-purple.svg?style=for-the-badge&logo=dotnet)](https://dotnet.microsoft.com/)
[![C#](https://img.shields.io/badge/C%23-13.0-blue.svg?style=for-the-badge&logo=csharp)](https://docs.microsoft.com/en-us/dotnet/csharp/)
[![Docker](https://img.shields.io/badge/Docker-Ready-blue.svg?style=for-the-badge&logo=docker)](https://www.docker.com/)
