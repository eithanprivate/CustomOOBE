# 📦 Instalación de Dependencias - Custom OOBE

## 🎯 Resumen

Se han creado **7 scripts** para facilitar la instalación de todas las dependencias necesarias para compilar el proyecto Custom OOBE.

## 📁 Archivos Creados

### Scripts de Instalación

| Archivo | Tamaño | Plataforma | Descripción |
|---------|--------|-----------|-------------|
| `install-dependencies.bat` | 5.6 KB | Windows | **[RECOMENDADO]** Script BAT para Windows |
| `install-dependencies.ps1` | 7.6 KB | Windows | Script PowerShell con más opciones |
| `install-dependencies.sh` | 6.5 KB | Linux/macOS | Script Bash completo |
| `install-dotnet.sh` | 6.6 KB | Linux/macOS | Instala solo .NET SDK |
| `restore-dependencies.sh` | 3.2 KB | Linux/macOS | Restaura solo paquetes NuGet |

### Documentación

| Archivo | Tamaño | Descripción |
|---------|--------|-------------|
| `LEEME-PRIMERO.md` | 5.7 KB | Guía rápida de inicio |
| `INSTALACION-DEPENDENCIAS.md` | 6.6 KB | Documentación completa |

## 🚀 Uso Rápido

### Windows

**Método 1: Doble clic (Más fácil)**
```
1. Navega a la carpeta CustomOOBE/
2. Haz doble clic en: install-dependencies.bat
3. Sigue las instrucciones en pantalla
4. ¡Listo!
```

**Método 2: Desde CMD**
```cmd
cd CustomOOBE
install-dependencies.bat
```

**Método 3: Desde PowerShell**
```powershell
cd CustomOOBE
.\install-dependencies.ps1
```

### Linux / macOS

```bash
cd CustomOOBE

# Paso 1: Instalar .NET SDK
./install-dotnet.sh

# Paso 2: Recargar PATH
source ~/.bashrc

# Paso 3: Restaurar dependencias
./restore-dependencies.sh
```

## ✅ ¿Qué Hacen los Scripts?

### 1. Verificación del Sistema
- ✅ Detecta el sistema operativo
- ✅ Verifica si .NET SDK está instalado
- ✅ Comprueba la versión de .NET

### 2. Instalación de .NET SDK 8.0
- ✅ **Windows:** Usa `winget` o descarga manual
- ✅ **Ubuntu/Debian:** Usa `apt-get` o script de Microsoft
- ✅ **Fedora/RHEL:** Usa `dnf`
- ✅ **Arch Linux:** Usa `pacman`
- ✅ **macOS:** Usa `brew`
- ✅ **Otros:** Script universal de Microsoft

### 3. Limpieza de Compilaciones Anteriores
- ✅ Elimina carpetas `bin/` y `obj/`
- ✅ Limpia caché de NuGet si es necesario

### 4. Restauración de Dependencias NuGet
Descarga automáticamente:
- ✅ **System.Management** (8.0.0) - Gestión de sistema Windows
- ✅ **Microsoft.Data.Sqlite** (8.0.0) - Base de datos SQLite
- ✅ **ManagedNativeWifi** (2.0.0) - Gestión de WiFi
- ✅ **AForge.Video** (2.2.5) - Captura de video
- ✅ **AForge.Video.DirectShow** (2.2.5) - Interfaz de cámara

### 5. Creación de Estructura de Carpetas
```
Assets/
├── Avatars/      ← Imágenes de perfil (PNG 512x512)
├── Wallpapers/   ← Fondos de pantalla (JPG/PNG 1920x1080+)
└── LockScreens/  ← Pantallas de bloqueo (JPG/PNG 1920x1080)
```

## 🎬 Ejemplo de Ejecución (Windows)

```
========================================
  Custom OOBE - Instalación de Dependencias
========================================

[1/6] Verificando sistema operativo...
      ✓ Windows detectado

[2/6] Verificando .NET SDK...
      ✓ .NET SDK encontrado (versión 8.0.1)

[3/6] .NET SDK ya está instalado

[4/6] Navegando al directorio del proyecto...
      ✓ En directorio: C:\Proyectos\CustomOOBE

[5/6] Limpiando compilaciones anteriores...
      ✓ Carpeta bin eliminada
      ✓ Carpeta obj eliminada

[6/6] Restaurando paquetes NuGet...
      Esto descargará las siguientes dependencias:
      - System.Management (8.0.0)
      - Microsoft.Data.Sqlite (8.0.0)
      - ManagedNativeWifi (2.0.0)
      - AForge.Video (2.2.5)
      - AForge.Video.DirectShow (2.2.5)

  Determining projects to restore...
  Restored C:\Proyectos\CustomOOBE\CustomOOBE.csproj (in 2.5 sec).
      ✓ Dependencias restauradas exitosamente

[Bonus] Creando estructura de carpetas Assets...
      ✓ Carpetas Assets creadas

========================================
  Instalación Completada
========================================

Próximos pasos:

1. Compilar el proyecto:
   dotnet build -c Release

2. O usar el script de compilación rápida:
   Build.bat

3. O publicar como ejecutable único:
   dotnet publish -c Release -r win-x64 --self-contained true

4. Agregar imágenes a las carpetas Assets:
   - Assets\Avatars\ (PNG 512x512)
   - Assets\Wallpapers\ (JPG/PNG 1920x1080+)
   - Assets\LockScreens\ (JPG/PNG 1920x1080)

¿Deseas compilar el proyecto ahora? (S/N):
```

