#region Namespaces

using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Structure;
using Autodesk.Revit.UI;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Windows.Forms;
using Autodesk.Revit.UI.Selection;
using Application = Autodesk.Revit.ApplicationServices.Application;
using View = Autodesk.Revit.DB.View;

#endregion

namespace AlphaBIM
{
    public class RebarSingleBeamViewModel : ViewModelBase
    {
        public RebarSingleBeamViewModel(UIDocument uiDoc)
        {
            // Lưu trữ data từ Revit | Store data from Revit
            UiDoc = uiDoc;
            Doc = UiDoc.Document;

            // Khởi tạo sự kiện(nếu có) | Initialize event (if have)
            Reference reference = UiDoc.Selection.PickObject(ObjectType.Element, new FramingSelectionFilter(), "Chọn 1 dầm");
            Element element = Doc.GetElement(reference);
            InforBeam = new InforFamilyInstance(element as FamilyInstance);
            //MessageBox.Show(AlphaBimUnitUtils.FeetToMm(InforBeam.ChieuDai) + "\n" +
            //                AlphaBimUnitUtils.FeetToMm(InforBeam.ChieuRong) + "\n" +
            //                AlphaBimUnitUtils.FeetToMm(InforBeam.ChieuCao));


            // Khởi tạo data cho WPF | Initialize data for WPF
            Initialize();

            // Get setting(if have)

        }

        private void Initialize()
        {
            AllDuongKinh = new FilteredElementCollector(Doc)
                .OfClass(typeof(RebarBarType))
                .Cast<RebarBarType>()
                .ToList();

            // Kiểm tra xem có RebarBarType nào có trong dự án không
            if (AllDuongKinh.Any())
            {
                SelectedUpperDiameter = AllDuongKinh[0];
                SelectedLowerDiameter = AllDuongKinh[0];
                SelectedDuongKinhDaiChinh = AllDuongKinh[0];
            }

            AllRebarShape = new FilteredElementCollector(Doc)
                .OfClass(typeof(RebarShape))
                .Cast<RebarShape>()
                .ToList();

            // Kiểm tra xem có RebarBarType nào có trong dự án không
            if (AllRebarShape.Any())
            {
                SelectedStirrupShape = AllRebarShape.FirstOrDefault(x => x.Name == "5122");
            }

            DistanceS1Stirrup = DistanceS3Stirrup = Math.Round(AlphaBimUnitUtils.FeetToMm(InforBeam.ChieuDai / 4));
            DistanceS2Stirrup = Math.Round(AlphaBimUnitUtils.FeetToMm(InforBeam.ChieuDai / 2));

            ScaleS1Stirrup = ScaleS3Stirrup = 100;
            ScaleS2Stirrup = 200;

            IsL2HasGirderReinforcement = true;
          

        }

        #region public property

        public UIDocument UiDoc;
        public Document Doc;

        #region Binding properties

        public List<RebarBarType> AllDuongKinh { get; set; }
        public RebarBarType SelectedUpperDiameter { get; set; }
        public RebarBarType SelectedLowerDiameter { get; set; }

        public int SoCayThepChuLopTren { get; set; }
        public int SoCayThepChuLopDuoi { get; set; }

        public List<RebarShape> AllRebarShape { get; set; }

        public RebarShape SelectedStirrupShape { get; set; }
        public RebarBarType SelectedDuongKinhDaiChinh { get; set; }

        public double DistanceS1Stirrup { get; set; }
        public double DistanceS2Stirrup { get; set; }
        public double DistanceS3Stirrup { get; set; }

        public double ScaleS1Stirrup { get; set; }
        public double ScaleS2Stirrup { get; set; }
        public double ScaleS3Stirrup { get; set; }

        public bool IsL2HasGirderReinforcement { get; set; }


        #endregion Binding properties


        #endregion public property


        internal InforFamilyInstance InforBeam { get; set; }


        

