using System;
using System.Collections.Generic;
using System.Text;

namespace FdoToolbox.Core.ETL.Specialized
{
    /// <summary>
    /// A specialized form of ETL process.
    /// </summary>
    public abstract class FdoSpecializedEtlProcess : EtlProcess, IFdoSpecializedEtlProcess
    {
        protected void FireFeatureProcessed(int count)
        {
            FeatureProcessed(this, new FeatureCountEventArgs(count));
        }

        protected void SendMessage(string msg)
        {
            ProcessMessage(this, new MessageEventArgs(msg));
        }

        protected void SendMessageFormatted(string format, params object[] args)
        {
            ProcessMessage(this, new MessageEventArgs(string.Format(format, args)));
        }

        /// <summary>
        /// Fires when a feature has been processed
        /// </summary>
        public event FeatureCountEventHandler FeatureProcessed = delegate { };

        /// <summary>
        /// Fires when a message is sent from this process
        /// </summary>
        public event MessageEventHandler ProcessMessage = delegate { };

        protected override void OnFeatureProcessed(FdoToolbox.Core.ETL.Operations.FdoOperationBase op, FdoRow dictionary)
        {
            //We want to avoid log chatter on specialized ETL processes so suppress the base call   
        }

        protected override void OnFinishedProcessing(FdoToolbox.Core.ETL.Operations.FdoOperationBase op)
        {
            //We want to avoid log chatter on specialized ETL processes so suppress the base call
        }

        public EtlProcess ToEtlProcess()
        {
            return this;
        }

        protected override void OnProcessCompleted()
        {
            ProcessCompleted(this, EventArgs.Empty);
        }

        public event EventHandler ProcessCompleted = delegate { };

        public event EventHandler ProcessAborted = delegate { };
    }
}
