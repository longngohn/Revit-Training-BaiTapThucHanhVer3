using Autodesk.Revit.DB;
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
