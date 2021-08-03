
#region Namespaces

using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using System;
using Application = Autodesk.Revit.ApplicationServices.Application;
#endregion

namespace AlphaBIM
{
    [Transaction(TransactionMode.Manual)]
    public class Lesson08ChangeLocationCmd : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData,
            ref string message, ElementSet elements)
        {
            UIApplication uiapp = commandData.Application;
            UIDocument uidoc = uiapp.ActiveUIDocument;
            Application app = uiapp.Application;
            Document doc = uidoc.Document;

            // code here

            try
            {
                // Pick Object
                Reference r = uidoc.Selection.PickObject(ObjectType.Element);

                // Retrieve Element
                ElementId id = r.ElementId;
                Element e = doc.GetElement(id);

                using (Transaction trans = new Transaction(doc, "Change Location"))
                {
                    LocationPoint lp = e.Location as LocationPoint;

                    if (lp != null)
                    {
                        trans.Start();

                        XYZ newloc = uidoc.Selection.PickPoint();
                        lp.Point = newloc;

                        trans.Commit();
                    }

                }

                return Result.Succeeded;
            }
            catch (Exception e)
            {
                message = e.Message;
                return Result.Failed;
            }

        }
    }
}
