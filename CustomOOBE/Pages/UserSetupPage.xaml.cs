using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using AForge.Video;
using AForge.Video.DirectShow;

namespace CustomOOBE.Pages
{
    public partial class UserSetupPage : UserControl
    {
        public event EventHandler<UserSetupData>? Completed;

        private string? selectedAvatarPath;
        private VideoCaptureDevice? videoSource;
        private Window? cameraWindow;

        public UserSetupPage()
        {
            InitializeComponent();
            LoadAvatars();
        }

        private void LoadAvatars()
        {
            // Cargar avatares predefinidos
            string avatarsPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Assets", "Avatars");

            if (Directory.Exists(avatarsPath))
            {
                var avatarFiles = Directory.GetFiles(avatarsPath, "*.png");

                foreach (var avatarFile in avatarFiles)
                {
                    var border = new Border
                    {
                        Width = 80,
                        Height = 80,
                        CornerRadius = new CornerRadius(40),
                        Margin = new Thickness(10),
                        Cursor = System.Windows.Input.Cursors.Hand,
                        Tag = avatarFile
                    };

                    border.Background = new ImageBrush
                    {
                        ImageSource = new BitmapImage(new Uri(avatarFile)),
                        Stretch = Stretch.UniformToFill
                    };

                    border.MouseLeftButtonDown += Avatar_Click;

                    AvatarPanel.Children.Add(border);
                }
            }
        }

        private void ChooseAvatar_Click(object sender, RoutedEventArgs e)
        {
            AvatarPopup.IsOpen = true;
        }

        private void Avatar_Click(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (sender is Border border && border.Tag is string avatarPath)
            {
                selectedAvatarPath = avatarPath;
                AvatarImage.ImageSource = new BitmapImage(new Uri(avatarPath));
                AvatarPopup.IsOpen = false;
            }
        }

        private void TakePhoto_Click(object sender, RoutedEventArgs e)
        {
            OpenCamera();
        }

        private void OpenCamera()
        {
            var videoDevices = new FilterInfoCollection(FilterCategory.VideoInputDevice);

            if (videoDevices.Count == 0)
            {
                MessageBox.Show("No se encontró ninguna cámara.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            videoSource = new VideoCaptureDevice(videoDevices[0].MonikerString);

            cameraWindow = new Window
            {
                Title = "Tomar Foto",
                Width = 640,
                Height = 480,
                WindowStartupLocation = WindowStartupLocation.CenterScreen,
                ResizeMode = ResizeMode.NoResize
            };

            var grid = new Grid();
            var image = new Image { Stretch = Stretch.Uniform };
            var captureButton = new Button
            {
                Content = "Capturar",
                Width = 120,
                Height = 40,
                VerticalAlignment = VerticalAlignment.Bottom,
                Margin = new Thickness(0, 0, 0, 20)
            };

            grid.Children.Add(image);
            grid.Children.Add(captureButton);
            cameraWindow.Content = grid;

            BitmapImage? capturedImage = null;

            videoSource.NewFrame += (s, args) =>
            {
                Dispatcher.Invoke(() =>
                {
                    using var bitmap = (System.Drawing.Bitmap)args.Frame.Clone();
                    var bitmapImage = ConvertBitmap(bitmap);
                    image.Source = bitmapImage;
                    capturedImage = bitmapImage;
                });
            };

            captureButton.Click += (s, args) =>
            {
                if (capturedImage != null)
                {
                    videoSource.SignalToStop();
                    videoSource.WaitForStop();

                    AvatarImage.ImageSource = capturedImage;

                    // Guardar la imagen
                    var tempPath = Path.Combine(Path.GetTempPath(), "user_avatar.png");
                    SaveBitmapImage(capturedImage, tempPath);
                    selectedAvatarPath = tempPath;

                    cameraWindow?.Close();
                }
            };

            cameraWindow.Closed += (s, args) =>
            {
                if (videoSource.IsRunning)
                {
                    videoSource.SignalToStop();
                    videoSource.WaitForStop();
                }
            };

            videoSource.Start();
            cameraWindow.ShowDialog();
        }

        private BitmapImage ConvertBitmap(System.Drawing.Bitmap bitmap)
        {
            using var memory = new MemoryStream();
            bitmap.Save(memory, System.Drawing.Imaging.ImageFormat.Png);
            memory.Position = 0;

            var bitmapImage = new BitmapImage();
            bitmapImage.BeginInit();
            bitmapImage.StreamSource = memory;
            bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
            bitmapImage.EndInit();
            bitmapImage.Freeze();

            return bitmapImage;
        }

        private void SaveBitmapImage(BitmapImage image, string filePath)
        {
            var encoder = new PngBitmapEncoder();
            encoder.Frames.Add(BitmapFrame.Create(image));

            using var fileStream = new FileStream(filePath, FileMode.Create);
            encoder.Save(fileStream);
        }

        private void UsernameTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            ValidateUsername();
        }

        private bool ValidateUsername()
        {
            var username = UsernameTextBox.Text.Trim();

            if (string.IsNullOrWhiteSpace(username))
            {
                ContinueButton.IsEnabled = false;
                ErrorText.Visibility = Visibility.Collapsed;
                return false;
            }

            // Validar caracteres permitidos
            if (!System.Text.RegularExpressions.Regex.IsMatch(username, @"^[a-zA-Z0-9_-]+$"))
            {
                ErrorText.Text = "El nombre de usuario solo puede contener letras, números, guiones y guiones bajos.";
                ErrorText.Visibility = Visibility.Visible;
                ContinueButton.IsEnabled = false;
                return false;
            }

            ErrorText.Visibility = Visibility.Collapsed;
            ContinueButton.IsEnabled = true;
            return true;
        }

        private void Continue_Click(object sender, RoutedEventArgs e)
        {
            if (ValidateUsername())
            {
                var data = new UserSetupData
                {
                    Username = UsernameTextBox.Text.Trim(),
                    AvatarPath = selectedAvatarPath
                };

                // Crear el usuario de Windows
                CreateWindowsUser(data.Username);

                // Establecer la imagen de perfil
                if (!string.IsNullOrEmpty(data.AvatarPath))
                {
                    SetUserProfilePicture(data.Username, data.AvatarPath);
                }

                Completed?.Invoke(this, data);
            }
        }

        private void CreateWindowsUser(string username)
        {
            try
            {
                var process = new System.Diagnostics.Process
                {
                    StartInfo = new System.Diagnostics.ProcessStartInfo
                    {
                        FileName = "net",
                        Arguments = $"user {username} /add",
                        UseShellExecute = false,
                        CreateNoWindow = true,
                        Verb = "runas"
                    }
                };

                process.Start();
                process.WaitForExit();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al crear usuario: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void SetUserProfilePicture(string username, string imagePath)
        {
            try
            {
                // Copiar imagen al directorio de perfil
                var accountPicturesPath = Path.Combine(
                    Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData),
                    "Microsoft", "User Account Pictures");

                Directory.CreateDirectory(accountPicturesPath);

                var destPath = Path.Combine(accountPicturesPath, $"{username}.png");
                File.Copy(imagePath, destPath, true);

                // Establecer en el registro
                using var key = Microsoft.Win32.Registry.LocalMachine.OpenSubKey(
                    $@"SOFTWARE\Microsoft\Windows\CurrentVersion\AccountPicture\Users\{username}", true);

                key?.SetValue("Image", destPath);
            }
            catch { }
        }
    }

    public class UserSetupData
    {
        public string Username { get; set; } = string.Empty;
        public string? AvatarPath { get; set; }
    }
}
