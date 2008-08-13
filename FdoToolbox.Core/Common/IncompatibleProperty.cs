using System;
using System.Collections.Generic;
using System.Text;

namespace FdoToolbox.Core.Common
{
    public class IncompatibleProperty
    {
        private string _Name;

        public string Name
        {
            get { return _Name; }
            set { _Name = value; }
        }

        private List<string> _Reasons;

        public List<string> Reasons
        {
            get { return _Reasons; }
        }
	

        public IncompatibleProperty(string name, string reason)
        {
            this.Name = name;
            _Reasons = new List<string>();
            _Reasons.Add(reason);
        }

        public IncompatibleProperty() { }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder("Incompatible Property: " + this.Name + "\n");
            if (this.Reasons.Count > 0)
            {
                sb.Append("Reasons: \n");
                foreach (string str in this.Reasons)
                {
                    sb.Append(" - " + str + "\n");
                }
            }
            return sb.ToString();
        }
    }
}
