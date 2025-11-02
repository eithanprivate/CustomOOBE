using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace CustomOOBE.Pages
{
    public partial class ReviewPage : UserControl
    {
        public event EventHandler? Completed;

        private int selectedRating = 0;
        private Button[] starButtons;

        public ReviewPage()
        {
            InitializeComponent();

            starButtons = new[] { Star1, Star2, Star3, Star4, Star5 };
        }

        private void Star_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.Tag is string tagStr)
            {
                selectedRating = int.Parse(tagStr);
                UpdateStars();
                SubmitButton.IsEnabled = true;
            }
        }

        private void UpdateStars()
        {
            for (int i = 0; i < starButtons.Length; i++)
            {
                if (i < selectedRating)
                {
                    starButtons[i].Content = "★";
                    starButtons[i].Foreground = new SolidColorBrush(Color.FromRgb(255, 193, 7)); // Dorado
                }
                else
                {
                    starButtons[i].Content = "☆";
                    starButtons[i].Foreground = (SolidColorBrush)Application.Current.Resources["TextSecondaryBrush"];
                }
            }
        }

        private void Skip_Click(object sender, RoutedEventArgs e)
        {
            Completed?.Invoke(this, EventArgs.Empty);
        }

        private async void Submit_Click(object sender, RoutedEventArgs e)
        {
            if (selectedRating == 0)
            {
                MessageBox.Show("Por favor selecciona una calificación", "Información",
                    MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            SubmitButton.IsEnabled = false;

            var review = new Review
            {
                Rating = selectedRating,
                Comment = CommentTextBox.Text.Trim(),
                Timestamp = DateTime.Now,
                MachineName = Environment.MachineName,
                UserName = Environment.UserName
            };

            await SaveReview(review);

            StatusText.Text = "¡Gracias por tu opinión!";
            StatusText.Visibility = Visibility.Visible;

            await Task.Delay(1500);

            Completed?.Invoke(this, EventArgs.Empty);
        }

        private async Task SaveReview(Review review)
        {
            try
            {
                // Guardar en base de datos SQLite
                await ReviewDatabase.SaveReviewAsync(review);

                // Si hay conexión a internet, enviar a servidor remoto
                if (System.Net.NetworkInformation.NetworkInterface.GetIsNetworkAvailable())
                {
                    await SendReviewToServer(review);
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error saving review: {ex.Message}");
            }
        }

        private async Task SendReviewToServer(Review review)
        {
            try
            {
                // En una implementación real, aquí enviarías a tu API
                // Por ejemplo, usando HttpClient para enviar a tu servidor

                var httpClient = new System.Net.Http.HttpClient();
                var json = System.Text.Json.JsonSerializer.Serialize(review);
                var content = new System.Net.Http.StringContent(json,
                    System.Text.Encoding.UTF8, "application/json");

                // URL de ejemplo - cambiar por tu API real
                // await httpClient.PostAsync("https://tuservidor.com/api/reviews", content);

                await Task.CompletedTask; // Placeholder
            }
            catch
            {
                // Si falla el envío al servidor, al menos está guardado localmente
            }
        }
    }

    public class Review
    {
        public int Id { get; set; }
        public int Rating { get; set; }
        public string Comment { get; set; } = string.Empty;
        public DateTime Timestamp { get; set; }
        public string MachineName { get; set; } = string.Empty;
        public string UserName { get; set; } = string.Empty;
    }
}
