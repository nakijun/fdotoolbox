using System;
using System.Collections.Generic;
using System.Text;

namespace FdoToolbox.Core.ETL.Specialized
{
    using Operations;
    using FdoToolbox.Core.Feature;
    using OSGeo.FDO.Commands.Feature;

    /// <summary>
    /// A specialized form of <see cref="EtlProcess"/> that copies
    /// a series of feature classes from one source to another
    /// </summary>
    public class FdoBulkCopy : EtlProcess
    {
        private FdoBulkCopyOptions _options;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="options"></param>
        public FdoBulkCopy(FdoBulkCopyOptions options)
        {
            _options = options;
        }

        /// <summary>
        /// Initializes the process
        /// </summary>
        protected override void Initialize()
        {
            foreach (FdoClassCopyOptions copt in _options.ClassCopyOptions)
            {
                if (copt.DeleteTarget)
                {
                    Info("Deleting data in target class {0} before copying", copt.TargetClassName);
                    using (FdoFeatureService service = copt.TargetConnection.CreateFeatureService())
                    {
                        using (IDelete del = service.CreateCommand<IDelete>(OSGeo.FDO.Commands.CommandType.CommandType_Delete) as IDelete)
                        {
                            try
                            {
                                del.SetFeatureClassName(copt.TargetClassName);
                                del.Execute();
                                Info("Data in target class {0} deleted");
                            }
                            catch
                            {

                            }
                        }
                    }
                }

                Register(new FdoInputOperation(copt.SourceConnection, CreateSourceQuery(copt)));
                if (copt.PropertyMappings.Count > 0)
                    Register(new FdoOutputOperation(copt.TargetConnection, copt.TargetClassName, copt.PropertyMappings));
                else
                    Register(new FdoOutputOperation(copt.TargetConnection, copt.TargetClassName));
            }
        }

        private static FeatureQueryOptions CreateSourceQuery(FdoClassCopyOptions copt)
        {
            FeatureQueryOptions query = new FeatureQueryOptions(copt.SourceClassName);
            query.AddFeatureProperty(copt.SourcePropertyNames);
            query.AddComputedProperty(copt.SourceExpressions);
            if (!string.IsNullOrEmpty(copt.SourceFilter))
                query.Filter = copt.SourceFilter;

            return query;
        }

        /// <summary>
        /// Saves the bulk copy configuration
        /// </summary>
        /// <param name="file"></param>
        public override void Save(string file)
        {
            
        }
    }
}
