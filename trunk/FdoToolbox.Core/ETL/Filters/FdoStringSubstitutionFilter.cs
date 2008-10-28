#region LGPL Header
// Copyright (C) 2008, Jackie Ng
// http://code.google.com/p/fdotoolbox, jumpinjackie@gmail.com
// 
// This library is free software; you can redistribute it and/or
// modify it under the terms of the GNU Lesser General Public
// License as published by the Free Software Foundation; either
// version 2.1 of the License, or (at your option) any later version.
// 
// This library is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
// Lesser General Public License for more details.
// 
// You should have received a copy of the GNU Lesser General Public
// License along with this library; if not, write to the Free Software
// Foundation, Inc., 51 Franklin Street, Fifth Floor, Boston, MA  02110-1301  USA
// 
#endregion

using System;
using System.Collections.Generic;
using System.Text;
using OSGeo.FDO.Expression;
using System.Collections.Specialized;
using System.Collections;

namespace FdoToolbox.Core.ETL.Filters
{
    /// <summary>
    /// FDO feature substitution filter. Features read in from an input pipe may have one 
    /// or more of its attributes replaced if its value can be substituted. 
    /// </summary>
    public class FdoStringSubstitutionFilter : FdoConditionalFilter
    {
        /// <summary>
        /// Adds a substitution rule for a given property
        /// </summary>
        /// <param name="name">The name of the property</param>
        /// <param name="value">The value to look for</param>
        /// <param name="replacement">The value to replace it with</param>
        public void AddSubstitution(string name, string value, string replacement)
        {
            //Test predicate. Return true if expression is string and == value
            Predicate<ValueExpression> test = delegate(ValueExpression expr)
            {
                StringValue sval = expr as StringValue;
                if (sval != null)
                    return sval.String == value;
                return false;
            };

            //Action delegate. Replace string expression with replacement value
            Action<ValueExpression> sub = delegate(ValueExpression expr)
            {
                StringValue sval = expr as StringValue;
                if (sval != null)
                    sval.String = replacement;
            };

            base.AddCondition(name, test, sub);
        }
    }
}
