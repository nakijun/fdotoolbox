using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

namespace FdoToolbox.Core.Controls
{
    public partial class ModuleInfoCtl : BaseDocumentCtl
    {
        internal ModuleInfoCtl()
        {
            InitializeComponent();
        }

        public override string Title
        {
            get
            {
                return "Module Information - " + _module.Name;
            }
        }

        private IModule _module;

        public ModuleInfoCtl(IModule module) : this()
        {
            _module = module;
            lblName.Text = _module.Name;
            lblDescription.Text = _module.Description;
            grdCommands.DataSource = _module.Commands;
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
