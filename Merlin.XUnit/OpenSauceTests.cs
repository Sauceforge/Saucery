using Saucery.Core.Dojo;
using Saucery.XUnit;

namespace Merlin.XUnit;

public class OpenSauceTests : SauceryXBase
{
    public OpenSauceTests(BrowserVersion browserVersion) : base(browserVersion)
    {
    }

    
    //[Theory, ClassData(typeof(RequestedPlatformData))]
    public void Test1()
    {

    }
}