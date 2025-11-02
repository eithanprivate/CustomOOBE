# Guía de Configuración - Custom OOBE

## Configuración Básica

### 1. Nombre del Equipo

El sistema detecta automáticamente el nombre del equipo desde el registro de Windows.

**Para personalizar manualmente:**

Editar en `PageNavigator.cs` línea 16:
```csharp
private string deviceName = "Tu Equipo Personalizado";
```

### 2. Ajustar Horario de Temas

Por defecto:
- Tema oscuro: 18:00 - 06:00
- Tema claro: 06:00 - 18:00

**Para cambiar:**

Editar en `App.xaml.cs`:
```csharp
// Cambiar hora de inicio del tema oscuro (ejemplo: 17:00)
var isDarkMode = currentHour >= 17 || currentHour < 7;
```

### 3. Personalizar Colores

#### Tema Claro (`Themes/LightTheme.xaml`)
```xml
<Color x:Key="LightPrimaryBackground">#FFFFFF</Color>
<Color x:Key="LightAccent">#0078D4</Color>  <!-- Azul Windows -->
```

#### Tema Oscuro (`Themes/DarkTheme.xaml`)
```xml
<Color x:Key="DarkPrimaryBackground">#1E1E1E</Color>
<Color x:Key="DarkAccent">#0078D4</Color>
```

### 4. Textos de Bienvenida

#### Mensaje de Agradecimiento
Editar en `Pages/WelcomePage.xaml`:
```xml
<TextBlock Text="¡Gracias por tu compra!"
```

#### Mensaje de Disfrute
Editar en `Pages/EnjoyPage.xaml`:
```xml
<TextBlock Text="Disfruta de tu"
```

#### Mensaje de Configuración
Editar en `Pages/SetupIntroPage.xaml`:
```xml
<Run Text="Pero primero, necesitas"/>
<Run Text="configurar unas cosas"/>
```

## Configuración de Programas

### Agregar Nuevo Programa

Editar `Pages/ProgramsPage.xaml.cs`, agregar en la lista de programas:

```csharp
new ProgramInfo
{
    Name = "Nombre del Programa",
    Description = "Descripción breve del programa",
    Category = "Categoría (Navegadores, Utilidades, etc.)",
    DownloadUrl = "https://url-descarga-directa.com/installer.exe",
    IsSelected = false  // true para selección por defecto
}
```

### Eliminar Programa

Simplemente elimina o comenta la entrada del programa en la lista.

### Argumentos de Instalación Silenciosa

Los argumentos por defecto son:
```csharp
Arguments = "/S /silent /verysilent /quiet /norestart"
```

Para personalizar por programa, modificar el método `InstallProgram()`.

## Configuración de Red

### Timeout de Conexión

Editar en `Pages/NetworkSetupPage.xaml.cs`:
```csharp
// Esperar 3 segundos después de conectar
await Task.Delay(3000);
```

### Intervalo de Monitoreo

```csharp
// Verificar conexión cada 2 segundos
connectionCheckTimer = new System.Timers.Timer(2000);
```

## Configuración de Reseñas

### API Endpoint

Editar en `Pages/ReviewPage.xaml.cs`:

```csharp
private async Task SendReviewToServer(Review review)
{
    var httpClient = new System.Net.Http.HttpClient();
    var json = System.Text.Json.JsonSerializer.Serialize(review);
    var content = new System.Net.Http.StringContent(json,
        System.Text.Encoding.UTF8, "application/json");

    // Cambiar por tu URL de API
    await httpClient.PostAsync("https://tu-api.com/api/reviews", content);
}
```

### Estructura de JSON enviado

```json
{
  "Id": 0,
  "Rating": 5,
  "Comment": "Excelente experiencia",
  "Timestamp": "2024-01-15T10:30:00",
  "MachineName": "DESKTOP-ABC123",
  "UserName": "Usuario"
}
```

### Base de Datos Local

Ubicación: `%ProgramData%\CustomOOBE\reviews.db`

Para cambiar la ubicación, editar en `ReviewDatabase.cs`:
```csharp
private static string GetDatabasePath()
{
    string appData = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData);
    string dbFolder = Path.Combine(appData, "MiEmpresa", "Reviews");
    // ...
}
```

## Configuración de Animaciones

### Velocidad de Transiciones

Editar en `App.xaml`:
```xml
<!-- Más lento: cambiar 0.8 a 1.5 -->
<DoubleAnimation Duration="0:0:0.8"/>

<!-- Más rápido: cambiar 0.8 a 0.4 -->
<DoubleAnimation Duration="0:0:0.4"/>
```

### Animaciones de Fondo

Editar en `AnimationController.cs`:
```csharp
// Cambiar número de formas animadas
for (int i = 0; i < 30; i++)  // Por defecto 20
{
    CreateAnimatedCircle();
}

// Cambiar velocidad de animación
Duration = TimeSpan.FromSeconds(random.Next(5, 15))  // Por defecto 10-20
```

## Configuración de Seguridad

### Bloqueo de Teclas

Para desactivar bloqueo de una tecla específica, editar `KeyboardHook.cs`:

```csharp
// Comentar o eliminar las líneas correspondientes:

// Permitir Windows Key
// if (vkCode == 0x5B || vkCode == 0x5C)
//     return (IntPtr)1;

// Permitir Alt+Tab
// if (altPressed && vkCode == 0x09)
//     return (IntPtr)1;
```

### Agregar Nueva Tecla Bloqueada

```csharp
// Bloquear F1 (Help)
if (vkCode == 0x70)  // F1 virtual key code
    return (IntPtr)1;
```

Códigos de teclas virtuales: https://docs.microsoft.com/en-us/windows/win32/inputdev/virtual-key-codes

## Configuración de Usuario

