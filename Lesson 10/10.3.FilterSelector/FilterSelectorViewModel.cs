#region Namespaces

using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Windows.Forms;
using Application = Autodesk.Revit.ApplicationServices.Application;
using View = Autodesk.Revit.DB.View;

#endregion

namespace AlphaBIM
{
    public class FilterSelectorViewModel : ViewModelBase
    {
        public FilterSelectorViewModel(UIDocument uiDoc)
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
            List<ElementId> ruleFiltersIds = Doc.GetAllRuleFilters().Select(r => r.Id).ToList();

            List<ElementId> selectionFilterIds = Doc.GetAllSelectionFilters().Select(r => r.Id).ToList();

            foreach (var id in ruleFiltersIds)
            {
                FilterExtension filter = new FilterExtension(Doc, id);
                AllRuleFilters.Add(filter);
            }

            foreach (var id in selectionFilterIds)
            {
                FilterExtension filter = new FilterExtension(Doc, id);
                AllSelectionFilters.Add(filter);
            }
        }

        #region public property

        public UIDocument UiDoc;
        public Document Doc;

        #region Binding properties

        public List<FilterExtension> AllRuleFilters { get; set; } = new List<FilterExtension>();
        public List<FilterExtension> SelectedRuleFilters { get; set; } = new List<FilterExtension>();

        public List<FilterExtension> AllSelectionFilters { get; set; } = new List<FilterExtension>();
        public List<FilterExtension> SelectedSelectionFilters { get; set; } = new List<FilterExtension>();


        #endregion Binding properties


        #endregion public property

        #region private variable



        #endregion private variable

        // Các method khác viết ở dưới đây | Other methods written below


        internal void SelectElement()
        {
            List<ElementId> listIds = new List<ElementId>();

            foreach (var f in AllRuleFilters)
            {
                if (!f.IsFilterSelected ||
                    f.FilterId == ElementId.InvalidElementId) continue;
                listIds.AddRange(ElementSelector.GetAllElementsInFilter(Doc, f.FilterId));
            }

            foreach (var f in AllSelectionFilters)
            {
                if (!f.IsFilterSelected ||
                    f.FilterId == ElementId.InvalidElementId)
                    continue;
                listIds.AddRange(ElementSelector.GetAllElementsInFilter(Doc, f.FilterId));
            }


            listIds = listIds
                .Where(id => id != ElementId.InvalidElementId)
                .ToList();

            if (listIds.Any())
            {
                UiDoc.Selection.SetElementIds(listIds);
            }
            else
            {
                MessageBox.Show("No element in the filters selected!",
                    "ALPHA BIM - LEAD ON TRUST",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
            }
            
        }
        
    }
}
