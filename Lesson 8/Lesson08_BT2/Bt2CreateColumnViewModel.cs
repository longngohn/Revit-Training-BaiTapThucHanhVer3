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
    public class BT2CreateColumnViewModel : ViewModelBase
    {
        public UIDocument UiDoc;
        public Document Doc;
        public BT2CreateColumnViewModel(UIDocument uiDoc)
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
             List<Family> listFamily = new FilteredElementCollector(Doc)
                .OfClass(typeof(Family))
                .Cast<Family>()
                .Where(s => s.FamilyCategory.Name.Equals("Structural Columns")|| 
                            s.FamilyCategory.Name.Equals("Columns")).ToList();

             foreach (Family f in listFamily)
             {
                 ISet<ElementId> familySymbolIds = f.GetFamilySymbolIds();
                 foreach (ElementId familySymbolId in familySymbolIds)
                 {
                     FamilySymbol symbol = Doc.GetElement(familySymbolId) as FamilySymbol;
                     ListColumnFamily.Add(symbol);
                 }
             }

             ItemColumnFamily = ListColumnFamily[0];

            ListLevel = new FilteredElementCollector(Doc)
                .OfClass(typeof(Level))
                .Cast<Level>()
                .ToList();
            ListLevel = ListLevel.OrderBy(l => l.Elevation).ToList();

            BaseLevel = ListLevel.First();
            TopLevel = ListLevel.Last();
        }

        #region Binding Properties

        public List<FamilySymbol> ListColumnFamily { get; set; } = new List<FamilySymbol>();
        public FamilySymbol ItemColumnFamily { get; set; }
        public List<Level> ListLevel { get; set; } = new List<Level>();
        public Level BaseLevel { get; set; }
        public Level TopLevel { get; set; }

        public double BaseOffset { get; set; }
        
        public double TopOffset { get; set; }
        

        #endregion

        // Method 
        public void DrawColumn()
        {

            XYZ tamCot = UiDoc.Selection.PickPoint("Pick center point of Column");

            FamilySymbol symbol = ItemColumnFamily;

            if (!symbol.IsActive) symbol.Activate();

            Level level = Doc.ActiveView.GenLevel;

            using (Transaction tran = new Transaction(Doc))
            {

                tran.Start("Create Column");
                FamilyInstance instance = Doc.Create.NewFamilyInstance(tamCot,
                    symbol, level, StructuralType.Column);
                instance.get_Parameter(BuiltInParameter.SCHEDULE_BASE_LEVEL_PARAM).Set(BaseLevel.Id);
                instance.get_Parameter(BuiltInParameter.SCHEDULE_TOP_LEVEL_PARAM).Set(TopLevel.Id);
                instance.get_Parameter(BuiltInParameter.SCHEDULE_BASE_LEVEL_OFFSET_PARAM).Set(AlphaBimUnitUtils.MmToFeet(BaseOffset));        
                instance.get_Parameter(BuiltInParameter.SCHEDULE_TOP_LEVEL_OFFSET_PARAM).Set(AlphaBimUnitUtils.MmToFeet(TopOffset));
                instance.get_Parameter(BuiltInParameter.ALL_MODEL_INSTANCE_COMMENTS)
                    .Set("Create Column from RevitAPI");
                tran.Commit();

            }
        }
    }
}
