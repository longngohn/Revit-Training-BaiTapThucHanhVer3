using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using Autodesk.Revit.DB;

namespace AlphaBIM
{
    internal class CreateDirectShape
    {
        public static void Execute(
          Document doc,
          Face face,
          string name)
        {
            TessellatedShapeBuilder builder = new TessellatedShapeBuilder();

            builder.OpenConnectedFaceSet(false);

            Mesh mesh = face.Triangulate();

            XYZ[] triangleCorners = new XYZ[3];

            List<TessellatedFace> tessellatedFaces = new List<TessellatedFace>();

            for (int i = 0; i < mesh.NumTriangles; i++)
            {
                MeshTriangle triangle = mesh.get_Triangle(i);

                triangleCorners[0] = triangle.get_Vertex(0);
                triangleCorners[1] = triangle.get_Vertex(1);
                triangleCorners[2] = triangle.get_Vertex(2);

                //bool inside = element.PointInside(triangleCorners.ToList());
                //if (inside)
                //{

                //}

                TessellatedFace tesseFace
                    = new TessellatedFace(triangleCorners,
                        ElementId.InvalidElementId);
                tessellatedFaces.Add(tesseFace);

                //if (builder.DoesFaceHaveEnoughLoopsAndVertices(
                //    tesseFace))
                //{
                //    builder.AddFace(tesseFace);
                //}

            }

            if (tessellatedFaces.Count >= 1)
            {

                foreach (TessellatedFace tessellatedFace in tessellatedFaces)
                {
                    if (builder.DoesFaceHaveEnoughLoopsAndVertices(
                        tessellatedFace))
                    {
                        builder.AddFace(tessellatedFace);
                    }
                }


                builder.CloseConnectedFaceSet();


                builder.Target = TessellatedShapeBuilderTarget.AnyGeometry; // 2018
                builder.Fallback = TessellatedShapeBuilderFallback.Mesh; // 2018

                builder.Build(); // 2018

                TessellatedShapeBuilderResult result = builder.GetBuildResult(); // 2018
                IList<GeometryObject> geomArr = result.GetGeometricalObjects();

                ElementId categoryId = new ElementId(
                    BuiltInCategory.OST_SpecialityEquipment);

                //DirectShapeType directShapeType=DirectShapeType.Create(doc,name,categoryId);

                DirectShape ds = DirectShape.CreateElement(
                    doc, categoryId); // 2018

                //ds.ChangeTypeId(ds.Id);

                ds.ApplicationId = Assembly.GetExecutingAssembly()
                    .GetType().GUID.ToString(); // 2018

                ds.ApplicationDataId = Guid.NewGuid().ToString(); // 2018

                ds.SetShape(geomArr);

                ds.Name = "ALB_FormWork";
              


            }

        }
    }

}