using Microsoft.Win32;
using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using System.Windows;
namespace Client {
    public partial class MainWindow : Window {
        private static readonly HttpClient _httpClient = new HttpClient();
        public MainWindow() => InitializeComponent();
        private async void BtnSelect_Click(object sender, RoutedEventArgs e) {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            if (openFileDialog.ShowDialog() == true) {
                string filePath = openFileDialog.FileName;
                //Assegna al label il nome del file selezionato
                lblFileName.Text = Path.GetFileName(filePath);
                await UploadFile(filePath);
            }
        }
        private async Task UploadFile(string filePath) {
            btnSelect.IsEnabled = false;
            uploadProgressBar.Visibility = Visibility.Visible;
            uploadProgressBar.IsIndeterminate = true;
            lblStatus.Text = "Caricamento in corso...";
            try {
                using var form = new MultipartFormDataContent();
                var fileInfo = new FileInfo(filePath);
                string fileName = Path.GetFileName(filePath);
                using var fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                using var streamContent = new StreamContent(fileStream);
                //Aggiunge il file al contenuto del form
                form.Add(streamContent, "file", Path.GetFileName(filePath));
                form.Add(new StringContent(fileName), "title");
                form.Add(new StringContent("Autore Default"), "author"); // Potrai collegarlo a una TextBox
                form.Add(new StringContent(filePath), "original_path");
                form.Add(new StringContent(fileInfo.Length.ToString()), "size");
                var response = await _httpClient.PostAsync("http://localhost:8080/upload", form);
                if (response.IsSuccessStatusCode) {
                    lblStatus.Text = "Successo! Il processo Go ha ricevuto il file.";
                    lblStatus.Foreground = System.Windows.Media.Brushes.Green;
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.Conflict) {
                    lblStatus.Text = "Il file esiste già nel server (Hash duplicato).";
                    lblStatus.Foreground = System.Windows.Media.Brushes.Orange;
                }
                else {
                    lblStatus.Text = $"Errore Server: {response.StatusCode}";
                    lblStatus.Foreground = System.Windows.Media.Brushes.Red;
                }
            }
            catch (Exception ex) {
                MessageBox.Show($"Errore: {ex.Message}", "Errore di Connessione");
                lblStatus.Text = "Caricamento fallito.";
            }
            finally {
                btnSelect.IsEnabled = true;
                uploadProgressBar.Visibility = Visibility.Hidden;
            }
        }
    }
}