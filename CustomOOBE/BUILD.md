# Guía de Compilación - Custom OOBE

## Requisitos Previos

### Software Necesario
1. **Visual Studio 2022** o superior (con carga de trabajo de desarrollo de escritorio .NET)
   - O **Visual Studio Code** con extensión C#
2. **.NET 8.0 SDK** o superior
   - Descargar de: https://dotnet.microsoft.com/download

### Verificar Instalación
```bash
dotnet --version
# Debe mostrar 8.0.x o superior
```

## Pasos para Compilar

### 1. Clonar o Descargar el Proyecto
```bash
cd C:\Proyectos
git clone [tu-repositorio] CustomOOBE
cd CustomOOBE
```

### 2. Restaurar Dependencias
```bash
dotnet restore
```

Esto descargará todos los paquetes NuGet necesarios:
- System.Management
- Microsoft.Data.Sqlite
- ManagedNativeWifi
- AForge.Video
- AForge.Video.DirectShow

### 3. Crear Estructura de Carpetas Assets
```bash
mkdir Assets\Avatars
mkdir Assets\Wallpapers
mkdir Assets\LockScreens
```

O ejecutar el script:
```powershell
.\Scripts\CreateAssetsFolders.ps1
```

### 4. Compilar en Modo Debug
```bash
dotnet build
```

### 5. Compilar en Modo Release (Recomendado para Producción)
```bash
dotnet build -c Release
```

### 6. Publicar Aplicación Independiente

Para crear un ejecutable que incluya todas las dependencias:

```bash
dotnet publish -c Release -r win-x64 --self-contained true -p:PublishSingleFile=true -p:IncludeNativeLibrariesForSelfExtract=true
```

Parámetros explicados:
- `-c Release`: Compilación optimizada
- `-r win-x64`: Para Windows 64-bit
- `--self-contained true`: Incluye .NET runtime
- `-p:PublishSingleFile=true`: Todo en un solo ejecutable
- `-p:IncludeNativeLibrariesForSelfExtract=true`: Incluye librerías nativas

