using System;
using System.Collections.Generic;
using System.Text;

namespace FdoToolbox.Core.ETL.Specialized
{
    public interface IFdoSpecializedEtlProcess
    {
        /// <summary>
        /// Executes the process
        /// </summary>
        void Execute();
        
        /// <summary>
        /// Fires when a feature has been processed
        /// </summary>
        event FeatureCountEventHandler FeatureProcessed;

        /// <summary>
        /// Fires when a feature
        /// </summary>
        event MessageEventHandler ProcessMessage;

        /// <summary>
        /// Casts this to a <see cref="EtlProcess"/>
        /// </summary>
        /// <returns></returns>
        EtlProcess ToEtlProcess();
    }

    public delegate void FeatureCountEventHandler(object sender, FeatureCountEventArgs e);

    public class FeatureCountEventArgs
    {
        public readonly int Features;

        public FeatureCountEventArgs(int features)
        {
            this.Features = features;
        }
    }

    public delegate void MessageEventHandler(object sender, MessageEventArgs e);
}
