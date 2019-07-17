using CssUI.CSS.Internal;
using CssUI.CSS.Parser;
using CssUI.CSS.Serialization;
using CssUI.Internal;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace CssUI.CSS
{
    /// <summary>
    /// Represents a CSS Value
    /// </summary>
    public partial class CssValue
    {
        #region Delegates
        public delegate double StyleUnitResolverDelegate(ECssUnit unit);
        #endregion

        #region Static Declerations
        /// <summary>
        /// Value depends on other properties
        /// </summary>
        public static CssValue Auto = new CssValue(ECssValueType.AUTO);
        /// <summary>
        /// (Non-Cascadeable)
        /// Treat as nothing. Eg; The stylesheet didnt specify anything for this property.
        /// </summary>
        public static CssValue Null = new CssValue(ECssValueType.NULL);
        /// <summary>
        /// (Non-Cascadeable)
        /// Intentionally unsets a property, forcing it to resolve to either its inherited or initial value.
        /// DOCS: https://www.w3.org/TR/css-cascade-3/#inherit-initial
        /// </summary>
        public static CssValue Unset = new CssValue(ECssValueType.UNSET);
        /// <summary>
        /// Value should resolve to it's definitions default value
        /// </summary>
        public static CssValue Initial = new CssValue(ECssValueType.INITIAL);
        /// <summary>
        /// Value is inherited from the parent element
        /// </summary>
        public static CssValue Inherit = new CssValue(ECssValueType.INHERIT);
        /// <summary>
        /// (Cascades)
        /// Value is purposly nothing, No value is assigned, some properties use this state to be ignored.
        /// As in they wont have an effect on the elements styling or block
        /// </summary>
        public static CssValue None = new CssValue(ECssValueType.NONE);

        public static CssValue Zero = CssValue.From_Int(0);
        public static CssValue CurrentColor = CssValue.From_Keyword("currentColor");
        /// <summary> 100% </summary>
        public static CssValue Pct_OneHundred = CssValue.From_Percent(100.0);
        #endregion

        #region Properties
        public readonly ECssValueFlags Flags = ECssValueFlags.None;
        public readonly ECssValueType Type = ECssValueType.NULL;
        public readonly dynamic Value = null;
        public readonly ECssUnit Unit = ECssUnit.None;
        #endregion

        #region Accessors
        public bool IsAuto => (this.Type == ECssValueType.AUTO);
        /// <summary>
        /// Returns whether or our value is a definite Number or Integer
        /// </summary>
        public bool IsDefinite => (0 != (this.Type & (ECssValueType.INTEGER | ECssValueType.NUMBER)));

        /// <summary>
        /// Returns whether the value type is <see cref="ECssValueType.NULL"/>
        /// </summary>
        public bool IsNull => (this.Type == ECssValueType.NULL);

        /// <summary>
        /// Returns whether there is actually a set value
        /// </summary>
        public bool HasValue
        {
            get
            {
                if (this.Type == ECssValueType.NULL)
                    return false;

                if (ReferenceEquals(this.Value, null))
                    return false;

                return true;
            }
        }
        #endregion

        #region Constructors
        internal CssValue(CssFunction function)
        {
            this.Type = ECssValueType.FUNCTION;
            this.Value = function;
        }

        private CssValue(ECssValueType Type)
        {
            this.Type = Type;

            switch (Type)
            {
                // NOTE: Auto values get calculated in vastly different ways, sometimes this doesn't even indicate that a value depends on the value of other properties,
                //so it should NOT get the 'Depends' flag
                case ECssValueType.INHERIT:// Inherited values function like redirects which compute to the current value of the matching property for the owning element's parent
                case ECssValueType.PERCENT:// Percentage values represent a percentage of another property's value
                    Flags = ECssValueFlags.Depends;
                    break;
                case ECssValueType.INTEGER:
                case ECssValueType.NUMBER:
                case ECssValueType.STRING:
                case ECssValueType.COLOR:
                    Flags = ECssValueFlags.Absolute;
                    break;
                case ECssValueType.UNSET:
                case ECssValueType.AUTO:
                case ECssValueType.DIMENSION:
                    {
                        /* XXX:
                         * These values when used on properties CAN be dependant but arent always so idk maybe its best to leave them as absolute?
                         * Maybe we can come up with some sort of intermediate state that causes them to resolve their flag to absolute/dependent AFTER they get assigned to a property?
                         */
                        Flags = ECssValueFlags.Absolute;
                    }
                    break;
            }
        }

        internal CssValue(ECssValueType Type, dynamic Value = null) : this(Type)
        {
            this.Value = Value;
        }

        internal CssValue(ECssValueType Type, dynamic Value, ECssUnit Unit) : this(Type)
        {
            this.Unit = Unit;
            this.Value = Value;

            if (Type == ECssValueType.KEYWORD)// Try and catch some common IMPORTANT keywords
            {
                /* If our keyword can be resolved to another ECssValueType then its an global keyword */

                // ECssValueType? keywordType = CssLookup.Enum_From_Keyword<ECssValueType>(((string)Value).ToLowerInvariant());
                ECssValueType? keywordType = CssLookup.Enum_From_Keyword<ECssValueType>(Value as string);
                if (keywordType.HasValue)
                {
                    this.Type = keywordType.Value;
                }
            }
            else if (Type == ECssValueType.DIMENSION || Type == ECssValueType.RESOLUTION)
            {
                /* Make sure to correct the value and distinguish whether we are a Dimension or a Resolution */
                switch (Unit)
                {
                    case ECssUnit.DPI:
                    case ECssUnit.DPCM:
                    case ECssUnit.DPPX:
                        this.Type = ECssValueType.RESOLUTION;
                        break;
                    default:
                        this.Type = ECssValueType.DIMENSION;
                        break;
                }
            }
        }

        /// <summary>
        /// Clones an already existing <see cref="CssValue"/>
        /// </summary>
        internal CssValue(CssValue sv)
        {
            Type = sv.Type;
            Value = sv.Value;
            Unit = sv.Unit;
            Flags = sv.Flags;
        }
        #endregion

        #region Instantiation
        /// <summary> Returns a new <see cref="CssValue"/> instance which is a copy of this one. </summary>
        public CssValue Clone() => new CssValue(this);
        
        /// <summary>Create an integer value from an enum</summary>
        public static CssValue From_Enum<Ty>(Ty value) where Ty: struct => new CssValue(ECssValueType.KEYWORD, CssLookup.Keyword_From_Enum<Ty>(value));

        /// <summary>Create an absolute integer value</summary>
        public static CssValue From_Int(int value) => new CssValue(ECssValueType.INTEGER, value);

        /// <summary>Create an absolute integer value if not null, or return the given default value</summary>
        public static CssValue From_Int(int? value, CssValue defaultValue) => (!value.HasValue ? defaultValue : new CssValue(ECssValueType.INTEGER, value.Value));
        
        /// <summary>Create an absolute number value</summary>
        public static CssValue From_Number(double value) => new CssValue(ECssValueType.NUMBER, (double)value);

        /// <summary>Create an absolute number value if not null, or return the given default value</summary>
        public static CssValue From_Number(double? value, CssValue defaultValue) => (!value.HasValue ? defaultValue : new CssValue(ECssValueType.NUMBER, (double)value.Value));
        
        /// <summary>Create a percentage value</summary>
        /// <param name="value">Floating-point value in the range [0 - 100]</param>
        public static CssValue From_Percent(double value) => new CssValue(ECssValueType.PERCENT, (double)value);

        /// <summary>Create an absolute length value</summary>
        public static CssValue From_Length(double value, ECssUnit Unit) => new CssValue(ECssValueType.DIMENSION, (double)value, Unit);

        /// <summary>Create an absolute length value if not null, or return the given default value</summary>
        public static CssValue From_Length(double? value, ECssUnit Unit, CssValue defaultValue) => (!value.HasValue ? defaultValue : new CssValue(ECssValueType.DIMENSION, (double)value.Value, Unit));

        

        /// <summary>Create an RGBA color value</summary>
        public static CssValue From_Color(cssColor value) => new CssValue(ECssValueType.COLOR, value);

        /// <summary>Create a string value</summary>
        public static CssValue From_String(string value) => new CssValue(ECssValueType.STRING, value);

        /// <summary>Create a keyword value</summary>
        public static CssValue From_Keyword(string value) => new CssValue(ECssValueType.KEYWORD, value);

        /// <summary>Create a css-value by parsing the given string as CSS markup</summary>
        public static CssValue From_CSS(string css) => new CssParser(css).Parse_CssValue();

        #endregion

        #region ToString
        public override string ToString()
        {
            switch (Type)
            {
                case ECssValueType.INTEGER:
                    return string.Concat((int)Value);
                case ECssValueType.NUMBER:
                    return string.Concat(((double)Value).ToString("0.###"));
                case ECssValueType.DIMENSION:
                    if (Unit == ECssUnit.None)
                        return string.Concat(((double)Value).ToString("0.###"), "<none>");
                    else
                        return string.Concat(((double)Value).ToString("0.###"), Unit.ToString().ToLower());
                case ECssValueType.PERCENT:
                    return string.Concat(((double)Value).ToString("0.###"), "%");
                case ECssValueType.STRING:
                    return Value as string;
                default:
                    return string.Concat("[", Enum.GetName(typeof(ECssValueType), Type), "]");
            }
        }
        #endregion

        #region HasFlags
        public bool Has_Flags(ECssValueFlags Flags) { return (this.Flags & Flags) != 0; }
        #endregion

        #region Dimension Resolution
        /// <summary>
        /// Gets the final resulting value for a dimension type by multiplying our current numeric value by the unit scale
        /// </summary>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private double ResolveDimension(StyleUnitResolverDelegate Resolver)
        {
            if (Unit == ECssUnit.None) return (double)Value;
            return (Resolver(Unit) * (double)Value);
        }
        #endregion

        #region Generic Resolution

        /// <summary>
        /// Resolves the value to a decimal and returns it if possible, returns NULL otherwise
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public double? Resolve(StyleUnitResolverDelegate UnitResolver)
        {
            switch (Type)
            {
                case ECssValueType.INTEGER:
                    return (int)Value;
                case ECssValueType.NUMBER:
                    return (double)Value;
                case ECssValueType.DIMENSION:
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
            double? num = Resolve(UnitResolver);
            return (num.HasValue ? num.Value : defaultValue);
        }


        /// <summary>
        /// If this instance matches the given Predicate then resolves the value to a decimal and returns it, returns NULL otherwise
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public double? Resolve(StyleUnitResolverDelegate UnitResolver, Func<CssValue, bool> Predicate)
        {
            if (Predicate(this))
            {
                switch (Type)
                {
                    case ECssValueType.INTEGER:
                        return (int)Value;
                    case ECssValueType.NUMBER:
                        return (double)Value;
                    case ECssValueType.DIMENSION:
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
            switch (Type)
            {
                case ECssValueType.INTEGER:
                    return (int)Value;
                case ECssValueType.NUMBER:
                    return (double)Value;
                case ECssValueType.DIMENSION:
                    return ResolveDimension(UnitResolver);
                case ECssValueType.PERCENT:
                    return (((double)Value / 100.0) * percentageMultiplier);
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
            switch (Type)
            {
                case ECssValueType.INTEGER:
                    return (int)Value;
                case ECssValueType.NUMBER:
                    return (double)Value;
                case ECssValueType.DIMENSION:
                    return ResolveDimension(UnitResolver);
                case ECssValueType.PERCENT:
                    return percentageResolver(((double)Value / 100.0));
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
            double? num = Resolve(UnitResolver, percentageResolver);
            return (num.HasValue ? num.Value : defaultValue);
        }
        #endregion

        #region Resolution To Integer

        /// <summary>
        /// If this instance matches the given Predicate then resolves the value to an integer and returns it, returns defaultValue otherwise
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int Resolve_Or_Default(StyleUnitResolverDelegate UnitResolver, int defaultValue, Func<CssValue, bool> Predicate)
        {
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
            int? num = (int?)Resolve(UnitResolver, percentageResolver);
            return (num.HasValue ? num.Value : defaultValue);
        }
        #endregion

        #region Non-Unit Resolution

        /// <summary>
        /// Resolves the value to a decimal and returns it if possible, returns NULL otherwise
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public double? Resolve()
        {
            switch (Type)
            {
                case ECssValueType.INTEGER:
                    return (int)Value;
                case ECssValueType.NUMBER:
                    return (double)Value;
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
                case ECssValueType.INTEGER:
                    return (int)Value;
                case ECssValueType.NUMBER:
                    return (double)Value;
                case ECssValueType.PERCENT:
                    return (((double)Value / 100.0) * percentageMultiplier);
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
            if (Predicate(this))
            {
                switch (Type)
                {
                    case ECssValueType.INTEGER:
                        return (int)Value;
                    case ECssValueType.NUMBER:
                        return (double)Value;
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
                case ECssValueType.INTEGER:
                    return (int)Value;
                case ECssValueType.NUMBER:
                    return (double)Value;
                case ECssValueType.PERCENT:
                    return (((double)Value / 100.0) * percentageMultiplier);
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
            switch (Type)
            {
                case ECssValueType.INTEGER:
                    return (int)Value;
                case ECssValueType.NUMBER:
                    return (double)Value;
                case ECssValueType.PERCENT:
                    return percentageResolver(((double)Value / 100.0));
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
            switch (Type)
            {
                case ECssValueType.INTEGER:
                    return (int)Value;
                case ECssValueType.NUMBER:
                    return (double)Value;
                case ECssValueType.PERCENT:
                    return percentageResolver(((double)Value / 100.0));
            }

            return defaultValue;
        }
        #endregion
        
        #region Non-Unit Resolution To Integer

        /// <summary>
        /// If this instance matches the given Predicate then resolves the value to an integer and returns it, returns defaultValue otherwise
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int Resolve_Or_Default(int defaultValue, Func<CssValue, bool> Predicate)
        {
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
            int? num = (int?)Resolve(percentageResolver);
            return (num.HasValue ? num.Value : defaultValue);
        }
        #endregion
        
        #region Color Resolution
        /// <summary>
        /// Returns the value as a Color4 if possible, or NULL if not possible.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public cssColor AsColor()
        {
            switch (Type)
            {
                case ECssValueType.COLOR:
                    return ((cssColor)Value);
                default:
                    return null;
            }
        }
        #endregion

        #region AsString

        /// <summary>
        /// Returns the value as a CSS string if possible, or NULL.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public string AsString()
        {
            switch (Type)
            {
                case ECssValueType.INTEGER:
                case ECssValueType.NUMBER:
                    return string.Concat(Value);
                case ECssValueType.DIMENSION:
                    {
                        if (Unit == ECssUnit.None)
                            return string.Concat(((double)Value).ToString());
                        else
                            return string.Concat(((double)Value).ToString(), Enum.GetName(typeof(ECssUnit), Unit).ToLower());
                    }
                case ECssValueType.PERCENT:
                    return string.Concat((double)Value, "%");
                case ECssValueType.COLOR:
                    return ((cssColor)Value).ToCssString();
                case ECssValueType.STRING:
                    return ((string)Value);
                default:
                    return null;
            }
        }
        #endregion

        #region AsEnum
        public Ty AsEnum<Ty>() where Ty : struct, IConvertible
        {
            return (Ty)Value;
        }
        #endregion

        #region Operators
        public static bool operator ==(CssValue A, CssValue B)
        {
            // If either object is null return whether they are BOTH null
            if (object.ReferenceEquals(A, null) || object.ReferenceEquals(B, null))
                return (object.ReferenceEquals(A, null) && object.ReferenceEquals(B, null));

            if (A.Type != B.Type) return false;
            if (A.Unit != B.Unit) return false;

            switch (A.Type)
            {
                case ECssValueType.NULL:
                case ECssValueType.UNSET:
                case ECssValueType.AUTO:
                case ECssValueType.INITIAL:
                case ECssValueType.INHERIT:
                case ECssValueType.NONE:
                    return true;
                case ECssValueType.INTEGER:
                case ECssValueType.COLOR:
                    return EqualityComparer<int>.Default.Equals(A.Value, B.Value);
                case ECssValueType.NUMBER:
                case ECssValueType.DIMENSION:
                case ECssValueType.PERCENT:
                    return EqualityComparer<double>.Default.Equals(A.Value, B.Value);
                case ECssValueType.STRING:
                case ECssValueType.KEYWORD:
                    return EqualityComparer<string>.Default.Equals(A.Value, B.Value);
                default:
                    throw new NotImplementedException(string.Concat("Equality comparison logic not implemented for type: ", Enum.GetName(typeof(ECssValueType), A.Type)));
            }
        }

        public static bool operator !=(CssValue A, CssValue B)
        {
            return !(A == B);
        }


        public override bool Equals(object o)
        {
            if (ReferenceEquals(o, null))
                return false;

            if (o is CssValue)
            {
                return this == (CssValue)o;
            }

            return false;
        }

        public override int GetHashCode()
        {
            return this.ToString().GetHashCode();
        }

        #endregion
    }

}
