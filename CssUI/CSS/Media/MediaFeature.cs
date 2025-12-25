using System;
using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;
using CssUI.CSS.Internal;
using CssUI.DOM;

namespace CssUI.CSS.Media;

/// <summary>
/// Represents a media feature condition in a media query.
/// </summary>
/// <remarks>
/// Media features describe specific characteristics of the user agent, output device, or environment.
/// They can be used in boolean context (e.g., <c>(color)</c>) or range context (e.g., <c>(width > 500px)</c>).
/// </remarks>
/// <seealso href="https://www.w3.org/TR/mediaqueries-4/#mq-features"/>
public class MediaFeature : IMediaCondition
{/* Docs: https://www.w3.org/TR/mediaqueries-4/#mq-features */
    #region Properties
    private readonly EMediaCombinator Combinator = EMediaCombinator.None;
    private readonly EMediaFeatureContext Context;

    private readonly CssValue[]? Values;
    private readonly EMediaOperator[]? Operators;
    private bool IsValid = false;
    // private bool IsNegated = false;
    #endregion

    #region Constructors
    /// <summary>
    /// Creates a new 'boolean' type media feature
    /// </summary>
    /// <param name="name"></param>
    public MediaFeature(EMediaFeatureName Name)
    {
        Context = EMediaFeatureContext.Boolean;

        MediaDefinition? def = MediaDefinition.Lookup(Name);
        if (def != null)
        {
            IsValid = true;
            /* if an allowed type is keywords and 'none' is an option then we match it against that keyword */
            if (0 != (def.AllowedTypes & ECssValueTypes.KEYWORD) && def.KeywordWhitelist.Contains("none"))
            {
                Values = new CssValue[] { CssValue.From(Name), CssValue.None };
            }
            else if (0 != (def.AllowedTypes & ECssValueTypes.INTEGER))
            {
                Values = new CssValue[] { CssValue.From(Name), CssValue.From(0) };
            }
            else if (0 != (def.AllowedTypes & ECssValueTypes.DIMENSION))
            {
                Values = new CssValue[] { CssValue.From(Name), CssValue.From(0, ECssUnit.PX) };
            }
            else if (0 != (def.AllowedTypes & ECssValueTypes.RESOLUTION))
            {
                Values = new CssValue[] { CssValue.From(Name), CssValue.From(0, ECssUnit.DPPX) };
            }
        }
        else
        {
            /* This isnt a supported feature, therefore it should always evaluate to false */
            IsValid = false;
            Values = Array.Empty<CssValue>();
        }

        /* For sanity just catch the case where we dont have values to compare here */
        if (Values == null || Values.Length <= 0)
        {
            IsValid = false;
        }
    }

    /// <summary>
    /// Creates a new 'range' type media feature
    /// </summary>
    /// <param name="name"></param>
    /// <param name="comparator"></param>
    /// <param name="value"></param>
    public MediaFeature(CssValue[] values, EMediaOperator[] ops)
    {
        Context = EMediaFeatureContext.Range;
        Values = values;
        Operators = ops;
        IsValid = true;
    }
    #endregion


    #region Matching
    public bool Matches(Document document)
    {
        ArgumentNullException.ThrowIfNull(document);
        Contract.EndContractBlock();

        if (!IsValid)
        {
            return false;
        }

        bool matched = Try_Match(document);
        switch (Combinator)
        {
            case EMediaCombinator.NOT:
                {
                    return !matched;
                }
            default:
                {
                    return matched;
                }
        }
    }

    private bool Try_Match(Document document)
    {
        switch (Context)
        {
            case EMediaFeatureContext.Boolean:
                {
                    /*
                     * ...the media feature is evaluated in a boolean context.
                     * If the feature would be true for any value other than the number 0, a <dimension> with the value 0, or the keyword none, the media feature evaluates to true.
                     * Otherwise, it evaluates to false.
                     */
                    CssValue value = Values[0];
                    if (!EMediaFeatureNameExtensions.TryFromKeyword(value.AsString(), out EMediaFeatureName nameLookup))
                    {/* This feature is not supported (or maybe valid) so we need to treat it as if it simply doesnt match */
                        IsValid = false;
                        return false;
                    }

                    // CssValue valueA = Values[0];
                    CssValue valueB = Values[1];

                    switch (valueB.Type)
                    {
                        case ECssValueTypes.KEYWORD:
                            {
                                return Compare_Keyword(document, nameLookup, valueB.AsString());
                            }
                        case ECssValueTypes.INTEGER:
                        case ECssValueTypes.NUMBER:
                            {
                                double A = (double)Resolve_Media_Name_Value(document, nameLookup);
                                double B = valueB.AsDecimal();
                                return !(A == B);
                            }
                        case ECssValueTypes.DIMENSION:
                        case ECssValueTypes.RESOLUTION:
                            {
                                double A = (double)Resolve_Media_Name_Value(document, nameLookup);
                                double B = valueB.Resolve(document.cssUnitResolver);
                                return !(A == B);
                            }
                    }
                }
                break;
            case EMediaFeatureContext.Range:
                {
                    /* Compare all value/operator pairs  */
                    for (int i = 1; i < Operators.Length; i++)
                    {
                        CssValue A = Values[i - 1];
                        CssValue B = Values[i];
                        var op = Operators[i - 1];

                        if (!Compare(document, A, B, op))
                        {
                            return false;
                        }
                    }

                    return true;
                }
        }

        return false;
    }


