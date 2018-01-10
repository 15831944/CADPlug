//acdbmgd
using Autodesk.AutoCAD.DatabaseServices;// (Database, DBPoint, Line, Spline)
using Autodesk.AutoCAD.Geometry;//(Point3d, Line3d, Curve3d)
using Autodesk.AutoCAD.Runtime;// (CommandMethodAttribute, RXObject, CommandFlag)
using Autodesk.AutoCAD.Colors;
//using Autodesk.AutoCAD.GraphicsInterface;
using Autodesk.AutoCAD.LayerManager;

//acmgd
using Autodesk.AutoCAD.ApplicationServices;// (Application, Document)
using Autodesk.AutoCAD.EditorInput;//(Editor, PromptXOptions, PromptXResult)
using Autodesk.AutoCAD.Windows;
using Autodesk.AutoCAD.Internal.Windows;
using Autodesk.AutoCAD.Internal.Forms;
using Autodesk.AutoCAD.Windows.ToolPalette;


using Autodesk.AutoCAD.Interop;
using System;
using System.Reflection;
using System.IO;

[assembly: CommandClass(typeof(AutoCADPlug.CAD))]
namespace AutoCADPlug
{
    public class CAD : IExtensionApplication
    {
        ContextMenuExtension m_ContextMenu;//定义右键菜单 
        PaletteSet ps;//定义工具栏按钮

        public void Initialize()
        {
            //AddContextMenu();//添加右键菜单
            AddMenuContent(); //添加菜单栏上项目
            //CreateCircle();
            //CreateRectangle();
        }

        public void Terminate()
        {
            RemoveContextMenu();
        }

        #region 添加菜单栏项目

        public void AddMenuContent()
        {
            try
            {

                AcadApplication acadApp = (AcadApplication)Application.AcadApplication;

                //添加根菜单
                AcadPopupMenu pMenu = acadApp.MenuGroups.Item(0).Menus.Add("psw二次开发");

                ////添加子菜单项,多级
                //AcadPopupMenu cMenu = pMenu.AddSubMenu(pMenu.Count + 1, "液压插件");
                //AcadPopupMenuItem cMenuItem0 = cMenu.AddMenuItem(cMenu.Count + 1, "插件1", "hydrPlugin1\n");
                //AcadPopupMenuItem cMenuItem1 = cMenu.AddMenuItem(cMenu.Count + 1, "插件2", "hydrPlugin2\n");
                //AcadPopupMenuItem cMenuItem2 = cMenu.AddMenuItem(cMenu.Count + 1, "插件3", "hydrPlugin3\n");
                //添加分隔条
                //pMenu.AddSeparator(pMenu.Count + 1);
                //添加子菜单项，单级
                AcadPopupMenuItem cMenu2 = pMenu.AddMenuItem(pMenu.Count + 1, "建筑插件", "architecturePlugin\n");
                pMenu.InsertInMenuBar(acadApp.MenuBar.Count + 1);

            }
            catch (System.Exception ex)
            {
                Application.DocumentManager.MdiActiveDocument.Editor.WriteMessage(ex.ToString());
            }
        }

        [CommandMethod("hydrPlugin1")]
        public void showSomething()
        {
            Editor ed = Application.DocumentManager.MdiActiveDocument.Editor;
            ed.WriteMessage("插件1被调用！");
        }

        #endregion

        #region 添加自定义面板

        //[CommandMethod("AddPalette")]
        //[CommandMethod("architecturePlugin")]
        public void AddPalette()
        {
            if (ps != null)
            {
                if (!ps.Visible)
                {
                    ps.Visible = true;
                }
                return;
            }

            MyControl contr = new MyControl();
            ps = new PaletteSet("插件操作面板");


            ps.Visible = true;
            ps.Style = PaletteSetStyles.ShowTabForSingle;
            ps.Style = PaletteSetStyles.NameEditable;
            ps.Style = PaletteSetStyles.ShowPropertiesMenu;
            ps.Style = PaletteSetStyles.ShowAutoHideButton;
            ps.Style = PaletteSetStyles.ShowCloseButton;
            ps.Opacity = 90;
            ps.Dock = DockSides.Left;
            ps.MinimumSize = new System.Drawing.Size(200, 100);
            ps.Add("PaletteSet", contr);
        }

        #endregion

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

        #region 用户交互：自定义用户界面

