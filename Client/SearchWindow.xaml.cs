using System.Net.Http;
using System.Windows;
using System.Windows.Controls;
using System.Diagnostics;
using System.IO;
namespace CloudFG
{
    public partial class SearchWindow : Window
    {
        private readonly string username;
        public SearchWindow(string query, string username)
        {
            InitializeComponent();
            this.username = username;
            this.Title = "Risultati per: " + query;
            LoadResult(query);
        }
        private async void LoadResult(string query)
        {
            try {
                HttpClient client = new HttpClient();
                // Chiamata al server Go passando la query nell'URL
                var response = await client.GetAsync($"http://localhost:8080/search?query={query}&user={username}");
                if (response.IsSuccessStatusCode) {
                    string json = await response.Content.ReadAsStringAsync();
                    // Trasforma il JSON ricevuto dal server in una lista di oggetti Document
                    var risultati = System.Text.Json.JsonSerializer.Deserialize<List<Document>>(json, new System.Text.Json.JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                    if (risultati != null)
                    {
                        lstResults.ItemsSource = risultati;
                        this.Show();
                    } else {
                        MessageBox.Show("Nessun risultato trovato.");
                        lstResults.ItemsSource = null;
                        this.Close();
                    }
                }
            } catch (Exception ex) {
                MessageBox.Show("Errore durante la ricerca: " + ex.Message);
            }
        }
        private async void BtnResultClick(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            if(button == null) return;
            var libro = button.DataContext as Document;
            if(libro == null) return;
            MessageBox.Show($"Hai scelto: {libro.Title}. Avvio download...");
            try {
                HttpClient client = new HttpClient();
                // Chiediamo il file al server usando l'hash
                var response = await client.GetAsync($"http://localhost:8080/download?hash={libro.Hash}&user={username}");
                if (response.IsSuccessStatusCode) {
                    // Definiamo dove salvare il file temporaneamente (es: nella cartella Download dell'utente)
                    string tempFolder = Path.Combine(Path.GetTempPath(), "CloudFGDownloads");
                    Directory.CreateDirectory(tempFolder);
                    // Se Title è null, usa l'Hash o una stringa generica come nome file
                    string fileName = libro.Title ?? libro.Hash ?? "documento_senza_nome";
                    string fullSavePath = Path.Combine(tempFolder, fileName);
                    // Salviamo il file su disco
                    var fileBytes = await response.Content.ReadAsByteArrayAsync();
                    await File.WriteAllBytesAsync(fullSavePath, fileBytes);
                    //apriamo il file con il programma predefinito (Word, PDF Reader, ecc.)
                    var psi = new ProcessStartInfo {
                        FileName = fullSavePath,
                        UseShellExecute = true // Fondamentale per aprire il file e non un esegbile
                    };
                    Process.Start(psi);
                }
            } catch (Exception ex) {
                MessageBox.Show($"Errore nel download: {ex.Message}");
            }
        }
        private async void BtnDeleteResultClick(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            var libro = button?.DataContext as Document;
            if (libro == null) return;
            var conferma = MessageBox.Show($"Sei sicuro di voler eliminare '{libro.Title}'?", "Conferma", MessageBoxButton.YesNo);
            if (conferma == MessageBoxResult.Yes)
            {
                try {
                    HttpClient client = new HttpClient();
                    var response = await client.DeleteAsync($"http://localhost:8080/delete?hash={libro.Hash}&user={username}");
                    if (response.IsSuccessStatusCode) {
                    MessageBox.Show("Eliminato!");
                    //ricarica la ricerca per aggiornare la lista
                    LoadResult(libro.Title); 
                }
                } catch (Exception ex) {
                    MessageBox.Show($"Errore: {ex.Message}");
                }
            }
        }


    }
}
