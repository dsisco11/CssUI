using System;
using CssUI.CSS.Internal;

namespace CssUI.CSS
{
    public sealed class CSSMediaRule : CSSGroupingRule
    {
        public new readonly ECssRuleType type = ECssRuleType.MEDIA_RULE;
        public readonly MedialList media = new MedialList();

        public CSSMediaRule()
        {
        }

        protected override string Serialize()
        {
            throw new NotImplementedException();
        }
    }
}
