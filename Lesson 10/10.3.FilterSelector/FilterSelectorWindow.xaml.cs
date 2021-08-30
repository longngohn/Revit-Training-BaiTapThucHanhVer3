using System;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Input;

namespace AlphaBIM
{
    public partial class FilterSelectorWindow
    {
        private FilterSelectorViewModel _viewModel;
        public FilterSelectorWindow(FilterSelectorViewModel viewModel)
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



        private void btnSelect_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
            Close();
           _viewModel.SelectElement();

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

        private void RuleFilters_CheckBoxClick(object sender, RoutedEventArgs e)
        {
            FilterExtension currentItem = DataGridRuleFilters.CurrentItem as FilterExtension;

            FilterExtension first = _viewModel.SelectedRuleFilters.FirstOrDefault(v => v.Equals(currentItem));
            if (first != null)
            {
                bool selected = first.IsFilterSelected;
                foreach (var v in _viewModel.SelectedRuleFilters)
                    v.IsFilterSelected = !selected;
            }
            else
            {
            }
        }

       
        private void MouseDown(object sender, MouseButtonEventArgs e)
        {
            FilterExtension currentItem = DataGridRuleFilters.CurrentItem as FilterExtension;

            FilterExtension first = _viewModel.SelectedRuleFilters.FirstOrDefault(v => v.Equals(currentItem));
            if (first != null)
            {
                bool selected = first.IsFilterSelected;
                foreach (var v in _viewModel.SelectedRuleFilters)
                    v.IsFilterSelected = !selected;
            }
            else
            {
            }
        }

        private void SelectionFilters_CheckBoxClick(object sender, RoutedEventArgs e)
        {
            FilterExtension currentItem = DataGridSelectionFilters.CurrentItem as FilterExtension;

            FilterExtension first = _viewModel.SelectedSelectionFilters.FirstOrDefault(v => v.Equals(currentItem));
            if (first != null)
            {
                bool selected = first.IsFilterSelected;
                foreach (var v in _viewModel.SelectedSelectionFilters)
                    v.IsFilterSelected = !selected;
            }
            else
            {
            }
        }
    }
}
