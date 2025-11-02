using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace CustomOOBE.Pages
{
    public partial class SetupIntroPage : UserControl
    {
        public event EventHandler? Completed;

        public SetupIntroPage()
        {
            InitializeComponent();
            Loaded += SetupIntroPage_Loaded;
        }

        private async void SetupIntroPage_Loaded(object sender, RoutedEventArgs e)
        {
            await AnimateText();
        }

        private async Task AnimateText()
        {
            var fadeIn = new DoubleAnimation(0, 1, TimeSpan.FromSeconds(1))
            {
                EasingFunction = new CubicEase { EasingMode = EasingMode.EaseOut }
            };

            var slideUp = new DoubleAnimation(30, 0, TimeSpan.FromSeconds(1))
            {
                EasingFunction = new CubicEase { EasingMode = EasingMode.EaseOut }
            };

            SetupText.BeginAnimation(OpacityProperty, fadeIn);
            ((TranslateTransform)SetupText.RenderTransform).BeginAnimation(TranslateTransform.YProperty, slideUp);

            await Task.Delay(3000);

            var fadeOut = new DoubleAnimation(1, 0, TimeSpan.FromSeconds(0.8));
            fadeOut.Completed += (s, args) => Completed?.Invoke(this, EventArgs.Empty);

            this.BeginAnimation(OpacityProperty, fadeOut);
        }
    }
}
