using System;
using System.Collections.Generic;
using System.Text;

namespace FdoToolbox.Core.ETL.Operations
{
    /// <summary>
    /// Pipeline operation interface
    /// </summary>
    public interface IFdoOperation : IDisposable
    {
        /// <summary>
        /// Name of the operation
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Fires when a feature is processed
        /// </summary>
        event FeatureProcessedEventHandler OnFeatureProcessed;

        /// <summary>
        /// Fires when all the features have been processed
        /// </summary>
        event FdoOperationEventHandler OnFinishedProcessing;

        /// <summary>
        /// Initializes the current instance
        /// </summary>
        /// <param name="pipelineExecuter">The current pipeline executer.</param>
        void PrepareForExecution(IPipelineExecuter pipelineExecuter);

        /// <summary>
        /// Executes this operation
        /// </summary>
        /// <param name="rows"></param>
        /// <returns></returns>
        IEnumerable<FdoRow> Execute(IEnumerable<FdoRow> rows);

        /// <summary>
        /// Raise the feature processed event
        /// </summary>
        /// <param name="row"></param>
        void RaiseFeatureProcessed(FdoRow row);

        /// <summary>
        /// Raises the finished processing event
        /// </summary>
        void RaiseFinishedProcessing();

        /// <summary>
        /// Gets all errors that occured when running this operation.
        /// </summary>
        /// <returns></returns>
        IEnumerable<Exception> GetAllErrors();
    }

    /// <summary>
    /// Event handler for tracking operation status
    /// </summary>
    public delegate void FdoOperationEventHandler(FdoOperationBase op);

    /// <summary>
    /// Event handler for processed features
    /// </summary>
    public delegate void FeatureProcessedEventHandler(FdoOperationBase op, FdoRow row);
}
