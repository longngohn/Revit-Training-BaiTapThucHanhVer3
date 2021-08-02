using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Structure;
using Autodesk.Revit.UI;

namespace AlphaBIM
{
    public class Bt3CreateFloorViewModel : ViewModelBase
    {
        public UIDocument UiDoc;
        public Document Doc;
        public Bt3CreateFloorViewModel(UIDocument uiDoc)
        {
            // Khởi tạo sự kiện(nếu có) | Initialize event (if have)

            // Lưu trữ data từ Revit | Store data from Revit
            UiDoc = uiDoc;
            Doc = UiDoc.Document;

            // Khởi tạo data cho WPF | Initialize data for WPF
            Initialize();

            // Get setting(if have)
        }
        private void Initialize()
        {
            // Get Floor Type


            ListFloorTypes = new FilteredElementCollector(Doc)
                .OfClass(typeof(FloorType))
                .Cast<FloorType>()
                .ToList();
            ItemFloorType = ListFloorTypes[0];
            

            ListLevel = new FilteredElementCollector(Doc)
                .OfClass(typeof(Level))
                .Cast<Level>()
                .ToList();

            ListLevel = ListLevel.OrderBy(l => l.Elevation).ToList();

            FloorLevel = ListLevel.First();
           
        }

        #region Binding Properties

        public List<FloorType> ListFloorTypes { get; set; } = new List<FloorType>();
        public FloorType ItemFloorType { get; set; }
        public List<Level> ListLevel { get; set; } = new List<Level>();
        public Level FloorLevel { get; set; }

        public double LevelOffset { get; set; }

        public bool IsChecked { get; set; }

        #endregion

        // Method 
        public void DrawFloor()
        {
            #region Create Floor

            // Create Points 
            XYZ p1 = UiDoc.Selection.PickPoint("Pick first point...");
            XYZ p2 = UiDoc.Selection.PickPoint("Pick second point...");
            XYZ p3 = UiDoc.Selection.PickPoint("Pick third point...");
            XYZ p4 = UiDoc.Selection.PickPoint("Pick 4th point...");
            XYZ p5 = UiDoc.Selection.PickPoint("Pick 5th point...");
            XYZ p6 = UiDoc.Selection.PickPoint("Pick 6th point...");

            // Create Curves
            Line l1 = Line.CreateBound(p1, p2);
            Arc l2 = Arc.Create(p2, p4, p3);
            Line l3 = Line.CreateBound(p4, p5);
            Line l4 = Line.CreateBound(p5, p6);
            Line l5 = Line.CreateBound(p6, p1);


            // Create CurveArray
            CurveArray profile = new CurveArray();
            profile.Append(l1);
            profile.Append(l2);
            profile.Append(l3);
            profile.Append(l4);
            profile.Append(l5);


            // Get Floor Type
            FloorType floorType = ItemFloorType;
            

            Level level = FloorLevel;
            XYZ normal = XYZ.BasisZ;

            using (Transaction tran = new Transaction(Doc))
            {
                tran.Start("Create Floor");
               

                Floor instance = Doc.Create.NewFloor(profile,
                    floorType, level, true, normal);

                instance.get_Parameter(BuiltInParameter.LEVEL_PARAM).Set(level.Id);
                instance.get_Parameter(BuiltInParameter.FLOOR_HEIGHTABOVELEVEL_PARAM).Set(AlphaBimUnitUtils.MmToFeet(LevelOffset));
                instance.get_Parameter(BuiltInParameter.FLOOR_PARAM_IS_STRUCTURAL).Set(IsChecked.ToString());
                instance.get_Parameter(BuiltInParameter.ALL_MODEL_INSTANCE_COMMENTS)
                    .Set("Create Floor from Revit API");
             
                tran.Commit();
            }

            #endregion

        }
    }
}
