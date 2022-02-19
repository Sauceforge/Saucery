using System;

namespace Saucery.Util
{
    public sealed class IDGenerator
    {
        private static readonly IDGenerator instance = new();
        private string TheId = null;

        public string Id => instance.TheId;

        // Explicit static constructor to tell C# compiler
        // not to mark type as beforefieldinit
        static IDGenerator()
        {
        }

        private IDGenerator()
        {
            if (TheId == null)
            {
                TheId = Guid.NewGuid().ToString();
            }
        }

        public static IDGenerator Instance
        {
            get
            {
                return instance;
            }
        }
    }
}