# StayHub Manager - Documentación

Sistema de gestión hotelera desarrollado con .NET 9 siguiendo principios de arquitectura limpia y patrones modernos de desarrollo.

## 🚀 **¡Nuevo! Inicio Rápido**

**¿Quieres tener StayHub funcionando en menos de 5 minutos?**
👉 **[Guía de Inicio Rápido](./quick-start-guide.md)** 👈

Un solo comando ejecuta todo:
```powershell
.\start-with-init.ps1
```

## 📑 Índice de Documentación

### 📋 Información General
- ⚡ **[Guía de Inicio Rápido](./quick-start-guide.md)** - ¡Funcionando en 5 minutos!
- [🏨 **Descripción del Proyecto**](./project-overview.md) - Propósito, características y visión general
- [🏗️ **Arquitectura del Sistema**](./architecture.md) - Estructura, patrones y decisiones arquitecturales
- [⚙️ **Instalación y Configuración**](./installation-guide.md) - Guía completa de instalación

### 💻 Desarrollo
- [🔍 **API Reference**](./api-reference.md) - Documentación completa de la API REST

### 📖 Referencias Técnicas
- [🧱 **Reglas de Negocio**](./business-rules.md) - Especificaciones detalladas BR-01 a BR-06

---

## 🚀 Inicio Rápido

Para empezar rápidamente con StayHub Manager:

```bash
# 1. Clonar el repositorio
git clone https://github.com/anthonymerive/stayhub-manager.git
cd stayhub-manager

# 2. Ejecutar con Docker
docker-compose up -d

# 3. Acceder a la API
curl http://localhost:8080/api/v1/hoteles
```

La API estará disponible en `http://localhost:8080` con documentación Swagger en `/swagger`.

## 🎯 Características Principales

- ✅ **Arquitectura Hexagonal** - Separación clara de responsabilidades
- ✅ **API REST Completa** - Endpoints para hoteles, habitaciones y reservas  
- ✅ **Reglas de Negocio** - Implementación robusta de BR-01 a BR-06
- ✅ **Trazabilidad Unificada** - Logs correlacionados por transacción
- ✅ **Testing Completo** - Cobertura de pruebas unitarias e integración
- ✅ **Docker Support** - Contenedorización completa
- ✅ **.NET 9** - Tecnologías más recientes de Microsoft

---

**StayHub Manager** - Gestión Hotelera AVIATUR 🏨
