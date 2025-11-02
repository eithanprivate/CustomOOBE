# Script de desinstalación del Custom OOBE

Write-Host "========================================" -ForegroundColor Cyan
Write-Host " Custom OOBE - Script de Desinstalación " -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""

# Verificar privilegios de administrador
$isAdmin = ([Security.Principal.WindowsPrincipal] [Security.Principal.WindowsIdentity]::GetCurrent()).IsInRole([Security.Principal.WindowsBuiltInRole]::Administrator)

if (-not $isAdmin) {
    Write-Host "ERROR: Este script requiere privilegios de administrador." -ForegroundColor Red
    exit 1
}

Write-Host "[1/5] Eliminando tarea programada..." -ForegroundColor Green
Unregister-ScheduledTask -TaskName "CustomOOBE" -TaskPath "\Microsoft\Windows\Setup" -Confirm:$false -ErrorAction SilentlyContinue

Write-Host "[2/5] Eliminando entrada del registro Run..." -ForegroundColor Green
$runPath = "HKLM:\SOFTWARE\Microsoft\Windows\CurrentVersion\Run"
Remove-ItemProperty -Path $runPath -Name "CustomOOBE" -ErrorAction SilentlyContinue

Write-Host "[3/5] Restaurando configuración de Shell..." -ForegroundColor Green
$regPath = "HKLM:\SOFTWARE\CustomOOBE"
if (Test-Path $regPath) {
    $originalShell = Get-ItemProperty -Path $regPath -Name "OriginalShell" -ErrorAction SilentlyContinue
    if ($originalShell) {
        $shellPath = "HKLM:\SOFTWARE\Microsoft\Windows NT\CurrentVersion\Winlogon"
        Set-ItemProperty -Path $shellPath -Name "Shell" -Value $originalShell.OriginalShell
    }
}

Write-Host "[4/5] Eliminando archivos..." -ForegroundColor Green
$installPath = "C:\CustomOOBE"
if (Test-Path $installPath) {
    Remove-Item -Path $installPath -Recurse -Force -ErrorAction SilentlyContinue
}

Write-Host "[5/5] Limpiando registro..." -ForegroundColor Green
if (Test-Path $regPath) {
    Remove-Item -Path $regPath -Recurse -Force -ErrorAction SilentlyContinue
}

Write-Host ""
Write-Host "========================================" -ForegroundColor Green
Write-Host " Desinstalación completada exitosamente " -ForegroundColor Green
Write-Host "========================================" -ForegroundColor Green
Write-Host ""
