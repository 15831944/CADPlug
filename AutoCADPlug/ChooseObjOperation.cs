using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;
using Autodesk.AutoCAD.EditorInput;
using System;

namespace AutoCADPlug
{
    public  class ChooseObjOperation
    {
        /// <summary>
        /// 类型过滤枚举类
        /// </summary>
        public enum FilterType
        {
            Curve, Dimension, Polyline, BlockRef, Circle, Line, Arc, Text, Mtext, Polyline3d
        }

        /// <summary>
        /// 选择单个实体
        /// </summary>
        /// <param name="message">选择提示</param>
        /// <returns>实体对象</returns>
        public static Entity ChooseEntity(string message)
        {
            Database db = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument.Database;
            Editor ed = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument.Editor;
            Entity entity = null;
            PromptEntityResult ent = ed.GetEntity(message);
            if (ent.Status == PromptStatus.OK)
            {
                using (Transaction transaction = db.TransactionManager.StartTransaction())
                {
                    entity = (Entity)transaction.GetObject(ent.ObjectId, OpenMode.ForRead, true);
                    transaction.Commit();
                }
            }
            return entity;
        }

        /// <summary>
        /// 过滤选择单个实体
        /// </summary>
        /// <param name="optionsWord">过滤提示</param>
        /// <param name="optionsMessage">错误提示</param>
        /// <param name="word">选择提示</param>
        /// <param name="tp">过滤类型</param>
        /// <param name="bo">true表示不包括其基类，false则表示包括其基类</param>
        /// <returns></returns>
        public static Entity ChooseEntity(string optionsWord, string optionsMessage, string word, Type tp, bool bo)
        {
            Database db = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument.Database;
            Editor ed = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument.Editor;
            Entity entity = null;
            PromptEntityOptions entops = new PromptEntityOptions(optionsWord);
            entops.SetRejectMessage(optionsMessage);
            entops.AddAllowedClass(tp, bo); //此处的true表示不包括其基类，false则表示包括其基类
            PromptEntityResult ent = ed.GetEntity(word);
            if (ent.Status == PromptStatus.OK)
            {
                using (Transaction transaction = db.TransactionManager.StartTransaction())
                {
                    entity = (Entity)transaction.GetObject(ent.ObjectId, OpenMode.ForWrite, true);
                    transaction.Commit();
                }
            }
            return entity;
        }

        /// <summary>
        /// 选择集合
        /// </summary>
        /// <returns></returns>
        public static DBObjectCollection ChooseCollection()
        {
            Database db = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument.Database;
            Editor ed = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument.Editor;
            Entity entity = null;
            DBObjectCollection EntityCollection = new DBObjectCollection();
            PromptSelectionResult ents = ed.GetSelection();
            if (ents.Status == PromptStatus.OK)
            {
                using (Transaction transaction = db.TransactionManager.StartTransaction())
                {
                    SelectionSet SS = ents.Value;
                    foreach (ObjectId id in SS.GetObjectIds())
                    {
                        entity = (Entity)transaction.GetObject(id, OpenMode.ForWrite, true);
                        if (entity != null)
                            EntityCollection.Add(entity);
                    }
                    transaction.Commit();
                }
            }
            return EntityCollection;
        }

        /// <summary>
        /// 过滤选择集合
        /// </summary>
        /// <param name="tps">过滤类型</param>
        /// <returns>ObjectId数组</returns>
        public static DBObjectCollection ChooseCollection(FilterType[] tps)
        {
            Database db = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument.Database;
            Editor ed = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument.Editor;
            Entity entity = null;
            DBObjectCollection EntityCollection = new DBObjectCollection();
            PromptSelectionOptions selops = new PromptSelectionOptions();
            // 建立选择的过滤器内容
            TypedValue[] filList = new TypedValue[tps.Length + 2];
            filList[0] = new TypedValue((int)DxfCode.Operator, "<or");
            filList[tps.Length + 1] = new TypedValue((int)DxfCode.Operator, "or>");
            for (int i = 0; i < tps.Length; i++)
            {
                filList[i + 1] = new TypedValue((int)DxfCode.Start, tps[i].ToString());
            }
            // 建立过滤器
            SelectionFilter filter = new SelectionFilter(filList);
            // 按照过滤器进行选择
            PromptSelectionResult ents = ed.GetSelection(selops, filter);
            if (ents.Status == PromptStatus.OK)
            {
                using (Transaction transaction = db.TransactionManager.StartTransaction())
                {
                    SelectionSet SS = ents.Value;
                    foreach (ObjectId id in SS.GetObjectIds())
                    {
                        entity = (Entity)transaction.GetObject(id, OpenMode.ForWrite, true);
                        if (entity != null)
                            EntityCollection.Add(entity);
                    }
                    transaction.Commit();
                }
            }
            return EntityCollection;
        }

