using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using WeifenLuo.WinFormsUI.Docking;
using ICSharpCode.Core;
using FdoToolbox.Base.Controls;

namespace FdoToolbox.Base
{
    public sealed class Workbench : Form
    {
        static Workbench instance;

        public static Workbench Instance
        {
            get
            {
                return instance;
            }
        }

        public static event EventHandler WorkbenchInitialized = delegate { };

        public static void InitializeWorkbench()
        {
            instance = new Workbench();
            WorkbenchInitialized(instance, EventArgs.Empty);
        }
 
        MenuStrip menu;
        ToolStrip toolbar;
        DockPanel contentPanel;
        StatusStrip status;
        ToolStripStatusLabel statusLabel;

        IObjectExplorer objExplorer;
        IConsole appConsole;

        public IConsole Console { get { return appConsole; } }

        public IObjectExplorer ObjectExplorer { get { return objExplorer; } }

        private Workbench()
        {
            InitializeComponent();

            this.Icon = ResourceService.GetIcon("FdoToolbox");

            contentPanel = new DockPanel();
            contentPanel.Dock = DockStyle.Fill;
            contentPanel.DockLeftPortion = 200;
            contentPanel.DockBottomPortion = 150;
            
            this.Controls.Add(contentPanel);

            menu = new MenuStrip();
            MenuService.AddItemsToMenu(menu.Items, this, "/Workbench/MainMenu");

            toolbar = ToolbarService.CreateToolStrip(this, "/Workbench/Toolbar");

            status = new StatusStrip();
            statusLabel = new ToolStripStatusLabel();
            status.Items.Add(statusLabel);

            this.Controls.Add(toolbar);
            this.Controls.Add(menu);
            this.Controls.Add(status);

            this.IsMdiContainer = true;

            ObjectExplorer exp = new ObjectExplorer();
            objExplorer = exp;

            ConsolePane console = new ConsolePane();
            appConsole = console;

            ShowContent(console, ViewRegion.Bottom);
            ShowContent(exp, ViewRegion.Left);
            
            // Use the Idle event to update the status of menu and toolbar items.
            Application.Idle += OnApplicationIdle;
        }

        public void SetStatusLabel(string text)
        {
            statusLabel.Text = text;
        }

        public void SetTitle(string title)
        {
            this.Text = title;
        }

        public void ShowContent(IViewContent vc, ViewRegion region)
        {
            DockContent content = new DockContent();
            content.TabText = vc.Title;
            content.Text = vc.Title;
            content.ToolTipText = vc.Title;
            content.CloseButton = vc.CanClose;

            switch (region)
            {
                case ViewRegion.Bottom:
                    content.DockAreas = DockAreas.DockBottom;
                    break;
                case ViewRegion.Top:
                    content.DockAreas = DockAreas.DockTop;
                    break;
                case ViewRegion.Left:
                    content.DockAreas = DockAreas.DockLeft;
                    break;
                case ViewRegion.Right:
                    content.DockAreas = DockAreas.DockRight;
                    break;
                case ViewRegion.Document:
                    content.DockAreas = DockAreas.Document;
                    break;
                case ViewRegion.Floating:
                    content.DockAreas = DockAreas.Float;
                    break;
            }

            vc.TitleChanged += delegate(object sender, EventArgs e)
            {
                content.TabText = vc.Title;
                content.Text = vc.Title;
                content.ToolTipText = vc.Title;
            };

            vc.ViewContentClosing += delegate(object sender, EventArgs e)
            {
                if(vc.CanClose && vc.Close())
                    content.Close();
            };

            content.ClientSize = vc.ContentControl.Size;

            vc.ContentControl.Dock = DockStyle.Fill;
            content.Controls.Add(vc.ContentControl);

            if (region == ViewRegion.Dialog)
            {
                content.StartPosition = FormStartPosition.CenterParent;
                content.ShowDialog();
            }
            else
            {
                content.Show(contentPanel);
            }
        }

        void OnApplicationIdle(object sender, EventArgs e)
        {
            // Use the Idle event to update the status of menu and toolbar.
            // Depending on your application and the number of menu items with complex conditions,
            // you might want to update the status less frequently.
            UpdateMenuItemStatus();
        }

        /// <summary>Update Enabled/Visible state of items in the main menu based on conditions</summary>
        void UpdateMenuItemStatus()
        {
            foreach (ToolStripItem item in menu.Items)
            {
                if (item is IStatusUpdate)
                    (item as IStatusUpdate).UpdateStatus();
            }
        }

        private void InitializeComponent()
        {
            this.SuspendLayout();
            // 
            // Workbench
            // 
            this.ClientSize = new System.Drawing.Size(792, 566);
            this.Name = "Workbench";
            this.ResumeLayout(false);

        }
    }
}
