using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;

namespace FdoToolbox.Express.Controls.Odbc
{
    public class OdbcAccess : IOdbcConnectionBuilder
    {
        private string _File;

        [Editor(typeof(System.Windows.Forms.Design.FileNameEditor), typeof(System.Drawing.Design.UITypeEditor))]
        [Description("The path to the Microsoft Access Database")]
        public string File
        {
            get { return _File; }
            set { _File = value; }
        }

        private string _UserId;

        [Description("The user id to connect as")]
        public string UserId
        {
            get { return _UserId; }
            set { _UserId = value; }
        }

        private string _Password;

        [Description("The password for the user id")]
        public string Password
        {
            get { return _Password; }
            set { _Password = value; }
        }

        public string ToConnectionString()
        {
            string connStr = string.Format("Driver={{Microsoft Access Driver (*.mdb)}};Dbq={0}", this.File);
            if (!string.IsNullOrEmpty(this.UserId))
                connStr += "Uid=" + this.UserId;
            if (!string.IsNullOrEmpty(this.Password))
                connStr += "Pwd=" + this.Password;

            return connStr;
        }
    }
}
