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

namespace FdoToolbox.Core
{
    public class WKTParser
    {
        private string _CsName;

        public string CSName
        {
            get { return _CsName; }
            set { _CsName = value; }
        }
	

        public WKTParser(string wktText)
        {
            string csName = null;
            string right = string.Empty;
            if (wktText.Contains("PROJCS"))
                right = wktText.Substring("PROJCS".Length);
            else if (wktText.Contains("GEOGCS"))
                right = wktText.Substring("GEOGCS".Length);
            else if (wktText.Contains("LOCAL_CS"))
                right = wktText.Substring("LOCAL_CS".Length);

            if (right.Length > 0)
            {
                string right2 = right.Substring("[".Length);
                string right3 = right2.Substring("\"".Length);
                csName = right3.Substring(0, right3.IndexOf("\""));
            }

            this.CSName = csName;
        }
    }
}
