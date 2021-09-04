using System;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Input;

namespace AlphaBIM
{
    public partial class FormworkAreaWindow
    {
        private FormworkAreaViewModel _viewModel;
        public FormworkAreaWindow(FormworkAreaViewModel viewModel)
        {
            InitializeComponent();
            _viewModel = viewModel;
            DataContext = _viewModel;
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }


        private void btnOK_Click(object sender, RoutedEventArgs e)
        {
            _viewModel.ClickOk();
            DialogResult = true;
            Close();
           
        }
    }
}
