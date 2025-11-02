# Guía de Instalación de Dependencias - Custom OOBE

Este documento explica cómo instalar todas las dependencias necesarias para compilar el proyecto Custom OOBE.

## 🚀 Instalación Rápida

### Windows

**Opción 1: Usando el script BAT (Recomendado)**
```cmd
install-dependencies.bat
```

**Opción 2: Usando PowerShell**
```powershell
.\install-dependencies.ps1
```

### Linux / macOS

```bash
./install-dependencies.sh
```

## 📋 ¿Qué hace el script?

El script de instalación automáticamente:

1. ✅ **Detecta tu sistema operativo**
2. ✅ **Verifica si .NET SDK está instalado**
3. ✅ **Instala .NET 8.0 SDK si es necesario** (Windows con winget)
4. ✅ **Limpia compilaciones anteriores** (carpetas bin/ y obj/)
5. ✅ **Descarga todas las dependencias NuGet**:
   - System.Management (8.0.0)
   - Microsoft.Data.Sqlite (8.0.0)
   - ManagedNativeWifi (2.0.0)
   - AForge.Video (2.2.5)
   - AForge.Video.DirectShow (2.2.5)
6. ✅ **Crea la estructura de carpetas Assets**

## 🔧 Requisitos Previos

### Windows
- **Windows 10/11** (64-bit)
- **PowerShell 5.1+** o **Command Prompt**
- **Conexión a Internet** (para descargar dependencias)

### Linux
- **Distribuciones soportadas**:
  - Ubuntu 20.04+ / Debian 10+
  - Fedora 36+ / RHEL 8+ / CentOS 8+
  - Arch Linux
- **Permisos sudo** (para instalar .NET SDK)
- **Conexión a Internet**

### macOS
- **macOS 10.15+** (Catalina o superior)
- **Homebrew** (para instalar .NET SDK)
- **Conexión a Internet**

## 📦 Dependencias que se Instalarán

### .NET SDK 8.0
El framework principal para compilar aplicaciones C#/.NET

**Instalación manual si el script falla:**

**Windows:**
```cmd
winget install Microsoft.DotNet.SDK.8
```
O descargar desde: https://dotnet.microsoft.com/download/dotnet/8.0

**Ubuntu/Debian:**
```bash
wget https://packages.microsoft.com/config/ubuntu/$(lsb_release -rs)/packages-microsoft-prod.deb
sudo dpkg -i packages-microsoft-prod.deb
sudo apt-get update
sudo apt-get install -y dotnet-sdk-8.0
```

**Fedora:**
```bash
sudo dnf install dotnet-sdk-8.0
```

**macOS:**
```bash
brew install --cask dotnet-sdk
```

### Paquetes NuGet

Estos se descargan automáticamente con `dotnet restore`:

| Paquete | Versión | Propósito |
|---------|---------|-----------|
| System.Management | 8.0.0 | Gestión de sistema Windows (WMI) |
| Microsoft.Data.Sqlite | 8.0.0 | Base de datos SQLite para reseñas |
| ManagedNativeWifi | 2.0.0 | Gestión de conexiones WiFi |
| AForge.Video | 2.2.5 | Captura de video/cámara |
| AForge.Video.DirectShow | 2.2.5 | Interfaz DirectShow para cámaras |

## 🛠️ Instalación Manual (Sin Scripts)

Si prefieres instalar manualmente:

### Paso 1: Instalar .NET SDK
Ver sección "Dependencias que se Instalarán" arriba.

### Paso 2: Verificar instalación
```bash
dotnet --version
# Debe mostrar 8.0.x o superior
```

### Paso 3: Navegar al proyecto
```bash
cd CustomOOBE
```

### Paso 4: Limpiar compilaciones anteriores
```bash
# Windows
rmdir /s /q bin
rmdir /s /q obj

# Linux/macOS
rm -rf bin obj
```

### Paso 5: Restaurar dependencias
```bash
dotnet restore
```

