using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using ICSharpCode.Core;
using log4net.Appender;
using log4net.Repository.Hierarchy;
using log4net;
using System.IO;
using log4net.Core;
using log4net.Config;

namespace FdoToolbox.Base.Controls
{
    public partial class ConsolePane : UserControl, IViewContent, IConsole
    {
        private ToolStrip toolStrip1;
        private RichTextBox txtConsole;
        private TextBox inputBox;

        public ConsolePane()
        {
            inputBox = new TextBox();
            txtConsole = new RichTextBox();
            txtConsole.Font = new Font(FontFamily.GenericMonospace, 8.0f);
            toolStrip1 = ToolbarService.CreateToolStrip(this, "/AppConsole/Toolbar");
            
            txtConsole.Dock = DockStyle.Fill;
            inputBox.Dock = DockStyle.Bottom;

            this.Controls.Add(txtConsole);
            this.Controls.Add(inputBox);
            this.Controls.Add(toolStrip1);
        }

        protected override void OnLoad(EventArgs e)
        {
            TextBoxAppender.SetTextBox(txtConsole);
            //HACK to force a write of buffered logs
            LoggingService.Info("Console Initialized"); 
            base.OnLoad(e);
        }

        public Control ContentControl
        {
            get { return this; }
        }

        public string Title
        {
            get { return ResourceService.GetString("UI_CONSOLE"); }
        }

        public event EventHandler TitleChanged;

        public bool CanClose
        {
            get { return false; }
        }

        public bool Close()
        {
            return false;
        }

        public bool Save()
        {
            return false;
        }

        public bool SaveAs()
        {
            return false;
        }

        public event EventHandler ViewContentClosing = delegate { };

        public string TextContent
        {
            get { return txtConsole.Text; }
        }

        public void Clear()
        {
            txtConsole.Clear();
        }
    }
}
