﻿#region Namespaces

using Autodesk.Revit.DB;
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
    public class FormworkAreaViewModel : ViewModelBase
    {
        public FormworkAreaViewModel(UIDocument uiDoc)
        {
            // Lưu trữ data từ Revit | Store data from Revit
            UiDoc = uiDoc;
            Doc = UiDoc.Document;

            // Khởi tạo data cho WPF | Initialize data for WPF
            Initialize();

            // Get setting(if have)


        }

        private void Initialize()
        {
            // Tạo share para cần thiết
            CreateShareParameterFormworkArea.CreateShareParameter(UiDoc);

            // Khởi tạo sự kiện(nếu có) | Initialize event (if have)
            IList<Reference> references = UiDoc.Selection.PickObjects(
                ObjectType.Element, 
                new BeamColumnSelectionFilter(), 
                "Chọn cột và dầm");

            Beamcolumns = references.Select(selector: x => Doc.GetElement(reference: x)).ToList();
        }


        #region public property

        public UIDocument UiDoc;
        public Document Doc;

        #region Binding properties

        /// <summary>
        /// Phạm vi đối tượng ảnh hưởng là view hiện tại
        /// </summary>
        public bool IsCurrentViewScope { get; set; } = true;

        /// <summary>
        /// Phạm vi đối tượng ảnh hưởng là những đối tượng được chọn
        /// </summary>
        public bool IsCurrentSelectionScope { get; set; }


        /// <summary>
        /// Tính ván khuôn bao gồm đáy dầm
        /// </summary>
        public bool IsCalBeamBottom { get; set; } = true;

        #endregion Binding properties


        #endregion public property

        #region private variable

        private List<Element> Beamcolumns { get; set; }


        #endregion private variable

        // Các method khác viết ở dưới đây | Other methods written below
        internal void ClickOk()
        {

            using (Transaction tran = new Transaction(Doc, "Formwork"))
            {
                tran.Start();

                foreach (Element element in Beamcolumns)
                {
                    Solid solid1 = element.GetSolid();

                    #region Lấy về tất cả đối tượng intersect

                    ElementCategoryFilter categoryFilterBeam
                        = new ElementCategoryFilter(BuiltInCategory.OST_StructuralFraming);
                    ElementCategoryFilter categoryFilterColumn
                        = new ElementCategoryFilter(BuiltInCategory.OST_StructuralColumns);

                    BoundingBoxXYZ box = element.get_BoundingBox(Doc.ActiveView);
                    Outline outline = new Outline(box.Min, box.Max);
                    BoundingBoxIntersectsFilter bbFilter
                        = new BoundingBoxIntersectsFilter(outline);

                    LogicalOrFilter logicalOrFilter = new LogicalOrFilter(categoryFilterBeam, categoryFilterColumn);

                    LogicalAndFilter logicalAndFilter
                        = new LogicalAndFilter(new List<ElementFilter>()
                        {
                            logicalOrFilter,
                        bbFilter
                        });

                    List<Element> intersectElements;

                    if (IsCurrentViewScope)
                    {
                        intersectElements
                            = new FilteredElementCollector(Doc)
                                .WherePasses(logicalAndFilter)
                                .ToList();
                    }
                    else
                    {
                        intersectElements
                            = new FilteredElementCollector(Doc, Beamcolumns.Select(x => x.Id).ToList())
                                .WherePasses(logicalAndFilter)
                                .ToList();
                    }
                    intersectElements = intersectElements
                        .Where(x => x.Id.IntegerValue != element.Id.IntegerValue)
                        .ToList();

                    #endregion

                    List<Face> faces1 = solid1.GetFaces();
                    double solid1Area = 0;

                    double beamBottom = 0;

                    if (element.Category.Id.IntegerValue == (int)BuiltInCategory.OST_StructuralColumns)
                    {
                        solid1Area = faces1.CalAreaNotTopNotBottom();
                    }
                    else if (element.Category.Id.IntegerValue == (int)BuiltInCategory.OST_StructuralFraming)
                    {
                        if (IsCalBeamBottom)
                        {
                            solid1Area = faces1.CalAreaNotTop();
                            beamBottom = faces1.CalAreaOnlyBottom();
                        }
                        else
                        {
                            solid1Area = faces1.CalAreaNotTopNotBottom();
                        }
                    }
                    double totalArea = solid1Area;

                    double beamSubBeam = 0;
                    double beamSubCol = 0;

                    double colSubCol = 0;
                    double colSubBeam = 0;



                    foreach (Element intersectElement in intersectElements)
                    {
                        Solid solid2 = intersectElement.GetSolid();

                        List<Face> faces2 = solid2.GetFaces();
                        double solid2Area = 0;
                        if (element.Category.Id.IntegerValue == (int)BuiltInCategory.OST_StructuralColumns)
                        {
                            solid2Area = faces2.CalAreaNotTopNotBottom();
                        }
                        else if(element.Category.Id.IntegerValue == (int)BuiltInCategory.OST_StructuralFraming)
                        {
                            if (!IsCalBeamBottom)
                            {
                                solid2Area = faces2.CalAreaNotTopNotBottom();
                            }
                            else
                            {
                                solid2Area = faces2.CalAreaNotTop();
                            }
                        }

                        try
                        {
                            Solid union = BooleanOperationsUtils.ExecuteBooleanOperation(
                                solid1,
                                solid2,
                                BooleanOperationsType.Union);
                            List<Face> facesUnion = union.GetFaces();

                            foreach (Face face in facesUnion)
                            {
                                CreateDirectShape.Execute(Doc, face,"aaa");
                            }

                            double unionArea = 0;
                            if (element.Category.Id.IntegerValue == (int)BuiltInCategory.OST_StructuralColumns)
                            {
                                unionArea = facesUnion.CalAreaNotTopNotBottom();
                            }
                            else if (element.Category.Id.IntegerValue == (int)BuiltInCategory.OST_StructuralFraming)
                            {
                                if (!IsCalBeamBottom)
                                {
                                    unionArea = facesUnion.CalAreaNotTopNotBottom();
                                }
                                else
                                {
                                    unionArea=facesUnion.CalAreaNotTop();
                                }
                            }
                            double areaIntersect = (solid1Area + solid2Area - unionArea) / 2;
                            
                            if (areaIntersect > 0)
                            {
                                if (element.Category.Id.IntegerValue == (int)BuiltInCategory.OST_StructuralColumns)
                                {
                                    if (intersectElement.Category.Id.IntegerValue == (int)BuiltInCategory.OST_StructuralColumns)
                                    {
                                        colSubCol += areaIntersect;
                                    }
                                    else if (intersectElement.Category.Id.IntegerValue == (int)BuiltInCategory.OST_StructuralFraming)
                                    {
                                        colSubBeam += areaIntersect;
                                    }
                                }
                                else if (element.Category.Id.IntegerValue == (int)BuiltInCategory.OST_StructuralFraming)
                                {
                                    if (intersectElement.Category.Id.IntegerValue == (int)BuiltInCategory.OST_StructuralColumns)
                                    {
                                        beamSubCol += areaIntersect;
                                    }
                                    else if (intersectElement.Category.Id.IntegerValue == (int)BuiltInCategory.OST_StructuralFraming)
                                    {
                                        beamSubBeam += areaIntersect;
                                    }

                                    if (IsCalBeamBottom)
                                    {
                                        beamBottom -= faces1.CalAreaOnlyBottom()
                                                     + faces2.CalAreaOnlyBottom()
                                                     - facesUnion.CalAreaOnlyBottom();
                                    }
                                }
                            }
                        }
                        catch (Exception e)
                        {

                        }
                    }

                    if (element.Category.Id.IntegerValue == (int)BuiltInCategory.OST_StructuralColumns)
                    {
                        Parameter nameAlbFormworkArea = element.LookupParameter(CreateShareParameterFormworkArea.NameAlbFormworkArea);
                        Parameter nameFwColumnTotal = element.LookupParameter(CreateShareParameterFormworkArea.NameFwColumnTotal);
                        Parameter nameFwColumnSubBeam = element.LookupParameter(CreateShareParameterFormworkArea.NameFwColumnSubBeam);
                        Parameter nameFwColumnSubColumn = element.LookupParameter(CreateShareParameterFormworkArea.NameFwColumnSubColumn);
                        nameAlbFormworkArea.Set(0);
                        nameFwColumnTotal.Set(0);
                        nameFwColumnSubBeam.Set(0);
                        nameFwColumnSubColumn.Set(0);

                        nameAlbFormworkArea.Set(totalArea - colSubBeam - colSubCol);
                        nameFwColumnTotal.Set(totalArea);
                        nameFwColumnSubBeam.Set(colSubBeam);
                        nameFwColumnSubColumn.Set(colSubCol);

                    }
                    else if (element.Category.Id.IntegerValue == (int)BuiltInCategory.OST_StructuralFraming)
                    {
                        Parameter nameAlbFormworkArea = element.LookupParameter(CreateShareParameterFormworkArea.NameAlbFormworkArea);
                        Parameter nameFwBeamTotal = element.LookupParameter(CreateShareParameterFormworkArea.NameFwBeamTotal);
                        Parameter nameFwBeamBottom = element.LookupParameter(CreateShareParameterFormworkArea.NameFwBeamBottom);
                        Parameter nameFwBeamSubCol = element.LookupParameter(CreateShareParameterFormworkArea.NameFwBeamSubCol);
                        Parameter nameFwBeamSubBeam = element.LookupParameter(CreateShareParameterFormworkArea.NameFwBeamSubBeam);
                        nameAlbFormworkArea.Set(0);
                        nameFwBeamTotal.Set(0);
                        nameFwBeamBottom.Set(0);
                        nameFwBeamSubCol.Set(0);
                        nameFwBeamSubBeam.Set(0);

                        nameAlbFormworkArea.Set(totalArea - beamSubBeam - beamSubCol);
                        nameFwBeamTotal.Set(totalArea);
                        if (IsCalBeamBottom)
                        {
                            nameFwBeamBottom.Set(beamBottom);
                        }
                        nameFwBeamSubCol.Set(beamSubCol);
                        nameFwBeamSubBeam.Set(beamSubBeam);
                    }
                }

                tran.Commit();

            }
        }

    }
}