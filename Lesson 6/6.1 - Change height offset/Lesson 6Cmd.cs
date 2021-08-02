
#region Namespaces

using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Application = Autodesk.Revit.ApplicationServices.Application;

#endregion

namespace AlphaBIM
{
    [Transaction(TransactionMode.Manual)]
    public class Lesson6Cmd : IExternalCommand
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
                tranGroup.Start("Lesson 6");
                Lesson6ViewModel viewModel = new Lesson6ViewModel(uidoc);
                Lesson6Wpf windowWpf = new Lesson6Wpf(viewModel);

                bool? dialog = windowWpf.ShowDialog();

                if (dialog == false) return Result.Cancelled;

                tranGroup.Assimilate();

            }





            // code here


            return Result.Succeeded;
        }
    }
}
