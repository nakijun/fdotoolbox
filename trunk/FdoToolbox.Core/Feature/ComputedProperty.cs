using System;
using System.Collections.Generic;
using System.Text;

namespace FdoToolbox.Core.Feature
{
    /// <summary>
    /// A computed property object
    /// </summary>
    public class ComputedProperty
    {
        private string _Alias;

        /// <summary>
        /// Gets or sets the alias.
        /// </summary>
        /// <value>The alias.</value>
        public string Alias
        {
            get { return _Alias; }
            set { _Alias = value; }
        }

        private string _Expression;

        /// <summary>
        /// Gets or sets the expression.
        /// </summary>
        /// <value>The expression.</value>
        public string Expression
        {
            get { return _Expression; }
            set { _Expression = value; }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ComputedProperty"/> class.
        /// </summary>
        /// <param name="alias">The alias.</param>
        /// <param name="expression">The expression.</param>
        public ComputedProperty(string alias, string expression)
        {
            this.Alias = alias;
            this.Expression = expression;
        }
    }
}
