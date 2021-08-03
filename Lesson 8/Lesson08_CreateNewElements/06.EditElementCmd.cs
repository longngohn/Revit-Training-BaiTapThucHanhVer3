
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
    public class Lesson08EditElementCmd : IExternalCommand
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

                using (Transaction trans = new Transaction(doc, "Edit Elements"))
                {
                    trans.Start();

                    // Move Element
                    XYZ moveVec = new XYZ(10, 10, 0);
                    ElementTransformUtils.MoveElement(doc, id, moveVec);

                    // Rotate Element
                    LocationPoint lc = e.Location as LocationPoint;

                    XYZ p1 = lc.Point;
                    XYZ p2 = new XYZ(p1.X, p1.Y, p1.Z + 1);
                    Line axis = Line.CreateBound(p1, p2);

                    double angle = 60 * Math.PI / 180;

                    ElementTransformUtils.RotateElement(doc, id, axis, angle);

                    trans.Commit();
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
