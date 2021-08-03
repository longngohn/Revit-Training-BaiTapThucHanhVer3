using System.Windows;

namespace AlphaBIM
{
    /// <summary>
    /// Interaction logic for BT1CreateFramingWpf.xaml
    /// </summary>
    public partial class Bt2CreateClolumnWpf : Window
    {
        private BT2CreateColumnViewModel _viewModel;


        public Bt2CreateClolumnWpf(BT2CreateColumnViewModel viewModel)
        {
            InitializeComponent();
            _viewModel = viewModel;
            DataContext = viewModel;


        }

        private void Button_Enter(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
            Close();
            _viewModel.DrawColumn();
        }

        private void Button_Cancel(object sender, RoutedEventArgs e)
        {
            DialogResult = false;

            MessageBox.Show("Progress is Cancel!", "Stop Progress", MessageBoxButton.OK, MessageBoxImage.Stop);
        }
    }
}
