using System;
using System.Collections.Generic;
using System.Text;
using FdoToolbox.Core.ETL;
using System.Threading;

namespace FdoToolbox.Tasks
{
    public class EtlBackgroundRunner
    {
        private EtlProcess _proc;

        public EtlBackgroundRunner(EtlProcess proc)
        {
            _proc = proc;
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
    }
}
