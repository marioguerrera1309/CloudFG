using Microsoft.Win32;//libreria per aprire la finestra per selezionare il file
using System;
using System.IO;
using System.Net.Http;
using System.Printing;
using System.Windows;
namespace LibgenUI
{
    public partial class MainWindow : Window
    {
        private static readonly HttpClient client = new HttpClient();
        public MainWindow()
        {
            InitializeComponent();
        }
        private async void BtnSendClick(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            if (openFileDialog.ShowDialog() == true)
            {
                string filePath = openFileDialog.FileName;
                //Assegna al label il nome del file selezionato
                lblFileName.Text = Path.GetFileName(filePath);
                await UploadFile(filePath);
            }
        }
        private async Task UploadFile(string filePath)
        {
            btnSend.IsEnabled = false;
            uploadProgressBar.Visibility = Visibility.Visible;
            uploadProgressBar.IsIndeterminate = true;
            lblStatus.Text = "Caricamento in corso...";
            try
            {
                using var form = new MultipartFormDataContent();
                using var fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read);
                using var streamContent = new StreamContent(fileStream);
                //Aggiunge il file al contenuto del form
                form.Add(streamContent, "file", Path.GetFileName(filePath));
                var response = await client.PostAsync("http://localhost:8080/upload", form);
                if (response.IsSuccessStatusCode)
                {
                    lblStatus.Text = "Successo! Il Server ha ricevuto il file.";
                    lblStatus.Foreground = System.Windows.Media.Brushes.Green;
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.Conflict)
                {
                    lblStatus.Text = "Errore: File già esistente sul server.";
                    lblStatus.Foreground = System.Windows.Media.Brushes.Red;
                }
                else
                {
                    lblStatus.Text = $"Errore Server: {response.StatusCode}";
                    lblStatus.Foreground = System.Windows.Media.Brushes.Red;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Errore: {ex.Message}", "Errore di Connessione");
                lblStatus.Text = "Caricamento fallito.";
            }
            finally
            {
                btnSend.IsEnabled = true;
                uploadProgressBar.Visibility = Visibility.Hidden;
            }
        }
        private void RemoveText(object sender, RoutedEventArgs e)
        {
            if (lblSearch.Text == "Inserisci un titolo o un autore...")
            {
                lblSearch.Text = "";
                lblSearch.Foreground = System.Windows.Media.Brushes.Black;
            }
        }
        private void BtnSearchClick(object sender, RoutedEventArgs e)
        {
            string query = lblSearch.Text;
            MessageBox.Show($"Ricerca per: {query}", "Ricerca");
        }
    }
}