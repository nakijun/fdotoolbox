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

namespace FdoToolbox
{
    public partial class FormMain : Form, IShell
    {
        private IConsoleWindow _console;

        private IObjectExplorer _objExplorer;

        public FormMain()
        {
            InitializeComponent();
            _BoundControls = new Dictionary<string, List<IConnectionBoundCtl>>();
            ObjectExplorer objExp = new ObjectExplorer();
            ConsoleWindow conWin = new ConsoleWindow();
            _objExplorer = objExp;
            _console = conWin;
            conWin.Show(mDockPanel, DockState.DockBottom);
            objExp.Show(mDockPanel, DockState.DockLeft);
            HostApplication.Instance.ConnectionManager.ConnectionRenamed += new ConnectionRenamedEventHandler(ConnectionManager_ConnectionRenamed);
            HostApplication.Instance.ConnectionManager.BeforeConnectionRemove += new ConnectionBeforeRemoveHandler(ConnectionManager_BeforeConnectionRemove);
        }

        void ConnectionManager_ConnectionRenamed(string oldName, string newName)
        {
            if (_BoundControls.ContainsKey(oldName))
            {
                //Rename all connections in old list
                List<IConnectionBoundCtl> controls = _BoundControls[oldName];
                foreach (IConnectionBoundCtl ctl in controls)
                {
                    ctl.SetName(newName);
                }

                _BoundControls.Remove(oldName);
                _BoundControls.Add(newName, controls);
            }
        }

        void ConnectionManager_BeforeConnectionRemove(string name, ref bool cancel)
        {
            if (_BoundControls.ContainsKey(name) && _BoundControls[name].Count > 0)
            {
                cancel = !AppConsole.Confirm("Tabs still open", "There are tabs still open which rely on the connection you are about to close.\nIf you close the connection they will be closed too.\n\nClose connection?");
            }
        }
        
        public IObjectExplorer ObjectExplorer
        {
            get { return _objExplorer; }
        }

        public Form FormObj
        {
            get { return this; }
        }

        public string Title
        {
            set { this.Text = value; }
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

        private Dictionary<string, List<IConnectionBoundCtl>> _BoundControls;

        public void ShowDocumentWindow(BaseDocumentCtl ctl)
        {
            ctl.Dock = DockStyle.Fill;
            DockContent content = new DockContent();
            content.Controls.Add(ctl);
            ctl.OnSetTabText += delegate(string title) { content.TabText = title; };
            ctl.OnClose += delegate { content.Close(); };
            ctl.OnAccept += delegate { content.DialogResult = DialogResult.OK; };
            ctl.OnCancel += delegate { content.DialogResult = DialogResult.Cancel; };
            content.Text = content.TabText = ctl.Title;
            IConnectionBoundCtl cbCtl = ctl as IConnectionBoundCtl;
            if (cbCtl != null)
            {
                IConnectionMgr connMgr = HostApplication.Instance.ConnectionManager;
                string name = cbCtl.BoundConnection.Name;
                cbCtl.SetName(name);
                if (!_BoundControls.ContainsKey(name))
                    _BoundControls[name] = new List<IConnectionBoundCtl>();

                _BoundControls[name].Add(cbCtl);
                ConnectionEventHandler removeHandler = new ConnectionEventHandler(delegate(string connName) 
                {
                    if (cbCtl.BoundConnection.Name == connName)
                        ctl.Close();
                });
                
                connMgr.ConnectionRemoved += removeHandler;
                ctl.Disposed += delegate
                {
                    connMgr.ConnectionRemoved -= removeHandler;
                    if(_BoundControls.ContainsKey(name))
                        _BoundControls[name].Remove(cbCtl);
                };
            }
            content.Show(mDockPanel, DockState.Document);
        }
        /*
        public List<IConnectionBoundCtl> GetConnectionBoundControlsByName(string name)
        {
            if (_BoundControls.ContainsKey(name))
                return _BoundControls[name];
            return null;
        }
        */
        public IConsoleWindow ConsoleWindow
        {
            get { return _console; }
        }
    }
}