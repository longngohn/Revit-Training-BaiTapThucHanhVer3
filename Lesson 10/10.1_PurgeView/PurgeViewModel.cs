#region Namespaces

using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using View = Autodesk.Revit.DB.View;

#endregion

namespace AlphaBIM
{
    public class PurgeViewModel : ViewModelBase
    {
        public PurgeViewModel(UIDocument uiDoc)
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
            // Khởi tạo data cho WPF window

            List<View> allView =
                new FilteredElementCollector(Doc)
                    .OfCategory(BuiltInCategory.OST_Views)
                    .Cast<View>()
                    .Where(v => v.Id != Doc.ActiveView.Id)
                    .ToList();

            foreach (View v in allView)
            {
                ViewExtension viewExtension = new ViewExtension(v);

                AllViewsExtension.Add(viewExtension);
            }

            AllViewsExtension.Sort((v1, v2) => string.CompareOrdinal(v1.Name, v2.Name));

        }

        #region public property

        public UIDocument UiDoc;
        public Document Doc;

        #region Binding properties

        public List<ViewExtension> AllViewsExtension { get; set; }
            = new List<ViewExtension>();
        public ViewExtension SelectedViewsExtension { get; set; }



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

        public void DeleteView()
        {
            List<ViewExtension> allViewToDelete = AllViewsExtension.Where(v => v.IsSelected).ToList();

            int num = 0;

            foreach (ViewExtension v in allViewToDelete)
            {
                using (Transaction trans = new Transaction(Doc))
                {
                    trans.Start("delete view");
                    Doc.Delete(v.View.Id);
                    num += 1;
                    //num = num + 1;

                    trans.Commit();
                }
            }

            MessageBox.Show("Have deleted " + num
                                            + " Views!",
                "Delete View",MessageBoxButtons.OK,MessageBoxIcon.Information
                );
        }
    }
}
