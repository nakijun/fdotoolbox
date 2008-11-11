using System;
using System.Collections.Generic;
using System.Text;

namespace FdoToolbox.Core.ETL.Pipelines
{
    using Enumerables;
    using Operations;

    /// <summary>
    /// Executes the pipeline on a single thread
    /// </summary>
    public class SingleThreadedPipelineExecuter : BasePipelineExecuter
    {
        /// <summary>
        /// Add a decorator to the enumerable for additional processing
        /// </summary>
        /// <param name="operation">The operation.</param>
        /// <param name="enumerator">The enumerator.</param>
        protected override IEnumerable<FdoRow> DecorateEnumerable(IFdoOperation operation, IEnumerable<FdoRow> enumerator)
        {
            return new EventRaisingEnumerator(operation, enumerator);
        }
    }
}
