using NUnit.Framework;
using Saucery.Core.Tests.Issue1118.Base;

namespace Saucery.Core.Tests.Issue1118;

public class Fixture : BaseClass {
    public Fixture(string browser) : base(browser) {
    }

    [Test]
    //[Ignore("Only needs to exist for Issue1118 Test")]
    public void Test1() {
    }

    [Test]
    //[Ignore("Only needs to exist for Issue1118 Test")]
    public void Test2() {
    }
}