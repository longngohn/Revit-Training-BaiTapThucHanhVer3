using Autodesk.Revit.DB;
using Autodesk.Revit.UI.Selection;

namespace AlphaBIM
{
    public class ColumnSelectionFilter : ISelectionFilter
    {
        public bool AllowElement(Element elem)
        {
            if (elem.Category == null)
            {
                return false;
            }
            return elem.Category.Id.IntegerValue.Equals((int)BuiltInCategory.OST_StructuralColumns);
        }

        public bool AllowReference(Reference reference, XYZ position)
        {
            return true;
        }
    }
}