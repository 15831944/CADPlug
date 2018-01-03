using System;
using System.Windows.Forms;
using Autodesk.AutoCAD.Interop;
using System.Reflection;

//acdbmgd
using Autodesk.AutoCAD.DatabaseServices;// (Database, DBPoint, Line, Spline)
using Autodesk.AutoCAD.Geometry;//(Point3d, Line3d, Curve3d)
using Autodesk.AutoCAD.Runtime;// (CommandMethodAttribute, RXObject, CommandFlag)
using Autodesk.AutoCAD.Colors;
using Autodesk.AutoCAD.GraphicsInterface;
using Autodesk.AutoCAD.LayerManager;

//acmgd
using Autodesk.AutoCAD.ApplicationServices;// (Application, Document)
using Autodesk.AutoCAD.EditorInput;//(Editor, PromptXOptions, PromptXResult)
using Autodesk.AutoCAD.Windows;
using Autodesk.AutoCAD.Internal.Windows;
using Autodesk.AutoCAD.Internal.Forms;
using Autodesk.AutoCAD.Windows.ToolPalette;




namespace AutoCADPlug
{
    public partial class MyControl : UserControl
    {
        private static Point3d bPoint = Point3d.Origin;

        public MyControl()
        {
            InitializeComponent();
        }

        /// <summary>
        /// 自动绘制梁
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button1_Click_1(object sender, EventArgs e)
        {
            try
            {
                //创建矩形
                double height = Convert.ToDouble(nudBeamHeight.Value);
                double width = Convert.ToDouble(nudBeamTopHeight.Value);

                Rectangle3d rec3d = CreateEntityOperation.CreateRectangle(height, width, bPoint);
                MyRectangle rect = new MyRectangle(rec3d);

                //将矩形添加到数据库中
                Database db = DBOperation.GetDocumentDatabase();
                DBOperation.AddToModelSpace(rect.CreateRect(), db);

            }
            catch (Autodesk.AutoCAD.Runtime.Exception ex)
            {
                ShowMsgOperation.Message(ex.Data + "\n" + ex.Source + "\n" + ex.TargetSite + "\n" + ex.ToString());
            }
        }

        /// <summary>
        /// 转化已有图
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button2_Click(object sender, EventArgs e)
        {

        }

        /// <summary>
        /// 选取插入的基点
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button3_Click(object sender, EventArgs e)
        {
            Editor ed = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument.Editor;

            PromptPointResult pt = ed.GetPoint("\n请选择点");
            if (pt.Status == PromptStatus.OK)
            {
                bPoint = (Point3d)pt.Value;
                ShowMsgOperation.Message("您选择了点："+bPoint.X+","+bPoint.Y+"\n");
            }
            else
            {
                ShowMsgOperation.Message("未选择点，默认为原点");
            }
            lblPointValue.Text = bPoint.X.ToString() + " , " + bPoint.Y.ToString();
            this.Focus();
        }



    }
}