El ejecutable estará en: `bin\Release\net8.0-windows\win-x64\publish\`

## Compilación con Visual Studio

1. Abrir `CustomOOBE.sln` en Visual Studio
2. Seleccionar configuración **Release** en la barra superior
3. Hacer clic derecho en el proyecto → **Publicar**
4. Configurar perfil de publicación:
   - Destino: Carpeta
   - Configuración: Release
   - Framework de destino: net8.0-windows
   - Modo de implementación: Independiente
   - Runtime de destino: win-x64
   - Archivo único: Sí
5. Hacer clic en **Publicar**

## Configuración Post-Compilación

### 1. Agregar Recursos de Assets

#### Avatares (Assets/Avatars/)
- Agregar imágenes PNG de 512x512 píxeles
- Nombrar como: `avatar_1.png`, `avatar_2.png`, etc.
- Mínimo 6 imágenes recomendadas

#### Fondos de Pantalla (Assets/Wallpapers/)
- Agregar imágenes JPG/PNG de 1920x1080 o superior
- Nombrar como: `wallpaper_1.jpg`, `wallpaper_2.jpg`, etc.
- Mínimo 6 imágenes recomendadas

#### Pantallas de Bloqueo (Assets/LockScreens/)
- Agregar imágenes JPG/PNG de 1920x1080
- Nombrar como: `lockscreen_1.jpg`, `lockscreen_2.jpg`, etc.
- Mínimo 6 imágenes recomendadas

**Nota:** El programa creará imágenes placeholder automáticamente si no encuentra imágenes en estas carpetas.

### 2. Crear Icono de Aplicación

Crear o descargar un archivo `icon.ico` y colocarlo en la raíz del proyecto.

### 3. Firmar el Ejecutable (Opcional pero Recomendado)

Para evitar advertencias de seguridad:

```bash
signtool sign /f "tu-certificado.pfx" /p "contraseña" /t http://timestamp.digicert.com "CustomOOBE.exe"
```

## Empaquetado para Distribución

### Crear Paquete de Instalación

1. Copiar el contenido de `bin\Release\net8.0-windows\win-x64\publish\` a una carpeta `CustomOOBE`
2. Incluir la carpeta `Assets` con todas las imágenes
3. Incluir la carpeta `Scripts` con los scripts de instalación
4. Incluir `README.md`

Estructura final:
```
CustomOOBE/
├── CustomOOBE.exe
├── Assets/
│   ├── Avatars/
│   ├── Wallpapers/
│   └── LockScreens/
├── Scripts/
│   ├── SetupOOBE.ps1
│   ├── Uninstall-CustomOOBE.ps1
│   └── TestOOBE.bat
└── README.md
```

3. Comprimir en ZIP o crear instalador con herramientas como:
   - Inno Setup
   - WiX Toolset
   - NSIS

## Solución de Problemas de Compilación

### Error: SDK no encontrado
```bash
# Instalar .NET 8.0 SDK
winget install Microsoft.DotNet.SDK.8
```

### Error: Paquete NuGet no se puede restaurar
```bash
# Limpiar y restaurar
dotnet clean
dotnet restore --force
```

### Error: Conflicto de versiones
```bash
# Eliminar carpetas bin y obj
rmdir /s /q bin
rmdir /s /q obj
dotnet restore
dotnet build
```

### Error de permisos al compilar
- Ejecutar Visual Studio o terminal como administrador

### Error con AForge.Video
Si hay problemas con las librerías de cámara:
```bash
# Reinstalar paquete
dotnet remove package AForge.Video.DirectShow
dotnet add package AForge.Video.DirectShow --version 2.2.5
```

## Optimizaciones

### Reducir Tamaño del Ejecutable

1. **Habilitar IL Trimming** (en .csproj):
```xml
<PublishTrimmed>true</PublishTrimmed>
<TrimMode>link</TrimMode>
```

2. **Compilar sin símbolos de depuración**:
```bash
dotnet publish -c Release -p:DebugType=None -p:DebugSymbols=false
```

### Mejorar Tiempo de Inicio

1. **Ready To Run**:
```bash
dotnet publish -c Release -p:PublishReadyToRun=true
```

## Testing

### Pruebas Locales
```bash
# Ejecutar en modo debug
dotnet run

# O ejecutar el binario directamente
.\bin\Debug\net8.0-windows\CustomOOBE.exe
```

### Pruebas en Máquina Virtual
Recomendado probar en:
- Windows 10 (versión 21H2 o superior)
- Windows 11

Usar Hyper-V o VirtualBox para crear entorno de prueba limpio.

## Deployment

### Para Equipos Individuales
1. Copiar la carpeta completa a `C:\CustomOOBE`
2. Ejecutar como administrador: `Scripts\SetupOOBE.ps1`
3. Reiniciar el equipo

### Para Imagen de Sistema (Sysprep)
1. Instalar el Custom OOBE en la imagen maestra
2. Configurar para que se ejecute después de Sysprep
3. Crear imagen con herramientas como DISM

## Mantenimiento

### Actualizar Programas Disponibles
Editar `Pages/ProgramsPage.xaml.cs` y actualizar URLs de descarga regularmente.

### Actualizar Temas Visuales
Modificar archivos en `Themes/` para cambiar colores y estilos.

### Actualizar Base de Datos de Reseñas
El esquema está en `ReviewDatabase.cs`. Si se modifican campos, actualizar también la versión de la BD.

## Recursos Adicionales

- [Documentación .NET](https://docs.microsoft.com/dotnet)
- [WPF Guide](https://docs.microsoft.com/wpf)
- [Windows App Development](https://docs.microsoft.com/windows/apps)

## Licencia y Distribución

Este software está diseñado para uso comercial en equipos que vendes. Asegúrate de:
- Personalizar el nombre y marca
- Actualizar información de contacto
- Incluir tu licencia
- Firmar digitalmente el ejecutable
