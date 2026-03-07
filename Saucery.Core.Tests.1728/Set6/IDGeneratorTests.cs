using Saucery.Core.Util;
using Shouldly;

namespace Set6;

public class IdGeneratorTests6
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
