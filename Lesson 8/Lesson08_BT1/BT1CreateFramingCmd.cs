using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;

namespace AlphaBIM
{
    [Transaction(TransactionMode.Manual)]
    public class Bt1CreateFramingCmd : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData,
            ref string message, ElementSet elements)
        {
            UIApplication uiapp = commandData.Application;
            UIDocument uidoc = uiapp.ActiveUIDocument;
            Application app = uiapp.Application;
            Document doc = uidoc.Document;

            // code

            using (TransactionGroup transG = new TransactionGroup(doc))
            {
                transG.Start("Create Framing from Family");

                Bt1CreateFramingViewModel viewModel
                    = new Bt1CreateFramingViewModel(uidoc);

                Bt1CreateFramingWpf window
                    = new Bt1CreateFramingWpf(viewModel);

                bool? showDialog = window.ShowDialog();

                if (showDialog == null || showDialog == false)
                {
                    transG.RollBack();
                    return Result.Cancelled;
                }

                transG.Assimilate();
                return Result.Succeeded;
            }
        }
      

    }
}
