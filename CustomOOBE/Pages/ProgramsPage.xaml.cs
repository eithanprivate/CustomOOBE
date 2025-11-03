using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace CustomOOBE.Pages
{
    public partial class ProgramsPage : UserControl
    {
        public event EventHandler? Completed;

        private ObservableCollection<ProgramInfo> programs = new ObservableCollection<ProgramInfo>();
        private readonly HttpClient httpClient = new HttpClient();

        public ProgramsPage()
        {
            InitializeComponent();
            InitializePrograms();
        }

        private void InitializePrograms()
        {
            // Definir programas disponibles con enlaces de descarga reales
            programs = new ObservableCollection<ProgramInfo>
            {
                new ProgramInfo
                {
                    Name = "Google Chrome",
                    Description = "Navegador web rápido y seguro",
                    Category = "Navegadores",
                    DownloadUrl = "https://dl.google.com/chrome/install/latest/chrome_installer.exe",
                    IsSelected = false
                },
                new ProgramInfo
                {
                    Name = "Mozilla Firefox",
                    Description = "Navegador web de código abierto",
                    Category = "Navegadores",
                    DownloadUrl = "https://download.mozilla.org/?product=firefox-latest&os=win&lang=es-ES",
                    IsSelected = false
                },
                new ProgramInfo
                {
                    Name = "7-Zip",
                    Description = "Herramienta de compresión de archivos",
                    Category = "Utilidades",
                    DownloadUrl = "https://www.7-zip.org/a/7z2301-x64.exe",
                    IsSelected = false
                },
                new ProgramInfo
                {
                    Name = "VLC Media Player",
                    Description = "Reproductor multimedia versátil",
                    Category = "Multimedia",
                    DownloadUrl = "https://get.videolan.org/vlc/last/win64/vlc-3.0.20-win64.exe",
                    IsSelected = false
                },
                new ProgramInfo
                {
                    Name = "CCleaner",
                    Description = "Herramienta de limpieza del sistema",
                    Category = "Limpieza",
                    DownloadUrl = "https://download.ccleaner.com/ccsetup.exe",
                    IsSelected = false
                },
                new ProgramInfo
                {
                    Name = "Malwarebytes",
                    Description = "Protección contra malware",
                    Category = "Seguridad",
                    DownloadUrl = "https://downloads.malwarebytes.com/file/mb-windows",
                    IsSelected = false
                },
                new ProgramInfo
                {
                    Name = "Adobe Reader",
                    Description = "Lector de documentos PDF",
                    Category = "Documentos",
                    DownloadUrl = "https://ardownload2.adobe.com/pub/adobe/reader/win/AcrobatDC/misc/AcroRdrDC.exe",
                    IsSelected = false
                },
                new ProgramInfo
                {
                    Name = "LibreOffice",
                    Description = "Suite ofimática gratuita",
                    Category = "Productividad",
                    DownloadUrl = "https://download.documentfoundation.org/libreoffice/stable/7.6.4/win/x86_64/LibreOffice_7.6.4_Win_x86-64.msi",
                    IsSelected = false
                },
                new ProgramInfo
                {
                    Name = "Spotify",
                    Description = "Streaming de música",
                    Category = "Multimedia",
                    DownloadUrl = "https://download.scdn.co/SpotifySetup.exe",
                    IsSelected = false
                },
                new ProgramInfo
                {
                    Name = "Discord",
                    Description = "Comunicación por voz y texto",
                    Category = "Comunicación",
                    DownloadUrl = "https://discord.com/api/downloads/distributions/app/installers/latest?channel=stable&platform=win&arch=x86",
                    IsSelected = false
                }
            };

            ProgramsList.ItemsSource = programs;
        }

        private void Skip_Click(object sender, RoutedEventArgs e)
        {
            Completed?.Invoke(this, EventArgs.Empty);
        }

        private async void Continue_Click(object sender, RoutedEventArgs e)
        {
            var selectedPrograms = programs.Where(p => p.IsSelected).ToList();

            if (selectedPrograms.Count == 0)
            {
                Completed?.Invoke(this, EventArgs.Empty);
                return;
            }

            // Ocultar lista y mostrar progreso
            ProgramsList.Visibility = Visibility.Collapsed;
            DownloadPanel.Visibility = Visibility.Visible;
            SkipButton.IsEnabled = false;
            ContinueButton.IsEnabled = false;

            await DownloadAndInstallPrograms(selectedPrograms);

            Completed?.Invoke(this, EventArgs.Empty);
        }

        private async Task DownloadAndInstallPrograms(List<ProgramInfo> selectedPrograms)
        {
            int totalPrograms = selectedPrograms.Count;
            int currentIndex = 0;

            string downloadFolder = Path.Combine(Path.GetTempPath(), "OOBEDownloads");
            Directory.CreateDirectory(downloadFolder);

            foreach (var program in selectedPrograms)
            {
                currentIndex++;
                CurrentProgramText.Text = $"Instalando {program.Name} ({currentIndex}/{totalPrograms})...";

                try
                {
                    // Descargar
                    DownloadStatusText.Text = $"Descargando {program.Name}...";
                    string fileName = Path.Combine(downloadFolder, $"{program.Name.Replace(" ", "")}.exe");

                    using (var response = await httpClient.GetAsync(program.DownloadUrl, HttpCompletionOption.ResponseHeadersRead))
                    {
                        response.EnsureSuccessStatusCode();

                        using (var fileStream = new FileStream(fileName, FileMode.Create, FileAccess.Write, FileShare.None))
                        using (var httpStream = await response.Content.ReadAsStreamAsync())
                        {
                            var totalBytes = response.Content.Headers.ContentLength ?? -1L;
                            var totalRead = 0L;
                            var buffer = new byte[8192];
                            var isMoreToRead = true;

                            do
                            {
                                var read = await httpStream.ReadAsync(buffer, 0, buffer.Length);
                                if (read == 0)
                                {
                                    isMoreToRead = false;
                                }
                                else
                                {
                                    await fileStream.WriteAsync(buffer, 0, read);
                                    totalRead += read;

                                    if (totalBytes > 0)
                                    {
                                        var progress = (double)totalRead / totalBytes * 100;
                                        program.Progress = (int)progress;
                                    }
                                }
                            }
                            while (isMoreToRead);
                        }
                    }

                    // Instalar
                    DownloadStatusText.Text = $"Instalando {program.Name}...";
                    await InstallProgram(fileName);

                    program.Progress = 100;
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"Error instalando {program.Name}: {ex.Message}");
                }

                TotalProgressBar.Value = (double)currentIndex / totalPrograms * 100;
            }

            DownloadStatusText.Text = "¡Instalación completada!";
            await Task.Delay(1500);
        }

        private async Task InstallProgram(string installerPath)
        {
            try
            {
                var process = new Process
                {
                    StartInfo = new ProcessStartInfo
                    {
                        FileName = installerPath,
                        Arguments = "/S /silent /verysilent /quiet /norestart", // Argumentos silenciosos comunes
                        UseShellExecute = true,
                        Verb = "runas" // Ejecutar como administrador
                    }
                };

                process.Start();

                // Esperar a que termine (máximo 5 minutos)
                await Task.Run(() => process.WaitForExit(300000));
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error en instalación: {ex.Message}");
            }
        }
    }

    public class ProgramInfo : INotifyPropertyChanged
    {
        private bool isSelected;
        private int progress;

        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
        public string DownloadUrl { get; set; } = string.Empty;

        public bool IsSelected
        {
            get => isSelected;
            set
            {
                isSelected = value;
                OnPropertyChanged();
            }
        }

        public int Progress
        {
            get => progress;
            set
            {
                progress = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(ProgressVisibility));
            }
        }

        public Visibility ProgressVisibility => Progress > 0 ? Visibility.Visible : Visibility.Collapsed;

        public event PropertyChangedEventHandler? PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
