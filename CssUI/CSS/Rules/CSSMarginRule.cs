using CssUI.CSS.Internal;
using System;

namespace CssUI.CSS
{
    public sealed class CSSMarginRule : CSSRule
    {
        public new readonly ECssRuleType type = ECssRuleType.MARGIN_RULE;
        public readonly string name;
        public readonly CSSStyleDeclaration style;

        public CSSMarginRule(string name, CSSStyleDeclaration style)
        {
            this.name = name;
            this.style = style;
        }

        protected override string Serialize()
        {
            throw new NotImplementedException();
        }
    }
}
