
using System.Collections.Generic;

namespace CssUI.CSS
{
    public sealed class CssTokenStream : ObjectStream<CssToken>
    {
        #region Constructors
        public CssTokenStream(CssToken[] Items) : base(Items, CssToken.EOF)
        {
        }
        public CssTokenStream(IEnumerable<CssToken> Items) : base(Items, CssToken.EOF)
        {
        }
        #endregion
    }
}
