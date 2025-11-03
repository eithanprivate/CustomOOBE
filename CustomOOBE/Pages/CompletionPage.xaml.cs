using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace CustomOOBE.Pages
{
    public partial class CompletionPage : UserControl
    {
        public event EventHandler? Completed;

        public CompletionPage()
        {
            InitializeComponent();
            Loaded += CompletionPage_Loaded;
        }

        private async void CompletionPage_Loaded(object sender, RoutedEventArgs e)
        {
            await AnimateCompletion();
        }

        private async Task AnimateCompletion()
        {
            // Animar primer texto
            var fadeIn1 = new DoubleAnimation(0, 1, TimeSpan.FromSeconds(1.5))
            {
                EasingFunction = new CubicEase { EasingMode = EasingMode.EaseOut }
            };
            var slideUp1 = new DoubleAnimation(30, 0, TimeSpan.FromSeconds(1.5))
            {
                EasingFunction = new CubicEase { EasingMode = EasingMode.EaseOut }
            };

            ThankYouText.BeginAnimation(OpacityProperty, fadeIn1);
            ((TranslateTransform)ThankYouText.RenderTransform).BeginAnimation(TranslateTransform.YProperty, slideUp1);

            await Task.Delay(1500);

            // Animar segundo texto
            var fadeIn2 = new DoubleAnimation(0, 1, TimeSpan.FromSeconds(1.5))
            {
                EasingFunction = new CubicEase { EasingMode = EasingMode.EaseOut }
            };
            var slideUp2 = new DoubleAnimation(30, 0, TimeSpan.FromSeconds(1.5))
            {
                EasingFunction = new CubicEase { EasingMode = EasingMode.EaseOut }
            };

            EnjoyText.BeginAnimation(OpacityProperty, fadeIn2);
            ((TranslateTransform)EnjoyText.RenderTransform).BeginAnimation(TranslateTransform.YProperty, slideUp2);

            await Task.Delay(3000);

            // Transición al escritorio
            await TransitionToDesktop();
        }

        private async Task TransitionToDesktop()
        {
            // Fade out
            var fadeOut = new DoubleAnimation(1, 0, TimeSpan.FromSeconds(1));
            this.BeginAnimation(OpacityProperty, fadeOut);

            await Task.Delay(1000);

            // Marcar como completado para que la ventana se pueda cerrar
            Completed?.Invoke(this, EventArgs.Empty);

            // Esperar un poco antes de mostrar el escritorio
            await Task.Delay(500);

            // Finalizar el proceso OOBE
            FinalizeOOBE();
        }

        private void FinalizeOOBE()
        {
            try
            {
                // Eliminar el script de auto-inicio
                RemoveStartupScript();

                // Reiniciar Explorer para mostrar el escritorio
                RestartExplorer();

                // Cerrar la aplicación
                Application.Current.Dispatcher.Invoke(() =>
                {
                    Application.Current.Shutdown();
                });
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error finalizing OOBE: {ex.Message}");
            }
        }

        private void RemoveStartupScript()
        {
            try
            {
                // Eliminar de la carpeta de inicio
                string startupFolder = Environment.GetFolderPath(Environment.SpecialFolder.Startup);
                string shortcutPath = System.IO.Path.Combine(startupFolder, "CustomOOBE.lnk");

                if (System.IO.File.Exists(shortcutPath))
                {
                    System.IO.File.Delete(shortcutPath);
                }

                // Eliminar del registro
                using var key = Microsoft.Win32.Registry.LocalMachine.OpenSubKey(
                    @"SOFTWARE\Microsoft\Windows\CurrentVersion\Run", true);

                if (key?.GetValue("CustomOOBE") != null)
                {
                    key.DeleteValue("CustomOOBE");
                }

                // Marcar como completado en el registro
                using var configKey = Microsoft.Win32.Registry.LocalMachine.CreateSubKey(
                    @"SOFTWARE\CustomOOBE");
                configKey?.SetValue("Completed", 1);
                configKey?.SetValue("CompletionDate", DateTime.Now.ToString("o"));
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error removing startup: {ex.Message}");
            }
        }

        private void RestartExplorer()
        {
            try
            {
                // Matar el proceso de Explorer
                var explorerProcesses = Process.GetProcessesByName("explorer");
                foreach (var process in explorerProcesses)
                {
                    process.Kill();
                    process.WaitForExit();
                }

                // Esperar un momento
                System.Threading.Thread.Sleep(1000);

                // Reiniciar Explorer
                Process.Start("explorer.exe");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error restarting Explorer: {ex.Message}");
            }
        }
    }
}
