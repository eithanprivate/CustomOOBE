using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace CustomOOBE
{
    public static class ThemeManager
    {
        public static bool IsDarkMode { get; private set; }

        public static void ApplyTheme(bool isDarkMode)
        {
            IsDarkMode = isDarkMode;
            var app = Application.Current;

            if (app?.Resources == null) return;

            string prefix = isDarkMode ? "Dark" : "Light";

            app.Resources["PrimaryBackgroundBrush"] = app.Resources[$"{prefix}PrimaryBackgroundBrush"];
            app.Resources["SecondaryBackgroundBrush"] = app.Resources[$"{prefix}SecondaryBackgroundBrush"];
            app.Resources["AccentBrush"] = app.Resources[$"{prefix}AccentBrush"];
            app.Resources["TextPrimaryBrush"] = app.Resources[$"{prefix}TextPrimaryBrush"];
            app.Resources["TextSecondaryBrush"] = app.Resources[$"{prefix}TextSecondaryBrush"];
            app.Resources["BorderBrush"] = app.Resources[$"{prefix}BorderBrush"];

            // Aplicar tema de Windows
            ApplyWindowsTheme(isDarkMode);
        }

        public static void ChangeThemeWithAnimation(bool isDarkMode, FrameworkElement element)
        {
            var fadeOut = new DoubleAnimation(1, 0, TimeSpan.FromMilliseconds(300));
            fadeOut.Completed += (s, e) =>
            {
                ApplyTheme(isDarkMode);
                var fadeIn = new DoubleAnimation(0, 1, TimeSpan.FromMilliseconds(300));
                element.BeginAnimation(UIElement.OpacityProperty, fadeIn);
            };
            element.BeginAnimation(UIElement.OpacityProperty, fadeOut);
        }

        private static void ApplyWindowsTheme(bool isDarkMode)
        {
            try
            {
                using var key = Microsoft.Win32.Registry.CurrentUser.OpenSubKey(
                    @"Software\Microsoft\Windows\CurrentVersion\Themes\Personalize", true);

                if (key != null)
                {
                    key.SetValue("AppsUseLightTheme", isDarkMode ? 0 : 1, Microsoft.Win32.RegistryValueKind.DWord);
                    key.SetValue("SystemUsesLightTheme", isDarkMode ? 0 : 1, Microsoft.Win32.RegistryValueKind.DWord);
                }
            }
            catch { }
        }
    }
}
