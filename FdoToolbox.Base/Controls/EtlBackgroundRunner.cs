using System;
using System.Collections.Generic;
using System.Text;
using FdoToolbox.Core.ETL;
using System.Threading;
using FdoToolbox.Core.ETL.Specialized;
using FdoToolbox.Core;

namespace FdoToolbox.Base.Controls
{
    public class EtlBackgroundRunner
    {
        private IFdoSpecializedEtlProcess _proc;

        public EtlBackgroundRunner(IFdoSpecializedEtlProcess proc)
        {
            _proc = proc;
            _proc.FeatureProcessed += delegate(object sender, FeatureCountEventArgs e)
            {
                this.FeatureProcessed(sender, e);
            };
            _proc.ProcessMessage += delegate(object sender, MessageEventArgs e)
            {
                this.ProcessMessage(sender, e);
            };
        }

        private Thread _execThread;

        public Thread ExecutingThread
        {
            get { return _execThread; }
            private set { _execThread = value; }
        }

        public void Run()
        {
            this.ExecutingThread = Thread.CurrentThread;
            try
            {
                _proc.Execute();
            }
            catch (ThreadAbortException)
            {
                Thread.ResetAbort();
            }
        }

        /// <summary>
        /// Fires when a feature has been processed
        /// </summary>
        public event FeatureCountEventHandler FeatureProcessed = delegate { };

        /// <summary>
        /// Fires when a feature
        /// </summary>
        public event MessageEventHandler ProcessMessage = delegate { };
    }
}
