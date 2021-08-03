﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Architecture;

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
                throw;
            }

        }

        /// <summary>
        /// Đường bao của Hatch sàn
        /// </summary>
        public CurveArray AllCurve { get; set; } = new CurveArray();
    }
}