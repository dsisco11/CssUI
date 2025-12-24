namespace CssUI.CSS.Parser;

public sealed class IdentToken : ValuedTokenBase
{
    /// <summary>
    /// Creates an IdentToken with optional case preservation.
    /// </summary>
    /// <param name="Value">The identifier value.</param>
    /// <param name="preserveCase">If true, preserves original case (for case-sensitive contexts like custom properties).</param>
    public IdentToken(string Value, bool preserveCase = false) : base(ECssTokenType.Ident, Value, AutoLowercase: !preserveCase)
    {
    }

    /// <summary>
    /// Indicates whether this identifier starts with '--' (a custom property/dashed-ident).
    /// </summary>
    public bool IsDashedIdent => Value?.StartsWith("--", System.StringComparison.Ordinal) ?? false;

    public override string? Encode()
    {
        return Value;
    }
}

