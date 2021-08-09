
#region Namespaces

using System.IO;
using System.Reflection;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Application = Autodesk.Revit.ApplicationServices.Application;
#endregion

namespace AlphaBIM
{
    [Transaction(TransactionMode.Manual)]
    public class Lesson10_PurgeViewCmd : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData,
            ref string message, ElementSet elements)
        {
            UIApplication uiapp = commandData.Application;
            UIDocument uidoc = uiapp.ActiveUIDocument;
            Application app = uiapp.Application;
            Document doc = uidoc.Document;

            using (TransactionGroup tranG = new TransactionGroup(doc))
            {
                tranG.Start("Purge Views");

                PurgeViewModel viewModel 
                    = new PurgeViewModel(uidoc);

                PurgeModelWindow window 
                    = new PurgeModelWindow(viewModel);

                if (window.ShowDialog() == false) return Result.Cancelled;
                
                tranG.Assimilate();
            }

            return Result.Succeeded;
        }
    }
}
