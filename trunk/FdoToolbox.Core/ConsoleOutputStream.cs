using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Drawing;
using System.IO;

namespace FdoToolbox.Core
{
    public delegate object PropertyGetter();

    /// <summary>
    /// Textbox-based output stream used by the Application console
    /// </summary>
    public class ConsoleOutputStream : Stream
    {
        private TextBoxBase _txtBox;

        public ConsoleOutputStream(TextBoxBase txtbox)
        {
            _txtBox = txtbox;
            this.TextColor = Color.Black;
        }

        private Color _TextColor;

        /// <summary>
        /// The color of the text to be output by this stream. This property only
        /// applies if the text box supplied is an instance of RichTextBox
        /// </summary>
        public Color TextColor
        {
            get { return _TextColor; }
            set { _TextColor = value; }
        }

        public override bool CanRead
        {
            get { return false; }
        }

        public override bool CanSeek
        {
            get { return false; }
        }

        public override bool CanWrite
        {
            get { return true; }
        }

        public override void Flush()
        {

        }

        public override long Length
        {
            get { return 0; }
        }

        public override long Position
        {
            get
            {
                return -1;
            }
            set
            {

            }
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            return -1;
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            return -1;
        }

        public override void SetLength(long value)
        {

        }

        private void AppendColoredText(RichTextBox box, Color color, string text)
        {
            if (box.InvokeRequired)
            {
                box.Invoke(new MethodInvoker(
                        delegate
                        {
                            int start = box.TextLength;
                            box.AppendText(text);
                            int end = box.TextLength;

                            // Textbox may transform chars, so (end-start) != text.Length
                            box.Select(start, end - start);
                            {
                                box.SelectionColor = color;
                                // could set box.SelectionBackColor, box.SelectionFont too.
                            }
                            box.SelectionLength = 0; // clear
                        }
                    )
                );
            }
            else
            {
                int start = box.TextLength;
                box.AppendText(text);
                int end = box.TextLength;

                // Textbox may transform chars, so (end-start) != text.Length
                box.Select(start, end - start);
                {
                    box.SelectionColor = color;
                    // could set box.SelectionBackColor, box.SelectionFont too.
                }
                box.SelectionLength = 0; // clear
            }
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            string s = Encoding.Default.GetString(buffer, offset, count);
            s = s.Trim();
            if (!string.IsNullOrEmpty(s))
            {
                if (_txtBox is RichTextBox)
                {
                    AppendColoredText((_txtBox as RichTextBox), this.TextColor, s);
                    _txtBox.Invoke(new MethodInvoker(delegate { _txtBox.ScrollToCaret(); }));
                }
                else
                {
                    _txtBox.AppendText(s);
                    _txtBox.ScrollToCaret();
                }
            }
        }

        public void Write(string s)
        {
            if (!string.IsNullOrEmpty(s))
            {
                if (_txtBox is RichTextBox)
                {
                    AppendColoredText((_txtBox as RichTextBox), this.TextColor, s);
                    _txtBox.Invoke(new MethodInvoker(delegate { _txtBox.ScrollToCaret(); }));
                }
                else
                {
                    _txtBox.AppendText(s);
                    _txtBox.ScrollToCaret();
                }
            }
        }

        public void WriteLine(string str)
        {
            this.Write(str + "\n");
        }

        public void Write(string format, params object[] args)
        {
            this.Write(string.Format(format, args));
        }

        public void WriteLine(string format, params object[] args)
        {
            this.WriteLine(string.Format(format, args));
        }
    }
}
