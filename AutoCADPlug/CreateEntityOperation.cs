//acdbmgd
using Autodesk.AutoCAD.DatabaseServices;// (Database, DBPoint, Line, Spline)
using Autodesk.AutoCAD.Geometry;//(Point3d, Line3d, Curve3d)
using Autodesk.AutoCAD.ApplicationServices;
using System;

namespace AutoCADPlug
{
    public class CreateEntityOperation
    {
        public static Rectangle3d CreateRectangle(double height,double width,Point3d bPoint)
        {
            Point3d upperLeft = new Point3d(bPoint.X, bPoint.Y + height, 0);
            Point3d upperRight = new Point3d(bPoint.X + width, bPoint.Y + height, 0);
            Point3d lowerLeft = new Point3d(bPoint.X, bPoint.Y, 0);
            Point3d lowerRight = new Point3d(bPoint.X + width, bPoint.Y, 0);

            return new Rectangle3d(upperLeft, upperRight, lowerLeft, lowerRight);
        }

        /// <summary>
        /// 由两点创建直线
        /// </summary>
        /// <param name="p1"></param>
        /// <param name="p2"></param>
        /// <returns></returns>
        public static Line CreateLine(Point3d p1, Point3d p2)
        {
            return new Line(p1, p2);
        }

        /// <summary>
        /// 由两点坐标创建直线
        /// </summary>
        /// <param name="x1"></param>
        /// <param name="y1"></param>
        /// <param name="z1"></param>
        /// <param name="x2"></param>
        /// <param name="y2"></param>
        /// <param name="z2"></param>
        /// <returns></returns>
        public static Line CreateLine(double x1, double y1, double z1, double x2, double y2, double z2)
        {
            return new Line(new Point3d(x1, y1, z1), new Point3d(x2, y2, z2));
        }

        /// <summary>
        /// 由圆心和半径创建圆
        /// </summary>
        /// <param name="cenPt">圆心</param>
        /// <param name="radius">半径</param>
        /// <returns>圆</returns>
        public static Circle CreateCircle(Point3d cenPt, double radius)
        {
            return new Autodesk.AutoCAD.DatabaseServices.Circle(cenPt, Vector3d.ZAxis, radius);
        }

        /// <summary>
        /// 由三点(Point3d)创建圆
        /// </summary>
        /// <param name="pt1">点1</param>
        /// <param name="pt2">点2</param>
        /// <param name="pt3">点3</param>
        /// <returns>过三点的圆</returns>
        public static Circle CreateCircle(Point3d pt1, Point3d pt2, Point3d pt3)
        {
            Vector3d va = pt1.GetVectorTo(pt2);
            Vector3d vb = pt1.GetVectorTo(pt3);
            if (va.GetAngleTo(vb) == 0 || va.GetAngleTo(vb) == Math.PI)
            {
                return new Circle();
            }
            else
            {
                CircularArc3d geoArc = new CircularArc3d(pt1, pt2, pt3);
                Point3d cenPt = new Point3d(geoArc.Center.X, geoArc.Center.Y, 0);
                double radius = geoArc.Radius;
                return new Circle(cenPt, Vector3d.ZAxis, radius);
            }
        }

        ///<summary>
        /// 由圆心、半径、起始角度和终止角度创建圆弧
        /// </summary>
        /// <param name="cenPt">圆心</param>
        /// <param name="radius">半径</param>
        /// <param name="startAng">起始角度</param>
        /// <param name="endAng">终止角度</param>
        /// <returns>圆弧</returns>
        public static Arc CreateArc(Point3d cenPt, double radius, double startAng, double endAng)
        {
            return new Arc(cenPt, radius, startAng, endAng);
        }

        /// <summary>
        /// 由圆心、圆弧起点、圆弧终点创建圆弧
        /// </summary>
        /// <param name="cenPt">圆心</param>
        /// <param name="sPt">起点</param>
        /// <param name="ePt">终点</param>
        /// <returns></returns>
        public static Arc CreateArc(Point3d cenPt, Point3d sPt, Point3d ePt)
        {
            Line l1 = new Line(cenPt, sPt);
            Line l2 = new Line(cenPt, ePt);
            double radius = cenPt.DistanceTo(sPt);
            double startAng = l1.Angle;
            double endAng = l2.Angle;
            return new Arc(cenPt, radius, startAng, endAng);
        }

        /// <summary>
        /// 创建椭圆
        /// </summary>
        /// <param name="center">椭圆中心</param>
        /// <param name="majorAxis">长轴半径</param>
        /// <param name="radiusRatio">短轴半径</param>
        /// <param name="startAngle">起始角度</param>
        /// <param name="endAngle">终止角度</param>
        /// <returns></returns>
        public static Ellipse CreateEllipse(Point3d center, Vector3d majorAxis, double radiusRatio, double startAngle, double endAngle)
        {
            Plane plane = new Plane();
            Ellipse ellipse = new Ellipse(center, plane.Normal, majorAxis, radiusRatio, startAngle, endAngle);
            return ellipse;
        }

