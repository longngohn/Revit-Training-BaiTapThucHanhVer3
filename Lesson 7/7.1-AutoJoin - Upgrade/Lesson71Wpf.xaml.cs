using Autodesk.Revit.DB;
using MahApps.Metro.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Threading;

namespace AlphaBIM
{
    /// <summary>
    /// Interaction logic for Lesson71Wpf.xaml
    /// </summary>
    public partial class Lesson71Wpf : MetroWindow
    {
        private Lesson71ViewModel _viewModel;
        private TransactionGroup tG;
        public Lesson71Wpf(Lesson71ViewModel viewModel)
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

            #region Lấy về maximum các đối tượng cần chạy

            List<Element> allCategoryToRun = new List<Element>();
            switch (_viewModel.DoiTuongUuTien)
            {
                case "CỘT":
                    allCategoryToRun = getAllCategoryToRun(BuiltInCategory.OST_StructuralColumns);
                    break;

                case "DẦM":
                    allCategoryToRun = getAllCategoryToRun(BuiltInCategory.OST_StructuralFraming);
                    break;
                case "SÀN":
                    allCategoryToRun = getAllCategoryToRun(BuiltInCategory.OST_Floors);
                    break;

                case "MÓNG":
                    allCategoryToRun = getAllCategoryToRun(BuiltInCategory.OST_StructuralFoundation);
                    break;
                case "TƯỜNG":
                    allCategoryToRun = getAllCategoryToRun(BuiltInCategory.OST_Walls);
                    break;
            }


            #endregion




            ProgressWindow.Maximum = allCategoryToRun.Count;

            tG.Start("Run process");

            double value = 0;

            #region Lọc các đối tượng bị cắt

            ElementCategoryFilter categoryFilter = null;
            switch (_viewModel.DoiTuongBiCat)
            {
                case "CỘT":
                    categoryFilter = new ElementCategoryFilter(BuiltInCategory.OST_StructuralColumns);
                    break;
                case "DẦM":
                    categoryFilter = new ElementCategoryFilter(BuiltInCategory.OST_StructuralFraming);
                    break;
                case "SÀN":
                    categoryFilter = new ElementCategoryFilter(BuiltInCategory.OST_Floors);
                    break;
                case "MÓNG":
                    categoryFilter = new ElementCategoryFilter(BuiltInCategory.OST_StructuralFoundation);
                    break;
                case "TƯỜNG":
                    categoryFilter = new ElementCategoryFilter(BuiltInCategory.OST_Walls);
                    break;
            }

            if (categoryFilter == null)
            {
                DialogResult = false;
                return;
            }
            ;


            #endregion

            foreach (Element doiTuongUuTien in allCategoryToRun)
            {
                if (tG.HasStarted())
                {
                    value = value + 1;
                    _viewModel.Percent = value / ProgressWindow.Maximum * 100;
                    ProgressWindow.Dispatcher.Invoke(() => ProgressWindow.Value = value,
                     DispatcherPriority.Background);


                    //Lấy về dầm đụng với floor trên active View

                    BoundingBoxXYZ box = doiTuongUuTien.get_BoundingBox(_viewModel.Doc.ActiveView);
                    Outline outline = new Outline(box.Min, box.Max);

                    BoundingBoxIntersectsFilter bbFilter = new BoundingBoxIntersectsFilter(outline);

                    LogicalAndFilter logicalAndFilter = new LogicalAndFilter(categoryFilter, bbFilter);

                    List<Element> allDoiTuongBiCat = new FilteredElementCollector(_viewModel.Doc, _viewModel.Doc.ActiveView.Id)
                        .WherePasses(logicalAndFilter)
                        .ToList();

                    using (Transaction trans = new Transaction(_viewModel.Doc))
                    {
                        trans.Start("Join" + _viewModel.DoiTuongUuTien + "với" + _viewModel.DoiTuongBiCat);

                        FailureHandlingOptions option = trans.GetFailureHandlingOptions();

                        option.SetFailuresPreprocessor(new DeleteWarningSuper());

                        trans.SetFailureHandlingOptions(option);

                        foreach (Element doiTuongBiCat in allDoiTuongBiCat)
                        {
                            if (JoinGeometryUtils.AreElementsJoined(_viewModel.Doc, doiTuongUuTien, doiTuongBiCat))
                            {
                                if (JoinGeometryUtils.IsCuttingElementInJoin(_viewModel.Doc, doiTuongBiCat,
                                    doiTuongUuTien))
                                {
                                    try
                                    {
                                        JoinGeometryUtils.SwitchJoinOrder(_viewModel.Doc, doiTuongUuTien,
                                            doiTuongBiCat);
                                    }
                                    catch (Exception ex)
                                    {

                                    }
                                }
                            }
                            else
                            {
                                JoinGeometryUtils.JoinGeometry(_viewModel.Doc, doiTuongUuTien, doiTuongBiCat);

                                if (JoinGeometryUtils.IsCuttingElementInJoin(_viewModel.Doc, doiTuongBiCat,
                                    doiTuongUuTien))
                                {
                                    try
                                    {
                                        JoinGeometryUtils.SwitchJoinOrder(_viewModel.Doc, doiTuongUuTien,
                                            doiTuongBiCat);
                                    }
                                    catch (Exception ex)
                                    {

                                    }
                                }
                            }
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

                MessageBox.Show("Đã Join " + _viewModel.DoiTuongUuTien + " và " + _viewModel.DoiTuongBiCat + " thành công!",
                    "Auto Join Successful!", MessageBoxButton.OK,
                    MessageBoxImage.Information);
            }


        }


        private List<Element> getAllCategoryToRun(BuiltInCategory builtInCategory)
        {
            List<Element> allCategoryToRun;
            if (_viewModel.IsEntireProject)
            {
                allCategoryToRun = new FilteredElementCollector(_viewModel.Doc)
                    .OfCategory(builtInCategory)
                    .WhereElementIsNotElementType()
                    .ToList();
            }
            else if (_viewModel.IsCurrentView)
            {
                allCategoryToRun = new FilteredElementCollector(_viewModel.Doc, _viewModel.Doc.ActiveView.Id)
                    .OfCategory(builtInCategory)
                    .WhereElementIsNotElementType()
                    .ToList();
            }
            else
            {
                allCategoryToRun = new FilteredElementCollector(_viewModel.Doc,
                        _viewModel.UiDoc.Selection
                            .GetElementIds())
                    .OfCategory(builtInCategory)
                    .ToList();
            }

            return allCategoryToRun;
        }
    }
}
