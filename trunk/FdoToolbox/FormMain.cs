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
            content.Show(mDockPanel, DockState.Document);
        }

        public IConsoleWindow ConsoleWindow
        {
            get { return _console; }
        }
    }
}