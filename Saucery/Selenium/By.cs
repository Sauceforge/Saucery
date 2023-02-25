using System.Globalization;

namespace Saucery.Selenium; 

internal class By : OpenQA.Selenium.By {
    /// <summary>
    /// Specialized "By" class for jQuery selector
    /// </summary>
    public class JQueryBy {
        public string Selector { get; set; }

        public JQueryBy(string selector) {
            Selector = selector;
        }

        #region ----Tree Traversal----

        public JQueryBy Children(string selector = "") => Function("children", selector);

        public JQueryBy Closest(string selector = "") => Function("closest", selector);

        public JQueryBy Find(string selector = "") => Function("find", selector);

        public JQueryBy Next(string selector = "") => Function("next", selector);

        public JQueryBy NextAll(string selector = "") => Function("nextAll", selector);

        public JQueryBy NextUntil(string selector = "", string filter = "") => Function("nextUntil", selector, filter);

        public JQueryBy OffsetParent() => Function("offsetParent");

        public JQueryBy Parent(string selector = "") => Function("parent", selector);

        public JQueryBy Parents(string selector = "") => Function("parents", selector);

        public JQueryBy ParentsUntil(string selector = "", string filter = "") => Function("parentsUntil", selector, filter);

        public JQueryBy Prev(string selector = "") => Function("prev", selector);

        public JQueryBy PrevAll(string selector = "") => Function("prevAll", selector);

        public JQueryBy PrevUntil(string selector = "", string filter = "") => Function("prevUntil", selector, filter);

        public JQueryBy Siblings(string selector = "") => Function("siblings", selector);

        #endregion

        #region -----Filtering----

        public JQueryBy Eq(int index) => Function("eq", index.ToString(CultureInfo.InvariantCulture));

        public JQueryBy First() => Function("first");

        public JQueryBy Has(string selector) => Function("has", selector);

        public JQueryBy Last() => Function("last");

        public JQueryBy Not(string selector) => Function("not", selector);

        #endregion

        private JQueryBy Function(string func, string selector = "", string additionalArg = "") {
            //Add quotes to selector
            if(selector != "") {
                selector = "\"" + selector + "\"";
            }
            //Add additional paramater
            if(additionalArg != "") {
                selector += ",\"" + additionalArg + "\"";
            }
            //Add either: .func() or .func("selector") to original selector
            return new JQueryBy(Selector + "." + func + "(" + selector + ")");
        }
    }
}
/*
* Copyright Andrew Gray, SauceForge
* Date: 27th July 2014
* 
*/