    bool Compare_Keyword(Document document, EMediaFeatureName Name, string keyword)
    {
        switch (Name)
        {
            case EMediaFeatureName.Orientation:
                {
                    if (EMediaOrientationExtensions.TryFromKeyword(keyword, out EMediaOrientation outOrientation))
                    {
                        double width = (double)Resolve_Media_Name_Value(document, EMediaFeatureName.Width);
                        double height = (double)Resolve_Media_Name_Value(document, EMediaFeatureName.Height);

                        return (outOrientation == EMediaOrientation.Landscape) ? width >= height : height >= width;
                    }
                }
                break;
            case EMediaFeatureName.Update:
                {
                    if (EMediaUpdateExtensions.TryFromKeyword(keyword, out EMediaUpdate outUpdateRate))
                    {
                        double refreshRate = (double)Resolve_Media_Name_Value(document, Name);
                        switch (outUpdateRate)
                        {
                            case EMediaUpdate.None:
                                {
                                    return (0 == refreshRate);
                                }
                            case EMediaUpdate.Slow:
                                {
                                    return refreshRate < 20;
                                }
                            case EMediaUpdate.Fast:
                                {
                                    return refreshRate > 20;
                                }
                        }
                    }
                }
                break;
            case EMediaFeatureName.Color:
            case EMediaFeatureName.Min_Color:
            case EMediaFeatureName.Max_Color:
                {
                    return (ulong)Resolve_Media_Name_Value(document, Name) > 0;
                }
            case EMediaFeatureName.Monochrome:
            case EMediaFeatureName.Min_Monochrome:
            case EMediaFeatureName.Max_Monochrome:
                {
                    return (ulong)Resolve_Media_Name_Value(document, Name) > 0;
                }
            case EMediaFeatureName.ColorIndex:
            case EMediaFeatureName.Min_ColorIndex:
            case EMediaFeatureName.Max_ColorIndex:
                {
                    return (ulong)Resolve_Media_Name_Value(document, Name) > 0;
                }
            case EMediaFeatureName.Grid:
                {
                    return (ulong)Resolve_Media_Name_Value(document, Name) > 0;
                }
            case EMediaFeatureName.Width:
            case EMediaFeatureName.Min_Width:
            case EMediaFeatureName.Max_Width:
                {
                    return (long)Resolve_Media_Name_Value(document, Name) > 0;
                }
            case EMediaFeatureName.Height:
            case EMediaFeatureName.Min_Height:
            case EMediaFeatureName.Max_Height:
                {
                    return (long)Resolve_Media_Name_Value(document, Name) > 0;
                }
            case EMediaFeatureName.AspectRatio:
            case EMediaFeatureName.Min_AspectRatio:
            case EMediaFeatureName.Max_AspectRatio:
                {
                    return (double)Resolve_Media_Name_Value(document, Name) > 0;
                }
        }

        return false;
    }

