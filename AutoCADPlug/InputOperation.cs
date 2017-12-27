using System;
using Autodesk.AutoCAD.Runtime;
using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;
using Autodesk.AutoCAD.EditorInput;
using System.Windows.Forms;

namespace AutoCADPlug
{
    public class InputOperation
    {
        /// <summary>
        /// 提示用户输入关键字
        /// </summary>
        /// <param name="message"></param>
        /// <param name="keywords"></param>
        /// <returns>用户输入的字符</returns>
        public static string GetKeywords(string message, string[] keywords)
        {
            Editor ed = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument.Editor;
            PromptKeywordOptions ko = new PromptKeywordOptions(message);
            for (int i = 0; i < keywords.Length; i++)
            {
                ko.Keywords.Add(keywords[i]);
            }
            ko.Keywords.Default = keywords[0];
            ko.AllowNone = true;
            PromptResult k = ed.GetKeywords(ko);
            if (k.Status == PromptStatus.OK)
            {
                return k.StringResult;
            }
            else
            {
                return "";
            }
        }

        /// <summary>
        /// 提示用户输入实数
        /// </summary>
        /// <param name="message"></param>
        /// <returns>用户输入的实数</returns>
        public static double GetDouble(string message)
        {
            Editor ed = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument.Editor;
            PromptDoubleResult d = ed.GetDouble(message);
            if (d.Status == PromptStatus.OK)
            {
                return (double)d.Value;
            }
            else
            {
                return 0.0;
            }
        }

        /// <summary>
        /// 提示用户输入整数
        /// </summary>
        /// <param name="message">提示</param>
        /// <returns>用户输入的整数</returns>
        public static int GetInt(string message)
        {
            Editor ed = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument.Editor;
            PromptIntegerResult i = ed.GetInteger(message);
            if (i.Status == PromptStatus.OK)
            {
                return (int)i.Value;
            }
            else
            {
                return 0;
            }
        }

        /// <summary>
        /// 提示用户输入角度
        /// </summary>
        /// <param name="message">提示</param>
        /// <returns>用户输入的角度</returns>
        public static double GetAngle(string message)
        {
            Editor ed = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument.Editor;
            PromptDoubleResult i = ed.GetAngle(message);
            if (i.Status == PromptStatus.OK)
            {
                return (double)i.Value;
            }
            else
            {
                return 0;
            }
        }

        /// <summary>
        /// 获取用户输入的矩形角点区域
        /// </summary>
        /// <param name="baseword">基点提示</param>
        /// <param name="targetword">角点提示</param>
        /// /// <returns>用户输入的矩形角点区域</returns>
        public static Extents3d GetCorner(string baseword, string targetword)
        {
            Editor ed = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument.Editor;
            Point3d basept = GetPoint(baseword);
            Point3d targetpt = new Point3d();
            PromptCornerOptions options = new PromptCornerOptions(targetword, basept);
            PromptPointResult i = ed.GetCorner(options);
            if (i.Status == PromptStatus.OK)
            {
                targetpt = i.Value;
            }
            Extents3d ext = new Extents3d();
            ext.AddPoint(targetpt);
            ext.AddPoint(basept);
            return ext;
        }

        /// <summary>
        /// 提示用户拾取点
        /// </summary>
        /// <param name="message">提示</param>
        /// <returns>返回Point3d</returns>
        public static Point3d GetPoint(string message)
        {
            Editor ed = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument.Editor;
            PromptPointResult pt = ed.GetPoint(message);
            if (pt.Status == PromptStatus.OK)
            {
                return (Point3d)pt.Value;
            }
            else
            {
                return new Point3d();
            }
        }

        /// <summary>
        /// 提示用户拾取点集合
        /// </summary>
        /// <returns>点集合</returns>
        public static Point3dCollection GetPoints()
        {
            Editor ed = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument.Editor;
            Point3dCollection p3d = new Point3dCollection();
            string message = "\n请选择点";
            try
            {
                while (true)
                {
                    PromptPointResult pt = ed.GetPoint(message);
                    if (pt.Status == PromptStatus.OK)
                    {
                        message = "\n请选择下一点";
                        p3d.Add(pt.Value);
                    }
                    else if (pt.Status == PromptStatus.Cancel)
                        break;
                }
            }
            catch (Autodesk.AutoCAD.Runtime.Exception ex)
            {
                ShowMsgOperation.Alert(ex.ToString());
            }

            return p3d;
        }

        /// <summary>
        /// 提示用户获取距离
        /// </summary>
        /// <param name="message">提示</param>
        /// <returns></returns>
        public static double GetDistance(string message)
        {
            Editor ed = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument.Editor;
            PromptDoubleResult dis = ed.GetDistance(message);
            if (dis.Status == PromptStatus.OK)
            {
                return dis.Value;
            }
            else
            {
                return 0;
            }
        }

        /// <summary>
        /// 提示框提示用户输入文件名作为文件打开
        /// </summary>
        /// <param name="message">提示</param>
        /// <returns></returns>
        public static string FileNameForOpen(string message)
        {
            Editor ed = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument.Editor;
            PromptFileNameResult i = ed.GetFileNameForOpen(message);
            if (i.Status == PromptStatus.OK)
            {
                return i.StringResult;
            }
            else
            {
                return "";
            }
        }

        /// <summary>
        /// 提示框提示用户输入文件名作为文件保存
        /// </summary>
        /// <param name="message">提示</param>
        /// <returns></returns>
        public static string FileNameForSave(string message)
        {
            Editor ed = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument.Editor;
            PromptFileNameResult i = ed.GetFileNameForSave(message);
            if (i.Status == PromptStatus.OK)
            {
                return i.StringResult;
            }
            else
            {
                return "";
            }
        }

        /// <summary>
        /// 输入文件夹路径
        /// </summary>
        /// <returns></returns>
        public static string Path()
        {
            FolderBrowserDialog path = new FolderBrowserDialog();
            if (path.ShowDialog() == DialogResult.OK)
            {
                return path.SelectedPath.ToString();
            }
            else
            {
                return "";
            }
        }

    }
}
