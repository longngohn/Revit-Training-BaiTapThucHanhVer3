using Autodesk.Revit.DB;
using System;
using System.Collections.Generic;

namespace AlphaBIM
{
    public class AlphaComparer : IComparer<View>,
        IComparer<ViewType>
    {
        public int Compare(View x, View y)
        {
            return string.Compare(x.Name, y.Name, StringComparison.Ordinal);
        }

        public int Compare(ViewType x, ViewType y)
        {
            return string.Compare(x.ToString(), y.ToString(), StringComparison.Ordinal);
        }

        public int Compare(ViewExtension x, ViewExtension y)
        {
            return string.Compare(x.View.Name, y.View.Name, StringComparison.Ordinal);
        }
    }
}