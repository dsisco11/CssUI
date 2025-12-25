using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using CssUI.CSS.Parser;
using CssUI.DOM;
using CssUI.DOM.Nodes;

namespace CssUI.CSS.Selectors;


public class PseudoClassSelectorFunction : PseudoClassSelector
{
    protected readonly List<CssToken> Args;

    public PseudoClassSelectorFunction(string Name, List<CssToken>? Args = null) : base(Name)
    {
        this.Args = Args;
    }

    /// <summary>
    /// Returns whether the selector matches a specified element or index
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    override public bool Matches(Element E, params Node[] scopeElements)
    {
        switch (Name)
        {
            case "drop":
                return PseudoClassFunctions.Drop(E, Args);
            default:
                throw new CssSelectorException("[CSS] Selector pseudo-class (", Name, ") logic not implemented!");
        }
    }

    #region Formatting
    /// <inheritdoc/>
    public override bool TryFormat(Span<char> destination, out int charsWritten, ReadOnlySpan<char> format, IFormatProvider? provider)
    {
        // Per CSSOM §5.2: functional pseudo-class serializes as ":name(...)"
        charsWritten = 0;

        if (destination.Length < 1)
            return false;
        destination[0] = ':';
        charsWritten = 1;

        if (!Name.AsSpan().TryCopyTo(destination[charsWritten..]))
            return false;
        charsWritten += Name.Length;

        if (destination.Length <= charsWritten)
            return false;
        destination[charsWritten] = '(';
        charsWritten++;

        // Serialize arguments
        if (Args is not null)
        {
            for (int i = 0; i < Args.Count; i++)
            {
                var arg = Args[i];
                var tokenStr = arg.ToString();
                if (!tokenStr.AsSpan().TryCopyTo(destination[charsWritten..]))
                    return false;
                charsWritten += tokenStr.Length;
            }
        }

        if (destination.Length <= charsWritten)
            return false;
        destination[charsWritten] = ')';
        charsWritten++;

        return true;
    }
    #endregion
}

public class PseudoClassSelectorAnBFunction : PseudoClassSelector
{
    protected readonly CssAnBMatcher AnB;
    protected readonly IEnumerable<ComplexSelector>? Selectors;

    public PseudoClassSelectorAnBFunction(string Name, DataConsumer<CssToken> Stream) : base(Name)
    {
        AnB = CssAnBMatcher.Consume(Stream);
        if (Stream.Next.Type == ECssTokenType.Ident && (Stream.Next as IdentToken).Value.Equals("or"))
        {
            Stream.Consume();// Consume the 'or' string token
            Selectors = SelectorParser.Consume_Selector_List(Stream);
        }
    }

    /// <summary>
    /// Returns whether the selector matches a specified element or index
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    override public bool Matches(Element E, params Node[] scopeElements)
    {
        switch (Name)
        {
            case "nth-child":
                {// SEE: https://www.w3.org/TR/2011/REC-css3-selectors-20110929/#nth-child-pseudo
                    // TODO: IMPLEMENT THIS!

                    return false;
                }
            default:
                throw new CssSelectorException("Selector pseudo-class function (", Name, ") logic not implemented!");
        }
    }

    #region Formatting
    /// <inheritdoc/>
    public override bool TryFormat(Span<char> destination, out int charsWritten, ReadOnlySpan<char> format, IFormatProvider? provider)
    {
        // Per CSSOM §5.2: :nth-child() etc serialize using An+B serialization
        charsWritten = 0;

        if (destination.Length < 1)
            return false;
        destination[0] = ':';
        charsWritten = 1;

        if (!Name.AsSpan().TryCopyTo(destination[charsWritten..]))
            return false;
        charsWritten += Name.Length;

        if (destination.Length <= charsWritten)
            return false;
        destination[charsWritten] = '(';
        charsWritten++;

        // Serialize An+B value (An+B format)
        string anbStr;
        if (AnB.A == 0)
            anbStr = AnB.B.ToString();
        else if (AnB.B == 0)
            anbStr = AnB.A == 1 ? "n" : AnB.A == -1 ? "-n" : $"{AnB.A}n";
        else
            anbStr = AnB.A == 1 ? $"n{(AnB.B >= 0 ? "+" : "")}{AnB.B}" :
                     AnB.A == -1 ? $"-n{(AnB.B >= 0 ? "+" : "")}{AnB.B}" :
                     $"{AnB.A}n{(AnB.B >= 0 ? "+" : "")}{AnB.B}";

        if (!anbStr.AsSpan().TryCopyTo(destination[charsWritten..]))
            return false;
        charsWritten += anbStr.Length;

        // @todo: Serialize " of <selector-list>" if Selectors is not null

        if (destination.Length <= charsWritten)
            return false;
        destination[charsWritten] = ')';
        charsWritten++;

        return true;
    }
    #endregion
}

public class PseudoClassSelectorNegationFunction : PseudoClassSelector
{
    protected readonly ComplexSelector Selector;

    public PseudoClassSelectorNegationFunction(string Name, DataConsumer<CssToken> Stream) : base(Name)
    {
        Selector = SelectorParser.Consume_Single_Selector(Stream);
    }

    /// <summary>
    /// Returns whether the selector matches a specified element or index
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    override public bool Matches(Element E, params Node[] scopeElements)
    {
        return false == Selector.Match(E, scopeElements);
    }

    #region Formatting
    /// <inheritdoc/>
    public override bool TryFormat(Span<char> destination, out int charsWritten, ReadOnlySpan<char> format, IFormatProvider? provider)
    {
        // Per CSSOM §5.2: :not() serializes as ":not(" + serialized selector list + ")"
        charsWritten = 0;

        if (destination.Length < 1)
            return false;
        destination[0] = ':';
        charsWritten = 1;

        if (!Name.AsSpan().TryCopyTo(destination[charsWritten..]))
            return false;
        charsWritten += Name.Length;

        if (destination.Length <= charsWritten)
            return false;
        destination[charsWritten] = '(';
        charsWritten++;

        // Serialize the selector
        if (!Selector.TryFormat(destination[charsWritten..], out int selectorWritten, format, provider))
            return false;
        charsWritten += selectorWritten;

        if (destination.Length <= charsWritten)
            return false;
        destination[charsWritten] = ')';
        charsWritten++;

        return true;
    }
    #endregion
}

static class PseudoClassFunctions
{
    /// <summary>
    /// Performs matching against the 'drop' function
    /// </summary>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool Drop(Element E, List<CssToken> Args)
    {
        return false;
        /*if (Args == null || Args.Count <= 0)
        {// :drop
            return E.hasAttribute(EAttributeName.Dropzone);
        }
        else
        {// drop(...)
            if (!E.hasAttribute(EAttributeName.Dropzone)) return false;

            // TODO: test the ":drop(active|valid|invalid) pseudo-class
            foreach (CssToken token in Args)
            {
                if (token.Type != ECssTokenType.Ident) throw new CssSyntaxErrorException("Expected Ident token!");
                IdentToken tok = Args[0] as IdentToken;
                switch (tok.Value.ToLower())
                {
                    case "active":
                        if (!E.IsDropTarget) return false;
                        break;
                    case "valid":// Matches if the drop-target CAN accept the type of object being dragged
                        if (!E.Accepts_Current_DragItem()) return false;
                        break;
                    case "invalid":// Matches if the drop-target CAN'T accept the type of object being dragged
                        if (E.Accepts_Current_DragItem()) return false;
                        break;
                }
            }

            return true;
        }*/
    }
}

