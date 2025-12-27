using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;
using System.Diagnostics.Contracts;
using System.Globalization;
using System.Runtime.CompilerServices;
using CssUI.CSS.Internal;
using CssUI.CSS.Parser;
using CssUI.CSS.Serialization;
using CssUI.Rendering;

namespace CssUI.CSS;

/// <summary>
/// Represents a CSS Value
/// </summary>
public partial record class CssValue : ISpanFormattable, IFormattable, IParsable<CssValue>
{
    #region Delegates
    public delegate double StyleUnitResolverDelegate(ECssUnit unit);
    #endregion

    #region Static Declerations
    /// <summary>
    /// Value depends on other properties
    /// </summary>
    public static readonly CssValue Auto = new CssValue(ECssValueTypes.AUTO);
    /// <summary>
    /// (Non-Cascadeable)
    /// Treat as nothing. Eg; The stylesheet didnt specify anything for this property.
    /// </summary>
    public static readonly CssValue Null = new CssValue(ECssValueTypes.NULL);
    /// <summary>
    /// (Non-Cascadeable)
    /// Intentionally unsets a property, forcing it to resolve to either its inherited or initial value.
    /// DOCS: https://www.w3.org/TR/css-cascade-3/#inherit-initial
    /// </summary>
    public static readonly CssValue Unset = new CssValue(ECssValueTypes.UNSET);
    /// <summary>
    /// Value should resolve to it's definitions default value
    /// </summary>
    public static readonly CssValue Initial = new CssValue(ECssValueTypes.INITIAL);
    /// <summary>
    /// Value is inherited from the parent element
    /// </summary>
    public static readonly CssValue Inherit = new CssValue(ECssValueTypes.INHERIT);
    /// <summary>
    /// (Cascades)
    /// Value is purposly nothing, No value is assigned, some properties use this state to be ignored.
    /// As in they wont have an effect on the elements styling or block
    /// </summary>
    public static readonly CssValue None = new CssValue(ECssValueTypes.NONE);
    /// <summary>
    /// integer 0
    /// </summary>
    public static readonly CssValue Zero = From(0);
    /// <summary>
    /// 50%
    /// </summary>
    public static readonly CssValue Percent_50 = From_Percent(50d);
    /// <summary>
    /// 100%
    /// </summary>
    public static readonly CssValue Percent_100 = From_Percent(100d);
    #endregion

    #region Properties
    private readonly ECssValueFlags flags = ECssValueFlags.None;
    private readonly ECssValueTypes type = ECssValueTypes.NULL;
    private readonly ECssUnit unit = ECssUnit.None;
    #endregion

    #region Accessors
    public ECssValueFlags Flags => flags;
    public ECssValueTypes Type => type;
    /// <summary>
    /// Gets the CSS unit for this value. Subclasses for dimension/resolution types override this.
    /// </summary>
    public virtual ECssUnit Unit => unit;

    /// <summary>
    /// Returns whether the value is 'Auto'
    /// </summary>
    public bool IsAuto => (Type == ECssValueTypes.AUTO);

    /// <summary>
    /// Returns whether the value is a definite Number or Integer
    /// </summary>
    public bool IsDefinite => (0 != (Type & (ECssValueTypes.INTEGER | ECssValueTypes.NUMBER)));

    /// <summary>
    /// Returns whether the value is a collection of sub-values
    /// </summary>
    public bool IsCollection => (Type == ECssValueTypes.COLLECTION);

    /// <summary>
    /// Returns whether the value type is <see cref="ECssValueTypes.NULL"/>
    /// </summary>
    public bool IsNull => (Type == ECssValueTypes.NULL);

    /// <summary>
    /// Returns whether there is actually a set value
    /// </summary>
    public virtual bool HasValue
    {
        get
        {
            // Singleton types (NULL, AUTO, INHERIT, etc.) have no data payload
            // Subclasses override to return true when they have actual values
            return Type switch
            {
                ECssValueTypes.NULL or
                ECssValueTypes.AUTO or
                ECssValueTypes.INHERIT or
                ECssValueTypes.INITIAL or
                ECssValueTypes.UNSET or
                ECssValueTypes.NONE or
                ECssValueTypes.DEFAULT => false,
                _ => true
            };
        }
    }
    #endregion

    #region Constructors
    /// <summary>
    /// Creates a CssValue from a CssFunction.
    /// </summary>
    /// <remarks>
    /// This factory method returns a <see cref="CssFunctionValue"/> which stores the function directly
    /// without boxing. Use this instead of the constructor for creating function values.
    /// </remarks>
    internal static CssValue From(CssFunction function) => new CssFunctionValue(function);

    /// <summary>
    /// Base constructor for creating a CssValue with only a type.
    /// </summary>
    /// <param name="type">The CSS value type.</param>
    /// <remarks>
    /// This constructor is <c>private protected</c> to prevent external assemblies from creating
    /// custom <see cref="CssValue"/> subclasses. All value creation should go through factory methods.
    /// </remarks>
    private protected CssValue(ECssValueTypes type)
    {
        this.type = type;
        flags |= Get_Inherent_Value_Type_Flags(type);
    }

    /// <summary>
    /// Attempts to parse a CSS global keyword string to its corresponding ECssValueTypes.
    /// This handles keywords like "inherit", "initial", "unset", "auto", "none", "default".
    /// </summary>
    private static bool TryParseCssGlobalKeyword(string keyword, out ECssValueTypes result)
    {
        result = keyword.ToLowerInvariant() switch
        {
            "inherit" => ECssValueTypes.INHERIT,
            "initial" => ECssValueTypes.INITIAL,
            "unset" => ECssValueTypes.UNSET,
            "auto" => ECssValueTypes.AUTO,
            "none" => ECssValueTypes.NONE,
            "default" => ECssValueTypes.DEFAULT,
            _ => ECssValueTypes.NULL
        };
        return result != ECssValueTypes.NULL;
    }
    #endregion

