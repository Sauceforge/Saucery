using System.Collections;
using Saucery.Core.Util;

namespace Saucery.Core.Tests.NUnitIssue1118;

public class SourceData : IEnumerable {
    public IEnumerator GetEnumerator() {
        yield return SauceryConstants.BROWSER_FIREFOX;
        yield return SauceryConstants.BROWSER_CHROME;
        yield return SauceryConstants.BROWSER_IE;
    }
}