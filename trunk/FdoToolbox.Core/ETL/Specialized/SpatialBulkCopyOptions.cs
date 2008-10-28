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
using OSGeo.FDO.Connections;
using OSGeo.FDO.ClientServices;
using OSGeo.FDO.Commands.Schema;
using OSGeo.FDO.Schema;
using OSGeo.FDO.Commands;
using System.IO;
using FdoToolbox.Core.ClientServices;
using FdoToolbox.Core.Common;
using FdoToolbox.Core.Utility;
using System.Collections.ObjectModel;

namespace FdoToolbox.Core.ETL.Specialized
{
    public enum ExpressProvider
    {
        SDF,
        SHP
    }

    /// <summary>
    /// Options object for the BulkCopyTask
    /// </summary>
    public class SpatialBulkCopyOptions : IDisposable
    {
        private FdoConnection _Source;
        private FdoConnection _Target;

        private string _TargetSchemaName;
        private string _SourceSchemaName;
        private List<ClassCopyOptions> _SourceClasses;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="source"></param>
        /// <param name="target"></param>
        public SpatialBulkCopyOptions(FdoConnection source, FdoConnection target)
        {
            _Source = source;
            _Target = target;
            _SourceClasses = new List<ClassCopyOptions>();
            _SourceSpatialContexts = new List<string>();
        }

        private bool _ExpressMode;

        /// <summary>
        /// Constructor for express bulk copy tasks. The target will be created
        /// and all necessary connections will be set up. Parameters will be set
        /// up so that ApplySchemaToTarget is true
        /// </summary>
        /// <param name="source"></param>
        /// <param name="target"></param>
        /// <param name="sourceFile"></param>
        /// <param name="targetFile"></param>
        public SpatialBulkCopyOptions(ExpressProvider source, ExpressProvider target, string sourceFile, string targetFile)
        {
            _SourceClasses = new List<ClassCopyOptions>();
            _SourceSpatialContexts = new List<string>();
            _ExpressMode = true;
            IConnection src = null;
            IConnection dest = null;

            switch (source)
            {
                case ExpressProvider.SDF:
                    src = FeatureAccessManager.GetConnectionManager().CreateConnection(ExpressUtility.PROVIDER_SDF);
                    src.ConnectionString = string.Format(ExpressUtility.CONN_FMT_SDF, sourceFile);
                    break;
                case ExpressProvider.SHP:
                    src = FeatureAccessManager.GetConnectionManager().CreateConnection(ExpressUtility.PROVIDER_SHP);
                    src.ConnectionString = string.Format(ExpressUtility.CONN_FMT_SHP, sourceFile);
                    break;
            }

            src.Open();

            //SDF and SHP are single schema, so grab the first schema from IDescribeSchema
            //and set it as the source schema name
            FeatureService srcService = new FeatureService(src);
            FeatureSchemaCollection schemas = srcService.DescribeSchema();
            if (schemas.Count == 0)
                throw new BulkCopyException("No schemas found on source connection");
            this.SourceSchemaName = schemas[0].Name;

            switch (target)
            {
                case ExpressProvider.SDF:
                    {
                        ExpressUtility.CreateSDF(targetFile);
                        dest = FeatureAccessManager.GetConnectionManager().CreateConnection(ExpressUtility.PROVIDER_SDF);
                        dest.ConnectionString = string.Format(ExpressUtility.CONN_FMT_SDF, targetFile);
                    }
                    break;
                case ExpressProvider.SHP:
                    {
                        dest = FeatureAccessManager.GetConnectionManager().CreateConnection(ExpressUtility.PROVIDER_SHP);
                        string name = Path.GetFileNameWithoutExtension(targetFile);
                        string path = Path.GetDirectoryName(targetFile);
                        DeleteRelatedShpFiles(path, name);
                        dest.ConnectionString = string.Format(ExpressUtility.CONN_FMT_SHP, path);
                    }
                    break;
            }

            dest.Open();

            if (src != null && dest != null)
            {
                this.Source = new FdoConnection("SOURCE", src);
                this.Target = new FdoConnection("TARGET", dest);
            }
        }

        private string _LogPath;

        /// <summary>
        /// The path where error logs will be written to
        /// </summary>
        public string LogPath
        {
            get { return _LogPath; }
            set { _LogPath = value; }
        }

        private string _GlobalSpatialFilter;

        /// <summary>
        /// A global spatial filter to apply to each class. (the condition
        /// is AND'ed to each class filter. Only applies to Feature Classes.
        /// </summary>
        public string GlobalSpatialFilter
        {
            get { return _GlobalSpatialFilter; }
            set { _GlobalSpatialFilter = value; }
        }

        private int _BatchInsertSize;

        /// <summary>
        /// The size of the batch parameter collection. If less than or equal to 0,
        /// batch parameters will be avoided.
        /// </summary>
        public int BatchInsertSize
        {
            get { return _BatchInsertSize; }
            set { _BatchInsertSize = value; }
        }

