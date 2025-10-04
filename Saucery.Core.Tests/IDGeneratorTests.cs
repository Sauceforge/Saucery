using Saucery.Core.Util;
using Shouldly;
using Xunit;

namespace Saucery.Core.Tests;

public class IdGeneratorTests
{
    [Fact]
    public void SingleRunTest()
    {
        var generator = IdGenerator.Instance;
        generator.ShouldNotBeNull();
        IdGenerator.Id.ShouldNotBeNull();
    }

    [Fact]
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
