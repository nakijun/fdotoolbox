using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;

namespace FdoToolbox.Express.Controls.Odbc
{
    public class OdbcText : IOdbcConnectionBuilder
    {
        private string _Directory;

        [Description("The directory containing the text files")]
        [Editor(typeof(System.Windows.Forms.Design.FolderNameEditor), typeof(System.Drawing.Design.UITypeEditor))]
        public string Directory
        {
            get { return _Directory; }
            set { _Directory = value; }
        }

        public string ToConnectionString()
        {
            return string.Format("Driver={{Microsoft Text Driver (*.txt; *.csv)}};Dbq={0};Extensions=asc,csv,tab,txt", this.Directory);
        }
    }
}
