@echo off
REM Script para probar el OOBE sin instalarlo en el sistema

echo ========================================
echo   Custom OOBE - Modo de Prueba
echo ========================================
echo.

cd /d "%~dp0.."

echo Ejecutando Custom OOBE...
echo.

start "" "CustomOOBE.exe"

echo.
echo El OOBE se esta ejecutando en modo de prueba.
echo Puedes cerrarlo con Alt+F4 (aunque este bloqueado normalmente).
echo.
pause
