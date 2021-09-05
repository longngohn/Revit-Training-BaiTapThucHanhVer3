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
            categories.Add(Category.GetCategory(doc, BuiltInCategory.OST_Walls));

            List<Category> categorieBeam = new List<Category>();
            categorieBeam.Add(Category.GetCategory(doc, BuiltInCategory.OST_StructuralFraming));

            List<Category> categorieColumn = new List<Category>();
            categorieColumn.Add(Category.GetCategory(doc, BuiltInCategory.OST_StructuralColumns));

            List<Category> categorieWall = new List<Category>();
            categorieWall.Add(Category.GetCategory(doc, BuiltInCategory.OST_Walls));


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

            //Add sub wall
            ParameterUtils.CreateSharedParamater(
                doc,
                app,
                path,
                group,
                NameFwBeamSubWall,
                ParameterType.Area,
                BuiltInParameterGroup.PG_STRUCTURAL,
                DescriptionFwBeamSubWall,
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

            ParameterUtils.CreateSharedParamater(
                doc,
                app,
                path,
                group,
                NameFwColumnSubWall,
                ParameterType.Area,
                BuiltInParameterGroup.PG_STRUCTURAL,
                DescriptionFwColumnSubWall,
                categorieColumn,
                true);

            #endregion

            #region Para của tường

            ParameterUtils.CreateSharedParamater(
                doc,
                app,
                path,
                group,
                NameFwWallTotal,
                ParameterType.Area,
                BuiltInParameterGroup.PG_STRUCTURAL,
                DescriptionFwWallTotal,
                categorieWall,
                true);

            ParameterUtils.CreateSharedParamater(
                doc,
                app,
                path,
                group,
                NameFwWallSubBeam,
                ParameterType.Area,
                BuiltInParameterGroup.PG_STRUCTURAL,
                DescriptionFwWallSubBeam,
                categorieWall,
                true);

            ParameterUtils.CreateSharedParamater(
                doc,
                app,
                path,
                group,
                NameFwWallSubColumn,
                ParameterType.Area,
                BuiltInParameterGroup.PG_STRUCTURAL,
                DescriptionFwWallSubColumn,
                categorieWall,
                true);

            ParameterUtils.CreateSharedParamater(
                doc,
                app,
                path,
                group,
                NameFwWallSubWall,
                ParameterType.Area,
                BuiltInParameterGroup.PG_STRUCTURAL,
                DescriptionFwWallSubWall,
                categorieWall,
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

        internal static string NameFwBeamSubWall { get; set; } = "FW.Beam.SubWall";
        internal static string DescriptionFwBeamSubWall { get; set; } = "Diện tích tiếp xúc với tường";



        internal static string NameFwColumnTotal { get; set; } = "FW.Column.Total";
        internal static string DescriptionFwColumnTotal { get; set; } = "Tổng diện tích ván khuôn";
        internal static string NameFwColumnSubBeam { get; set; } = "FW.Column.SubBeam";
        internal static string DescriptionFwColumnSubBeam { get; set; } = "Diện tích tiếp xúc với dầm";
        internal static string NameFwColumnSubColumn{ get; set; } = "FW.Column.SubCol";
        internal static string DescriptionFwColumnSubColumn { get; set; } = "Diện tích tiếp xúc với cột";

        internal static string NameFwColumnSubWall { get; set; } = "FW.Column.SubWall";
        internal static string DescriptionFwColumnSubWall { get; set; } = "Diện tích tiếp xúc với tường";


        internal static string NameFwWallTotal { get; set; } = "FW.Wall.Total";
        internal static string DescriptionFwWallTotal { get; set; } = "Tổng diện tích ván khuôn";
        internal static string NameFwWallSubColumn { get; set; } = "FW.Wall.SubCol";
        internal static string DescriptionFwWallSubColumn { get; set; } = "Diện tích tiếp xúc với cột";
        internal static string NameFwWallSubBeam { get; set; } = "FW.Wall.SubBeam";
        internal static string DescriptionFwWallSubBeam { get; set; } = "Diện tích tiếp xúc với dầm";
        internal static string NameFwWallSubWall { get; set; } = "FW.Wall.SubWall";
        internal static string DescriptionFwWallSubWall { get; set; } = "Diện tích tiếp xúc với tường";
    }
}