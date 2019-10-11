using System;
using CssUI.CSS.Parser;

namespace CssUI.CSS.Serialization
{
    public sealed class TokenStream : DataConsumer<CssToken>
    {
        #region Constructors
        public TokenStream(CssToken[] Items) : base(Items, CssToken.EOF)
        {
        }

        public TokenStream(ReadOnlyMemory<CssToken> Items) : base(Items, CssToken.EOF)
        {
        }
        #endregion
    }
}
