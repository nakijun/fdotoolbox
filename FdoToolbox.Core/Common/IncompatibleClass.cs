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

        private ISet<IncompatibleClassReason> _ReasonCodes;

        public ISet<IncompatibleClassReason> ReasonCodes
        {
            get { return _ReasonCodes; }
        }

        public IncompatibleClass(string name, string reason)
        {
            this.Name = name;
            _Properties = new List<IncompatibleProperty>();
            _Reasons = new List<string>();
            _Reasons.Add(reason);
            _ReasonCodes = new HashedSet<IncompatibleClassReason>();
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
