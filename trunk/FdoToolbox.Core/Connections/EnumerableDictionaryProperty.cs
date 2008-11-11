using System;
using System.Collections.Generic;
using System.Text;

namespace FdoToolbox.Core.Connections
{
    public class EnumerableDictionaryProperty : DictionaryProperty
    {
        private string[] _Values;

        public string[] Values
        {
            get { return _Values; }
            set { _Values = value; }
        }

        private bool _RequiresConnection;

        /// <summary>
        /// Returns true if the values can only be sought when the connection is open
        /// </summary>
        public bool RequiresConnection
        {
            get { return _RequiresConnection; }
            set { _RequiresConnection = value; }
        }

        internal EnumerableDictionaryProperty() : base() { this.Enumerable = true; this.RequiresConnection = false; }
    }
}
