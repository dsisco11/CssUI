using System;

namespace CssUI.CSS
{
    /// <summary>
    /// A CSS rule that holds other rules within it, things like @media and @page rules inherit from this
    /// </summary>
    public abstract class CSSGroupingRule : CSSRule
    {
        public readonly CSSRuleList cssRules = new CSSRuleList();

        public int insertRule(string rule, int index) { return cssRules.InsertRule(index, rule); }
        public void deleteRule(int index) => cssRules.RemoveRule(index);

        protected override string Serialize()
        {
            throw new NotImplementedException();
        }
    }
}
