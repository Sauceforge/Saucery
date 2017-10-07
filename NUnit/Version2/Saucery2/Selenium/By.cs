using System.Globalization;

namespace Saucery2.Selenium {
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

            public JQueryBy Children(string selector = "") {
                return Function("children", selector);
            }

            public JQueryBy Closest(string selector = "") {
                return Function("closest", selector);
            }

            public JQueryBy Find(string selector = "") {
                return Function("find", selector);
            }

            public JQueryBy Next(string selector = "") {
                return Function("next", selector);
            }

            public JQueryBy NextAll(string selector = "") {
                return Function("nextAll", selector);
            }

            public JQueryBy NextUntil(string selector = "", string filter = "") {
                return Function("nextUntil", selector, filter);
            }

            public JQueryBy OffsetParent() {
                return Function("offsetParent");
            }

            public JQueryBy Parent(string selector = "") {
                return Function("parent", selector);
            }

            public JQueryBy Parents(string selector = "") {
                return Function("parents", selector);
            }

            public JQueryBy ParentsUntil(string selector = "", string filter = "") {
                return Function("parentsUntil", selector, filter);
            }

            public JQueryBy Prev(string selector = "") {
                return Function("prev", selector);
            }

            public JQueryBy PrevAll(string selector = "") {
                return Function("prevAll", selector);
            }

            public JQueryBy PrevUntil(string selector = "", string filter = "") {
                return Function("prevUntil", selector, filter);
            }

            public JQueryBy Siblings(string selector = "") {
                return Function("siblings", selector);
            }

            #endregion

            #region -----Filtering----

            public JQueryBy Eq(int index) {
                return Function("eq", index.ToString(CultureInfo.InvariantCulture));
            }

            public JQueryBy First() {
                return Function("first");
            }

            public JQueryBy Has(string selector) {
                return Function("has", selector);
            }

            public JQueryBy Last() {
                return Function("last");
            }

            public JQueryBy Not(string selector) {
                return Function("not", selector);
            }

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
}
/*
 * Copyright Andrew Gray, Full Circle Solutions
 * Date: 27th July 2014
 * 
 */