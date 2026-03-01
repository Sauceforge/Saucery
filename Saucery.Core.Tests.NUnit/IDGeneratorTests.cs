using NUnit.Framework;
using Saucery.Core.Util;
using Shouldly;

namespace Saucery.Core.Tests.NUnit;

public class IdGeneratorTests
{
    [Test]
    public void SingleRunTest()
    {
        var generator = IdGenerator.Instance;
        generator.ShouldNotBeNull();
        IdGenerator.Id.ShouldNotBeNull();
    }

    [Test]
    public void MultipleRunTest()
    {
        var generator = IdGenerator.Instance;
        generator.ShouldNotBeNull();
        IdGenerator.Id.ShouldNotBeNull();

        var generator2 = IdGenerator.Instance;
        generator2.ShouldNotBeNull();
        IdGenerator.Id.ShouldNotBeNull();
        IdGenerator.Id.ShouldBeEquivalentTo(IdGenerator.Id);
    }
}
