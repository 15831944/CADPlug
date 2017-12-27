//acdbmgd
using Autodesk.AutoCAD.DatabaseServices;// (Database, DBPoint, Line, Spline)
using Autodesk.AutoCAD.Geometry;//(Point3d, Line3d, Curve3d)
using Autodesk.AutoCAD.Runtime;// (CommandMethodAttribute, RXObject, CommandFlag)
using Autodesk.AutoCAD.Colors;
using Autodesk.AutoCAD.GraphicsInterface;
using Autodesk.AutoCAD.LayerManager;

//acmgd
using Autodesk.AutoCAD.ApplicationServices;// (Application, Document)
using Autodesk.AutoCAD.EditorInput;//(Editor, PromptXOptions, PromptXResult)
using Autodesk.AutoCAD.Windows;
using Autodesk.AutoCAD.Internal.Windows;
using Autodesk.AutoCAD.Internal.Forms;
using Autodesk.AutoCAD.Windows.ToolPalette;

using System;
using System.Reflection;


[assembly:CommandClass(typeof(AutoCADPlug.CAD))]
namespace AutoCADPlug
{
    public class CAD : IExtensionApplication
    {
        ContextMenuExtension m_ContextMenu;//定义右键菜单 
        PaletteSet palSet;//定义工具栏按钮

        //初始化方法，这里加了一个面板工具栏和右键菜单。如果不要右键菜单，注释即可
        public void Initialize()
        {
            AddContextMenu();//添加右键菜单
            HelloWorld();
        }

        //卸载方法
        public void Terminate()
        { 
            RemoveContextMenu();
        }

        //有CommandMethod标注，是提供给CAD使用的命令
        [CommandMethod("HelloWorld")]
        public void HelloWorld()
        {
            //这段代码的作用是弹出一个提示
            Editor ed = Application.DocumentManager.MdiActiveDocument.Editor;
            ed.WriteMessage("第一个CAD插件程序！");
        }

        #region 添加一个右键菜单，并实现画一个圆的功能
        /// <summary>点击响应事件，创建一个圆
        /// 
        /// </summary>
        /// <param name="o"></param>
        /// <param name="e"></param>
        private void MyMenuItem_OnClick(object o, EventArgs e)
        {
            using (DocumentLock doclock = Application.DocumentManager.MdiActiveDocument.LockDocument())
            {
                //创建一个红色的圆
                Database db = HostApplicationServices.WorkingDatabase;
                using (Transaction trans = db.TransactionManager.StartTransaction())
                {
                    BlockTable bt = trans.GetObject(db.BlockTableId, OpenMode.ForRead) as BlockTable;
                    BlockTableRecord btr = trans.GetObject(bt[BlockTableRecord.ModelSpace], OpenMode.ForWrite) as BlockTableRecord;
                    Circle cir = new Circle(new Point3d(10, 10, 0), Vector3d.ZAxis, 100);
                    cir.ColorIndex = 1;
                    btr.AppendEntity(cir);
                    trans.AddNewlyCreatedDBObject(cir, true);
                    trans.Commit();
                }
            }
        }

        /// <summary>添加右键菜单项
        /// 
        /// </summary>
        private void AddContextMenu()
        {
            m_ContextMenu = new ContextMenuExtension();
            m_ContextMenu.Title = "PSW自定义菜单";
            Autodesk.AutoCAD.Windows.MenuItem mi;
            mi = new Autodesk.AutoCAD.Windows.MenuItem("创建圆");
            mi.Click += MyMenuItem_OnClick;

            m_ContextMenu.MenuItems.Add(mi);

            Application.AddDefaultContextMenuExtension(m_ContextMenu);
        }

        /// <summary>移除菜单项
        /// 
        /// </summary>
        private void RemoveContextMenu()
        {
            if (m_ContextMenu != null)
            {

                Application.RemoveDefaultContextMenuExtension(m_ContextMenu);
                m_ContextMenu = null;
            }
        }
        #endregion

