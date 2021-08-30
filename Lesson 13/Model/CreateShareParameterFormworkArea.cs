using System;
using System.Collections.Generic;
using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;

namespace AlphaBIM
{
    internal class CreateShareParameterFormworkArea
    {
        internal static void CreateShareParameter(UIDocument uiDoc)
        {
            Document doc = uiDoc.Document;
            Application app = uiDoc.Application.Application;

            string pathDesktop = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            string path = System.IO.Path.Combine(pathDesktop, "ALB_SharedParameter.txt");
            string group = "STR";


            List<Category> categories = new List<Category>();
            categories.Add(Category.GetCategory(doc, BuiltInCategory.OST_StructuralFraming));
            categories.Add(Category.GetCategory(doc, BuiltInCategory.OST_StructuralColumns));

            List<Category> categorieBeam = new List<Category>();
            categorieBeam.Add(Category.GetCategory(doc, BuiltInCategory.OST_StructuralFraming));

            List<Category> categorieColumn = new List<Category>();
            categorieColumn.Add(Category.GetCategory(doc, BuiltInCategory.OST_StructuralColumns));


            Transaction t = new Transaction(doc, "Tạo Shared Parameter");
            t.Start();

            ParameterUtils.CreateSharedParamater(
                doc,
                app,
                path,
                group,
                NameAlbFormworkArea,
                ParameterType.Area,
                BuiltInParameterGroup.PG_STRUCTURAL,
                DescriptionAlbFormworkArea,
                categories,
                true);

            #region Para của dầm

            ParameterUtils.CreateSharedParamater(
                doc,
                app,
                path,
                group,
                NameFwBeamTotal,
                ParameterType.Area,
                BuiltInParameterGroup.PG_STRUCTURAL,
                DescriptionFwBeamTotal,
                categorieBeam,
                true);

            ParameterUtils.CreateSharedParamater(
                doc,
                app,
                path,
                group,
                NameFwBeamBottom,
                ParameterType.Area,
                BuiltInParameterGroup.PG_STRUCTURAL,
                DescriptionFwBeamBottom,
                categorieBeam,
                true);

            ParameterUtils.CreateSharedParamater(
                doc,
                app,
                path,
                group,
                NameFwBeamSubCol,
                ParameterType.Area,
                BuiltInParameterGroup.PG_STRUCTURAL,
                DescriptionFwBeamSubCol,
                categorieBeam,
                true);

            ParameterUtils.CreateSharedParamater(
                doc,
                app,
                path,
                group,
                NameFwBeamSubBeam,
                ParameterType.Area,
                BuiltInParameterGroup.PG_STRUCTURAL,
                DescriptionFwBeamSubBeam,
                categorieBeam,
                true);

            #endregion

            #region Para của cột

            ParameterUtils.CreateSharedParamater(
                doc,
                app,
                path,
                group,
                NameFwColumnTotal,
                ParameterType.Area,
                BuiltInParameterGroup.PG_STRUCTURAL,
                DescriptionFwColumnTotal,
                categorieColumn,
                true);

            ParameterUtils.CreateSharedParamater(
                doc,
                app,
                path,
                group,
                NameFwColumnSubBeam,
                ParameterType.Area,
                BuiltInParameterGroup.PG_STRUCTURAL,
                DescriptionFwColumnSubBeam,
                categorieColumn,
                true);

            ParameterUtils.CreateSharedParamater(
                doc,
                app,
                path,
                group,
                NameFwColumnSubColumn,
                ParameterType.Area,
                BuiltInParameterGroup.PG_STRUCTURAL,
                DescriptionFwColumnSubColumn,
                categorieColumn,
                true);

            #endregion


            t.Commit();
        }

        internal static string NameAlbFormworkArea { get; set; } = "FW_FormworkArea";
        internal static string DescriptionAlbFormworkArea { get; set; } = "Diện tích ván khuôn";


        internal static string NameFwBeamTotal { get; set; } = "FW.Beam.Total";
        internal static string DescriptionFwBeamTotal { get; set; } = "Tổng diện tích ván khuôn";
        internal static string NameFwBeamBottom { get; set; } = "FW.Beam.Bottom";
        internal static string DescriptionFwBeamBottom { get; set; } = "Diện tích ván khuôn đáy";
        internal static string NameFwBeamSubCol { get; set; } = "FW.Beam.SubCol";
        internal static string DescriptionFwBeamSubCol { get; set; } = "Diện tích tiếp xúc với cột";
        internal static string NameFwBeamSubBeam { get; set; } = "FW.Beam.SubBeam";
        internal static string DescriptionFwBeamSubBeam { get; set; } = "Diện tích tiếp xúc với dầm";

        internal static string NameFwColumnTotal { get; set; } = "FW.Column.Total";
        internal static string DescriptionFwColumnTotal { get; set; } = "Tổng diện tích ván khuôn";
        internal static string NameFwColumnSubBeam { get; set; } = "FW.Column.SubBeam";
        internal static string DescriptionFwColumnSubBeam { get; set; } = "Diện tích tiếp xúc với dầm";
        internal static string NameFwColumnSubColumn{ get; set; } = "FW.Column.SubCol";
        internal static string DescriptionFwColumnSubColumn { get; set; } = "Diện tích tiếp xúc với cột";
    }
}