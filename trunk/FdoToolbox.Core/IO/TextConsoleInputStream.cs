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
using System.Text;
using System.IO;
using System.Windows.Forms;

namespace FdoToolbox.Core.IO
{
    /// <summary>
    /// Input stream for GUI applications
    /// </summary>
    public class TextConsoleInputStream : Stream, IConsoleInputStream
    {
        private TextBoxBase _txtBox;

        public TextConsoleInputStream(TextBoxBase txtbox)
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
