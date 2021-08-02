
#region Namespaces
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Application = Autodesk.Revit.ApplicationServices.Application;
#endregion

namespace AlphaBIM
{
    [Transaction(TransactionMode.Manual)]
    public class Lesson08LoadFamilyCreateSymbolCmd : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData,
            ref string message, ElementSet elements)
        {
            UIApplication uiapp = commandData.Application;
            UIDocument uidoc = uiapp.ActiveUIDocument;
            Application app = uiapp.Application;
            Document doc = uidoc.Document;

            // code

            #region Load Family

            //using (Transaction t = new Transaction(doc, "Load Family"))
            //{
            //    t.Start();

            //    doc.LoadFamily(@"D:\Khoa_Hoc_Co_Ban\01 new 2020\04.Model Test\ALB_STR_Rectangular Concrete Beam.rfa");

            //    t.Commit();
            //}

            #endregion

            #region Create new Family Symbol

            ////Get Family Symbol
            //FilteredElementCollector collector = new FilteredElementCollector(doc);
            //FamilySymbol symbol = collector.OfClass(typeof(FamilySymbol))
            //    .Cast<FamilySymbol>()
            //    .FirstOrDefault(s => s.Family.Name.Equals("ALB_STR_Rectangular Concrete Beam"));


            //if (symbol!=null)
            //{
            //    using (Transaction t = new Transaction(doc))
            //    {
            //        t.Start("Create new Framing Type");

            //        ElementType s1 = symbol.Duplicate("new 400x800");
            //        s1.LookupParameter("b")?.Set(AlphaBIMUnitUtils.MmToFeet(400));
            //        s1.LookupParameter("h")?.Set(AlphaBIMUnitUtils.MmToFeet(800));

            //        t.Commit();
            //    }
            //}


            #endregion

            return Result.Succeeded;
        }
    }
}
