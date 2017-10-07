using System;
using KeyMaster.Core;

namespace KeyMaster {
    internal class Program {
        private static void Main(string[] args) {
            Console.WriteLine("New Symmetric Key:\n\n{0}", KeyGenerator.GetSymmetricKey());

        }
    }
}