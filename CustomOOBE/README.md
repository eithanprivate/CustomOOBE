# Custom OOBE (Out-of-Box Experience)

Sistema OOBE personalizado para Windows con interfaz moderna, animaciones fluidas inspiradas en macOS, y configuración completa del sistema.

## Características

### Interfaz y Diseño
- **Diseño dividido** al estilo macOS con animaciones en un lado y contenido en el otro
- **Temas claro y oscuro** que se seleccionan automáticamente según la hora del día:
  - Tema oscuro: 6:00 PM - 6:00 AM
  - Tema claro: 6:00 AM - 6:00 PM
- **Animaciones fluidas** con transiciones suaves entre pantallas
- **Interfaz moderna** con controles personalizados

### Pantallas y Funcionalidades

1. **Pantallas de Bienvenida**
   - "¡Gracias por tu compra!"
   - "Disfruta de tu [Nombre del Equipo]"
   - "Pero primero, necesitas configurar unas cosas"

2. **Configuración de Usuario**
   - Campo para nombre de usuario de Windows
   - Selector de avatar con imágenes prediseñadas
   - Opción para tomar foto con la cámara web
   - Creación automática de usuario de Windows
   - Asignación de foto de perfil

3. **Configuración de Red**
   - Detección automática de conexión Ethernet
   - Lista de redes WiFi disponibles con intensidad de señal
   - Conexión WiFi con contraseña
   - Opción para saltar (funciones que requieren internet se omitirán)

4. **Instalación de Programas** (requiere internet)
   - Selección de programas útiles por categorías:
     - Navegadores (Chrome, Firefox)
     - Utilidades (7-Zip)
     - Multimedia (VLC, Spotify)
     - Limpieza (CCleaner)
     - Seguridad (Malwarebytes)
     - Productividad (LibreOffice)
     - Comunicación (Discord)
   - Descarga e instalación automática en segundo plano
   - Barra de progreso por programa y total

5. **Personalización**
   - Selección de tema (claro/oscuro) con cambio animado
   - Aplicación del tema en Windows
   - Selección de fondo de pantalla (6 opciones prediseñadas)
   - Selección de pantalla de bloqueo (6 opciones prediseñadas)
   - Aplicación automática de configuraciones

6. **Sistema de Reseñas** (requiere internet)
   - Calificación con estrellas (1-5)
   - Campo de comentario opcional
   - Guardado local en base de datos SQLite
   - Envío a servidor remoto (configurable)

7. **Pantalla Final**
   - "Muchas gracias por configurar tu equipo"
   - "A continuación, disfrute"
   - Transición suave al escritorio de Windows

### Seguridad y Control
- **Bloqueo de teclas** durante el proceso:
  - Ctrl+Alt+Del
  - Alt+F4
  - Ctrl+Shift+Esc (Administrador de tareas)
  - Ctrl+Tab
  - Tecla Windows
  - Alt+Tab
- **Pantalla completa obligatoria** hasta completar la configuración
- **No se puede cerrar** hasta finalizar el proceso

## Instalación

### Requisitos
- Windows 10/11 (64-bit)
- .NET 8.0 Runtime
- Privilegios de administrador

### Pasos de Instalación

1. **Compilar el proyecto:**
   ```bash
   dotnet build -c Release
   ```

2. **Ejecutar el script de instalación como administrador:**
   ```powershell
   cd Scripts
   .\SetupOOBE.ps1
   ```

3. **El script realizará:**
   - Copia de archivos a `C:\CustomOOBE`
   - Configuración del registro de Windows
   - Creación de tarea programada para ejecutar al inicio
   - Configuración del shell personalizado
   - Desactivación del OOBE estándar de Windows

4. **Reiniciar el equipo**
   - El OOBE se ejecutará automáticamente antes del inicio de sesión

## Modo de Prueba

Para probar el OOBE sin instalarlo en el sistema:

```bash
cd Scripts
TestOOBE.bat
```

## Desinstalación

Para eliminar el Custom OOBE del sistema:

```powershell
cd Scripts
.\Uninstall-CustomOOBE.ps1
```

## Configuración Avanzada

### Personalizar Nombre del Equipo
El sistema detecta automáticamente el nombre del equipo desde el registro. Para personalizarlo:

```powershell
# En el registro: HKLM\SYSTEM\CurrentControlSet\Control\SystemInformation
# Modificar: SystemProductName y SystemManufacturer
```

### Agregar Avatares Personalizados
Colocar imágenes PNG en: `Assets/Avatars/`

### Agregar Fondos de Pantalla
Colocar imágenes JPG/PNG en: `Assets/Wallpapers/`

### Agregar Fondos de Bloqueo
Colocar imágenes JPG/PNG en: `Assets/LockScreens/`

### Configurar API de Reseñas
Editar en `ReviewPage.xaml.cs`:

