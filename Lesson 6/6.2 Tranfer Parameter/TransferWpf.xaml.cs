﻿using System;
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
    /// Interaction logic for TransferWpf.xaml
    /// </summary>
    public partial class TransferParameterWindow
    {
        private TransferViewModel _viewModel;
        public TransferParameterWindow(TransferViewModel viewModel)
        {
            InitializeComponent();
            _viewModel = viewModel;
            DataContext = viewModel;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
            Close();
            _viewModel.TransferParameter();
        }
    }
}