    private static ECssValueFlags Get_Inherent_Value_Type_Flags(ECssValueTypes Type)
    {
        ECssValueFlags Flags = ECssValueFlags.None;

        switch (Type)
        {
            // NOTE: Auto values get calculated in vastly different ways, sometimes this doesn't even indicate that a value depends on the value of other properties,
            //so it should NOT get the 'Depends' flag
            case ECssValueTypes.INHERIT:// Inherited values function like redirects which compute to the current value of the matching property for the owning element's parent
            case ECssValueTypes.PERCENT:// Percentage values represent a percentage of another property's value
                {
                    Flags |= ECssValueFlags.Depends;
                    break;
                }
            case ECssValueTypes.NULL:
            case ECssValueTypes.NONE:
            case ECssValueTypes.INITIAL:
            case ECssValueTypes.INTEGER:
            case ECssValueTypes.NUMBER:
            case ECssValueTypes.STRING:
            case ECssValueTypes.KEYWORD:
            case ECssValueTypes.COLOR:
            case ECssValueTypes.COLOR_HDR:
            case ECssValueTypes.IMAGE:
            case ECssValueTypes.URL:
            case ECssValueTypes.UNICODE_RANGE:// Unicode ranges are absolute values
            case ECssValueTypes.POSITION:// The position has already been resolved here.
            case ECssValueTypes.FUNCTION:// The function args have already been resolved here.
                {
                    Flags |= ECssValueFlags.Absolute;
                    break;
                }
            case ECssValueTypes.UNSET:
            case ECssValueTypes.AUTO:
            case ECssValueTypes.DIMENSION:
            case ECssValueTypes.RATIO:
            case ECssValueTypes.RESOLUTION:
            case ECssValueTypes.CALC:// calc() expressions may contain percentages that need resolution
            case ECssValueTypes.VAR:// var() references need to be resolved by looking up custom properties
            case ECssValueTypes.ENV:// env() references need to be resolved by looking up environment variables
                {
                    /* XXX:
                     * These values when used on properties CAN be dependant but arent always so idk maybe its best to leave them as absolute?
                     * Maybe we can come up with some sort of intermediate state that causes them to resolve their flag to absolute/dependent AFTER they get assigned to a property?
                     */
                    Flags |= ECssValueFlags.Absolute;
                    break;
                }
            case ECssValueTypes.COLLECTION:// A collections flags are determined by the subclass (CssListValue)
                {
                    // Collections calculate their flags during construction in subclass
                    Flags |= ECssValueFlags.Absolute;
                    break;
                }
            default:
                {
                    throw new NotImplementedException($"Flag handling for the '{Enum.GetName(typeof(ECssValueTypes), Type)}' css-value type has not been implemented!");
                }
        }

        return Flags;
    }

    #region Instantiation
    /// <summary>Create a keyword value from an enum.</summary>
    /// <typeparam name="T">The enum type (must have EnumRecords keyword support).</typeparam>
    /// <param name="value">The enum value.</param>
    /// <returns>A strongly-typed <see cref="CssEnumValue{T}"/>.</returns>
    public static CssValue From<T>(T value) where T : struct, Enum
    {
        return new CssEnumValue<T>(value);
    }

    /// <summary>Create an absolute integer value</summary>
    [Obsolete("Use specific class constructor instead, eg: new CssIntegerValue(...)")]
    public static CssValue From(int value) => new CssIntegerValue(value);

    /// <summary>Create an absolute integer value if not null, or return the given default value</summary>
    public static CssValue From(int? value, CssValue defaultValue) => (!value.HasValue ? defaultValue : new CssIntegerValue(value.Value));

    /// <summary>Create an absolute number value</summary>
    [Obsolete("Use specific class constructor instead, eg: new CssNUmberValue(...)")]
    public static CssValue From(double value) => new CssNumberValue(value);

    /// <summary>Create an absolute number value if not null, or return the given default value</summary>
    public static CssValue From(double? value, CssValue defaultValue) => (!value.HasValue ? defaultValue : new CssNumberValue(value.Value));

    /// <summary>Create a percentage value</summary>
    /// <param name="value">Floating-point value in the range [0 - 100]</param>
    [Obsolete("Use specific class constructor instead, eg: new CssPercentValue(...)")]
    public static CssValue From_Percent(double value) => new CssPercentValue(value);

    /// <summary>Create an absolute length value</summary>
    public static CssValue From_Dimension(double value, ECssUnit Unit) => Unit switch
    {
        ECssUnit.DPI or ECssUnit.DPCM or ECssUnit.DPPX => new CssResolutionValue(value, Unit),
        _ => new CssDimensionValue(value, Unit)
    };

    /// <summary>Create an absolute length value if not null, or return the given default value</summary>
    public static CssValue From(double? value, ECssUnit Unit, CssValue defaultValue) => (!value.HasValue ? defaultValue : From_Dimension(value.Value, Unit));

    /// <summary>Create an 8-bit RGBA CSS color value</summary>
    [Obsolete("Use specific class constructor instead, eg: new CssColorValue(...)")]
    public static CssValue From(CssColor value) => new CssColorValue(value);

    /// <summary>Create an HDR (wide-gamut) CSS color value</summary>
    [Obsolete("Use specific class constructor instead, eg: new CssColorHdrValue(...)")]
    public static CssValue From(CssColorHdr value) => new CssColorHdrValue(value);

    /// <summary>Create an RGBA color value from rendering Color class</summary>
    [Obsolete("Use specific class constructor instead, eg: new CssColorValue(...)")]
    public static CssValue From(Rendering.Color value) => new CssColorValue(new CssColor(value.R, value.G, value.B, value.A));

    /// <summary>Create an RGBA color value from rendering Rgba struct</summary>
    [Obsolete("Use specific class constructor instead, eg: new CssColorValue(...)")]
    public static CssValue From(Rgba value) => new CssColorValue(new CssColor(value.Red, value.Green, value.Blue, value.Alpha));