    static bool Compare(Document document, CssValue ValueA, CssValue ValueB, EMediaOperator Op)
    {

        /* An Excerpt:
         * "If a media feature references a concept which does not exist on the device where the UA is running (for example, speech UAs do not have a concept of “width”), the media feature must always evaluate to false."
         */
        double A, B;
        /* First resolve these values to numbers we can compare */

        if (ValueA.Type == ECssValueTypes.KEYWORD)
        {
            if (!EMediaFeatureNameExtensions.TryFromKeyword(ValueA.AsString(), out EMediaFeatureName _))
            {/* If we cant find this keyword then we treat it as if it were an unsupported feature */
                return false;
            }

            A = (double)Resolve_Media_Name_Value(document, ValueA.AsEnum<EMediaFeatureName>());
        }
        else
        {
            A = ValueA.AsDecimal();
        }


        if (ValueB.Type == ECssValueTypes.KEYWORD)
        {
            if (!EMediaFeatureNameExtensions.TryFromKeyword(ValueB.AsString(), out EMediaFeatureName _))
            {/* If we cant find this keyword then we treat it as if it were an unsupported feature */
                return false;
            }

            B = (double)Resolve_Media_Name_Value(document, ValueB.AsEnum<EMediaFeatureName>());
        }
        else
        {
            B = ValueB.AsDecimal();
        }


        /* Compare them and return the result */
        switch (Op)
        {
            case EMediaOperator.EqualTo:
                {
                    return (A == B);
                }
            case EMediaOperator.LessThan:
                {
                    return (A < B);
                }
            case EMediaOperator.GreaterThan:
                {
                    return (A > B);
                }
            case EMediaOperator.LessThanEq:
                {
                    return (A < B || (A == B));
                }
            case EMediaOperator.GreaterThanEq:
                {
                    return (A > B || (A == B));
                }
        }

        return false;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    static object Resolve_Media_Name_Value(Document document, EMediaFeatureName Name)
    {
        Screen screen = document.defaultView.screen;
        switch (Name)
        {
            case EMediaFeatureName.AspectRatio:
            case EMediaFeatureName.Min_AspectRatio:
            case EMediaFeatureName.Max_AspectRatio:
                {
                    return screen.width / (double)screen.height;
                }
            case EMediaFeatureName.Width:
            case EMediaFeatureName.Min_Width:
            case EMediaFeatureName.Max_Width:
                {
                    return screen.width;
                }
            case EMediaFeatureName.Height:
            case EMediaFeatureName.Min_Height:
            case EMediaFeatureName.Max_Height:
                {
                    return screen.height;
                }
            case EMediaFeatureName.Color:
            case EMediaFeatureName.Min_Color:
            case EMediaFeatureName.Max_Color:
                {
                    return screen.colorDepth;
                }
            case EMediaFeatureName.Monochrome:
            case EMediaFeatureName.Min_Monochrome:
            case EMediaFeatureName.Max_Monochrome:
                {
                    return screen.monochromeDepth;
                }
            case EMediaFeatureName.ColorIndex:
            case EMediaFeatureName.Min_ColorIndex:
            case EMediaFeatureName.Max_ColorIndex:
                {
                    return screen.colorIndex;
                }
            case EMediaFeatureName.Grid:
                {
                    return screen.isGrid;
                }
            case EMediaFeatureName.Update:
                {
                    return screen.refreshRate;
                }
            default:
                {
                    throw new NotImplementedException($"Media feature \"{Enum.GetName(typeof(EMediaFeatureName), Name)}\" is not implemented");
                }
        }
    }

    #endregion

    #region Formatting (ISpanFormattable)

    /// <inheritdoc/>
    public override string ToString() => ToString(null, null);

    /// <inheritdoc/>
    public string ToString(string? format, IFormatProvider? formatProvider)
    {
        Span<char> buffer = stackalloc char[256];
        if (TryFormat(buffer, out int charsWritten, format.AsSpan(), formatProvider))
        {
            return buffer[..charsWritten].ToString();
        }
        return string.Empty;
    }

    /// <summary>
    /// Tries to format the media feature into the provided span.
    /// </summary>
    /// <param name="destination">The span to write to.</param>
    /// <param name="charsWritten">The number of characters written.</param>
    /// <param name="format">The format string (ignored).</param>
    /// <param name="provider">The format provider (ignored).</param>
    /// <returns><c>true</c> if formatting succeeded; otherwise, <c>false</c>.</returns>
    /// <remarks>
    /// Serialization follows CSS Media Queries Level 4 §4.
    /// Format: (name) for boolean, (name: value) for plain range, or (value op name op value) for range.
    /// </remarks>
    /// <seealso href="https://www.w3.org/TR/mediaqueries-4/#mq-features"/>
    public bool TryFormat(Span<char> destination, out int charsWritten, ReadOnlySpan<char> format = default, IFormatProvider? provider = null)
    {
        charsWritten = 0;

        if (!IsValid)
        {
            return true; // Empty string is valid for invalid features
        }

        string featureName = Values![0].AsEnum<EMediaFeatureName>().Keyword();

        int pos = 0;

        // 1) Append opening parenthesis
        if (destination.Length < 1) return false;
        destination[pos++] = UnicodeCommon.CHAR_LEFT_PARENTHESES;

        if (Context == EMediaFeatureContext.Boolean)
        {
            // Boolean context: (name)
            if (destination.Length < pos + featureName.Length) return false;
            featureName.AsSpan().CopyTo(destination[pos..]);
            pos += featureName.Length;
        }
        else if (Context == EMediaFeatureContext.Range)
        {
            // Append feature name
            if (destination.Length < pos + featureName.Length) return false;
            featureName.AsSpan().CopyTo(destination[pos..]);
            pos += featureName.Length;

            if (Operators!.Length == 0 && Values.Length == 2)
            {
                // Plain range: (name: value)
                if (destination.Length < pos + 2) return false;
                destination[pos++] = UnicodeCommon.CHAR_COLON;
                destination[pos++] = UnicodeCommon.CHAR_SPACE;

                if (!Values[1].TryFormat(destination[pos..], out int valueWritten, default, provider))
                    return false;
                pos += valueWritten;
            }
            else
            {
                // Range with operators: serialize all value/operator pairs
                for (int i = 1; i < Operators.Length; i++)
                {
                    CssValue value = Values[i];
                    EMediaOperator op = Operators[i - 1];

                    string comparator = op.Keyword();

                    if (!value.TryFormat(destination[pos..], out int valueWritten, default, provider))
                        return false;
                    pos += valueWritten;

                    if (destination.Length < pos + comparator.Length) return false;
                    comparator.AsSpan().CopyTo(destination[pos..]);
                    pos += comparator.Length;
                }
            }
        }

        // 3) Append closing parenthesis
        if (destination.Length <= pos) return false;
        destination[pos++] = UnicodeCommon.CHAR_RIGHT_PARENTHESES;

        charsWritten = pos;
        return true;
    }

    #endregion




}

