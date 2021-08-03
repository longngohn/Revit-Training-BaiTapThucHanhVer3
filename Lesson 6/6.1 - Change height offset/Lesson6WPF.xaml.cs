using System.Windows;

namespace AlphaBIM
{
    /// <summary>
    /// Interaction logic for Lesson6WPF.xaml
    /// </summary>
    public partial class Lesson6Wpf : Window
    {
        private Lesson6ViewModel _viewModel;
        public Lesson6Wpf(Lesson6ViewModel viewModel)
        {
            InitializeComponent();
            _viewModel = viewModel;
            DataContext = viewModel;
        }



        private void Click(object sender, RoutedEventArgs e)
        {

            DialogResult = true;
            Close();
            _viewModel.SetElevationFloor();
        }
    }
}
