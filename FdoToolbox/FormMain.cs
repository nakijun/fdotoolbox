#region LGPL Header
// Copyright (C) 2008, Jackie Ng
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
#endregion
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using FdoToolbox.Core;
using WeifenLuo.WinFormsUI.Docking;
using FdoToolbox.Core.Controls;
using FdoToolbox.Core.ClientServices;

namespace FdoToolbox
{
    public partial class FormMain : Form, IShell
    {
        private IConsoleWindow _console;

        private IObjectExplorer _objExplorer;

        public FormMain()
        {
            InitializeComponent();
            ObjectExplorer objExp = new ObjectExplorer();
            ConsoleWindow conWin = new ConsoleWindow();
            _objExplorer = objExp;
            _console = conWin;
            conWin.Show(mDockPanel, DockState.DockBottom);
            objExp.Show(mDockPanel, DockState.DockLeft);
        }

        public IObjectExplorer ObjectExplorer
        {
            get { return _objExplorer; }
        }

        public Form FormObj
        {
            get { return this; }
        }

        public void SetTitle(string title)
        {
            this.Text = title;
        }

        public void SetStatusBarText(string text)
        {
            
        }

        public MenuStrip MainMenu
        {
            get { return mMenuStrip; }
        }

        public ToolStripMenuItem GetRootMenuItem(string name)
        {
            return this.MainMenu.Items[name] as ToolStripMenuItem;
        }

        
        public void ShowDocumentWindow(ISpatialConnectionBoundCtl ctl)
        {
            DockContent content = FindDocumentWindow(ctl);
            if (content == null)
            {
                content = new DockContent();
                ctl.WrappedControl.Dock = DockStyle.Fill;
                
                content.Controls.Add(ctl.WrappedControl);
                ctl.OnSetTabText += delegate(string title) { content.TabText = title; };
                ctl.OnClose += delegate { content.Close(); };
                ctl.OnAccept += delegate { content.DialogResult = DialogResult.OK; };
                ctl.OnCancel += delegate { content.DialogResult = DialogResult.Cancel; };
                content.Text = content.TabText = ctl.Title;
            }
            content.Show(mDockPanel, DockState.Document);
        }

        private DockContent FindDocumentWindow(IBaseDocumentCtl baseDocumentCtl)
        {
            IDockContent [] documents = mDockPanel.DocumentsToArray();
            foreach (IDockContent dock in documents)
            {
                DockContent content = dock as DockContent;
                if (content != null)
                {
                    if (content.Controls.Contains(baseDocumentCtl.WrappedControl))
                        return content;
                }
            }
            return null;
        }

        public void ShowDocumentWindow(IBaseDocumentCtl ctl)
        {
            if (ctl is ISpatialConnectionBoundCtl)
            {
                ShowDocumentWindow(ctl as ISpatialConnectionBoundCtl);
                return;
            }
            ctl.WrappedControl.Dock = DockStyle.Fill;
            DockContent content = new DockContent();
            content.Controls.Add(ctl.WrappedControl);
            ctl.OnSetTabText += delegate(string title) { content.TabText = title; };
            ctl.OnClose += delegate { content.Close(); };
            ctl.OnAccept += delegate { content.DialogResult = DialogResult.OK; };
            ctl.OnCancel += delegate { content.DialogResult = DialogResult.Cancel; };
            content.Text = content.TabText = ctl.Title;
            content.Show(mDockPanel, DockState.Document);
        }
        
        public IConsoleWindow ConsoleWindow
        {
            get { return _console; }
        }

        private void FormMain_FormClosing(object sender, FormClosingEventArgs e)
        {
            AppGateway.RunningApplication.Quit();
        }
    }
}