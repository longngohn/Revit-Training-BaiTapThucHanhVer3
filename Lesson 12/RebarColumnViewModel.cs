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
    public class RebarColumnViewModel : ViewModelBase
    {
        public RebarColumnViewModel(UIDocument uiDoc)
        {
            // Lưu trữ data từ Revit | Store data from Revit
            UiDoc = uiDoc;
            Doc = UiDoc.Document;

            // Khởi tạo sự kiện(nếu có) | Initialize event (if have)
            Reference reference =
                UiDoc.Selection.PickObject(ObjectType.Element, new ColumnSelectionFilter(), "Select A Column");
            Element element = Doc.GetElement(reference);
            InforFamilyInstance = new InforFamilyInstance(element as FamilyInstance);
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
                SelectedCanhDaiDiameter = AllDuongKinh[0];
                SelectedCanhNganDiameter = AllDuongKinh[0];
                SelectedDuongKinhDaiChinh = AllDuongKinh[0];
            }

            AllRebarShape = new FilteredElementCollector(Doc)
                .OfClass(typeof(RebarShape))
                .Cast<RebarShape>()
                .ToList();

            // Kiểm tra xem có RebarBarType nào có trong dự án không
            if (AllRebarShape.Any()) SelectedStirrupShape = AllRebarShape.FirstOrDefault(x => x.Name == "5122");

            DistanceS1Stirrup = DistanceS3Stirrup =
                Math.Round(AlphaBimUnitUtils.FeetToMm(InforFamilyInstance.ChieuCao / 4));
            //DistanceS2Stirrup = Math.Round(AlphaBimUnitUtils.FeetToMm(InforFamilyInstance.ChieuCao / 2));

            ScaleS1Stirrup = ScaleS3Stirrup = 100;
            ScaleS2Stirrup = 200;

            IsL2HasGirderReinforcement = true;
        }

        #region public property

        public UIDocument UiDoc;
        public Document Doc;
        private double _distanceS1Stirrup;
        private double _distanceS2Stirrup;
        private double _distanceS3Stirrup;

        #region Binding properties

        public List<RebarBarType> AllDuongKinh { get; set; }
        public RebarBarType SelectedCanhDaiDiameter { get; set; }
        public RebarBarType SelectedCanhNganDiameter { get; set; }

        public int SoCayThepChuCanhDai { get; set; }
        public int SoCayThepChuCanhNgan { get; set; }

        public List<RebarShape> AllRebarShape { get; set; }

        public RebarShape SelectedStirrupShape { get; set; }
        public RebarBarType SelectedDuongKinhDaiChinh { get; set; }

        public double DistanceS1Stirrup
        {
            get => _distanceS1Stirrup;
            set
            {
                _distanceS1Stirrup = value;
                DistanceS2Stirrup = InforFamilyInstance.ChieuCao - AlphaBimUnitUtils.MmToFeet(DistanceS3Stirrup) -
                                    AlphaBimUnitUtils.MmToFeet(value);

            }
        }

        public double DistanceS2Stirrup
        {
            get => _distanceS2Stirrup;
            set
            {
                _distanceS2Stirrup = Math.Round(AlphaBimUnitUtils.FeetToMm(value));
                OnPropertyChanged();
            }
        }

        public double DistanceS3Stirrup
        {
            get => _distanceS3Stirrup;
            set
            {
                _distanceS3Stirrup = value;
                DistanceS2Stirrup = InforFamilyInstance.ChieuCao - AlphaBimUnitUtils.MmToFeet(DistanceS1Stirrup) -
                                    AlphaBimUnitUtils.MmToFeet(value);
            }
        }

        public double ScaleS1Stirrup { get; set; }
        public double ScaleS2Stirrup { get; set; }
        public double ScaleS3Stirrup { get; set; }

        public bool IsL2HasGirderReinforcement { get; set; }

        #endregion Binding properties

        #endregion public property


        internal InforFamilyInstance InforFamilyInstance { get; set; }


        // Các method khác viết ở dưới đây | Other methods written below
        internal void ButtonOk()
        {
            InforFamilyInstance inforFamilyInstance = InforFamilyInstance;
            XYZ diemTraiDuoi = inforFamilyInstance.DiemTraiDuoi;
            XYZ diemPhaiDuoi = inforFamilyInstance.DiemPhaiDuoi;
            XYZ diemTraiTren = inforFamilyInstance.DiemTraiTren;
            XYZ diemPhaiTren = inforFamilyInstance.DiemPhaiTren;
            double coverBottom = inforFamilyInstance.CoverBottom;
            double coverOther = inforFamilyInstance.CoverOther;
            double coverTop = inforFamilyInstance.CoverTop;
            XYZ basisX = inforFamilyInstance.BasisX;
            XYZ basisY = inforFamilyInstance.BasisY;
            XYZ basisZ = inforFamilyInstance.BasisZ;


            bool onNormal = basisY.CrossProduct(basisX).IsAlmostEqualTo(XYZ.BasisZ);
            

            using (Transaction trans = new Transaction(Doc, "Create Column Rebar"))
            {
                trans.Start();

                #region THEP DAI COT

                double diameterThepDai = SelectedDuongKinhDaiChinh.BarDiameter;

                XYZ origin = diemTraiDuoi + basisX * coverOther
                                          + basisY * coverOther
                                          + basisZ * coverBottom;

                double chieuRong = inforFamilyInstance.ChieuRong;
                double chieuDai = inforFamilyInstance.ChieuDai;
                double chieuCao = inforFamilyInstance.ChieuCao;

                XYZ xScaleToBox = basisY * (chieuRong - 2 * coverOther);
                XYZ yScaleToBox = basisX * (chieuDai - 2 * coverOther);

                //Doan dau

                double arrayLength = AlphaBimUnitUtils.MmToFeet(DistanceS1Stirrup) - coverOther;
                RebarForReBarShapeSetSpacing(
                    Doc,
                    SelectedStirrupShape,
                    SelectedDuongKinhDaiChinh,
                    inforFamilyInstance.Beam,
                    origin,
                    basisY,
                    basisX,
                    xScaleToBox,
                    yScaleToBox,
                    AlphaBimUnitUtils.MmToFeet(ScaleS1Stirrup),
                    arrayLength,
                    onNormal
                );


                //Doan giua

                origin = origin + basisZ * arrayLength;
                arrayLength = AlphaBimUnitUtils.MmToFeet(DistanceS2Stirrup);

                RebarForReBarShapeSetSpacing(
                    Doc,
                    SelectedStirrupShape,
                    SelectedDuongKinhDaiChinh,
                    inforFamilyInstance.Beam,
                    origin,
                    basisY,
                    basisX,
                    xScaleToBox,
                    yScaleToBox,
                    AlphaBimUnitUtils.MmToFeet(ScaleS2Stirrup),
                    arrayLength,
                    onNormal,
                    false,
                    false
                );

                //Doan cuoi

                origin = origin + basisZ * arrayLength;

                arrayLength = AlphaBimUnitUtils.MmToFeet(DistanceS3Stirrup) - coverOther;

                RebarForReBarShapeSetSpacing(
                    Doc,
                    SelectedStirrupShape,
                    SelectedDuongKinhDaiChinh,
                    inforFamilyInstance.Beam,
                    origin,
                    basisY,
                    basisX,
                    xScaleToBox,
                    yScaleToBox,
                    AlphaBimUnitUtils.MmToFeet(ScaleS1Stirrup),
                    arrayLength,
                    onNormal
                );

                #endregion

                #region THEP CANH DAI 1

                XYZ diemCanhDai1 = diemTraiDuoi + basisX * (coverOther + diameterThepDai + SelectedCanhDaiDiameter.BarDiameter/2)
                                                + basisY * (coverOther + diameterThepDai + SelectedCanhDaiDiameter.BarDiameter / 2)
                                                + basisZ * coverBottom;

                XYZ diemCanhDai2 = diemTraiTren + basisX * (coverOther + diameterThepDai + SelectedCanhDaiDiameter.BarDiameter / 2)
                                                            + basisY * (coverOther + diameterThepDai + SelectedCanhDaiDiameter.BarDiameter / 2)
                                                            - basisZ * coverTop;

                Line line = Line.CreateBound(diemCanhDai1, diemCanhDai2);
                List<Curve> curves = new List<Curve>();

                curves.Add(line);

                arrayLength = chieuDai
                              - 2 * coverOther
                              - diameterThepDai
                              - SelectedCanhDaiDiameter.BarDiameter;

                RebarForCurveSetFixNumber(
                    Doc,
                    RebarStyle.Standard,
                    SelectedCanhDaiDiameter,
                    null,
                    null,
                    InforFamilyInstance.Beam,
                    basisX,
                    curves,
                    RebarHookOrientation.Left,
                    RebarHookOrientation.Left,
                    true,
                    true,
                    SoCayThepChuCanhDai,
                    arrayLength,
                    true,
                    true,
                    true
                );

                #endregion

                #region THEP CANH DAI 2

                XYZ diem2CanhDai1 = diemTraiDuoi + basisX * (coverOther + diameterThepDai + SelectedCanhDaiDiameter.BarDiameter / 2)
                                                 + basisY * (chieuRong - coverOther - diameterThepDai -
                                                             SelectedCanhDaiDiameter.BarDiameter / 2)
                                                 + basisZ * coverBottom;

                XYZ diem2CanhDai2 = diemTraiTren + basisX * (coverOther + diameterThepDai + SelectedCanhDaiDiameter.BarDiameter / 2)
                                                 + basisY * (chieuRong - coverOther - diameterThepDai -
                                                             SelectedCanhDaiDiameter.BarDiameter / 2)
                                                 - basisZ * coverTop;

                line = Line.CreateBound(diem2CanhDai1, diem2CanhDai2);
                curves = new List<Curve>();

                curves.Add(line);


                RebarForCurveSetFixNumber(
                    Doc,
                    RebarStyle.Standard,
                    SelectedCanhDaiDiameter,
                    null,
                    null,
                    InforFamilyInstance.Beam,
                    basisX,
                    curves,
                    RebarHookOrientation.Left,
                    RebarHookOrientation.Left,
                    true,
                    true,
                    SoCayThepChuCanhDai,
                    arrayLength,
                    true,
                    true,
                    true
                );

                #endregion

                #region THEP CANH NGAN 1

                XYZ diemCanhNgan1 = diemTraiDuoi + basisX * (coverOther + diameterThepDai + SelectedCanhNganDiameter.BarDiameter/2)
                                                 + basisY * (coverOther + diameterThepDai + SelectedCanhNganDiameter.BarDiameter / 2)
                                                 + basisZ * coverBottom;

                XYZ diemCanhNgan2 = diemTraiTren + basisX * (coverOther + diameterThepDai + SelectedCanhNganDiameter.BarDiameter/2)
                                                 + basisY * (coverOther + diameterThepDai + SelectedCanhNganDiameter.BarDiameter / 2)
                                    - basisZ * coverTop;

                line = Line.CreateBound(diemCanhNgan1, diemCanhNgan2);
                curves = new List<Curve>();

                curves.Add(line);
                arrayLength = chieuRong
                              - 2 * coverOther
                              - SelectedCanhNganDiameter.BarDiameter;

                RebarForCurveSetFixNumber(
                    Doc,
                    RebarStyle.Standard,
                    SelectedCanhNganDiameter,
                    null,
                    null,
                    InforFamilyInstance.Beam,
                    basisY,
                    curves,
                    RebarHookOrientation.Left,
                    RebarHookOrientation.Left,
                    true,
                    true,
                    SoCayThepChuCanhNgan,
                    arrayLength,
                    true,
                    false,
                    false
                );

                #endregion


                #region THEP CANH NGAN 2

                XYZ diem2CanhNgan1 = diemTraiDuoi + basisX * (chieuDai - coverOther - diameterThepDai - SelectedCanhNganDiameter.BarDiameter/2)
                                                  + basisY * (coverOther + diameterThepDai + SelectedCanhNganDiameter.BarDiameter / 2)
                                                  + basisZ * coverBottom;

                XYZ diem2CanhNgan2 = diemTraiTren + basisX * (chieuDai - coverOther - diameterThepDai - SelectedCanhNganDiameter.BarDiameter/2)
                                                  + basisY * (coverOther + diameterThepDai + SelectedCanhNganDiameter.BarDiameter / 2)
                                                     - basisZ * coverTop;

                line = Line.CreateBound(diem2CanhNgan1, diem2CanhNgan2);
                curves = new List<Curve>();

                curves.Add(line);
                arrayLength = chieuRong
                              - 2 * coverOther
                              - SelectedCanhNganDiameter.BarDiameter;

                RebarForCurveSetFixNumber(
                    Doc,
                    RebarStyle.Standard,
                    SelectedCanhNganDiameter,
                    null,
                    null,
                    InforFamilyInstance.Beam,
                    basisY,
                    curves,
                    RebarHookOrientation.Left,
                    RebarHookOrientation.Left,
                    true,
                    true,
                    SoCayThepChuCanhNgan,
                    arrayLength,
                    true,
                    false,
                    false
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
            if (document.ActiveView is View3D) rebar?.SetSolidInView(document.ActiveView as View3D, true);

            rebar?.SetUnobscuredInView(document.ActiveView, true);

            if (numberOfBarPositions > 1)
                rebar?.GetShapeDrivenAccessor().SetLayoutAsFixedNumber(
                    numberOfBarPositions,
                    arrayLength,
                    barsOnNormalSide, // Cùng hướng với norm nên defaul là true
                    includeFirstBar,
                    includeLastBar);
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
