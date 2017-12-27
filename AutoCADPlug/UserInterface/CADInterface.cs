using System;
using System.Drawing;

namespace AutoCADPlug.UserInterface
{
    //此应用程序界面就是autocad软件界面
    class CADInterface
    {
        /// <summary>
        /// 修改标题
        /// </summary>
        /// <param name="title">标题</param>
        public static void SetTitle(string title)
        {
            Autodesk.AutoCAD.Windows.Window MainWindow = Autodesk.AutoCAD.ApplicationServices.Application.MainWindow;
            MainWindow.Text = title;
        }
        /// <summary>
        /// 修改图标
        /// </summary>
        /// <param name="fileName">图标路径</param>
        public static void SetIcon(string fileName)
        {
            Autodesk.AutoCAD.Windows.Window MainWindow = Autodesk.AutoCAD.ApplicationServices.Application.MainWindow; MainWindow.Icon = new Icon(fileName);
        }
    }
}
