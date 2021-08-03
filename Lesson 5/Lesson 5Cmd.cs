
#region Namespaces

using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System.Collections.Generic;
using System.Linq;
using Application = Autodesk.Revit.ApplicationServices.Application;

#endregion

namespace AlphaBIM
{
    [Transaction(TransactionMode.Manual)]
    public class Lesson_5Cmd : IExternalCommand
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

            string path = @"C:\ALB_ShareParameter.txt";
            string group = "Revit API Course";
            string name = "Alb_Volumn";
            string description = "Gán giá trị Volumn vào đối tượng Structure Framing";

            List<Category> categoriesList = new List<Category>();

            categoriesList.Add(Category.GetCategory(doc, BuiltInCategory.OST_StructuralFraming));

            Transaction tr = new Transaction(doc, "Tao Shared Parameter");

            tr.Start();
            ParameterUtils.CreateSharedParamater(doc,
                app,
                path,
                group,
                name,
                ParameterType.Volume,
                BuiltInParameterGroup.PG_TEXT,
                description,
                categoriesList,
                true,
                true,
                true
            );

            List<ElementId> elementIds = new FilteredElementCollector(doc, doc.ActiveView.Id)
                .OfCategory(BuiltInCategory.OST_StructuralFraming)
                .ToElementIds()
                .ToList();


            foreach (ElementId eId in elementIds)
            {
                Element element = doc.GetElement(eId);
                Parameter albVolumn = element.LookupParameter("Alb_Volumn");
                albVolumn.Set(UnitUtils.ConvertToInternalUnits(100, UnitTypeId.CubicMeters));
            };


            tr.Commit();








            return Result.Succeeded;
        }
    }
}
