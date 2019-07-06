using CssUI.Enums;
using System;

namespace CssUI.CSS
{
    public sealed class CSSStyleRule : CSSRule
    {
        public new readonly ECssRuleType type = ECssRuleType.STYLE_RULE;
        public CssSelector Selector;
        public readonly CSSStyleDeclaration style;


        protected override string Serialize()
        {
            throw new NotImplementedException();
        }
    }
}
