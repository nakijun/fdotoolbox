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
    public class FdoBulkCopy : FdoSpecializedEtlProcess
    {
        private FdoBulkCopyOptions _options;

        //private TimeSpan _totalSpan;
        //private int _featuresCopied;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="options"></param>
        public FdoBulkCopy(FdoBulkCopyOptions options)
        {
            _options = options;
            //_totalSpan = new TimeSpan();
        }

        /// <summary>
        /// Initializes the process
        /// </summary>
        protected override void Initialize()
        {
            //Copy Spatial Contexts
            IList<SpatialContextInfo> contexts = _options.SourceSpatialContexts;
            if (contexts.Count > 0)
            {   
                using (FdoFeatureService targetService = _options.TargetConnection.CreateFeatureService())
                {
                    if (_options.BatchSize > 0 && !targetService.SupportsBatchInsertion())
                    {
                        SendMessage("Batch insert not supported. Using regular inserts");
                        _options.BatchSize = 0;
                    }

                    SendMessage("Copying Spatial Contexts");
                    if (_options.TargetConnection.Capability.GetBooleanCapability(CapabilityType.FdoCapabilityType_SupportsMultipleSpatialContexts).Value)
                    {
                        foreach (SpatialContextInfo ctx in contexts)
                        {
                            targetService.CreateSpatialContext(ctx, true);
                            SendMessage("Spatial Context Copied: " + ctx.Name);
                        }
                    }
                    else
                    {
                        SpatialContextInfo srcCtx = contexts[0];
                        SpatialContextInfo destCtx = targetService.GetSpatialContext(srcCtx.Name);
                        bool canDestroy = targetService.SupportsCommand(OSGeo.FDO.Commands.CommandType.CommandType_DestroySchema);
                        if (canDestroy)
                        {
                            SendMessage("Destroying target spatial context: " + destCtx.Name);
                            targetService.DestroySpatialContext(destCtx);
                        }
                        SendMessage("Copying spatial context to target: " + srcCtx.Name);
                        targetService.CreateSpatialContext(srcCtx, !canDestroy);
                    }
                }
            }

            //Set class copy tasks
            for(int i = 0; i < _options.ClassCopyOptions.Count; i++)
            {
                FdoClassCopyOptions copt = _options.ClassCopyOptions[i];
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

                IFdoOperation input = new FdoInputOperation(copt.SourceConnection, CreateSourceQuery(copt)); 
                IFdoOperation output = null;
                if (copt.PropertyMappings.Count > 0)
                {
                    if (_options.BatchSize > 0)
                        output = new FdoBatchedOutputOperation(copt.TargetConnection, copt.TargetClassName, copt.PropertyMappings, _options.BatchSize);
                    else
                        output = new FdoOutputOperation(copt.TargetConnection, copt.TargetClassName, copt.PropertyMappings);
                }
                else
                {
                    if(_options.BatchSize > 0)
                        output = new FdoBatchedOutputOperation(copt.TargetConnection, copt.TargetClassName, _options.BatchSize);
                    else
                        output = new FdoOutputOperation(copt.TargetConnection, copt.TargetClassName);
                }

                string sourceClass = copt.SourceClassName;
                string targetClass = copt.TargetClassName;

                Register(input);
                Register(output);
            }
        }

        /// <summary>
        /// Called when a row is processed.
        /// </summary>
        /// <param name="op">The operation.</param>
        /// <param name="dictionary">The dictionary.</param>
        protected override void OnFeatureProcessed(FdoOperationBase op, FdoRow dictionary)
        {
            if (op.Statistics.OutputtedRows % 50 == 0)
            {
                if (op is FdoOutputOperation)
                {
                    string className = (op as FdoOutputOperation).ClassName;
                    SendMessageFormatted("[Bulk Copy => {0}]: {1} features written", className, op.Statistics.OutputtedRows);
                }
            }
        }

        /// <summary>
        /// Called when this process has finished processing.
        /// </summary>
        /// <param name="op">The op.</param>
        protected override void OnFinishedProcessing(FdoOperationBase op)
        {
            if (op is FdoOutputOperation)
            {
                string className = (op as FdoOutputOperation).ClassName;
                SendMessageFormatted("[Bulk Copy => {0}]: {1} features written in {2}", className, op.Statistics.OutputtedRows, op.Statistics.Duration.ToString());
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
        /// Releases unmanaged and - optionally - managed resources
        /// </summary>
        /// <param name="disposing"><c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _options.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
