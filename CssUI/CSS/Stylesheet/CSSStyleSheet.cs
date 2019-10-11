
namespace CssUI.CSS
{
    public class CSSStyleSheet : Stylesheet
    {
        public readonly CSSRule ownerRule;
        public readonly CSSRuleList cssRules = new CSSRuleList();

        public CSSStyleSheet(CSSRule ownerRule)
        {
            this.ownerRule = ownerRule;
        }

        public int insertRule(string rule, int index)
        {
            return cssRules.InsertRule(index, rule);
        }

        public void deleteRule(int index)
        {
            cssRules.RemoveRule(index);
        }
    }
}
