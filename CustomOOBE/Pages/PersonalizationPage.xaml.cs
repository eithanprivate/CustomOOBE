using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Microsoft.Win32;

namespace CustomOOBE.Pages
{
    public partial class PersonalizationPage : UserControl
    {
        public event EventHandler? Completed;

        private bool isDarkTheme = false;
        private string? selectedWallpaper;
        private string? selectedLockScreen;

        public PersonalizationPage()
        {
            InitializeComponent();

            // Marcar el tema actual
            var currentHour = DateTime.Now.Hour;
            isDarkTheme = currentHour >= 18 || currentHour < 6;

            UpdateThemeSelection();
            LoadWallpapers();
            LoadLockScreens();
        }

        private void UpdateThemeSelection()
        {
            if (isDarkTheme)
            {
                DarkThemeOption.BorderBrush = new SolidColorBrush(Color.FromRgb(0, 120, 212));
                DarkSelectedMark.Visibility = Visibility.Visible;
                LightThemeOption.BorderBrush = new SolidColorBrush(Color.FromRgb(224, 224, 224));
                LightSelectedMark.Visibility = Visibility.Collapsed;
            }
            else
            {
                LightThemeOption.BorderBrush = new SolidColorBrush(Color.FromRgb(0, 120, 212));
                LightSelectedMark.Visibility = Visibility.Visible;
                DarkThemeOption.BorderBrush = new SolidColorBrush(Color.FromRgb(63, 63, 63));
                DarkSelectedMark.Visibility = Visibility.Collapsed;
            }
        }

        private void LightTheme_Click(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (!isDarkTheme) return;

            isDarkTheme = false;
            ThemeManager.ChangeThemeWithAnimation(false, this);
            UpdateThemeSelection();
        }

        private void DarkTheme_Click(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (isDarkTheme) return;

            isDarkTheme = true;
            ThemeManager.ChangeThemeWithAnimation(true, this);
            UpdateThemeSelection();
        }

        private void LoadWallpapers()
        {
            string wallpapersPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Assets", "Wallpapers");

            if (!Directory.Exists(wallpapersPath))
            {
                Directory.CreateDirectory(wallpapersPath);
                CreateDefaultWallpapers(wallpapersPath);
            }

            LoadImages(wallpapersPath, WallpaperPanel, (path) => selectedWallpaper = path);
        }

        private void LoadLockScreens()
        {
            string lockScreensPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Assets", "LockScreens");

            if (!Directory.Exists(lockScreensPath))
            {
                Directory.CreateDirectory(lockScreensPath);
                CreateDefaultLockScreens(lockScreensPath);
            }

            LoadImages(lockScreensPath, LockScreenPanel, (path) => selectedLockScreen = path);
        }

        private void LoadImages(string path, WrapPanel panel, Action<string> onSelect)
        {
            var imageFiles = Directory.GetFiles(path, "*.jpg")
                .Concat(Directory.GetFiles(path, "*.png"))
                .ToArray();

            if (imageFiles.Length == 0)
            {
                // Crear imágenes de ejemplo
                CreatePlaceholderImages(path, 6);
                imageFiles = Directory.GetFiles(path, "*.png");
            }

            foreach (var imagePath in imageFiles)
            {
                var border = new Border
                {
                    Width = 200,
                    Height = 120,
                    CornerRadius = new CornerRadius(8),
                    Margin = new Thickness(0, 0, 15, 15),
                    Cursor = System.Windows.Input.Cursors.Hand,
                    BorderThickness = new Thickness(3),
                    BorderBrush = Brushes.Transparent,
                    Tag = imagePath
                };

                try
                {
                    border.Background = new ImageBrush
                    {
                        ImageSource = new BitmapImage(new Uri(imagePath)),
                        Stretch = Stretch.UniformToFill
                    };
                }
                catch
                {
                    border.Background = new SolidColorBrush(Colors.Gray);
                }

                border.MouseLeftButtonDown += (s, e) =>
                {
                    // Deseleccionar otros
                    foreach (Border b in panel.Children)
                    {
                        b.BorderBrush = Brushes.Transparent;
                    }

                    // Seleccionar este
                    border.BorderBrush = new SolidColorBrush(Color.FromRgb(0, 120, 212));
                    onSelect(imagePath);
                };

                panel.Children.Add(border);

                // Seleccionar el primero por defecto
                if (panel.Children.Count == 1)
                {
                    border.BorderBrush = new SolidColorBrush(Color.FromRgb(0, 120, 212));
                    onSelect(imagePath);
                }
            }
        }