        public void test1()
        {
            ContextMenuExtension menuExt = new ContextMenuExtension();
            menuExt.Title = "psw_test";
            MenuItem item = new MenuItem("test");

            menuExt.MenuItems.Add(item);

            Application.AddDefaultContextMenuExtension(menuExt);

        }

        public void test2()
        {
            Document doc = Application.DocumentManager.MdiActiveDocument;
            DocumentLock docLock = doc.LockDocument();
            Database db = HostApplicationServices.WorkingDatabase;
            using (Transaction tran = db.TransactionManager.StartTransaction())
            {
                BlockTable bt = tran.GetObject(db.BlockTableId, OpenMode.ForRead) as BlockTable;

            }
        }

        public void database_test()
        {
            Database db = HostApplicationServices.WorkingDatabase;
            ObjectId layer =  db.Clayer;
        }

        [CommandMethod("psw")]
        public void test3()
        {
            Editor editor = Application.DocumentManager.MdiActiveDocument.Editor;
            //double data = InputOperation.Double("请输入数据！");
            //ShowMsgOperation.Alert(data.ToString());
        }


        #region 用户交互：自定义用户界面

        [CommandMethod("ShowModalDialog")]
        public void ShowModalDialog()
        {
            using (MyForm form = new MyForm())
            {
                form.ShowInTaskbar = false;
                Application.ShowModalDialog(form);
                if (form.DialogResult == System.Windows.Forms.DialogResult.OK)
                {
                    Application.DocumentManager.MdiActiveDocument.Editor.WriteMessage("\n" + form.Name);
                }
            }
        }

        #endregion

        #region 添加块记录
        [CommandMethod("AddBlockDef")]
        public void addBlockDef()
        {
            Database db = HostApplicationServices.WorkingDatabase;
            BlockTableRecord btr = new BlockTableRecord();
            btr.Name = "psw_cad";
            Line line = new Line(Point3d.Origin, new Point3d(10, 15, 0));
            line.Color = Autodesk.AutoCAD.Colors.Color.FromColorIndex(Autodesk.AutoCAD.Colors.ColorMethod.ByLayer, 1);
            Circle circle = new Circle(Point3d.Origin, Vector3d.ZAxis, 10);
            circle.Color = Autodesk.AutoCAD.Colors.Color.FromColorIndex(Autodesk.AutoCAD.Colors.ColorMethod.ByLayer, 1);
            btr.AppendEntity(line);
            btr.AppendEntity(circle);
            AddBlockTableRecord(btr, db);
        }

        public ObjectId AddBlockTableRecord(BlockTableRecord btr, Database db)
        {
            ObjectId id = new ObjectId();
            try
            {
                using (Transaction trans = db.TransactionManager.StartTransaction())
                {
                    BlockTable bt = trans.GetObject(db.BlockTableId, OpenMode.ForWrite) as BlockTable;
                    id = bt.Add(btr);
                    trans.AddNewlyCreatedDBObject(btr, true);
                    trans.Commit();
                }
            }
            catch (Autodesk.AutoCAD.Runtime.Exception ex)
            {
                Autodesk.AutoCAD.ApplicationServices.Application.ShowAlertDialog(ex.Message);
            }
            return id;

        }

        #endregion

        #region 添加实体到模型空间

        [CommandMethod("AddEntToModelSpace")]
        public void AddEntToModelSpace()
        {
            Database db = HostApplicationServices.WorkingDatabase;
            DBText txt = new DBText();
            txt.Position = new Point3d();
            txt.TextString = "PSWCAD.org";
            ToModalSpace(txt, db);
        }

        public ObjectId ToModalSpace(DBText txt, Database db)
        {
            ObjectId id;
            using (Transaction trans = db.TransactionManager.StartTransaction())
            {
                BlockTable bt = trans.GetObject(db.BlockTableId, OpenMode.ForRead) as BlockTable;
                BlockTableRecord btr = trans.GetObject(bt[BlockTableRecord.ModelSpace], OpenMode.ForWrite) as BlockTableRecord;
                id = btr.AppendEntity(txt);
                trans.AddNewlyCreatedDBObject(txt, true);
                trans.Commit();
            }
            return id;
        }

