using System;
using System.Collections.Generic;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.ApplicationServices;// (Application, Document)


namespace AutoCADPlug
{
    public class DBOperation
    {
        /// <summary>
        /// 当前工作的数据库
        /// </summary>
        /// <returns></returns>
        public static Database GetWorkingDataBase()
        {
            return HostApplicationServices.WorkingDatabase;
        }

        /// <summary>
        /// 当前活动文档数据库
        /// </summary>
        /// <returns></returns>
        public static Database GetDocumentDatabase()
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
        public static void RemoveObj(DBObject obj)
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
        public static void RemoveCollection(ObjectIdCollection ids)
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
        public static void CloneDatabase(ObjectIdCollection idCollection, string fileName)
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

        /// <summary>
        /// 将一个实体加入到指定的DataBase的模型空间中
        /// </summary>
        /// <param name="entity">实体对象</param>
        /// <param name="db">数据库</param>
        /// <returns></returns>
        public static ObjectId AddToModelSpace(Entity entity)
        {
            ObjectId entityId;
            Database db = Application.DocumentManager.MdiActiveDocument.Database;

            using (DocumentLock docLock = Application.DocumentManager.MdiActiveDocument.LockDocument())
            {
                using (Transaction trans = db.TransactionManager.StartTransaction())
                {
                    BlockTable bt = trans.GetObject(db.BlockTableId, OpenMode.ForRead) as BlockTable;
                    BlockTableRecord btr = trans.GetObject(bt[BlockTableRecord.ModelSpace], OpenMode.ForWrite) as BlockTableRecord;
                    entityId = btr.AppendEntity(entity);
                    trans.AddNewlyCreatedDBObject(entity, true);
                    trans.Commit();
                }
            }
            return entityId;
        }

        public static ObjectId AddToModelSpace(Entity entity, Database db)
        {
            ObjectId entityId;
            using (DocumentLock docLock = Application.DocumentManager.MdiActiveDocument.LockDocument())
            {
                using (Transaction trans = db.TransactionManager.StartTransaction())
                {
                    BlockTable bt = trans.GetObject(db.BlockTableId, OpenMode.ForRead) as BlockTable;
                    BlockTableRecord btr = trans.GetObject(bt[BlockTableRecord.ModelSpace], OpenMode.ForWrite) as BlockTableRecord;
                    entityId = btr.AppendEntity(entity);
                    trans.AddNewlyCreatedDBObject(entity, true);
                    trans.Commit();
                }
            }
            return entityId;
        }

        /// <summary>
        /// 将一组实体加入到指定的Database的模型空间
        /// </summary>
        /// <param name="entCollection">要添加的对象集合</param>
        /// <param name="db">数据库</param>
        /// <returns>加入的对象ObjectId集合</returns>
        public static ObjectIdCollection AddToModelSpace(DBObjectCollection entCollection)
        {
            ObjectIdCollection objIds = new ObjectIdCollection();
            Database db = Application.DocumentManager.MdiActiveDocument.Database;

            using (DocumentLock doclock = Application.DocumentManager.MdiActiveDocument.LockDocument())
            {
                using (Transaction trans = db.TransactionManager.StartTransaction())
                {
                    BlockTable bt = trans.GetObject(db.BlockTableId, OpenMode.ForRead) as BlockTable;
                    BlockTableRecord btr = trans.GetObject(bt[BlockTableRecord.ModelSpace], OpenMode.ForWrite) as BlockTableRecord;
                    foreach (DBObject obj in entCollection)
                    {
                        Entity entity = obj as Entity;

                        objIds.Add(btr.AppendEntity(entity));
                        trans.AddNewlyCreatedDBObject(entity, true);
                    }
                    trans.Commit();
                }
            }
            return objIds;
        }

        public static ObjectIdCollection AddToModelSpace(DBObjectCollection entCollection, Database db)
        {
            ObjectIdCollection objIds = new ObjectIdCollection();

            using (DocumentLock doclock = Application.DocumentManager.MdiActiveDocument.LockDocument())
            {
                using (Transaction trans = db.TransactionManager.StartTransaction())
                {
                    BlockTable bt = trans.GetObject(db.BlockTableId, OpenMode.ForRead) as BlockTable;
                    BlockTableRecord btr = trans.GetObject(bt[BlockTableRecord.ModelSpace], OpenMode.ForWrite) as BlockTableRecord;
                    foreach (DBObject obj in entCollection)
                    {
                        Entity entity = obj as Entity;

                        objIds.Add(btr.AppendEntity(entity));
                        trans.AddNewlyCreatedDBObject(entity, true);
                    }
                    trans.Commit();
                }
            }
            return objIds;
        }

    }

}
