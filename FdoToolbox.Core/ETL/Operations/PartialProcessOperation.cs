using System;
using System.Collections.Generic;
using System.Text;

namespace FdoToolbox.Core.ETL.Operations
{
    /// <summary>
    /// A partial process that can take part in another process
    /// </summary>
    public class PartialProcessOperation : EtlProcessBase<PartialProcessOperation>, IFdoOperation
    {
        private IPipelineExecuter pipelineExeuter;
        private readonly OperationStatistics statistics = new OperationStatistics();

        /// <summary>
        /// Occurs when all the rows has finished processing.
        /// </summary>
        public event FdoOperationEventHandler OnFinishedProcessing
        {
            add
            {
                foreach (IFdoOperation operation in operations)
                {
                    operation.OnFinishedProcessing += value;
                }
            }
            remove
            {
                foreach (IFdoOperation operation in operations)
                {
                    operation.OnFinishedProcessing -= value;
                }
            }
        }

        /// <summary>
        /// Initializes the current instance
        /// </summary>
        /// <param name="pipelineExecuter">The current pipeline executer.</param>
        public void PrepareForExecution(IPipelineExecuter pipelineExecuter)
        {
            Statistics.MarkStarted();
            this.pipelineExeuter = pipelineExecuter;
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
        /// Occurs when a row is processed.
        /// </summary>
        public event FeatureProcessedEventHandler OnFeatureProcessed
        {
            add
            {
                foreach (IFdoOperation operation in operations)
                {
                    operation.OnFeatureProcessed += value;
                }
            }
            remove
            {
                foreach (IFdoOperation operation in operations)
                {
                    operation.OnFeatureProcessed -= value;
                }
            }
        }

        /// <summary>
        /// Executes this operation
        /// </summary>
        /// <param name="rows">The rows.</param>
        /// <returns></returns>
        public IEnumerable<FdoRow> Execute(IEnumerable<FdoRow> rows)
        {
            MergeLastOperationsToOperations();
            return pipelineExeuter.PipelineToEnumerable(operations, rows);
        }

        ///<summary>
        ///Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        ///</summary>
        ///<filterpriority>2</filterpriority>
        public void Dispose()
        {
            foreach (IFdoOperation operation in operations)
            {
                operation.Dispose();
            }
        }

        /// <summary>
        /// Raises the row processed event
        /// </summary>
        /// <param name="dictionary">The dictionary.</param>
        void IFdoOperation.RaiseFeatureProcessed(FdoRow dictionary)
        {
            Statistics.MarkFeatureProcessed();
            // we don't have a real event here, so we ignore it
            // it will be handled by the children at any rate
        }

        /// <summary>
        /// Raises the finished processing event
        /// </summary>
        void IFdoOperation.RaiseFinishedProcessing()
        {
            Statistics.MarkFinished();
            // we don't have a real event here, so we ignore it
            // it will be handled by the children at any rate
        }

        /// <summary>
        /// Gets all errors that occured when running this operation
        /// </summary>
        /// <returns></returns>
        public IEnumerable<Exception> GetAllErrors()
        {
            foreach (IFdoOperation operation in operations)
            {
                foreach (Exception error in operation.GetAllErrors())
                {
                    yield return error;
                }
            }
        }
    }
}
