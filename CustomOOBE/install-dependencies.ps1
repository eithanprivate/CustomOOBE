# Script de instalación de dependencias para Custom OOBE
# PowerShell version

Write-Host "========================================" -ForegroundColor Cyan
Write-Host "  Custom OOBE - Instalación de Dependencias" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""

# Función para verificar si un comando existe
function Test-CommandExists {
    param($command)
    $null = Get-Command $command -ErrorAction SilentlyContinue
    return $?
}

# Función para imprimir mensajes de éxito
function Write-Success {
    param($message)
    Write-Host "✓ $message" -ForegroundColor Green
}

# Función para imprimir mensajes de error
function Write-Error-Custom {
    param($message)
    Write-Host "ERROR: $message" -ForegroundColor Red
}

# Función para imprimir mensajes de advertencia
function Write-Warning-Custom {
    param($message)
    Write-Host "⚠ $message" -ForegroundColor Yellow
}

# [1/6] Verificar sistema operativo
Write-Host "[1/6] Verificando sistema operativo..." -ForegroundColor Yellow
if ($IsWindows -or $env:OS -match "Windows") {
    Write-Success "Windows detectado"
} else {
    Write-Warning-Custom "Este proyecto está diseñado para Windows (WPF)"
}
Write-Host ""

# [2/6] Verificar .NET SDK
Write-Host "[2/6] Verificando .NET SDK..." -ForegroundColor Yellow
if (Test-CommandExists dotnet) {
    $dotnetVersion = dotnet --version
    Write-Success ".NET SDK encontrado (versión $dotnetVersion)"
    
    # Verificar si es versión 8.0 o superior
    $majorVersion = [int]($dotnetVersion.Split('.')[0])
    if ($majorVersion -lt 8) {
        Write-Warning-Custom "Se requiere .NET 8.0 o superior. Versión actual: $dotnetVersion"
        $installDotnet = $true
    } else {
        $installDotnet = $false
    }
} else {
    Write-Error-Custom ".NET SDK no encontrado"
    $installDotnet = $true
}
Write-Host ""

# [3/6] Instalar .NET SDK si es necesario
if ($installDotnet) {
    Write-Host "[3/6] Instalando .NET 8.0 SDK..." -ForegroundColor Yellow
    
    # Verificar si winget está disponible
    if (Test-CommandExists winget) {
        Write-Host "      Usando winget para instalar .NET SDK..."
        try {
            winget install Microsoft.DotNet.SDK.8 --silent --accept-package-agreements --accept-source-agreements
            Write-Success ".NET SDK instalado"
        } catch {
            Write-Error-Custom "Fallo al instalar con winget"
            Write-Host "      Por favor instala manualmente desde:"
            Write-Host "      https://dotnet.microsoft.com/download/dotnet/8.0"
            exit 1
        }
    } else {
        Write-Warning-Custom "winget no encontrado"
        Write-Host ""
        Write-Host "      Opciones de instalación:"
        Write-Host "      1. Descargar desde: https://dotnet.microsoft.com/download/dotnet/8.0"
        Write-Host "      2. Usar Chocolatey: choco install dotnet-sdk"
        Write-Host "      3. Instalar winget y ejecutar: winget install Microsoft.DotNet.SDK.8"
        Write-Host ""
        
        $response = Read-Host "¿Deseas abrir el navegador para descargar .NET SDK? (S/N)"
        if ($response -eq "S" -or $response -eq "s") {
            Start-Process "https://dotnet.microsoft.com/download/dotnet/8.0"
        }
        exit 1
    }
} else {
    Write-Host "[3/6] .NET SDK ya está instalado" -ForegroundColor Yellow
}
Write-Host ""

# [4/6] Navegar al directorio del proyecto
Write-Host "[4/6] Navegando al directorio del proyecto..." -ForegroundColor Yellow
$scriptPath = Split-Path -Parent $MyInvocation.MyCommand.Path
Set-Location $scriptPath
Write-Success "En directorio: $(Get-Location)"
Write-Host ""

# [5/6] Limpiar compilaciones anteriores
Write-Host "[5/6] Limpiando compilaciones anteriores..." -ForegroundColor Yellow
if (Test-Path "bin") {
    Remove-Item -Recurse -Force "bin"
    Write-Success "Carpeta bin eliminada"
}
if (Test-Path "obj") {
    Remove-Item -Recurse -Force "obj"
    Write-Success "Carpeta obj eliminada"
}
Write-Host ""

