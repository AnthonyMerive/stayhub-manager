# ⚡ Guía de Inicio Rápido - StayHub Manager

Esta guía te permitirá tener StayHub Manager funcionando en **menos de 5 minutos** en cualquier computador con Windows.

## 📋 **Requisitos Previos** (Solo una vez)

### 1. **Docker Desktop** 
- 📥 **Descargar**: https://www.docker.com/products/docker-desktop
- 🚀 **Instalar y ejecutar** Docker Desktop
- ✅ **Verificar**: Debe aparecer el icono de Docker en la bandeja del sistema

### 2. **Git** (para clonar el proyecto)
- 📥 **Descargar**: https://git-scm.com/download/win
- 🚀 **Instalar con opciones por defecto**

## 🚀 **Pasos de Instalación**

### **Paso 1: Descargar el proyecto**
```powershell
# Abrir PowerShell o Terminal de Windows
# Navegar a tu carpeta de proyectos (ejemplo: C:\proyectos)
cd C:\proyectos

# Clonar el repositorio
git clone https://github.com/usuario/stayhub-manager.git
cd stayhub-manager
```

### **Paso 2: Ejecutar el script de inicialización**
```powershell
# Un solo comando para levantar todo
.\start-with-init.ps1
```

**¡Eso es todo!** 🎉

El script automáticamente:
- ✅ Verifica que Docker esté funcionando
- ✅ Descarga las imágenes necesarias (puede tardar unos minutos la primera vez)
- ✅ Inicia SQL Server y la API
- ✅ Crea la base de datos con datos de ejemplo
- ✅ Verifica que todo funcione correctamente

## 📊 **¿Cómo saber que funciona?**

### **Indicadores de éxito:**
Al final del script deberías ver:
```
🎉 StayHub Manager iniciado exitosamente!
=========================================

🌐 Servicios disponibles:
   🔗 API Principal:      http://localhost:8080
   📋 Swagger UI:         http://localhost:8080/swagger
   🗄️ SQL Server:         localhost:1433
```

### **Pruebas rápidas:**
1. **Abrir navegador:** http://localhost:8080/swagger
2. **Probar endpoint:** Expandir "Hoteles" → GET `/api/hoteles` → "Try it out" → "Execute"
3. **Debería devolver** una lista con hoteles de ejemplo

## 🗄️ **Acceso a la Base de Datos**

### **Opción 1: Herramientas Web (Incluidas)**
```powershell
# Ejecutar con herramientas adicionales
.\start-with-init.ps1 -WithTools

# Luego ir a: http://localhost:8080 (Adminer)
# Sistema: SQL Server
# Servidor: db
# Usuario: sa
# Contraseña: YourStrong@Passw0rd
```

### **Opción 2: Herramientas de Escritorio**

#### **Azure Data Studio** (Recomendado - Gratuito)
- 📥 **Descargar**: https://docs.microsoft.com/en-us/sql/azure-data-studio/download
- 🔌 **Conectar con**:
  - Server: `localhost,1433`
  - Authentication: `SQL Login`
  - Username: `sa`
  - Password: `YourStrong@Passw0rd`

#### **DBeaver** (Alternativa multiplataforma)
- 📥 **Descargar**: https://dbeaver.io/download/
- 🔌 **Conectar con**:
  - Host: `localhost`
  - Port: `1433`
  - Database: `StayHubDb`
  - Username: `sa`
  - Password: `YourStrong@Passw0rd`
  - **Importante**: En "Driver Properties" agregar `trustServerCertificate=true`

## ❌ **Solución de Problemas Comunes**

### **"Docker Desktop no está ejecutándose"**
```powershell
# 1. Buscar "Docker Desktop" en el menú inicio y abrirlo
# 2. Esperar que aparezca el icono de ballena en la bandeja
# 3. Volver a ejecutar: .\start-with-init.ps1
```

### **"No se puede ejecutar scripts"** (Error de PowerShell)
```powershell
# Ejecutar una sola vez para habilitar scripts:
Set-ExecutionPolicy RemoteSigned -Scope CurrentUser -Force

# Luego ejecutar normalmente:
.\start-with-init.ps1
```

### **"Puerto 1433 ocupado"**
```powershell
# Ver qué está usando el puerto:
netstat -ano | findstr :1433

# Si hay otro SQL Server ejecutándose, detenerlo o:
# Editar .env y cambiar: DB_PORT=1434
# Luego: .\start-with-init.ps1 -Clean
```

### **"La API tarda mucho en responder"**
```powershell
# Algunas máquinas más lentas requieren más tiempo
# Esperar hasta 5 minutos la primera vez
# O ejecutar con información detallada:
.\start-with-init.ps1 -Verbose
```

### **Empezar desde cero**
```powershell
# Limpiar todo y empezar de nuevo:
.\start-with-init.ps1 -Clean -Verbose
```

## 🎯 **Comandos Útiles para el Día a Día**

```powershell
# Iniciar servicios (si ya están creados)
docker-compose up -d

# Ver qué está ejecutándose
docker-compose ps

# Ver logs de todos los servicios
docker-compose logs

# Ver solo logs de la API
docker-compose logs api

# Ver solo logs de la base de datos
docker-compose logs db

# Detener todos los servicios
docker-compose down

# Reiniciar todo desde cero
.\start-with-init.ps1 -Clean
```

## 🔧 **Opciones Avanzadas del Script**

```powershell
# Con herramientas adicionales (Adminer, logs, métricas)
.\start-with-init.ps1 -WithTools

# Omitir inicialización de BD (si ya existe)
.\start-with-init.ps1 -SkipInit

# Limpiar contenedores existentes antes de empezar
.\start-with-init.ps1 -Clean

# Modo detallado para debugging
.\start-with-init.ps1 -Verbose

# Combinación de opciones
.\start-with-init.ps1 -WithTools -Clean -Verbose
```

## 📚 **Siguientes Pasos**

Una vez que tengas StayHub funcionando:

1. **🔍 Explorar la API**: http://localhost:8080/swagger
2. **🗄️ Revisar la base de datos** con Azure Data Studio o DBeaver
3. **📖 Leer la documentación completa**: `docs/README.md`
4. **🧪 Ejecutar las pruebas**: `dotnet test`
5. **⚙️ Configurar tu IDE** (Visual Studio, VS Code, JetBrains Rider)

## 🆘 **Soporte**

Si tienes problemas:
1. **📋 Consultar**: `docs/docker-testing-guide.md`
2. **🐛 Issues**: https://github.com/usuario/stayhub-manager/issues
3. **💬 Discusiones**: https://github.com/usuario/stayhub-manager/discussions

---

**🎉 ¡Bienvenido a StayHub Manager!** Una vez que tengas todo funcionando, estarás listo para desarrollar, hacer pruebas o simplemente explorar el sistema.

**⏱️ Tiempo total estimado**: 3-5 minutos (+ tiempo de descarga de Docker images la primera vez)
