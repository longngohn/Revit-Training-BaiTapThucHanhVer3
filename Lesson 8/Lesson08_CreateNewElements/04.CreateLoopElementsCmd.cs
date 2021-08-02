
#region Namespaces
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System.Linq;
using Application = Autodesk.Revit.ApplicationServices.Application;
#endregion

namespace AlphaBIM
{
    [Transaction(TransactionMode.Manual)]
    public class Lesson08CreateLoopElementsCmd : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData,
            ref string message, ElementSet elements)
        {
            UIApplication uiapp = commandData.Application;
            UIDocument uidoc = uiapp.ActiveUIDocument;
            Application app = uiapp.Application;
            Document doc = uidoc.Document;

            // code here

            #region Create Floor

            // Create Points 
            XYZ p1 = uidoc.Selection.PickPoint("Pick first point...");
            XYZ p2 = uidoc.Selection.PickPoint("Pick second point...");
            XYZ p3 = uidoc.Selection.PickPoint("Pick third point...");
            XYZ p4 = uidoc.Selection.PickPoint("Pick 4th point...");
            XYZ p5 = uidoc.Selection.PickPoint("Pick 5th point...");

            // Create Curves
            Line l1 = Line.CreateBound(p1, p2);
            Arc l2 = Arc.Create(p2, p4, p3);
            Line l3 = Line.CreateBound(p4, p5);
            Line l4 = Line.CreateBound(p5, p1);


            // Create CurveArray
            CurveArray profile = new CurveArray();
            profile.Append(l1);
            profile.Append(l2);
            profile.Append(l3);
            profile.Append(l4);


            // Get Floor Type
            FloorType floorType = new FilteredElementCollector(doc)
                .OfClass(typeof(FloorType))
                .Cast<FloorType>()
                .FirstOrDefault(type => type.get_Parameter(BuiltInParameter.ELEM_TYPE_PARAM)
                    .AsValueString()
                    .Equals("S 200"));

            Level level = doc.ActiveView.GenLevel;
            XYZ normal = XYZ.BasisZ;

            using (Transaction tran = new Transaction(doc))
            {
                tran.Start("Create Floor");

                Floor instance = doc.Create.NewFloor(profile,
                    floorType, level, true, normal);

                instance.get_Parameter(BuiltInParameter.ALL_MODEL_INSTANCE_COMMENTS)
                    .Set("Create Floor from Revit API");

                tran.Commit();
            }

            #endregion

            return Result.Succeeded;
        }
    }
}
