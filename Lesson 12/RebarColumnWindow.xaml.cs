using System;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Input;

namespace AlphaBIM
{
    public partial class RebarColumnWindow
    {
        private RebarColumnViewModel _viewModel;
        public RebarColumnWindow(RebarColumnViewModel viewModel)
        {
            InitializeComponent();
            _viewModel = viewModel;
            DataContext = _viewModel;
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            Height = DockPanel.ActualHeight + 45;
            MinHeight = DockPanel.ActualHeight + 45;
        }


        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }


        #region Copy Title bar

        private void OpenWebSite(object sender, RoutedEventArgs e)
        {
            try
            {
                Process.Start("https://alphabimvn.com/vi");
            }
            catch (Exception)
            {
            }
        }

        private void CustomDevelopment(object sender, RoutedEventArgs e)
        {
            try
            {
                Process.Start("http://bit.ly/3bNeJek");
            }
            catch (Exception)
            {
            }
        }

        private void Feedback(object sender, RoutedEventArgs e)
        {
            try
            {
                Process.Start("mailto:contact@alphabimvn.com");
            }
            catch (Exception)
            {
            }
        }

        #endregion Copy Title bar


        private void btnOK_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
            Close();
            _viewModel.ButtonOk();
        }
    }
}
