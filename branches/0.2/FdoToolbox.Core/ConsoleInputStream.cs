using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Windows.Forms;

namespace FdoToolbox.Core
{
    public class ConsoleInputStream : Stream
    {
        private TextBoxBase _txtBox;

        public ConsoleInputStream(TextBoxBase txtbox)
		{
			_txtBox = txtbox;
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
        	char [] input = _txtBox.Text.ToCharArray();
        	Array.Copy(input, offset, buffer, 0, count);
            return count;
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            return -1;
        }

        public override void SetLength(long value)
        {
            
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            
        }
    }
}
