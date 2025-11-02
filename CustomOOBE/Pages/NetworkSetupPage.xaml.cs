using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using ManagedNativeWifi;

namespace CustomOOBE.Pages
{
    public partial class NetworkSetupPage : UserControl
    {
        public event EventHandler<bool>? Completed;

        private bool isConnected = false;
        private NetworkInfo? selectedNetwork;
        private System.Timers.Timer? connectionCheckTimer;

        public NetworkSetupPage()
        {
            InitializeComponent();
            Loaded += NetworkSetupPage_Loaded;
        }

        private void NetworkSetupPage_Loaded(object sender, RoutedEventArgs e)
        {
            CheckEthernetConnection();
            LoadWiFiNetworks();
            StartConnectionMonitoring();
        }

        private void StartConnectionMonitoring()
        {
            connectionCheckTimer = new System.Timers.Timer(2000);
            connectionCheckTimer.Elapsed += (s, args) =>
            {
                Dispatcher.Invoke(() =>
                {
                    CheckEthernetConnection();
                    UpdateConnectionStatus();
                });
            };
            connectionCheckTimer.Start();
        }

        private void CheckEthernetConnection()
        {
            var ethernetConnected = NetworkInterface.GetAllNetworkInterfaces()
                .Any(ni => ni.NetworkInterfaceType == NetworkInterfaceType.Ethernet &&
                          ni.OperationalStatus == OperationalStatus.Up &&
                          ni.GetIPProperties().UnicastAddresses.Any(ip => ip.Address.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork));

            if (ethernetConnected)
            {
                EthernetPanel.Visibility = Visibility.Visible;
                isConnected = true;
                ConnectionStatusText.Text = "Conectado por Ethernet";
                ConnectionStatusText.Foreground = System.Windows.Media.Brushes.Green;
                ContinueButton.IsEnabled = true;
            }
            else
            {
                EthernetPanel.Visibility = Visibility.Collapsed;
            }
        }

        private async void LoadWiFiNetworks()
        {
            try
            {
                var networks = new List<NetworkInfo>();

                await Task.Run(() =>
                {
                    var availableNetworks = NativeWifi.EnumerateAvailableNetworks();

                    foreach (var network in availableNetworks)
                    {
                        networks.Add(new NetworkInfo
                        {
                            Name = network.ProfileName,
                            Security = network.IsSecurityEnabled ? "Segura" : "Abierta",
                            SignalStrength = $"{network.SignalQuality}%",
                            NetworkData = network
                        });
                    }
                });

                NetworksList.ItemsSource = networks;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al cargar redes: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void RefreshNetworks_Click(object sender, RoutedEventArgs e)
        {
            LoadWiFiNetworks();
        }

        private void Network_Click(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (sender is Border border && border.DataContext is NetworkInfo network)
            {
                selectedNetwork = network;
                NetworkNameText.Text = network.Name;

                if (network.Security == "Abierta")
                {
                    ConnectToWiFi(network.Name, null);
                }
                else
                {
                    PasswordPopup.IsOpen = true;
                }
            }
        }

        private void ShowPassword_Changed(object sender, RoutedEventArgs e)
        {
            // Implementar mostrar/ocultar contraseña si es necesario
        }

        private void CancelPassword_Click(object sender, RoutedEventArgs e)
        {
            PasswordPopup.IsOpen = false;
            PasswordBox.Clear();
            ConnectionErrorText.Visibility = Visibility.Collapsed;
        }

        private void ConnectWiFi_Click(object sender, RoutedEventArgs e)
        {
            if (selectedNetwork != null)
            {
                var password = PasswordBox.Password;
                ConnectToWiFi(selectedNetwork.Name, password);
            }
        }

        private async void ConnectToWiFi(string ssid, string? password)
        {
            try
            {
                ConnectionErrorText.Visibility = Visibility.Collapsed;

                await Task.Run(() =>
                {
                    if (password != null)
                    {
                        // Crear perfil de WiFi con contraseña
                        var profileXml = CreateWiFiProfile(ssid, password);
                        NativeWifi.SetProfile(NativeWifi.EnumerateInterfaces().First().Id, ProfileType.AllUser, profileXml, null, true);
                    }

                    // Conectar
                    var targetNetwork = NativeWifi.EnumerateAvailableNetworks()
                        .FirstOrDefault(n => n.ProfileName == ssid);

                    if (targetNetwork != null)
                    {
                        NativeWifi.ConnectNetwork(NativeWifi.EnumerateInterfaces().First().Id, targetNetwork.ProfileName, BssType.Any);
                    }
                });

                // Esperar a que se conecte
                await Task.Delay(3000);

                if (IsWiFiConnected())
                {
                    isConnected = true;
                    PasswordPopup.IsOpen = false;
                    PasswordBox.Clear();
                    UpdateConnectionStatus();
                }
                else
                {
                    ConnectionErrorText.Text = "No se pudo conectar. Verifica la contraseña.";
                    ConnectionErrorText.Visibility = Visibility.Visible;
                }
            }
            catch (Exception ex)
            {
                ConnectionErrorText.Text = $"Error: {ex.Message}";
                ConnectionErrorText.Visibility = Visibility.Visible;
            }
        }

        private string CreateWiFiProfile(string ssid, string password)
        {
            return $@"<?xml version=""1.0""?>
<WLANProfile xmlns=""http://www.microsoft.com/networking/WLAN/profile/v1"">
    <name>{ssid}</name>
    <SSIDConfig>
        <SSID>
            <name>{ssid}</name>
        </SSID>
    </SSIDConfig>
    <connectionType>ESS</connectionType>
    <connectionMode>auto</connectionMode>
    <MSM>
        <security>
            <authEncryption>
                <authentication>WPA2PSK</authentication>
                <encryption>AES</encryption>
                <useOneX>false</useOneX>
            </authEncryption>
            <sharedKey>
                <keyType>passPhrase</keyType>
                <protected>false</protected>
                <keyMaterial>{password}</keyMaterial>
            </sharedKey>
        </security>
    </MSM>
</WLANProfile>";
        }

        private bool IsWiFiConnected()
        {
            return NetworkInterface.GetAllNetworkInterfaces()
                .Any(ni => (ni.NetworkInterfaceType == NetworkInterfaceType.Wireless80211) &&
                          ni.OperationalStatus == OperationalStatus.Up &&
                          ni.GetIPProperties().UnicastAddresses.Any(ip => ip.Address.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork));
        }

        private void UpdateConnectionStatus()
        {
            if (IsWiFiConnected() || NetworkInterface.GetAllNetworkInterfaces()
                .Any(ni => ni.NetworkInterfaceType == NetworkInterfaceType.Ethernet &&
                          ni.OperationalStatus == OperationalStatus.Up))
            {
                isConnected = true;
                ConnectionStatusText.Text = "Conectado";
                ConnectionStatusText.Foreground = System.Windows.Media.Brushes.Green;
                ContinueButton.IsEnabled = true;
            }
        }

        private void Skip_Click(object sender, RoutedEventArgs e)
        {
            connectionCheckTimer?.Stop();
            Completed?.Invoke(this, false);
        }

        private void Continue_Click(object sender, RoutedEventArgs e)
        {
            if (isConnected)
            {
                connectionCheckTimer?.Stop();
                Completed?.Invoke(this, true);
            }
        }
    }

    public class NetworkInfo
    {
        public string Name { get; set; } = string.Empty;
        public string Security { get; set; } = string.Empty;
        public string SignalStrength { get; set; } = string.Empty;
        public object? NetworkData { get; set; }
    }
}
