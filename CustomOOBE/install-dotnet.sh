#!/bin/bash
# Script para instalar .NET 8.0 SDK en diferentes sistemas Linux

echo "========================================"
echo "  Instalador de .NET 8.0 SDK"
echo "========================================"
echo ""

# Colores
GREEN='\033[0;32m'
RED='\033[0;31m'
YELLOW='\033[1;33m'
CYAN='\033[0;36m'
NC='\033[0m'

# Verificar si ya está instalado
if command -v dotnet &> /dev/null; then
    DOTNET_VERSION=$(dotnet --version)
    echo -e "${GREEN}✓ .NET SDK ya está instalado (versión $DOTNET_VERSION)${NC}"
    
    MAJOR_VERSION=$(echo $DOTNET_VERSION | cut -d. -f1)
    if [ "$MAJOR_VERSION" -ge 8 ]; then
        echo -e "${GREEN}✓ La versión es compatible (8.0+)${NC}"
        exit 0
    else
        echo -e "${YELLOW}⚠ Se requiere versión 8.0 o superior${NC}"
        echo "Continuando con la instalación..."
        echo ""
    fi
fi

# Detectar sistema operativo
if [ -f /etc/os-release ]; then
    . /etc/os-release
    OS=$ID
    VERSION=$VERSION_ID
    echo "Sistema detectado: $PRETTY_NAME"
else
    echo -e "${RED}ERROR: No se pudo detectar el sistema operativo${NC}"
    exit 1
fi

echo ""
echo "Método de instalación recomendado:"
echo ""

case $OS in
    ubuntu|debian)
        echo -e "${CYAN}=== Ubuntu/Debian ===${NC}"
        echo ""
        echo "Ejecuta los siguientes comandos:"
        echo ""
        echo -e "${YELLOW}# Descargar script de instalación${NC}"
        echo "wget https://dot.net/v1/dotnet-install.sh -O dotnet-install.sh"
        echo "chmod +x dotnet-install.sh"
        echo ""
        echo -e "${YELLOW}# Instalar .NET 8.0 SDK${NC}"
        echo "./dotnet-install.sh --channel 8.0"
        echo ""
        echo -e "${YELLOW}# Agregar al PATH${NC}"
        echo 'export DOTNET_ROOT=$HOME/.dotnet'
        echo 'export PATH=$PATH:$DOTNET_ROOT:$DOTNET_ROOT/tools'
        echo ""
        echo -e "${YELLOW}# Hacer permanente (agregar a ~/.bashrc o ~/.zshrc)${NC}"
        echo 'echo "export DOTNET_ROOT=\$HOME/.dotnet" >> ~/.bashrc'
        echo 'echo "export PATH=\$PATH:\$DOTNET_ROOT:\$DOTNET_ROOT/tools" >> ~/.bashrc'
        echo 'source ~/.bashrc'
        echo ""
        echo "O usando el gestor de paquetes:"
        echo ""
        echo "wget https://packages.microsoft.com/config/ubuntu/$VERSION/packages-microsoft-prod.deb"
        echo "sudo dpkg -i packages-microsoft-prod.deb"
        echo "sudo apt-get update"
        echo "sudo apt-get install -y dotnet-sdk-8.0"
        ;;
        
    fedora|rhel|centos)
        echo -e "${CYAN}=== Fedora/RHEL/CentOS ===${NC}"
        echo ""
        echo "sudo dnf install -y dotnet-sdk-8.0"
        echo ""
        echo "O usando el script de instalación:"
        echo ""
        echo "wget https://dot.net/v1/dotnet-install.sh"
        echo "chmod +x dotnet-install.sh"
        echo "./dotnet-install.sh --channel 8.0"
        ;;
        
    arch|manjaro)
        echo -e "${CYAN}=== Arch Linux ===${NC}"
        echo ""
        echo "sudo pacman -S dotnet-sdk"
        ;;
        
    amzn)
        echo -e "${CYAN}=== Amazon Linux ===${NC}"
        echo ""
        echo "Amazon Linux no tiene paquetes oficiales de .NET en sus repositorios."
        echo "Usa el script de instalación universal:"
        echo ""
        echo -e "${YELLOW}# Descargar e instalar${NC}"
        echo "wget https://dot.net/v1/dotnet-install.sh"
        echo "chmod +x dotnet-install.sh"
        echo "./dotnet-install.sh --channel 8.0"
        echo ""
        echo -e "${YELLOW}# Agregar al PATH${NC}"
        echo 'export DOTNET_ROOT=$HOME/.dotnet'
        echo 'export PATH=$PATH:$DOTNET_ROOT:$DOTNET_ROOT/tools'
        echo ""
        echo -e "${YELLOW}# Hacer permanente${NC}"
        echo 'echo "export DOTNET_ROOT=\$HOME/.dotnet" >> ~/.bashrc'
        echo 'echo "export PATH=\$PATH:\$DOTNET_ROOT:\$DOTNET_ROOT/tools" >> ~/.bashrc'
        echo 'source ~/.bashrc'
        ;;
        
    *)
        echo -e "${YELLOW}Sistema no reconocido: $OS${NC}"
        echo ""
        echo "Usa el script de instalación universal:"
        echo ""
        echo "wget https://dot.net/v1/dotnet-install.sh"
        echo "chmod +x dotnet-install.sh"
        echo "./dotnet-install.sh --channel 8.0"
        ;;
