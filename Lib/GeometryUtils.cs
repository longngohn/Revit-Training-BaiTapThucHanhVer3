#region Namespaces

using System.Collections.Generic;
using System.Linq;
using Autodesk.Revit.DB;

#endregion

namespace AlphaBIM
{
    public static class GeometryUtils
    {
        public static PlanarFace GetPlanarFaceBottom(Solid solid)
        {
            foreach (Face f in solid.Faces)
            {
                if (f.ComputeNormal(new UV())
                    .Negate().IsAlmostEqualTo(XYZ.BasisZ))
                {
                    return f as PlanarFace;
                }
            }

            return null;
        }

        public static Plane GetPlaneOfFace(this Face face)
        {
            List<XYZ> origin = face.Triangulate().Vertices.ToList();
            XYZ faceNormal = face.ComputeNormal(UV.Zero);
            return Plane.CreateByNormalAndOrigin(faceNormal, origin[0]);

        }
    }
}