using System;
using System.Collections.Generic;
using System.Text;

namespace FdoToolbox.Core.ETL.Specialized
{
    /// <summary>
    /// Specialized ETL process interface
    /// </summary>
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

    /// <summary>
    /// Event handler for counting features processed
    /// </summary>
    public delegate void FeatureCountEventHandler(object sender, FeatureCountEventArgs e);

    /// <summary>
    /// Event object for counting features processed
    /// </summary>
    public class FeatureCountEventArgs
    {
        /// <summary>
        /// The number of features
        /// </summary>
        public readonly int Features;

        /// <summary>
        /// Initializes a new instance of the <see cref="FeatureCountEventArgs"/> class.
        /// </summary>
        /// <param name="features">The features.</param>
        public FeatureCountEventArgs(int features)
        {
            this.Features = features;
        }
    }

    /// <summary>
    /// Event handler for sending messages
    /// </summary>
    public delegate void MessageEventHandler(object sender, MessageEventArgs e);
}
