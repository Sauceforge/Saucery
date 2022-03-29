using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Saucery.Util
{
    internal class OnceOnlyWriter {
        private static readonly HashSet<string> WrittenMessages = new();

        [MethodImpl(MethodImplOptions.NoInlining)]
        public static void WriteLine(string message) {
            if (!WrittenMessages.Contains(message)) {
                Console.WriteLine(message);
                Console.Out.Flush();
                WrittenMessages.Add(message);
            }
        }
    }
}

/*
 * Author: Andrew Gray, SauceForge
 * Date: 17th December 2015
 * 
 */