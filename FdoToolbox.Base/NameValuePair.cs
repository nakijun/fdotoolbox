using System;
using System.Collections.Generic;
using System.Text;

namespace FdoToolbox.Base
{
    public class NameValuePair
    {
        private string _Name;

        public string Name
        {
            get { return _Name; }
            set { _Name = value; }
        }

        private string _Value;

        public string Value
        {
            get { return _Value; }
            set { _Value = value; }
        }

        public NameValuePair(string name, string value)
        {
            this.Name = name;
            this.Value = value;
        }
    }
}
