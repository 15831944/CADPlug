using System;
using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;
using Autodesk.AutoCAD.EditorInput;

namespace AutoCADPlug
{
    public class ShowMsgOperation
    {
        /// <summary>
        /// 命令行中显示字符
        /// </summary>
        /// <param name="message"></param>
        public static void Message(string message)
        {
            Document doc = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument;
            Editor ed = doc.Editor;
            ed.WriteMessage(message);
        }

        /// <summary>
        /// 弹出帮助网页
        /// </summary>
        /// <param name="web"></param>
        public static void Web(string url)
        {
            System.Diagnostics.Process.Start("explorer.exe", url);
        }

        /// <summary>
        /// 弹出警告框
        /// </summary>
        /// <param name="message"></param>
        public static void Alert(string message)
        {
            Autodesk.AutoCAD.ApplicationServices.Application.ShowAlertDialog(message);
        }


    }
}
