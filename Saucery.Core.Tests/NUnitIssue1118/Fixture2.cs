using NUnit.Framework;
using Saucery.Core.Tests.NUnitIssue1118.Base;

namespace Saucery.Core.Tests.NUnitIssue1118;

public class Fixture2(string browser) : BaseClass(browser) {
    [Test]
    //[Ignore("Only needs to exist for Issue1118 Test")]
    public void Test3() {
    }

    [Test]
    //[Ignore("Only needs to exist for Issue1118 Test")]
    public void Test4() {
    }
}