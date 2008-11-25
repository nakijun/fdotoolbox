using System;
using System.Collections.Generic;
using System.Text;

namespace FdoToolbox.Core.ETL.Specialized
{
    using Operations;
    using Feature;

    public class FdoClassCopyProcess : EtlProcess
    {
        private FdoClassCopyOptions _options;

        public FdoClassCopyProcess(FdoClassCopyOptions options)
        {
            _options = options;
        }

        protected override void Initialize()
        {
            Register(new FdoInputOperation(_options.SourceConnection, CreateSourceQuery()));
            if (_options.PropertyMappings.Count > 0)
                Register(new FdoOutputOperation(_options.TargetConnection, _options.TargetClassName, _options.PropertyMappings));
            else
                Register(new FdoOutputOperation(_options.TargetConnection, _options.TargetClassName));
        }

        private FeatureQueryOptions CreateSourceQuery()
        {
            FeatureQueryOptions query = new FeatureQueryOptions(_options.SourceClassName);
            query.AddFeatureProperty(_options.SourcePropertyNames);
            if (!string.IsNullOrEmpty(_options.SourceFilter))
                query.Filter = _options.SourceFilter;

            return query;
        }
    }
}
