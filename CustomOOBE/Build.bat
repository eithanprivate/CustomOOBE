@echo off
REM Script de compilación rápida para Custom OOBE

echo ========================================
echo   Custom OOBE - Compilación Rápida
echo ========================================
echo.

REM Verificar que existe dotnet
where dotnet >nul 2>nul
if %ERRORLEVEL% NEQ 0 (
    echo ERROR: .NET SDK no encontrado
    echo Por favor instala .NET 8.0 SDK desde:
    echo https://dotnet.microsoft.com/download
    pause
    exit /b 1
)

echo [1/5] Limpiando compilaciones anteriores...
if exist "bin" rmdir /s /q bin
if exist "obj" rmdir /s /q obj
echo       ✓ Limpieza completada

echo.
echo [2/5] Restaurando dependencias...
dotnet restore
if %ERRORLEVEL% NEQ 0 (
    echo ERROR: Fallo al restaurar dependencias
    pause
    exit /b 1
)
echo       ✓ Dependencias restauradas

echo.
echo [3/5] Compilando en modo Release...
dotnet build -c Release
if %ERRORLEVEL% NEQ 0 (
    echo ERROR: Fallo en la compilación
    pause
    exit /b 1
)
echo       ✓ Compilación exitosa

echo.
echo [4/5] Creando carpetas Assets...
powershell -ExecutionPolicy Bypass -File "Scripts\CreateAssetsFolders.ps1"

echo.
echo [5/5] Publicando aplicación...
dotnet publish -c Release -r win-x64 --self-contained true -p:PublishSingleFile=true -p:IncludeNativeLibrariesForSelfExtract=true
if %ERRORLEVEL% NEQ 0 (
    echo ERROR: Fallo en la publicación
    pause
    exit /b 1
)
echo       ✓ Publicación completada

echo.
echo ========================================
echo   Compilación Completada Exitosamente
echo ========================================
echo.
echo El ejecutable se encuentra en:
echo bin\Release\net8.0-windows\win-x64\publish\CustomOOBE.exe
echo.
echo Próximos pasos:
echo 1. Agrega imágenes a la carpeta Assets
echo 2. Ejecuta Scripts\TestOOBE.bat para probar
echo 3. Ejecuta Scripts\SetupOOBE.ps1 para instalar
echo.
pause