esac

echo ""
echo "========================================"
echo "  Instalación Automática"
echo "========================================"
echo ""
read -p "¿Deseas intentar la instalación automática ahora? (s/N): " -n 1 -r
echo ""

if [[ $REPLY =~ ^[Ss]$ ]]; then
    echo ""
    echo "Descargando script de instalación de Microsoft..."
    
    wget https://dot.net/v1/dotnet-install.sh -O /tmp/dotnet-install.sh
    
    if [ $? -eq 0 ]; then
        chmod +x /tmp/dotnet-install.sh
        
        echo "Instalando .NET 8.0 SDK..."
        /tmp/dotnet-install.sh --channel 8.0
        
        if [ $? -eq 0 ]; then
            echo ""
            echo -e "${GREEN}✓ .NET SDK instalado exitosamente${NC}"
            echo ""
            echo "Configurando PATH..."
            
            export DOTNET_ROOT=$HOME/.dotnet
            export PATH=$PATH:$DOTNET_ROOT:$DOTNET_ROOT/tools
            
            # Agregar a .bashrc si no existe
            if ! grep -q "DOTNET_ROOT" ~/.bashrc; then
                echo "" >> ~/.bashrc
                echo "# .NET SDK" >> ~/.bashrc
                echo 'export DOTNET_ROOT=$HOME/.dotnet' >> ~/.bashrc
                echo 'export PATH=$PATH:$DOTNET_ROOT:$DOTNET_ROOT/tools' >> ~/.bashrc
                echo -e "${GREEN}✓ PATH configurado en ~/.bashrc${NC}"
            fi
            
            echo ""
            echo "Verificando instalación..."
            $DOTNET_ROOT/dotnet --version
            
            echo ""
            echo -e "${GREEN}========================================"
            echo "  Instalación Completada"
            echo "========================================${NC}"
            echo ""
            echo "Para usar dotnet en esta sesión, ejecuta:"
            echo -e "${YELLOW}source ~/.bashrc${NC}"
            echo ""
            echo "O cierra y abre una nueva terminal."
            echo ""
            echo "Luego ejecuta:"
            echo -e "${YELLOW}./restore-dependencies.sh${NC}"
            echo ""
        else
            echo -e "${RED}ERROR: Fallo en la instalación${NC}"
            exit 1
        fi
    else
        echo -e "${RED}ERROR: No se pudo descargar el script de instalación${NC}"
        echo "Verifica tu conexión a internet"
        exit 1
    fi
else
    echo ""
    echo "Instalación cancelada."
    echo "Puedes instalar manualmente siguiendo las instrucciones arriba."
fi

echo ""
echo "Documentación oficial:"
echo "https://dotnet.microsoft.com/download/dotnet/8.0"
echo ""