        /// <summary>
        /// 由二维点集合和线宽创建二维优化多段线
        /// </summary>
        /// <param name="pts">二维点集合</param>
        /// <param name="width">线宽</param>
        /// <returns>二维多段线</returns>
        public static Polyline CreatePolyline(Point2dCollection pts, double width)
        {
            try
            {
                Polyline ent = new Polyline();
                for (int i = 0; i < pts.Count; i++)
                {
                    ent.AddVertexAt(i, pts[i], 0, width, width);
                }
                return ent;
            }
            catch
            {
                return new Polyline();
            }
        }

        /// <summary>
        /// 由插入点、文字内容、文字样式、文字高度创建单行文字
        /// </summary>
        /// <param name="textString">文字内容</param>
        /// <param name="position">基点</param>
        /// <param name="height">文字高度</param>
        /// <param name="rot">文字转角</param>
        /// <param name="isfield">是否是包含域</param>
        /// <returns></returns>
        public static DBText CreateDBText(string textString, Point3d position, double height, double rot, bool isfield)
        {
            DBText txt = new DBText();
            txt.Position = position;
            txt.Height = height;
            txt.Rotation = rot;
            if (isfield)
            {
                Field field = new Field(textString);
                txt.SetField(field);
            }
            else
                txt.TextString = textString;
            return txt;
        }

        /// <summary>
        /// 由插入点、文字内容、文字高度创建单行文字
        /// </summary>
        /// <param name="position">插入点</param>
        /// <param name="text">文字内容</param>
        /// <param name="height">文字高度</param>
        /// <returns></returns>
        public DBText CreateDBText(Point3d position, string text, double height)
        {
            DBText txt = new DBText();
            txt.Position = position;
            txt.TextString = text;
            txt.Height = height;
            return txt;
        }

        /// <summary>
        /// 由插入点、文字内容、文字样式、文字高度、文字宽度创建多行文字
        /// </summary>
        /// <param name="textString">文字内容</param>
        /// <param name="location">基点</param>
        /// <param name="height">文字高度</param>
        /// <param name="width">宽度</param>
        /// <param name="rot">文字转角</param>
        /// <param name="isfield">是否是包含域</param>
        /// <returns></returns>
        public static MText CreateMtext(string textString, Point3d location, double height, double width, double rot, bool isfield)
        {
            MText txt = new MText();
            txt.Location = location;
            txt.TextHeight = height;
            txt.Width = width;
            txt.Rotation = rot;
            if (isfield)
            {
                Field field = new Field(textString);
                txt.SetField(field);
            }
            else
                txt.Contents = textString;
            return txt;
        }


        /// <summary>
        /// 以默认样式由两条尺寸界线原点和尺寸文本位置创建转角标注
        /// </summary>
        /// <param name="pt1">标注点1</param>
        /// <param name="pt2">标注点2</param>
        /// <param name="ptText">标注文本定位点</param>
        /// <returns></returns>
        public static RotatedDimension CreateRotatedDimension(Point3d pt1, Point3d pt2, Point3d ptText)
        {
            double angle = new Line(pt1, pt2).Angle;
            Database db = HostApplicationServices.WorkingDatabase;
            ObjectId style = db.Dimstyle;
            RotatedDimension ent = new RotatedDimension(angle, pt1, pt2, ptText, "", style);
            return ent;
        }


        /// <summary>
        /// 由尺寸线旋转角度、两条尺寸界线原点、尺寸文本位置、尺寸文本和标注样式创建转角标注
        /// </summary>
        /// <param name="pt1">标注点1</param>
        /// <param name="pt2">标注点2</param>
        /// <param name="ptText">标注文本定位点</param>
        /// <param name="text">标注替换文本</param>
        /// <returns></returns>
        public static RotatedDimension CreateRotatedDimension(Point3d pt1, Point3d pt2, Point3d ptText, string text)
        {
            double angle = new Line(pt1, pt2).Angle;
            Database db = HostApplicationServices.WorkingDatabase;
            ObjectId style = db.Dimstyle;
            RotatedDimension ent = new RotatedDimension(angle, pt1, pt2, ptText, text, style);
            return ent;
        }


        /// <summary>
        /// 创建属性定义
        /// </summary>
        /// <param name="label">标记名</param>
        /// <param name="prompt">提示</param>
        /// <param name="value">属性值</param>
        /// <param name="pt">属性插入点位置</param>
        /// <returns></returns>
        public static AttributeDefinition CreateAttributeDefinition(string label, string prompt, string value, Point3d pt)
        {
            AttributeDefinition ad = new AttributeDefinition();
            ad.Constant = false;
            ad.Tag = label;
            ad.Prompt = prompt;
            ad.TextString = value;
            ad.Position = pt;
            return ad;
        }

        /// <summary>
        /// 创建属性参照
        /// </summary>
        /// <param name="ad"></param>
        /// <returns></returns>
        public static AttributeReference CreateAttributeReference(AttributeDefinition ad)
        {
            AttributeReference ar = new AttributeReference();
            ar.SetAttributeFromBlock(ad, new Matrix3d());
            return ar;
        }


    }
}
