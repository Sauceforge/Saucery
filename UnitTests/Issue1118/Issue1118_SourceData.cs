using System.Collections;

namespace UnitTests.Issue1118 {
    public class Issue1118_SourceData : IEnumerable {
        public IEnumerator GetEnumerator() {
            yield return "Firefox";
            yield return "Chrome";
            yield return "Internet Explorer";
        }
    }
}