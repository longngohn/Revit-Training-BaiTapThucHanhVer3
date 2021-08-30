using Autodesk.Revit.DB;

namespace AlphaBIM
{
    internal static class Interactor
    {
        internal static Solid Union(this Solid solid1, Solid solid2)
        {
            // Gộp 2 solid lại
            Solid solid = BooleanOperationsUtils.ExecuteBooleanOperation(
                solid1,
                solid2,
                BooleanOperationsType.Union);

            return solid;
        }
    }
}