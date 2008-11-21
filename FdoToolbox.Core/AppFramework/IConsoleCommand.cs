using System;
using System.Collections.Generic;
using System.Text;

namespace FdoToolbox.Core.AppFramework
{
    /// <summary>
    /// Console command interface
    /// </summary>
    public interface IConsoleCommand
    {
        /// <summary>
        /// Execute the command
        /// </summary>
        /// <returns>The status code of the execution</returns>
        int Execute();

        /// <summary>
        /// If true, executes under simulation. Nothing is changed by the command.
        /// </summary>
        bool IsTestOnly { get; set; }

        /// <summary>
        /// If true, suppresses all console output.
        /// </summary>
        bool IsSilent { get; set; }
    }
}
