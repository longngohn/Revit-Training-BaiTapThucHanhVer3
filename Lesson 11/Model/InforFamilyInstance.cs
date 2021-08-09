using System.Collections.Generic;
using System.Linq;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Structure;

namespace AlphaBIM
{
    internal class InforFamilyInstance
    {
        internal InforFamilyInstance(FamilyInstance beam)
        {
            Beam = beam;
            Doc = beam.Document;

            // Xác định 3 hướng của dầm
            BasisX = beam.HandOrientation;
            BasisY = beam.FacingOrientation;
            BasisZ = BasisX.CrossProduct(BasisY);
            if (BasisZ.Z < 0)
            {
                BasisZ = BasisZ.Negate();
            }

            Solid solid = beam.GetSolid();
            FaceArray faceArray = solid.Faces;

            List<PlanarFace> planarFaces = new List<PlanarFace>();
            foreach (Face face in faceArray)
            {
                if (face is PlanarFace)
                {
                    planarFaces.Add(face as PlanarFace);
                }
            }

            // Hướng theo chiều dài
            PlanarFace planarFaceBasis1A = planarFaces.FirstOrDefault(x => x.FaceNormal.IsAlmostEqualTo(BasisX));
            PlanarFace planarFaceBasis1B = planarFaces.FirstOrDefault(x => x.FaceNormal.IsAlmostEqualTo(-BasisX));

            // Hướng theo chiều rộng
            PlanarFace planarFaceBasis2A = planarFaces.FirstOrDefault(x => x.FaceNormal.IsAlmostEqualTo(BasisY));
            PlanarFace planarFaceBasis2B = planarFaces.FirstOrDefault(x => x.FaceNormal.IsAlmostEqualTo(BasisY.Negate()));

            // Hướng theo chiều cao
            PlanarFace planarFaceTop = planarFaces.FirstOrDefault(x => x.FaceNormal.IsAlmostEqualTo(BasisZ));
            PlanarFace planarFaceBottom = planarFaces.FirstOrDefault(x => x.FaceNormal.IsAlmostEqualTo(BasisZ.Negate()));

            if (planarFaceBasis1A is null
                || planarFaceBasis1B is null
                || planarFaceBasis2A is null
                || planarFaceBasis2B is null
                || planarFaceTop is null
                || planarFaceBottom is null)
            {
                return;
            }

            double chieuDai = GeometryUtils.GetLengthOfPointToPlane(
                planarFaceBasis1A.FaceNormal,
                planarFaceBasis1A.Origin,
                planarFaceBasis1B.Origin);

            double chieuRong = GeometryUtils.GetLengthOfPointToPlane(
                planarFaceBasis2A.FaceNormal,
                planarFaceBasis2A.Origin,
                planarFaceBasis2B.Origin);

            double chieuCao = GeometryUtils.GetLengthOfPointToPlane(
                planarFaceBottom.FaceNormal,
                planarFaceBottom.Origin,
                planarFaceTop.Origin);


            ChieuRong = chieuRong;
            ChieuDai = chieuDai;
            ChieuCao = chieuCao;
            Center = solid.ComputeCentroid();

            DiemTraiDuoi = Center
                           - BasisZ * chieuCao / 2
                           - BasisX * ChieuDai / 2
                           - BasisY * ChieuRong / 2;

            DiemPhaiDuoi = DiemTraiDuoi
                           + BasisX * ChieuDai;

            DiemTraiTren = DiemTraiDuoi + BasisZ * ChieuCao;
            DiemPhaiTren = DiemPhaiDuoi + BasisZ * ChieuCao;
          

            // Lấy Rebar Cover
            ElementId id1 = beam.get_Parameter(BuiltInParameter.CLEAR_COVER_BOTTOM).AsElementId();
            ElementId id2 = beam.get_Parameter(BuiltInParameter.CLEAR_COVER_TOP).AsElementId();
            ElementId id3 = beam.get_Parameter(BuiltInParameter.CLEAR_COVER_OTHER).AsElementId();

            if (id1 != ElementId.InvalidElementId)
            {
                RebarCoverType rebarCover = Doc.GetElement(id1) as RebarCoverType;
                if (rebarCover != null) CoverBottom = rebarCover.CoverDistance;
            }

            if (id2 != ElementId.InvalidElementId)
            {
                RebarCoverType rebarCover = Doc.GetElement(id2) as RebarCoverType;
                if (rebarCover != null) CoverTop = rebarCover.CoverDistance;
            }

            if (id3 != ElementId.InvalidElementId)
            {
                RebarCoverType rebarCover = Doc.GetElement(id3) as RebarCoverType;
                if (rebarCover != null) CoverOther = rebarCover.CoverDistance;
            }
        }

        internal Document Doc { get; set; }
        internal double CoverBottom { get; set; } = 0;
        internal double CoverTop { get; set; } = 0;
        internal double CoverOther { get; set; } = 0;

        internal double ChieuRong { get; set; }
        internal double ChieuDai { get; set; }
        internal double ChieuCao { get; set; }
        internal XYZ Center { get; set; }
        internal XYZ DiemTraiDuoi { get; set; }
        internal XYZ DiemPhaiDuoi { get; set; }
        internal XYZ DiemTraiTren { get; set; }
        internal XYZ DiemPhaiTren { get; set; }

        internal FamilyInstance Beam { get; set; }

        /// <summary>
        /// Hướng theo chiều dài dầm
        /// </summary>
        internal XYZ BasisX { get; set; }

        /// <summary>
        /// Hướng theo bề rộng dầm
        /// </summary>
        internal XYZ BasisY { get; set; }

        /// <summary>
        /// Hướng theo Chiều cao Dầm và nó luôn hướng lên
        /// </summary>
        internal XYZ BasisZ { get; set; }




    }
}