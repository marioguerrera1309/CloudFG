using System.Windows;
namespace LibgenUI
{
    public partial class UploadDetailsWindow : Window
    {
        public string DocumentTitle { get; private set; } = string.Empty;
        public string DocumentAuthor { get; private set; } = string.Empty;
        public UploadDetailsWindow()
        {
            InitializeComponent();
        }
        private void BtnConfirmClick(object sender, RoutedEventArgs e)
        {
            DocumentTitle = title.Text;
            DocumentAuthor = author.Text;
            this.DialogResult = true;// Chiude la finestra e conferma l'operazione
        }
    }
}
