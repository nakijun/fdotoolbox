using System;
using System.Collections.Generic;
using System.Text;

namespace FdoToolbox.Core.Feature
{
    public class ComputedProperty
    {
        private string _Alias;

        public string Alias
        {
            get { return _Alias; }
            set { _Alias = value; }
        }

        private string _Expression;

        public string Expression
        {
            get { return _Expression; }
            set { _Expression = value; }
        }

        public ComputedProperty(string alias, string expression)
        {
            this.Alias = alias;
            this.Expression = expression;
        }
    }
}
