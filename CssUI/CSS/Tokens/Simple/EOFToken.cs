
using CssUI.CSS.Parser;

namespace CssUI.CSS
{
    public class EOFToken : CssToken
    {
        public static EOFToken Instance = new EOFToken();
        public EOFToken() : base (ECssTokenType.EOF)
        {
        }

        public override string Encode()
        {
            //return new string(new char[] { CssTokenizer.TOKEN_EOF });
            return string.Empty;
        }
    }
}
