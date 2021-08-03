using System.Windows;

namespace AlphaBIM
{
    /// <summary>
    /// Interaction logic for BT1CreateFramingWpf.xaml
    /// </summary>
    public partial class Bt3CreateFloorWpf : Window
    {
        private Bt3CreateFloorViewModel _viewModel;


        public Bt3CreateFloorWpf(Bt3CreateFloorViewModel viewModel)
        {
            InitializeComponent();
            _viewModel = viewModel;
            DataContext = viewModel;


        }

        private void Button_Enter(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
            Close();
            _viewModel.DrawFloor();
        }

        private void Button_Cancel(object sender, RoutedEventArgs e)
        {
            DialogResult = false;

            MessageBox.Show("Progress is Cancel!", "Stop Progress", MessageBoxButton.OK, MessageBoxImage.Stop);
        }
    }
}
