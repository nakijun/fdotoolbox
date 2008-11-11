using System;
using System.Collections.Generic;
using System.Text;

namespace FdoToolbox.Core.Connections
{
    public class DictionaryProperty
    {
        private string _Name;

        public string Name
        {
            get { return _Name; }
            set { _Name = value; }
        }

        private string _LocalizedName;

        public string LocalizedName
        {
            get { return _LocalizedName; }
            set { _LocalizedName = value; }
        }

        private string _DefaultValue;

        public string DefaultValue
        {
            get { return _DefaultValue; }
            set { _DefaultValue = value; }
        }

        private bool _Required;

        public bool Required
        {
            get { return _Required; }
            set { _Required = value; }
        }

        private bool _Protected;

        public bool Protected
        {
            get { return _Protected; }
            set { _Protected = value; }
        }

        private bool _Enumerable;

        public bool Enumerable
        {
            get { return _Enumerable; }
            set { _Enumerable = value; }
        }

        private bool _IsFile;

        public bool IsFile
        {
            get { return _IsFile; }
            set { _IsFile = value; }
        }

        private bool _IsPath;

        public bool IsPath
        {
            get { return _IsPath; }
            set { _IsPath = value; }
        }

        internal DictionaryProperty() { this.Enumerable = false; this.IsFile = false; this.IsPath = false; }
    }
}
