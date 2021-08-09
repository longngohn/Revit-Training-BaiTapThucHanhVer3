#region Namespaces

using System;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System.IO;
using System.Reflection;
using Application = Autodesk.Revit.ApplicationServices.Application;

#endregion

namespace AlphaBIM
{
    [Transaction(TransactionMode.Manual)]
    public class Lesson11RebarSingleBeamCmd : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData,
            ref string message, ElementSet elements)
        {
            UIApplication uiapp = commandData.Application;
            UIDocument uidoc = uiapp.ActiveUIDocument;
            Application app = uiapp.Application;
            Document doc = uidoc.Document;

            // Khi chạy bằng Add-in Manager hoặc Debug thì commnent 2 dòng bên dưới để tránh lỗi    
            //string dllFolder = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location));
            //AssemblyLoader.LoadAllRibbonAssemblies(dllFolder);


            using (TransactionGroup tranG = new TransactionGroup(doc))
            {
                tranG.Start("Rebar Single Beam");

                RebarSingleBeamViewModel viewModel
                    = new RebarSingleBeamViewModel(uidoc);

                RebarSingleBeamWindow window
                    = new RebarSingleBeamWindow(viewModel);

                if (window.ShowDialog() == false) return Result.Cancelled;

                tranG.Assimilate();
            }

            return Result.Succeeded;
        }
    }
}