    /// <summary>Create an RGBA color value from a ReadOnlyColor (legacy)</summary>
    [Obsolete("Use CssValue.From(CssColor) instead")]
    public static CssValue From(ReadOnlyColor value) => new CssColorValue(new CssColor(value.R, value.G, value.B, value.A));

    /// <summary>Create a string value</summary>
    [Obsolete("Use specific class constructor instead, eg: new CssStringValue(...)")]
    public static CssValue From_String(string value) => new CssStringValue(value);

    /// <summary>Create a keyword (identifier) value from a string</summary>
    /// <remarks>
    /// Use this for untyped CSS identifiers. For strongly-typed enum keywords,
    /// use <see cref="From{T}(T)"/> with an enum value instead.
    /// </remarks>
    [Obsolete("Use specific class constructor instead, eg: new CssKeywordValue(...)")]
    public static CssValue From_Keyword(string keyword) => new CssKeywordValue(keyword);

    /// <summary>Create a URL value</summary>
    [Obsolete("Use specific class constructor instead, eg: new CssUrlValue(...)")]
    public static CssValue From(CssUrl value) => new CssUrlValue(value);

    /// <summary>Create a URL value from a string</summary>
    [Obsolete("Use specific class constructor instead, eg: new CssUrlValue(...)")]
    public static CssValue From_Url(string url) => new CssUrlValue(url);

    /// <summary>Create a calc() expression value</summary>
    [Obsolete("Use specific class constructor instead, eg: new CssCalcValue(...)")]
    public static CssValue From(CssCalcExpression value) => new CssCalcValue(value);

    /// <summary>Create a var() function reference value</summary>
    /// <remarks>
    /// var() references custom properties and are resolved during computed value time.
    /// Docs: https://www.w3.org/TR/css-variables-1/#using-variables
    /// </remarks>
    [Obsolete("Use specific class constructor instead, eg: new CssVarValue(...)")]
    public static CssValue From(CssVarFunction value) => new CssVarValue(value);

    /// <summary>Create an env() function reference value</summary>
    /// <remarks>
    /// env() references environment variables and are resolved during computed value time.
    /// Unlike var(), env() variables are global to a document.
    /// Docs: https://www.w3.org/TR/css-env-1/
    /// </remarks>
    [Obsolete("Use specific class constructor instead, eg: new CssEnvValue(...)")]
    public static CssValue From(CssEnvFunction value) => new CssEnvValue(value);

    /// <summary>Create a unicode-range value</summary>
    /// <remarks>
    /// Unicode-ranges are used in @font-face rules to specify which characters a font supports.
    /// Docs: https://www.w3.org/TR/css-syntax-3/#urange
    /// </remarks>
    [Obsolete("Use specific class constructor instead, eg: new CssUnicodeRangeValue(...)")]
    public static CssValue From(CssUnicodeRange value) => new CssUnicodeRangeValue(value);

    /// <summary>Create a css-value by parsing the given string as CSS markup</summary>
    [Obsolete("Use CssValue.Parse(string, IFormatProvider?) or CssValue.TryParse() instead.")]
    public static CssValue From_CSS(string css) => new CssParser(css).Parse_CssValue();

    #region Parsing (IParsable)
    /// <summary>
    /// Parses a CSS value string.
    /// </summary>
    /// <param name="s">The CSS string to parse.</param>
    /// <param name="provider">The format provider (ignored - CSS is locale-independent).</param>
    /// <returns>The parsed CSS value.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="s"/> is null.</exception>
    /// <exception cref="FormatException">Thrown when the string is not a valid CSS value.</exception>
    public static CssValue Parse(string s, IFormatProvider? provider)
    {
        ArgumentNullException.ThrowIfNull(s);
        if (!TryParse(s, provider, out CssValue? result) || result is null)
        {
            throw new FormatException($"Invalid CSS value: '{s}'");
        }
        return result;
    }

    /// <summary>
    /// Tries to parse a CSS value string.
    /// </summary>
    /// <param name="s">The CSS string to parse.</param>
    /// <param name="provider">The format provider (ignored - CSS is locale-independent).</param>
    /// <param name="result">The parsed CSS value if successful.</param>
    /// <returns>True if parsing succeeded; otherwise, false.</returns>
    /// <remarks>
    /// Delegates to <see cref="CssParser.Parse_CssValue"/> internally.
    /// </remarks>
    public static bool TryParse([NotNullWhen(true)] string? s, IFormatProvider? provider, [NotNullWhen(true)] out CssValue? result)
    {
        result = null;
        if (string.IsNullOrWhiteSpace(s))
            return false;

        try
        {
            var parser = new CssParser(s);
            result = parser.Parse_CssValue();
            // Check if we got a valid, non-null result
            return result is not null && result.Type != ECssValueTypes.NULL;
        }
        catch
        {
            return false;
        }
    }
    #endregion

    /// <summary>
    /// Creates a CSS list value containing the specified values.
    /// </summary>
    /// <param name="values">The values to include in the list.</param>
    /// <returns>A <see cref="CssListValue"/> containing the specified values.</returns>
    /// <exception cref="ArgumentException">Thrown when no values are specified.</exception>
    public static CssValue From(params CssValue[] values)
    {
        if (values.Length <= 0) throw new ArgumentException("One or more values must be specified");
        Contract.EndContractBlock();

        return new CssListValue(values);
    }

    /// <summary>
    /// Creates a CSS list value with a specific separator.
    /// </summary>
    /// <param name="separator">The separator to use when serializing the list.</param>
    /// <param name="values">The values to include in the list.</param>
    /// <returns>A <see cref="CssListValue"/> containing the specified values.</returns>
    /// <exception cref="ArgumentException">Thrown when no values are specified.</exception>
    public static CssValue FromList(ECssListSeparator separator, params CssValue[] values)
    {
        if (values.Length <= 0) throw new ArgumentException("One or more values must be specified");
        Contract.EndContractBlock();

        return new CssListValue(values, separator);
    }


    #endregion

