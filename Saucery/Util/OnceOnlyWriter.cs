using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Saucery.Util
{
    internal class OnceOnlyWriter {
        private static readonly HashSet<string> WrittenMessages = new HashSet<string>();

        [MethodImpl(MethodImplOptions.NoInlining)]
        public static void WriteLine(string message) {
            if (!WrittenMessages.Contains(message)) {
                Console.WriteLine(message);
                Console.Out.Flush();
                WrittenMessages.Add(message);
            }
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        public static void WriteLine(string messageWithParams, object o1) {
            WriteLine(string.Format(messageWithParams, o1));
        }
    }
}

/*
 * Author: Andrew Gray, SauceForge
 * Date: 17th December 2015
 * 
 */