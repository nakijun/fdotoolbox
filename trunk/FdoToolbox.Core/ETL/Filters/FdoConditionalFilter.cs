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

namespace FdoToolbox.Core.ETL.Filters
{
    /// <summary>
    /// FDO conditional filter operation. 
    /// </summary>
    public class FdoConditionalFilter : FdoStreamingOperation
    {
        private Dictionary<string, List<ConditionAction>> _conditions = new Dictionary<string, List<ConditionAction>>();

        class ConditionAction
        {
            public Predicate<ValueExpression> comparision;
            public Action<ValueExpression> action;
        }

        /// <summary>
        /// Adds a conditional action. Please note that conditional actions are processed in
        /// the order they are added in. For example if we add the following conditional action
        /// 
        /// name: NAME
        /// test: NAME is equal to "Foobar"
        /// action: set NAME to "Snafu"
        /// 
        /// followed with this conditional action
        /// 
        /// name: NAME
        /// test: NAME is equal to "Foobar"
        /// action: set NAME to "Whatever"
        /// 
        /// On a NAME property of value "Foobar", the first action will be performed and the
        /// second action will be skipped, since NAME is now "Snafu" thus the second test will not
        /// evaluate to true.
        /// 
        /// </summary>
        /// <param name="name">The property to apply a conditional action</param>
        /// <param name="comparison">A delegate that tests a condition</param>
        /// <param name="action">An action to perform if the condition passes</param>
        public void AddCondition(string name, Predicate<ValueExpression> comparison, Action<ValueExpression> action)
        {
            if (!_conditions.ContainsKey(name))
                _conditions[name] = new List<ConditionAction>();

            ConditionAction ca = new ConditionAction();
            ca.comparision = comparison;
            ca.action = action;

            _conditions[name].Add(ca);
        }

        protected override void ProcessFeature(FdoFeature feat)
        {
            foreach (string name in _conditions.Keys)
            {
                ValueExpression expr = feat[name];
                if (expr != null)
                {
                    //Test all conditions for this feature
                    foreach (ConditionAction ca in _conditions[name])
                    {
                        //Invoke action if comparsion evaluates to true
                        if (ca.comparision(expr))
                            ca.action(expr);
                    }
                }
            }
        }
    }
}
