using Autodesk.Revit.DB;
using Autodesk.Revit.UI.Selection;

namespace AlphaBIM
{
    public class BeamColumnWallSelectionFilter : ISelectionFilter
    {
        public bool AllowElement(Element elem)
        {
            if (elem.Category == null)
            {
                return false;
            }
            return elem.Category.Id.IntegerValue.Equals((int)BuiltInCategory.OST_StructuralColumns) ||
                   elem.Category.Id.IntegerValue.Equals((int)BuiltInCategory.OST_StructuralFraming) ||
                   elem.Category.Id.IntegerValue.Equals((int)BuiltInCategory.OST_Walls);
        }

        public bool AllowReference(Reference reference, XYZ position)
        {
            return true;
        }
    }
}