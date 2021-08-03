#region Namespaces

using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System.Collections.Generic;
using System.Linq;
using Autodesk.Revit.UI.Selection;

#endregion

namespace AlphaBIM
{
    public class FloorFromCadViewModel : ViewModelBase
    {
        public FloorFromCadViewModel(UIDocument uiDoc)
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
            Reference r = UiDoc.Selection.PickObject(ObjectType.Element,
                new ImportInstanceSelectionFilter(), "CHỌN CAD LINK");

            SelectedCadLink = Doc.GetElement(r) as ImportInstance;

            AllLayers = CadUtils.GetAllLayer(SelectedCadLink);

            if (AllLayers.Any())
                SelectedLayer = AllLayers[0];

            AllFloorType = new FilteredElementCollector(Doc)
                .OfClass(typeof(FloorType))
                .Cast<FloorType>()
                .ToList();

            if (AllFloorType.Any())
                SelectedFloorType = AllFloorType[0];

            AllLevel = new FilteredElementCollector(Doc)
                .OfClass(typeof(Level))
                .Cast<Level>().ToList();

            AllLevel = AllLevel.OrderBy(l => l.Elevation)
                .ToList();

            BaseLevel = AllLevel.First();
            IsStructural = true;


        }

        #region public property

        public UIDocument UiDoc;
        public Document Doc;

        internal ImportInstance SelectedCadLink;

        #region Binding properties

        public List<string> AllLayers { get; set; }
        public string SelectedLayer { get; set; }
        public List<FloorType> AllFloorType { get; set; }
        public FloorType SelectedFloorType { get; set; }
        public List<Level> AllLevel { get; set; }
        public Level BaseLevel { get; set; }
        public double LevelOffset { get; set; }
        public bool IsStructural { get; set; }

        #endregion Binding properties

        public double Percent
        {
            get => _percent;
            set
            {
                _percent = value;

                // Thông báo cho WPF là property "Percent" đã thay đổi giá trị,
                // WPF hãy thay đổi theo
                OnPropertyChanged();
            }
        }

        #endregion public property

        #region private variable

        private double _percent;

        #endregion private variable


        // Các method khác viết ở dưới đây | Other methods written below

       
    }
}
