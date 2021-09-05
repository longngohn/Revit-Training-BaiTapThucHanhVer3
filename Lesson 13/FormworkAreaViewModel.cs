#region Namespaces

using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using System;
using System.Collections.Generic;
using System.Linq;

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
                new BeamColumnWallSelectionFilter(), 
                "Chọn cột dầm và tường");

            ElementsBCW = references.Select(selector: x => Doc.GetElement(reference: x)).ToList();
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


        /// <summary>
        /// List bao gồm Beam Column và Wall
        /// </summary>
        private List<Element> ElementsBCW { get; set; }


        #endregion private variable

        // Các method khác viết ở dưới đây | Other methods written below
        internal void ClickOk()
        {

            using (Transaction tran = new Transaction(Doc, "Formwork"))
            {
                tran.Start();

                foreach (Element element in ElementsBCW)
                {
                    Solid solid1 = element.GetSolid();

                    #region Lấy về tất cả đối tượng intersect

                    //Lọc đối tượng

                    List<ElementFilter> elementFilters = new List<ElementFilter>();

                    ElementCategoryFilter categoryFilterBeam
                        = new ElementCategoryFilter(BuiltInCategory.OST_StructuralFraming);
                    ElementCategoryFilter categoryFilterColumn
                        = new ElementCategoryFilter(BuiltInCategory.OST_StructuralColumns);
                    ElementCategoryFilter categoryFilterWall
                        = new ElementCategoryFilter(BuiltInCategory.OST_Walls);

                    elementFilters.Add(categoryFilterWall);
                    elementFilters.Add(categoryFilterColumn);
                    elementFilters.Add(categoryFilterBeam);


                    BoundingBoxXYZ box = element.get_BoundingBox(Doc.ActiveView);
                    Outline outline = new Outline(box.Min, box.Max);
                    BoundingBoxIntersectsFilter bbFilter
                        = new BoundingBoxIntersectsFilter(outline);

                    LogicalOrFilter logicalOrFilter = new LogicalOrFilter(elementFilters);

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
                            = new FilteredElementCollector(Doc, ElementsBCW.Select(x => x.Id).ToList())
                                .WherePasses(logicalAndFilter)
                                .ToList();
                    }
                    intersectElements = intersectElements
                        .Where(x => x.Id.IntegerValue != element.Id.IntegerValue)
                        .ToList();

                    #endregion

                    #region Solid1Area

                    List<Face> faces1 = solid1.GetFaces();

                    double solid1Area = 0;
                    double beamBottom = 0;

                    int ostStructuralFraming = (int)BuiltInCategory.OST_StructuralFraming;
                    int ostStructuralColumns = (int)BuiltInCategory.OST_StructuralColumns;
                    int ostWalls = (int)BuiltInCategory.OST_Walls;

                    if (element.Category.Id.IntegerValue == ostStructuralColumns)
                    {
                        solid1Area = faces1.CalAreaNotTopNotBottom();
                    }

                    else if (element.Category.Id.IntegerValue == ostWalls)
                    {
                        solid1Area = faces1.CalAreaNotTopNotBottom();
                    }

                    else if (element.Category.Id.IntegerValue == ostStructuralFraming)
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

                    #endregion

                    double totalArea = solid1Area;

                    double beamSubBeam = 0;
                    double beamSubCol = 0;
                    double beamSubWall = 0;

                    double colSubCol = 0;
                    double colSubBeam = 0;
                    double colSubWall = 0;

                    double wallSubCol = 0;
                    double wallSubBeam = 0;
                    double wallSubWall = 0;

                    #region Solid2Area

                    foreach (Element intersectElement in intersectElements)
                    {

                        Solid solid2 = intersectElement.GetSolid();

                        List<Face> faces2 = solid2.GetFaces();

                        double solid2Area = 0;

                        if (element.Category.Id.IntegerValue == ostStructuralColumns)
                        {
                            solid2Area = faces2.CalAreaNotTopNotBottom();
                        }

                        else if (element.Category.Id.IntegerValue == ostStructuralFraming)
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

                        else if (element.Category.Id.IntegerValue == ostWalls)
                        {
                            solid2Area = faces2.CalAreaNotTopNotBottom();
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
                            if (element.Category.Id.IntegerValue == ostStructuralColumns)
                            {
                                unionArea = facesUnion.CalAreaNotTopNotBottom();
                            }
                            else if (element.Category.Id.IntegerValue == ostStructuralFraming)
                            {
                                if (!IsCalBeamBottom)
                                {
                                    unionArea = facesUnion.CalAreaNotTopNotBottom();
                                }
                                else
                                {
                                    unionArea = facesUnion.CalAreaNotTop();
                                }
                            }

                            else if (element.Category.Id.IntegerValue == ostWalls)
                            {
                                unionArea = facesUnion.CalAreaNotTopNotBottom();
                            }


                            double areaIntersect = (solid1Area + solid2Area - unionArea) / 2;

                            if (areaIntersect > 0)
                            {
                                if (element.Category.Id.IntegerValue == ostStructuralColumns)
                                {
                                    if (intersectElement.Category.Id.IntegerValue == ostStructuralColumns)
                                    {
                                        colSubCol += areaIntersect;
                                    }
                                    else if (intersectElement.Category.Id.IntegerValue == ostStructuralFraming)
                                    {
                                        colSubBeam += areaIntersect;
                                    }
                                    else if (intersectElement.Category.Id.IntegerValue == ostWalls)
                                    {
                                        colSubWall += areaIntersect;
                                    }
                                }
                                else if (element.Category.Id.IntegerValue == ostStructuralFraming)
                                {
                                    if (intersectElement.Category.Id.IntegerValue == ostStructuralColumns)
                                    {
                                        beamSubCol += areaIntersect;
                                    }
                                    else if (intersectElement.Category.Id.IntegerValue == ostStructuralFraming)
                                    {
                                        beamSubBeam += areaIntersect;
                                    }
                                    else if (intersectElement.Category.Id.IntegerValue == ostWalls)
                                    {
                                        beamSubWall += areaIntersect;
                                    }

                                    if (IsCalBeamBottom)
                                    {
                                        beamBottom -= faces1.CalAreaOnlyBottom()
                                                     + faces2.CalAreaOnlyBottom()
                                                     - facesUnion.CalAreaOnlyBottom();
                                    }
                                }

                                else if (element.Category.Id.IntegerValue == ostWalls)
                                {

                                    if (intersectElement.Category.Id.IntegerValue == ostStructuralColumns)
                                    {
                                        wallSubCol += areaIntersect;
                                    }
                                    else if (intersectElement.Category.Id.IntegerValue == ostStructuralFraming)
                                    {
                                        wallSubBeam += areaIntersect;
                                    }
                                    else
                                    {
                                        wallSubWall += areaIntersect;
                                    }

                                }
                            }
                        }
                        catch (Exception e)
                        {

                        }
                    }

                    #endregion

                    #region Ghi giá trị cho Parameter

                    if (element.Category.Id.IntegerValue == ostStructuralColumns)
                    {
                        Parameter nameAlbFormworkArea = element.LookupParameter(CreateShareParameterFormworkArea.NameAlbFormworkArea);
                        Parameter nameFwColumnTotal = element.LookupParameter(CreateShareParameterFormworkArea.NameFwColumnTotal);
                        Parameter nameFwColumnSubBeam = element.LookupParameter(CreateShareParameterFormworkArea.NameFwColumnSubBeam);
                        Parameter nameFwColumnSubWall = element.LookupParameter(CreateShareParameterFormworkArea.NameFwColumnSubWall);
                        Parameter nameFwColumnSubColumn = element.LookupParameter(CreateShareParameterFormworkArea.NameFwColumnSubColumn);
                        nameAlbFormworkArea.Set(0);
                        nameFwColumnTotal.Set(0);
                        nameFwColumnSubBeam.Set(0);
                        nameFwColumnSubColumn.Set(0);
                        nameFwColumnSubWall.Set(0);

                        nameAlbFormworkArea.Set(totalArea - colSubBeam - colSubCol-colSubWall);
                        nameFwColumnTotal.Set(totalArea);
                        nameFwColumnSubBeam.Set(colSubBeam);
                        nameFwColumnSubColumn.Set(colSubCol);
                        nameFwColumnSubWall.Set(colSubWall);

                    }
                    else if (element.Category.Id.IntegerValue == ostStructuralFraming)
                    {
                        Parameter nameAlbFormworkArea = element.LookupParameter(CreateShareParameterFormworkArea.NameAlbFormworkArea);
                        Parameter nameFwBeamTotal = element.LookupParameter(CreateShareParameterFormworkArea.NameFwBeamTotal);
                        Parameter nameFwBeamBottom = element.LookupParameter(CreateShareParameterFormworkArea.NameFwBeamBottom);
                        Parameter nameFwBeamSubCol = element.LookupParameter(CreateShareParameterFormworkArea.NameFwBeamSubCol);
                        Parameter nameFwBeamSubWall= element.LookupParameter(CreateShareParameterFormworkArea.NameFwBeamSubWall);

                        Parameter nameFwBeamSubBeam = element.LookupParameter(CreateShareParameterFormworkArea.NameFwBeamSubBeam);
                        nameAlbFormworkArea.Set(0);
                        nameFwBeamTotal.Set(0);
                        nameFwBeamBottom.Set(0);
                        nameFwBeamSubCol.Set(0);
                        nameFwBeamSubWall.Set(0);
                        nameFwBeamSubBeam.Set(0);

                        nameAlbFormworkArea.Set(totalArea - beamSubBeam - beamSubCol-beamSubWall);
                        nameFwBeamTotal.Set(totalArea);
                        if (IsCalBeamBottom)
                        {
                            nameFwBeamBottom.Set(beamBottom);
                        }
                        nameFwBeamSubCol.Set(beamSubCol);
                        nameFwBeamSubWall.Set(beamSubWall);
                        nameFwBeamSubBeam.Set(beamSubBeam);
                    }
                    else if (element.Category.Id.IntegerValue == ostWalls)
                    {
                        Parameter nameAlbFormworkArea = element.LookupParameter(CreateShareParameterFormworkArea.NameAlbFormworkArea);
                        Parameter nameFwWallTotal = element.LookupParameter(CreateShareParameterFormworkArea.NameFwWallTotal);

                        Parameter nameFwWallSubCol = element.LookupParameter(CreateShareParameterFormworkArea.NameFwWallSubColumn);
                        Parameter nameFwWallSubBeam = element.LookupParameter(CreateShareParameterFormworkArea.NameFwWallSubBeam);
                        Parameter nameFwWallSubWall = element.LookupParameter(CreateShareParameterFormworkArea.NameFwWallSubWall);

                        nameAlbFormworkArea.Set(0);
                        nameFwWallTotal.Set(0);
                        nameFwWallSubCol.Set(0);
                        nameFwWallSubBeam.Set(0);
                        nameFwWallSubWall.Set(0);

                        nameAlbFormworkArea.Set(totalArea - wallSubCol - wallSubCol - wallSubWall);
                        nameFwWallTotal.Set(totalArea);
                        nameFwWallSubCol.Set(wallSubCol);
                        nameFwWallSubBeam.Set(wallSubBeam);
                        nameFwWallSubWall.Set(wallSubWall);
                    }

                    #endregion

                }

                tran.Commit();

            }
        }

    }
}
