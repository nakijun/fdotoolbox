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
using System.Drawing;
using System.Windows.Forms;
using FdoToolbox.Core;
using WeifenLuo.WinFormsUI.Docking;

namespace FdoToolbox
{
	/// <summary>
	/// Description of ConsoleWindow.
	/// </summary>
	public partial class ConsoleWindow : DockContent, IConsoleWindow 
	{
		public ConsoleWindow()
		{
			//
			// The InitializeComponent() call is required for Windows Forms designer support.
			//
			InitializeComponent();
            this.Activated += new EventHandler(ConsoleWindow_Activated);
		}

        void ConsoleWindow_Activated(object sender, EventArgs e)
        {
            this.InputTextBox.Focus();
        }
		
		public TextBoxBase TextWindow
		{
			get { return this.richTextBox1; }
		}
		
		public TextBoxBase InputTextBox
		{
			get { return this.textBox1; }
		}
		
		public event ConsoleInputHandler ConsoleInput;
		
		[System.ComponentModel.EditorBrowsableAttribute()]
		protected override void OnClosing(System.ComponentModel.CancelEventArgs e)
		{
			e.Cancel = true;
			this.Hide();
		}
		
		
		void TextBox1KeyUp(object sender, KeyEventArgs e)
		{
			if(e.KeyCode == Keys.Enter && !string.IsNullOrEmpty(textBox1.Text))
			{
				string cmdText = textBox1.Text;
				this.textBox1.Clear();
				if(this.ConsoleInput != null)
					this.ConsoleInput(cmdText);
			}
		}
	}
}
