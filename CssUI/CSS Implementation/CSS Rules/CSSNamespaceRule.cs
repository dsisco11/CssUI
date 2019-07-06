using CssUI.Enums;
using System;

namespace CssUI.CSS
{
    public sealed class CSSNamespaceRule : CSSRule
    {
        public new readonly ECssRuleType type = ECssRuleType.NAMESPACE_RULE;
        public readonly string namespaceURI;
        public readonly string prefix;

        public CSSNamespaceRule(string namespaceURI, string prefix)
        {
            this.namespaceURI = namespaceURI;
            this.prefix = prefix;
        }

        protected override string Serialize()
        {
            throw new NotImplementedException();
        }
    }
}
