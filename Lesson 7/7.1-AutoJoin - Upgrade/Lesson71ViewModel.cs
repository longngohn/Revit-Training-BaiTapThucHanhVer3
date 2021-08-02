
#region Namespaces

using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Remoting.Messaging;
using Application = Autodesk.Revit.ApplicationServices.Application;

#endregion

namespace AlphaBIM
{
    
    public class Lesson71ViewModel : ViewModelBase
    {
        internal UIDocument UiDoc;
        internal Document Doc;

        public Lesson71ViewModel(UIDocument uiDoc)
        {
            UiDoc = uiDoc;
            Doc = UiDoc.Document;
            Initialize();
            

        }

        private void Initialize()
        {
            

            ListDoiTuong.Add("DẦM");
            ListDoiTuong.Add("SÀN");
            ListDoiTuong.Add("CỘT");
            ListDoiTuong.Add("MÓNG");
            ListDoiTuong.Add("TƯỜNG");


            DoiTuongUuTien = ListDoiTuong[0];
            DoiTuongBiCat = ListDoiTuong[1];
            IsCurrentView = true;

        }
        #region BindingType

        public List<string> ListDoiTuong { get; set; } = new List<string>();
        public string DoiTuongUuTien { get; set; }
        public string DoiTuongBiCat { get; set; }
        public bool IsEntireProject { get; set; }
        public bool IsCurrentView { get; set; }
        public bool IsCurrentSelection { get; set; }

        #endregion

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

        
    }
}
