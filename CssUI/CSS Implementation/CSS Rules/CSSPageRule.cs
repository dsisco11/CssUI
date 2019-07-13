namespace CssUI.CSS.Internal
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
