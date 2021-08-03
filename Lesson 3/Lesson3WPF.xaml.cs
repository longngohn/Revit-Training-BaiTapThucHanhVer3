using System.Windows;

namespace AlphaBIM
{
    /// <summary>
    /// Interaction logic for Lesson3WPF.xaml
    /// </summary>
    public partial class Lesson3WPF : Window
    {

        public Lesson3ViewModel viewModel;
        public Lesson3WPF(Lesson3ViewModel viewModel)
        {
            this.viewModel = viewModel;
            InitializeComponent();
            DataContext = viewModel;

        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            viewModel.TxtBlock = TextBox1.Text;
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            MessageBox.Show(

                string.Concat("Giá trị IsCheck của các Control: ", "\nRadio 1 == ", Radio1.IsChecked.ToString(),
                               "\nRadio 2 == ", Radio2.IsChecked.ToString(),
                               "\nCheck box 1 == ", CkcBox1.IsChecked.ToString(),
                               "\nCheck box 2 == ", CkcBox2.IsChecked.ToString()));
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            listBox1.Items.Add(viewModel.SelectedCombobox.ToString());
        }
    }
}
