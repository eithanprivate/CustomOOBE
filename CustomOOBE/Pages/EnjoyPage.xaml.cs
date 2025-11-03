using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace CustomOOBE.Pages
{
    public partial class EnjoyPage : UserControl
    {
        public event EventHandler? Completed;

        public EnjoyPage(string deviceName)
        {
            InitializeComponent();
            DeviceNameText.Text = deviceName;
            Loaded += EnjoyPage_Loaded;
        }

        private async void EnjoyPage_Loaded(object sender, RoutedEventArgs e)
        {
            await AnimateText();
        }

        private async Task AnimateText()
        {
            // Animar primer texto
            var fadeIn1 = new DoubleAnimation(0, 1, TimeSpan.FromSeconds(1))
            {
                EasingFunction = new CubicEase { EasingMode = EasingMode.EaseOut }
            };
            var slideUp1 = new DoubleAnimation(30, 0, TimeSpan.FromSeconds(1))
            {
                EasingFunction = new CubicEase { EasingMode = EasingMode.EaseOut }
            };

            EnjoyText.BeginAnimation(OpacityProperty, fadeIn1);
            ((TranslateTransform)EnjoyText.RenderTransform).BeginAnimation(TranslateTransform.YProperty, slideUp1);

            await Task.Delay(800);

            // Animar segundo texto
            var fadeIn2 = new DoubleAnimation(0, 1, TimeSpan.FromSeconds(1))
            {
                EasingFunction = new CubicEase { EasingMode = EasingMode.EaseOut }
            };
            var slideUp2 = new DoubleAnimation(30, 0, TimeSpan.FromSeconds(1))
            {
                EasingFunction = new CubicEase { EasingMode = EasingMode.EaseOut }
            };

            DeviceNameText.BeginAnimation(OpacityProperty, fadeIn2);
            ((TranslateTransform)DeviceNameText.RenderTransform).BeginAnimation(TranslateTransform.YProperty, slideUp2);

            await Task.Delay(3000);

            // Fade out
            var fadeOut = new DoubleAnimation(1, 0, TimeSpan.FromSeconds(0.8));
            fadeOut.Completed += (s, args) => Completed?.Invoke(this, EventArgs.Empty);

            this.BeginAnimation(OpacityProperty, fadeOut);
        }
    }
}
