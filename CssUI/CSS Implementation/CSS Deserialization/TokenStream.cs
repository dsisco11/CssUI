using CssUI.CSS.Parser;
using System.Collections.Generic;

namespace CssUI.CSS.Serialization
{
    public sealed class TokenStream : ObjectStream<CssToken>
    {
        #region Constructors
        public TokenStream(CssToken[] Items) : base(Items, CssToken.EOF)
        {
        }
        public TokenStream(IEnumerable<CssToken> Items) : base(Items, CssToken.EOF)
        {
        }
        #endregion
    }
}
