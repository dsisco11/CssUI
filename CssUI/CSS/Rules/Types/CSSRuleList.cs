using System;
using System.Collections.Generic;
using System.Text;

namespace CssUI.CSS
{
    public class CSSRuleList : List<CSSRule>
    {

        public int InsertRule(int index, string rule)
        {
            CSSRule newRule = CSSRule.From_String(rule);
            if (index > this.Count)
                throw new IndexOutOfRangeException();

            /*
             * XXX: If new rule cannot be inserted into list at the zero-index position index due to constraints specified by CSS, then throw a HierarchyRequestError exception. [CSS21]
             * Note: For example, a CSS style sheet cannot contain an @import at-rule after a style rule.
             */

            /*
             * XXX: If new rule is an @namespace at-rule, and list contains anything other than @import at-rules, and @namespace at-rules, throw an InvalidStateError exception.
             */

            this.Insert(index, newRule);
            return index;
        }

        public void RemoveRule(int index)
        {
            if (index >= this.Count)
                throw new IndexOutOfRangeException();

            CSSRule oldRule = this[index];

            // XXX: If old rule is an @namespace at-rule, and list contains anything other than @import at-rules, and @namespace at-rules, throw an InvalidStateError exception.

            this.RemoveAt(index);
            //oldRule.parentRule = oldRule.parentStyleSheet = null;
        }
    }
}