### Paso 6: Crear carpetas Assets
```bash
# Windows
mkdir Assets\Avatars Assets\Wallpapers Assets\LockScreens

# Linux/macOS
mkdir -p Assets/{Avatars,Wallpapers,LockScreens}
```

## ✅ Verificar Instalación

Después de ejecutar el script, verifica que todo esté correcto:

```bash
# Verificar .NET SDK
dotnet --version

# Verificar que las dependencias se restauraron
dotnet list package

# Intentar compilar
dotnet build -c Release
```

## 🐛 Solución de Problemas

### Error: "dotnet: command not found"
**Solución:** .NET SDK no está instalado o no está en el PATH.
- Reinstala .NET SDK
- En Windows, reinicia la terminal después de instalar
- En Linux/macOS, ejecuta: `source ~/.bashrc` o `source ~/.zshrc`

### Error: "Unable to load the service index for source https://api.nuget.org/v3/index.json"
**Solución:** Problema de conexión a NuGet.org
```bash
# Limpiar caché de NuGet
dotnet nuget locals all --clear

# Reintentar con forzado
dotnet restore --force
```

### Error: "Access to the path is denied"
**Solución:** Permisos insuficientes
- **Windows:** Ejecuta el script como Administrador
- **Linux/macOS:** Usa `sudo` si es necesario

### Error: "The project file could not be loaded"
**Solución:** Asegúrate de estar en el directorio correcto
```bash
cd CustomOOBE
# Verifica que existe CustomOOBE.csproj
ls CustomOOBE.csproj
```

### Error con AForge.Video en Linux/macOS
**Nota:** AForge.Video es específico de Windows. En Linux/macOS solo se restaurarán las dependencias, pero la compilación completa requiere Windows debido a WPF.

### Error: "winget: command not found" (Windows)
**Solución:** Instala App Installer desde Microsoft Store o descarga .NET SDK manualmente.

## 🔄 Actualizar Dependencias

Para actualizar a las últimas versiones de los paquetes:

```bash
# Ver paquetes desactualizados
dotnet list package --outdated

# Actualizar un paquete específico
dotnet add package System.Management

# Actualizar todos (editar CustomOOBE.csproj manualmente)
```

## 📝 Próximos Pasos

Después de instalar las dependencias:

1. **Compilar el proyecto:**
   ```bash
   dotnet build -c Release
   ```

2. **O usar el script de compilación rápida:**
   ```bash
   # Windows
   Build.bat
   
   # Linux/macOS (solo restaura dependencias)
   ./install-dependencies.sh
   ```

3. **Publicar como ejecutable único:**
   ```bash
   dotnet publish -c Release -r win-x64 --self-contained true -p:PublishSingleFile=true
   ```

4. **Agregar recursos visuales:**
   - `Assets/Avatars/` - Imágenes PNG de 512x512 píxeles
   - `Assets/Wallpapers/` - Imágenes JPG/PNG de 1920x1080+
   - `Assets/LockScreens/` - Imágenes JPG/PNG de 1920x1080

## 📚 Documentación Adicional

- [BUILD.md](BUILD.md) - Guía completa de compilación
- [CONFIGURATION.md](CONFIGURATION.md) - Configuración del proyecto
- [README.md](README.md) - Información general del proyecto

## 🆘 Soporte

Si encuentras problemas:

1. Revisa la sección "Solución de Problemas" arriba
2. Verifica que tienes conexión a Internet
3. Asegúrate de tener permisos suficientes
4. Consulta los logs de error completos
5. Verifica que tu sistema cumple los requisitos mínimos

## 📄 Licencia

Este proyecto está diseñado para uso comercial. Consulta el archivo LICENSE para más detalles.

---

**Nota Importante:** Este proyecto está diseñado específicamente para Windows debido al uso de WPF (Windows Presentation Foundation) y APIs específicas de Windows. En Linux/macOS, solo se pueden restaurar las dependencias, pero la compilación y ejecución completa requiere Windows.
