
#region Namespaces

using System.Collections.Generic;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Application = Autodesk.Revit.ApplicationServices.Application;
#endregion

namespace AlphaBIM
{
    [Transaction(TransactionMode.Manual)]
    public class Lesson10_FiltersCmd : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData,
            ref string message, ElementSet elements)
        {
            UIApplication uiapp = commandData.Application;
            UIDocument uidoc = uiapp.ActiveUIDocument;
            Application app = uiapp.Application;
            Document doc = uidoc.Document;

            // code here

            #region Create Rules Filter

            //List<ElementId> elementIds = new List<ElementId>();
            //elementIds.Add(new ElementId(BuiltInCategory.OST_StructuralColumns));

            //ElementId parameterId = new ElementId(BuiltInParameter.HOST_VOLUME_COMPUTED);
            //FilterRule equalsRule =
            //    ParameterFilterRuleFactory.CreateEqualsRule(parameterId,
            //        UnitUtils.ConvertToInternalUnits(3.726, DisplayUnitType.DUT_CUBIC_METERS),
            //        UnitUtils.ConvertToInternalUnits(0.01, DisplayUnitType.DUT_CUBIC_METERS));

            //ElementParameterFilter parameterFilter = new ElementParameterFilter(equalsRule);

            //using (Transaction trans = new Transaction(doc, "Apply Filter"))
            //{
            //    trans.Start();

            //    // Apply Filter
            //    ParameterFilterElement filter = ParameterFilterElement.Create(doc,
            //        "Columns Filter", elementIds, parameterFilter);

            //    doc.ActiveView.AddFilter(filter.Id);
            //    doc.ActiveView.SetFilterVisibility(filter.Id, false);

            //    trans.Commit();
            //}

            #endregion

            #region Create Selection Filter

            //using (Transaction trans = new Transaction(doc, "Apply Filter"))
            //{
            //    trans.Start();

            //    SelectionFilterElement selectionFilter
            //        = SelectionFilterElement.Create(doc, "new Selection Filter2");
            //    selectionFilter.AddSet(uidoc.Selection.GetElementIds());

            //    doc.ActiveView.AddFilter(selectionFilter.Id);
            //    doc.ActiveView.SetFilterVisibility(selectionFilter.Id, false);

            //    trans.Commit();
            //}

            #endregion

            #region Delete all Filter in View

            //using (Transaction trans = new Transaction(doc, "Delete Filter"))
            //{
            //    trans.Start();

            //    ICollection<ElementId> elementIds = doc.ActiveView.GetFilters();
            //    foreach (var id in elementIds)
            //    {
            //        doc.ActiveView.RemoveFilter(id);
            //    }

            //    trans.Commit();
            //}

            #endregion

            return Result.Succeeded;
        }
    }
}
