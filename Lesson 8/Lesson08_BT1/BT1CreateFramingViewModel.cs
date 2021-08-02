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

    public class Bt1CreateFramingViewModel : ViewModelBase
    {
        public UIDocument UiDoc;
        public Document Doc;
        public Bt1CreateFramingViewModel(UIDocument uiDoc)
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
           ListFramingFamily = new FilteredElementCollector(Doc)
                .OfClass(typeof(Family))
                .Cast<Family>()
                .Where(s => s.FamilyCategory.Name.Equals("Structural Framing")).ToList();

            //foreach (Family fl in families)
            //{

            //    ElementId eId = fl.GetFamilySymbolIds().FirstOrDefault();
            //    Element e = Doc.GetElement(eId);
            //    Category eCategory = e.Category;
            //    if (eCategory.Id.IntegerValue == (int) BuiltInCategory.OST_StructuralFraming)
            //        ListFramingFamily.Add(fl);
                
            //}

            ItemFramingFamily = ListFramingFamily[0];

            ListReferenceLevel = new FilteredElementCollector(Doc)
                .OfClass(typeof(Level))
                .Cast<Level>()
                .ToList();

            ItemReferenceLevel = ListReferenceLevel[0];
        }

        #region Binding Properties

        public List<Family> ListFramingFamily { get; set; } = new List<Family>();
        public Family ItemFramingFamily { get; set; }
        public List<Level> ListReferenceLevel { get; set; } = new List<Level>();
        public Level ItemReferenceLevel { get; set; }

        public double b { get; set; }
      

        public double h { get; set; }

        public  double z { get; set; }

        #endregion

        // Method 
        public void DrawBeam()
        {
            string symbolBeamName = String.Concat("New ", b, " x ", h);

            //Get Family Symbol

            ElementType s1 = null;

            //Kiem tra xem co the duplicate Type khong

            List<FamilySymbol> familySymbols = new FilteredElementCollector(Doc).OfClass(typeof(FamilySymbol))
                .Cast<FamilySymbol>()
                .Where(s => s.Family.Name.Equals(ItemFramingFamily.Name))
                .ToList();

                using (TransactionGroup tAll = new TransactionGroup(Doc))
                {
                    tAll.Start("Create new Framing Type");

                    using (Transaction tSet = new Transaction(Doc))
                    {
                        tSet.Start("Create Family Type");
                    

                       
                            //Tìm Family giống để gán vào Element Type
                            foreach (FamilySymbol familySymbol1 in familySymbols)
                            {
                                if (familySymbol1.Name.Equals(symbolBeamName))
                                {

                                    s1 = familySymbol1;
                                    break;
                                }
                            }

                            if (familySymbols[0].LookupParameter("b") != null)
                            {
                                s1 = familySymbols[0].Duplicate(symbolBeamName);
                                s1.LookupParameter("b").Set(AlphaBimUnitUtils.MmToFeet(b));
                                s1.LookupParameter("h").Set(AlphaBimUnitUtils.MmToFeet(h));
                            }
                      
                          
                   
                        tSet.Commit();
                    }

                    FamilySymbol familySymbol = new FilteredElementCollector(Doc).OfClass(typeof(FamilySymbol))
                        .Cast<FamilySymbol>()
                        .Where(s => s.Family.Name.Equals(ItemFramingFamily.Name))
                        .FirstOrDefault(s => s.Name.Equals(symbolBeamName));

                    if (familySymbol != null)
                    {
                        if (!familySymbol.IsActive) familySymbol.Activate();

                        Level level = ItemReferenceLevel;

                        XYZ pt1 = UiDoc.Selection.PickPoint("Pick start point...");
                        XYZ pt2 = UiDoc.Selection.PickPoint("Pick end point...");

                        double levelElevation = level.Elevation;
                        XYZ startPoint = new XYZ(pt1.X, pt1.Y, levelElevation);
                        XYZ endPoint = new XYZ(pt2.X, pt2.Y, levelElevation);
                        Line framingLocation = Line.CreateBound(startPoint, endPoint);

                        using (Transaction t = new Transaction(Doc))
                        {
                            t.Start("Create Framing");

                            FamilyInstance instance = Doc.Create.NewFamilyInstance(framingLocation, familySymbol, level,
                                StructuralType.Beam);

                            instance.get_Parameter(BuiltInParameter.ALL_MODEL_INSTANCE_COMMENTS)
                                .Set("Create Framing from Automate");

                            instance.get_Parameter(BuiltInParameter.Z_OFFSET_VALUE).Set(AlphaBimUnitUtils.MmToFeet(z));

                            t.Commit();
                        }

                        if (tAll.HasStarted()) tAll.Commit();
                    }

                    if (tAll.HasStarted())
                    {
                        tAll.RollBack();
                        MessageBox.Show("Family này không có biến B và H, vui lòng chọn lại family!");
                    }
                    
                }

                //Lay ve Family Symbol equal symbolBeamName

              
        }
    }
}
