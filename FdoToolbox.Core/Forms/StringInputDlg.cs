using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace FdoToolbox.Core.Forms
{
    public partial class StringInputDlg : Form
    {
        internal StringInputDlg()
        {
            InitializeComponent();
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.OK;
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
        }

        public static string GetInput(string title, string prompt)
        {
            StringInputDlg dlg = new StringInputDlg();
            dlg.Text = title;
            dlg.lblPrompt.Text = prompt;
            if (dlg.ShowDialog() == DialogResult.OK)
            {
                return dlg.txtInput.Text;
            }
            return null;
        }

        public static string GetInput(string title, string prompt, string existingInput)
        {
            StringInputDlg dlg = new StringInputDlg();
            dlg.Text = title;
            dlg.lblPrompt.Text = prompt;
            dlg.txtInput.Text = existingInput;
            if (dlg.ShowDialog() == DialogResult.OK)
            {
                return dlg.txtInput.Text;
            }
            return null;
        }
    }
}