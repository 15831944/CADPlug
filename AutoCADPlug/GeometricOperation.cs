using System.Collections.Generic;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;
using System;


namespace AutoCADPlug
{
    public class GeometricOperation
    {
        /// <summary>
        /// 角度转化为弧度
        /// </summary>
        /// <param name="angle">角度</param>
        /// <returns>弧度</returns>
        public static double AngToRad(double angle)
        {
            return angle * Math.PI / 180;
        }

        /// <summary>
        /// 弧度转化为角度
        /// </summary>
        /// <param name="Rad">弧度</param>
        /// <returns>角度</returns>
        public static double RadToAng(double rad)
        {
            return rad * 180 / Math.PI;
        }

        /// <summary>
        /// 两点组成的线段与X轴之间夹角
        /// </summary>
        /// <param name="basePt">起点</param>
        /// <param name="endPt">终点</param>
        /// <returns>角度</returns>
        public static double Angle(Point3d startPt, Point3d endPt)
        {
            Line L = new Line(startPt, endPt);
            return L.Angle;
        }

        /// <summary>
        /// 两直线间夹角
        /// </summary>
        /// <param name="line1">线1</param>
        /// <param name="line2">线2</param>
        /// <returns></returns>
        public static double Angle(Line line1, Line line2)
        {
            return Math.Abs(line1.Angle - line2.Angle);
        }

        /// <summary>
        /// 点到曲线距离
        /// </summary>
        /// <param name="point">指定点</param>
        /// <param name="cur">指定线</param>
        /// <param name="bo">是否延伸</param>
        /// <returns>距离</returns>
        public static double Distance(Point3d point, Curve cur, bool bo)
        {
            return cur.GetClosestPointTo(point, bo).DistanceTo(point);
        }

        //这里有误
        /// <summary>
        /// 直线距离
        /// </summary>
        /// <param name="line1">直线1</param>
        /// <param name="line2">直线2</param>
        /// <returns>两线间距离</returns>
        public static double Distance(Line line1, Line line2)
        {
            //if (Relation.Parallel(line1, line2))
            //{
            //    return line1.GetClosestPointTo(line2.StartPoint, true).DistanceTo(line2.StartPoint);
            //}
            //else
            //{
            //    return 0;
            //}

            return 0;
        }

        /// <summary>
        /// 获取与给定点指定角度和距离的点
        /// </summary>
        /// <param name="basePt">基点</param>
        /// <param name="angle">角度</param>
        /// <param name="distance">距离</param>
        /// <returns>相对点</returns>
        public static Point3d RelativePoint(Point3d basePt, double angle, double distance)
        {
            double[] pt = new double[3];
            angle = angle * Math.PI / 180;
            pt[0] = basePt[0] + distance * Math.Cos(angle);
            pt[1] = basePt[1] + distance * Math.Sin(angle);
            pt[2] = basePt[2];
            Point3d point = new Point3d(pt[0], pt[1], pt[2]);
            return point;
        }

        /// <summary>
        /// 获取与给定点相对距离的点
        /// </summary>
        /// <param name="basePt">起点</param>
        /// <param name="x">相对X距离</param>
        /// <param name="y">相对Y距离</param>
        /// <returns>点</returns>
        public static Point3d RelativePoint(Point3d basePt, double x, double y, double z)
        {
            return new Point3d(basePt.X + x, basePt.Y + y, basePt.Z + z);
        }

        /// <summary>
        /// 计算两点中点
        /// </summary>
        /// <param name="pt1">点1</param>
        /// <param name="pt2">点2</param>
        /// <returns>中点</returns>
        public static Point3d MidPoint(Point3d pt1, Point3d pt2)
        {
            return new Point3d((pt1.X + pt2.X) / 2.0, (pt1.Y + pt2.Y) / 2.0, 0.0);
        }

        /// <summary>
        /// 计算线段中点
        /// </summary>
        /// <param name="line">指定线段</param>
        /// <returns>中点</returns>
        public static Point3d MidPoint(Line line)
        {
            return MidPoint(line.StartPoint, line.EndPoint);
        }

        /// <summary>
        /// 判断点是否重合
        /// </summary>
        /// <param name="point1">点1</param>
        /// <param name="point2">点2</param>
        /// <param name="Allowance">容差</param>
        /// <returns>是否重合</returns>
        public static bool Coincide(Point3d point1, Point3d point2, double allowance)
        {
            if (point1.DistanceTo(point2) < allowance)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// 判断线段是否重合
        /// </summary>
        /// <param name="line1">线段1</param>
        /// <param name="line2">线段2</param>
        /// <param name="Allowance">容差</param>
        /// <returns>线段是否重合</returns>
        public static bool Coincide(Line line1, Line line2, double allowance)
        {
            LineSegment3d l1 = new LineSegment3d(line1.StartPoint, line1.EndPoint);
            LineSegment3d l2 = new LineSegment3d(line2.StartPoint, line2.EndPoint);
            Tolerance tol = new Tolerance(allowance, allowance);
            return l1.IsColinearTo(l2, tol);
        }

        /// <summary>
        /// 获取两条线段重合部分
        /// </summary>
        /// <param name="line1">线段1</param>
        /// <param name="line2">线段2</param>
        /// <returns>重合部分</returns>
        public static Line Coincide(Line line1, Line line2)
        {
            LineSegment3d l1 = new LineSegment3d(line1.StartPoint, line1.EndPoint);
            LineSegment3d l2 = new LineSegment3d(line2.StartPoint, line2.EndPoint);
            LinearEntity3d l3 = l1.Overlap(l2);
            return new Line(l3.StartPoint, l3.EndPoint);
        }

        /// <summary>
        /// 判断直线是否平行
        /// </summary>
        /// <param name="line1">直线1</param>
        /// <param name="line2">直线2</param>
        /// <returns>是否平行</returns>
        public static bool Parallel(Line line1, Line line2)
        {
            Plane P = new Plane();
            LineSegment3d l1 = new LineSegment3d(line1.StartPoint, line1.EndPoint);
            LineSegment3d l2 = new LineSegment3d(line2.StartPoint, line2.EndPoint);
            return l1.IsCoplanarWith(l2, out P);
        }

        /// <summary>
        /// 判断直线是否垂直
        /// </summary>
        /// <param name="line1">直线1</param>
        /// <param name="line2">直线2</param>
        /// <param name="Allowance">容差</param>
        /// <returns>两线是否垂直</returns>
        public static bool Perpendicular(Line line1, Line line2, double allowance)
        {
            LineSegment3d l1 = new LineSegment3d(line1.StartPoint, line1.EndPoint);
            LineSegment3d l2 = new LineSegment3d(line2.StartPoint, line2.EndPoint);
            Tolerance tol = new Tolerance(allowance, allowance);
            return l1.IsPerpendicularTo(l2, tol);
        }


    }
}