        private void CreateDefaultWallpapers(string path)
        {
            // En una implementación real, aquí copiarías imágenes prediseñadas
            CreatePlaceholderImages(path, 6);
        }

        private void CreateDefaultLockScreens(string path)
        {
            CreatePlaceholderImages(path, 6);
        }

        private void CreatePlaceholderImages(string path, int count)
        {
            var colors = new[]
            {
                Color.FromRgb(0, 120, 212),
                Color.FromRgb(232, 17, 35),
                Color.FromRgb(0, 153, 188),
                Color.FromRgb(142, 140, 216),
                Color.FromRgb(0, 183, 195),
                Color.FromRgb(247, 99, 12)
            };

            for (int i = 0; i < count; i++)
            {
                var bitmap = new System.Drawing.Bitmap(1920, 1080);
                using (var g = System.Drawing.Graphics.FromImage(bitmap))
                {
                    var color = colors[i % colors.Length];
                    var brush = new System.Drawing.SolidBrush(
                        System.Drawing.Color.FromArgb(color.A, color.R, color.G, color.B));

                    g.FillRectangle(brush, 0, 0, bitmap.Width, bitmap.Height);

                    // Agregar un gradiente simple
                    using (var gradientBrush = new System.Drawing.Drawing2D.LinearGradientBrush(
                        new System.Drawing.Rectangle(0, 0, bitmap.Width, bitmap.Height),
                        System.Drawing.Color.FromArgb(50, 255, 255, 255),
                        System.Drawing.Color.FromArgb(0, 0, 0, 0),
                        45f))
                    {
                        g.FillRectangle(gradientBrush, 0, 0, bitmap.Width, bitmap.Height);
                    }
                }

                var filePath = Path.Combine(path, $"image_{i + 1}.png");
                bitmap.Save(filePath, System.Drawing.Imaging.ImageFormat.Png);
            }
        }

        private void Continue_Click(object sender, RoutedEventArgs e)
        {
            // Aplicar configuraciones
            ApplyTheme();

            if (selectedWallpaper != null)
                SetWallpaper(selectedWallpaper);

            if (selectedLockScreen != null)
                SetLockScreen(selectedLockScreen);

            Completed?.Invoke(this, EventArgs.Empty);
        }

        private void ApplyTheme()
        {
            ThemeManager.ApplyTheme(isDarkTheme);
        }

        private void SetWallpaper(string imagePath)
        {
            try
            {
                // Método 1: Registry
                using (RegistryKey key = Registry.CurrentUser.OpenSubKey(@"Control Panel\Desktop", true)!)
                {
                    key.SetValue("Wallpaper", imagePath);
                    key.SetValue("WallpaperStyle", "10"); // Fill
                    key.SetValue("TileWallpaper", "0");
                }

                // Método 2: SystemParametersInfo
                const int SPI_SETDESKWALLPAPER = 20;
                const int SPIF_UPDATEINIFILE = 0x01;
                const int SPIF_SENDCHANGE = 0x02;

                SystemParametersInfo(SPI_SETDESKWALLPAPER, 0, imagePath,
                    SPIF_UPDATEINIFILE | SPIF_SENDCHANGE);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error setting wallpaper: {ex.Message}");
            }
        }

        private void SetLockScreen(string imagePath)
        {
            try
            {
                // Copiar imagen a la ubicación de Windows
                string lockScreenPath = Path.Combine(
                    Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                    "Packages",
                    "Microsoft.Windows.ContentDeliveryManager_cw5n1h2txyewy",
                    "LocalState",
                    "Assets");

                if (Directory.Exists(lockScreenPath))
                {
                    string destPath = Path.Combine(lockScreenPath, Path.GetFileName(imagePath));
                    File.Copy(imagePath, destPath, true);
                }

                // Establecer en el registro
                using (RegistryKey key = Registry.CurrentUser.OpenSubKey(
                    @"SOFTWARE\Microsoft\Windows\CurrentVersion\Lock Screen\Creative", true)!)
                {
                    key?.SetValue("LandscapeAssetPath", imagePath);
                    key?.SetValue("PortraitAssetPath", imagePath);
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error setting lock screen: {ex.Message}");
            }
        }

        [System.Runtime.InteropServices.DllImport("user32.dll", CharSet = System.Runtime.InteropServices.CharSet.Auto)]
        private static extern int SystemParametersInfo(int uAction, int uParam, string lpvParam, int fuWinIni);
    }
}
