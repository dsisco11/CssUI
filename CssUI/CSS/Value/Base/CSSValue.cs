using System;
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
public partial class CssValue : ISpanFormattable, IFormattable, IParsable<CssValue>
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
    /// <summary>
    /// Union storage for primitive values (numbers, integers, colors) without boxing.
    /// </summary>
    private readonly CssValueData data;
    #endregion

    #region Accessors
    public ECssValueFlags Flags => flags;
    public ECssValueTypes Type => type;
    public ECssUnit Unit => unit;

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
            if (Type == ECssValueTypes.NULL)
                return false;

            // For primitive types stored in the union, they always have a value
            if (Type == ECssValueTypes.INTEGER || Type == ECssValueTypes.NUMBER ||
                Type == ECssValueTypes.COLOR || Type == ECssValueTypes.COLOR_HDR ||
                Type == ECssValueTypes.PERCENT || Type == ECssValueTypes.DIMENSION ||
                Type == ECssValueTypes.RESOLUTION)
            {
                return true;
            }

            // For reference types, check if ObjectValue is set
            return data.ObjectValue is not null;
        }
    }
    #endregion

    #region Constructors
    internal CssValue(CssFunction function)
    {
        type = ECssValueTypes.FUNCTION;
        data = CssValueData.FromObject(function);
    }

    /// <summary>
    /// Base constructor for creating a CssValue with only a type.
    /// </summary>
    /// <param name="type">The CSS value type.</param>
    protected CssValue(ECssValueTypes type)
    {
        this.type = type;
        this.data = default;
        flags |= Get_Inherent_Value_Type_Flags(type, unit, in data);
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

    internal CssValue(ECssValueTypes type, CssValueData data) : this(type)
    {
        this.data = data;
    }

    internal CssValue(ECssValueTypes type, CssValueData data, ECssValueFlags flags) : this(type)
    {
        this.data = data;
        this.flags |= flags;
    }

    internal CssValue(ECssValueTypes type, CssValueData data, ECssUnit unit) : this(type)
    {
        this.unit = unit;
        this.data = data;

        if (type == ECssValueTypes.KEYWORD)// Try and catch some common IMPORTANT keywords
        {
            /* If our keyword can be resolved to another ECssValueType then its an global keyword */
            if (data.ObjectValue is string strValue && TryParseCssGlobalKeyword(strValue, out ECssValueTypes outKeyword))
            {
                this.type = outKeyword;
            }
        }
        else if (type == ECssValueTypes.DIMENSION || type == ECssValueTypes.RESOLUTION)
        {
            /* Make sure to correct the value and distinguish whether we are a Dimension or a Resolution */
            switch (unit)
            {
                case ECssUnit.DPI:
                case ECssUnit.DPCM:
                case ECssUnit.DPPX:
                    this.type = ECssValueTypes.RESOLUTION;
                    break;
                default:
                    this.type = ECssValueTypes.DIMENSION;
                    break;
            }
        }
    }

    /// <summary>
    /// Clones an already existing <see cref="CssValue"/>
    /// </summary>
    internal CssValue(CssValue sv)
    {
        type = sv.Type;
        data = sv.data;
        unit = sv.Unit;
        flags = sv.Flags;
    }

    #region Legacy Constructors (for internal parser/resolver compatibility)
    /// <summary>
    /// Legacy constructor for parsers - creates a value from an object (will be boxed for non-primitives).
    /// </summary>
    internal CssValue(ECssValueTypes type, object? value) : this(type)
    {
        this.data = value switch
        {
            int i => CssValueData.FromInteger(i),
            double d => CssValueData.FromNumber(d),
            float f => CssValueData.FromNumber(f),
            string s => CssValueData.FromObject(s),
            CssColor c => CssValueData.FromColor(c),
            CssColorHdr ch => CssValueData.FromColorHdr(ch),
            _ => CssValueData.FromObject(value)
        };
    }

    /// <summary>
    /// Legacy constructor for parsers - creates a dimension value.
    /// </summary>
    internal CssValue(ECssValueTypes type, object? value, ECssUnit unit) : this(type)
    {
        this.unit = unit;
        this.data = value switch
        {
            int i => CssValueData.FromNumber(i),
            double d => CssValueData.FromNumber(d),
            float f => CssValueData.FromNumber(f),
            _ => CssValueData.FromObject(value)
        };

        if (type == ECssValueTypes.KEYWORD && data.ObjectValue is string strValue && TryParseCssGlobalKeyword(strValue, out ECssValueTypes outKeyword))
        {
            this.type = outKeyword;
        }
        else if (type == ECssValueTypes.DIMENSION || type == ECssValueTypes.RESOLUTION)
        {
            switch (unit)
            {
                case ECssUnit.DPI:
                case ECssUnit.DPCM:
                case ECssUnit.DPPX:
                    this.type = ECssValueTypes.RESOLUTION;
                    break;
                default:
                    this.type = ECssValueTypes.DIMENSION;
                    break;
            }
        }
    }
    #endregion
    #endregion

    private static ECssValueFlags Get_Inherent_Value_Type_Flags(ECssValueTypes Type, ECssUnit Unit, in CssValueData Data)
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
            case ECssValueTypes.COLLECTION:// A collections flags are the combined flags of all it's sub-values
                {
                    if (Data.ObjectValue is null)
                        break;

                    if (Data.ObjectValue is CssValue cssValue)
                    {// we are a nested collection.
                        Flags |= Get_Inherent_Value_Type_Flags(cssValue.Type, cssValue.Unit, in cssValue.data);
                        break;
                    }

                    if (Data.ObjectValue is not CssValue[] valueArray)
                        throw new CssException($"Expected an array of {nameof(CssValue)}s for a collection type!");

                    foreach (CssValue val in valueArray)
                    {
                        Flags |= Get_Inherent_Value_Type_Flags(val.Type, val.Unit, in val.data);
                    }
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
    /// <summary> Returns a new <see cref="CssValue"/> instance which is a copy of this one. </summary>
    public CssValue Clone() => new CssValue(this);

    /// <summary>Create a keyword value from an enum.</summary>
    /// <typeparam name="T">The enum type (must have EnumRecords keyword support).</typeparam>
    /// <param name="value">The enum value.</param>
    /// <returns>A strongly-typed <see cref="CssEnumValue{T}"/>.</returns>
    public static CssValue From<T>(T value) where T : struct, Enum
    {
        return new CssEnumValue<T>(value);
    }

    /// <summary>Create an absolute integer value</summary>
    public static CssValue From(int value) => new CssValue(ECssValueTypes.INTEGER, CssValueData.FromInteger(value));

    /// <summary>Create an absolute integer value if not null, or return the given default value</summary>
    public static CssValue From(int? value, CssValue defaultValue) => (!value.HasValue ? defaultValue : new CssValue(ECssValueTypes.INTEGER, CssValueData.FromInteger(value.Value)));

    /// <summary>Create an absolute number value</summary>
    public static CssValue From(double value) => new CssValue(ECssValueTypes.NUMBER, CssValueData.FromNumber(value));

    /// <summary>Create an absolute number value if not null, or return the given default value</summary>
    public static CssValue From(double? value, CssValue defaultValue) => (!value.HasValue ? defaultValue : new CssValue(ECssValueTypes.NUMBER, CssValueData.FromNumber(value.Value)));

    /// <summary>Create a percentage value</summary>
    /// <param name="value">Floating-point value in the range [0 - 100]</param>
    public static CssValue From_Percent(double value) => new CssValue(ECssValueTypes.PERCENT, CssValueData.FromNumber(value));

    /// <summary>Create an absolute length value</summary>
    public static CssValue From(double value, ECssUnit Unit) => new CssValue(ECssValueTypes.DIMENSION, CssValueData.FromNumber(value), Unit);

    /// <summary>Create an absolute length value if not null, or return the given default value</summary>
    public static CssValue From(double? value, ECssUnit Unit, CssValue defaultValue) => (!value.HasValue ? defaultValue : new CssValue(ECssValueTypes.DIMENSION, CssValueData.FromNumber(value.Value), Unit));

    /// <summary>Create an 8-bit RGBA CSS color value</summary>
    public static CssValue From(CssColor value) => new CssValue(ECssValueTypes.COLOR, CssValueData.FromColor(value));

    /// <summary>Create an HDR (wide-gamut) CSS color value</summary>
    public static CssValue From(CssColorHdr value) => new CssValue(ECssValueTypes.COLOR_HDR, CssValueData.FromColorHdr(value));

    /// <summary>Create an RGBA color value from rendering Color class</summary>
    public static CssValue From(Rendering.Color value) => new CssValue(ECssValueTypes.COLOR, CssValueData.FromColor(new CssColor(value.R, value.G, value.B, value.A)));

    /// <summary>Create an RGBA color value from rendering Rgba struct</summary>
    public static CssValue From(Rgba value) => new CssValue(ECssValueTypes.COLOR, CssValueData.FromColor(new CssColor(value.Red, value.Green, value.Blue, value.Alpha)));

    /// <summary>Create an RGBA color value from a ReadOnlyColor (legacy)</summary>
    [Obsolete("Use CssValue.From(CssColor) instead")]
    public static CssValue From(ReadOnlyColor value) => new CssValue(ECssValueTypes.COLOR, CssValueData.FromColor(new CssColor(value.R, value.G, value.B, value.A)));

    /// <summary>Create a string value</summary>
    public static CssValue From_String(string value) => new CssStringValue(value);

    /// <summary>Create a URL value</summary>
    public static CssValue From(CssUrl value) => new CssUrlValue(value);

    /// <summary>Create a URL value from a string</summary>
    public static CssValue From_Url(string url) => new CssUrlValue(url);

    /// <summary>Create a calc() expression value</summary>
    public static CssValue From(CssCalcExpression value) => new CssCalcValue(value);

    /// <summary>Create a var() function reference value</summary>
    /// <remarks>
    /// var() references custom properties and are resolved during computed value time.
    /// Docs: https://www.w3.org/TR/css-variables-1/#using-variables
    /// </remarks>
    public static CssValue From(CssVarFunction value) => new CssValue(ECssValueTypes.VAR, CssValueData.FromObject(value));

    /// <summary>Create an env() function reference value</summary>
    /// <remarks>
    /// env() references environment variables and are resolved during computed value time.
    /// Unlike var(), env() variables are global to a document.
    /// Docs: https://www.w3.org/TR/css-env-1/
    /// </remarks>
    public static CssValue From(CssEnvFunction value) => new CssValue(ECssValueTypes.ENV, CssValueData.FromObject(value));

    /// <summary>Create a unicode-range value</summary>
    /// <remarks>
    /// Unicode-ranges are used in @font-face rules to specify which characters a font supports.
    /// Docs: https://www.w3.org/TR/css-syntax-3/#urange
    /// </remarks>
    public static CssValue From(CssUnicodeRange value) => new CssValue(ECssValueTypes.UNICODE_RANGE, CssValueData.FromObject(value));

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
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public virtual T AsEnum<T>() where T : struct, Enum
    {
        // Base implementation: extract from boxed ObjectValue
        return (T)(data.ObjectValue ?? default(T));
    }

    /// <summary>
    /// Returns the value as a Point2f position if possible.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Point2f AsPosition()
    {
        if (Type != ECssValueTypes.POSITION) throw new CssException($"{nameof(CssValue)} is not a Position! {this}");
        Contract.EndContractBlock();

        return (Point2f)(data.ObjectValue ?? default(Point2f));
    }

    /// <summary>
    /// Returns the value as a CssColor.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public CssColor AsCssColor()
    {
        if (Type != ECssValueTypes.COLOR) throw new CssException($"{nameof(CssValue)} is not a Color! {this}");
        Contract.EndContractBlock();

        return data.ColorValue;
    }

    /// <summary>
    /// Returns the value as a CssColorHdr (wide-gamut HDR color).
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public CssColorHdr AsCssColorHdr()
    {
        if (Type != ECssValueTypes.COLOR_HDR) throw new CssException($"{nameof(CssValue)} is not an HDR Color! {this}");
        Contract.EndContractBlock();

        return data.ColorHdrValue;
    }

    /// <summary>
    /// Returns the value as a CssUrl.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public virtual CssUrl AsUrl()
    {
        if (Type != ECssValueTypes.URL) throw new CssException($"{nameof(CssValue)} is not a URL! {this}");
        Contract.EndContractBlock();

        return (CssUrl)(data.ObjectValue ?? CssUrl.Empty);
    }

    /// <summary>
    /// Returns the value as a CssCalcExpression.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public virtual CssCalcExpression AsCalcExpression()
    {
        if (Type != ECssValueTypes.CALC) throw new CssException($"{nameof(CssValue)} is not a calc() expression! {this}");
        Contract.EndContractBlock();

        return (CssCalcExpression)(data.ObjectValue ?? throw new CssException("calc() expression is null"));
    }

    /// <summary>
    /// Returns the value as a CssVarFunction.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public CssVarFunction AsVarFunction()
    {
        if (Type != ECssValueTypes.VAR) throw new CssException($"{nameof(CssValue)} is not a var() function! {this}");
        Contract.EndContractBlock();

        return (CssVarFunction)(data.ObjectValue ?? throw new CssException("var() function is null"));
    }

    /// <summary>
    /// Returns the value as a CssEnvFunction.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public CssEnvFunction AsEnvFunction()
    {
        if (Type != ECssValueTypes.ENV) throw new CssException($"{nameof(CssValue)} is not an env() function! {this}");
        Contract.EndContractBlock();

        return (CssEnvFunction)(data.ObjectValue ?? throw new CssException("env() function is null"));
    }

    /// <summary>
    /// Returns the value as a CssUnicodeRange.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public CssUnicodeRange AsUnicodeRange()
    {
        if (Type != ECssValueTypes.UNICODE_RANGE) throw new CssException($"{nameof(CssValue)} is not a unicode-range! {this}");
        Contract.EndContractBlock();

        return (CssUnicodeRange)(data.ObjectValue ?? throw new CssException("unicode-range is null"));
    }

    /// <summary>
    /// Returns the value as a ReadOnlyColor (legacy accessor, prefer AsCssColor).
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    [Obsolete("Use AsCssColor() instead")]
    public ReadOnlyColor AsColor()
    {
        if (Type != ECssValueTypes.COLOR) throw new CssException($"{nameof(CssValue)} is not a Color! {this}");
        Contract.EndContractBlock();

        var color = data.ColorValue;
        return new ReadOnlyColor(color.R, color.G, color.B, color.A);
    }

    /// <summary>
    /// Attempts to get the color value regardless of whether it's standard or HDR.
    /// </summary>
    /// <param name="color">The color value if successful.</param>
    /// <returns>True if this is a color type.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool TryGetColor(out CssColor color)
    {
        if (Type == ECssValueTypes.COLOR)
        {
            color = data.ColorValue;
            return true;
        }
        if (Type == ECssValueTypes.COLOR_HDR)
        {
            color = data.ColorHdrValue.ToCssColor();
            return true;
        }
        color = default;
        return false;
    }

    /// <summary>
    /// Returns the value as a collection of CssValues.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public virtual ReadOnlyCollection<CssValue> AsCollection()
    {
        if (!IsCollection) throw new CssException($"{nameof(CssValue)} is not a collection! {this}");
        Contract.EndContractBlock();

        return new ReadOnlyCollection<CssValue>((CssValue[])(data.ObjectValue ?? Array.Empty<CssValue>()));
    }

    /// <summary>
    /// Returns the value as the preferred Integer type.
    /// For INTEGER types, returns the stored integer. For NUMBER/DIMENSION/PERCENT, converts from double with banker's rounding.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public int AsInteger() => Type == ECssValueTypes.INTEGER ? (int)data.IntegerValue : Convert.ToInt32(data.NumberValue);

    /// <summary>
    /// Returns the value as the preferred (Nullable) Integer type
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public int? AsIntegerN() => !HasValue ? null : AsInteger();

    /// <summary>
    /// Returns the value as the preferred Decimal type.
    /// For INTEGER types, converts from int. For NUMBER/DIMENSION/PERCENT, returns the stored double.
    /// </summary>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public double AsDecimal() => Type == ECssValueTypes.INTEGER ? data.IntegerValue : data.NumberValue;

    /// <summary>
    /// Returns the value as the preferred (Nullable) Decimal type
    /// </summary>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public double? AsDecimalN() => !HasValue ? null : AsDecimal();

    /// <summary>
    /// Returns the value as a string
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public virtual string AsString() => (string)(data.ObjectValue ?? string.Empty);

    /// <summary>
    /// Returns the value as a CssFunction (for FUNCTION type values).
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal CssFunction? AsFunction() => data.ObjectValue as CssFunction;
    #endregion

    #region Operators
    public static bool operator ==(in CssValue? A, in CssValue? B)
    {
        // If either object is null return whether they are BOTH null
        if (A is null || B is null)
            return (A is null && B is null);

        if (A.Type != B.Type) return false;
        if (A.Unit != B.Unit) return false;

        switch (A.Type)
        {
            case ECssValueTypes.NULL:
            case ECssValueTypes.UNSET:
            case ECssValueTypes.AUTO:
            case ECssValueTypes.INITIAL:
            case ECssValueTypes.INHERIT:
            case ECssValueTypes.NONE:
                return true;// Types are already the same, meaning A/B are equal
            case ECssValueTypes.COLOR:
                return A.data.ColorValue == B.data.ColorValue;
            case ECssValueTypes.COLOR_HDR:
                return A.data.ColorHdrValue == B.data.ColorHdrValue;
            case ECssValueTypes.INTEGER:
                return A.data.IntegerValue == B.data.IntegerValue;
            case ECssValueTypes.NUMBER:
            case ECssValueTypes.DIMENSION:
            case ECssValueTypes.PERCENT:
                return A.data.NumberValue == B.data.NumberValue;
            case ECssValueTypes.STRING:
                return string.Equals((string?)A.data.ObjectValue, (string?)B.data.ObjectValue, StringComparison.Ordinal);
            case ECssValueTypes.KEYWORD:
                // Delegate to Equals which handles CssEnumValue<T> comparison
                return A.Equals(B);
            default:
                throw new NotImplementedException($"Equality comparison logic not implemented for type: {Enum.GetName(typeof(ECssValueTypes), A.Type)}");
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool operator !=(in CssValue? A, in CssValue? B)
    {
        return !(A == B);
    }


    public override bool Equals(object? o)
    {
        if (o is null)
            return false;

        if (o is CssValue cssVal)
        {
            return this == cssVal;
        }

        return false;
    }

    public override int GetHashCode()
    {
        return ToString().GetHashCode();
    }

    #endregion

    #region ToString
    public new virtual string ToString()
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
                return Lookup.Keyword(data.ObjectValue!.GetType(), data.ObjectValue);// Enum.GetName(value.GetType(), value)
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
                    var keyword = Lookup.Keyword(data.ObjectValue!.GetType(), data.ObjectValue);
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