## 🔧 Características de los Scripts

### Detección Inteligente
- ✅ Detecta automáticamente el sistema operativo
- ✅ Identifica la distribución de Linux
- ✅ Verifica versiones de software instalado
- ✅ Adapta el método de instalación según el sistema

### Manejo de Errores
- ✅ Reintentos automáticos si falla la descarga
- ✅ Limpieza de caché si hay conflictos
- ✅ Mensajes de error claros y útiles
- ✅ Sugerencias de solución para problemas comunes

### Interactividad
- ✅ Pregunta antes de instalar software
- ✅ Opción de compilar inmediatamente después
- ✅ Confirmaciones para acciones importantes
- ✅ Progreso visual con indicadores

### Multiplataforma
- ✅ Windows (BAT, PowerShell)
- ✅ Linux (Bash)
- ✅ macOS (Bash)
- ✅ Scripts específicos para cada plataforma

## 📊 Comparación de Scripts

### Windows

| Script | Ventajas | Cuándo Usar |
|--------|----------|-------------|
| `.bat` | Más simple, doble clic | Usuario promedio |
| `.ps1` | Más potente, mejor manejo de errores | Usuario avanzado |

### Linux/macOS

| Script | Ventajas | Cuándo Usar |
|--------|----------|-------------|
| `install-dependencies.sh` | Todo en uno | Primera instalación |
| `install-dotnet.sh` | Solo .NET SDK | Ya tienes el proyecto |
| `restore-dependencies.sh` | Solo paquetes | .NET ya instalado |

## 🐛 Solución de Problemas

### Error: "dotnet: command not found"
```bash
# Recargar PATH
source ~/.bashrc

# O cerrar y abrir nueva terminal
```

### Error: "Unable to load the service index"
```bash
# Limpiar caché de NuGet
dotnet nuget locals all --clear
dotnet restore --force
```

### Error: "Access denied"
```bash
# Windows: Ejecutar como Administrador
# Linux/macOS: Usar sudo si es necesario
sudo ./install-dotnet.sh
```

### Script de PowerShell no se ejecuta
```powershell
# Habilitar ejecución de scripts
Set-ExecutionPolicy -ExecutionPolicy RemoteSigned -Scope CurrentUser
```

## 📚 Documentación Adicional

Para más información, consulta:

1. **LEEME-PRIMERO.md** - Guía rápida de inicio
2. **INSTALACION-DEPENDENCIAS.md** - Documentación completa
3. **BUILD.md** - Guía de compilación
4. **CONFIGURATION.md** - Configuración del proyecto

## 🎓 Comandos Útiles Post-Instalación

```bash
# Verificar instalación
dotnet --version

# Ver paquetes instalados
dotnet list package

# Compilar proyecto
dotnet build -c Release

# Publicar ejecutable
dotnet publish -c Release -r win-x64 --self-contained

# Limpiar proyecto
dotnet clean

# Ejecutar (solo Windows)
dotnet run
```

## ⚠️ Notas Importantes

### Sobre la Compilación
- ✅ **Windows:** Compilación completa funcional
- ⚠️ **Linux/macOS:** Solo restauración de dependencias
  - El proyecto usa WPF (Windows Presentation Foundation)
  - WPF solo funciona en Windows
  - Útil para desarrollo en equipo multiplataforma
  - La compilación final debe hacerse en Windows

### Sobre las Dependencias
- Todas las dependencias se descargan de **NuGet.org**
- Se requiere **conexión a Internet**
- El tamaño total de descarga es aproximadamente **50-100 MB**
- Las dependencias se almacenan en caché local

### Sobre .NET SDK
- Se requiere **.NET 8.0 o superior**
- El SDK incluye el runtime
- Tamaño de descarga: **~200 MB**
- Espacio en disco requerido: **~500 MB**

## 🎉 ¡Listo para Empezar!

Ahora que tienes todos los scripts, simplemente:

1. **Elige tu plataforma** (Windows, Linux, macOS)
2. **Ejecuta el script correspondiente**
3. **Sigue las instrucciones en pantalla**
4. **¡Comienza a compilar!**

---

## 📞 Soporte

Si encuentras problemas:

1. Revisa la sección "Solución de Problemas"
2. Consulta la documentación completa
3. Verifica los requisitos del sistema
4. Asegúrate de tener conexión a Internet

---

**Creado para facilitar el desarrollo de Custom OOBE** 🚀

¡Buena suerte con tu proyecto! 🎊
