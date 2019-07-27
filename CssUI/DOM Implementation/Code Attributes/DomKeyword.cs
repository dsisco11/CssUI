using System;

namespace CssUI.DOM.Internal
{
    [System.AttributeUsage(System.AttributeTargets.Field)]
    public class DomKeywordAttribute : System.Attribute
    {
        #region Properties
        public readonly string Keyword;
        public readonly dynamic[] Values = null;
        #endregion

        #region Constructor
        [Obsolete("Use CssUI.Internal.EnumKeyword")]
        public DomKeywordAttribute(string Keyword, params object[] Values)
        {
            this.Keyword = Keyword;
            this.Values = Values;
        }
        #endregion
    }
}
