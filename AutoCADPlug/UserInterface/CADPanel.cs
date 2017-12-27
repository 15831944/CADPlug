using System.Windows.Forms;
using Autodesk.AutoCAD.Windows;
using Autodesk.Windows;

namespace AutoCADPlug.UserInterface
{
    class CADPanel
    {
        /// <summary>
        /// 面板
        /// </summary>
        /// <param name="name">面板名</param>
        /// <param name="title">面板标题</param>
        /// <param name="myCtrl">用户控件</param>
        static public PaletteSet CPalette(string name, string title, UserControl myCtrl)
        {
            PaletteSet ps = new PaletteSet(title);
            ps.Add(name, myCtrl);//myCtrl为自定义面板类
            ps.Visible = true;
            return ps;
        }
    }
}
