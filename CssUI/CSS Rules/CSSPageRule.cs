using CssUI.Enums;
using System;

namespace CssUI.CSS
{
    public sealed class CSSPageRule : CSSGroupingRule
    {
        public new readonly ECssRuleType type = ECssRuleType.PAGE_RULE;
        public CssSelector selector;
        public readonly CSSStyleDeclaration style;

        public CSSPageRule(CssSelector selector, CSSStyleDeclaration style)
        {
            this.selector = selector;
            this.style = style;
        }
    }
}
