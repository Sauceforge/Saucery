using Saucery.Core.Dojo;

namespace Merlin.TUnit
{
    public record DataDrivenData
    {
        public BrowserVersion RequestedPlatform { get; set; }
        public int Data { get; set; }
    }
}
