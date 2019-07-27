namespace CssUI.CSS.Internal
{
    [System.AttributeUsage(System.AttributeTargets.Field)]
    public class CssKeywordAttribute : System.Attribute
    {
        #region Properties
        public readonly string Keyword;
        public readonly dynamic[] Values = null;
        #endregion

        #region Constructor
        [Obsolete("Use CssUI.Internal.EnumKeyword")]
        public CssKeywordAttribute(string Keyword, params dynamic[] Values)
        {
            this.Keyword = Keyword;
            this.Values = Values;
        }
        #endregion
    }
}
