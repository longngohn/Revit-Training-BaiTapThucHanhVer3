using System.Windows;

namespace AlphaBIM
{
    /// <summary>
    /// Interaction logic for TransferWpf.xaml
    /// </summary>
    public partial class Bai2Wpf
    {
        private Bai2ViewModel _viewModel;
        public Bai2Wpf(Bai2ViewModel viewModel)
        {
            InitializeComponent();
            _viewModel = viewModel;
            DataContext = viewModel;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
            Close();
            _viewModel.ChangeFloorLevel();
        }
    }
}