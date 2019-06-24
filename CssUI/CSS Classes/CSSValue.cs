using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace CssUI.CSS
{
    /// <summary>
    /// Represents a CSS Value
    /// </summary>
    public class CssValue
    {
        #region Delegates
        public delegate double StyleUnitResolver(EStyleUnit unit);
        #endregion

        #region Static Declerations
        /// <summary>
        /// Value depends on other properties
        /// </summary>
        public static CssValue Auto = new CssValue(EStyleDataType.AUTO);
        /// <summary>
        /// (Non-Cascadeable)
        /// Treat as if no value were given. Eg; The element might as well not have this property at all.
        /// Properties using this value will be ignored during cascade logic.
        /// </summary>
        public static CssValue Null = new CssValue(EStyleDataType.UNSET);
        /// <summary>
        /// Value should resolve to it's default
        /// </summary>
        public static CssValue Initial = new CssValue(EStyleDataType.INITIAL);
        /// <summary>
        /// Value is inherited from the owning elements parent peoperty of the same name
        /// </summary>
        public static CssValue Inherit = new CssValue(EStyleDataType.INHERIT);
        /// <summary>
        /// (Cascades)
        /// Value is purposly nothing, No value is assigned, some properties use this state to be ignored.
        /// </summary>
        public static CssValue None = new CssValue(EStyleDataType.NONE);
        public static CssValue Zero = CssValue.From_Int(0);
        public static CssValue CurrentColor = CssValue.From_String("currentColor");
        /// <summary> 100% </summary>
        public static CssValue Pct_OneHundred = CssValue.From_Percent(100.0);
        #endregion

        #region Properties
        public readonly StyleValueFlags Flags = StyleValueFlags.None;
        public readonly EStyleDataType Type = EStyleDataType.UNSET;
        public readonly dynamic Value = null;
        public readonly EStyleUnit Unit = EStyleUnit.None;

        /// <summary>
        /// If this value was formed from a string representation then this is that original string
        /// </summary>
        string Str = null;
        #endregion

        /// <summary>
        /// Interprets the given string as a dimension specifier
        /// </summary>
        /// <param name="ValueStr"></param>
        /// <param name="DataType"></param>
        /// <param name="UnitType"></param>
        /// <param name="newValue"></param>
        /// <returns>Success</returns>
        bool Get_Dimension_From_String(string ValueStr, out EStyleDataType DataType, out EStyleUnit UnitType, out dynamic newValue)
        {
            DataType = EStyleDataType.NONE;
            UnitType = EStyleUnit.None;
            newValue = null;

            CssTokenizer Parser = new CssTokenizer(ValueStr);
            if (Parser[0].Type == ECssTokenType.Dimension)
            {
                DimensionToken tok = (DimensionToken)Parser[0];
                DataType = EStyleDataType.NUMBER;
                newValue = tok.Number;
                UnitType = (EStyleUnit)Enum.Parse(typeof(EStyleUnit), tok.Unit.ToUpper());
                if (tok.DataType == ENumericTokenType.Integer) DataType = EStyleDataType.INTEGER;
                return true;
            }

            return false;
        }

        #region Constructors
        private CssValue(EStyleDataType Type)
        {
            this.Type = Type;

            switch (Type)
            {
                //case SVType.AUTO:// Auto values get calculated in vastly different ways, sometimes this doesn't even indicate that a value depends on the value of other properties, so it shouldnt get the 'Depends' flag
                case EStyleDataType.INHERIT:// Inherited values function like redirects which compute to the current value of the matching property for the owning element's parent
                case EStyleDataType.PERCENT:// Percentage values represent a percentage of another property's value
                    Flags = StyleValueFlags.Depends;
                    break;
                case EStyleDataType.INTEGER:
                case EStyleDataType.NUMBER:
                case EStyleDataType.DIMENSION:
                case EStyleDataType.COLOR:
                case EStyleDataType.STRING:
                    Flags = StyleValueFlags.Absolute;
                    break;
            }
        }

        public CssValue(EStyleDataType Type, dynamic Value, EStyleUnit Unit) : this(Type)
        {
            this.Unit = Unit;
            this.Value = Value;
        }

        public CssValue(EStyleDataType Type, dynamic Value = null, bool parseUnitType = false) : this(Type)
        {
            this.Value = Value;
            if (parseUnitType && Type != EStyleDataType.STRING && Value is string)
            {// Find which unit type our string value was given
                string vStr = (string)Value;
                EStyleDataType dataTy;
                EStyleUnit unitTy;
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
        public static CssValue From_Enum(object value) { return new CssValue(EStyleDataType.INTEGER, (int)value); }

        /// <summary>Create an absolute integer value</summary>
        public static CssValue From_Int(int value) { return new CssValue(EStyleDataType.INTEGER, value); }

        /// <summary>Create an absolute integer value if not null, or return the given default value</summary>
        public static CssValue From_Int(int? value, CssValue defaultValue) { return (!value.HasValue ? defaultValue : new CssValue(EStyleDataType.INTEGER, value.Value)); }
        
        /// <summary>Create an absolute number value</summary>
        public static CssValue From_Number(double value) { return new CssValue(EStyleDataType.NUMBER, (double)value); }

        /// <summary>Create an absolute number value if not null, or return the given default value</summary>
        public static CssValue From_Number(double? value, CssValue defaultValue) { return (!value.HasValue ? defaultValue : new CssValue(EStyleDataType.NUMBER, (double)value.Value)); }
        
        /// <summary>Create a percentage value</summary>
        /// <param name="value">Floating-point value in the range [0 - 100]</param>
        public static CssValue From_Percent(double value) { return new CssValue(EStyleDataType.PERCENT, (double)value); }



        /// <summary>Create an absolute length value</summary>
        public static CssValue From_Length(string value) { return new CssValue(EStyleDataType.DIMENSION, (string)value, true); }

        /// <summary>Create an absolute length value</summary>
        public static CssValue From_Length(double value, EStyleUnit Unit) { return new CssValue(EStyleDataType.DIMENSION, (double)value, Unit); }

        /// <summary>Create an absolute length value if not null, or return the given default value</summary>
        public static CssValue From_Length(double? value, EStyleUnit Unit, CssValue defaultValue) { return (!value.HasValue ? defaultValue : new CssValue(EStyleDataType.DIMENSION, (double)value.Value, Unit)); }

        

        /// <summary>Create an RGBA color value</summary>
        public static CssValue From_Color(cssColor value) { return new CssValue(EStyleDataType.COLOR, value); }

        /// <summary>Create a string value</summary>
        public static CssValue From_String(string value) { return new CssValue(EStyleDataType.STRING, value); }

        #endregion

        #region ToString
        public override string ToString()
        {
            switch (Type)
            {
                case EStyleDataType.INTEGER:
                    return string.Concat((int)Value);
                case EStyleDataType.NUMBER:
                    return string.Concat(((double)Value).ToString("0.###"));
                case EStyleDataType.DIMENSION:
                    if (Unit == EStyleUnit.None)
                        return string.Concat(((double)Value).ToString("0.###"), "<none>");
                    else
                        return string.Concat(((double)Value).ToString("0.###"), Unit.ToString().ToLower());
                case EStyleDataType.PERCENT:
                    return string.Concat(((double)Value).ToString("0.###"), "%");
                case EStyleDataType.STRING:
                    return Str;
                default:
                    return string.Concat("[", Enum.GetName(typeof(EStyleDataType), Type), "]");
            }
        }
        #endregion

        #region HasFlags
        public bool Has_Flags(StyleValueFlags Flags) { return (this.Flags & Flags) != 0; }
        #endregion


        #region Dimension Resolution
        /// <summary>
        /// Gets the final resulting value for a dimension type by multiplying our current numeric value by the unit scale
        /// </summary>
        /// <returns></returns>
        private double ResolveDimension(StyleUnitResolver Resolver)
        {
            if (Unit == EStyleUnit.None) return (double)Value;
            return (Resolver(Unit) * (double)Value);
        }
        #endregion

        #region Generic Resolution

        /// <summary>
        /// Resolves the value to a decimal and returns it if possible, returns NULL otherwise
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public double? Resolve(StyleUnitResolver UnitResolver)
        {
            switch (Type)
            {
                case EStyleDataType.INTEGER:
                    return (int)Value;
                case EStyleDataType.NUMBER:
                    return (double)Value;
                case EStyleDataType.DIMENSION:
                    return ResolveDimension(UnitResolver);
                default:
                    return null;
            }
        }

        /// <summary>
        /// Resolves the value to a decimal and returns it if possible, returns defaultValue otherwise
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public double Resolve_Or_Default(StyleUnitResolver UnitResolver, double defaultValue)
        {
            double? num = Resolve(UnitResolver);
            return (num.HasValue ? num.Value : defaultValue);
        }


        /// <summary>
        /// If this instance matches the given Predicate then resolves the value to a decimal and returns it, returns NULL otherwise
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public double? Resolve(StyleUnitResolver UnitResolver, Func<CssValue, bool> Predicate)
        {
            if (Predicate(this))
            {
                switch (Type)
                {
                    case EStyleDataType.INTEGER:
                        return (int)Value;
                    case EStyleDataType.NUMBER:
                        return (double)Value;
                    case EStyleDataType.DIMENSION:
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
        public double Resolve_Or_Default(StyleUnitResolver UnitResolver, double defaultValue, Func<CssValue, bool> Predicate)
        {
            double? num = Resolve(UnitResolver, Predicate);
            return (num.HasValue ? num.Value : defaultValue);
        }
        
        /// <summary>
        /// Resolves the value to a decimal and returns it if possible, returns NULL otherwise
        /// <para>Additionally takes an multiplier as input for resolving the value to a decimal if it's a percentage type</para>
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public double? Resolve(StyleUnitResolver UnitResolver, double percentageMultiplier)
        {
            switch (Type)
            {
                case EStyleDataType.INTEGER:
                    return (int)Value;
                case EStyleDataType.NUMBER:
                    return (double)Value;
                case EStyleDataType.DIMENSION:
                    return ResolveDimension(UnitResolver);
                case EStyleDataType.PERCENT:
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
        public double Resolve_Or_Default(StyleUnitResolver UnitResolver, double percentageMultiplier, double defaultValue)
        {
            double? num = Resolve(UnitResolver, percentageMultiplier);
            return (num.HasValue ? num.Value : defaultValue);
        }

        /// <summary>
        /// Resolves the value to a decimal and returns it if possible, returns NULL otherwise
        /// <para>Additionally takes an action as input for resolving the value to a decimal if it's a percentage type</para>
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public double? Resolve(StyleUnitResolver UnitResolver, Func<double, double> percentageResolver)
        {
            switch (Type)
            {
                case EStyleDataType.INTEGER:
                    return (int)Value;
                case EStyleDataType.NUMBER:
                    return (double)Value;
                case EStyleDataType.DIMENSION:
                    return ResolveDimension(UnitResolver);
                case EStyleDataType.PERCENT:
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
        public double Resolve_Or_Default(StyleUnitResolver UnitResolver, Func<double, double> percentageResolver, double defaultValue)
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
        public int Resolve_Or_Default(StyleUnitResolver UnitResolver, int defaultValue, Func<CssValue, bool> Predicate)
        {
            int? num = (int?)Resolve(UnitResolver, Predicate);
            return (num.HasValue ? num.Value : defaultValue);
        }

        /// <summary>
        /// Resolves the value to an integer and returns it if possible, returns defaultValue otherwise
        /// <para>Additionally takes an multiplier as input for resolving the value to a decimal if it's a percentage type</para>
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int Resolve_Or_Default(StyleUnitResolver UnitResolver, double percentageMultiplier, int defaultValue)
        {
            int? num = (int?)Resolve(UnitResolver, percentageMultiplier);
            return (num.HasValue ? num.Value : defaultValue);
        }

        /// <summary>
        /// Resolves the value to an integer and returns it if possible, returns defaultValue otherwise
        /// <para>Additionally takes an action as input for resolving the value to a decimal if it's a percentage type</para>
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int Resolve_Or_Default(StyleUnitResolver UnitResolver, Func<double, double> percentageResolver, int defaultValue)
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
                case EStyleDataType.INTEGER:
                    return (int)Value;
                case EStyleDataType.NUMBER:
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
                case EStyleDataType.INTEGER:
                    return (int)Value;
                case EStyleDataType.NUMBER:
                    return (double)Value;
                case EStyleDataType.PERCENT:
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
                    case EStyleDataType.INTEGER:
                        return (int)Value;
                    case EStyleDataType.NUMBER:
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
                case EStyleDataType.INTEGER:
                    return (int)Value;
                case EStyleDataType.NUMBER:
                    return (double)Value;
                case EStyleDataType.PERCENT:
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
                case EStyleDataType.INTEGER:
                    return (int)Value;
                case EStyleDataType.NUMBER:
                    return (double)Value;
                case EStyleDataType.PERCENT:
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
                case EStyleDataType.INTEGER:
                    return (int)Value;
                case EStyleDataType.NUMBER:
                    return (double)Value;
                case EStyleDataType.PERCENT:
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
                case EStyleDataType.COLOR:
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
                case EStyleDataType.INTEGER:
                case EStyleDataType.NUMBER:
                    return string.Concat(Value);
                case EStyleDataType.DIMENSION:
                    {
                        if (Unit == EStyleUnit.None)
                            return string.Concat(((double)Value).ToString());
                        else
                            return string.Concat(((double)Value).ToString(), Enum.GetName(typeof(EStyleUnit), Unit).ToLower());
                    }
                case EStyleDataType.PERCENT:
                    return string.Concat((double)Value, "%");
                case EStyleDataType.COLOR:
                    return ((cssColor)Value).ToCssString();
                case EStyleDataType.STRING:
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
            if (object.ReferenceEquals(A, null) || object.ReferenceEquals(B, null)) return (object.ReferenceEquals(A, null) && object.ReferenceEquals(B, null));
            if (A.Type != B.Type) return false;
            if (A.Unit != B.Unit) return false;

            switch (A.Type)
            {
                case EStyleDataType.AUTO:
                case EStyleDataType.INITIAL:
                case EStyleDataType.INHERIT:
                case EStyleDataType.UNSET:
                case EStyleDataType.NONE:
                    return true;
                case EStyleDataType.INTEGER:
                case EStyleDataType.COLOR:
                    return EqualityComparer<int>.Default.Equals(A.Value, B.Value);
                case EStyleDataType.NUMBER:
                case EStyleDataType.DIMENSION:
                case EStyleDataType.PERCENT:
                    return EqualityComparer<double>.Default.Equals(A.Value, B.Value);
                case EStyleDataType.STRING:
                    return EqualityComparer<string>.Default.Equals(A.Value, B.Value);
                default:
                    throw new NotImplementedException(string.Concat("Equality comparison logic not implemented for type: ", Enum.GetName(typeof(EStyleDataType), A.Type)));
            }
        }

        public static bool operator !=(CssValue A, CssValue B)
        {
            if (object.ReferenceEquals(A, null) || object.ReferenceEquals(B, null)) return !(object.ReferenceEquals(A, null) && object.ReferenceEquals(B, null));
            if (A.Type != B.Type) return true;
            if (A.Unit != B.Unit) return true;

            switch (A.Type)
            {
                case EStyleDataType.AUTO:
                case EStyleDataType.INITIAL:
                case EStyleDataType.INHERIT:
                case EStyleDataType.UNSET:
                case EStyleDataType.NONE:
                    return false;
                case EStyleDataType.INTEGER:
                case EStyleDataType.COLOR:
                    return !EqualityComparer<int>.Default.Equals(A.Value, B.Value);
                case EStyleDataType.NUMBER:
                case EStyleDataType.DIMENSION:
                case EStyleDataType.PERCENT:
                    return !EqualityComparer<double>.Default.Equals(A.Value, B.Value);
                case EStyleDataType.STRING:
                    return !EqualityComparer<string>.Default.Equals(A.Value, B.Value);
                default:
                    throw new NotImplementedException(string.Concat("Inequality comparison logic not implemented for type: ", Enum.GetName(typeof(EStyleDataType), A.Type)));
            }
        }

        
        public override bool Equals(object o)
        {

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

    public static class StyleValue_Ext
    {
        public static bool IsNull(this CssValue SV)
        {
            return object.ReferenceEquals(SV, null);
        }

        public static bool IsNullOrUnset(this CssValue SV)
        {
            if (object.ReferenceEquals(SV, null)) return true;
            return (SV.Type == EStyleDataType.UNSET);
        }
    }

}
