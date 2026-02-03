using System.Windows;
using System.Windows.Controls;
namespace LibgenUI
{
    public partial class SearchWindow : Window
    {
        public SearchWindow(string query)
        {
            InitializeComponent();
            this.Title = "Risultati per: " + query;
            LoadResult(query);
        }
        private async void LoadResult(string query)
        {
            var listaEsempio = new List<Document> {
                new Document("6846e4a1f2199721c8a318dabaf730d12886f06a94c06bf17f290b51dacdef29", "prova", "prova", 389614, "./uploads\\1374076.jpg")
            };
            lstResults.ItemsSource = listaEsempio;
        }
        private void BtnResultClick(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            if(button == null) return;
            var libro = button.DataContext as Document;
            if(libro == null) return;
            MessageBox.Show($"Hai scelto: {libro.Title}. Avvio download...");
        }
    }
}
