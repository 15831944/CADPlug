using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;
using Autodesk.AutoCAD.Runtime;
using Autodesk.AutoCAD.Colors;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.ApplicationServices;

namespace AutoCADPlug
{
    public class MyRectangle 
    {
        public MyRectangle(Rectangle3d rect3d)
        {
            LowerLeft = rect3d.LowerLeft;
            LowerRight = rect3d.LowerRight;
            UpperLeft = rect3d.UpperLeft;
            UpperRight = rect3d.UpperRight;
        }

        public Point3d LowerLeft { get; set; }
        public Point3d LowerRight { get; set; }
        public Point3d UpperLeft { get; set; }
        public Point3d UpperRight { get; set; }

        public DBObjectCollection CreateRect()
        {
            DBObjectCollection objs = new DBObjectCollection();
            Line line2 = new Line(LowerLeft, UpperLeft);
            Line line3 = new Line(UpperLeft, UpperRight);
            Line line4 = new Line(UpperRight, LowerRight);
            Line line1 = new Line( LowerRight,LowerLeft);
            objs.Add(line1);
            objs.Add(line2);
            objs.Add(line3);
            objs.Add(line4);
            return objs;
        }

        //public sealed override string ToString()
        //public string ToString(IFormatProvider provider);



    }
}
