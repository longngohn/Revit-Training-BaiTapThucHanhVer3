#region Namespaces
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Application = Autodesk.Revit.ApplicationServices.Application;
#endregion

namespace AlphaBIM
{
    [Transaction(TransactionMode.Manual)]
    public class Lesson09_FloorFromCadCmd : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData,
            ref string message, ElementSet elements)
        {
            UIApplication uiapp = commandData.Application;
            UIDocument uidoc = uiapp.ActiveUIDocument;
            Application app = uiapp.Application;
            Document doc = uidoc.Document;

            // khi chạy bằng add-in manager hoặc debug thì comment 2 dòng bên dưới
            //string dllFolder = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location));
            //AssemblyLoader.LoadAllRibbonAssemblies(dllFolder);

            // code here

            using (TransactionGroup transGr = new TransactionGroup(doc))
            {
                transGr.Start("Model Wall from AutoCAD");

                WallFromCadViewModel viewModel = new WallFromCadViewModel(uidoc);

                WallFromCadWindow window = new WallFromCadWindow(viewModel);

                if (window.ShowDialog() == false) return Result.Cancelled;


                transGr.Assimilate();
            }

            return Result.Succeeded;

        }
    }
}