# [6/6] Restaurar dependencias NuGet
Write-Host "[6/6] Restaurando paquetes NuGet..." -ForegroundColor Yellow
Write-Host "      Esto descargará las siguientes dependencias:"
Write-Host "      - System.Management (8.0.0)"
Write-Host "      - Microsoft.Data.Sqlite (8.0.0)"
Write-Host "      - ManagedNativeWifi (2.0.0)"
Write-Host "      - AForge.Video (2.2.5)"
Write-Host "      - AForge.Video.DirectShow (2.2.5)"
Write-Host ""

try {
    dotnet restore
    if ($LASTEXITCODE -eq 0) {
        Write-Success "Dependencias restauradas exitosamente"
    } else {
        throw "Fallo al restaurar dependencias"
    }
} catch {
    Write-Error-Custom "Fallo al restaurar dependencias"
    Write-Host ""
    Write-Host "Intentando con limpieza forzada..." -ForegroundColor Yellow
    
    dotnet nuget locals all --clear
    dotnet restore --force
    
    if ($LASTEXITCODE -eq 0) {
        Write-Success "Dependencias restauradas exitosamente (segundo intento)"
    } else {
        Write-Error-Custom "No se pudieron restaurar las dependencias"
        Write-Host ""
        Write-Host "Posibles soluciones:"
        Write-Host "1. Verificar conexión a internet"
        Write-Host "2. Verificar que NuGet.org esté accesible"
        Write-Host "3. Ejecutar como administrador"
        Write-Host "4. Verificar configuración de proxy si aplica"
        exit 1
    }
}
Write-Host ""

# [Bonus] Crear carpetas de Assets
Write-Host "[Bonus] Creando estructura de carpetas Assets..." -ForegroundColor Yellow
$null = New-Item -ItemType Directory -Force -Path "Assets\Avatars"
$null = New-Item -ItemType Directory -Force -Path "Assets\Wallpapers"
$null = New-Item -ItemType Directory -Force -Path "Assets\LockScreens"
Write-Success "Carpetas Assets creadas"
Write-Host ""

# Verificar compilación
Write-Host "========================================" -ForegroundColor Cyan
Write-Host "  Instalación Completada" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""
Write-Host "Próximos pasos:" -ForegroundColor Green
Write-Host ""
Write-Host "1. Compilar el proyecto:" -ForegroundColor White
Write-Host "   dotnet build -c Release" -ForegroundColor Gray
Write-Host ""
Write-Host "2. O usar el script de compilación rápida:" -ForegroundColor White
Write-Host "   .\Build.bat" -ForegroundColor Gray
Write-Host ""
Write-Host "3. O publicar como ejecutable único:" -ForegroundColor White
Write-Host "   dotnet publish -c Release -r win-x64 --self-contained true -p:PublishSingleFile=true" -ForegroundColor Gray
Write-Host ""
Write-Host "4. Agregar imágenes a las carpetas Assets:" -ForegroundColor White
Write-Host "   - Assets\Avatars\ (PNG 512x512)" -ForegroundColor Gray
Write-Host "   - Assets\Wallpapers\ (JPG/PNG 1920x1080+)" -ForegroundColor Gray
Write-Host "   - Assets\LockScreens\ (JPG/PNG 1920x1080)" -ForegroundColor Gray
Write-Host ""

# Preguntar si desea compilar ahora
$compile = Read-Host "¿Deseas compilar el proyecto ahora? (S/N)"
if ($compile -eq "S" -or $compile -eq "s") {
    Write-Host ""
    Write-Host "Compilando proyecto..." -ForegroundColor Yellow
    dotnet build -c Release
    
    if ($LASTEXITCODE -eq 0) {
        Write-Host ""
        Write-Success "Compilación exitosa!"
        Write-Host ""
        Write-Host "El ejecutable se encuentra en:" -ForegroundColor Green
        Write-Host "bin\Release\net8.0-windows\CustomOOBE.exe" -ForegroundColor Cyan
    } else {
        Write-Host ""
        Write-Error-Custom "La compilación falló. Revisa los errores arriba."
    }
}

Write-Host ""
Write-Host "Presiona cualquier tecla para salir..."
$null = $Host.UI.RawUI.ReadKey("NoEcho,IncludeKeyDown")
