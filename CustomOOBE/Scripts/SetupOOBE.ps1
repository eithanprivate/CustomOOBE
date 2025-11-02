# Script de configuración del Custom OOBE
# Este script debe ejecutarse con privilegios de administrador

param(
    [string]$InstallPath = "C:\CustomOOBE"
)

Write-Host "========================================" -ForegroundColor Cyan
Write-Host "  Custom OOBE - Script de Instalación  " -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""

# Verificar privilegios de administrador
$isAdmin = ([Security.Principal.WindowsPrincipal] [Security.Principal.WindowsIdentity]::GetCurrent()).IsInRole([Security.Principal.WindowsBuiltInRole]::Administrator)

if (-not $isAdmin) {
    Write-Host "ERROR: Este script requiere privilegios de administrador." -ForegroundColor Red
    Write-Host "Por favor, ejecute PowerShell como administrador." -ForegroundColor Yellow
    exit 1
}

Write-Host "[1/7] Creando directorio de instalación..." -ForegroundColor Green
if (-not (Test-Path $InstallPath)) {
    New-Item -ItemType Directory -Path $InstallPath -Force | Out-Null
}

Write-Host "[2/7] Copiando archivos..." -ForegroundColor Green
# Copiar los archivos del ejecutable al directorio de instalación
$sourcePath = Split-Path -Parent $PSScriptRoot
Copy-Item -Path "$sourcePath\*" -Destination $InstallPath -Recurse -Force

Write-Host "[3/7] Configurando registro de Windows..." -ForegroundColor Green
# Crear clave de registro
$regPath = "HKLM:\SOFTWARE\CustomOOBE"
if (-not (Test-Path $regPath)) {
    New-Item -Path $regPath -Force | Out-Null
}
Set-ItemProperty -Path $regPath -Name "InstallPath" -Value $InstallPath
Set-ItemProperty -Path $regPath -Name "InstallDate" -Value (Get-Date).ToString("o")
Set-ItemProperty -Path $regPath -Name "Completed" -Value 0

Write-Host "[4/7] Configurando auto-inicio..." -ForegroundColor Green
# Crear tarea programada para ejecutar antes del inicio de sesión
$taskName = "CustomOOBE"
$taskPath = "\Microsoft\Windows\Setup"
$exePath = Join-Path $InstallPath "CustomOOBE.exe"

# Eliminar tarea si existe
Unregister-ScheduledTask -TaskName $taskName -TaskPath $taskPath -Confirm:$false -ErrorAction SilentlyContinue

# Crear nueva tarea
$action = New-ScheduledTaskAction -Execute $exePath
$trigger = New-ScheduledTaskTrigger -AtStartup
$principal = New-ScheduledTaskPrincipal -UserId "SYSTEM" -LogonType ServiceAccount -RunLevel Highest
$settings = New-ScheduledTaskSettingsSet -AllowStartIfOnBatteries -DontStopIfGoingOnBatteries -StartWhenAvailable

Register-ScheduledTask -TaskName $taskName -TaskPath $taskPath -Action $action -Trigger $trigger -Principal $principal -Settings $settings | Out-Null

Write-Host "[5/7] Configurando inicio automático en registro..." -ForegroundColor Green
# Agregar al registro Run para asegurar ejecución
$runPath = "HKLM:\SOFTWARE\Microsoft\Windows\CurrentVersion\Run"
Set-ItemProperty -Path $runPath -Name "CustomOOBE" -Value "`"$exePath`""

Write-Host "[6/7] Deshabilitando OOBE de Windows..." -ForegroundColor Green
# Deshabilitar el OOBE estándar de Windows
$oobePath = "HKLM:\SOFTWARE\Microsoft\Windows\CurrentVersion\OOBE"
if (Test-Path $oobePath) {
    Set-ItemProperty -Path $oobePath -Name "DisablePrivacyExperience" -Value 1 -Type DWord -ErrorAction SilentlyContinue
}

# Crear archivo de bypass para OOBE de Windows
$unattendPath = "C:\Windows\System32\Sysprep"
if (Test-Path $unattendPath) {
    $unattendXml = @"
<?xml version="1.0" encoding="utf-8"?>
<unattend xmlns="urn:schemas-microsoft-com:unattend">
    <settings pass="oobeSystem">
        <component name="Microsoft-Windows-Shell-Setup" processorArchitecture="amd64" publicKeyToken="31bf3856ad364e35" language="neutral" versionScope="nonSxS">
            <OOBE>
                <HideEULAPage>true</HideEULAPage>
                <HideOEMRegistrationScreen>true</HideOEMRegistrationScreen>
                <HideOnlineAccountScreens>true</HideOnlineAccountScreens>
                <HideWirelessSetupInOOBE>true</HideWirelessSetupInOOBE>
                <ProtectYourPC>3</ProtectYourPC>
            </OOBE>
        </component>
    </settings>
</unattend>
"@
    $unattendXml | Out-File -FilePath "$unattendPath\unattend.xml" -Encoding UTF8 -Force
}

Write-Host "[7/7] Configurando Shell personalizado..." -ForegroundColor Green
# Temporalmente establecer CustomOOBE como shell
$shellPath = "HKLM:\SOFTWARE\Microsoft\Windows NT\CurrentVersion\Winlogon"
$originalShell = Get-ItemProperty -Path $shellPath -Name "Shell" -ErrorAction SilentlyContinue
if ($originalShell) {
    # Guardar el shell original
    Set-ItemProperty -Path $regPath -Name "OriginalShell" -Value $originalShell.Shell
}

Write-Host ""
Write-Host "========================================" -ForegroundColor Green
Write-Host "  Instalación completada exitosamente  " -ForegroundColor Green
Write-Host "========================================" -ForegroundColor Green
Write-Host ""
Write-Host "El Custom OOBE se ejecutará automáticamente en el próximo reinicio." -ForegroundColor Cyan
Write-Host ""
Write-Host "Para desinstalar, ejecute: Uninstall-CustomOOBE.ps1" -ForegroundColor Yellow
Write-Host ""

# Preguntar si desea reiniciar ahora
$restart = Read-Host "¿Desea reiniciar el equipo ahora? (S/N)"
if ($restart -eq "S" -or $restart -eq "s") {
    Write-Host "Reiniciando en 5 segundos..." -ForegroundColor Yellow
    Start-Sleep -Seconds 5
    Restart-Computer -Force
}
