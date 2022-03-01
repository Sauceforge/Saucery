using System.Collections;

namespace UnitTests.Issue1118 {
    public class Issue1118_SourceData : IEnumerable {
        public IEnumerator GetEnumerator() {
            yield return "firefox";
            yield return "chrome";
            yield return "internet explorer";
        }
    }
}