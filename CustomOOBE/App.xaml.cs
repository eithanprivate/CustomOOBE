using System.Windows;

namespace CustomOOBE
{
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            // Aplicar tema según la hora
            var currentHour = DateTime.Now.Hour;
            var isDarkMode = currentHour >= 18 || currentHour < 6;

            ThemeManager.ApplyTheme(isDarkMode);
        }
    }
}