    #region HasFlags
    public bool Has_Flags(ECssValueFlags Flags) => 0 != (flags & Flags);
    #endregion

    #region Value Resolution
    /// <summary>
    /// Gets the final resulting value for a dimension type by multiplying our current numeric value by the unit scale
    /// </summary>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private double ResolveDimension(StyleUnitResolverDelegate UnitResolver)
    {
        ArgumentNullException.ThrowIfNull(UnitResolver);
        Contract.EndContractBlock();

        if (Unit == ECssUnit.None) return AsDecimal();
        return (UnitResolver(Unit) * AsDecimal());
    }

    /// <summary>
    /// Resolves the value to a decimal and returns it if possible, returns NULL otherwise
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public double Resolve(CssUnitResolver UnitResolver)
    {
        ArgumentNullException.ThrowIfNull(UnitResolver);
        Contract.EndContractBlock();

        switch (Type)
        {
            case ECssValueTypes.INTEGER:
                return AsInteger();
            case ECssValueTypes.NUMBER:
                return AsDecimal();
            case ECssValueTypes.DIMENSION:
            case ECssValueTypes.RESOLUTION:
                return UnitResolver.Resolve(AsDecimal(), Unit);
            default:
                {
                    throw new CssException($"CSS Value type \"{Enum.GetName(typeof(ECssValueTypes), Type)}\" cannot be resolved to a number");
                }
        }
    }

    /// <summary>
    /// Resolves the value to a decimal and returns it if possible, returns NULL otherwise
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public double? Resolve(StyleUnitResolverDelegate UnitResolver)
    {
        ArgumentNullException.ThrowIfNull(UnitResolver);
        Contract.EndContractBlock();

        switch (Type)
        {
            case ECssValueTypes.INTEGER:
                return AsInteger();
            case ECssValueTypes.NUMBER:
                return AsDecimal();
            case ECssValueTypes.DIMENSION:
                return ResolveDimension(UnitResolver);
            default:
                return null;
        }
    }

    /// <summary>
    /// Resolves the value to a decimal and returns it if possible, returns defaultValue otherwise
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public double Resolve_Or_Default(StyleUnitResolverDelegate UnitResolver, double defaultValue)
    {
        ArgumentNullException.ThrowIfNull(UnitResolver);
        Contract.EndContractBlock();

        double? num = Resolve(UnitResolver);
        return (num.HasValue ? num.Value : defaultValue);
    }


    /// <summary>
    /// If this instance matches the given Predicate then resolves the value to a decimal and returns it, returns NULL otherwise
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public double? Resolve(StyleUnitResolverDelegate UnitResolver, Func<CssValue, bool> Predicate)
    {
        ArgumentNullException.ThrowIfNull(UnitResolver);
        ArgumentNullException.ThrowIfNull(Predicate);
        Contract.EndContractBlock();

        if (Predicate(this))
        {
            switch (Type)
            {
                case ECssValueTypes.INTEGER:
                    return AsInteger();
                case ECssValueTypes.NUMBER:
                    return AsDecimal();
                case ECssValueTypes.DIMENSION:
                    return ResolveDimension(UnitResolver);
                default:
                    return null;
            }
        }

        return null;
    }

    /// <summary>
    /// If this instance matches the given Predicate then resolves the value to a decimal and returns it, returns defaultValue otherwise
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public double Resolve_Or_Default(StyleUnitResolverDelegate UnitResolver, double defaultValue, Func<CssValue, bool> Predicate)
    {
        ArgumentNullException.ThrowIfNull(UnitResolver);
        ArgumentNullException.ThrowIfNull(Predicate);
        Contract.EndContractBlock();

        double? num = Resolve(UnitResolver, Predicate);
        return (num.HasValue ? num.Value : defaultValue);
    }

    /// <summary>
    /// Resolves the value to a decimal and returns it if possible, returns NULL otherwise
    /// <para>Additionally takes an multiplier as input for resolving the value to a decimal if it's a percentage type</para>
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public double? Resolve(StyleUnitResolverDelegate UnitResolver, double percentageMultiplier)
    {
        ArgumentNullException.ThrowIfNull(UnitResolver);
        Contract.EndContractBlock();

        switch (Type)
        {
            case ECssValueTypes.INTEGER:
                return AsInteger();
            case ECssValueTypes.NUMBER:
                return AsDecimal();
            case ECssValueTypes.DIMENSION:
                return ResolveDimension(UnitResolver);
            case ECssValueTypes.PERCENT:
                return ((AsDecimal() / 100.0) * percentageMultiplier);
            default:
                return null;
        }
    }

    /// <summary>
    /// Resolves the value to a decimal and returns it if possible, returns defaultValue otherwise
    /// <para>Additionally takes an multiplier as input for resolving the value to a decimal if it's a percentage type</para>
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public double Resolve_Or_Default(StyleUnitResolverDelegate UnitResolver, double percentageMultiplier, double defaultValue)
    {
        ArgumentNullException.ThrowIfNull(UnitResolver);
        Contract.EndContractBlock();

        double? num = Resolve(UnitResolver, percentageMultiplier);
        return (num.HasValue ? num.Value : defaultValue);
    }

    /// <summary>
    /// Resolves the value to a decimal and returns it if possible, returns NULL otherwise
    /// <para>Additionally takes an action as input for resolving the value to a decimal if it's a percentage type</para>
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public double? Resolve(StyleUnitResolverDelegate UnitResolver, Func<double, double> percentageResolver)
    {
        ArgumentNullException.ThrowIfNull(UnitResolver);
        ArgumentNullException.ThrowIfNull(percentageResolver);
        Contract.EndContractBlock();

        switch (Type)
        {
            case ECssValueTypes.INTEGER:
                return AsInteger();
            case ECssValueTypes.NUMBER:
                return AsDecimal();
            case ECssValueTypes.DIMENSION:
                return ResolveDimension(UnitResolver);
            case ECssValueTypes.PERCENT:
                return percentageResolver((AsDecimal() / 100.0));
            default:
                return null;
        }
    }

