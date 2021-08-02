#region Namespace

using System.Collections.Generic;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System.Linq;
using Autodesk.Revit.UI.Selection;

#endregion

namespace AlphaBIM
{
    public class TransferViewModel : ViewModelBase

    {
        public TransferViewModel(UIDocument uiDoc)
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

        public List<string> AllSourceParameter { get; set; } = new List<string>();
        public string SelectedSourceParameter { get; set; }

        public List<string> AllTargetParameter { get; set; } = new List<string>();
        public string SelectedTargetParameter { get; set; }

        #endregion


        //Method

        private void Initialize()
        {
            //ElementId eId = UiDoc.Selection.GetElementIds().FirstOrDefault();


            List<Element> allFloor = new FilteredElementCollector(Doc, Doc.ActiveView.Id).OfClass(typeof(Floor)).ToList();


            ParameterSet parameterSet = allFloor[0].Parameters;

            foreach (Parameter p in parameterSet)
            {
                AllSourceParameter.Add(p.Definition.Name);
                if (p.Definition.ParameterType == ParameterType.Text && !p.IsReadOnly)
                {
                    AllTargetParameter.Add(p.Definition.Name);
                }
            }
        }
        public void TransferParameter()
        {
            using (Transaction trans = new Transaction(Doc))
            {
                trans.Start("x");
                TransferLocSan filter = new TransferLocSan();
                Reference reference = UiDoc.Selection.PickObject(ObjectType.Element, filter, "Hay chon 1 san");
                Floor floor = Doc.GetElement(reference) as Floor;
                //double value = floor.LookupParameter(SelectedSourceParameter).AsDouble();
                //floor.LookupParameter(SelectedTargetParameter).Set(value.ToString());
                ElementId idLevel = floor.LookupParameter(SelectedSourceParameter).AsElementId();
                
                Level level = Doc.GetElement(idLevel) as Level;
                floor.LookupParameter(SelectedTargetParameter).Set(level.Name);

                trans.Commit();
            }
        }


    }
}