        #endregion

        #region 添加和删除图层

        [CommandMethod("AddLayer")]
        public void AddLayer()
        {
            Database db = HostApplicationServices.WorkingDatabase;
            AddLayerTableRecord("PSWCAD.org", 1, db);
        }
        public ObjectId AddLayerTableRecord(string layerName, short colorIndex, Database db)
        {
            short cindex = (short)(colorIndex % 256);
            using (Transaction trans = db.TransactionManager.StartTransaction())
            {
                LayerTable lt = trans.GetObject(db.LayerTableId, OpenMode.ForWrite) as LayerTable;
                ObjectId layerId = ObjectId.Null;
                if (lt.Has(layerName) == false)
                {
                    LayerTableRecord ltr = new LayerTableRecord();
                    ltr.Name = layerName;
                    ltr.Color = Color.FromColorIndex(ColorMethod.ByColor, colorIndex);
                    layerId = lt.Add(ltr);
                    trans.AddNewlyCreatedDBObject(ltr, true);
                }
                trans.Commit();
                return layerId;
            }

        }

        [CommandMethod("RemoveLayer")]
        public void RemoveLayer()
        {
            Database db = HostApplicationServices.WorkingDatabase;
            RemoveLayerTableRecord("PSWCAD", db);
        }

        public void RemoveLayerTableRecord(string layerName, Database db)
        {
            Editor ed = Application.DocumentManager.MdiActiveDocument.Editor;
            using (Transaction trans = db.TransactionManager.StartTransaction())
            {
                LayerTable lt = trans.GetObject(db.LayerTableId, OpenMode.ForWrite) as LayerTable;
                LayerTableRecord ltr = trans.GetObject(db.Clayer, OpenMode.ForRead) as LayerTableRecord;
                if (ltr.Name.ToLower() == layerName.ToLower())
                {
                    ed.WriteMessage("\n不能删除当前层");
                }
                else
                {
                    ltr = new LayerTableRecord();
                    if (lt.Has(layerName))
                    {
                        ltr = trans.GetObject(lt[layerName], OpenMode.ForWrite) as LayerTableRecord;
                        if (ltr.IsErased)
                        {
                            ed.WriteMessage("\n此图层已经被删除");
                        }
                        else
                        {
                            ObjectIdCollection ids = new ObjectIdCollection();
                            ids.Add(ltr.ObjectId);
                            db.Purge(ids);
                            if (ids.Count == 0)
                            {
                                ed.WriteMessage("不能删除包含对象的图层");
                            }
                            else
                            {
                                ltr.Erase();
                            }
                        }
                    }
                    else
                    {
                        ed.WriteMessage("没有此图层");
                    }
                }
                trans.Commit();
            }
        }
        #endregion

        #region 写块克隆

        [CommandMethod("WriteBlockClone")]
        public void WriteBlockClone()
        {
            ObjectIdCollection ids = new ObjectIdCollection();
            foreach (DBObject obj in Collection())
            {
                ids.Add(obj.ObjectId);
            }
            if (ids.Count != 0)
            {
                WClone(ids, @"C:\Users\admin\Desktop\test.dwg");

            }
        }

        public void WClone(ObjectIdCollection ids, string fileName)
        {
            Database tdb = new Database(true, true);
            ObjectId objId = new ObjectId();
            Database fdb = ids[0].Database;
            IdMapping map = new IdMapping();
            using (Transaction trans = tdb.TransactionManager.StartTransaction())
            {
                BlockTable bt = trans.GetObject(tdb.BlockTableId, OpenMode.ForRead) as BlockTable;
                BlockTableRecord btr = trans.GetObject(bt[BlockTableRecord.ModelSpace], OpenMode.ForRead) as BlockTableRecord;
                objId = btr.ObjectId;
                trans.Commit();
            }
            fdb.WblockCloneObjects(ids, objId, map, DuplicateRecordCloning.Replace, false);
            tdb.SaveAs(fileName, DwgVersion.Current);
        }

