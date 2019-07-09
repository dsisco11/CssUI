namespace CssUI.DOM.Internal
{
    [System.AttributeUsage(System.AttributeTargets.Field)]
    public class DomKeywordAttribute : System.Attribute
    {
        public string Keyword;
        public DomKeywordAttribute(string Keyword)
        {
            this.Keyword = Keyword;
        }
    }
}
