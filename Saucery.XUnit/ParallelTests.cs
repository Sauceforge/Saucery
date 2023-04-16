using Xunit;

[assembly: CollectionBehavior(MaxParallelThreads = 4)]

namespace Saucery.XUnit;

public class ParallelTests : IDisposable
{
    public void Dispose()
    {
        // Closure handled in each test case
    }
}