        // Các method khác viết ở dưới đây | Other methods written below
        internal void ButtonOk()
        {
          
            using (Transaction trans = new Transaction(Doc, "Tạo thép dầm"))
            {
                trans.Start();

                InforFamilyInstance inforBeam = InforBeam;
                XYZ diemTraiDuoi = inforBeam.DiemTraiDuoi;
                XYZ diemPhaiDuoi = inforBeam.DiemPhaiDuoi;
                XYZ diemTraiTren = inforBeam.DiemTraiTren;
                XYZ diemPhaiTren = inforBeam.DiemPhaiTren;
                double coverBottom = inforBeam.CoverBottom;
                double coverOther = inforBeam.CoverOther;
                double coverTop = inforBeam.CoverTop;
                XYZ basisX = inforBeam.BasisX;
                XYZ basisY = inforBeam.BasisY;
                XYZ basisZ = inforBeam.BasisZ;
                double chieuRong = inforBeam.ChieuRong;
                double chieuCao = inforBeam.ChieuCao;
                double chieuDai = inforBeam.ChieuDai;


                #region Thép đai

                XYZ traiDuoi = diemTraiDuoi
                               + basisX * coverOther
                               + basisY * coverOther
                               + basisZ * coverBottom;
             

                XYZ xScaleToBox = basisY * (chieuRong - coverOther * 2);
                XYZ yScaleToBox = basisZ * (chieuCao - coverBottom - coverTop);

                // Đoạn đầu
                double arrayLength = AlphaBimUnitUtils.MmToFeet(DistanceS1Stirrup) - coverOther;
                RebarForReBarShapeSetSpacing(
                    Doc,
                    SelectedStirrupShape,
                    SelectedDuongKinhDaiChinh,
                    inforBeam.Beam,
                    traiDuoi,
                    basisY,
                    basisZ,
                    xScaleToBox,
                    yScaleToBox,
                    AlphaBimUnitUtils.MmToFeet(ScaleS1Stirrup),
                    arrayLength,
                    true,
                    true,
                    true);

                // Đoạn giữa

                XYZ diem1 = traiDuoi + basisX * arrayLength;
                arrayLength = AlphaBimUnitUtils.MmToFeet(DistanceS2Stirrup);
                RebarForReBarShapeSetSpacing(
                    Doc,
                    SelectedStirrupShape,
                    SelectedDuongKinhDaiChinh,
                    inforBeam.Beam,
                    diem1,
                    basisY,
                    basisZ,
                    xScaleToBox,
                    yScaleToBox,
                    AlphaBimUnitUtils.MmToFeet(ScaleS2Stirrup),
                    arrayLength,
                    true,
                    false,
                    false);

                // Đoạn cuối
                diem1 = diem1 + basisX * arrayLength;
                arrayLength = AlphaBimUnitUtils.MmToFeet(DistanceS3Stirrup) - coverOther;
                RebarForReBarShapeSetSpacing(
                    Doc,
                    SelectedStirrupShape,
                    SelectedDuongKinhDaiChinh,
                    inforBeam.Beam,
                    diem1,
                    basisY,
                    basisZ,
                    xScaleToBox,
                    yScaleToBox,
                    AlphaBimUnitUtils.MmToFeet(ScaleS3Stirrup),
                    arrayLength,
                    true,
                    true,
                    true);

                #endregion

                #region Thép bottom

                XYZ traiDuoiBottom = diemTraiDuoi
                               + basisX * coverOther
                               + basisY * (coverOther + SelectedDuongKinhDaiChinh.BarDiameter + SelectedLowerDiameter.BarDiameter / 2)
                               + basisZ * (coverBottom + SelectedDuongKinhDaiChinh.BarDiameter + SelectedLowerDiameter.BarDiameter / 2);
                XYZ phaiDuoiBottom = diemPhaiDuoi
                               - basisX * coverOther
                               + basisY * (coverOther + SelectedDuongKinhDaiChinh.BarDiameter + SelectedLowerDiameter.BarDiameter / 2)
                               + basisZ * (coverBottom + SelectedDuongKinhDaiChinh.BarDiameter + SelectedLowerDiameter.BarDiameter / 2);

                Line line = Line.CreateBound(
                    traiDuoiBottom,
                    phaiDuoiBottom
                );

                List<Curve> curves = new List<Curve>();
                curves.Add(line);
                arrayLength = chieuRong
                              - 2 * coverOther
                              - 2 * SelectedDuongKinhDaiChinh.BarDiameter
                              - SelectedLowerDiameter.BarDiameter;
                Rebar rebar = RebarForCurveSetFixNumber(
                    Doc,
                    RebarStyle.Standard,
                    SelectedLowerDiameter,
                    null,
                    null,
                    inforBeam.Beam,
                    basisY,
                    curves,
                    RebarHookOrientation.Left,
                    RebarHookOrientation.Left,
                    true,
                    true,
                    SoCayThepChuLopDuoi,
                    arrayLength,
                    true,
                    true,
                    true
                );

                #endregion

                #region Thép gia cường tại đoạn L2

                //Cở sở lý thuyết
                // https://namtrungsafety.com/kinh-nghiem-bo-tri-thep-dam.html
                // Đoạn cắt thép sẽ bằng Lo/5 (với Lo = chiều dài dầm)

                XYZ traiDuoiGiaCuong = diemTraiDuoi
                                     + basisX *(chieuDai/5)
                                     + basisY * (coverOther + SelectedDuongKinhDaiChinh.BarDiameter + SelectedLowerDiameter.BarDiameter/2)
                                     + basisZ * (coverBottom + SelectedDuongKinhDaiChinh.BarDiameter + SelectedLowerDiameter.BarDiameter * 2);
                XYZ phaiDuoiGiaCuong = diemPhaiDuoi
                                       - basisX * (chieuDai / 5)
                                       + basisY * (coverOther + SelectedDuongKinhDaiChinh.BarDiameter + SelectedLowerDiameter.BarDiameter / 2)
                                       + basisZ * (coverBottom + SelectedDuongKinhDaiChinh.BarDiameter + SelectedLowerDiameter.BarDiameter * 2);

                line = Line.CreateBound(
                    traiDuoiGiaCuong,
                    phaiDuoiGiaCuong);
                curves = new List<Curve>();
                curves.Add(line);
                arrayLength = chieuRong
                              - 2 * coverOther
                              - 2 * SelectedDuongKinhDaiChinh.BarDiameter
                              - SelectedLowerDiameter.BarDiameter;
               
                if (IsL2HasGirderReinforcement == true)
                {
                    rebar = RebarForCurveSetFixNumber(
                        Doc,
                        RebarStyle.Standard,
                        SelectedLowerDiameter,
                        null,
                        null,
                        inforBeam.Beam,
                        basisY,
                        curves,
                        RebarHookOrientation.Left,
                        RebarHookOrientation.Left,
                        true,
                        true,
                        SoCayThepChuLopDuoi,
                        arrayLength,
                        true,
                        true,
                        true
                    );
                }
                else
                {
                    MessageBox.Show("Khong ve thep gia cuong lop duoi!");
                }
               

                #endregion

                #region Thép top

                XYZ traiTrenTop = diemTraiTren
                               + basisX * coverOther
                               + basisY * (coverOther + SelectedDuongKinhDaiChinh.BarDiameter + SelectedUpperDiameter.BarDiameter / 2)
                               - basisZ * (coverTop + SelectedDuongKinhDaiChinh.BarDiameter + SelectedUpperDiameter.BarDiameter / 2);
                XYZ phaiTrenTop = diemPhaiTren
                               - basisX * coverOther
                               + basisY * (coverOther + SelectedDuongKinhDaiChinh.BarDiameter + SelectedUpperDiameter.BarDiameter / 2)
                               - basisZ * (coverTop + SelectedDuongKinhDaiChinh.BarDiameter + SelectedUpperDiameter.BarDiameter / 2);

                line = Line.CreateBound(
                    traiTrenTop,
                    phaiTrenTop
                );

                curves = new List<Curve>();
                curves.Add(line);
                arrayLength = chieuRong
                              - 2 * coverOther
                              - 2 * SelectedDuongKinhDaiChinh.BarDiameter
                              - SelectedUpperDiameter.BarDiameter;
                rebar = RebarForCurveSetFixNumber(
                    Doc,
                    RebarStyle.Standard,
                    SelectedUpperDiameter,
                    null,
                    null,
                    inforBeam.Beam,
                    basisY,
                    curves,
                    RebarHookOrientation.Left,
                    RebarHookOrientation.Left,
                    true,
                    true,
                    SoCayThepChuLopTren,
                    arrayLength,
                    true,
                    true,
                    true
                );

                #endregion


                trans.Commit();
            }
        }