### Validación de Nombre de Usuario

Editar en `Pages/UserSetupPage.xaml.cs`:

```csharp
// Permitir espacios en nombres de usuario
if (!System.Text.RegularExpressions.Regex.IsMatch(username, @"^[a-zA-Z0-9 _-]+$"))
{
    // Error
}

// Longitud mínima/máxima
if (username.Length < 3 || username.Length > 20)
{
    // Error
}
```

### Avatares Predeterminados

El sistema busca imágenes en `Assets/Avatars/`.

Para cambiar el avatar por defecto:
```xml
<!-- En UserSetupPage.xaml -->
<BitmapImage UriSource="/Assets/Avatars/default.png"/>
```

## Personalización de Pantallas

### Tiempo de Visualización

Editar el tiempo que se muestra cada pantalla de bienvenida:

```csharp
// En WelcomePage.xaml.cs, EnjoyPage.xaml.cs, etc.
await Task.Delay(3000);  // 3 segundos (por defecto)
```

### Pantalla Final

Editar el mensaje final en `Pages/CompletionPage.xaml.cs`:

```xml
<TextBlock Text="¡Gracias por elegir nuestros equipos!"
           FontSize="64"/>

<TextBlock Text="Esperamos que disfrutes tu nueva computadora"
           FontSize="48"/>
```

## Configuración de Instalación

### Ruta de Instalación

Editar en `Scripts/SetupOOBE.ps1`:
```powershell
param(
    [string]$InstallPath = "C:\TuEmpresa\OOBE"  # Por defecto C:\CustomOOBE
)
```

### Tareas Programadas

Nombre de la tarea:
```powershell
$taskName = "CustomOOBE"  # Cambiar a "TuEmpresa_OOBE"
$taskPath = "\Microsoft\Windows\Setup"  # Mantener esta ruta
```

## Configuración Regional

### Idioma

Para agregar soporte multiidioma:

1. Crear archivos de recursos (.resx)
2. Usar `System.Globalization.CultureInfo`
3. Implementar cambio dinámico de idioma

Ejemplo básico:
```csharp
// En App.xaml.cs
System.Threading.Thread.CurrentThread.CurrentUICulture =
    new System.Globalization.CultureInfo("es-ES");
```

### Formato de Fecha

```csharp
// En CompletionPage.xaml.cs
configKey?.SetValue("CompletionDate",
    DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss"));
```

## Logging y Diagnóstico

### Habilitar Logs

Crear clase `Logger.cs`:

```csharp
public static class Logger
{
    private static string logPath = Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData),
        "CustomOOBE", "logs.txt");

    public static void Log(string message)
    {
        File.AppendAllText(logPath,
            $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] {message}\n");
    }
}
```

Usar en el código:
```csharp
Logger.Log("Usuario creado exitosamente");
```

## Variables de Entorno

Puedes leer información del sistema:

```csharp
// Nombre del equipo
string computerName = Environment.MachineName;

// Usuario actual
string userName = Environment.UserName;

// Versión de Windows
string osVersion = Environment.OSVersion.ToString();

// Arquitectura
string architecture = Environment.Is64BitOperatingSystem ? "64-bit" : "32-bit";
```

## Configuración de Rendimiento

### Optimizar Inicio

```xml
<!-- En App.xaml.cs -->
protected override void OnStartup(StartupEventArgs e)
{
    // Prioridad alta del proceso
    Process.GetCurrentProcess().PriorityClass = ProcessPriorityClass.High;

    base.OnStartup(e);
}
```

### Liberar Memoria

```csharp
// Después de completar operaciones pesadas
GC.Collect();
GC.WaitForPendingFinalizers();
GC.Collect();
```

## Marca Personal

### Logo de Empresa

Agregar logo en todas las páginas:

```xml
<!-- En cada página -->
<Image Source="/Assets/logo.png"
       Width="150"
       HorizontalAlignment="Right"
       VerticalAlignment="Top"
       Margin="20"/>
```

### Información de Contacto

Agregar footer con información:

```xml
<TextBlock Text="Soporte: soporte@tuempresa.com | Tel: 123-456-7890"
           FontSize="10"
           Foreground="{DynamicResource TextSecondaryBrush}"
           HorizontalAlignment="Center"
           VerticalAlignment="Bottom"
           Margin="0,0,0,10"/>
```

## Testing

### Modo de Prueba

Agregar flag de depuración:

```csharp
public static class Config
{
    public static bool IsTestMode = true;  // Cambiar a false en producción
}

// Usar en KeyboardHook:
if (Config.IsTestMode)
    return CallNextHookEx(hookID, nCode, wParam, lParam);  // No bloquear
```

### Datos de Prueba

```csharp
if (Config.IsTestMode)
{
    UsernameTextBox.Text = "TestUser";
    // Auto-seleccionar primer avatar
    // Auto-conectar a red de prueba
}
```

## Recursos Adicionales

- Códigos de color: https://htmlcolorcodes.com
- Iconos: https://www.flaticon.com
- Fuentes: https://fonts.google.com
- Imágenes de stock: https://unsplash.com

## Checklist de Configuración Pre-Deploy

- [ ] Cambiar textos de bienvenida personalizados
- [ ] Agregar logo de empresa
- [ ] Configurar colores de marca
- [ ] Actualizar lista de programas con URLs válidas
- [ ] Agregar avatares personalizados
- [ ] Agregar fondos de pantalla
- [ ] Configurar API endpoint de reseñas
- [ ] Establecer información de contacto
- [ ] Deshabilitar modo de prueba
- [ ] Firmar ejecutable digitalmente
- [ ] Probar en máquina virtual limpia
- [ ] Verificar todos los assets incluidos
- [ ] Actualizar README con información específica
