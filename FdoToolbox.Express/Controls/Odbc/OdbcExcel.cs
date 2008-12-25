using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;

namespace FdoToolbox.Express.Controls.Odbc
{
    public class OdbcExcel : IOdbcConnectionBuilder
    {
        private string _File;

        [Editor(typeof(System.Windows.Forms.Design.FileNameEditor), typeof(System.Drawing.Design.UITypeEditor))]
        [Description("The path to the excel spreadsheet")]
        public string File
        {
            get { return _File; }
            set { _File = value; }
        }
	
        public string ToConnectionString()
        {
            return string.Format("Driver={{Microsoft Excel Driver (*.xls)}};DriverId=790;Dbq={0}", this.File);
        }
    }
}
