using System.Collections.Generic;
using System.Linq;
using Autodesk.Revit.DB;

namespace AlphaBIM
{
    public class FloorData
    {
        public FloorData(PlanarFace planarFace)
        {
            
            EdgeArrayArray edgeLoops = planarFace.EdgeLoops;

            foreach (EdgeArray edgeArray in edgeLoops)
            {
                foreach (Edge edge in edgeArray)
                {
                    AllCurve.Append(edge.AsCurve());
                }
            }
//a
          
        }

        /// <summary>
        /// đơn vị: feet
        /// </summary>
        public CurveArray AllCurve { get; set; }
    }
}