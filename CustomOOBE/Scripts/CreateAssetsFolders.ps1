# Script para crear la estructura de carpetas de Assets

Write-Host "Creando estructura de carpetas Assets..." -ForegroundColor Cyan

$projectRoot = Split-Path -Parent $PSScriptRoot
$assetsPath = Join-Path $projectRoot "Assets"

# Crear carpetas principales
$folders = @(
    "Assets",
    "Assets\Avatars",
    "Assets\Wallpapers",
    "Assets\LockScreens"
)

foreach ($folder in $folders) {
    $fullPath = Join-Path $projectRoot $folder
    if (-not (Test-Path $fullPath)) {
        New-Item -ItemType Directory -Path $fullPath -Force | Out-Null
        Write-Host "✓ Creada: $folder" -ForegroundColor Green
    } else {
        Write-Host "○ Ya existe: $folder" -ForegroundColor Yellow
    }
}

# Crear archivo README en Assets
$readmePath = Join-Path $assetsPath "README.txt"
$readmeContent = @"
CARPETA DE ASSETS - Custom OOBE
================================

Esta carpeta contiene los recursos visuales para el Custom OOBE.

AVATARES (Assets/Avatars/)
--------------------------
Coloca aquí imágenes PNG de 512x512 píxeles para usar como avatares de usuario.
- Formato: PNG
- Resolución recomendada: 512x512 px
- Nombres sugeridos: avatar_1.png, avatar_2.png, etc.
- Mínimo recomendado: 6 imágenes

Si no hay imágenes, el programa creará avatares por defecto.

FONDOS DE PANTALLA (Assets/Wallpapers/)
---------------------------------------
Coloca aquí imágenes para fondos de escritorio.
- Formato: JPG o PNG
- Resolución recomendada: 1920x1080 px o superior
- Nombres sugeridos: wallpaper_1.jpg, wallpaper_2.jpg, etc.
- Mínimo recomendado: 6 imágenes

Si no hay imágenes, el programa creará fondos por defecto.

PANTALLAS DE BLOQUEO (Assets/LockScreens/)
------------------------------------------
Coloca aquí imágenes para pantallas de bloqueo.
- Formato: JPG o PNG
- Resolución recomendada: 1920x1080 px
- Nombres sugeridos: lockscreen_1.jpg, lockscreen_2.jpg, etc.
- Mínimo recomendado: 6 imágenes

Si no hay imágenes, el programa creará pantallas por defecto.

NOTAS IMPORTANTES
-----------------
- Las imágenes deben tener nombres de archivo válidos sin caracteres especiales
- Se recomienda usar imágenes de alta calidad para mejor experiencia
- El programa redimensionará automáticamente las imágenes según sea necesario
- Los archivos PNG soportan transparencia para avatares

Para más información, consulta el archivo README.md del proyecto.
"@

$readmeContent | Out-File -FilePath $readmePath -Encoding UTF8 -Force
Write-Host "✓ Creado: Assets\README.txt" -ForegroundColor Green

Write-Host ""
Write-Host "Estructura de Assets creada exitosamente!" -ForegroundColor Green
Write-Host ""
Write-Host "Próximos pasos:" -ForegroundColor Cyan
Write-Host "1. Agrega imágenes de avatares en Assets\Avatars\" -ForegroundColor White
Write-Host "2. Agrega fondos de pantalla en Assets\Wallpapers\" -ForegroundColor White
Write-Host "3. Agrega imágenes de bloqueo en Assets\LockScreens\" -ForegroundColor White
Write-Host ""
Write-Host "El programa creará recursos por defecto si no encuentras imágenes." -ForegroundColor Yellow
Write-Host ""
