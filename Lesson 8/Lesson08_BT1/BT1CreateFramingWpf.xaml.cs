using System.Windows;

namespace AlphaBIM
{
    /// <summary>
    /// Interaction logic for BT1CreateFramingWpf.xaml
    /// </summary>
    public partial class Bt1CreateFramingWpf : Window
    {
        private Bt1CreateFramingViewModel _viewModel;


        public Bt1CreateFramingWpf(Bt1CreateFramingViewModel viewModel)
        {
            InitializeComponent();
            _viewModel = viewModel;
            DataContext = viewModel;


        }

        private void Button_Enter(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
            Close();
            _viewModel.DrawBeam();
        }

        private void Button_Cancel(object sender, RoutedEventArgs e)
        {
            DialogResult = false;

            MessageBox.Show("Progress is Cancel!", "Stop Progress", MessageBoxButton.OK, MessageBoxImage.Stop);
        }
    }
}
