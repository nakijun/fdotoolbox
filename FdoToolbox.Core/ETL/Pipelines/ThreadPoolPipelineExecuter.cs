using System;
using System.Collections.Generic;
using System.Text;

namespace FdoToolbox.Core.ETL.Pipelines
{
    using Enumerables;
    using Operations;
    using System.Threading;

    /// <summary>
    /// Execute all the actions concurrently, in the thread pool
    /// </summary>
    public class ThreadPoolPipelineExecuter : BasePipelineExecuter
    {
        /// <summary>
        /// Add a decorator to the enumerable for additional processing
        /// </summary>
        /// <param name="operation">The operation.</param>
        /// <param name="enumerator">The enumerator.</param>
        protected override IEnumerable<FdoRow> DecorateEnumerable(IFdoOperation operation, IEnumerable<FdoRow> enumerator)
        {
            ThreadSafeEnumerator<FdoRow> threadedEnumerator = new ThreadSafeEnumerator<FdoRow>();
            ThreadPool.QueueUserWorkItem(delegate
            {
                try
                {
                    foreach (FdoRow t in new EventRaisingEnumerator(operation, enumerator))
                    {
                        threadedEnumerator.AddItem(t);
                    }
                }
                catch (Exception e)
                {
                    Error(e, "Failed to execute operation {0}", operation);
                    threadedEnumerator.MarkAsFinished();
                }
                finally
                {
                    threadedEnumerator.MarkAsFinished();
                }
            });
            return threadedEnumerator;
        }
    }
}
