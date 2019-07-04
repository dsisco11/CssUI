namespace CssUI.Internal
{
    [System.AttributeUsage(System.AttributeTargets.Field)]
    public class CssKeywordAttribute : System.Attribute
    {
        public readonly string Keyword;

        public CssKeywordAttribute(string Keyword)
        {
            this.Keyword = Keyword;
        }
    }
}