        public DBObjectCollection Collection()
        {
            Document doc = Application.DocumentManager.MdiActiveDocument;
            Database db = doc.Database;
            Editor editor = doc.Editor;
            Entity ent = null;
            DBObjectCollection entityCollection = new DBObjectCollection();
            PromptSelectionResult ents = editor.GetSelection();
            if (ents.Status == PromptStatus.OK)
            {
                using (Transaction trans = db.TransactionManager.StartTransaction())
                {
                    SelectionSet ss = ents.Value;
                    foreach (ObjectId id in ss.GetObjectIds())
                    {
                        ent = trans.GetObject(id, OpenMode.ForWrite, true) as Entity;
                        if (ent != null)
                        {
                            entityCollection.Add(ent);
                        }
                    }
                    trans.Commit();
                }
            }
            return entityCollection;


        }

        #endregion

        #region 设置当前视图

        ///程序提示选择一个对象，根据其边界设置当前视图的范围刚好完全显示。
        ///本例并没有直接对视图表进行操作，而是通过Editor的SetCurrentView方法
        ///设置了当前视图
        [CommandMethod("SetCurrentView")]
        public void SetCurrentView()
        {
            Entity ent = Select("\n请选择一个实体");
            if (ent != null)
            {
                ViewEntity(ent, 1.2);
            }
        }

        public void ViewEntity(Entity ent, double scale)
        {
            Database db = ent.Database;
            Editor ed = Application.DocumentManager.MdiActiveDocument.Editor;
            using (ViewTableRecord viewtr = new ViewTableRecord())
            {
                Point2d ptMin = new Point2d(ent.GeometricExtents.MinPoint.X, ent.GeometricExtents.MinPoint.Y);
                Point2d ptMax = new Point2d(ent.GeometricExtents.MaxPoint.X, ent.GeometricExtents.MaxPoint.Y);
                viewtr.CenterPoint = new Point2d((ptMin.X + ptMax.X) / 2, (ptMin.Y + ptMax.Y) / 2);
                viewtr.Width = Math.Abs(ptMax.X - ptMin.X) * scale;
                viewtr.Height = Math.Abs(ptMax.Y - ptMin.Y) * scale;
                ed.SetCurrentView(viewtr);
            }
        }

        public Entity Select(string message)
        {
            Document doc = Application.DocumentManager.MdiActiveDocument;
            Database db = doc.Database;
            Editor ed = doc.Editor;
            Entity ent = null;
            PromptEntityResult res = ed.GetEntity(message);
            if (res.Status == PromptStatus.OK)
            {
                using (Transaction trans = db.TransactionManager.StartTransaction())
                {
                    ent = trans.GetObject(res.ObjectId, OpenMode.ForRead, true) as Entity;
                    trans.Commit();
                }
            }
            return ent;
        }

        #endregion

        #region 引用外部文件为块定义创建块参照

        [CommandMethod("AddRefBlock")]
        public void AddRefBlock()
        {
            Database db = HostApplicationServices.WorkingDatabase;
            using (Transaction trans = db.TransactionManager.StartTransaction())
            {
                BlockTable bt = trans.GetObject(db.BlockTableId, OpenMode.ForRead) as BlockTable;
                BlockTableRecord btr = trans.GetObject(bt[BlockTableRecord.ModelSpace], OpenMode.ForWrite) as BlockTableRecord;
                ObjectId refid = db.OverlayXref(@"C:\Users\admin\Desktop\psw.dwg", "psw");// 把外部文件转换为块定义
                BlockReference br = new BlockReference(Point3d.Origin, refid); // 通过块定义创建块参照
                btr.AppendEntity(br); //把块参照添加到块表记录
                trans.AddNewlyCreatedDBObject(br, true); // 通过事务添加块参照到数据库
                trans.Commit();
            }
        }

        #endregion

    }
}
