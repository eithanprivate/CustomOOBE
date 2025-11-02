#!/bin/bash
# Script simplificado para restaurar solo las dependencias NuGet
# Asume que .NET SDK ya está instalado

echo "========================================"
echo "  Restauración de Dependencias NuGet"
echo "========================================"
echo ""

# Colores
GREEN='\033[0;32m'
RED='\033[0;31m'
YELLOW='\033[1;33m'
NC='\033[0m'

# Verificar que dotnet está instalado
if ! command -v dotnet &> /dev/null; then
    echo -e "${RED}ERROR: .NET SDK no encontrado${NC}"
    echo ""
    echo "Por favor instala .NET 8.0 SDK primero:"
    echo ""
    echo "Windows:"
    echo "  winget install Microsoft.DotNet.SDK.8"
    echo "  O descarga desde: https://dotnet.microsoft.com/download/dotnet/8.0"
    echo ""
    echo "Ubuntu/Debian:"
    echo "  wget https://dot.net/v1/dotnet-install.sh"
    echo "  chmod +x dotnet-install.sh"
    echo "  ./dotnet-install.sh --channel 8.0"
    echo ""
    echo "macOS:"
    echo "  brew install --cask dotnet-sdk"
    echo ""
    exit 1
fi

DOTNET_VERSION=$(dotnet --version)
echo -e "${GREEN}✓ .NET SDK encontrado (versión $DOTNET_VERSION)${NC}"
echo ""

# Navegar al directorio del script
cd "$(dirname "$0")"
echo "Directorio de trabajo: $(pwd)"
echo ""

# Limpiar compilaciones anteriores
echo "Limpiando compilaciones anteriores..."
rm -rf bin obj
echo -e "${GREEN}✓ Limpieza completada${NC}"
echo ""

# Restaurar dependencias
echo "Restaurando paquetes NuGet..."
echo "Dependencias a descargar:"
echo "  - System.Management (8.0.0)"
echo "  - Microsoft.Data.Sqlite (8.0.0)"
echo "  - ManagedNativeWifi (2.0.0)"
echo "  - AForge.Video (2.2.5)"
echo "  - AForge.Video.DirectShow (2.2.5)"
echo ""

dotnet restore

if [ $? -eq 0 ]; then
    echo ""
    echo -e "${GREEN}✓ Dependencias restauradas exitosamente${NC}"
else
    echo ""
    echo -e "${RED}ERROR: Fallo al restaurar dependencias${NC}"
    echo ""
    echo "Intentando con limpieza de caché..."
    dotnet nuget locals all --clear
    dotnet restore --force
    
    if [ $? -eq 0 ]; then
        echo ""
        echo -e "${GREEN}✓ Dependencias restauradas (segundo intento)${NC}"
    else
        echo ""
        echo -e "${RED}ERROR: No se pudieron restaurar las dependencias${NC}"
        echo ""
        echo "Posibles soluciones:"
        echo "1. Verificar conexión a internet"
        echo "2. Verificar acceso a https://api.nuget.org"
        echo "3. Revisar configuración de proxy"
        echo "4. Ejecutar con permisos elevados"
        exit 1
    fi
fi

# Crear carpetas Assets
echo ""
echo "Creando estructura de carpetas Assets..."
mkdir -p Assets/Avatars
mkdir -p Assets/Wallpapers
mkdir -p Assets/LockScreens
echo -e "${GREEN}✓ Carpetas creadas${NC}"

echo ""
echo "========================================"
echo "  ✓ Restauración Completada"
echo "========================================"
echo ""
echo "Próximos pasos:"
echo ""
echo "1. Compilar (solo en Windows):"
echo "   dotnet build -c Release"
echo ""
echo "2. Publicar ejecutable:"
echo "   dotnet publish -c Release -r win-x64 --self-contained"
echo ""
echo "Nota: Este proyecto requiere Windows para compilar"
echo "debido a WPF y APIs específicas de Windows."
echo ""
