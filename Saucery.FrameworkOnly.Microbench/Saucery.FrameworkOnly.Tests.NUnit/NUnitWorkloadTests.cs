using NUnit.Framework;
using Saucery.FrameworkOnly.Workload;

namespace Saucery.FrameworkOnly.Tests.NUnit;

[TestFixture]
public sealed class NUnitWorkloadTests
{
    [TestCaseSource(nameof(Cases))]
    public void Compute_ShouldBeDeterministic(int i)
    {
        var a = WorkloadProvider.Compute(i);
        var b = WorkloadProvider.Compute(i);
        Assert.That(a, Is.EqualTo(b));
    }

    public static IEnumerable<int> Cases => Enumerable.Range(0, 1_000);
}
