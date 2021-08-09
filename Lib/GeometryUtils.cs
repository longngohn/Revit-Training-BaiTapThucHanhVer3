#region Namespaces

using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
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

        public static List<Face> GetFaces(this Solid solid)
        {
            List<Face> faces = new List<Face>();

            FaceArray faceArray = solid.Faces;

            List<Face> planarFaces = new List<Face>();
            foreach (Face face in faceArray)
            {
                faces.Add(face);
            }

            return faces;
        }

        public static double CalAreaNotTop(this List<Face> faces)
        {
            double result = 0;
            faces = faces.Where(x => !x.GetNormal().IsAlmostEqualTo(XYZ.BasisZ)).ToList();
            foreach (Face face in faces)
            {
                result += face.Area;
            }

            return result;
        }

        public static double CalAreaOnlyBottom(this List<Face> faces)
        {
            double result = 0;
            faces = faces.Where(x => x.GetNormal().IsAlmostEqualTo(XYZ.BasisZ.Negate())).ToList();
            foreach (Face face in faces)
            {
                result += face.Area;
            }

            return result;
        }

        public static double CalAreaNotTopNotBottom(this List<Face> faces)
        {
            double result = 0;
            faces = faces
                .Where(x => !x.GetNormal().IsAlmostEqualTo(XYZ.BasisZ))
                .Where(x => !x.GetNormal().IsAlmostEqualTo(XYZ.BasisZ.Negate()))
                .ToList();
            foreach (Face face in faces)
            {
                result += face.Area;
            }

            return result;
        }

        public static XYZ GetNormal(this Face face)
        {
            BoundingBoxUV uv = face.GetBoundingBox();
            UV min = uv.Min;
            UV max = uv.Max;

            UV uvCenter = (min + max) / 2;

            XYZ computeNormal = face.ComputeNormal(uvCenter);
            return computeNormal;
        }


        public static Plane GetPlaneOfFace(this Face face)
        {
            List<XYZ> origin = face.Triangulate().Vertices.ToList();
            XYZ faceNormal = face.ComputeNormal(UV.Zero);
            return Plane.CreateByNormalAndOrigin(faceNormal, origin[0]);

        }
        /// <summary>
        /// Lấy độ dài của điểm tới mặt phẳng
        /// </summary>
        /// <param name="normalOfPlane"></param>
        /// <param name="originOfPlane"></param>
        /// <param name="anyPoint"></param>
        /// <returns></returns>
        internal static double GetLengthOfPointToPlane(XYZ normalOfPlane, XYZ originOfPlane, XYZ anyPoint)
        {
            // plane
            Plane plane = Plane.CreateByNormalAndOrigin(normalOfPlane, originOfPlane);
            double d = plane.Normal.DotProduct(-plane.Origin);
            // tu
            //double tu = Math.Abs(plane.Normal.DotProduct(AnyPoint) + d);
            double tu = plane.Normal.DotProduct(anyPoint) + d;

            // mau
            double mau = Math.Sqrt(plane.Normal.DotProduct(plane.Normal));
            return Math.Abs(tu / mau);
        }

        internal static void ToClipboard(this List<XYZ> xyzs)
        {
            string a = "[";
            foreach (XYZ xyz in xyzs)
            {
                a += "[" + xyz.X + "," + xyz.Y + "," + xyz.Z + "],\n";
            }

            try
            {
                a = a.Remove(a.Length - 2, 2);

            }
            catch (Exception e)
            {

            }
            a += "]";
            Clipboard.SetText(a);
        }
    }

}