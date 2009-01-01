using System;
using System.Collections.Generic;
using System.Text;

namespace FdoToolbox.Core.AppFramework
{
    public class ConsoleColorSwitcher : IDisposable
    {
        private ConsoleColor originalColor;

        public ConsoleColorSwitcher(ConsoleColor color)
        {
            originalColor = Console.ForegroundColor;
            Console.ForegroundColor = color;
        }

        public void Dispose()
        {
            Console.ForegroundColor = originalColor;
        }
    }
}
