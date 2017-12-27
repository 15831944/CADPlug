using System.Runtime.InteropServices;
using System.Windows.Forms;
using Autodesk.AutoCAD.Windows;
using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.Windows;
using System;

namespace AutoCADPlug.UserInterface
{
    class CADMenu
    {
        /// <summary>
        /// 创建菜单按钮
        /// </summary>
        /// <param name="btName">按钮名</param>
        /// <param name="cmdName">命令名</param>
        /// <returns></returns>
        public static RibbonButton RibbonButton(string btName, string cmdName)
        {
            RibbonButton ribButton = new RibbonButton();
            ribButton.Text = btName;
            ribButton.CommandParameter = cmdName;
            ribButton.ShowText = true;
            //ribButton.CommandHandler = new AdskCommandHandler();
            return ribButton;
        }

        /// <summary>
        /// 创建面板
        /// </summary>
        /// <param name="title">面板名</param>
        /// <returns></returns>
        public static RibbonPanel RibbonPanel(string title)
        {
            RibbonPanelSource ribSourcePanel = new RibbonPanelSource();
            ribSourcePanel.Title = title;
            RibbonPanel ribPanel = new RibbonPanel();
            ribPanel.Source = ribSourcePanel;
            return ribPanel;
        }

        /// <summary>
        /// 创建菜单标签
        /// </summary>
        /// <param name="name">标签名</param>
        /// <param name="id">标签ID</param>
        /// <returns></returns>
        public static RibbonTab RibbonTab(string name, string id)
        {
            RibbonTab ribTab = new Autodesk.Windows.RibbonTab();
            ribTab.Title = name;
            ribTab.Id = id;
            ribTab.IsActive = true;
            return ribTab;
        }

        /// <summary>
        /// 获得菜单控制
        /// </summary>
        /// <returns></returns>
        public static RibbonControl RibbonControl()
        {
            if (ComponentManager.Ribbon == null)
                Autodesk.Windows.ComponentManager.ItemInitialized += new EventHandler<RibbonItemEventArgs>(ComponentManager_ItemInitialized);
            return ComponentManager.Ribbon; ;
        }

        /// <summary>
        /// 用于激活菜单
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        static void ComponentManager_ItemInitialized(object sender, RibbonItemEventArgs e)
        {
            if (ComponentManager.Ribbon != null)
                ComponentManager.ItemInitialized -= new EventHandler<RibbonItemEventArgs>(ComponentManager_ItemInitialized);
        }
    }
}
