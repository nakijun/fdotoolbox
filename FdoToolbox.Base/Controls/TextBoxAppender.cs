using System;
using System.Collections.Generic;
using System.Text;
using log4net.Appender;
using System.Windows.Forms;
using System.IO;
using System.Globalization;
using System.Collections;
using log4net.Repository.Hierarchy;
using log4net;

namespace FdoToolbox.Base.Controls
{
    public class TextBoxAppender : AppenderSkeleton
    {
        private RichTextBox _TextBox;
        private StringBuilder _buffer;

        public TextBoxAppender() { _buffer = new StringBuilder(); }

        protected override void Append(log4net.Core.LoggingEvent loggingEvent)
        {
            using (StringWriter sw = new StringWriter(CultureInfo.InvariantCulture))
            {
                Layout.Format(sw, loggingEvent);
                //The textbox may not be ready at this point, so cache the appended
                //entries into a temp buffer until a textbox is ready. When it is
                //write the contents of the buffer first before writing the most
                //recent event
                if (_TextBox != null && !_TextBox.IsDisposed)
                {
                    string content = sw.ToString();
                    string level = loggingEvent.Level.Name;
                    if (_TextBox.InvokeRequired)
                    {
                        _TextBox.BeginInvoke(new WriteContentHandler(this.WriteContent), content, level);
                    }
                    else
                    {
                        WriteContent(content, level);
                    }
                }
                else if (_buffer != null)
                {
                    _buffer.Append(sw.ToString());
                }
            }
        }

        private delegate void WriteContentHandler(string content, string level);

        private void WriteContent(string content, string level)
        {
            if (_buffer != null && _buffer.Length > 0)
            {
                _TextBox.AppendText(_buffer.ToString());
                _buffer = null;
            }
            switch (level)
            {
                case "ERROR":
                case "FATAL":
                    _TextBox.SelectionColor = System.Drawing.Color.Red;
                    break;
                case "WARN":
                    _TextBox.SelectionColor = System.Drawing.Color.Orange;
                    break;
                case "INFO":
                case "DEBUG":
                default:
                    _TextBox.SelectionColor = System.Drawing.Color.Black;
                    break;
            }
            _TextBox.AppendText(content);
            _TextBox.ScrollToCaret();
        }

        public RichTextBox TextBox
        {
            get { return _TextBox; }
            set { _TextBox = value; }
        }

        public static void SetTextBox(RichTextBox tb)
        {
            foreach (IAppender appender in GetAppenders())
            {
                TextBoxAppender app = appender as TextBoxAppender;
                if (app != null)
                {
                    app.TextBox = tb;
                }
            }
        }

        private static IAppender[] GetAppenders()
        {
            ArrayList appenders = new ArrayList();
            appenders.AddRange(((Hierarchy)LogManager.GetLoggerRepository()).Root.Appenders);
            foreach (ILog log in LogManager.GetCurrentLoggers())
            {
                appenders.AddRange(((Logger)log.Logger).Appenders);
            }
            return (IAppender[])appenders.ToArray(typeof(IAppender));
        }
    }
}
