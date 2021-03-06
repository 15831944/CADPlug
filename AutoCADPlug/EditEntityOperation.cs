﻿using System.Collections.Generic;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;
using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.EditorInput;

namespace AutoCADPlug
{
    public class EditEntityOperation
    {
        /// <summary>
        /// 指定基点与目标点移动实体
        /// </summary>
        /// <param name="ent">实体对象</param>
        /// <param name="BasePt">基点</param>
        /// <param name="TargetPt">目标点</param>
        public static void Move(Entity ent, Point3d basePt, Point3d targetPt)
        {
            Vector3d vec = targetPt - basePt;
            Matrix3d mt = Matrix3d.Displacement(vec);
            ent.TransformBy(mt);
        }

        /// <summary>
        /// 指定基点与目标点复制实体
        /// </summary>
        /// <param name="ent">实体对象</param>
        /// <param name="sourcePt">基点</param>
        /// <param name="targetPt">目标点</param>
        /// <returns>复制的实体对象</returns>
        public static Entity CopyTo(Entity ent, Point3d sourcePt, Point3d targetPt)
        {
            Matrix3d mt = Matrix3d.Displacement(targetPt - sourcePt);
            Entity entCopy = ent.GetTransformedCopy(mt);
            return entCopy;
        }

        /// <summary>
        /// 指定基点与旋转角度旋转实体
        /// </summary>
        /// <param name="ent">实体对象</param>
        /// <param name="basePt">基点</param>
        /// <param name="angle">旋转角度</param>
        /// <param name="Axis">旋转轴(XY平面内旋转则设为Vector3d.ZAxis)</param>
        public static void Rotate(Entity ent, Point3d basePt, double angle, Vector3d Axis)
        {
            Matrix3d mt = Matrix3d.Rotation(angle, Axis, basePt);
            ent.TransformBy(mt);
        }

        /// <summary>
        /// 指定基点与比例缩放实体
        /// </summary>
        /// <param name="ent">实体对象</param>
        /// <param name="basePt">基点</param>
        /// <param name="scaleFactor">缩放比例</param>
        public static void Scale(Entity ent, Point3d basePt, double scaleFactor)
        {
            Matrix3d mt = Matrix3d.Scaling(scaleFactor, basePt);
            ent.TransformBy(mt);
        }

        /// <summary>
        /// 按照参照点镜像实体
        /// </summary>
        /// <param name="ent">实体对象</param>
        /// <param name="mirrorPt1">镜像点1</param>
        /// <param name="mirrorPt2">镜像点2</param>
        public static void Mirror(Entity ent, Point3d mirrorPt1, Point3d mirrorPt2)
        {
            Line3d mirrorLine = new Line3d(mirrorPt1, mirrorPt2);
            Matrix3d mt = Matrix3d.Mirroring(mirrorLine);
            ent.TransformBy(mt);
        }

        /// <summary>
        /// 按照指定直线镜像实体
        /// </summary>
        /// <param name="ent"></param>
        /// <param name="line"></param>
        public static void Mirror(Entity ent, Line line)
        {
            Line3d mirrorLine = new Line3d(line.StartPoint, line.EndPoint);
            Matrix3d mt = Matrix3d.Mirroring(mirrorLine);
            ent.TransformBy(mt);
        }

        /// <summary>
        /// 获得指定偏移距的偏移对象
        /// </summary>
        /// <param name="cur">待偏移曲线</param>
        /// <param name="dis">便宜距离</param>
        /// <returns>偏移后对象集合</returns>
        public static List<Curve> Offset(Curve cur, double dis)
        {
            ObjectId ModelSpaseID =
            SymbolUtilityServices.GetBlockModelSpaceId(HostApplicationServices.WorkingDatabase);
            List<Curve> Cl = new List<Curve>();
            try
            {
                DBObjectCollection offsetCur = cur.GetOffsetCurves(dis);
                foreach (var i in offsetCur)
                {
                    Cl.Add((Curve)i);
                }
            }
            catch
            {
            }
            return Cl;
        }

        /// <summary>
        /// 指定行数、列数、行距、列距矩形阵列实体
        /// </summary>
        /// <param name="ent">要阵列的对象</param>
        /// <param name="numRows">行数</param>
        /// <param name="numCols">列数</param>
        /// <param name="disRows">行距</param>
        /// <param name="disCols">列距</param>
        /// <returns>阵列后的对象集合</returns>
        public static Entity[] ArrayRectang(Entity ent, int numRows, int numCols, double disRows, double disCols)
        {
            Entity[] ents = new Entity[numRows * numCols];
            int N = 0;
            for (int m = 0; m < numRows; m++)
            {
                for (int n = 0; n < numCols; n++)
                {
                    Matrix3d mt = Matrix3d.Displacement(new Vector3d(n * disCols, m * disRows, 0));
                    ents[N] = ent.GetTransformedCopy(mt);
                    N++;
                }
            }
            return ents;
        }

        /// <summary>
        /// 指定圆心、阵列数量、角度圆形阵列实体
        /// </summary>
        /// <param name="ent">要整列的实体对象</param>
        /// <param name="cenPt">阵列圆心</param>
        /// <param name="numObj">阵列数量</param> 
        /// <param name="Angle">角度</param>
        /// <returns>阵列后的实体对象</returns>
        public static Entity[] ArrayPolar(Entity ent, Point3d cenPt, int numObj, double Angle)
        {
            Entity[] ents = new Entity[numObj];
            ents[0] = ent;
            for (int i = 1; i < numObj; i++)
            {
                Matrix3d mt = Matrix3d.Rotation(Angle * i / numObj, Vector3d.ZAxis, cenPt);
                ents[i] = ent.GetTransformedCopy(mt);
            }
            return ents;
        }

        /// <summary>
        /// 提示用户选择实体，改变选择的所选实体的颜色
        /// </summary>
        /// <param name="colorIndex">需要变成的颜色</param>
        public static void ChangeEntityColor(int colorIndex)
        {
            //获取当前文档和数据库
            Document doc = Application.DocumentManager.MdiActiveDocument;
            Database db = doc.Database;
            //启动事务
            using (Transaction trans = db.TransactionManager.StartTransaction())
            {
                //请求在图形区域选择对象
                PromptSelectionResult psr = doc.Editor.GetSelection();
                //如果提示状态OK，表示已选择对象
                if (psr.Status == PromptStatus.OK)
                {
                    SelectionSet ss = psr.Value;
                    //遍历选择集内的对象
                    foreach (SelectedObject sobj in ss)
                    {
                        //确认返回的是合法的SelectedObject对象
                        if (sobj != null)
                        {
                            //以写打开所选对象
                            Entity ent = trans.GetObject(sobj.ObjectId,
                            OpenMode.ForWrite) as Entity;
                            if (ent != null)
                            {
                                //将对象颜色修改为绿色，绿色：3
                                ent.ColorIndex = colorIndex;
                            }
                        }
                    }
                    //保存新对象到数据库
                    trans.Commit();
                }
                //关闭事务
            }
        }

    }

}
