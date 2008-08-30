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
using Iesi.Collections.Generic;
using System.Collections.ObjectModel;

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

        public ReadOnlyCollection<string> Reasons
        {
            get { return _Reasons.AsReadOnly(); }
        }

        private ISet<IncompatiblePropertyReason> _ReasonCodes;

        public ISet<IncompatiblePropertyReason> ReasonCodes
        {
            get { return _ReasonCodes; }
        }

        public IncompatibleProperty(string name, string reason)
        {
            this.Name = name;
            _Reasons = new List<string>();
            _ReasonCodes = new HashedSet<IncompatiblePropertyReason>();
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

        public void AddReason(string propReason)
        {
            _Reasons.Add(propReason);
        }
    }
}
