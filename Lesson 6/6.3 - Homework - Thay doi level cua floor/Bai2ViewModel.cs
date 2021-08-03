#region Namespace

using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System.Collections.Generic;
using System.Linq;

#endregion

namespace AlphaBIM
{
    public class Bai2ViewModel : ViewModelBase

    {
        public Bai2ViewModel(UIDocument uiDoc)
        {
            UiDoc = uiDoc;
            Doc = UiDoc.Document;
            Initialize();
        }



        #region Private Property

        #endregion

        #region Public Property

        public UIDocument UiDoc;
        public Document Doc;

        public List<string> AllLevel { get; set; } = new List<string>();
        public string SelectedLevel { get; set; }


        #endregion


        //Method

        private void Initialize()
        {
            List<Element> elements = new FilteredElementCollector(Doc)
                .OfClass(typeof(Level))
                .ToElements().ToList();

            foreach (Element e in elements)
            {
                AllLevel.Add(e.Name.ToString());
            }
        }

        internal void ChangeFloorLevel()
        {
            ICollection<ElementId> elementIds = UiDoc.Selection.GetElementIds();
            if (elementIds == null) return;

            Transaction tr = new Transaction(Doc);
            tr.Start("Change level of floor");

            //Lấy về list Level

            List<Element> elements = new FilteredElementCollector(Doc)
                .OfClass(typeof(Level))
                .ToElements().ToList();


            ElementId levelId = null;

            List<Element> list = elements.Where(x => x.Name == SelectedLevel).ToList();
            levelId = list.FirstOrDefault().Id;

            foreach (var e in elements)
                if (e.Name == SelectedLevel)
                {
                    levelId = e.Id;
                    break;
                }

            foreach (ElementId id in elementIds)
            {
                Element element = Doc.GetElement(id);
                Parameter p = element.get_Parameter(BuiltInParameter.LEVEL_PARAM);
                p.Set(levelId);
            }


            tr.Commit();


        }


    }
}
