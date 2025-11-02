#!/bin/bash

echo "========================================"
echo "  Custom OOBE - Instalación de Dependencias"
echo "========================================"
echo ""

# Colores para output
RED='\033[0;31m'
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
NC='\033[0m' # No Color

# Función para verificar si un comando existe
command_exists() {
    command -v "$1" >/dev/null 2>&1
}

# Función para imprimir mensajes de error
error() {
    echo -e "${RED}ERROR: $1${NC}"
}

# Función para imprimir mensajes de éxito
success() {
    echo -e "${GREEN}✓ $1${NC}"
}

# Función para imprimir mensajes de advertencia
warning() {
    echo -e "${YELLOW}⚠ $1${NC}"
}

# Detectar sistema operativo
OS="unknown"
if [[ "$OSTYPE" == "linux-gnu"* ]]; then
    OS="linux"
elif [[ "$OSTYPE" == "darwin"* ]]; then
    OS="macos"
elif [[ "$OSTYPE" == "msys" ]] || [[ "$OSTYPE" == "cygwin" ]] || [[ "$OSTYPE" == "win32" ]]; then
    OS="windows"
fi

echo "[1/6] Detectando sistema operativo..."
echo "      Sistema detectado: $OS"
echo ""

# Verificar si .NET SDK está instalado
echo "[2/6] Verificando .NET SDK..."
if command_exists dotnet; then
    DOTNET_VERSION=$(dotnet --version)
    success ".NET SDK encontrado (versión $DOTNET_VERSION)"
    
    # Verificar si es versión 8.0 o superior
    MAJOR_VERSION=$(echo $DOTNET_VERSION | cut -d. -f1)
    if [ "$MAJOR_VERSION" -lt 8 ]; then
        warning "Se requiere .NET 8.0 o superior. Versión actual: $DOTNET_VERSION"
        INSTALL_DOTNET=true
    else
        INSTALL_DOTNET=false
    fi
else
    error ".NET SDK no encontrado"
    INSTALL_DOTNET=true
fi
echo ""

# Instalar .NET SDK si es necesario
if [ "$INSTALL_DOTNET" = true ]; then
    echo "[3/6] Instalando .NET 8.0 SDK..."
    
    if [ "$OS" = "linux" ]; then
        # Detectar distribución de Linux
        if [ -f /etc/os-release ]; then
            . /etc/os-release
            DISTRO=$ID
            VERSION_ID=$VERSION_ID
            
            echo "      Distribución detectada: $DISTRO $VERSION_ID"
            
            case $DISTRO in
                ubuntu|debian)
                    echo "      Instalando dependencias para Ubuntu/Debian..."
                    sudo apt-get update
                    sudo apt-get install -y wget apt-transport-https
                    
                    # Agregar repositorio de Microsoft
                    wget https://packages.microsoft.com/config/ubuntu/$(lsb_release -rs)/packages-microsoft-prod.deb -O packages-microsoft-prod.deb
                    sudo dpkg -i packages-microsoft-prod.deb
                    rm packages-microsoft-prod.deb
                    
                    # Instalar .NET SDK
                    sudo apt-get update
                    sudo apt-get install -y dotnet-sdk-8.0
                    ;;
                    
                fedora|rhel|centos)
                    echo "      Instalando dependencias para Fedora/RHEL/CentOS..."
                    sudo dnf install -y dotnet-sdk-8.0
                    ;;
                    
                arch)
                    echo "      Instalando dependencias para Arch Linux..."
                    sudo pacman -S --noconfirm dotnet-sdk
                    ;;
                    
                *)
                    error "Distribución no soportada automáticamente: $DISTRO"
                    echo "      Por favor instala .NET 8.0 SDK manualmente desde:"
                    echo "      https://dotnet.microsoft.com/download/dotnet/8.0"
                    exit 1
                    ;;
            esac
        fi
        
    elif [ "$OS" = "macos" ]; then
        echo "      Instalando para macOS..."
        if command_exists brew; then
            brew install --cask dotnet-sdk
        else
            error "Homebrew no encontrado. Por favor instala Homebrew primero:"
            echo "      /bin/bash -c \"\$(curl -fsSL https://raw.githubusercontent.com/Homebrew/install/HEAD/install.sh)\""
            exit 1
        fi
        
    elif [ "$OS" = "windows" ]; then
        error "Por favor instala .NET 8.0 SDK manualmente desde:"
        echo "      https://dotnet.microsoft.com/download/dotnet/8.0"
        echo "      O usa winget: winget install Microsoft.DotNet.SDK.8"
        exit 1
    fi
    
    success ".NET SDK instalado"
else
    echo "[3/6] .NET SDK ya está instalado"
fi
echo ""

# Navegar al directorio del proyecto
echo "[4/6] Navegando al directorio del proyecto..."
cd "$(dirname "$0")"
success "En directorio: $(pwd)"
echo ""

# Limpiar compilaciones anteriores
echo "[5/6] Limpiando compilaciones anteriores..."
if [ -d "bin" ]; then
    rm -rf bin
    success "Carpeta bin eliminada"
fi
if [ -d "obj" ]; then
    rm -rf obj
    success "Carpeta obj eliminada"
fi
echo ""

# Restaurar dependencias NuGet
echo "[6/6] Restaurando paquetes NuGet..."
echo "      Esto descargará las siguientes dependencias:"
echo "      - System.Management (8.0.0)"
echo "      - Microsoft.Data.Sqlite (8.0.0)"
echo "      - ManagedNativeWifi (2.0.0)"
echo "      - AForge.Video (2.2.5)"
echo "      - AForge.Video.DirectShow (2.2.5)"
echo ""

dotnet restore

if [ $? -eq 0 ]; then
    success "Dependencias restauradas exitosamente"
else
    error "Fallo al restaurar dependencias"
    echo ""
    echo "Intentando con limpieza forzada..."
    dotnet nuget locals all --clear
    dotnet restore --force
    
    if [ $? -eq 0 ]; then
        success "Dependencias restauradas exitosamente (segundo intento)"
    else
        error "No se pudieron restaurar las dependencias"
        exit 1
    fi
fi
echo ""

# Crear carpetas de Assets si no existen
echo "[Bonus] Creando estructura de carpetas Assets..."
mkdir -p Assets/Avatars
mkdir -p Assets/Wallpapers
mkdir -p Assets/LockScreens
success "Carpetas Assets creadas"
echo ""

echo "========================================"
echo "  Instalación Completada"
echo "========================================"
echo ""
echo "Próximos pasos:"
echo "1. Compilar el proyecto:"
echo "   dotnet build -c Release"
echo ""
echo "2. O publicar como ejecutable único:"
echo "   dotnet publish -c Release -r win-x64 --self-contained true -p:PublishSingleFile=true"
echo ""
echo "3. Agregar imágenes a las carpetas Assets:"
echo "   - Assets/Avatars/ (PNG 512x512)"
echo "   - Assets/Wallpapers/ (JPG/PNG 1920x1080+)"
echo "   - Assets/LockScreens/ (JPG/PNG 1920x1080)"
echo ""
echo "Nota: Este proyecto está diseñado para Windows."
echo "Para compilar en Linux/macOS, solo se restaurarán las dependencias,"
echo "pero la ejecución requiere Windows debido a WPF y APIs específicas."
echo ""
