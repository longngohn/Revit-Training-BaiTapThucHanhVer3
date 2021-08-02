#region Namespace

using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System.Linq;

#endregion

namespace AlphaBIM
{
    public class Lesson6ViewModel : ViewModelBase

    {
        public Lesson6ViewModel(UIDocument uiDoc)
        {
            UiDoc = uiDoc;
            Doc = UiDoc.Document;

        }

        private void Initialize()
        {
            //Code here 
        }

        #region Private Property


        private double _topElevation;


        #endregion

        #region Public Property

        public UIDocument UiDoc;
        public Document Doc;


        public double TopElevation
        {
            get => _topElevation;
            set
            {
                _topElevation = value;
                OnPropertyChanged();
            }
        }

        #endregion


        //Method

        internal void SetElevationFloor()
        {
            ElementId id = UiDoc.Selection.GetElementIds().FirstOrDefault();

            if (id == null) return;

            Element e = Doc.GetElement(id);
            ElementId elementId = e.get_Parameter(BuiltInParameter.LEVEL_PARAM).AsElementId();
            Level eLevel = Doc.GetElement(elementId) as Level;

            //double height = UnitUtils.Convert(TopElevation, DisplayUnitType.DUT_MILLIMETERS,
            //    DisplayUnitType.DUT_DECIMAL_FEET);

            double height = UnitUtils.ConvertToInternalUnits(TopElevation, UnitTypeId.Millimeters);
            using (Transaction tr = new Transaction(Doc))
            {
                tr.Start("Change Height");

                e.get_Parameter(BuiltInParameter.FLOOR_HEIGHTABOVELEVEL_PARAM).Set(height);

                tr.Commit();

            }

        }


    }
}
