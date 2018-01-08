using System;
using System.Windows.Forms;

using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;
using Autodesk.AutoCAD.Runtime;
using Autodesk.AutoCAD.Colors;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.ApplicationServices;

namespace AutoCADPlug
{
    public partial class MyForm : Form
    {
        private static string userName = "admin";
        private static string passwd = "admin";


        public MyForm()
        {
            InitializeComponent();
        }

        //private void button1_Click(object sender, EventArgs e)
        //{
        //    Editor ed = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument.Editor;
        //    using(EditorUserInteraction edUsrInt = ed.StartUserInteraction(this))
        //    {
        //        Point3d pt = GetPoint("/n选择点：");
        //        this.textBox1.Text = "(" + pt.X.ToString() + "," + pt.Y.ToString() +","+pt.Z.ToString()+ ")";
        //        edUsrInt.End();
        //        this.Focus();
        //    }
        //}

        //public Point3d GetPoint(string word)
        //{
        //    Document doc = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument;
        //    Editor ed = doc.Editor;
        //    PromptPointResult pt = ed.GetPoint(word);
        //    if(pt.Status == PromptStatus.OK)
        //    {
        //        return (Point3d)pt.Value;
        //    }
        //    else
        //    {
        //        return new Point3d();
        //    }
        //}

        private void button2_Click(object sender, EventArgs e)
        {
            if(userName == tbxUserName.Text && passwd == tbxPasswd.Text)
            {
                this.DialogResult  = DialogResult.OK;
                this.Close();
            }
            else
            {
                lblShowMsg.Visible = true;
                lblShowMsg.Text = "密码错误，请重新输入！";
                lblShowMsg.ForeColor = System.Drawing.Color.Red;
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

    

      
    }
}