        internal static Rebar RebarForCurveSetFixNumber(
            Document document,
            RebarStyle style,
            RebarBarType rebarBarType,
            RebarHookType startHook,
            RebarHookType endHook,
            Element host,
            XYZ norm,
            IList<Curve> curves,
            RebarHookOrientation startHookOrient,
            RebarHookOrientation endHookOrient,
            bool useExistingShapeIfPossible,
            bool createNewShape,
            int numberOfBarPositions,
            double arrayLength,
            bool barsOnNormalSide,
            bool includeFirstBar,
            bool includeLastBar)
        {
            Rebar rebar = Rebar.CreateFromCurves(
                document,
                style,
                rebarBarType,
                startHook,
                endHook,
                host,
                norm,
                curves,
                startHookOrient,
                endHookOrient,
                useExistingShapeIfPossible,
                createNewShape);
            if (document.ActiveView is View3D)
            {
                rebar?.SetSolidInView(document.ActiveView as View3D, true);
            }

            rebar?.SetUnobscuredInView(document.ActiveView, true);

            if (numberOfBarPositions > 1)
            {
                rebar?.GetShapeDrivenAccessor().SetLayoutAsFixedNumber(
                    numberOfBarPositions,
                    arrayLength,
                    barsOnNormalSide, // Cùng hướng với norm nên defaul là true
                    includeFirstBar,
                    includeLastBar);
            }
            return rebar;
        }

