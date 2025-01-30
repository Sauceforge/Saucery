using NUnit.Framework;

namespace Saucery.Core.Tests.NUnitIssue1118.Base;

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
        //Testing purposes only
    }

    [TearDown]
    public void Cleanup()
    {
        //Testing purposes only
    }
}