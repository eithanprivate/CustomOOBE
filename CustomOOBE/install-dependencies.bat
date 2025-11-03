@echo off
REM Script de instalación de dependencias para Custom OOBE
REM Versión Windows Batch

setlocal enabledelayedexpansion

echo ========================================
echo   Custom OOBE - Instalacion de Dependencias
echo ========================================
echo.

REM [1/6] Verificar sistema operativo
echo [1/6] Verificando sistema operativo...
echo       Windows detectado
echo.

REM [2/6] Verificar .NET SDK
echo [2/6] Verificando .NET SDK...
where dotnet >nul 2>nul
if %ERRORLEVEL% NEQ 0 (
    echo ERROR: .NET SDK no encontrado
    set INSTALL_DOTNET=1
) else (
    for /f "tokens=*" %%i in ('dotnet --version') do set DOTNET_VERSION=%%i
    echo       ✓ .NET SDK encontrado ^(version !DOTNET_VERSION!^)
    
    REM Extraer versión mayor
    for /f "tokens=1 delims=." %%a in ("!DOTNET_VERSION!") do set MAJOR_VERSION=%%a
    
    if !MAJOR_VERSION! LSS 8 (
        echo       ⚠ Se requiere .NET 8.0 o superior. Version actual: !DOTNET_VERSION!
        set INSTALL_DOTNET=1
    ) else (
        set INSTALL_DOTNET=0
    )
)
echo.

REM [3/6] Instalar .NET SDK si es necesario
if !INSTALL_DOTNET! EQU 1 (
    echo [3/6] Instalando .NET 8.0 SDK...
    
    REM Verificar si winget está disponible
    where winget >nul 2>nul
    if %ERRORLEVEL% EQU 0 (
        echo       Usando winget para instalar .NET SDK...
        winget install Microsoft.DotNet.SDK.8 --silent --accept-package-agreements --accept-source-agreements
        
        if %ERRORLEVEL% EQU 0 (
            echo       ✓ .NET SDK instalado
        ) else (
            echo       ERROR: Fallo al instalar con winget
            echo.
            echo       Por favor instala manualmente desde:
            echo       https://dotnet.microsoft.com/download/dotnet/8.0
            echo.
            pause
            exit /b 1
        )
    ) else (
        echo       ⚠ winget no encontrado
        echo.
        echo       Opciones de instalacion:
        echo       1. Descargar desde: https://dotnet.microsoft.com/download/dotnet/8.0
        echo       2. Usar Chocolatey: choco install dotnet-sdk
        echo       3. Instalar winget y ejecutar: winget install Microsoft.DotNet.SDK.8
        echo.
        
        set /p OPEN_BROWSER="¿Deseas abrir el navegador para descargar .NET SDK? (S/N): "
        if /i "!OPEN_BROWSER!"=="S" (
            start https://dotnet.microsoft.com/download/dotnet/8.0
        )
        
        echo.
        echo Por favor instala .NET SDK y ejecuta este script nuevamente.
        pause
        exit /b 1
    )
) else (
    echo [3/6] .NET SDK ya esta instalado
)
echo.

REM [4/6] Navegar al directorio del proyecto
echo [4/6] Navegando al directorio del proyecto...
cd /d "%~dp0"
echo       ✓ En directorio: %CD%
echo.

REM [5/6] Limpiar compilaciones anteriores
echo [5/6] Limpiando compilaciones anteriores...
if exist "bin" (
    rmdir /s /q "bin"
    echo       ✓ Carpeta bin eliminada
)
if exist "obj" (
    rmdir /s /q "obj"
    echo       ✓ Carpeta obj eliminada
)
echo.

REM [6/6] Restaurar dependencias NuGet
echo [6/6] Restaurando paquetes NuGet...
echo       Esto descargara las siguientes dependencias:
echo       - System.Management ^(8.0.0^)
echo       - Microsoft.Data.Sqlite ^(8.0.0^)
echo       - ManagedNativeWifi ^(2.0.0^)
echo       - AForge.Video ^(2.2.5^)
echo       - AForge.Video.DirectShow ^(2.2.5^)
echo.

dotnet restore

if %ERRORLEVEL% EQU 0 (
    echo       ✓ Dependencias restauradas exitosamente
) else (
    echo       ERROR: Fallo al restaurar dependencias
    echo.
    echo       Intentando con limpieza forzada...
    
    dotnet nuget locals all --clear
    dotnet restore --force
    
    if %ERRORLEVEL% EQU 0 (
        echo       ✓ Dependencias restauradas exitosamente ^(segundo intento^)
    ) else (
        echo       ERROR: No se pudieron restaurar las dependencias
        echo.
        echo       Posibles soluciones:
        echo       1. Verificar conexion a internet
        echo       2. Verificar que NuGet.org este accesible
        echo       3. Ejecutar como administrador
        echo       4. Verificar configuracion de proxy si aplica
        echo.
        pause
        exit /b 1
    )
)
echo.

REM [Bonus] Crear carpetas de Assets
echo [Bonus] Creando estructura de carpetas Assets...
if not exist "Assets\Avatars" mkdir "Assets\Avatars"
if not exist "Assets\Wallpapers" mkdir "Assets\Wallpapers"
if not exist "Assets\LockScreens" mkdir "Assets\LockScreens"
echo       ✓ Carpetas Assets creadas
echo.

echo ========================================
echo   Instalacion Completada
echo ========================================
echo.
echo Proximos pasos:
echo.
echo 1. Compilar el proyecto:
echo    dotnet build -c Release
echo.
echo 2. O usar el script de compilacion rapida:
echo    Build.bat
echo.
echo 3. O publicar como ejecutable unico:
echo    dotnet publish -c Release -r win-x64 --self-contained true -p:PublishSingleFile=true
echo.
echo 4. Agregar imagenes a las carpetas Assets:
echo    - Assets\Avatars\ ^(PNG 512x512^)
echo    - Assets\Wallpapers\ ^(JPG/PNG 1920x1080+^)
echo    - Assets\LockScreens\ ^(JPG/PNG 1920x1080^)
echo.

REM Preguntar si desea compilar ahora
set /p COMPILE="¿Deseas compilar el proyecto ahora? (S/N): "
if /i "!COMPILE!"=="S" (
    echo.
    echo Compilando proyecto...
    dotnet build -c Release
    
    if %ERRORLEVEL% EQU 0 (
        echo.
        echo ✓ Compilacion exitosa!
        echo.
        echo El ejecutable se encuentra en:
        echo bin\Release\net8.0-windows\CustomOOBE.exe
    ) else (
        echo.
        echo ERROR: La compilacion fallo. Revisa los errores arriba.
    )
)

echo.
pause
