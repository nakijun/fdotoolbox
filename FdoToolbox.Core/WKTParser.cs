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