    /// <summary>
    /// Resolves the value to a decimal and returns it if possible, returns defaultValue otherwise
    /// <para>Additionally takes an action as input for resolving the value to a decimal if it's a percentage type</para>
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public double Resolve_Or_Default(StyleUnitResolverDelegate UnitResolver, Func<double, double> percentageResolver, double defaultValue)
    {
        ArgumentNullException.ThrowIfNull(UnitResolver);
        ArgumentNullException.ThrowIfNull(percentageResolver);
        Contract.EndContractBlock();

        double? num = Resolve(UnitResolver, percentageResolver);
        return (num.HasValue ? num.Value : defaultValue);
    }


    /* Resolution To Integer */

    /// <summary>
    /// If this instance matches the given Predicate then resolves the value to an integer and returns it, returns defaultValue otherwise
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public int Resolve_Or_Default(StyleUnitResolverDelegate UnitResolver, int defaultValue, Func<CssValue, bool> Predicate)
    {
        ArgumentNullException.ThrowIfNull(UnitResolver);
        ArgumentNullException.ThrowIfNull(Predicate);
        Contract.EndContractBlock();

        int? num = (int?)Resolve(UnitResolver, Predicate);
        return (num.HasValue ? num.Value : defaultValue);
    }

    /// <summary>
    /// Resolves the value to an integer and returns it if possible, returns defaultValue otherwise
    /// <para>Additionally takes an multiplier as input for resolving the value to a decimal if it's a percentage type</para>
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public int Resolve_Or_Default(StyleUnitResolverDelegate UnitResolver, double percentageMultiplier, int defaultValue)
    {
        ArgumentNullException.ThrowIfNull(UnitResolver);
        Contract.EndContractBlock();

        int? num = (int?)Resolve(UnitResolver, percentageMultiplier);
        return (num.HasValue ? num.Value : defaultValue);
    }

    /// <summary>
    /// Resolves the value to an integer and returns it if possible, returns defaultValue otherwise
    /// <para>Additionally takes an action as input for resolving the value to a decimal if it's a percentage type</para>
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public int Resolve_Or_Default(StyleUnitResolverDelegate UnitResolver, Func<double, double> percentageResolver, int defaultValue)
    {
        ArgumentNullException.ThrowIfNull(UnitResolver);
        Contract.EndContractBlock();

        int? num = (int?)Resolve(UnitResolver, percentageResolver);
        return (num.HasValue ? num.Value : defaultValue);
    }


    /* Non-Unit Resolution */

    /// <summary>
    /// Resolves the value to a decimal and returns it if possible, returns NULL otherwise
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public double? Resolve()
    {
        switch (Type)
        {
            case ECssValueTypes.INTEGER:
                return AsInteger();
            case ECssValueTypes.NUMBER:
                return AsDecimal();
            default:
                return null;
        }
    }

    /// <summary>
    /// Resolves the value to a decimal and returns it if possible, returns NULL otherwise
    /// <para>Additionally takes an multiplier as input for resolving the value to a decimal if it's a percentage type</para>
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public double? Resolve(double percentageMultiplier)
    {
        switch (Type)
        {
            case ECssValueTypes.INTEGER:
                return AsInteger();
            case ECssValueTypes.NUMBER:
                return AsDecimal();
            case ECssValueTypes.PERCENT:
                return ((AsDecimal() / 100.0) * percentageMultiplier);
            default:
                return null;
        }
    }

    /// <summary>
    /// Resolves the value to a decimal and returns it if possible, returns defaultValue otherwise
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public double Resolve_Or_Default(double defaultValue)
    {
        double? num = Resolve();
        return (num.HasValue ? num.Value : defaultValue);
    }

    /// <summary>
    /// If this instance matches the given Predicate then resolves the value to a decimal and returns it, returns NULL otherwise
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public double? Resolve(Func<CssValue, bool> Predicate)
    {
        ArgumentNullException.ThrowIfNull(Predicate);
        Contract.EndContractBlock();

        if (Predicate(this))
        {
            switch (Type)
            {
                case ECssValueTypes.INTEGER:
                    return AsInteger();
                case ECssValueTypes.NUMBER:
                    return AsDecimal();
                default:
                    return null;
            }
        }

        return null;
    }

    /// <summary>
    /// If this instance matches the given Predicate then resolves the value to a decimal and returns it, returns defaultValue otherwise
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public double Resolve_Or_Default(double defaultValue, Func<CssValue, bool> Predicate)
    {
        ArgumentNullException.ThrowIfNull(Predicate);
        Contract.EndContractBlock();

        double? num = Resolve(Predicate);
        return (num.HasValue ? num.Value : defaultValue);
    }

    /// <summary>
    /// Resolves the value to a decimal and returns it if possible, returns defaultValue otherwise
    /// <para>Additionally takes an multiplier as input for resolving the value to a decimal if it's a percentage type</para>
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public double Resolve_Or_Default(double percentageMultiplier, double defaultValue)
    {
        switch (Type)
        {
            case ECssValueTypes.INTEGER:
                return AsInteger();
            case ECssValueTypes.NUMBER:
                return AsDecimal();
            case ECssValueTypes.PERCENT:
                return ((AsDecimal() / 100.0) * percentageMultiplier);
        }

        return defaultValue;
    }

    /// <summary>
    /// Resolves the value to a decimal and returns it if possible, returns NULL otherwise
    /// <para>Additionally takes an action as input for resolving the value to a decimal if it's a percentage type</para>
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public double? Resolve(Func<double, double> percentageResolver)
    {
        ArgumentNullException.ThrowIfNull(percentageResolver);
        Contract.EndContractBlock();

        switch (Type)
        {
            case ECssValueTypes.INTEGER:
                return AsInteger();
            case ECssValueTypes.NUMBER:
                return AsDecimal();
            case ECssValueTypes.PERCENT:
                return percentageResolver((AsDecimal() / 100.0));
            default:
                return null;
        }
    }

