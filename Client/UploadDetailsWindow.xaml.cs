using System.Windows;
namespace LibgenUI
{
    public partial class UploadDetailsWindow : Window
    {
        public string DocumentTitle { get; private set; } = string.Empty;
        public UploadDetailsWindow()
        {
            InitializeComponent();
        }
        private void BtnConfirmClick(object sender, RoutedEventArgs e)
        {
            DocumentTitle = title.Text;
            this.DialogResult = true;// Chiude la finestra e conferma l'operazione
        }
    }
}
