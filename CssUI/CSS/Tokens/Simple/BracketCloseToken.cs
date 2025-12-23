
using CssUI.CSS.Parser;

namespace CssUI.CSS;

/// <summary>
/// Represents '}'
/// </summary>
public sealed class BracketCloseToken : CssToken
{
    public static BracketCloseToken Instance = new BracketCloseToken();
    public BracketCloseToken() : base(ECssTokenType.Bracket_Close)
    {
    }

    public override string Encode() => "}";
}