        /// <summary>
        /// 选择所有对象
        /// </summary>
        /// <returns></returns>
        public static DBObjectCollection ChooseAll()
        {
            Database db = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument.Database;
            Editor ed = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument.Editor;
            Entity entity = null;
            DBObjectCollection EntityCollection = new DBObjectCollection();
            PromptSelectionResult ents = ed.SelectAll();
            if (ents.Status == PromptStatus.OK)
            {
                using (Transaction transaction = db.TransactionManager.StartTransaction())
                {
                    SelectionSet ss = ents.Value;
                    foreach (ObjectId id in ss.GetObjectIds())
                    {
                        entity = transaction.GetObject(id, OpenMode.ForWrite, true) as Entity;
                        if (entity != null)
                            EntityCollection.Add(entity);
                    }
                    transaction.Commit();
                }
            }
            return EntityCollection;
        }

        /// <summary>
        /// 过滤选择所有对象
        /// </summary>
        /// <returns></returns>
        public static DBObjectCollection ChooseAll(FilterType[] tps)
        {
            Database db = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument.Database;
            Editor ed = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument.Editor;
            Entity entity = null;
            // 建立选择的过滤器内容
            TypedValue[] filList = new TypedValue[tps.Length + 2];
            filList[0] = new TypedValue((int)DxfCode.Operator, "<or");
            filList[tps.Length + 1] = new TypedValue((int)DxfCode.Operator, "or>");
            for (int i = 0; i < tps.Length; i++)
            {
                filList[i + 1] = new TypedValue((int)DxfCode.Start, tps[i].ToString());
            }
            SelectionFilter filter = new SelectionFilter(filList);
            DBObjectCollection EntityCollection = new DBObjectCollection();
            PromptSelectionResult ents = ed.SelectAll(filter);
            if (ents.Status == PromptStatus.OK)
            {
                using (Transaction transaction = db.TransactionManager.StartTransaction())
                {
                    SelectionSet ss = ents.Value;
                    foreach (ObjectId id in ss.GetObjectIds())
                    {
                        entity = transaction.GetObject(id, OpenMode.ForWrite, true) as Entity;
                        if (entity != null)
                            EntityCollection.Add(entity);
                    }
                    transaction.Commit();
                }
            }
            return EntityCollection;
        }

        /// <summary>
        /// 选择多边形内对象
        /// </summary>
        /// <param name="pc">多边形顶点</param>
        /// <returns></returns>
        public static DBObjectCollection ChooseCrossingPolygon(Point3dCollection pc)
        {
            Database db = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument.Database;
            Editor ed = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument.Editor;
            Entity entity = null;
            DBObjectCollection EntityCollection = new DBObjectCollection(); PromptSelectionResult ents = ed.SelectCrossingPolygon(pc);
            if (ents.Status == PromptStatus.OK)
            {
                using (Transaction transaction = db.TransactionManager.StartTransaction())
                {
                    SelectionSet ss = ents.Value;
                    foreach (ObjectId id in ss.GetObjectIds())
                    {
                        entity = transaction.GetObject(id, OpenMode.ForWrite, true) as Entity;
                        if (entity != null)
                            EntityCollection.Add(entity);
                    }
                    transaction.Commit();
                }
            }
            return EntityCollection;
        }

