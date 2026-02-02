using Microsoft.Win32;
using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using System.Windows;

namespace Client
{
    public partial class MainWindow : Window
    {
        // Riutilizziamo HttpClient per efficienza (Best Practice)
        //prova visuallizzazione commit
        private static readonly HttpClient _httpClient = new HttpClient();

        public MainWindow() => InitializeComponent();

        private async void BtnSelect_Click(object sender, RoutedEventArgs e)
        {
            // 1. Selezione del file
            OpenFileDialog openFileDialog = new OpenFileDialog();
            if (openFileDialog.ShowDialog() == true)
            {
                string filePath = openFileDialog.FileName;
                lblFileName.Text = Path.GetFileName(filePath);

                await UploadFile(filePath);
            }
        }

        private async Task UploadFile(string filePath)
        {
            // Reset Interfaccia
            btnSelect.IsEnabled = false;
            uploadProgressBar.Visibility = Visibility.Visible;
            uploadProgressBar.IsIndeterminate = true; // In attesa di risposta
            lblStatus.Text = "Caricamento in corso...";

            try
            {
                using var form = new MultipartFormDataContent();
                using var fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read);
                using var streamContent = new StreamContent(fileStream);

                // Aggiungiamo il file con la chiave "file" (che Go dovrà leggere)
                form.Add(streamContent, "file", Path.GetFileName(filePath));

                // Chiamata all'API di Go
                var response = await _httpClient.PostAsync("http://localhost:8080/upload", form);

                if (response.IsSuccessStatusCode)
                {
                    lblStatus.Text = "Successo! Il processo Go ha ricevuto il file.";
                    lblStatus.Foreground = System.Windows.Media.Brushes.Green;
                }
                else
                {
                    lblStatus.Text = $"Errore Server: {response.StatusCode}";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Errore: {ex.Message}", "Errore di Connessione");
                lblStatus.Text = "Caricamento fallito.";
            }
            finally
            {
                btnSelect.IsEnabled = true;
                uploadProgressBar.Visibility = Visibility.Hidden;
            }
        }
    }
}