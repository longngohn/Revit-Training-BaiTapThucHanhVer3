using System;
using System.Collections.Generic;
using Autodesk.Revit.DB;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Threading;
using Autodesk.Revit.DB.Structure;
using MessageBox = System.Windows.Forms.MessageBox;

namespace AlphaBIM
{
    public partial class FloorFromCadWindow
    {
        private FloorFromCadViewModel _viewModel;
        private TransactionGroup transG;

        public FloorFromCadWindow(FloorFromCadViewModel viewModel)
        {
            InitializeComponent();
            _viewModel = viewModel;
            DataContext = _viewModel;

            transG = new TransactionGroup(_viewModel.Doc);
        }


        private void btnOk_Click(object sender, RoutedEventArgs e)
        {
            #region Lấy về maximum những element cần chạy

            List<PlanarFace> hatchToCreateFloor =
                CadUtils.GetHatchHaveName(_viewModel.SelectedCadLink,
                    _viewModel.SelectedLayer);

            List<FloorData> allFloorData = new List<FloorData>();

            foreach (PlanarFace hatch in hatchToCreateFloor)
            {
                FloorData floorData = new FloorData(hatch);
                allFloorData.Add(floorData);
            }

            #endregion

            ProgressWindow.Maximum = allFloorData.Count;
            transG.Start("Run Process");

            #region Code

            List<ElementId> newFloors = new List<ElementId>();
            double value = 0;

            foreach (FloorData floorData in allFloorData)
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

                    FloorType floorType  = _viewModel.SelectedFloorType;
                           

                    if (floorType == null) continue;

                    using (Transaction tran = new Transaction(_viewModel.Doc, "Create Floor"))
                    {
                        tran.Start();

                        DeleteWarningSuper warningSuper = new DeleteWarningSuper();
                        FailureHandlingOptions failOpt = tran.GetFailureHandlingOptions();
                        failOpt.SetFailuresPreprocessor(warningSuper);
                        tran.SetFailureHandlingOptions(failOpt);


                            XYZ normal = XYZ.BasisZ;

                            Floor floor = _viewModel.Doc.Create.NewFloor(floorData.AllCurve,floorType,_viewModel.BaseLevel,_viewModel.IsStructural, XYZ.BasisZ);

                            floor.get_Parameter(BuiltInParameter.LEVEL_PARAM)
                                .Set(_viewModel.BaseLevel.Id);

                            floor.get_Parameter(BuiltInParameter.FLOOR_HEIGHTABOVELEVEL_PARAM)
                                .Set(AlphaBimUnitUtils.MmToFeet(AlphaBimUnitUtils.FeetToMm(_viewModel.LevelOffset)));

                            newFloors.Add(floor.Id);
                        




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

                MessageBox.Show(string.Concat("Success: ", newFloors.Count, " elements!"),
                    "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);

                _viewModel.UiDoc.Selection.SetElementIds(newFloors);
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
