/*
 * Created by SharpDevelop.
 * User: Jackie Ng
 * Date: 24/04/2008
 * Time: 6:44 PM
 * 
 */

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
