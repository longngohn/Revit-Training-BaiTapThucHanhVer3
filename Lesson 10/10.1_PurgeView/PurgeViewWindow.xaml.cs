using System;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using MessageBox = System.Windows.Forms.MessageBox;

namespace AlphaBIM
{
    public partial class PurgeModelWindow
    {
        PurgeViewModel _viewModel;

        public PurgeModelWindow(PurgeViewModel viewModel)
        {
            InitializeComponent();
            _viewModel = viewModel;
            DataContext = _viewModel;
        }

        private void DeleteView_SelectNone_Checked(object sender, RoutedEventArgs e)
        {
            foreach (var vs in _viewModel.AllViewsExtension)
            {
                vs.IsSelected = false;
            }
        }

        private void DeleteView_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
            _viewModel.DeleteView();
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }

        private void SelectAll_Checked(object sender, RoutedEventArgs e)
        {
            foreach (ViewExtension v in _viewModel.AllViewsExtension)
            {
                v.IsSelected = true;
            }
        }

        private void SelectNone_Checked(object sender, RoutedEventArgs e)
        {
            foreach (ViewExtension v in _viewModel.AllViewsExtension)
            {
                v.IsSelected = false;
            }
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
    }
}
