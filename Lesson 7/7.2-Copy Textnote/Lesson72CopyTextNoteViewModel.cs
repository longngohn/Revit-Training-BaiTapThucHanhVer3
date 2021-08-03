
#region Namespaces

using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System.Collections.Generic;
using System.Linq;

#endregion

namespace AlphaBIM
{

    public class Lesson72CopyTextNoteViewModel : ViewModelBase
    {
        internal UIDocument UiDoc;
        internal Document Doc;

        public Lesson72CopyTextNoteViewModel(UIDocument uiDoc)
        {

            //Lưu trữ data từ Revit
            UiDoc = uiDoc;
            Doc = UiDoc.Document;
            //Khởi tạo data cho WPF
            Initialize();


        }

        private void Initialize()
        {


            #region Lấy về all parameter có trong project

            IList<Element> allElements
                = new FilteredElementCollector(Doc)
                    .WhereElementIsNotElementType()
                    .Where(e => e.Category != null)
                    .ToList();

            IEqualityComparer comparer = new IEqualityComparer();
            allElements =
                allElements
                    .Distinct(comparer)
                    .ToList();

            foreach (Element e in allElements)
            {
                AllParameters
                    .AddRange(ParameterUtils.GetAllParameters(e));
            }

            AllParameters
                = AllParameters.Distinct().ToList();

            AllParameters.Sort();
            SelectedParameter = AllParameters[0];

            #endregion

        }


        public List<Element> SelectedElements = new List<Element>();
        public List<TextNote> AllTextNotes = new List<TextNote>();

        #region BindingProperties

        public List<string> AllParameters { get; set; } = new List<string>();
        public string SelectedParameter { get; set; }
        public double SaiSo { get; set; }

        private double _percent1;
        public double Percent
        {
            get => _percent1;
            set
            {
                _percent1 = value;
                OnPropertyChanged();
            }
        }


        #endregion

    }
}