        /// <summary>
        /// Deletes a SHP file and all its related files
        /// </summary>
        /// <param name="path"></param>
        /// <param name="name"></param>
        private static void DeleteRelatedShpFiles(string path, string name)
        {
            string[] extensions = { "shp", "dbf", "prj", "shx", "idx", "cpg" };
            foreach (string ext in extensions)
            {
                string file = Path.Combine(path, string.Format("{0}.{1}", name, ext));
                File.Delete(file);
                AppConsole.WriteLine("[Express Bulk Copy]: Deleted file {0}", file);
            }
        }

        /// <summary>
        /// The source connection
        /// </summary>
        public FdoConnection Source
        {
            get { return _Source; }
            set { _Source = value; }
        }

        private List<string> _SourceSpatialContexts;

        /// <summary>
        /// The name of the source spatial context to copy over if we are
        /// to copy spatial contexts
        /// </summary>
        public ReadOnlyCollection<string> SourceSpatialContexts
        {
            get { return _SourceSpatialContexts.AsReadOnly(); }
        }

        /// <summary>
        /// Adds a source spatial context to be copied
        /// </summary>
        /// <param name="context"></param>
        public void AddSourceSpatialContext(string context)
        {
            _SourceSpatialContexts.Add(context);
        }

        /// <summary>
        /// The target connection
        /// </summary>
        public FdoConnection Target
        {
            get { return _Target; }
            set { _Target = value; }
        }

        private bool _CopySpatialContexts;

        /// <summary>
        /// If true copies all source spatial contexts from the source to the
        /// target. If this is a express mode bulk copy option, then it will
        /// automatically set the first available spatial context as the one
        /// to copy.
        /// </summary>
        public bool CopySpatialContexts
        {
            get { return _CopySpatialContexts; }
            set 
            { 
                _CopySpatialContexts = value;
                if (_ExpressMode)
                {
                    using (FeatureService service = new FeatureService(_Source.InternalConnection))
                    {
                        ReadOnlyCollection<SpatialContextInfo> contexts = service.GetSpatialContexts();
                        _SourceSpatialContexts.Clear();
                        _SourceSpatialContexts.Add(contexts[0].Name);
                    }
                }
            }
        }

        private bool _CoerceDataTypes;

        /// <summary>
        /// If true, the bulk copy process will attempt to convert the source
        /// data type to the target data type. This option is currently not in
        /// use.
        /// </summary>
        public bool CoerceDataTypes
        {
            get { return _CoerceDataTypes; }
            set { _CoerceDataTypes = value; }
        }

        /// <summary>
        /// If true, the mappings are ignored and the full source schema
        /// will be applied to the target. Is only true if the target
        /// schema is undefined
        /// </summary>
        public bool ApplySchemaToTarget
        {
            get { return string.IsNullOrEmpty(this.TargetSchemaName); }
        }

        /// <summary>
        /// The source schema
        /// </summary>
        public string SourceSchemaName
        {
            get { return _SourceSchemaName; }
            set { _SourceSchemaName = value; }
        }

        /// <summary>
        /// The target schema. If this schema does not exist in the
        /// target connection, then every class specified in the source
        /// will be copied across.
        /// </summary>
        public string TargetSchemaName
        {
            get { return _TargetSchemaName; }
            set { _TargetSchemaName = value; }
        }

        /// <summary>
        /// Adds a source class to be copied over
        /// </summary>
        /// <param name="options"></param>
        public void AddClassCopyOption(ClassCopyOptions options)
        {
            _SourceClasses.Add(options);
        }

        /// <summary>
        /// Removes all added class copy options
        /// </summary>
        public void ClearClassCopyOptions()
        {
            _SourceClasses.Clear();
        }

        /// <summary>
        /// Gets all the classes to copy
        /// </summary>
        /// <returns></returns>
        public ReadOnlyCollection<ClassCopyOptions> ClassCopyOptions
        {
            get
            {
                return _SourceClasses.AsReadOnly();
            }
        }

        private bool _AlterSchema;

        /// <summary>
        /// If true, will alter the source schema before applying to the target connection
        /// if the source schema is not compatible. Otherwise an exception will be thrown
        /// if the source schema is not compatbile.
        /// </summary>
        public bool AlterSchema
        {
            get { return _AlterSchema; }
            set { _AlterSchema = value; }
        }
	

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                //Since connections in express mode are created outside of the
                //connection manager's knowledge we have to clean them up explicitly
                if (_ExpressMode)
                {
                    if (this.Source.InternalConnection.ConnectionState == ConnectionState.ConnectionState_Open)
                        this.Source.InternalConnection.Close();
                    if (this.Target.InternalConnection.ConnectionState == ConnectionState.ConnectionState_Open)
                        this.Target.InternalConnection.Close();

                    this.Source.InternalConnection.Dispose();
                    this.Target.InternalConnection.Dispose();
                }
            }
        }
    }
}
