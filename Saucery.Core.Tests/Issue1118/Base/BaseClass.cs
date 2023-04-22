using NUnit.Framework;

namespace Saucery.Core.Tests.Issue1118.Base;

[TestFixtureSource(typeof(SourceData))]
[Parallelizable(ParallelScope.All)]
public class BaseClass
{
    protected readonly string Browser;

    protected BaseClass(string browser)
    {
        Browser = browser;
    }

    [SetUp]
    public void Setup()
    {
    }

    [TearDown]
    public void Cleanup()
    {
    }
}