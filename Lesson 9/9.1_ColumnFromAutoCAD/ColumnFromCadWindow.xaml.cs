using System;
using System.Collections.Generic;
using Autodesk.Revit.DB;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Threading;
using Autodesk.Revit.DB.Structure;
using MessageBox = System.Windows.Forms.MessageBox;

namespace AlphaBIM
{
    public partial class ColumnFromCadWindow
    {
        private ColumnFromCadViewModel _viewModel;
        private TransactionGroup transG;

        public ColumnFromCadWindow(ColumnFromCadViewModel viewModel)
        {
            InitializeComponent();
            _viewModel = viewModel;
            DataContext = _viewModel;

            transG = new TransactionGroup(_viewModel.Doc);
        }


        private void btnOk_Click(object sender, RoutedEventArgs e)
        {
            #region Lấy về maximum những element cần chạy

            List<PlanarFace> hatchToCreateColumn =
                CadUtils.GetHatchHaveName(_viewModel.SelectedCadLink,
                    _viewModel.SelectedLayer);

            List<ColumnData> allColumnsData = new List<ColumnData>();

            foreach (PlanarFace hatch in hatchToCreateColumn)
            {
                ColumnData columnData = new ColumnData(hatch);
                allColumnsData.Add(columnData);
            }

            #endregion

            ProgressWindow.Maximum = allColumnsData.Count;
            transG.Start("Run Process");

            #region Code

            List<ElementId> newColumns = new List<ElementId>();
            double value = 0;

            foreach (ColumnData columnData in allColumnsData)
            {
                if (transG.HasStarted())
                {
                    #region Setup cho ProgressBar nhảy % tiến trình

                    value = value + 1;

                    try
                    {
                        Show();
                    }
                    catch (Exception)
                    {
                        Close();
                        if (transG.HasStarted()) transG.RollBack();
                        System.Windows.MessageBox.Show("Progress is Cancel!", "Stop Progress",
                            MessageBoxButton.OK, MessageBoxImage.Stop);
                        break;
                    }

                    _viewModel.Percent
                        = value / ProgressWindow.Maximum * 100;

                    ProgressWindow.Dispatcher?.Invoke(() => ProgressWindow.Value = value,
                        DispatcherPriority.Background);

                    #endregion

                    #region Viết Code ở đây

                    FamilySymbol familySymbol
                        = FamilySymbolUtils.GetFamilySymbolColumn(_viewModel.SelectedFamilyColumn,
                            columnData.CanhNgan,
                            columnData.CanhDai,
                            "b", "h");

                    if (familySymbol == null) continue;

                    using (Transaction tran = new Transaction(_viewModel.Doc, "Create Column"))
                    {
                        tran.Start();

                        DeleteWarningSuper warningSuper = new DeleteWarningSuper();
                        FailureHandlingOptions failOpt = tran.GetFailureHandlingOptions();
                        failOpt.SetFailuresPreprocessor(warningSuper);
                        tran.SetFailureHandlingOptions(failOpt);


                        FamilyInstance instance = _viewModel.Doc.Create
                            .NewFamilyInstance(columnData.TamCot,
                            familySymbol, _viewModel.BaseLevel, StructuralType.Column);


                        instance.get_Parameter(BuiltInParameter.FAMILY_BASE_LEVEL_PARAM)
                            .Set(_viewModel.BaseLevel.Id);

                        instance.get_Parameter(BuiltInParameter.FAMILY_TOP_LEVEL_PARAM)
                            .Set(_viewModel.TopLevel.Id);


                        instance.get_Parameter(BuiltInParameter.FAMILY_BASE_LEVEL_OFFSET_PARAM)
                            .Set(AlphaBimUnitUtils.MmToFeet(_viewModel.BaseOffset));

                        instance.get_Parameter(BuiltInParameter.FAMILY_TOP_LEVEL_OFFSET_PARAM)
                            .Set(AlphaBimUnitUtils.MmToFeet(_viewModel.TopOffset));

                        Line axis = Line.CreateUnbound(columnData.TamCot, XYZ.BasisZ);

                        ElementTransformUtils.RotateElement(_viewModel.Doc,
                            instance.Id,
                            axis,
                            columnData.GocXoay);

                        newColumns.Add(instance.Id);
                        tran.Commit();

                    }

                    #endregion
                }
                else
                {
                    break;
                }
            }

            #endregion

            if (transG.HasStarted())
            {
                transG.Commit();
                DialogResult = true;

                MessageBox.Show(string.Concat("Success: ", newColumns.Count, " elements!"),
                    "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);

                _viewModel.UiDoc.Selection.SetElementIds(newColumns);
            }
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            if (transG.HasStarted())
            {
                DialogResult = false;
                transG.RollBack();
                System.Windows.MessageBox.Show("Progress is Cancel!", "Stop Progress",
                    MessageBoxButton.OK, MessageBoxImage.Stop);
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
