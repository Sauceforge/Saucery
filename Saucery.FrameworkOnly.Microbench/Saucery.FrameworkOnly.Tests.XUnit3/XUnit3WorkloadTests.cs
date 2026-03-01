using Saucery.FrameworkOnly.Workload;
using Xunit;

namespace Saucery.FrameworkOnly.Tests.XUnit3;

public sealed class XUnit3WorkloadTests
{
    public static IEnumerable<object[]> Cases
        => Enumerable.Range(0, 1_000).Select(i => new object[] { i });

    [Theory]
    [MemberData(nameof(Cases))]
    public void Compute_ShouldBeDeterministic(int i)
    {
        var a = WorkloadProvider.Compute(i);
        var b = WorkloadProvider.Compute(i);
        Assert.Equal(a, b);
    }
}
