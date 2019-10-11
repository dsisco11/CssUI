using CssUI.CSS.Internal;
using System;

namespace CssUI.CSS
{
    public sealed class CSSImportRule : CSSRule
    {
        public new readonly ECssRuleType type = ECssRuleType.IMPORT_RULE;
        public readonly string href;
        public readonly CSSStyleSheet stylesheet;
        // public MediaList media { get { return stylesheet.media; } }


        public CSSImportRule(string href, CSSStyleSheet stylesheet)
        {
            this.href = href;
            this.stylesheet = stylesheet;
        }

        protected override string Serialize()
        {
            throw new NotImplementedException();
        }
    }
}
