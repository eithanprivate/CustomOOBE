using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media.Animation;

namespace CustomOOBE
{
    public partial class MainWindow : Window
    {
        private KeyboardHook keyboardHook;
        private AnimationController animationController;
        private PageNavigator pageNavigator;

        public MainWindow()
        {
            InitializeComponent();

            Loaded += MainWindow_Loaded;
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            // Inicializar el hook de teclado para bloquear teclas
            keyboardHook = new KeyboardHook();
            keyboardHook.InstallHook();

            // Inicializar el controlador de animaciones
            animationController = new AnimationController(AnimationCanvas);
            animationController.StartBackgroundAnimation();

            // Inicializar el navegador de páginas
            pageNavigator = new PageNavigator(ContentArea, animationController);
            pageNavigator.NavigateToWelcome();

            // Animación de entrada
            this.Opacity = 0;
            var fadeIn = new DoubleAnimation(0, 1, TimeSpan.FromSeconds(0.8));
            this.BeginAnimation(OpacityProperty, fadeIn);
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            // Bloquear teclas específicas
            if ((Keyboard.Modifiers & ModifierKeys.Alt) == ModifierKeys.Alt && e.Key == Key.F4)
            {
                e.Handled = true;
            }
        }

        protected override void OnClosing(System.ComponentModel.CancelEventArgs e)
        {
            // No permitir cerrar la ventana hasta completar el proceso
            if (!pageNavigator?.IsCompleted ?? true)
            {
                e.Cancel = true;
            }
            else
            {
                keyboardHook?.UninstallHook();
                base.OnClosing(e);
            }
        }
    }
}
