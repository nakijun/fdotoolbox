using System;
using System.Collections.Generic;
using System.Text;

namespace FdoToolbox.Core.ETL.Operations
{
    /// <summary>
    /// Represent a single operation that can occur during the ETL process
    /// </summary>
    public abstract class FdoOperationBase : WithLoggingMixin, IFdoOperation
    {
        private readonly OperationStatistics statistics = new OperationStatistics();
        private IPipelineExecuter pipelineExecuter;

        /// <summary>
        /// Gets the pipeline executer
        /// </summary>
        protected IPipelineExecuter PipelineExecuter
        {
            get { return pipelineExecuter; }
        }

        /// <summary>
        /// Gets the name of this instance
        /// </summary>
        /// <value>The name.</value>
        public virtual string Name
        {
            get { return GetType().Name; }
        }

        /// <summary>
        /// Gets the statistics for this operation
        /// </summary>
        /// <value>The statistics.</value>
        public OperationStatistics Statistics
        {
            get { return statistics; }
        }

        /// <summary>
        /// Initializes this instance
        /// </summary>
        /// <param name="pipelineExecuter"></param>
        public virtual void PrepareForExecution(IPipelineExecuter pipelineExecuter)
        {
            this.pipelineExecuter = pipelineExecuter;
            Statistics.MarkStarted();
        }

        public event FeatureProcessedEventHandler OnFeatureProcessed = delegate { };

        public event FdoOperationEventHandler OnFinishedProcessing = delegate { };

        public abstract IEnumerable<FdoRow> Execute(IEnumerable<FdoRow> rows);

        public void RaiseFeatureProcessed(FdoRow row)
        {
            Statistics.MarkFeatureProcessed();
            OnFeatureProcessed(this, row);
        }

        public void RaiseFinishedProcessing()
        {
            Statistics.MarkFinished();
            OnFinishedProcessing(this);
        }

        /// <summary>
        /// Gets all errors that occured when running this operation
        /// </summary>
        /// <returns></returns>
        public virtual IEnumerable<Exception> GetAllErrors()
        {
            return this.Errors;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            
        }
    }
}
