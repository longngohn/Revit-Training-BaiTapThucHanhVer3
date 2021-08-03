using System.Windows;

namespace AlphaBIM
{
    /// <summary>
    /// Interaction logic for TransferWpf.xaml
    /// </summary>
    public partial class TransferParameterWindow
    {
        private TransferViewModel _viewModel;
        public TransferParameterWindow(TransferViewModel viewModel)
        {
            InitializeComponent();
            _viewModel = viewModel;
            DataContext = viewModel;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
            Close();
            _viewModel.TransferParameter();
        }
    }
}
