
#region Namespaces

using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Structure;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using System.Linq;
using Application = Autodesk.Revit.ApplicationServices.Application;
#endregion

namespace AlphaBIM
{
    [Transaction(TransactionMode.Manual)]
    public class Lesson08CreateBasePointElementsCmd : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData,
            ref string message, ElementSet elements)
        {
            UIApplication uiapp = commandData.Application;
            UIDocument uidoc = uiapp.ActiveUIDocument;
            Application app = uiapp.Application;
            Document doc = uidoc.Document;

            // code here

            #region Create Column

            //XYZ tamCot = uidoc.Selection.PickPoint("Pick center point of Column");

            //FamilySymbol symbol
            //    = new FilteredElementCollector(doc)
            //        .OfClass(typeof(FamilySymbol))
            //        .OfCategory(BuiltInCategory.OST_StructuralColumns)
            //        .Cast<FamilySymbol>()
            //        .FirstOrDefault();
            //// if (!symbol.IsActive) symbol.Activate();

            //Level level = doc.ActiveView.GenLevel;

            //using (Transaction tran = new Transaction(doc))
            //{
            //    tran.Start("Create Column");
            //    FamilyInstance instance = doc.Create.NewFamilyInstance(tamCot,
            //        symbol, level, StructuralType.Column);
            //    instance.get_Parameter(BuiltInParameter.ALL_MODEL_INSTANCE_COMMENTS)
            //        .Set("Create Column from Revit API");
            //    tran.Commit();
            //}

            #endregion

            #region Create Door

            XYZ location = uidoc.Selection.PickPoint("Chọn điểm đặt cửa");
            FamilySymbol symbol
                = new FilteredElementCollector(doc)
                    .OfClass(typeof(FamilySymbol))
                    .OfCategory(BuiltInCategory.OST_Doors)
                    .Cast<FamilySymbol>().FirstOrDefault();

            Reference r = uidoc.Selection.PickObject(ObjectType.Element, "Chọn host của Door");
            Element host = doc.GetElement(r);

            Level level = doc.ActiveView.GenLevel;

            using (Transaction tran = new Transaction(doc))
            {
                tran.Start("Create Door");

                if (!symbol.IsActive) symbol.Activate();

                FamilyInstance instance
                    = doc.Create.NewFamilyInstance(location, symbol,
                        host, level, StructuralType.NonStructural);

                instance.get_Parameter(BuiltInParameter.ALL_MODEL_INSTANCE_COMMENTS)
                    .Set("Create Door from Revit API");

                tran.Commit();
            }

            #endregion

            return Result.Succeeded;
        }
    }
}
