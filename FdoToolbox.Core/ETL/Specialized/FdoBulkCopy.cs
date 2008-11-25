using System;
using System.Collections.Generic;
using System.Text;

namespace FdoToolbox.Core.ETL.Specialized
{
    public class FdoBulkCopy : IDisposable
    {
        private FdoBulkCopyOptions _options;
        private List<FdoClassCopyProcess> _classCopyProcesses;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="options"></param>
        public FdoBulkCopy(FdoBulkCopyOptions options)
        {
            _options = options;
            _classCopyProcesses = new List<FdoClassCopyProcess>();
            foreach (FdoClassCopyOptions copt in _options.ClassCopyOptions)
            {
                FdoClassCopyProcess proc = new FdoClassCopyProcess(copt);
                _classCopyProcesses.Add(proc);
            }
        }

        /// <summary>
        /// Finalizer
        /// </summary>
        ~FdoBulkCopy()
        {
            Dispose(false);
        }

        private List<Exception> _errors = new List<Exception>();

        /// <summary>
        /// Executes the bulk copy
        /// </summary>
        public void Execute()
        {
            //Validate options

            foreach (FdoClassCopyProcess fcp in _classCopyProcesses)
            {
                fcp.Execute();
                if (fcp.Errors != null)
                    _errors.AddRange(fcp.Errors);
            }
        }

        public Exception[] Errors
        {
            get { return _errors.ToArray(); }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected void Dispose(bool disposing)
        {
            if (disposing)
            {
                foreach (FdoClassCopyProcess fcp in _classCopyProcesses)
                {
                    fcp.Dispose();
                }
                _classCopyProcesses.Clear();
            }
        }
    }
}
