using System;
using System.Collections.Generic;
using System.Text;
using System.Collections.ObjectModel;
using System.Collections.Specialized;

namespace FdoToolbox.Core.ETL.Specialized
{
    using Feature;

    public class FdoBulkCopyOptions 
    {
        private FdoConnection _sourceConn;

        /// <summary>
        /// Gets the source connection to read features from
        /// </summary>
        public FdoConnection SourceConnection
        {
            get { return _sourceConn; }
        }

        private FdoConnection _targetConn;

        /// <summary>
        /// Gets the target connection to write features to
        /// </summary>
        public FdoConnection TargetConnection
        {
            get { return _targetConn; }
        }

        private List<FdoClassCopyOptions> _classOptions;

        /// <summary>
        /// Gets the collection of class copy options
        /// </summary>
        public ReadOnlyCollection<FdoClassCopyOptions> ClassCopyOptions
        {
            get { return _classOptions.AsReadOnly(); }
        }

        private int _BatchSize;

        /// <summary>
        /// Gets or sets the batch size. If greater than zero, a batched
        /// insert operation will be used in place of a regular insert operation
        /// (if supported by the target connection)
        /// </summary>
        public int BatchSize
        {
            get { return _BatchSize; }
            set 
            { 
                _BatchSize = value;
                _classOptions.ForEach(delegate(FdoClassCopyOptions copt)
                {
                    copt.BatchSize = value;
                });
            }
        }
	

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="source"></param>
        /// <param name="target"></param>
        /// <param name="classOptions"></param>
        public FdoBulkCopyOptions(FdoConnection source, FdoConnection target)
        {
            _sourceConn = source;
            _targetConn = target;
            _classOptions = new List<FdoClassCopyOptions>();
            this.BatchSize = 0;
        }

        /// <summary>
        /// Adds a class copy option
        /// </summary>
        /// <param name="sourceClass"></param>
        /// <param name="targetClass"></param>
        public void AddClassCopyOption(string sourceClass, string targetClass)
        {
            FdoClassCopyOptions copt = new FdoClassCopyOptions(_sourceConn, _targetConn, sourceClass, targetClass);
            copt.BatchSize = this.BatchSize;
            _classOptions.Add(copt);
        }

        /// <summary>
        /// Adds a class copy option with property mappings
        /// </summary>
        /// <param name="sourceClass"></param>
        /// <param name="targetClass"></param>
        /// <param name="mappings"></param>
        public void AddClassCopyOption(string sourceClass, string targetClass, NameValueCollection mappings)
        {
            FdoClassCopyOptions copt = new FdoClassCopyOptions(_sourceConn, _targetConn, sourceClass, targetClass);
            foreach (string key in mappings.AllKeys)
            {
                copt.AddPropertyMapping(key, mappings[key]);
            }
            copt.BatchSize = this.BatchSize;
            _classOptions.Add(copt);
        }
    }
}
