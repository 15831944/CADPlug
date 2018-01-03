using System;
using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Geometry;
using Autodesk.AutoCAD.Runtime;
using Autodesk.AutoCAD.Colors;

namespace AutoCADPlug
{
    public class LayerTableOperation
    {
        ///<summary>
        /// 获得层表ObjectId
        /// </summary>
        /// <param name="db"></param>
        /// <returns></returns>
        public static ObjectId Id(Database db)
        {
            return db.LayerTableId;
        }

        /// <summary>
        /// 获得层表
        /// </summary>
        /// <param name="db"></param>
        /// <returns></returns>
        public static LayerTable GetLayerTable(Database db)
        {
            LayerTable layertable;
            using (Transaction trans = db.TransactionManager.StartTransaction())
            {
                layertable = (LayerTable)trans.GetObject(db.LayerTableId, OpenMode.ForWrite);
            }
            return layertable;
        }

        /// <summary>
        /// 根据图层名获得图层(没有删除标记的)
        /// </summary>
        /// <param name="name">图层名</param>
        /// <param name="db">数据库</param>
        /// <returns></returns>
        public static LayerTableRecord GetLayer(string name, Database db)
        {
            LayerTableRecord layer = new LayerTableRecord();
            using (Transaction trans = db.TransactionManager.StartTransaction())
            {
                LayerTableRecord layer0 = new LayerTableRecord();
                LayerTable lt = (LayerTable)trans.GetObject(db.LayerTableId, OpenMode.ForRead);
                if (lt.Has(name))
                {
                    layer0 = trans.GetObject(lt[name], OpenMode.ForRead) as LayerTableRecord;
                    if (!layer0.IsErased)
                        layer = layer0;
                }
                trans.Commit();
            }
            return layer;
        }

        /// <summary>
        /// 获取当前图层ObjectId
        /// </summary>
        /// <param name="db">数据库</param>
        /// <returns></returns>
        public static LayerTableRecord GetCurrentLayer(Database db)
        {
            LayerTableRecord layer = new LayerTableRecord();
            using (Transaction tr = db.TransactionManager.StartTransaction())
            {
                layer = tr.GetObject(db.Clayer, OpenMode.ForRead) as LayerTableRecord;
            }
            return layer;
        }

        /// <summary>
        /// 设置当前层
        /// </summary>
        /// <param name="layer">要设置为当前的图层</param>
        /// <param name="db">数据库</param>
        public static void SetCurrentLayer(LayerTableRecord layer, Database db)
        {
            if (layer.ObjectId != ObjectId.Null)
                db.Clayer = layer.ObjectId;
        }

        /// <summary>
        /// 新建一个给定名字的图层
        /// </summary>
        /// <param name="layerName">新增图层名</param>
        /// <param name="db">数据库</param>
        /// <returns></returns>
        public static ObjectId AddIn(string layerName, Database db)
        {
            ObjectId layerId = ObjectId.Null;
            using (Transaction trans = db.TransactionManager.StartTransaction())
            {
                LayerTable lt = (LayerTable)trans.GetObject(db.LayerTableId, OpenMode.ForWrite);
                if (!lt.Has(layerName))
                {
                    LayerTableRecord ltr = new LayerTableRecord();
                    ltr.Name = layerName;
                    layerId = lt.Add(ltr);
                    trans.AddNewlyCreatedDBObject(ltr, true);
                }
                trans.Commit();
            }
            return layerId;
        }

        /// <summary>
        /// 建立指定名字，颜色的图层
        /// </summary>
        /// <param name="layerName">新增图层名</param>
        /// <param name="colorIndex">颜色值</param>
        /// <param name="db">数据库</param>
        /// <returns></returns>
        public static ObjectId AddIn(string layerName, short colorIndex, Database db)
        {
            short colorIndex1 = (short)(colorIndex % 256);//防止输入的颜色超出256
            using (Transaction trans = db.TransactionManager.StartTransaction())
            {
                LayerTable lt = (LayerTable)trans.GetObject(db.LayerTableId, OpenMode.ForWrite);
                ObjectId layerId = ObjectId.Null;
                if (lt.Has(layerName) == false)
                {
                    LayerTableRecord ltr = new LayerTableRecord();
                    ltr.Name = layerName;
                    ltr.Color = Color.FromColorIndex(ColorMethod.ByColor, colorIndex1);
                    layerId = lt.Add(ltr);
                    trans.AddNewlyCreatedDBObject(ltr, true);
                }
                trans.Commit();
                return layerId;
            }
        }

