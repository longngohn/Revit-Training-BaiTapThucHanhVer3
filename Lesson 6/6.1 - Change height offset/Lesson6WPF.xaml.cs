using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

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
