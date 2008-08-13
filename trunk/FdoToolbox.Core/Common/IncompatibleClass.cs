using System;
using System.Collections.Generic;
using System.Text;

namespace FdoToolbox.Core.Common
{
    public class IncompatibleClass
    {
        private string _Name;

        public string Name
        {
            get { return _Name; }
            set { _Name = value; }
        }

        private List<IncompatibleProperty> _Properties;

        public List<IncompatibleProperty> Properties
        {
            get { return _Properties; }
        }

        private List<string> _Reasons;

        public List<string> Reasons
        {
            get { return _Reasons; }
        }
	

        public IncompatibleClass(string name, string reason)
        {
            this.Name = name;
            _Properties = new List<IncompatibleProperty>();
            _Reasons = new List<string>();
            _Reasons.Add(reason);
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder("Incompatible Class: " + this.Name + "\n");
            if (this.Reasons.Count > 0)
            {
                sb.Append("Reasons:\n");
                foreach (string str in this.Reasons)
                {
                    sb.Append(" - " + str + "\n");
                }
            }
            if (this.Properties.Count > 0)
            {
                sb.Append("Incompatible Properties:\n");
                foreach (IncompatibleProperty prop in this.Properties)
                {
                    sb.Append(prop.ToString() + "\n");
                }
            }
            return sb.ToString();
        }
    }
}
