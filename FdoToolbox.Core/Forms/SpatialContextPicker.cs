using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using OSGeo.FDO.Connections;
using OSGeo.FDO.Commands.SpatialContext;

namespace FdoToolbox.Core.Forms
{
    /// <summary>
    /// A dialog that allows a user to select a spatial context (by name)
    /// from a list of known spatial contexts in the current connection
    /// </summary>
    public partial class SpatialContextPicker : Form
    {
        internal SpatialContextPicker()
        {
            InitializeComponent();
        }
        
        public SpatialContextPicker(IConnection conn) : this()
        {
            using (IGetSpatialContexts cmd = conn.CreateCommand(OSGeo.FDO.Commands.CommandType.CommandType_GetSpatialContexts) as IGetSpatialContexts)
            {
                using (ISpatialContextReader reader = cmd.Execute())
                {
                    lstNames.Items.Clear();
                    while (reader.ReadNext())
                    {
                        lstNames.Items.Add(reader.GetName());
                    }
                }
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.OK;
        }

        private void lstNames_SelectedIndexChanged(object sender, EventArgs e)
        {
            btnOK.Enabled = (lstNames.SelectedIndex >= 0);
        }

        public static string GetName(IConnection conn)
        {
            SpatialContextPicker picker = new SpatialContextPicker(conn);
            if (picker.ShowDialog() == DialogResult.OK)
            {
                return picker.lstNames.SelectedItem.ToString();
            }
            return null;
        }
    }
}