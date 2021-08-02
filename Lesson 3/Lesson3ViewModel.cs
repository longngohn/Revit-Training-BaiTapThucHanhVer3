using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;

namespace AlphaBIM
{
    public class Lesson3ViewModel: ViewModelBase
    {
        public UIDocument UiDoc;
            public Document Doc;

            public Lesson3ViewModel (UIDocument uiDoc)
            {
                UiDoc = uiDoc;
                Doc = UiDoc.Document;
                List<string> _listDanhMuc = new List<string>();
                _listDanhMuc.Add("Thứ tự đầu tiên");
                _listDanhMuc.Add("Thứ tự thứ hai");
                _listDanhMuc.Add("Thứ tự thứ ba");
                ListDanhMuc = _listDanhMuc;
                SelectedIndex = 0;
                SelectedCombobox = ListDanhMuc[SelectedIndex];


        }
            //Private 
            private string _txtBlock = "100";

         


        //Public
      
        
        public string TxtBlock
            {
                get { return _txtBlock; }
                set
                {
                    _txtBlock = value;
                    OnPropertyChanged();
                }
            }

        public List<string> ListDanhMuc { get; set; }
        public string SelectedCombobox { get; set; }
        public int SelectedIndex { get; set; }

     


    }
}