        internal static Rebar RebarForReBarShapeSetSpacing(
            Document doc,
            RebarShape rebarShape,
            RebarBarType rebarBarType,
            Element host,
            XYZ origin,
            XYZ xVec,
            XYZ yVec,
            XYZ xScaleToBox,
            XYZ yScaleToBox,
            double spacing,
            double arrayLength,
            bool onNormalOnBar,
            bool incluFirstBar = true,
            bool incluEndBar = true)
        {
            Rebar fromRebarShape = Rebar.CreateFromRebarShape(
                doc,
                rebarShape,
                rebarBarType,
                host,
                origin,
                xVec,
                yVec);
            fromRebarShape?.GetShapeDrivenAccessor().ScaleToBox(origin, xScaleToBox, yScaleToBox);

            if (doc.ActiveView is View3D) fromRebarShape?.SetSolidInView(doc.ActiveView as View3D, true);
            fromRebarShape?.SetUnobscuredInView(doc.ActiveView, true);
            if (arrayLength != 0)
            {
                fromRebarShape?.GetShapeDrivenAccessor().SetLayoutAsMaximumSpacing(
                    spacing,
                    arrayLength,
                    onNormalOnBar,
                    incluFirstBar,
                    incluEndBar);

                fromRebarShape.IncludeFirstBar = incluFirstBar;
                fromRebarShape.IncludeLastBar = incluEndBar;
            }
            return fromRebarShape;
        }

    }
}
