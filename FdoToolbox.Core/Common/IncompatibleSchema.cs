using System;
using System.Collections.Generic;
using System.Text;

namespace FdoToolbox.Core.Common
{
    public class IncompatibleSchema
    {
        private string _Name;

        public string Name
        {
            get { return _Name; }
            set { _Name = value; }
        }

        private List<IncompatibleClass> _Classes;

        public List<IncompatibleClass> Classes
        {
            get { return _Classes; }
        }

        public IncompatibleSchema(string name)
        {
            this.Name = name;
            _Classes = new List<IncompatibleClass>();
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder("Incompatible schema: " + this.Name + "\n");
            foreach (IncompatibleClass cls in this.Classes)
            {
                sb.Append(cls.ToString() + "\n");
            }
            return sb.ToString();
        }
    }
}
