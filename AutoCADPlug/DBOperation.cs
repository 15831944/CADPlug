using System;
using System.Collections.Generic;
using Autodesk.AutoCAD.DatabaseServices;

namespace AutoCADPlug
{
    public class DBOperation
    {
        /// <summary>
        /// 当前工作的数据库
        /// </summary>
        /// <returns></returns>
        public static Database WorkingDataBase()
        {
            return HostApplicationServices.WorkingDatabase;
        }

        /// <summary>
        /// 当前活动文档数据库
        /// </summary>
        /// <returns></returns>
        public static Database DocumentDatabase()
        {
            return Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument.Database;
        }

        /// <summary>
        /// 由对象ObjectId获得对象
        /// </summary>
        /// <param name="id">ObjectId</param>
        /// <param name="mode">打开模式</param>
        /// <param name="db">数据库</param>
        /// <returns></returns>
        public static DBObject GetObject(ObjectId id, OpenMode mode, Database db)
        {
            DBObject Obj;
            using (Transaction tr = db.TransactionManager.StartTransaction())
            {
                Obj = tr.GetObject(id, mode) as DBObject;
                tr.Commit();
            }
            return Obj;
        }

        /// <summary>
        /// 删除单个对象
        /// </summary>
        /// <param name="obj">要删除对象</param>
        public static void Remove(DBObject obj)
        {
            Database db = obj.Database;
            using (Transaction trans = db.TransactionManager.StartTransaction())
            {
                obj.Erase();
                trans.Commit();
            }
        }

        /// <summary>
        /// 删除ObjectId集合中的对象
        /// </summary>
        /// <param name="ids"></param>
        public static void Remove(ObjectIdCollection ids)
        {
            if (ids.Count == 0)
            {
                return;
            }
            //获得所选对象第一个ID所在的数据库
            Database db = ids[0].OriginalDatabase;
            using (Transaction trans = db.TransactionManager.StartTransaction())
            {
                Entity ent;
                foreach (ObjectId id in ids)
                {
                    ent = trans.GetObject(id, OpenMode.ForWrite) as Entity;
                    if (ent != null)
                        ent.Erase();
                }
                trans.Commit();
            }
        }

        /// <summary>
        /// 数据库克隆
        /// </summary>
        /// <param name="idCollection">克隆的对象ID集合</param>
        /// <param name="fileName">克隆到的文件名</param>
        public static void Clone(ObjectIdCollection idCollection, string fileName)
        {
            Database ndb = new Database(true, true);
            ObjectId IdBtr = new ObjectId();
            Database db = idCollection[0].Database;
            IdMapping map = new IdMapping();
            using (Transaction trans = ndb.TransactionManager.StartTransaction())
            {
                BlockTable bt = (BlockTable)trans.GetObject(ndb.BlockTableId, OpenMode.ForRead);
                BlockTableRecord btr = (BlockTableRecord)trans.GetObject(bt[BlockTableRecord.ModelSpace], OpenMode.ForRead);
                IdBtr = btr.ObjectId;
                trans.Commit();
            }
            db.WblockCloneObjects(idCollection, IdBtr, map, DuplicateRecordCloning.Replace, false);
            ndb.SaveAs(fileName, DwgVersion.Current);
        }

    }

}
