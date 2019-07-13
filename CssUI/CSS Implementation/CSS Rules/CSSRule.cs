using System;
using CssUI.CSS.Internal;

namespace CssUI.CSS
{
    /// <summary>
    /// Defines a generic rule for CSS, this could be anything from an @media rule to the familiar Style rule.
    /// These values held by this class and all its subtypes are UNINTERPRETED and essentially just hold and organize the rules as text
    /// </summary>
    public abstract class CSSRule
    {
        public readonly ECssRuleType type;
        public string cssText { get { return this.Serialize(); } }
        public readonly CSSRule parentRule;
        public readonly CSSStyleSheet parentStyleSheet;

        protected abstract string Serialize();

        public static CSSRule From_String(string rule)
        {
            throw new NotImplementedException();
        }
    }
}
