using System;
using System.Collections.Generic;
using System.Text;

namespace FdoToolbox.Core.AppFramework
{
    /// <summary>
    /// A helper object to switch the text color of the console. All text written
    /// out to the console will be of the specified color until this object is 
    /// disposed of.
    /// </summary>
    public class ConsoleColorSwitcher : IDisposable
    {
        private ConsoleColor originalColor;

        /// <summary>
        /// Initializes a new instance of the <see cref="ConsoleColorSwitcher"/> class.
        /// </summary>
        /// <param name="color">The color.</param>
        public ConsoleColorSwitcher(ConsoleColor color)
        {
            originalColor = Console.ForegroundColor;
            Console.ForegroundColor = color;
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            Console.ForegroundColor = originalColor;
        }
    }
}