        /// <summary>
        /// 删除指定名字的图层
        /// </summary>
        /// <param name="LayerName"></param>
        /// <param name="db"></param>
        public static void RemoveLayer(string layerName, Database db)
        {
            using (Transaction trans = db.TransactionManager.StartTransaction())
            {
                LayerTable lt = (LayerTable)trans.GetObject(db.LayerTableId, OpenMode.ForWrite);
                LayerTableRecord currentLayer = (LayerTableRecord)trans.GetObject(db.Clayer, OpenMode.ForRead);
                if (currentLayer.Name.ToLower() == layerName.ToLower())
                    Application.DocumentManager.MdiActiveDocument.Editor.WriteMessage("\n不能删除当前层");
                else
                {
                    LayerTableRecord ltr = new LayerTableRecord();
                    if (lt.Has(layerName))
                    {
                        ltr = trans.GetObject(lt[layerName], OpenMode.ForWrite) as LayerTableRecord;
                        if (ltr.IsErased)
                            Application.DocumentManager.MdiActiveDocument.Editor.WriteMessage("\n此层已经被删除");
                        else
                        {
                            ObjectIdCollection idCol = new ObjectIdCollection();
                            idCol.Add(ltr.ObjectId);
                            db.Purge(idCol);
                            if (idCol.Count == 0)
                                Application.DocumentManager.MdiActiveDocument.Editor.WriteMessage("\n不能删除包含对象的图层");
                            else
                                ltr.Erase();
                        }
                    }
                    else
                        Application.DocumentManager.MdiActiveDocument.Editor.WriteMessage("\n没有此图层");
                }
                trans.Commit();
            }
        }

        /// <summary>
        /// 删除与图层关联的对象
        /// </summary>
        /// <param name="layer">图层</param>
        /// <param name="db">数据库</param>
        public static void RemoveAllEntFromLayer(LayerTableRecord layer, Database db)
        {
            using (Transaction trans = db.TransactionManager.StartTransaction())
            {
                BlockTable bt = (BlockTable)trans.GetObject(db.BlockTableId, OpenMode.ForWrite);
                foreach (ObjectId btrid in bt)//遍历块表
                {
                    BlockTableRecord btr = (BlockTableRecord)trans.GetObject(btrid, OpenMode.ForWrite);
                    foreach (ObjectId eid in btr)//遍历块表记录
                    {
                        Entity ent = trans.GetObject(eid, OpenMode.ForWrite) as Entity;
                        if (ent != null)
                        {
                            if (ent.LayerId == layer.ObjectId)
                                ent.Erase();
                        }
                    }
                }
                trans.Commit();
            }
        }

        /// <summary>
        /// 根据图层名称，得到图层中所有的实体id
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static ObjectIdCollection GetObjectIdsAtLayer(string name)
        {
            ObjectIdCollection ids = new ObjectIdCollection();
            PromptSelectionResult sResult = null;
            TypedValue[] filtList = new TypedValue[] { new TypedValue((int)DxfCode.LayerName, name) };
            SelectionFilter filter = new SelectionFilter(filtList);
            Editor ed = Application.DocumentManager.MdiActiveDocument.Editor;
            sResult = ed.SelectAll(filter);
            if (sResult.Status == PromptStatus.OK)
            {
                SelectionSet st = sResult.Value;
                ObjectId[] oids = st.GetObjectIds();

                foreach (ObjectId id in oids)
                {
                    ids.Add(id);
                }
            }
            return ids;
        }

    }
}
