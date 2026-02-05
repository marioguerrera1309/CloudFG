using Microsoft.Win32;//libreria per aprire la finestra per selezionare il file
using System.IO;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Windows;
namespace LibgenUI
{
    public partial class MainWindow : Window
    {
        private static readonly HttpClient client = new HttpClient();
        private Window last;
        private readonly string username;
        public MainWindow(string username)
        {
            InitializeComponent();
            last = this;
            this.username = username;
            lblWelcome.Text = "Benvenuto, " + username;
        }
        private async void BtnSendClick(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            if (openFileDialog.ShowDialog() == true) {
                string filePath = openFileDialog.FileName;
                UploadDetailsWindow detailsWin = new UploadDetailsWindow();
                detailsWin.Owner = this;
                if (detailsWin.ShowDialog() == true) // Resta bloccato finché non preme Conferma
                {
                    string title = detailsWin.DocumentTitle;
                    string author = username;
                    //Assegna al label il nome del file selezionato<
                    lblFileName.Text = Path.GetFileName(filePath);
                    await UploadFile(filePath, title, author);
                }
            }
        }
        private async Task UploadFile(string filePath, string title, string author)
        {
            btnSend.IsEnabled = false;
            uploadProgressBar.Visibility = Visibility.Visible;
            uploadProgressBar.IsIndeterminate = true;
            lblStatus.Text = "Caricamento in corso...";
            try {
                using var form = new MultipartFormDataContent();
                using var fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read);
                using var streamContent = new StreamContent(fileStream);
                //Aggiunge il file al contenuto del form
                form.Add(new StringContent(title), "title");
                form.Add(new StringContent(author), "author");
                form.Add(streamContent, "file", Path.GetFileName(filePath));
                var response = await client.PostAsync("http://localhost:8080/upload", form);
                if (response.IsSuccessStatusCode) {
                    lblStatus.Text = "Successo! Il Server ha ricevuto il file.";
                    lblStatus.Foreground = System.Windows.Media.Brushes.Green;
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.Conflict) {
                    lblStatus.Text = "Errore: File già esistente sul server.";
                    lblStatus.Foreground = System.Windows.Media.Brushes.Red;
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
                btnSend.IsEnabled = true;
                uploadProgressBar.Visibility = Visibility.Hidden;
            }
        }
        private void RemoveText(object sender, RoutedEventArgs e)
        {
            if (lblSearch.Text == "Inserisci un titolo") {
                lblSearch.Text = "";
                lblSearch.Foreground = System.Windows.Media.Brushes.Black;
            }
        }
        private void BtnSearchClick(object sender, RoutedEventArgs e)
        {
            string query = lblSearch.Text;
            SearchWindow searchWin = new SearchWindow(query, username);
            double offset = 40;//spostamento della finestra rispetto alla finestra principale
            double nextLeft = last.Left + offset;
            // Se superiamo la larghezza dello schermo ricominciamo da sinistra
            if (nextLeft + searchWin.Width > SystemParameters.VirtualScreenWidth) {
                nextLeft = 0;
            }
            searchWin.Left = nextLeft;
            double nextTop = last.Top + offset;
            // Se superiamo la larghezza dello schermo, ricominciamo da sinistra
            if (nextTop + searchWin.Height > SystemParameters.VirtualScreenHeight) {
                nextTop = 0;
            }
            searchWin.Top = nextTop;
            last = searchWin;
            searchWin.Show();
        }
        private void WindowClosed(object sender, EventArgs e)
        {
            Application.Current.Shutdown();
        }
        private void BtnLogoutClick(object sender, RoutedEventArgs e)
        {
            // Rimuove il token salvato
            LibgenUI.Properties.Settings.Default.UserToken = string.Empty;
            LibgenUI.Properties.Settings.Default.Save();
            // Rimuove l'evento di chiusura per evitare che chiuda l'intera applicazione quando chiudiamo la MainWindow per aprire la LoginWindow
            this.Closed -= WindowClosed;
            // Apre la LoginWindow
            LoginWindow loginWin = new LoginWindow();
            loginWin.Show();
            // Chiude la MainWindow
            this.Close();
        }
    }
}