    /// <summary>
    /// Resolves the value to a decimal and returns it if possible, returns defaultValue otherwise
    /// <para>Additionally takes an action as input for resolving the value to a decimal if it's a percentage type</para>
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public double Resolve_Or_Default(Func<double, double> percentageResolver, double defaultValue)
    {
        ArgumentNullException.ThrowIfNull(percentageResolver);
        Contract.EndContractBlock();

        switch (Type)
        {
            case ECssValueTypes.INTEGER:
                return AsInteger();
            case ECssValueTypes.NUMBER:
                return AsDecimal();
            case ECssValueTypes.PERCENT:
                return percentageResolver((AsDecimal() / 100.0));
        }

        return defaultValue;
    }


    /* Non-Unit Resolution To Integer */

    /// <summary>
    /// If this instance matches the given Predicate then resolves the value to an integer and returns it, returns defaultValue otherwise
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public int Resolve_Or_Default(int defaultValue, Func<CssValue, bool> Predicate)
    {
        ArgumentNullException.ThrowIfNull(Predicate);
        Contract.EndContractBlock();

        int? num = (int?)Resolve(Predicate);
        return (num.HasValue ? num.Value : defaultValue);
    }

    /// <summary>
    /// Resolves the value to an integer and returns it if possible, returns defaultValue otherwise
    /// <para>Additionally takes an multiplier as input for resolving the value to a decimal if it's a percentage type</para>
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public int Resolve_Or_Default(double percentageMultiplier, int defaultValue)
    {
        int? num = (int?)Resolve(percentageMultiplier);
        return (num.HasValue ? num.Value : defaultValue);
    }

    /// <summary>
    /// Resolves the value to an integer and returns it if possible, returns defaultValue otherwise
    /// <para>Additionally takes an action as input for resolving the value to a decimal if it's a percentage type</para>
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public int Resolve_Or_Default(Func<double, double> percentageResolver, int defaultValue)
    {
        ArgumentNullException.ThrowIfNull(percentageResolver);
        Contract.EndContractBlock();

        int? num = (int?)Resolve(percentageResolver);
        return (num.HasValue ? num.Value : defaultValue);
    }

    #endregion

    #region Converters
    /// <summary>
    /// Returns the value as the specified enum type.
    /// </summary>
    /// <typeparam name="T">The enum type to cast to.</typeparam>
    /// <returns>The enum value.</returns>
    /// <exception cref="CssException">Thrown if the value is not a keyword type.</exception>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public virtual T AsEnum<T>() where T : struct, Enum
    {
        throw new CssException($"{nameof(CssValue)} base type does not support AsEnum. Use CssEnumValue<T> subclass.");
    }

    /// <summary>
    /// Returns the value as a Point2f position if possible.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public virtual Point2f AsPosition()
    {
        throw new CssException($"{nameof(CssValue)} is not a Position! {this}");
    }

    /// <summary>
    /// Returns the value as a CssColor.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public virtual CssColor AsCssColor()
    {
        throw new CssException($"{nameof(CssValue)} is not a Color! {this}");
    }

    /// <summary>
    /// Returns the value as a CssColorHdr (wide-gamut HDR color).
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public virtual CssColorHdr AsCssColorHdr()
    {
        throw new CssException($"{nameof(CssValue)} is not an HDR Color! {this}");
    }

    /// <summary>
    /// Returns the value as a CssUrl.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public virtual CssUrl AsUrl()
    {
        throw new CssException($"{nameof(CssValue)} is not a URL! {this}");
    }

    /// <summary>
    /// Returns the value as a CssCalcExpression.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public virtual CssCalcExpression AsCalcExpression()
    {
        throw new CssException($"{nameof(CssValue)} is not a calc() expression! {this}");
    }

    /// <summary>
    /// Returns the value as a CssVarFunction.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public virtual CssVarFunction AsVarFunction()
    {
        throw new CssException($"{nameof(CssValue)} is not a var() function! {this}");
    }

    /// <summary>
    /// Returns the value as a CssEnvFunction.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public virtual CssEnvFunction AsEnvFunction()
    {
        throw new CssException($"{nameof(CssValue)} is not an env() function! {this}");
    }

    /// <summary>
    /// Returns the value as a CssUnicodeRange.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public virtual CssUnicodeRange AsUnicodeRange()
    {
        throw new CssException($"{nameof(CssValue)} is not a unicode-range! {this}");
    }

    /// <summary>
    /// Returns the value as a ReadOnlyColor (legacy accessor, prefer AsCssColor).
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    [Obsolete("Use AsCssColor() instead")]
    public ReadOnlyColor AsColor()
    {
        var color = AsCssColor();
        return new ReadOnlyColor(color.R, color.G, color.B, color.A);
    }

    /// <summary>
    /// Attempts to get the color value regardless of whether it's standard or HDR.
    /// </summary>
    /// <param name="color">The color value if successful.</param>
    /// <returns>True if this is a color type.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public virtual bool TryGetColor(out CssColor color)
    {
        color = default;
        return false;
    }

    /// <summary>
    /// Returns the value as a collection of CssValues.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public virtual IReadOnlyList<CssValue> AsCollection()
    {
        throw new CssException($"{nameof(CssValue)} is not a collection! {this}");
    }

    /// <summary>
    /// Returns the value as the preferred Integer type.
    /// </summary>
    /// <exception cref="CssException">Thrown if the value type does not support integer conversion.</exception>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public virtual long AsInteger()
    {
        throw new CssException($"{nameof(CssValue)} type {Type} does not support AsInteger.");
    }

    /// <summary>
    /// Returns the value as the preferred (Nullable) Integer type
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public long? AsIntegerN() => !HasValue ? null : AsInteger();

    /// <summary>
    /// Returns the value as the preferred Decimal type.
    /// </summary>
    /// <exception cref="CssException">Thrown if the value type does not support decimal conversion.</exception>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public virtual double AsDecimal()
    {
        throw new CssException($"{nameof(CssValue)} type {Type} does not support AsDecimal.");
    }

