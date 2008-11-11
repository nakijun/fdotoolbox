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
        private MemoryAppender _appender;
        private ToolStrip toolStrip1;
        private RichTextBox txtConsole;
        private TextBox inputBox;
        private Timer consoleTimer;

        public ConsolePane()
        {
            inputBox = new TextBox();
            txtConsole = new RichTextBox();
            txtConsole.Font = new Font(FontFamily.GenericMonospace, 8.0f);
            toolStrip1 = ToolbarService.CreateToolStrip(this, "/AppConsole/Toolbar");
            consoleTimer = new Timer();
            consoleTimer.Tick += new EventHandler(this.loggerTimer_Tick);
            consoleTimer.Interval = 500;
            consoleTimer.Enabled = true;

            this.AppenderName = "ConsoleLog";

            txtConsole.Dock = DockStyle.Fill;
            inputBox.Dock = DockStyle.Bottom;

            this.Controls.Add(txtConsole);
            this.Controls.Add(inputBox);
            this.Controls.Add(toolStrip1);
        }

        private string _AppenderName;

        public string AppenderName
        {
            get { return _AppenderName; }
            set { _AppenderName = value; }
        }
	

        protected override void OnLoad(EventArgs e)
        {
            XmlConfigurator.Configure();
            Hierarchy loggerHierarchy = LogManager.GetRepository() as Hierarchy;
            if(loggerHierarchy != null)
            {
                _appender = loggerHierarchy.Root.GetAppender(_AppenderName) as MemoryAppender;
            }
            base.OnLoad(e);
        }

        private void WriteLogEventToLogTextBox(LoggingEvent logEvent)
        {
            Color colour = GetLogEventLevelColour(logEvent);

            using (StringWriter writer = new StringWriter())
            {
                _appender.Layout.Format(writer, logEvent);

                this.txtConsole.SelectionColor = colour;
                this.txtConsole.AppendText(writer.ToString());

                this.txtConsole.ScrollToCaret();
            }
        }

        public void RefreshLogView()
        {
            if (_appender != null) // will only be null if not configured in app.config
            {
                // Get all events recorded since we last outputted them to textbox then clear buffer
                LoggingEvent[] events = _appender.GetEvents();
                _appender.Clear();

                foreach (LoggingEvent logEvent in events)
                {
                    WriteLogEventToLogTextBox(logEvent);
                }
            }
        }

        private static Color GetLogEventLevelColour(LoggingEvent logEvent)
        {
            Color colour;
            switch (logEvent.Level.Name)
            {
                case "ERROR":
                case "FATAL":
                    colour = Color.Red;
                    break;
                case "WARN":
                    colour = Color.Orange;
                    break;
                case "INFO":
                case "DEBUG":
                default:
                    colour = Color.Black;
                    break;
            }
            return colour;
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

        private void loggerTimer_Tick(object sender, EventArgs e)
        {
            RefreshLogView();
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
