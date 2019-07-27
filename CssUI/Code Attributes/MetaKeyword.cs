namespace CssUI.Internal
{
    [System.AttributeUsage(System.AttributeTargets.Field)]
    public class MetaKeywordAttribute : System.Attribute
    {
        #region Properties
        public readonly string Keyword;
        public readonly object[] Values = null;
        #endregion

        #region Constructor
        public MetaKeywordAttribute(string Keyword, params object[] Values)
        {
            this.Keyword = Keyword;
            this.Values = Values;
        }
        #endregion
    }
}
