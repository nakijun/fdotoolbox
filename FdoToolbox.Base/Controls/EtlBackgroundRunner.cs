#region LGPL Header
// Copyright (C) 2009, Jackie Ng
// http://code.google.com/p/fdotoolbox, jumpinjackie@gmail.com
// 
// This library is free software; you can redistribute it and/or
// modify it under the terms of the GNU Lesser General Public
// License as published by the Free Software Foundation; either
// version 2.1 of the License, or (at your option) any later version.
// 
// This library is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
// Lesser General Public License for more details.
// 
// You should have received a copy of the GNU Lesser General Public
// License along with this library; if not, write to the Free Software
// Foundation, Inc., 51 Franklin Street, Fifth Floor, Boston, MA  02110-1301  USA
// 
//
// See license.txt for more/additional licensing information
#endregion
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
                EtlProcess p = _proc.ToEtlProcess();
                if (p.Errors.Length > 0)
                {
                    this.ProcessMessage(this, new MessageEventArgs(p.Errors.Length + " errors were found"));
                }
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
