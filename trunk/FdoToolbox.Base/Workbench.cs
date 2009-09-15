#region LGPL Header
// Copyright (C) 2009, Jackie Ng
// http://code.google.com/p/fdotoolbox, jumpinjackie@gmail.com
// 
// This library is free software; you can redistribute it and/or
// modify it under the terms of the GNU Lesser General Public
// License as published by the Free Software Foundation; either
// version 2.1 of the License, or (at your option) any later version.
// 
// This library is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
// Lesser General Public License for more details.
// 
// You should have received a copy of the GNU Lesser General Public
// License along with this library; if not, write to the Free Software
// Foundation, Inc., 51 Franklin Street, Fifth Floor, Boston, MA  02110-1301  USA
// 
//
// See license.txt for more/additional licensing information
#endregion
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using WeifenLuo.WinFormsUI.Docking;
using ICSharpCode.Core;
using FdoToolbox.Base.Controls;
using FdoToolbox.Base.Services;
using FdoToolbox.Base.Services.DragDropHandlers;

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

        private static bool _init = false;

        public static void InitializeWorkbench()
        {
            if (!_init)
            {
                instance = new Workbench();
                _init = true;
                WorkbenchInitialized(instance, EventArgs.Empty);
            }
        }
 
        MenuStrip menu;
        ToolStripContainer toolStripContainer;
        ToolStrip toolbar;
        DockPanel contentPanel;
        StatusStrip status;
        ToolStripStatusLabel statusLabel;

        IObjectExplorer objExplorer;
        IConsole appConsole;

        ContextMenuStrip ctxToolbar;

        public IConsole Console { get { return appConsole; } }

        public IObjectExplorer ObjectExplorer { get { return objExplorer; } }

        private Workbench()
        {
            InitializeComponent();

            _toolstrips = new Dictionary<string, ToolStrip>();
            _toolstripRegions = new Dictionary<string, WorkbenchRegion>();

            this.Icon = ResourceService.GetIcon("FdoToolbox");

            contentPanel = new DockPanel();
            contentPanel.DocumentStyle = DocumentStyle.DockingWindow;
            contentPanel.Dock = DockStyle.Fill;
            contentPanel.DockLeftPortion = 200;
            contentPanel.DockBottomPortion = 150;
            contentPanel.DockRightPortion = 200;
            
            menu = new MenuStrip();
            MenuService.AddItemsToMenu(menu.Items, this, "/Workbench/MainMenu");

            toolStripContainer = new ToolStripContainer();
            toolStripContainer.ContentPanel.Controls.Add(contentPanel);
            toolStripContainer.Dock = DockStyle.Fill;

            this.Controls.Add(toolStripContainer);

            ctxToolbar = new ContextMenuStrip();
            toolStripContainer.TopToolStripPanel.ContextMenuStrip = ctxToolbar;
            toolStripContainer.BottomToolStripPanel.ContextMenuStrip = ctxToolbar;
            toolStripContainer.LeftToolStripPanel.ContextMenuStrip = ctxToolbar;
            toolStripContainer.RightToolStripPanel.ContextMenuStrip = ctxToolbar;

            toolbar = ToolbarService.CreateToolStrip(this, "/Workbench/Toolbar");
            AddToolbar("Base", toolbar, WorkbenchRegion.Top, false);

            status = new StatusStrip();
            statusLabel = new ToolStripStatusLabel();
            status.Items.Add(statusLabel);

            this.Controls.Add(menu);
            this.Controls.Add(status);

            //this.IsMdiContainer = true;

            ObjectExplorer exp = new ObjectExplorer();
            objExplorer = exp;

            ConsolePane console = new ConsolePane();
            appConsole = console;

            ShowContent(console, ViewRegion.Bottom);
            ShowContent(exp, ViewRegion.Left);
            
            // Use the Idle event to update the status of menu and toolbar items.
            Application.Idle += OnApplicationIdle;
        }

        

        private Dictionary<string, ToolStrip> _toolstrips;
        private Dictionary<string, WorkbenchRegion> _toolstripRegions;

        public void AddToolbar(string name, ToolStrip toolbar, WorkbenchRegion region, bool canToggleVisibility)
        {
            _toolstrips.Add(name, toolbar);
            _toolstripRegions.Add(name, region);

            if (canToggleVisibility)
            {
                ToolStripMenuItem item = new ToolStripMenuItem();
                item.Text = name;
                item.Tag = name;
                item.Checked = true;
                item.CheckOnClick = true;
                item.Click += delegate
                {
                    SetToolbarVisibility(name, item.Checked);
                };
                ctxToolbar.Items.Add(item);
            }

            switch (region)
            {
                case WorkbenchRegion.Top:
                    toolStripContainer.TopToolStripPanel.Controls.Add(toolbar);
                    break;
                case WorkbenchRegion.Bottom:
                    toolStripContainer.BottomToolStripPanel.Controls.Add(toolbar);
                    break;
                case WorkbenchRegion.Left:
                    toolStripContainer.LeftToolStripPanel.Controls.Add(toolbar);
                    break;
                case WorkbenchRegion.Right:
                    toolStripContainer.RightToolStripPanel.Controls.Add(toolbar);
                    break;
            }
        }

        public void SetToolbarVisibility(string toolbarName, bool visible)
        {
            ToolStrip strip = GetToolbar(toolbarName);
            if (strip != null)
            {
                WorkbenchRegion region = _toolstripRegions[toolbarName];
                if (visible)
                {
                    switch (region)
                    {
                        case WorkbenchRegion.Bottom:
                            toolStripContainer.BottomToolStripPanel.Controls.Add(strip);
                            break;
                        case WorkbenchRegion.Left:
                            toolStripContainer.LeftToolStripPanel.Controls.Add(strip);
                            break;
                        case WorkbenchRegion.Right:
                            toolStripContainer.RightToolStripPanel.Controls.Add(strip);
                            break;
                        case WorkbenchRegion.Top:
                            toolStripContainer.TopToolStripPanel.Controls.Add(strip);
                            break;
                    }
                }
                else
                {
                    switch (region)
                    {
                        case WorkbenchRegion.Bottom:
                            toolStripContainer.BottomToolStripPanel.Controls.Remove(strip);
                            break;
                        case WorkbenchRegion.Left:
                            toolStripContainer.LeftToolStripPanel.Controls.Remove(strip);
                            break;
                        case WorkbenchRegion.Right:
                            toolStripContainer.RightToolStripPanel.Controls.Remove(strip);
                            break;
                        case WorkbenchRegion.Top:
                            toolStripContainer.TopToolStripPanel.Controls.Remove(strip);
                            break;
                    }
                }
            }
        }

        public ToolStrip GetToolbar(string name)
        {
            if(_toolstrips.ContainsKey(name))
                return _toolstrips[name];
            return null;
        }

        public ICollection<string> ToolbarNames
        {
            get { return _toolstrips.Keys; }
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
            TabManager tm = ServiceManager.Instance.GetService<TabManager>();

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
                if(vc.CanClose)
                    content.Close();
            };

            content.ClientSize = vc.ContentControl.Size;
            content.CloseButton = vc.CanClose;

            vc.ContentControl.Dock = DockStyle.Fill;
            content.Controls.Add(vc.ContentControl);

            if (vc is IConnectionDependentView)
            {
                tm.Register((IConnectionDependentView)vc);
            }

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

        /// <summary>
        /// Invokes the method in the GUI thread context
        /// </summary>
        /// <param name="del">The delegate to invoke</param>
        /// <param name="args">The arguments</param>
        public void InvokeMethod(Delegate del, params object [] args)
        {
            this.Invoke(del, args);
        }

        /// <summary>
        /// Invokes the method in the GUI thread context
        /// </summary>
        /// <param name="del">The delegate to invoke</param>
        public void InvokeMethod(Delegate del)
        {
            this.Invoke(del);
        }

        protected override void WndProc(ref Message m)
        {
            if (m.Msg == NativeMethods.WM_SHOWME)
            {
                if (WindowState == FormWindowState.Minimized)
                {
                    WindowState = FormWindowState.Normal;
                }
                // get our current "TopMost" value (ours will always be false though)
                bool top = TopMost;
                // make our form jump to the top of everything
                TopMost = true;
                // set it back to whatever it was
                TopMost = top;
            }
            base.WndProc(ref m);
        }
    }

    public enum WorkbenchRegion
    {
        Top,
        Left,
        Right,
        Bottom
    }
}
