using CssUI.CSS.Parser;
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
        public static CssValue Auto = new CssValue(ECssDataType.AUTO);
        /// <summary>
        /// (Non-Cascadeable)
        /// Treat as nothing. Eg; The stylesheet didnt specify anything for this property.
        /// </summary>
        public static CssValue Null = new CssValue(ECssDataType.NULL);
        /// <summary>
        /// (Non-Cascadeable)
        /// Intentionally unsets a property, forcing it to resolve to either its inherited or initial value.
        /// DOCS: https://www.w3.org/TR/css-cascade-3/#inherit-initial
        /// </summary>
        public static CssValue Unset = new CssValue(ECssDataType.UNSET);
        /// <summary>
        /// Value should resolve to it's definitions default value
        /// </summary>
        public static CssValue Initial = new CssValue(ECssDataType.INITIAL);
        /// <summary>
        /// Value is inherited from the parent element
        /// </summary>
        public static CssValue Inherit = new CssValue(ECssDataType.INHERIT);
        /// <summary>
        /// (Cascades)
        /// Value is purposly nothing, No value is assigned, some properties use this state to be ignored.
        /// As in they wont have an effect on the elements styling or block
        /// </summary>
        public static CssValue None = new CssValue(ECssDataType.NONE);

        public static CssValue Zero = CssValue.From_Int(0);
        public static CssValue CurrentColor = CssValue.From_Keyword("currentColor");
        /// <summary> 100% </summary>
        public static CssValue Pct_OneHundred = CssValue.From_Percent(100.0);
        #endregion

        #region Properties
        public readonly ECssValueFlags Flags = ECssValueFlags.None;
        public readonly ECssDataType Type = ECssDataType.NULL;
        public readonly dynamic Value = null;
        public readonly ECssUnit Unit = ECssUnit.None;
        #endregion

        #region Accessors
        public bool IsAuto => (this.Type == ECssDataType.AUTO);
        /// <summary>
        /// Returns whether or our value is a definite Number or Integer
        /// </summary>
        public bool IsDefinite => (0 != (this.Type & (ECssDataType.INTEGER | ECssDataType.NUMBER)));

        /// <summary>
        /// Returns whether the value type is <see cref="ECssDataType.NULL"/>
        /// </summary>
        public bool IsNull => (this.Type == ECssDataType.NULL);

        /// <summary>
        /// Returns whether there is actually a set value
        /// </summary>
        public bool HasValue
        {
            get
            {
                if (this.Type == ECssDataType.NULL)
                    return false;

                if (ReferenceEquals(this.Value, null))
                    return false;

                return true;
            }
        }
        #endregion

        /// <summary>
        /// Interprets the given string as a dimension specifier
        /// </summary>
        /// <param name="ValueStr"></param>
        /// <param name="DataType"></param>
        /// <param name="UnitType"></param>
        /// <param name="newValue"></param>
        /// <returns>Success</returns>
        bool Get_Dimension_From_String(string ValueStr, out ECssDataType DataType, out ECssUnit UnitType, out dynamic newValue)
        {
            DataType = ECssDataType.NONE;
            UnitType = ECssUnit.None;
            newValue = null;

            CssTokenizer Parser = new CssTokenizer(ValueStr);
            if (Parser[0].Type == ECssTokenType.Dimension)
            {
                DimensionToken tok = (DimensionToken)Parser[0];
                DataType = ECssDataType.NUMBER;
                newValue = tok.Number;
                UnitType = (ECssUnit)Enum.Parse(typeof(ECssUnit), tok.Unit.ToUpper());
                if (tok.DataType == ENumericTokenType.Integer) DataType = ECssDataType.INTEGER;
                return true;
            }

            return false;
        }

        #region Constructors
        private CssValue(ECssDataType Type)
        {
            this.Type = Type;

            switch (Type)
            {
                // NOTE: Auto values get calculated in vastly different ways, sometimes this doesn't even indicate that a value depends on the value of other properties,
                //so it should NOT get the 'Depends' flag
                //case EStyleDataType.AUTO:
                case ECssDataType.INHERIT:// Inherited values function like redirects which compute to the current value of the matching property for the owning element's parent
                case ECssDataType.PERCENT:// Percentage values represent a percentage of another property's value
                    Flags = ECssValueFlags.Depends;
                    break;
                case ECssDataType.INTEGER:
                case ECssDataType.NUMBER:
                case ECssDataType.DIMENSION:// XXX: This is a double edged sword, some unit values DO depend on other value states BUT those values are external and get resolved once we query this sooo....
                case ECssDataType.COLOR:
                case ECssDataType.STRING:
                    Flags = ECssValueFlags.Absolute;
                    break;
            }
        }

        public CssValue(ECssDataType Type, dynamic Value, ECssUnit Unit) : this(Type)
        {
            this.Unit = Unit;
            this.Value = Value;
        }

        public CssValue(ECssDataType Type, dynamic Value = null, bool parseUnitType = false) : this(Type)
        {
            this.Value = Value;
            if (parseUnitType && Type != ECssDataType.STRING && Value is string)
            {// Find which unit type our string value was given
                string vStr = (string)Value;
                ECssDataType dataTy;
                ECssUnit unitTy;
                dynamic NewValue;
                if (Get_Dimension_From_String(vStr, out dataTy, out unitTy, out NewValue))
                {// for lengths our value can only be stored as either an Integer or a Number
                    this.Unit = unitTy;
                    this.Value = (double)NewValue;
                }
            }
        }

        /// <summary>
        /// Clones an already existing <see cref="CssValue"/>
        /// </summary>
        public CssValue(CssValue sv)
        {
            Type = sv.Type;
            Value = sv.Value;
            Unit = sv.Unit;
            Flags = sv.Flags;
        }

        
        /// <summary>Create an integer value from an enum</summary>
        public static CssValue From_Enum<Ty>(Ty value) where Ty: struct {
            /* XXX: set value to enums CssKeyword */
            return new CssValue(ECssDataType.KEYWORD, CssLookup.Enum<Ty>(value));
        }

        /// <summary>Create an absolute integer value</summary>
        public static CssValue From_Int(int value) { return new CssValue(ECssDataType.INTEGER, value); }

        /// <summary>Create an absolute integer value if not null, or return the given default value</summary>
        public static CssValue From_Int(int? value, CssValue defaultValue) { return (!value.HasValue ? defaultValue : new CssValue(ECssDataType.INTEGER, value.Value)); }
        
        /// <summary>Create an absolute number value</summary>
        public static CssValue From_Number(double value) { return new CssValue(ECssDataType.NUMBER, (double)value); }

        /// <summary>Create an absolute number value if not null, or return the given default value</summary>
        public static CssValue From_Number(double? value, CssValue defaultValue) { return (!value.HasValue ? defaultValue : new CssValue(ECssDataType.NUMBER, (double)value.Value)); }
        
        /// <summary>Create a percentage value</summary>
        /// <param name="value">Floating-point value in the range [0 - 100]</param>
        public static CssValue From_Percent(double value) { return new CssValue(ECssDataType.PERCENT, (double)value); }



        /// <summary>Create an absolute length value</summary>
        public static CssValue From_Length(string value) { return new CssValue(ECssDataType.DIMENSION, (string)value, true); }

        /// <summary>Create an absolute length value</summary>
        public static CssValue From_Length(double value, ECssUnit Unit) { return new CssValue(ECssDataType.DIMENSION, (double)value, Unit); }

        /// <summary>Create an absolute length value if not null, or return the given default value</summary>
        public static CssValue From_Length(double? value, ECssUnit Unit, CssValue defaultValue) { return (!value.HasValue ? defaultValue : new CssValue(ECssDataType.DIMENSION, (double)value.Value, Unit)); }

        

        /// <summary>Create an RGBA color value</summary>
        public static CssValue From_Color(cssColor value) { return new CssValue(ECssDataType.COLOR, value); }

        /// <summary>Create a string value</summary>
        public static CssValue From_String(string value) { return new CssValue(ECssDataType.STRING, value); }

        /// <summary>Create a keyword value</summary>
        public static CssValue From_Keyword(string value) { return new CssValue(ECssDataType.KEYWORD, value); }

        #endregion

        #region ToString
        public override string ToString()
        {
            switch (Type)
            {
                case ECssDataType.INTEGER:
                    return string.Concat((int)Value);
                case ECssDataType.NUMBER:
                    return string.Concat(((double)Value).ToString("0.###"));
                case ECssDataType.DIMENSION:
                    if (Unit == ECssUnit.None)
                        return string.Concat(((double)Value).ToString("0.###"), "<none>");
                    else
                        return string.Concat(((double)Value).ToString("0.###"), Unit.ToString().ToLower());
                case ECssDataType.PERCENT:
                    return string.Concat(((double)Value).ToString("0.###"), "%");
                case ECssDataType.STRING:
                    return Value as string;
                default:
                    return string.Concat("[", Enum.GetName(typeof(ECssDataType), Type), "]");
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
                case ECssDataType.INTEGER:
                    return (int)Value;
                case ECssDataType.NUMBER:
                    return (double)Value;
                case ECssDataType.DIMENSION:
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
                    case ECssDataType.INTEGER:
                        return (int)Value;
                    case ECssDataType.NUMBER:
                        return (double)Value;
                    case ECssDataType.DIMENSION:
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
                case ECssDataType.INTEGER:
                    return (int)Value;
                case ECssDataType.NUMBER:
                    return (double)Value;
                case ECssDataType.DIMENSION:
                    return ResolveDimension(UnitResolver);
                case ECssDataType.PERCENT:
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
                case ECssDataType.INTEGER:
                    return (int)Value;
                case ECssDataType.NUMBER:
                    return (double)Value;
                case ECssDataType.DIMENSION:
                    return ResolveDimension(UnitResolver);
                case ECssDataType.PERCENT:
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
                case ECssDataType.INTEGER:
                    return (int)Value;
                case ECssDataType.NUMBER:
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
                case ECssDataType.INTEGER:
                    return (int)Value;
                case ECssDataType.NUMBER:
                    return (double)Value;
                case ECssDataType.PERCENT:
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
                    case ECssDataType.INTEGER:
                        return (int)Value;
                    case ECssDataType.NUMBER:
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
                case ECssDataType.INTEGER:
                    return (int)Value;
                case ECssDataType.NUMBER:
                    return (double)Value;
                case ECssDataType.PERCENT:
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
                case ECssDataType.INTEGER:
                    return (int)Value;
                case ECssDataType.NUMBER:
                    return (double)Value;
                case ECssDataType.PERCENT:
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
                case ECssDataType.INTEGER:
                    return (int)Value;
                case ECssDataType.NUMBER:
                    return (double)Value;
                case ECssDataType.PERCENT:
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
                case ECssDataType.COLOR:
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
                case ECssDataType.INTEGER:
                case ECssDataType.NUMBER:
                    return string.Concat(Value);
                case ECssDataType.DIMENSION:
                    {
                        if (Unit == ECssUnit.None)
                            return string.Concat(((double)Value).ToString());
                        else
                            return string.Concat(((double)Value).ToString(), Enum.GetName(typeof(ECssUnit), Unit).ToLower());
                    }
                case ECssDataType.PERCENT:
                    return string.Concat((double)Value, "%");
                case ECssDataType.COLOR:
                    return ((cssColor)Value).ToCssString();
                case ECssDataType.STRING:
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
                case ECssDataType.NULL:
                case ECssDataType.UNSET:
                case ECssDataType.AUTO:
                case ECssDataType.INITIAL:
                case ECssDataType.INHERIT:
                case ECssDataType.NONE:
                    return true;
                case ECssDataType.INTEGER:
                case ECssDataType.COLOR:
                    return EqualityComparer<int>.Default.Equals(A.Value, B.Value);
                case ECssDataType.NUMBER:
                case ECssDataType.DIMENSION:
                case ECssDataType.PERCENT:
                    return EqualityComparer<double>.Default.Equals(A.Value, B.Value);
                case ECssDataType.STRING:
                case ECssDataType.KEYWORD:
                    return EqualityComparer<string>.Default.Equals(A.Value, B.Value);
                default:
                    throw new NotImplementedException(string.Concat("Equality comparison logic not implemented for type: ", Enum.GetName(typeof(ECssDataType), A.Type)));
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