    /// <summary>
    /// Returns the value as the preferred (Nullable) Decimal type
    /// </summary>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public double? AsDecimalN() => !HasValue ? null : AsDecimal();

    /// <summary>
    /// Returns the value as a string
    /// </summary>
    /// <exception cref="CssException">Thrown if the value is not a string type.</exception>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public virtual string AsString()
    {
        throw new CssException($"{nameof(CssValue)} is not a String! {this}");
    }

    /// <summary>
    /// Returns the value as a keyword string (for KEYWORD type values).
    /// </summary>
    /// <exception cref="CssException">Thrown if the value is not a keyword type.</exception>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public virtual string AsKeyword()
    {
        throw new CssException($"{nameof(CssValue)} is not a Keyword! {this}");
    }

    /// <summary>
    /// Returns the value as a CssFunction (for FUNCTION type values).
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal virtual CssFunction? AsFunction() => null;
    #endregion

    #region Equality
    /// <summary>
    /// Determines equality between this CssValue and another.
    /// </summary>
    /// <remarks>
    /// <para>
    /// This method handles base type comparison (Type and Unit fields).
    /// For singleton types (NULL, AUTO, INHERIT, etc.) that have no data payload,
    /// this is sufficient for equality.
    /// </para>
    /// <para>
    /// For value-carrying types, derived record classes extend this check with
    /// their own field comparisons via the record-generated equality chain.
    /// </para>
    /// </remarks>
    public virtual bool Equals(CssValue? other)
    {
        if (other is null)
            return false;

        if (ReferenceEquals(this, other))
            return true;

        // Compare base type fields - Type and Unit must match
        if (Type != other.Type) return false;
        if (Unit != other.Unit) return false;

        // For the base CssValue type, type matching is sufficient
        // Derived records will add their own field comparisons
        return true;
    }

    public override int GetHashCode()
    {
        // Base implementation: hash only type and unit (for singleton values)
        // Subclasses override this via record-generated GetHashCode
        return HashCode.Combine(type, unit);
    }

    #endregion

    #region ToString
    public override string ToString()
    {
        const string DECIMAL_FORMAT = "0.###";
        switch (Type)
        {
            case ECssValueTypes.COLOR:
                return CssColorSerializer.Serialize(AsCssColor());
            case ECssValueTypes.COLOR_HDR:
                return CssColorSerializer.Serialize(AsCssColorHdr());
            case ECssValueTypes.INTEGER:
                return AsInteger().ToString(CultureInfo.InvariantCulture);
            case ECssValueTypes.NUMBER:
                return string.Concat(AsDecimal().ToString(DECIMAL_FORMAT, CultureInfo.InvariantCulture));
            case ECssValueTypes.DIMENSION:
                if (Unit == ECssUnit.None)
                    return string.Concat(AsDecimal().ToString(DECIMAL_FORMAT, CultureInfo.InvariantCulture), "<none>");
                else
                    return string.Concat(AsDecimal().ToString(DECIMAL_FORMAT, CultureInfo.InvariantCulture), unit.Keyword());
            case ECssValueTypes.PERCENT:
                return string.Concat(AsDecimal().ToString(DECIMAL_FORMAT, CultureInfo.InvariantCulture), "%");
            case ECssValueTypes.STRING:
                return string.Concat(UnicodeCommon.CHAR_QUOTATION_MARK, AsString(), UnicodeCommon.CHAR_QUOTATION_MARK);
            case ECssValueTypes.KEYWORD:
                return AsKeyword();
            default:
                return string.Concat("[", Enum.GetName(typeof(ECssValueTypes), Type), "]");
        }
    }

    /// <inheritdoc/>
    public virtual string ToString(string? format, IFormatProvider? formatProvider) => ToString();

    /// <summary>
    /// Tries to format this CSS value into the provided span.
    /// </summary>
    /// <param name="destination">The span to write to.</param>
    /// <param name="charsWritten">The number of characters written.</param>
    /// <param name="format">The format string (ignored).</param>
    /// <param name="provider">The format provider (ignored for CSS values).</param>
    /// <returns>True if formatting succeeded; otherwise, false.</returns>
    public virtual bool TryFormat(Span<char> destination, out int charsWritten, ReadOnlySpan<char> format = default, IFormatProvider? provider = null)
    {
        charsWritten = 0;

        switch (Type)
        {
            case ECssValueTypes.INTEGER:
                return AsInteger().TryFormat(destination, out charsWritten, default, CultureInfo.InvariantCulture);

            case ECssValueTypes.NUMBER:
                return AsDecimal().TryFormat(destination, out charsWritten, "0.###", CultureInfo.InvariantCulture);

            case ECssValueTypes.PERCENT:
                {
                    if (!AsDecimal().TryFormat(destination, out charsWritten, "0.###", CultureInfo.InvariantCulture))
                        return false;

                    if (destination.Length <= charsWritten)
                        return false;

                    destination[charsWritten] = '%';
                    charsWritten++;
                    return true;
                }

            case ECssValueTypes.DIMENSION:
                return TryFormatDimension(destination, out charsWritten);

            case ECssValueTypes.COLOR:
                return AsCssColor().TryFormat(destination, out charsWritten, format, provider);

            case ECssValueTypes.COLOR_HDR:
                // HDR colors fall back to string allocation (complex formatting)
                return TryFormatFromString(CssColorSerializer.Serialize(AsCssColorHdr()), destination, out charsWritten);

            case ECssValueTypes.STRING:
                return TryFormatString(destination, out charsWritten);

            case ECssValueTypes.KEYWORD:
                {
                    var keyword = AsKeyword();
                    return TryFormatFromString(keyword, destination, out charsWritten);
                }

            case ECssValueTypes.AUTO:
                return TryFormatFromSpan("auto".AsSpan(), destination, out charsWritten);

            case ECssValueTypes.INHERIT:
                return TryFormatFromSpan("inherit".AsSpan(), destination, out charsWritten);

            case ECssValueTypes.INITIAL:
                return TryFormatFromSpan("initial".AsSpan(), destination, out charsWritten);

            case ECssValueTypes.DEFAULT:
                return TryFormatFromSpan("default".AsSpan(), destination, out charsWritten);

            case ECssValueTypes.UNSET:
                return TryFormatFromSpan("unset".AsSpan(), destination, out charsWritten);

            case ECssValueTypes.NONE:
                return TryFormatFromSpan("none".AsSpan(), destination, out charsWritten);

            case ECssValueTypes.NULL:
                // Empty string for null values
                charsWritten = 0;
                return true;

            default:
                // For unhandled types, format as "[TYPE]"
                return TryFormatAsTypeName(destination, out charsWritten);
        }
    }

