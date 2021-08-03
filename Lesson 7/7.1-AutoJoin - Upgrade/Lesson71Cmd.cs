
#region Namespaces

using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Application = Autodesk.Revit.ApplicationServices.Application;

#endregion

namespace AlphaBIM
{
    [Transaction(TransactionMode.Manual)]
    public class Lesson71Cmd : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData,
            ref string message, ElementSet elements)
        {
            UIApplication uiapp = commandData.Application;
            UIDocument uidoc = uiapp.ActiveUIDocument;
            Application app = uiapp.Application;
            Document doc = uidoc.Document;

            // Khi chạy bằng Add-in Manager thì comment 2 dòng bên dưới để tránh lỗi
            // When running with Add-in Manager, comment the 2 lines below to avoid errors
            //string dllFolder = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location));
            //AssemblyLoader.LoadAllRibbonAssemblies(dllFolder);

            // code here
            Lesson71ViewModel lesson71ViewModel = new Lesson71ViewModel(uidoc);
            Lesson71Wpf lesson71Wpf = new Lesson71Wpf(lesson71ViewModel);

            using (TransactionGroup tranG = new TransactionGroup(doc))
            {
                tranG.Start("Auto Join");


                bool? showDialog = lesson71Wpf.ShowDialog();

                if (showDialog == null || showDialog == false)
                {
                    tranG.RollBack();
                    return Result.Cancelled;
                }

                tranG.Assimilate();
                return Result.Succeeded;

            }

        }
    }
}