```csharp
private async Task SendReviewToServer(Review review)
{
    var httpClient = new System.Net.Http.HttpClient();
    var json = System.Text.Json.JsonSerializer.Serialize(review);
    var content = new System.Net.Http.StringContent(json,
        System.Text.Encoding.UTF8, "application/json");

    // Cambiar URL por tu API
    await httpClient.PostAsync("https://tuservidor.com/api/reviews", content);
}
```

### Modificar Programas Disponibles
Editar la lista en `ProgramsPage.xaml.cs`:

```csharp
programs = new ObservableCollection<ProgramInfo>
{
    new ProgramInfo
    {
        Name = "Nombre del Programa",
        Description = "Descripción",
        Category = "Categoría",
        DownloadUrl = "URL de descarga directa",
        IsSelected = false
    },
    // ... más programas
};
```

## Estructura del Proyecto

```
CustomOOBE/
├── App.xaml                    # Configuración de la aplicación
├── App.xaml.cs                 # Lógica de inicio
├── MainWindow.xaml             # Ventana principal
├── MainWindow.xaml.cs          # Lógica de ventana principal
├── ThemeManager.cs             # Gestor de temas
├── KeyboardHook.cs             # Bloqueo de teclas
├── AnimationController.cs      # Control de animaciones
├── PageNavigator.cs            # Navegación entre páginas
├── ReviewDatabase.cs           # Base de datos de reseñas
├── Pages/
│   ├── WelcomePage.xaml        # Pantalla de bienvenida
│   ├── EnjoyPage.xaml          # Pantalla "Disfruta"
│   ├── SetupIntroPage.xaml     # Introducción a configuración
│   ├── UserSetupPage.xaml      # Configuración de usuario
│   ├── NetworkSetupPage.xaml   # Configuración de red
│   ├── ProgramsPage.xaml       # Instalación de programas
│   ├── PersonalizationPage.xaml # Personalización
│   ├── ReviewPage.xaml         # Sistema de reseñas
│   └── CompletionPage.xaml     # Pantalla final
├── Themes/
│   ├── LightTheme.xaml         # Tema claro
│   └── DarkTheme.xaml          # Tema oscuro
├── Styles/
│   └── ButtonStyles.xaml       # Estilos de botones
├── Scripts/
│   ├── SetupOOBE.ps1          # Script de instalación
│   ├── Uninstall-CustomOOBE.ps1 # Script de desinstalación
│   └── TestOOBE.bat           # Script de prueba
└── Assets/
    ├── Avatars/               # Imágenes de avatar
    ├── Wallpapers/            # Fondos de pantalla
    └── LockScreens/           # Pantallas de bloqueo
```

## Base de Datos de Reseñas

Las reseñas se guardan en: `%ProgramData%\CustomOOBE\reviews.db`

Estructura de la tabla:
```sql
CREATE TABLE Reviews (
    Id INTEGER PRIMARY KEY AUTOINCREMENT,
    Rating INTEGER NOT NULL,
    Comment TEXT,
    Timestamp TEXT NOT NULL,
    MachineName TEXT,
    UserName TEXT
)
```

## Registro de Windows

El sistema utiliza las siguientes claves de registro:

```
HKLM\SOFTWARE\CustomOOBE
├── InstallPath (REG_SZ)          # Ruta de instalación
├── InstallDate (REG_SZ)          # Fecha de instalación
├── Completed (REG_DWORD)         # Estado de completado (0/1)
├── CompletionDate (REG_SZ)       # Fecha de finalización
└── OriginalShell (REG_SZ)        # Shell original guardado
```

## Solución de Problemas

### El OOBE no se ejecuta al iniciar
1. Verificar que la tarea programada existe:
   ```powershell
   Get-ScheduledTask -TaskName "CustomOOBE"
   ```

2. Verificar entrada en el registro:
   ```powershell
   Get-ItemProperty "HKLM:\SOFTWARE\Microsoft\Windows\CurrentVersion\Run" -Name "CustomOOBE"
   ```

### No se pueden instalar programas
1. Verificar conexión a internet
2. Ejecutar como administrador
3. Verificar que Windows Defender no esté bloqueando las descargas

### La cámara no funciona
1. Verificar que hay una cámara web conectada
2. Dar permisos de cámara a la aplicación
3. Verificar que ninguna otra aplicación está usando la cámara

### Error al crear usuario
1. Verificar que la aplicación se ejecuta como administrador
2. Verificar que el nombre de usuario es válido (solo letras, números, guiones)

## Licencia

Este proyecto es un sistema personalizado de OOBE para uso privado o comercial en equipos que vendes.

## Soporte

Para reportar problemas o sugerencias, contactar al desarrollador.

## Notas Importantes

- **Backup**: Siempre hacer un backup del sistema antes de instalar
- **Pruebas**: Probar en una máquina virtual antes de usar en producción
- **Personalización**: Este sistema está diseñado para ser personalizado según tus necesidades
- **Actualizaciones**: Mantener los enlaces de descarga de programas actualizados
- **Seguridad**: Las contraseñas WiFi se manejan de forma segura pero temporal

## Créditos

Desarrollado para proporcionar una experiencia de primera configuración profesional y moderna para equipos Windows.
