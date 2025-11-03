# 🚀 LÉEME PRIMERO - Instalación Rápida de Dependencias

## ⚡ Inicio Rápido

### Windows (Recomendado)

**Opción más fácil - Doble clic:**
```
📁 Haz doble clic en: install-dependencies.bat
```

**O desde PowerShell:**
```powershell
.\install-dependencies.ps1
```

**O desde CMD:**
```cmd
install-dependencies.bat
```

### Linux / macOS

**Paso 1: Instalar .NET SDK**
```bash
./install-dotnet.sh
```

**Paso 2: Restaurar dependencias**
```bash
./restore-dependencies.sh
```

## 📋 Scripts Disponibles

| Script | Plataforma | Descripción |
|--------|-----------|-------------|
| `install-dependencies.bat` | Windows | **[RECOMENDADO]** Instala todo automáticamente |
| `install-dependencies.ps1` | Windows | Versión PowerShell con más opciones |
| `install-dotnet.sh` | Linux/macOS | Instala .NET 8.0 SDK |
| `restore-dependencies.sh` | Linux/macOS | Restaura paquetes NuGet |
| `Build.bat` | Windows | Compila el proyecto completo |

## ✅ ¿Qué se Instalará?

### 1. .NET 8.0 SDK
Framework necesario para compilar aplicaciones C#

### 2. Paquetes NuGet (automático)
- ✅ System.Management (8.0.0)
- ✅ Microsoft.Data.Sqlite (8.0.0)
- ✅ ManagedNativeWifi (2.0.0)
- ✅ AForge.Video (2.2.5)
- ✅ AForge.Video.DirectShow (2.2.5)

### 3. Estructura de Carpetas
```
Assets/
├── Avatars/      (para imágenes de perfil)
├── Wallpapers/   (para fondos de pantalla)
└── LockScreens/  (para pantallas de bloqueo)
```

## 🎯 Flujo Completo de Instalación

### Windows
```cmd
1. install-dependencies.bat    ← Instala todo
2. Build.bat                   ← Compila el proyecto
3. ¡Listo! 🎉
```

### Linux/macOS
```bash
1. ./install-dotnet.sh         ← Instala .NET SDK
2. source ~/.bashrc            ← Recarga el PATH
3. ./restore-dependencies.sh   ← Descarga dependencias
4. Nota: Compilar requiere Windows (WPF)
```

## 🔍 Verificar que Todo Funciona

Después de instalar, verifica:

```bash
# Verificar .NET SDK
dotnet --version
# Debe mostrar: 8.0.x

# Ver paquetes instalados
dotnet list package

# Intentar compilar (solo Windows)
dotnet build -c Release
```

## ❌ Problemas Comunes

### "dotnet: command not found"
**Solución:**
```bash
# Windows: Reinicia la terminal
# Linux/macOS: Ejecuta
source ~/.bashrc
# o cierra y abre nueva terminal
```

### "Unable to load the service index"
**Solución:** Problema de conexión a NuGet
```bash
dotnet nuget locals all --clear
dotnet restore --force
```

### "Access denied" / "Permission denied"
**Solución:**
- Windows: Ejecuta como Administrador
- Linux/macOS: Usa `sudo` si es necesario

### Script no se ejecuta en Windows
**Solución:** Habilita ejecución de scripts
```powershell
Set-ExecutionPolicy -ExecutionPolicy RemoteSigned -Scope CurrentUser
```

## 📦 Después de Instalar

### 1. Agregar Recursos Visuales (Opcional)

**Avatares** (`Assets/Avatars/`)
- Formato: PNG
- Tamaño: 512x512 píxeles
- Nombres: `avatar_1.png`, `avatar_2.png`, etc.

**Fondos de Pantalla** (`Assets/Wallpapers/`)
- Formato: JPG o PNG
- Tamaño: 1920x1080 o superior
- Nombres: `wallpaper_1.jpg`, `wallpaper_2.jpg`, etc.

**Pantallas de Bloqueo** (`Assets/LockScreens/`)
- Formato: JPG o PNG
- Tamaño: 1920x1080
- Nombres: `lockscreen_1.jpg`, `lockscreen_2.jpg`, etc.

### 2. Compilar el Proyecto

**Compilación simple:**
```bash
dotnet build -c Release
```

**Compilación con Build.bat (Windows):**
```cmd
Build.bat
```

**Publicar ejecutable único:**
```bash
dotnet publish -c Release -r win-x64 --self-contained true -p:PublishSingleFile=true
```

El ejecutable estará en:
```
bin/Release/net8.0-windows/win-x64/publish/CustomOOBE.exe
```

## 🆘 Necesitas Ayuda?

1. **Documentación completa:** Lee `INSTALACION-DEPENDENCIAS.md`
2. **Guía de compilación:** Lee `BUILD.md`
3. **Configuración:** Lee `CONFIGURATION.md`

## ⚠️ Nota Importante

Este proyecto usa **WPF (Windows Presentation Foundation)** y APIs específicas de Windows.

- ✅ **Windows:** Compilación y ejecución completa
- ⚠️ **Linux/macOS:** Solo restauración de dependencias
  - No se puede compilar ni ejecutar
  - Útil para desarrollo en equipo multiplataforma
  - La compilación final debe hacerse en Windows

## 🎓 Comandos Útiles

```bash
# Ver versión de .NET
dotnet --version

# Limpiar proyecto
dotnet clean

# Restaurar dependencias
dotnet restore

# Compilar en Debug
dotnet build

# Compilar en Release
dotnet build -c Release

# Ejecutar (solo Windows)
dotnet run

# Ver paquetes instalados
dotnet list package

# Ver paquetes desactualizados
dotnet list package --outdated

# Limpiar caché de NuGet
dotnet nuget locals all --clear
```

## 📚 Estructura del Proyecto

```
CustomOOBE/
├── 📄 install-dependencies.bat    ← [WINDOWS] Instalar todo
├── 📄 install-dependencies.ps1    ← [WINDOWS] PowerShell
├── 📄 install-dotnet.sh           ← [LINUX/MAC] Instalar .NET
├── 📄 restore-dependencies.sh     ← [LINUX/MAC] Restaurar paquetes
├── 📄 Build.bat                   ← [WINDOWS] Compilar
├── 📄 CustomOOBE.csproj          ← Configuración del proyecto
├── 📁 Assets/                     ← Recursos visuales
├── 📁 Pages/                      ← Páginas de la aplicación
├── 📁 Themes/                     ← Temas visuales
└── 📄 MainWindow.xaml            ← Ventana principal
```

## ✨ Siguiente Paso

Después de instalar las dependencias:

1. ✅ Ejecuta `Build.bat` (Windows)
2. ✅ O ejecuta `dotnet build -c Release`
3. ✅ Prueba el ejecutable generado
4. ✅ Agrega tus propias imágenes en `Assets/`
5. ✅ Personaliza según tus necesidades

---

**¿Todo listo?** 🚀

Ejecuta el script de instalación y en minutos tendrás todo configurado!

```cmd
install-dependencies.bat
```

¡Buena suerte! 🎉
