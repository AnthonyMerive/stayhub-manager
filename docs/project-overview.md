# 🏨 StayHub Manager - Descripción del Proyecto

## 📖 Propósito

StayHub Manager es un sistema de gestión hotelera moderno diseñado para automatizar y optimizar las operaciones de hoteles y cadenas hoteleras. El sistema permite la administración completa de:

- **Hoteles** y sus propiedades
- **Habitaciones** y disponibilidad  
- **Reservas** y huéspedes
- **Reglas de negocio** específicas del sector hotelero

## 🎯 Objetivos del Proyecto

### Objetivos Principales

1. **Digitalización Completa**: Reemplazar procesos manuales con automatización inteligente
2. **Prevención de Errores**: Implementar validaciones robustas para evitar overbooking y conflictos
3. **Escalabilidad**: Diseñar para soportar desde hoteles boutique hasta cadenas internacionales  
4. **Experiencia de Usuario**: Proporcionar APIs intuitivas y bien documentadas
5. **Trazabilidad**: Garantizar seguimiento completo de todas las operaciones

### Objetivos Técnicos

- **Arquitectura Limpia**: Implementar patrones que faciliten mantenimiento y evolución
- **Performance**: Optimizar para alta concurrencia y respuesta rápida
- **Observabilidad**: Logs estructurados y métricas de negocio
- **Testabilidad**: Cobertura completa de pruebas automatizadas
- **DevOps**: CI/CD y containerización

## 🏗️ Características Principales

### ✨ Funcionalidades de Negocio

| Característica | Descripción | Beneficio |
|----------------|-------------|-----------|
| **Gestión de Hoteles** | CRUD completo de hoteles con validaciones | Centralización de propiedades |
| **Administración de Habitaciones** | Tipos, capacidades, tarifas y estados | Control granular de inventario |
| **Sistema de Reservas** | Creación, modificación y cancelación | Automatización del proceso de booking |
| **Prevención de Overbooking** | Validación automática de disponibilidad | Evita conflictos y problemas operativos |
| **Consulta de Disponibilidad** | Búsqueda inteligente de habitaciones | Optimiza ocupación hotelera |
| **Gestión de Estados** | Activación/desactivación de recursos | Control operativo flexible |

### 🛠️ Características Técnicas

| Aspecto | Implementación | Ventaja |
|---------|----------------|---------|
| **Arquitectura** | Hexagonal (Ports & Adapters) | Bajo acoplamiento, alta cohesión |
| **API Design** | REST con OpenAPI/Swagger | Estándar industry, fácil consumo |
| **Persistencia** | Entity Framework Core | ORM maduro, migraciones automáticas |
| **Validación** | FluentValidation + Annotations | Reglas expresivas y mantenibles |
| **Logging** | Serilog estructurado | Observabilidad y debugging |
| **Testing** | xUnit + FluentAssertions | Calidad y confiabilidad |
| **Containerización** | Docker + Docker Compose | Despliegue consistente |

## 🎨 Reglas de Negocio

StayHub implementa seis reglas de negocio fundamentales:

### BR-01: Integridad de Fechas
```csharp
// ❌ Incorrecto
FechaEntrada: 2024-01-15
FechaSalida:  2024-01-12

// ✅ Correcto  
FechaEntrada: 2024-01-15
FechaSalida:  2024-01-18
```

### BR-02: Prevención de Overbooking
- Validación automática de conflictos de fechas
- Búsqueda de overlapping en reservas existentes
- Protección contra condiciones de carrera

### BR-03: Validación de Capacidad
- Verificación de límites por habitación
- Control de huéspedes vs capacidad máxima
- Alertas preventivas

### BR-04: Filtro de Estado
- Solo habitaciones activas disponibles para reserva
- Estados: Activo, Inactivo, Mantenimiento
- Control operativo granular

### BR-05: Cálculo Automático
```csharp
Total = Noches × TarifaPorNoche
Noches = (FechaSalida - FechaEntrada).Days
```

### BR-06: Persistencia de Datos
- Cancelación lógica (soft delete)
- Auditoría completa de cambios
- Historial de operaciones

## 🔧 Stack Tecnológico

### Backend
- **.NET 9** - Framework principal
- **ASP.NET Core** - Web API
- **Entity Framework Core** - ORM
- **SQL Server** - Base de datos principal
- **FluentValidation** - Validaciones complejas
- **AutoMapper** - Mapping entre capas

### DevOps & Infraestructura  
- **Docker** - Containerización
- **Docker Compose** - Orquestación local
- **GitHub Actions** - CI/CD
- **Swagger/OpenAPI** - Documentación API

### Testing
- **xUnit** - Framework de pruebas
- **FluentAssertions** - Assertions expresivas
- **Moq** - Mocking
- **Microsoft.AspNetCore.Mvc.Testing** - Integration tests

### Monitoring & Observability
- **Serilog** - Logging estructurado
- **Application Insights** - Telemetría (opcional)
- **Health Checks** - Monitoreo de salud

## 🎯 Casos de Uso Principales

### 👤 Administrador de Hotel
1. **Gestionar propiedades**: Crear, actualizar hoteles
2. **Configurar habitaciones**: Definir tipos, capacidades, tarifas
3. **Monitorear ocupación**: Ver estadísticas y disponibilidad
4. **Gestionar estados**: Activar/desactivar recursos

### 🏢 Sistema de Reservas
1. **Consultar disponibilidad**: Buscar habitaciones por criterios
2. **Crear reservas**: Procesar booking con validaciones
3. **Modificar reservas**: Cambios según disponibilidad
4. **Cancelar reservas**: Cancelación lógica con auditoría

### 🔌 Integración con Terceros
1. **APIs externas**: Recibir reservas de OTAs
2. **Sistemas legados**: Sincronización de datos
3. **Servicios de pago**: Procesamiento de transacciones
4. **CRM**: Gestión de huéspedes

## 📈 Métricas y KPIs

### Métricas Técnicas
- **Response Time**: < 200ms promedio
- **Availability**: 99.9% uptime
- **Throughput**: 1000 req/min
- **Error Rate**: < 0.1%

### Métricas de Negocio
- **Ocupación Promedio**: %
- **Revenue per Room**: $/habitación
- **Booking Conversion**: %
- **Cancelation Rate**: %

## 🚀 Roadmap

### Fase 1: Core (Completada)
- ✅ Gestión básica de hoteles y habitaciones
- ✅ Sistema de reservas con reglas BR-01 a BR-06
- ✅ API REST completa
- ✅ Trazabilidad unificada

### Fase 2: Avanzada (En desarrollo)
- 🔄 Sistema de notificaciones
- 🔄 Reportes y dashboards
- 🔄 Integración con sistemas de pago
- 🔄 Multi-tenancy

### Fase 3: Enterprise (Planificada)
- ⏳ Machine Learning para pricing dinámico
- ⏳ Integración con IoT (smart rooms)
- ⏳ Mobile app para huéspedes
- ⏳ Analytics avanzados

---