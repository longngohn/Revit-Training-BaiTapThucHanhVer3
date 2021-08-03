using Autodesk.Revit.DB;
using System;
using System.Windows.Forms;

namespace AlphaBIM
{
    public class FloorData
    {
        public FloorData(PlanarFace planarFace)
        {

            try
            {
                EdgeArrayArray edgeLoops = planarFace.EdgeLoops;

                foreach (EdgeArray edgeArray in edgeLoops)
                {
                    foreach (Edge edge in edgeArray)
                    {
                        Curve newCur = edge.AsCurve();
                        AllCurve.Append(newCur);
                    }
                }
            }
            catch (Exception e)
            {
                MessageBox.Show(e.ToString());
            }

        }
        /// <summary>
        /// Đường bao của Hatch sàn
        /// </summary>
        public CurveArray AllCurve { get; set; } = new CurveArray();
    }
}