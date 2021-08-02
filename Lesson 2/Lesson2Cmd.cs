
#region Namespaces

using System.Windows.Forms;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Application = Autodesk.Revit.ApplicationServices.Application;

#endregion

namespace BetaDragon
{
    [Transaction(TransactionMode.Manual)]
    public class Lesson2Cmd : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData,
      
            
            
            
            
            
            
            
            
            
            
            
            
            
            
            
            
            
            
            
            
            
            
            
            
            
            
            
            
            
            
            
            
            
            
            
            
            
            
            
            
            
            
            
            ref string message, ElementSet elements)
        {
            UIApplication uiapp = commandData.Application;
            UIDocument uidoc = uiapp.ActiveUIDocument;
            Application app = uiapp.Application;
            Document doc = uidoc.Document;

            // Khi chạy bằng Add-in Manager thì comment 2 dòng bên dưới để tránh lỗi
            // When running with Add-in Manager, comment the 2 lines below to avoid errors
            //string dllFolder = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location));
            //AssemblyLoader.LoadAllRibbonAssemblies(dllFolder);


            // code here
            // Sau đó tạo một instance của class PTBH và gán giá trị A,B,C cho instance đó và dùng method NghiemPTBH để show ra màn hình kết quả nghiệm

            PTBH ptbh = new PTBH(15, 28, -10);

            //Định nghĩa dấu của A, B, C
            string daucuaB;
            if (ptbh.B > 0)
            {
                daucuaB = "+";
            }
            else
            {
                daucuaB = "";

            }
            string daucuaC;
            if (ptbh.C > 0)
            {
                daucuaC = "+";
            }
            else
            {
                daucuaC = "";
            }
            //Tạo chuối string phương trình bậc hai
            string phuongTrinhBacHaiText = string.Concat("Ta có phương trình bậc hai: ", 
                ptbh.A, "X^2 ", 
                daucuaB ,ptbh.B, "X", 
                daucuaC,ptbh.C,
                " = 0");

            //Chạy method phương trình bậc hai
            string nghiemPtbh = ptbh.NghiemPTBH();

            MessageBox.Show(phuongTrinhBacHaiText + nghiemPtbh ,"Chương trình tính nghiệm của PTBH - BetaLong",
                MessageBoxButtons.OKCancel, MessageBoxIcon.Question);


            return Result.Succeeded;
        }
    }
}