        /// <summary>
        /// 过滤选择多边形内对象
        /// </summary>
        /// <param name="pc">多边形顶点</param>
        /// <param name="tps">类型过滤集合</param>
        /// <returns></returns>
        public static DBObjectCollection ChooseCrossingPolygon(Point3dCollection pc, FilterType[] tps)
        {
            Database db = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument.Database;
            Editor ed = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument.Editor;
            Entity entity = null;
            // 建立选择的过滤器内容
            TypedValue[] filList = new TypedValue[tps.Length + 2];
            filList[0] = new TypedValue((int)DxfCode.Operator, "<or");
            filList[tps.Length + 1] = new TypedValue((int)DxfCode.Operator, "or>");
            for (int i = 0; i < tps.Length; i++)
            {
                filList[i + 1] = new TypedValue((int)DxfCode.Start, tps[i].ToString());
            }
            SelectionFilter filter = new SelectionFilter(filList);
            DBObjectCollection EntityCollection = new DBObjectCollection();
            PromptSelectionResult ents = ed.SelectCrossingPolygon(pc, filter);
            if (ents.Status == PromptStatus.OK)
            {
                using (Transaction transaction = db.TransactionManager.StartTransaction())
                {
                    SelectionSet ss = ents.Value;
                    foreach (ObjectId id in ss.GetObjectIds())
                    {
                        entity = transaction.GetObject(id, OpenMode.ForWrite, true) as Entity;
                        if (entity != null)
                            EntityCollection.Add(entity);
                    }
                    transaction.Commit();
                }
            }
            return EntityCollection;
        }

        /// <summary>
        /// 选择在窗口区域中的对象
        /// </summary>
        /// <param name="pt1">窗口角点1</param>
        /// <param name="pt2">窗口角点2</param>
        /// <returns></returns>
        public static DBObjectCollection ChooseCrossingWindow(Point3d pt1, Point3d pt2)
        {
            Database db = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument.Database;
            Editor ed = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument.Editor;
            Entity entity = null;
            DBObjectCollection EntityCollection = new DBObjectCollection();
            PromptSelectionResult ents = ed.SelectCrossingWindow(pt1, pt2);
            if (ents.Status == PromptStatus.OK)
            {
                using (Transaction transaction = db.TransactionManager.StartTransaction())
                {
                    SelectionSet ss = ents.Value;
                    foreach (ObjectId id in ss.GetObjectIds())
                    {
                        entity = transaction.GetObject(id, OpenMode.ForWrite, true) as Entity;
                        if (entity != null)
                            EntityCollection.Add(entity);
                    }
                    transaction.Commit();
                }
            }
            return EntityCollection;
        }

        /// <summary>
        /// 过滤选择在窗口区域中的对象
        /// </summary>
        /// <param name="pt1">窗口角点1</param>
        /// <param name="pt2">窗口角点2</param>
        /// <param name="tps">类型过滤集合</param>
        /// <returns></returns>
        public static DBObjectCollection ChooseCrossingWindow(Point3d pt1, Point3d pt2, FilterType[] tps)
        {
            Database db = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument.Database;
            Editor ed = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument.Editor;
            Entity entity = null;
            // 建立选择的过滤器内容
            TypedValue[] filList = new TypedValue[tps.Length + 2];
            filList[0] = new TypedValue((int)DxfCode.Operator, "<or");
            filList[tps.Length + 1] = new TypedValue((int)DxfCode.Operator, "or>");
            for (int i = 0; i < tps.Length; i++)
            {
                filList[i + 1] = new TypedValue((int)DxfCode.Start, tps[i].ToString());
            }
            SelectionFilter filter = new SelectionFilter(filList);
            DBObjectCollection EntityCollection = new DBObjectCollection();
            PromptSelectionResult ents = ed.SelectCrossingWindow(pt1, pt2, filter);
            if (ents.Status == PromptStatus.OK)
            {
                using (Transaction transaction = db.TransactionManager.StartTransaction())
                {
                    SelectionSet ss = ents.Value;
                    foreach (ObjectId id in ss.GetObjectIds())
                    {
                        entity = transaction.GetObject(id, OpenMode.ForWrite, true) as Entity;
                        if (entity != null)
                            EntityCollection.Add(entity);
                    }
                    transaction.Commit();
                }
            }
            return EntityCollection;
        }

        /// <summary>
        /// 选择所有隐藏对象
        /// </summary>
        /// <returns></returns>
        public static DBObjectCollection ChooseImplied()
        {
            Database db = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument.Database;
            Editor ed = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument.Editor;
            Entity entity = null;
            DBObjectCollection EntityCollection = new DBObjectCollection();
            PromptSelectionResult ents = ed.SelectImplied();
            if (ents.Status == PromptStatus.OK)
            {
                using (Transaction transaction = db.TransactionManager.StartTransaction())
                {
                    SelectionSet ss = ents.Value;
                    foreach (ObjectId id in ss.GetObjectIds())
                    {
                        entity = transaction.GetObject(id, OpenMode.ForWrite, true) as Entity;
                        if (entity != null)
                            EntityCollection.Add(entity);
                    }
                    transaction.Commit();
                }
            }
            return EntityCollection;
        }

    }
}
