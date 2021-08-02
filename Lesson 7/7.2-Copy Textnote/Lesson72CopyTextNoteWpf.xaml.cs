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
using System.Windows.Threading;
using Autodesk.Revit.DB;
using MahApps.Metro.Controls;

namespace AlphaBIM
{
    /// <summary>
    /// Interaction logic for Lesson71Wpf.xaml
    /// </summary>
    public partial class Lesson72CopyTextNoteWpf : MetroWindow
    {
        private Lesson72CopyTextNoteViewModel _viewModel;
        private TransactionGroup tG;
        public Lesson72CopyTextNoteWpf(Lesson72CopyTextNoteViewModel viewModel)
        {
            InitializeComponent();
            _viewModel = viewModel;
            DataContext = _viewModel;
            tG = new TransactionGroup(_viewModel.Doc);
        }

        private void Cancel(object sender, RoutedEventArgs e)
        {
            DialogResult = false;

            tG.RollBack();
            MessageBox.Show("Progress is Cancel!", "Stop Progress",
                MessageBoxButton.OK, MessageBoxImage.Stop);

        }

        private void Join(object sender, RoutedEventArgs e)
        {
            #region Lấy về maximum những element cần chạy

            List<Element> allFoundationToRun = new List<Element>();

            allFoundationToRun = new FilteredElementCollector(_viewModel.Doc, _viewModel.Doc.ActiveView.Id)
                .OfCategory(BuiltInCategory.OST_StructuralFoundation)
                .ToList();

            if (allFoundationToRun.Count == 0) return;

            #endregion

            ProgressWindow.Maximum = allFoundationToRun.Count;
            tG.Start("Run process");

            List<ElementId> elementSuccess = new List<ElementId>();
            double value = 0;

            foreach(Element foundation in allFoundationToRun)
            {
                if (tG.HasStarted())
                {
                    #region ProgressBar

                    value = value + 1;
                    _viewModel.Percent = value / ProgressWindow.Maximum * 100;
                    ProgressWindow.Dispatcher.Invoke(() => ProgressWindow.Value = value, DispatcherPriority.Background);
                    

                    #endregion

                    //Lấy về dầm đụng với floor trên active View
                    List<TextNote> textNote = foundation.GetTextNoteIntersectWithElement(_viewModel.Doc, _viewModel.SaiSo);
                    if (textNote.Count == 0) continue;

                    using (Transaction trans = new Transaction(_viewModel.Doc))
                    {
                        trans.Start("Copy Textnote To Parameter");

                        #region DeleteWarning

                        DeleteWarningSuper warningSuper = new DeleteWarningSuper();
                        FailureHandlingOptions failOpt = trans.GetFailureHandlingOptions();
                        failOpt.SetFailuresPreprocessor(warningSuper);
                        trans.SetFailureHandlingOptions(failOpt);

                        #endregion

                        Parameter parameter = foundation.LookupParameter(_viewModel.SelectedParameter);

                        if (parameter != null)
                        {
                            parameter.SetValue(textNote[0].Text);

                            elementSuccess.Add(foundation.Id);
                        }


                        trans.Commit();
                    }

                    
                }
                else
                {
                    break;
                }





                    
            }

            if (tG.HasStarted())
            {
                tG.Commit();
                DialogResult = true;

                MessageBox.Show("Đã Copy TextNote vào Parameter",
                    "Copy Textnote Successful", MessageBoxButton.OK,
                    MessageBoxImage.Information);
            }



        }
    }
}