    /// <summary>
    /// Formats a dimension value (number + unit) into the span.
    /// </summary>
    private bool TryFormatDimension(Span<char> destination, out int charsWritten)
    {
        charsWritten = 0;

        // Format the numeric part
        if (!AsDecimal().TryFormat(destination, out charsWritten, "0.###", CultureInfo.InvariantCulture))
            return false;

        // Format the unit
        if (Unit == ECssUnit.None)
        {
            // Special case: no unit → "<none>" for debugging
            ReadOnlySpan<char> noneStr = "<none>".AsSpan();
            if (destination.Length < charsWritten + noneStr.Length)
                return false;

            noneStr.CopyTo(destination[charsWritten..]);
            charsWritten += noneStr.Length;
            return true;
        }
        else
        {
            // Get unit string
            string unitStr = Unit.Keyword();

            if (destination.Length < charsWritten + unitStr.Length)
                return false;

            unitStr.AsSpan().CopyTo(destination[charsWritten..]);
            charsWritten += unitStr.Length;
            return true;
        }
    }

    /// <summary>
    /// Formats a quoted CSS string value into the span.
    /// </summary>
    private bool TryFormatString(Span<char> destination, out int charsWritten)
    {
        charsWritten = 0;
        var str = AsString();

        // Need space for: quote + string + quote
        int required = 1 + str.Length + 1;
        if (destination.Length < required)
            return false;

        destination[0] = UnicodeCommon.CHAR_QUOTATION_MARK;
        str.AsSpan().CopyTo(destination[1..]);
        destination[1 + str.Length] = UnicodeCommon.CHAR_QUOTATION_MARK;
        charsWritten = required;
        return true;
    }

    /// <summary>
    /// Formats unhandled value types as "[TYPE]" for debugging.
    /// </summary>
    private bool TryFormatAsTypeName(Span<char> destination, out int charsWritten)
    {
        charsWritten = 0;

        var typeName = Enum.GetName(typeof(ECssValueTypes), Type) ?? "UNKNOWN";
        int required = 1 + typeName.Length + 1; // "[" + typeName + "]"

        if (destination.Length < required)
            return false;

        destination[0] = '[';
        typeName.AsSpan().CopyTo(destination[1..]);
        destination[1 + typeName.Length] = ']';
        charsWritten = required;
        return true;
    }

    /// <summary>
    /// Helper to copy a pre-formatted string into the destination span.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static bool TryFormatFromString(string value, Span<char> destination, out int charsWritten)
    {
        if (destination.Length < value.Length)
        {
            charsWritten = 0;
            return false;
        }

        value.AsSpan().CopyTo(destination);
        charsWritten = value.Length;
        return true;
    }

    /// <summary>
    /// Helper to copy a pre-formatted span into the destination span.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static bool TryFormatFromSpan(ReadOnlySpan<char> value, Span<char> destination, out int charsWritten)
    {
        if (destination.Length < value.Length)
        {
            charsWritten = 0;
            return false;
        }

        value.CopyTo(destination);
        charsWritten = value.Length;
        return true;
    }
    #endregion

    #region Serialization
    public virtual string Serialize()
    {
        switch (Type)
        {
            case ECssValueTypes.AUTO:
                return "auto";
            case ECssValueTypes.INHERIT:
                return "inherit";
            case ECssValueTypes.INITIAL:
                return "initial";
            case ECssValueTypes.DEFAULT:
                return "default";
            case ECssValueTypes.UNSET:
                return "unset";
            case ECssValueTypes.NONE:
                return "none";
            case ECssValueTypes.NULL:
                {
                    return string.Empty;
                }
            case ECssValueTypes.KEYWORD:
                {
                    return AsString();
                }
            case ECssValueTypes.STRING:
                {
                    return string.Concat(UnicodeCommon.CHAR_QUOTATION_MARK, AsString(), UnicodeCommon.CHAR_QUOTATION_MARK);
                }
            case ECssValueTypes.COLOR:
                {
                    return CssColorSerializer.Serialize(AsCssColor());
                }
            case ECssValueTypes.COLOR_HDR:
                {
                    return CssColorSerializer.Serialize(AsCssColorHdr());
                }
            case ECssValueTypes.DIMENSION:
                {
                    if (Unit == ECssUnit.None)
                    {
                        return AsDecimal().ToString(CultureInfo.InvariantCulture);
                    }
                    else
                    {
                        string unitStr = Unit.Keyword();

                        return string.Concat(AsDecimal().ToString(CultureInfo.InvariantCulture), unitStr);
                    }
                }
            case ECssValueTypes.PERCENT:
                {
                    return string.Concat(AsDecimal().ToString(CultureInfo.InvariantCulture), UnicodeCommon.CHAR_PERCENT);
                }
            case ECssValueTypes.NUMBER:
                {
                    return AsDecimal().ToString(CultureInfo.InvariantCulture);
                }
            case ECssValueTypes.INTEGER:
                {
                    return AsInteger().ToString(CultureInfo.InvariantCulture);
                }
            default:
                {
                    throw new NotImplementedException($"Serialization logic for CSS value type {Type} has not been implemented");
                }
        }
    }
    #endregion
}

