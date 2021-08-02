using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;

namespace AlphaBIM 

{
    public class TransferLocSan : ISelectionFilter
    {


        //private Document _doc;
        public bool AllowElement(Element elem)
        {

            int idIntegerValue = elem.Category.Id.IntegerValue;
            if (idIntegerValue == (int)BuiltInCategory.OST_Floors)
            {

                return true;

            }


            return false;

        }

        public bool AllowReference(Reference reference, XYZ position)
        {
            //Code test alway true!
            //Element e = _doc.GetElement(reference);
            //GeometryObject geometryObject = e.GetGeometryObjectFromReference(reference);
            //Face face = geometryObject as Face;
            //if (face is CylindricalFace)
            //{
            //    return true;
            //}

            return true;

        }
           
        
    }
}
