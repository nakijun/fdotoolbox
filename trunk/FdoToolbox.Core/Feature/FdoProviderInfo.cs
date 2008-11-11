using System;
using System.Collections.Generic;
using System.Text;
using OSGeo.FDO.ClientServices;

namespace FdoToolbox.Core.Feature
{
    public class FdoProviderInfo
    {
        private string _DisplayName;

        public string DisplayName
        {
            get { return _DisplayName; }
            internal set { _DisplayName = value; }
        }
        private string _Description;

        public string Description
        {
            get { return _Description; }
            internal set { _Description = value; }
        }
        private string _FeatureDataObjectsVersion;

        public string FeatureDataObjectsVersion
        {
            get { return _FeatureDataObjectsVersion; }
            internal set { _FeatureDataObjectsVersion = value; }
        }
        private bool _IsManaged;

        public bool IsManaged
        {
            get { return _IsManaged; }
            internal set { _IsManaged = value; }
        }
        private string _LibraryPath;

        public string LibraryPath
        {
            get { return _LibraryPath; }
            internal set { _LibraryPath = value; }
        }
        private string _Name;

        public string Name
        {
            get { return _Name; }
            internal set { _Name = value; }
        }
        private string _Version;

        public string Version
        {
            get { return _Version; }
            internal set { _Version = value; }
        }

        private bool _IsFlatFile;

        public bool IsFlatFile
        {
            get { return _IsFlatFile; }
            internal set { _IsFlatFile = value; }
        }
	

        internal FdoProviderInfo()
        {
        }
    }
}
