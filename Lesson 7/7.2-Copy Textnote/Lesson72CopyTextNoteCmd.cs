
#region Namespaces

using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Application = Autodesk.Revit.ApplicationServices.Application;

#endregion

namespace AlphaBIM
{
    [Transaction(TransactionMode.Manual)]
    public class Lesson72CopyTextNoteCmd : IExternalCommand
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
            Lesson72CopyTextNoteViewModel lesson72CopyTextNoteViewModel = new Lesson72CopyTextNoteViewModel(uidoc);
            Lesson72CopyTextNoteWpf Lesson72CopyTextNoteWpf = new Lesson72CopyTextNoteWpf(lesson72CopyTextNoteViewModel);

            using (TransactionGroup tranG = new TransactionGroup(doc))
            {
                tranG.Start("Auto Join");


                bool? showDialog = Lesson72CopyTextNoteWpf.ShowDialog();

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