        [CommandMethod("architecturePlugin")]
        [CommandMethod("ShowModalDialog")]
        public void ShowModalDialog()
        {
            using (MyForm form = new MyForm())
            {
                form.ShowInTaskbar = false;
                Application.ShowModalDialog(form);
                if (form.DialogResult == System.Windows.Forms.DialogResult.OK)
                {
                    //Application.DocumentManager.MdiActiveDocument.Editor.WriteMessage("\n" + form.Name);
                    AddPalette();

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

        #region 添加和删除图层 “PSWCAD”

        /// <summary>
        /// 新建名为“PSWCAD”的图层
        /// </summary>
        [CommandMethod("AddLayer")]
        public void AddLayer()
        {
            Database db = HostApplicationServices.WorkingDatabase;
            AddLayerTableRecord("PSWCAD", 1, db);
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

        #region  扩展数据，用DataTable

        [CommandMethod("WriteData")]
        public void AddData()
        {
            DataTable dt = new DataTable();
            dt.TableName = "ParameterTable";
            dt.AppendColumn(CellType.CharPtr, "Name");
            dt.AppendColumn(CellType.CharPtr, "Meterial");
            dt.AppendColumn(CellType.CharPtr, "Parameter");
            DataCellCollection Row = new DataCellCollection();
            DataCell Name = new DataCell();
            DataCell Meterial = new DataCell();
            DataCell Parameter = new DataCell();
            Name.SetString("工字钢");
            Meterial.SetString("Q235B");
            Parameter.SetString("200*200*32*25");
            Row.Add(Name);
            Row.Add(Meterial);
            Row.Add(Parameter);
            dt.AppendRow(Row, true);
            Document doc = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument;
            Database db = doc.Database;
            Editor ed = doc.Editor;
            PromptEntityResult ent = ed.GetEntity("\n选择要写数据的对象");
            if (ent.Status == PromptStatus.OK)
            {
                using (Transaction tr = db.TransactionManager.StartTransaction())
                {
                    Entity entity = (Entity)tr.GetObject(ent.ObjectId, OpenMode.ForWrite, true);
                    if (entity.ExtensionDictionary == new ObjectId())
                        entity.CreateExtensionDictionary();
                    DBDictionary extensionDic = (DBDictionary)tr.GetObject(entity.ExtensionDictionary,
                    OpenMode.ForWrite, false);
                    extensionDic.SetAt("ParameterTable", dt);
                    tr.Commit();
                }
            }
        }

        [CommandMethod("ReadData")]
        public void ReadData()
        {
            Document doc = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument;
            Database db = doc.Database;
            Editor ed = doc.Editor;
            Entity entity = null;
            PromptEntityResult ent = ed.GetEntity("\n选择要读取数据的对象");
            if (ent.Status == PromptStatus.OK)
            {
                using (Transaction transaction = db.TransactionManager.StartTransaction())
                {
                    entity = (Entity)transaction.GetObject(ent.ObjectId, OpenMode.ForRead, true);
                    DBDictionary extensionDic = transaction.GetObject(entity.ExtensionDictionary, OpenMode.ForRead)
                    as DBDictionary;
                    DataTable dt = transaction.GetObject(extensionDic.GetAt("ParameterTable"), OpenMode.ForRead)
                    as DataTable;
                    string name = dt.GetCellAt(0, 0).Value.ToString();
                    string material = dt.GetCellAt(0, 1).Value.ToString();
                    string param = dt.GetCellAt(0, 2).Value.ToString();
                    ed.WriteMessage("\n Name:" + name);
                    ed.WriteMessage("\n Meterial:" + material);
                    ed.WriteMessage("\n Parameter:" + param);
                    transaction.Commit();
                }
            }
        }

        #endregion


        #region 测试命令
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
            ObjectId layer = db.Clayer;
        }

        [CommandMethod("psw")]
        public void test3()
        {
            Editor editor = Application.DocumentManager.MdiActiveDocument.Editor;
            //double data = InputOperation.Double("请输入数据！");
            //ShowMsgOperation.Alert(data.ToString());
        }

        [CommandMethod("hydrPlugin2")]
        public void TestCreateRec()
        {
            //创建矩形
            Point3d bPoint = Point3d.Origin;
            double height = 100;
            double width = 200;
            Point3d upperLeft = new Point3d(bPoint.X, bPoint.Y + height, 0);
            Point3d upperRight = new Point3d(bPoint.X + width, bPoint.Y + height, 0);
            Point3d lowerLeft = new Point3d(bPoint.X, bPoint.Y, 0);
            Point3d lowerRight = new Point3d(bPoint.X + width, bPoint.Y, 0);

            DBObjectCollection objs = new DBObjectCollection();
            Line line2 = new Line(lowerLeft, upperLeft);
            Line line3 = new Line(upperLeft, upperRight);
            Line line4 = new Line(upperRight, lowerRight);
            Line line1 = new Line(lowerRight, lowerLeft);
            objs.Add(line1);
            objs.Add(line2);
            objs.Add(line3);
            objs.Add(line4);

            //将矩形添加到数据库中
            Database db = DBOperation.GetDocumentDatabase();
            //DBOperation.AddToModelSpace(objs);
            DBOperation.AddToModelSpace(objs, db);
        }
        #endregion

        #region 测试改变图层

        [CommandMethod("ChangeLayer")]
        public void TestChangeLayer()
        {
            //改变圆的图层
            Entity ent = SelectObjOperation.SelectEntity("请选择一个实体：\n");
            change(ent);
        }

        /// <summary>
        /// 改变圆的图层为“PSWCAD”
        /// </summary>
        /// <param name="ent"></param>
        private void change(Entity ent)
        {
            Database db = Application.DocumentManager.MdiActiveDocument.Database;
            Editor ed = Application.DocumentManager.MdiActiveDocument.Editor;
            using (Transaction transaction = db.TransactionManager.StartTransaction())
            {
                Entity en = transaction.GetObject(ent.Id, OpenMode.ForWrite) as Entity;

                LayerTable lt = transaction.GetObject(db.LayerTableId, OpenMode.ForRead) as LayerTable;
                LayerTableRecord ltr = transaction.GetObject(db.Clayer, OpenMode.ForRead) as LayerTableRecord;
                if (!lt.Has("PSWCAD"))
                {
                    ShowMsgOperation.Alert("没有PSWCAD图层！");
                    return;
                }
                ent.Layer = "PSWCAD";
                transaction.Commit();
            }
        }

        #endregion

        #region 测试添加实体
        [CommandMethod("CCircle")]
        public void CreateCircle()
        {
            Editor ed = Application.DocumentManager.MdiActiveDocument.Editor;
            //新建一个圆
            int count = 5;
            while (count-- > 1)
            {
                Point3d pt1 = new Point3d(10 + 100 * count, 10 + 100 * count, 0);
                double radius = 10 * count;

                Circle cc = CreateEntityOperation.CreateCircle(pt1, radius);
                DBOperation.AddToModelSpace(cc);
            }
            ed.UpdateScreen();

        }

        private void CreateRectangle()
        {
            Editor ed = Application.DocumentManager.MdiActiveDocument.Editor;
            //新建矩形
            int count = 5;
            while (count-- > 1)
            {
                Point2d bPoint = new Point2d(-100 * count, 100 * count);
                double height = 20;
                double width = 10;
                Point2dCollection ptCol = new Point2dCollection();
                ptCol.Add(new Point2d(bPoint.X, bPoint.Y));
                ptCol.Add(new Point2d(bPoint.X, bPoint.Y + height));
                ptCol.Add(new Point2d(bPoint.X + width, bPoint.Y + height));
                ptCol.Add(new Point2d(bPoint.X + width, bPoint.Y));

                Polyline pl = new Polyline();

                for (int i = 0; i < ptCol.Count; i++)
                {
                    pl.AddVertexAt(i, ptCol[i], 0, 0, 0);
                }

                pl.Closed = true;
                DBOperation.AddToModelSpace(pl);
            }
            ed.UpdateScreen();
        }

        #endregion

        #region 测试选择实体

        [CommandMethod("TestGetSelection")]
        public void TestGetSelection()
        {
            Editor ed = Application.DocumentManager.MdiActiveDocument.Editor;
            PromptSelectionOptions pso = new PromptSelectionOptions();
            pso.MessageForAdding = "请选择实体集：：：";
            PromptSelectionResult psr = ed.GetSelection(pso);
            if (psr.Status == PromptStatus.OK)
            {
                SelectionSet ss = psr.Value;
                int count = ss.Count;
                ed.WriteMessage("选择了{0}个实体", count);
            }
        }

        [CommandMethod("TestSelectRectangle")]
        public void TestSelectRectangle()
        {
            Editor ed = Application.DocumentManager.MdiActiveDocument.Editor;
            SelectObjOperation.FilterType[] filter = new SelectObjOperation.FilterType[1];
            filter[0] = SelectObjOperation.FilterType.LWPOLYLINE;
            ed.WriteMessage(SelectObjOperation.SelectAll(filter).Count.ToString());
        }

        [CommandMethod("TestSelectFilterXdata")]
        public void TestSelectFilterXdata()
        {
            //获取当前文档编辑器
            Editor ed = Application.DocumentManager.MdiActiveDocument.Editor;
            TypedValue[] tValue = new TypedValue[2];
            //tValue.SetValue(new TypedValue((int)DxfCode.Start, "circle"), 0);
            tValue.SetValue(new TypedValue((int)DxfCode.Start, "LWPOLYLINE"), 0);
            //tValue.SetValue(new TypedValue((int)DxfCode.ExtendedDataRegAppName, "psw"), 1);
            tValue.SetValue(new TypedValue((int)DxfCode.ExtendedDataReal, 20.00), 1);
            //将过滤条件赋给 SelectionFilter 对象
            SelectionFilter sFilter = new SelectionFilter(tValue);
            //请求在图形区域选择对象
            PromptSelectionResult psr;
            psr = ed.SelectAll(sFilter);
            ObjectIdCollection objCollection = new ObjectIdCollection();
            //如果提示状态 OK，说明已选对象
            if (psr.Status == PromptStatus.OK)
            {
                SelectionSet ss = psr.Value;
                Application.ShowAlertDialog("Number of objects selected: " +
                ss.Count.ToString());

            }
            else
            {
                ed.WriteMessage("已选择0000个对象");
            }
            //return objCollection;
        }

        [CommandMethod("TestAddEntityXdata")]
        public void TestAddEntityXdata()
        {
            Editor ed = Application.DocumentManager.MdiActiveDocument.Editor;
            ed.WriteMessage("添加扩展数据XDATA\n");
            PromptSelectionOptions pso = new PromptSelectionOptions();
            pso.MessageForAdding = "请选择实体集\n";
            PromptSelectionResult psr = ed.GetSelection(pso);
            if (psr.Status != PromptStatus.OK)
            {
                ed.WriteMessage("选择对象失败，退出");
                return;
            }
            SelectionSet ss = psr.Value;
            int count = ss.Count;
            ed.WriteMessage("选择了{0}个实体", count);

            Database db = HostApplicationServices.WorkingDatabase;
            using (Transaction trans = db.TransactionManager.StartTransaction())
            {
                RegAppTable appTable = trans.GetObject(db.RegAppTableId, OpenMode.ForWrite) as RegAppTable;
                if (!appTable.Has("psw"))
                {
                    RegAppTableRecord appTableRec = new RegAppTableRecord();
                    appTableRec.Name = "psw";
                    appTable.Add(appTableRec);
                    trans.AddNewlyCreatedDBObject(appTableRec, true);
                }
                foreach (ObjectId id in ss.GetObjectIds())
                {
                    using (Entity ent = trans.GetObject(id, OpenMode.ForWrite) as Entity)
                    {
                        ent.ColorIndex = 1;
                        ResultBuffer rBuffer = new ResultBuffer(
                            new TypedValue((int)DxfCode.ExtendedDataRegAppName, "psw"),
                            new TypedValue((int)DxfCode.ExtendedDataReal, 20.00)
                            );
                        ent.XData = rBuffer;
                    }
                }
                trans.Commit();
            }

        }

        [CommandMethod("TestPolyLine")]
        public void TestPolyLine()
        {
            Editor ed = Application.DocumentManager.MdiActiveDocument.Editor;
            PromptSelectionOptions pso = new PromptSelectionOptions();
            pso.MessageForAdding = "请选择实体集：：：";
            PromptSelectionResult psr = ed.GetSelection(pso);
            if (psr.Status == PromptStatus.OK)
            {
                SelectionSet ss = psr.Value;
                int count = ss.Count;
                ed.WriteMessage("选择了{0}个实体\n", count);


                Database db = HostApplicationServices.WorkingDatabase;
                using (Transaction trans = db.TransactionManager.StartTransaction())
                {
                    foreach (ObjectId id in ss.GetObjectIds())
                    {
                        using (Polyline pline = trans.GetObject(id, OpenMode.ForWrite) as Polyline)
                        {
                            Point2d pt;
                            for (int i = 0; i < pline.NumberOfVertices; i++)
                            {
                                pt = pline.GetPoint2dAt(i);
                                ed.WriteMessage(pt.X.ToString() + "," + pt.Y.ToString() + "\n");
                            }
                        }
                    }
                    trans.Commit();
                }

            }
        }

        [CommandMethod("TestIntersectWith")]
        public void TestIntersectWith()
        {
            //获取当前文档编辑器
            Editor ed = Application.DocumentManager.MdiActiveDocument.Editor;
            TypedValue[] tValue = new TypedValue[2];
            tValue.SetValue(new TypedValue((int)DxfCode.Start, "LWPOLYLINE"), 0);
            tValue.SetValue(new TypedValue((int)DxfCode.ExtendedDataReal, 20.00), 1);
            //将过滤条件赋给 SelectionFilter 对象
            SelectionFilter sFilter = new SelectionFilter(tValue);
            //请求在图形区域选择对象
            PromptSelectionResult psr = ed.SelectAll(sFilter);
            ObjectIdCollection objCollection = new ObjectIdCollection();
            //如果提示状态 OK，说明已选对象
            if (psr.Status == PromptStatus.OK)
            {
                SelectionSet ss = psr.Value;
                Application.ShowAlertDialog("Number of objects selected: " +
                ss.Count.ToString());

                PromptEntityResult per = ed.GetEntity("请选择直线实体");
                ObjectId lineId;
                if (per.Status != PromptStatus.OK)
                {
                    return;
                }
                lineId = per.ObjectId;
                int count = 1;
                Database db = HostApplicationServices.WorkingDatabase;
                using (Transaction trans = db.TransactionManager.StartTransaction())
                {
                    using (Line lineEntity = trans.GetObject(lineId, OpenMode.ForRead) as Line)
                    {
                        foreach (ObjectId id in ss.GetObjectIds())
                        {
                            using (Polyline pline = trans.GetObject(id, OpenMode.ForRead) as Polyline)
                            {
                                Point3dCollection pt3d = new Point3dCollection();
                                lineEntity.IntersectWith(pline, Intersect.OnBothOperands, new Plane(), pt3d, IntPtr.Zero, IntPtr.Zero);
                                if (pt3d.Count > 0)
                                {
                                    ed.WriteMessage("与多段线" + count + ":" + id + "有交点\n");
                                }
                                else
                                    ed.WriteMessage("与多段线" + count + ":" + id + "没有交点\n");

                            }
                            count++;
                        }
                    }
                    trans.Commit();
                }

            }
            else
            {
                ed.WriteMessage("已选择0000个对象");
            }
        }
        #endregion

        #region 测试添加文字

        [CommandMethod("CreateText")]
        public void CreateText()
        {
            Editor ed = Application.DocumentManager.MdiActiveDocument.Editor;
            ed.WriteMessage("添加扩展数据XDATA\n");
            PromptSelectionOptions pso = new PromptSelectionOptions();
            pso.MessageForAdding = "请选择实体集\n";
            PromptSelectionResult psr = ed.GetSelection(pso);
            if (psr.Status != PromptStatus.OK)
            {
                ed.WriteMessage("选择对象失败，退出");
                return;
            }
            SelectionSet ss = psr.Value;
            int count = ss.Count;
            ed.WriteMessage("选择了{0}个实体", count);

            //获取当前文档及数据库
            Document doc = Application.DocumentManager.MdiActiveDocument;
            Database db = doc.Database;
            //启动事务
            using (Transaction trans = db.TransactionManager.StartTransaction())
            {
                //以读模式打开Block表
                BlockTable bt = trans.GetObject(db.BlockTableId, OpenMode.ForRead) as BlockTable;
                //以写模式打开Block表记录Model空间
                BlockTableRecord btr = trans.GetObject(bt[BlockTableRecord.ModelSpace], OpenMode.ForWrite) as BlockTableRecord;
                foreach (ObjectId id in ss.GetObjectIds())
                {
                    using (Polyline rec = trans.GetObject(id, OpenMode.ForWrite) as Polyline)
                    {
                        if (rec == null)
                        {
                            ed.WriteMessage("选择的不是矩形！");
                            return;
                        }
                        //创建一个单行文字对象
                        DBText acText = new DBText();
                        acText.Position = CalculatePosition(rec);
                        acText.Height = 50;
                        acText.TextString = "H500";
                        //添加到模型空间
                        btr.AppendEntity(acText);
                        trans.AddNewlyCreatedDBObject(acText, true);
                    }
                }
                //保存修改，关闭事务
                trans.Commit();
            }
        }

        private Point3d CalculatePosition(Polyline rec)
        {
            Point3d position;
            Point3d p1 = rec.GetPoint3dAt(0);
            Point3d p2 = rec.GetPoint3dAt(1);
            Point3d p3 = rec.GetPoint3dAt(2);
            double x = Math.Max(Math.Abs(p1.X - p2.X), Math.Abs(p2.X - p3.X));
            double y = Math.Max(Math.Abs(p1.Y - p2.Y), Math.Abs(p2.Y - p3.Y));

            if (x > y)
            {
                double px = (p1.X + p3.X) / 2 - 50;
                double py = Math.Max(p1.Y, p3.Y) + 20;
                position = new Point3d(px, py, 0);
            }
            else
            {
                double px = Math.Min(p1.X, p3.X) - 150;
                double py = (p1.Y + p3.Y) / 2;
                position = new Point3d(px, py, 0);
            }

            return position;
        }

        #endregion

        #region 测试添加块引用

        [CommandMethod("TestReadDwgFile")]
        public void TestReadDwgFile()
        {
            Document doc = Application.DocumentManager.MdiActiveDocument;
            using (Database db = new Database(false, true))
            {
                try
                {
                    string dwgFileFullPath = @"‪C:\Users\admin\Desktop\psw.dwg";
                    db.ReadDwgFile(dwgFileFullPath, FileShare.Read, false, "");
                    db.CloseInput(true);
                }
                catch (System.Exception ex)
                {
                    doc.Editor.WriteMessage("打开文件失败" + "\n" + ex.Message
                        + "\n" + ex.Source
                        + "\n" + ex.ToString()
                        + "\n" + ex.StackTrace);
                    return;
                }
            }
        }

        [CommandMethod("ReadDwgFileTest")]
        public static void ReadDwgFileTest()
        {
            Document doc = Application.DocumentManager.MdiActiveDocument;

            using (Database db = new Database(false, false))
            {
                try
                {
                    // 读取dwg文件
                    const string filename = "psw.dwg";
                    string dwgFileFullPath = HostApplicationServices.Current.FindFile(filename, db, FindFileHint.Default);
                    db.ReadDwgFile(dwgFileFullPath,
                        FileOpenMode.OpenForReadAndAllShare, false, null);
                    db.CloseInput(true);
                }
                catch
                {
                    doc.Editor.WriteMessage("\nUnable to read drawing file.");
                    return;
                }

                using (Transaction tr = db.TransactionManager.StartTransaction())
                {
                    // 模型空间
                    BlockTable blkTbl = tr.GetObject(db.BlockTableId, OpenMode.ForRead)
                        as BlockTable;
                    BlockTableRecord modelSpace = tr.GetObject(
                        blkTbl[BlockTableRecord.ModelSpace], OpenMode.ForRead)
                        as BlockTableRecord;

                    // 遍历模型空间
                    foreach (ObjectId oid in modelSpace)
                    {
                        DBObject dbobj = tr.GetObject(oid, OpenMode.ForRead);
                        if (dbobj is Entity)
                        {
                            Entity entity = dbobj as Entity;
                            doc.Editor.WriteMessage("\nEntity: {0}, {1}, {2}, {3}",
                                entity.Id,
                                entity.GetType().ToString(),
                                entity.Layer,
                                entity.Color.ToString());
                        }
                    }
                }
            }
        }

        private static string filepath = @"‪E:\Drawing1.dwg";

        [CommandMethod("modifyDWG")]
        public void addTextToDWG()
        {
            Document doc = Application.DocumentManager.MdiActiveDocument;
            Editor ed = doc.Editor;
            Database workingDB = HostApplicationServices.WorkingDatabase;
            Database db = new Database(false, true);

            try
            {
                db.ReadDwgFile(filepath, System.IO.FileShare.ReadWrite, false, "");
                db.CloseInput(true);
                HostApplicationServices.WorkingDatabase = db;
            }
            catch (Autodesk.AutoCAD.Runtime.Exception e)
            {
                ed.WriteMessage("\nUnable to open .dwg file : " + e.StackTrace);
                return;
            }

            using (Transaction tr = db.TransactionManager.StartTransaction())
            {
                BlockTable bt = tr.GetObject(db.BlockTableId, OpenMode.ForRead) as BlockTable;
                BlockTableRecord btr = tr.GetObject(bt[BlockTableRecord.ModelSpace], OpenMode.ForWrite) as BlockTableRecord;
                MText objText = new MText();
                objText.SetDatabaseDefaults();
                objText.Location = new Autodesk.AutoCAD.Geometry.Point3d(2, 2, 0);
                objText.Contents = "added text in a closed .dwg file!";
                objText.TextStyleId = db.Textstyle;
                btr.AppendEntity(objText);
                tr.AddNewlyCreatedDBObject(objText, true);
                tr.Commit();
            }
            HostApplicationServices.WorkingDatabase = workingDB;
            db.SaveAs(filepath, DwgVersion.Current);
        }


        #endregion

        #region 测试生成.dwg文件

        [CommandMethod("CreateDwgFile")]
        public void CreateDwgFile()
        {
            Database db = new Database(true, true);
            DBText txt = new DBText();
            txt.Position = new Point3d();
            txt.TextString = "PSWCAD.org22";
            ToModalSpace(txt, db);
            db.SaveAs(".\\psw_createdwgFile.dwg", DwgVersion.Current);
        }

        #endregion

        #region 测试从外部文件中读取块到当前数据库文件中

        [CommandMethod("ImportBlocks")]
        public void ImportBlocks()
        {
            DocumentCollection dm =
            Application.DocumentManager;
            Editor ed = dm.MdiActiveDocument.Editor;
            Database destDb = dm.MdiActiveDocument.Database;
            Database sourceDb = new Database(false, true);
            try
            {
                const string filename = "psw.dwg";
                string dwgFileFullPath = HostApplicationServices.Current.FindFile(filename, destDb, FindFileHint.Default);

                sourceDb.ReadDwgFile(dwgFileFullPath, System.IO.FileShare.Read, true, "");

                // Create a variable to store the list of block identifiers
                ObjectIdCollection blockIds = new ObjectIdCollection();

                var tm = sourceDb.TransactionManager;

                using (var myT = tm.StartOpenCloseTransaction())
                {
                    // Open the block table
                    BlockTable bt = (BlockTable)myT.GetObject(sourceDb.BlockTableId, OpenMode.ForRead, false);

                    // Check each block in the block table
                    foreach (ObjectId btrId in bt)
                    {
                        BlockTableRecord btr =
                        (BlockTableRecord)myT.GetObject(btrId, OpenMode.ForRead, false);

                        // Only add named & non-layout blocks to the copy list
                        if (!btr.IsAnonymous && !btr.IsLayout)
                            blockIds.Add(btrId);
                        btr.Dispose();
                    }
                }
                // Copy blocks from source to destination database
                var mapping = new IdMapping();
                sourceDb.WblockCloneObjects(blockIds,
                    destDb.BlockTableId,
                    mapping,
                    DuplicateRecordCloning.Replace,
                    false);
            }
            catch (Autodesk.AutoCAD.Runtime.Exception ex)
            {
                ed.WriteMessage("\nError during copy: " + ex.Message);
            }
            sourceDb.Dispose();

            //显示到界面上
            using (Transaction trans = destDb.TransactionManager.StartTransaction())
            {
                BlockTable bt = trans.GetObject(destDb.BlockTableId, OpenMode.ForRead) as BlockTable;
                BlockTableRecord btr = trans.GetObject(bt[BlockTableRecord.ModelSpace], OpenMode.ForWrite) as BlockTableRecord;

                ObjectId refid = bt["psw"];
                BlockReference br = new BlockReference(Point3d.Origin, refid); // 通过块定义创建块参照
                btr.AppendEntity(br); //把块参照添加到块表记录
                trans.AddNewlyCreatedDBObject(br, true); // 通过事务添加块参照到数据库
                trans.Commit();
            }
        }


        #endregion


    }
}
