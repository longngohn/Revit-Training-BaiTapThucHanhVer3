
#region Namespaces

using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System.Collections.Generic;
using Application = Autodesk.Revit.ApplicationServices.Application;
#endregion

namespace AlphaBIM
{
    [Transaction(TransactionMode.Manual)]
    public class Lesson08CreateLineBasedElementsCmd : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData,
            ref string message, ElementSet elements)
        {
            UIApplication uiapp = commandData.Application;
            UIDocument uidoc = uiapp.ActiveUIDocument;
            Application app = uiapp.Application;
            Document doc = uidoc.Document;

            // code here
            #region Create Framing

            //XYZ pt1 = uidoc.Selection.PickPoint("Pick start point...");
            //XYZ pt2 = uidoc.Selection.PickPoint("Pick end point...");
            //double z = doc.ActiveView.GenLevel.Elevation;
            //XYZ startPoint = new XYZ(pt1.X, pt1.Y, z);
            //XYZ endPoint = new XYZ(pt2.X, pt2.Y, z);
            //Line framingLocation = Line.CreateBound(startPoint, endPoint);

            //FamilySymbol familySymbol = new FilteredElementCollector(doc)
            //    .OfClass(typeof(FamilySymbol))
            //    .Cast<FamilySymbol>()
            //    .Where(s => s.Family.Name.Equals("M_Concrete-Rectangular Beam"))
            //    .FirstOrDefault(s => s.Name.Equals("220 x 200"));

            ////if (!familySymbol.IsActive) familySymbol.Activate();

            //Level level = doc.ActiveView.GenLevel;
            ////Level level = new FilteredElementCollector(doc)
            ////    .OfClass(typeof(Level))
            ////    .Cast<Level>()
            ////    .FirstOrDefault(l => l.Name.Equals("Level 1"));

            //using (Transaction t = new Transaction(doc))
            //{
            //    t.Start("Create Framing");

            //    FamilyInstance instance = doc.Create.NewFamilyInstance(framingLocation,
            //        familySymbol, level, StructuralType.Beam);

            //    instance.get_Parameter(BuiltInParameter.ALL_MODEL_INSTANCE_COMMENTS)
            //       .Set("Create Framing from Revit API");
            //    //instance.get_Parameter(BuiltInParameter.INSTANCE_REFERENCE_LEVEL_PARAM)
            //    //   .Set(level.Id);

            //    t.Commit();
            //}

            #endregion Create Framing

            #region Create Wall

            //XYZ pt1 = uidoc.Selection.PickPoint("Pick start point...");
            //XYZ pt2 = uidoc.Selection.PickPoint("Pick end point...");
            //double z = doc.ActiveView.GenLevel.Elevation;
            //XYZ startPoint = new XYZ(pt1.X, pt1.Y, z);
            //XYZ endPoint = new XYZ(pt2.X, pt2.Y, z);

            //Curve wallLocation = Line.CreateBound(startPoint, endPoint);

            //ElementId wallTypeId = new FilteredElementCollector(doc)
            //    .OfClass(typeof(WallType))
            //    .Cast<WallType>()
            //    .FirstOrDefault(t => t.Name.Equals("W200"))?.Id;

            //ElementId levelId = doc.ActiveView.GenLevel.Id;
            //double height = AlphaBIMUnitUtils.MeterToFeet(6);

            //using (Transaction t = new Transaction(doc))
            //{
            //    t.Start("Create Wall");

            //    Wall instance = Wall.Create(doc,
            //        wallLocation,
            //        wallTypeId,
            //        levelId,
            //        height,
            //        0,
            //        false,
            //        true);

            //    instance.get_Parameter(BuiltInParameter.ALL_MODEL_INSTANCE_COMMENTS)
            //        .Set("Create Wall from Revit API");
            //    t.Commit();
            //}

            #endregion

            #region Create Grid

            //ObjectSnapTypes snapSettings = ObjectSnapTypes.Endpoints;

            //XYZ point1 = uidoc.Selection.PickPoint(snapSettings, "Pick start point");
            //XYZ point2 = uidoc.Selection.PickPoint("Pick second point");

            //Line line = Line.CreateBound(point1, point2);

            //using (Transaction t = new Transaction(doc))
            //{
            //    t.Start("Create Grid from API");

            //    Grid grid1 = Grid.Create(doc, line);
            //    grid1.Name = "new Grid";

            //    t.Commit();
            //}

            #endregion

            #region Create Level

            //using (Transaction t = new Transaction(doc))
            //{
            //    t.Start("Create Level");

            //    double elevation = UnitUtils.ConvertToInternalUnits(5, DisplayUnitType.DUT_METERS);
            //    Level level = Level.Create(doc, elevation);

            //    level.Name = "Level 100 - from API";

            //    t.Commit();
            //}

            #endregion

            #region Create Detail Line

            XYZ first = uidoc.Selection.PickPoint("Pick start point...");
            XYZ second = uidoc.Selection.PickPoint("Pick end point...");
            double z = doc.ActiveView.GenLevel.Elevation;

            Line line = Line.CreateBound(first, second);

            using (Transaction tran = new Transaction(doc))
            {
                tran.Start("Create Line from Point");

                DetailCurve detailCurve =
                    doc.Create.NewDetailCurve(doc.ActiveView, line);
                DetailCurve detailCurve2 =
                    doc.Create.NewDetailCurve(doc.ActiveView, line);

                //ICollection<ElementId> elementIds = new List<ElementId>();
                //elementIds.Add(detailCurve.Id);
                //uidoc.Selection.SetElementIds(elementIds);

                uidoc.Selection.SetElementIds(new List<ElementId>()
                {
                    detailCurve.Id,
                    detailCurve2.Id,
                });

                tran.Commit();
            }

            #endregion

            return Result.Succeeded;
        }
    }
}
