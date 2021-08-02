
#region Namespaces

using System;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using Application = Autodesk.Revit.ApplicationServices.Application;

#endregion

namespace AlphaBIM
{
    [Transaction(TransactionMode.Manual)]
    public class Lesson4Cmd : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData,
            ref string message, ElementSet elements)
        {
            UIApplication uiapp = commandData.Application;
            UIDocument uidoc = uiapp.ActiveUIDocument;
            Application app = uiapp.Application;
            Document doc = uidoc.Document;

            #region Bài tập về nhà - Bài 4

            //Lấy về Id của đối tượng chọn trước

            ICollection<ElementId> ids = uidoc.Selection.GetElementIds();

            //Tạo biến level va length cua dầm

            
       

            Element framing = null;

            //Kiểm tra điều kiện chọn 1 đối tượng

            try
            {
                if (ids.Count == 1)
                {

                    MessageBox.Show("Bạn đã chọn một đối tượng!");

                    //Duyệt từng phần tử trong mảng element
                    foreach (ElementId elementId in ids)
                    {

                        //Lấy về element 
                        Element e = doc.GetElement(elementId);

                        //Kiểm tra điều kiện là dầm hay không
                        int idIntegerValue = e.Category.Id.IntegerValue;
                        if (idIntegerValue == (int)BuiltInCategory.OST_StructuralFraming)
                        {
                            //Lấy về ElementId của Level và Parameter của Length

                            framing = e;
                            break;
                        }
                        else
                        {
                            MessageBox.Show("Đối tượng không thuộc loại dầm!");
                        }

                    }

                    try
                    {
                        if (framing == null)
                        {
                            return Result.Cancelled;
                        }

                            //Lấy về level và chiều dài

                            ElementId levelBeamId = framing
                                .get_Parameter(BuiltInParameter.INSTANCE_REFERENCE_LEVEL_PARAM)
                                .AsElementId();

                            double beamLength = framing.get_Parameter(BuiltInParameter.INSTANCE_LENGTH_PARAM)
                                .AsDouble();

                        //Khởi tạo bộ lọc

                        FilteredElementCollector collector = new FilteredElementCollector(doc, doc.ActiveView.Id);

                        //Tạo bộ lọc dầm

                        ElementCategoryFilter filter0 =
                            new ElementCategoryFilter(BuiltInCategory.OST_StructuralFraming);




                        //Tạo bộ lọc chiều dài


                        //if (beamLength == null) return Result.Cancelled;

                        ElementId parameterId = new ElementId(BuiltInParameter.INSTANCE_LENGTH_PARAM);
                        double valueFilter =  beamLength;
                        double epsilon = 0.01;
                        FilterRule filterRule2 =
                            ParameterFilterRuleFactory.CreateEqualsRule(parameterId, valueFilter, epsilon);
                        ElementParameterFilter filter2 = new ElementParameterFilter(filterRule2);

                        //Tạo bộ lọc Level

                        ElementId eId = new ElementId(BuiltInParameter.INSTANCE_REFERENCE_LEVEL_PARAM);
                        FilterRule filterRule1 = ParameterFilterRuleFactory.CreateEqualsRule(eId, levelBeamId);
                        ElementParameterFilter filter1 = new ElementParameterFilter(filterRule1);

                        //Gộp bộ lọc Filter
                        List<ElementFilter> filters = new List<ElementFilter>();

                        filters.Add(filter0);
                        filters.Add(filter1);
                        filters.Add(filter2);

                        LogicalAndFilter andFilter = new LogicalAndFilter(filters);

                        //Lấy các ElementId trên view vào list bằng Collector

                        IList<ElementId> list = collector.WherePasses(andFilter).ToElementIds().ToList();

                        MessageBox.Show(list.Count.ToString());

                        uidoc.Selection.SetElementIds(list);
                        
                    }
                    catch (Exception e)
                    {

                        MessageBox.Show("Ket thuc" +e.ToString());
                    }
                   
                }
                else
                {
                    MessageBox.Show("Bạn chưa chọn đối tượng hoặc chọn nhiều đối tượng");
                }
            }
            catch (Exception e)
            {
                MessageBox.Show("Ket thuc" + e.ToString());

            }




            #endregion Bài tập về nhà - Bài 4



            #region  Cách 1: Dùng class Selection

            #region 1.1: Lấy về các elements đang được chọn trước
            ////Tao list Id cua Element
            //ICollection<ElementId> ids = uidoc.Selection.GetElementIds();
            ////Tao list Cot
            //List<Element> allCot = new List<Element>();
            //double tongV = 0.0;
            ////Duyet tung Id trong List Id
            //foreach (ElementId elementId in ids)
            //{
            //    //Lay ra Element tu Element Id
            //    Element e = doc.GetElement(elementId);
            //    //string categoryName = e.Category.Name;

            //    ////Kiem tra dieu kien voi Structural Columns
            //    //if (categoryName.Equals("Structural Columns"))
            //    //{
            //    //    allCot.Add(e);
            //    //    tongV += e.get_Parameter(BuiltInParameter.HOST_VOLUME_COMPUTED).AsDouble();
            //    //}
            //    //-1012806

            //    int idIntegerValue = e.Category.Id.IntegerValue;
            //    if (idIntegerValue == (int)BuiltInCategory.OST_StructuralColumns)
            //    {
            //        allCot.Add(e);
            //        tongV += e.get_Parameter(BuiltInParameter.HOST_VOLUME_COMPUTED).AsDouble();
            //    }
            //}

            //MessageBox.Show(tongV.ToString());
            #endregion 1.1: Lấy về các elements đang được chọn trước

            #region 1.2: PickObject

            #region 1.2.1 Pick một đối tượng, dùng bộ lọc

            /* 1. Lọc Element
             * 2. Lọc Face
             * 3. Edge
             */

            //LocCot filter = new LocCot();

            //Reference r = uidoc.Selection.PickObject(ObjectType.Element, filter, "Hãy chọn đôi tượng cột");

            //MessageBox.Show(doc.GetElement(r).Name);

            //Reference r = uidoc.Selection.PickObject(ObjectType.Face);
            //Element e = doc.GetElement(r);
            //GeometryObject geometryObject = e.GetGeometryObjectFromReference(r);
            //Face face = geometryObject as Face;
            //if (face != null)
            //{
            //    double area = face.Area;
            //    area = UnitUtils.ConvertFromInternalUnits(area, UnitTypeId.SquareMeters);
            //    MessageBox.Show(Math.Round(area,2).ToString());
            //}


            //CylindricalFace



            #endregion 1.2.1 Pick một đối tượng, dùng bộ lọc

            #region 1.2.2 Pick nhiều đối tượng

            //LocCot filter = new LocCot();
            //uidoc.Selection.PickObjects(ObjectType.Element, filter,"Chon Cot");

            //while (true)
            //{
            //    try
            //    {
            //        LocCylindricalFaceFace filter = new LocCylindricalFaceFace(doc);
            //        Reference r = uidoc.Selection.PickObject(ObjectType.Face, filter, "Chọn bề mặt cong");
            //        Element e = doc.GetElement(r);
            //        GeometryObject geometryObject = e.GetGeometryObjectFromReference(r);
            //        Face face = geometryObject as Face;
            //        if (face != null)
            //        {
            //            double area = face.Area;
            //            area = UnitUtils.ConvertFromInternalUnits(area, UnitTypeId.SquareMeters);
            //            MessageBox.Show(Math.Round(area, 2).ToString());
            //        }
            //    }
            //    catch (Exception e)
            //    {
            //        MessageBox.Show("Ket thuc");
            //        break;
            //    }
            //}

            #endregion 1.2.2 Pick nhiều đối tượng

            #endregion 1.2: PickObject

            #endregion Cách 1: Dùng class Selection


            #region Cách 2: Dùng class FilteredElementCollector
            // Tham khảo Apply Filter: http://bit.ly/2mDKH5n

            #region 2.1 Áp 1 Filter

            //FilteredElementCollector collector = new FilteredElementCollector(doc, doc.ActiveView.Id);

            //collector.OfClass(typeof(Floor));

            //collector.OfCategory(BuiltInCategory.OST_Floors);

            //IList<Element> list = collector.ToElements();

            //MessageBox.Show(list.Count.ToString());

            //IList<Element> elements1 = new FilteredElementCollector(doc, doc.ActiveView.Id)
            //    .OfCategory(BuiltInCategory.OST_Floors)
            //    .ToElements();

            //ElementCategoryFilter filter = new ElementCategoryFilter(BuiltInCategory.OST_Floors);

            //IList<Element> list = collector.WherePasses(filter).ToElements();


            //MessageBox.Show(list.Count.ToString());

            #endregion

            #region 2.2 ElementLogicalFilter

            #region 2.2.1 LogicalOrFilter

            #region Cách 1 : Truyền vô 2 Filter

            //FilteredElementCollector collector = new FilteredElementCollector(doc, doc.ActiveView.Id);

            //ElementCategoryFilter wallCategoryFilter =
            //    new ElementCategoryFilter(BuiltInCategory.OST_Walls);
            //ElementCategoryFilter windowCategoryFilter =
            //    new ElementCategoryFilter(BuiltInCategory.OST_Windows);

            //LogicalAndFilter orFilter = new LogicalAndFilter(wallCategoryFilter, windowCategoryFilter);

            //FilteredElementCollector elementCollector = collector.WherePasses(orFilter);

            //IList<Element> list = elementCollector.ToElements();
            //MessageBox.Show(list.Count.ToString());


            #endregion

            #region Cách 2 : Truyền vô List Filter

            //FilteredElementCollector collector = new FilteredElementCollector(doc, doc.ActiveView.Id);

            //List<ElementFilter> elementFilters = new List<ElementFilter>();


            //ElementCategoryFilter wallCategoryFilter =
            //    new ElementCategoryFilter(BuiltInCategory.OST_Walls);
            //ElementCategoryFilter windowCategoryFilter =
            //    new ElementCategoryFilter(BuiltInCategory.OST_Windows);

            //elementFilters.Add(wallCategoryFilter);
            //elementFilters.Add(windowCategoryFilter);

            //LogicalOrFilter orFilter = new LogicalOrFilter(elementFilters);

            //IList<Element> list = collector.WherePasses(orFilter).ToElements();

            #endregion Truyền vô List Filter

            #endregion 2.2.1 LogicalOrFilter

            #region 2.2.2 LogicalAndFilter

            #region Chọn tất cả sàn có level 3

            //FilteredElementCollector collector = new FilteredElementCollector(doc, doc.ActiveView.Id);

            //ElementCategoryFilter floorCategoryFilter =
            //    new ElementCategoryFilter(BuiltInCategory.OST_Floors);

            //var firstLevel = new FilteredElementCollector(doc)
            //    .OfClass(typeof(Level))
            //    .Where(level => level.Name.Equals("Level 2"))
            //    .FirstOrDefault();

            //ElementLevelFilter levelFilter = null;
            //if (firstLevel != null)
            //{
            //   levelFilter = new ElementLevelFilter(firstLevel.Id);
            //    LogicalAndFilter andFilter = new LogicalAndFilter(floorCategoryFilter, levelFilter);
            //    IList<Element> list = collector.WherePasses(andFilter).ToElements();
            //    MessageBox.Show(list.Count.ToString());
            //}

            #endregion Chọn tất cả sàn có level 3

            #region Chọn tất cả dầm có Length = 6m


            //FilteredElementCollector collector = new FilteredElementCollector(doc, doc.ActiveView.Id);


            ////Filter category Framing
            //ElementCategoryFilter framingCategoryFilter =
            // new ElementCategoryFilter(BuiltInCategory.OST_StructuralFraming);

            ////Filter length of Framing
            //ElementId parameterId = new ElementId(BuiltInParameter.INSTANCE_LENGTH_PARAM);
            //double valueFilter = UnitUtils.ConvertToInternalUnits(6,UnitTypeId.Meters);
            //double epsilon = 0.001;
            //FilterRule filterRule = ParameterFilterRuleFactory.CreateEqualsRule(parameterId,valueFilter, epsilon);
            //ElementParameterFilter filter = new ElementParameterFilter(filterRule);


            //parameterId = new ElementId(BuiltInParameter.ALL_MODEL_MARK);
            //string markValue = "D25";
            //FilterRule markFilterRule = ParameterFilterRuleFactory.CreateEqualsRule(markId, markValue, false);
            //ElementParameterFilter markFilter = new ElementParameterFilter(markFilterRule);

            //IList<ElementFilter> elementFilters = new List<ElementFilter>();
            //elementFilters.Add(filter);
            //elementFilters.Add(markFilter);

            ////Loc filter
            //LogicalAndFilter andFilter = new LogicalAndFilter(elementFilters);

            ////Lay list element tu filter
            //IList<Element> list = collector.WherePasses(andFilter).ToElements();

            //MessageBox.Show(list.Count.ToString());


            #endregion Chọn tất cả dầm có Length = 6m

            #endregion 2.2.2 LogicalAndFilter

            #endregion 2.2 ElementLogicalFilter

            #region 2.3 ElementMulticategoryFilter, ElementMulticlassFilter

            #region ElementMulticategoryFilter
            //FilteredElementCollector collector = new FilteredElementCollector(doc, doc.ActiveView.Id);

            //ICollection<BuiltInCategory> categories = new List<BuiltInCategory>();
            //categories.Add(BuiltInCategory.OST_Floors);
            //categories.Add(BuiltInCategory.OST_Walls);
            //categories.Add(BuiltInCategory.OST_StructuralFraming);

            //ElementMulticategoryFilter elementMulticategoryFilter = new ElementMulticategoryFilter(categories);

            //IList<Element> list = collector.WherePasses(elementMulticategoryFilter).ToElements();

            //FilteredElementCollector collector = new FilteredElementCollector(doc, doc.ActiveView.Id);

            //IList<Type> typeList = new List<Type>();
            //typeList.Add(typeof(Wall));
            //typeList.Add(typeof(Rebar));
            //typeList.Add(typeof(Floor));

            //ElementMulticlassFilter elementMulticlassFilter = new ElementMulticlassFilter(typeList);

            //IList<Element> list = collector.WherePasses(elementMulticlassFilter).ToElements();

            #endregion ElementMulticategoryFilter

            #region ElementMulticlassFilter



            #endregion ElementMulticlassFilter

            #endregion 2.3 ElementMulticategoryFilter, ElementMulticlassFilter

            #endregion Cách 2: Dùng class FilteredElementCollector

            return Result.Succeeded;
        }
    }
}
