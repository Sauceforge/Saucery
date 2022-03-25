using Saucery.Util;
using System.Collections;

namespace UnitTests.Issue1118 {
    public class Issue1118_SourceData : IEnumerable {
        public IEnumerator GetEnumerator() {
            yield return SauceryConstants.BROWSER_FIREFOX;
            yield return SauceryConstants.BROWSER_CHROME;
            yield return SauceryConstants.BROWSER_IE;
        }
    }
}