using System;
using Autodesk.AutoCAD.DatabaseServices;

namespace AutoCADPlug
{
    public class BlockTableRecordation
    {
        /// <summary>
        /// 将一个实体加入到指定的DataBase的模型空间中
        /// </summary>
        /// <param name="entity">实体对象</param>
        /// <param name="db">数据库</param>
        /// <returns></returns>
        public ObjectId ToModelSpace(Entity entity,Database db)
        {
            ObjectId entityId;
            using (Transaction trans = db.TransactionManager.StartTransaction())
            {
                BlockTable bt = trans.GetObject(db.BlockTableId, OpenMode.ForRead) as BlockTable;
                BlockTableRecord btr = trans.GetObject(bt[BlockTableRecord.ModelSpace], OpenMode.ForWrite) as BlockTableRecord;
                entityId = btr.AppendEntity(entity);
                trans.AddNewlyCreatedDBObject(entity, true);
                trans.Commit();
            }
            return entityId;
        }

        /// <summary>
        /// 将一组实体加入到指定的Database的模型空间
        /// </summary>
        /// <param name="entCollection">要添加的对象集合</param>
        /// <param name="db">数据库</param>
        /// <returns>加入的对象ObjectId集合</returns>
        public ObjectIdCollection ToModelSpace(DBObjectCollection entCollection,Database db)
        {
            ObjectIdCollection objIds = new ObjectIdCollection();
            using (Transaction trans = db.TransactionManager.StartTransaction())
            {
                BlockTable bt = trans.GetObject(db.BlockTableId, OpenMode.ForRead) as BlockTable;
                BlockTableRecord btr = trans.GetObject(bt[BlockTableRecord.ModelSpace], OpenMode.ForWrite) as BlockTableRecord;
                foreach (DBObject obj in entCollection)
                {
                    Entity ent = obj as Entity;
                    if(ent != null)
                    {
                        objIds.Add(btr.AppendEntity(ent));
                        trans.AddNewlyCreatedDBObject(ent, true);
                    }
                    trans.Commit();
                }
            }
            return objIds;
        }
     
    }
}
