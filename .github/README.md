# 🔧 GitHub Actions Workflows

Este directorio contiene los workflows de CI/CD para el proyecto StayHub Manager.

## 📋 Workflows Disponibles

### 🔧 [ci.yml](.github/workflows/ci.yml) - Integración Continua
**Trigger:** Push a `main`/`develop` y Pull Requests a `main`

**Funcionalidades:**
- ✅ Build y test en múltiples OS (Ubuntu, Windows)
- 📊 Cobertura de código con reportes
- 🔍 Análisis de calidad de código
- 🔒 Escaneo de seguridad de paquetes NuGet
- 📋 Resumen automatizado de resultados

### 🚀 [production.yml](.github/workflows/production.yml) - Pipeline de Producción
**Trigger:** Push a `main` únicamente

**Funcionalidades:**
- 🎯 Testing comprehensivo con cobertura detallada
- 🔍 Validación completa de build de Release
- 📦 Verificación de publicación sin errores
- 📚 Validación de documentación
- 📊 Reporte HTML de cobertura de código
- 🎉 Verificación de estado "Production Ready"

### 🔄 [pr-validation.yml](.github/workflows/pr-validation.yml) - Validación de PR
**Trigger:** Pull Requests (abiertos, sincronizados, reabiertos)

**Funcionalidades:**
- ⚡ Validación rápida y eficiente
- 🎨 Verificación de formato de código
- 🔍 Análisis básico de calidad
- 📝 Manejo especial de Draft PRs
- 📋 Resumen automático de estado del PR
- 🚫 Cancelación automática de ejecuciones previas

## 🎯 Características Especiales

### 🚀 **Optimización de Performance**
- **Cache de NuGet**: Packages cacheados para builds más rápidos
- **Builds incrementales**: Solo rebuild cuando es necesario
- **Paralelización**: Jobs ejecutados en paralelo cuando es posible
- **Cancelación inteligente**: PRs cancelan ejecuciones previas automáticamente

### 📊 **Reportes Avanzados**
- **Summaries interactivos**: Resultados mostrados directamente en GitHub
- **Artifacts persistentes**: Reportes de cobertura guardados 30 días
- **Badges dinámicos**: Estado visual en tiempo real
- **Métricas detalladas**: Información completa de cada ejecución

### 🔒 **Seguridad Integrada**
- **Vulnerability scanning**: Paquetes NuGet escaneados automáticamente
- **Dependabot ready**: Compatible con actualizaciones automáticas
- **Secrets management**: Preparado para variables de entorno seguras

## 🛠️ Configuración Local

### Validación Pre-Push
Ejecuta las mismas validaciones localmente antes de hacer push:

```powershell
# Validación completa
.\scripts\validate-local.ps1

# Validación rápida (sin tests)
.\scripts\validate-local.ps1 -SkipTests

# Validación sin formato
.\scripts\validate-local.ps1 -SkipFormat

# Modo verbose
.\scripts\validate-local.ps1 -Verbose
```

### Formato Automático
```bash
# Verificar formato
dotnet format --verify-no-changes

# Aplicar formato automáticamente
dotnet format
```

## 📈 Métricas y Monitoreo

### Cobertura de Código
- **Target**: >80% cobertura general
- **Reportes**: HTML generados automáticamente
- **Trends**: Seguimiento histórico en artifacts

### Tiempo de Ejecución
- **CI Workflow**: ~3-5 minutos
- **Production Pipeline**: ~8-12 minutos  
- **PR Validation**: ~2-3 minutos

### Success Rate
- **Target**: >98% success rate
- **Alertas**: Fallas automáticas en Slack/Teams (configurable)

## 🔧 Personalización

### Variables de Entorno
```yaml
env:
  DOTNET_VERSION: '9.0.x'
  BUILD_CONFIGURATION: 'Release'
  TEST_CONFIGURATION: 'Debug'
```

### Configuración por Proyecto
Modifica estos archivos según necesidades específicas:
- `.editorconfig` - Formato y estilo de código
- `Directory.Build.props` - Configuraciones globales de MSBuild
- `global.json` - Versión específica de .NET SDK

## 🚨 Troubleshooting

### Problemas Comunes

**❌ Build falla con "Package not found"**
```bash
# Limpiar cache de NuGet
dotnet nuget locals all --clear
dotnet restore --force
```

**❌ Tests fallan localmente pero pasan en CI**
```bash
# Verificar configuración de test
dotnet test --configuration Release --verbosity diagnostic
```

**❌ Formato de código falla**
```bash
# Instalar herramientas globales
dotnet tool install -g dotnet-format
dotnet format
```

### Logs Detallados
Para debugging de workflows:
1. Ve a la pestaña "Actions" en GitHub
2. Selecciona la ejecución fallida  
3. Expande los pasos para ver logs detallados
4. Descarga artifacts si están disponibles

## 📞 Soporte

Para issues relacionados con los workflows:
1. 🐛 **Bugs**: Crea un issue con label `ci/cd`
2. 💡 **Mejoras**: Discute en GitHub Discussions
3. 🚨 **Urgente**: Contacta al equipo DevOps

---

**Última actualización**: Diciembre 2024  
**Compatibilidad**: .NET 9, GitHub Actions, Visual Studio 2026
