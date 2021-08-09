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
    public class Lesson10_FilterSelectorCmd : IExternalCommand
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

                FilterSelectorViewModel viewModel
                    = new FilterSelectorViewModel(uidoc);

                FilterSelectorWindow window
                    = new FilterSelectorWindow(viewModel);

                if (window.ShowDialog() == false) return Result.Cancelled;

                tranG.Assimilate();
            }

            return Result.Succeeded;
        }
    }
}


