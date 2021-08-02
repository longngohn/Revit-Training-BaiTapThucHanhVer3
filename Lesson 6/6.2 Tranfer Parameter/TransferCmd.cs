
#region Namespaces

using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Application = Autodesk.Revit.ApplicationServices.Application;

#endregion

namespace AlphaBIM
{
    [Transaction(TransactionMode.Manual)]
    public class TransferCmd : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData,
            ref string message, ElementSet elements)
        {
            UIApplication uiapp = commandData.Application;
            UIDocument uidoc = uiapp.ActiveUIDocument;
            Application app = uiapp.Application;
            Document doc = uidoc.Document;

            //string dllFolder = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location));
            //AssemblyLoader.LoadAllRibbonAssemblies(dllFolder);
            using (TransactionGroup tranGroup = new TransactionGroup(doc))
            {
                tranGroup.Start("Transfer");
                TransferViewModel viewModel = new TransferViewModel(uidoc);
                TransferParameterWindow transferParameterWindow = new TransferParameterWindow(viewModel);

                bool? dialog = transferParameterWindow.ShowDialog();

                if (dialog == false) return Result.Cancelled;

                tranGroup.Assimilate();

            }

            return Result.Succeeded;
        }
    }
}
