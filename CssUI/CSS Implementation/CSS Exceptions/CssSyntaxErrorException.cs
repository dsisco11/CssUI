using CssUI.CSS.Serialization;
using System.Linq;

namespace CssUI
{
    public class CssSyntaxErrorException : CssException
    {
        public CssSyntaxErrorException(string Message) : base("Syntax", Message)
        {
        }
        public CssSyntaxErrorException(string Message, TokenStream Stream) : base("Syntax", $"{Message} @: \"{ParserCommon.Get_Location(Stream)}\"")
        {
        }

        public CssSyntaxErrorException(params object[] Args) : base("Syntax", Args)
        {
        }
    }
}
