using CssUI.CSS.Internal;
using CssUI.CSS.Parser;
using CssUI.CSS.Serialization;
using CssUI.Rendering;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics.Contracts;
using System.Globalization;
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
#if ALLOW_DYNAMIC
        private readonly dynamic value = null;
#else
        private readonly object value = null;
#endif
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
        public bool HasValue
        {
            get
            {
                if (Type == ECssValueTypes.NULL)
                    return false;

                if (value is null)
                    return false;

                return true;
            }
        }
        #endregion

        #region Constructors
        internal CssValue(CssFunction function)
        {
            type = ECssValueTypes.FUNCTION;
            value = function;
        }

        private CssValue(ECssValueTypes type)
        {
            this.type = type;
            flags |= Get_Inherent_Value_Type_Flags(type, unit, value);
        }

        internal CssValue(ECssValueTypes type, object value = null) : this(type)
        {
            this.value = value;
        }

        internal CssValue(ECssValueTypes type, object value, ECssValueFlags flags) : this(type)
        {
            this.value = value;
            this.flags |= flags;
            this.value = value;
        }

        internal CssValue(ECssValueTypes type, object value, ECssUnit unit) : this(type)
        {
            this.unit = unit;
            this.value = value;

            if (type == ECssValueTypes.KEYWORD)// Try and catch some common IMPORTANT keywords
            {
                /* If our keyword can be resolved to another ECssValueType then its an global keyword */
                if (!Lookup.TryEnum(value as string, out ECssValueTypes outType))
                {
                    type = outType;
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
                        type = ECssValueTypes.RESOLUTION;
                        break;
                    default:
                        type = ECssValueTypes.DIMENSION;
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
            value = sv.value;
            unit = sv.Unit;
            flags = sv.Flags;
        }
        #endregion

        private static ECssValueFlags Get_Inherent_Value_Type_Flags(ECssValueTypes Type, ECssUnit Unit, object Value)
        {
            ECssValueFlags Flags = ECssValueFlags.None;

            /*if (Value is Array)
            {
                Flags |= ECssValueFlags.Collection;
            }*/

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
                case ECssValueTypes.IMAGE:
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
                        if (Value is Array array)
                        {// Multi object
                            foreach (object o in array)
                            {
                                if (o is CssValue cssValue)
                                {
                                    Flags |= Get_Inherent_Value_Type_Flags(cssValue.Type, cssValue.Unit, cssValue.value);
                                }
                                else
                                {
                                    throw new CssException($"All {nameof(CssValue)} collection members must be {nameof(CssValue)}s");
                                }
                            }
                            return Flags;
                        }
                        else if (Value is CssValue cssValue)
                        {// Single object
                            Flags |= Get_Inherent_Value_Type_Flags(cssValue.Type, cssValue.Unit, cssValue.value);
                        }
                        else
                        {
                            throw new CssException($"All {nameof(CssValue)} collection members must be {nameof(CssValue)}s");
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

        /// <summary>Create an keyword value</summary>
        public static CssValue From<T>(T value) where T : struct, IConvertible
        {
            if (!Lookup.Is_Declared<T>())
            {
                throw new CssException($"Unable to find '{typeof(T).Name}' in CSS enum table");
            }
            Contract.EndContractBlock();

            return new CssValue(ECssValueTypes.KEYWORD, value);
        }

        /// <summary>Create an absolute integer value</summary>
        public static CssValue From(int value) => new CssValue(ECssValueTypes.INTEGER, value);

        /// <summary>Create an absolute integer value if not null, or return the given default value</summary>
        public static CssValue From(int? value, CssValue defaultValue) => (!value.HasValue ? defaultValue : new CssValue(ECssValueTypes.INTEGER, value.Value));

        /// <summary>Create an absolute number value</summary>
        public static CssValue From(double value) => new CssValue(ECssValueTypes.NUMBER, value);

        /// <summary>Create an absolute number value if not null, or return the given default value</summary>
        public static CssValue From(double? value, CssValue defaultValue) => (!value.HasValue ? defaultValue : new CssValue(ECssValueTypes.NUMBER, value.Value));

        /// <summary>Create a percentage value</summary>
        /// <param name="value">Floating-point value in the range [0 - 100]</param>
        public static CssValue From_Percent(double value) => new CssValue(ECssValueTypes.PERCENT, value);

        /// <summary>Create an absolute length value</summary>
        public static CssValue From(double value, ECssUnit Unit) => new CssValue(ECssValueTypes.DIMENSION, value, Unit);

        /// <summary>Create an absolute length value if not null, or return the given default value</summary>
        public static CssValue From(double? value, ECssUnit Unit, CssValue defaultValue) => (!value.HasValue ? defaultValue : new CssValue(ECssValueTypes.DIMENSION, value.Value, Unit));



        /// <summary>Create an RGBA color value</summary>
        public static CssValue From(ReadOnlyColor value) => new CssValue(ECssValueTypes.COLOR, value.AsInteger());

        /// <summary>Create a string value</summary>
        public static CssValue From_String(string value) => new CssValue(ECssValueTypes.STRING, value);

        /// <summary>Create a css-value by parsing the given string as CSS markup</summary>
        public static CssValue From_CSS(string css) => new CssParser(css).Parse_CssValue();

        /// <summary>Create a css-value with a specific type and one or more values</summary>
        public static CssValue From(params CssValue[] values)
        {
            if (values.Length <= 0) throw new ArgumentException("One or more values must be specified");
            Contract.EndContractBlock();

            return new CssValue(ECssValueTypes.COLLECTION, values);
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
            if (UnitResolver is null) throw new ArgumentNullException(nameof(UnitResolver));
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
            if (UnitResolver is null) throw new ArgumentNullException(nameof(UnitResolver));
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
            if (UnitResolver is null) throw new ArgumentNullException(nameof(UnitResolver));
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
            if (UnitResolver is null) throw new ArgumentNullException(nameof(UnitResolver));
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
            if (UnitResolver is null) throw new ArgumentNullException(nameof(UnitResolver));
            if (Predicate is null) throw new ArgumentNullException(nameof(Predicate));
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
            if (UnitResolver is null) throw new ArgumentNullException(nameof(UnitResolver));
            if (Predicate is null) throw new ArgumentNullException(nameof(Predicate));
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
            if (UnitResolver is null) throw new ArgumentNullException(nameof(UnitResolver));
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
            if (UnitResolver is null) throw new ArgumentNullException(nameof(UnitResolver));
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
            if (UnitResolver is null) throw new ArgumentNullException(nameof(UnitResolver));
            if (percentageResolver is null) throw new ArgumentNullException(nameof(percentageResolver));
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
            if (UnitResolver is null) throw new ArgumentNullException(nameof(UnitResolver));
            if (percentageResolver is null) throw new ArgumentNullException(nameof(percentageResolver));
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
            if (UnitResolver is null) throw new ArgumentNullException(nameof(UnitResolver));
            if (Predicate is null) throw new ArgumentNullException(nameof(Predicate));
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
            if (UnitResolver is null) throw new ArgumentNullException(nameof(UnitResolver));
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
            if (UnitResolver is null) throw new ArgumentNullException(nameof(UnitResolver));
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
            if (Predicate is null) throw new ArgumentNullException(nameof(Predicate));
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
            if (Predicate is null) throw new ArgumentNullException(nameof(Predicate));
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
            if (percentageResolver is null) throw new ArgumentNullException(nameof(percentageResolver));
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
            if (percentageResolver is null) throw new ArgumentNullException(nameof(percentageResolver));
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
            if (Predicate is null) throw new ArgumentNullException(nameof(Predicate));
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
            if (percentageResolver is null) throw new ArgumentNullException(nameof(percentageResolver));
            Contract.EndContractBlock();

            int? num = (int?)Resolve(percentageResolver);
            return (num.HasValue ? num.Value : defaultValue);
        }

        #endregion

        #region Converters
        /// <summary>
        /// Returns the value as the specified enum type
        /// </summary>
        /// <typeparam name="T"></typeparam>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public T AsEnum<T>() where T : struct, IConvertible => (T)value;

        /// <summary>
        /// Returns the value as a Color4 if possible, or NULL if not possible.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Point2f AsPosition()
        {
            if (Type != ECssValueTypes.POSITION) throw new CssException($"{nameof(CssValue)} is not a Position! {this}");
            Contract.EndContractBlock();

            return (Point2f)value;
        }

        /// <summary>
        /// Returns the value as a Color4 if possible, or NULL if not possible.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ReadOnlyColor AsColor()
        {
            if (Type != ECssValueTypes.COLOR) throw new CssException($"{nameof(CssValue)} is not a Color! {this}");
            Contract.EndContractBlock();

            return (int)value;
        }

        /// <summary>
        /// Returns the value as a Color4 if possible, or NULL if not possible.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ReadOnlyCollection<CssValue> AsCollection()
        {
            if (!IsCollection) throw new CssException($"{nameof(CssValue)} is not a collection! {this}");
            Contract.EndContractBlock();

            return new ReadOnlyCollection<CssValue>((CssValue[])value);
        }

        /// <summary>
        /// Returns the value as the preferred Integer type
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int AsInteger() => (int)value;

        /// <summary>
        /// Returns the value as the preferred (Nullable) Integer type
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int? AsIntegerN() => !HasValue ? null : (int?)value;

        /// <summary>
        /// Returns the value as the preferred Decimal type
        /// </summary>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public double AsDecimal() => (double)value;

        /// <summary>
        /// Returns the value as the preferred (Nullable) Decimal type
        /// </summary>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public double? AsDecimalN() => !HasValue ? null : (double?)value;

        /// <summary>
        /// Returns the value as a string
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public string AsString() => (string)value;
        #endregion

        #region Operators
        public static bool operator ==(CssValue A, CssValue B)
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
                    return true;
                case ECssValueTypes.COLOR:
                    return EqualityComparer<int>.Default.Equals((int)A.value, (int)B.value);
                case ECssValueTypes.INTEGER:
                    return EqualityComparer<long>.Default.Equals((long)A.value, (long)B.value);
                case ECssValueTypes.NUMBER:
                case ECssValueTypes.DIMENSION:
                case ECssValueTypes.PERCENT:
                    return EqualityComparer<double>.Default.Equals((double)A.value, (double)B.value);
                case ECssValueTypes.STRING:
                    return EqualityComparer<string>.Default.Equals((string)A.value, (string)B.value);
                case ECssValueTypes.KEYWORD:
                    return A.value.Equals(B.value);
                //return EqualityComparer<int>.Default.Equals((int)A.Value, (int)B.Value);
                default:
                    throw new NotImplementedException($"Equality comparison logic not implemented for type: {Enum.GetName(typeof(ECssValueTypes), A.Type)}");
            }
        }

        public static bool operator !=(CssValue A, CssValue B)
        {
            return !(A == B);
        }


        public override bool Equals(object o)
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
        public override string ToString()
        {
            const string DECIMAL_FORMAT = "0.###";
            switch (Type)
            {
                case ECssValueTypes.COLOR:
                    return AsColor().Serialize();
                case ECssValueTypes.INTEGER:
                    return AsInteger().ToString(CultureInfo.InvariantCulture);
                case ECssValueTypes.NUMBER:
                    return string.Concat(AsDecimal().ToString(DECIMAL_FORMAT, CultureInfo.InvariantCulture));
                case ECssValueTypes.DIMENSION:
                    if (Unit == ECssUnit.None)
                        return string.Concat(AsDecimal().ToString(DECIMAL_FORMAT, CultureInfo.InvariantCulture), "<none>");
                    else
                        return string.Concat(AsDecimal().ToString(DECIMAL_FORMAT, CultureInfo.InvariantCulture), Lookup.Keyword(unit));
                case ECssValueTypes.PERCENT:
                    return string.Concat(AsDecimal().ToString(DECIMAL_FORMAT, CultureInfo.InvariantCulture), "%");
                case ECssValueTypes.STRING:
                    return string.Concat(UnicodeCommon.CHAR_QUOTATION_MARK, (string)value, UnicodeCommon.CHAR_QUOTATION_MARK);
                case ECssValueTypes.KEYWORD:
                    return Lookup.Keyword(value.GetType(), value);// Enum.GetName(value.GetType(), value);
                default:
                    return string.Concat("[", Enum.GetName(typeof(ECssValueTypes), Type), "]");
            }
        }
        #endregion

        #region Serialization
        public string Serialize()
        {
            switch (Type)
            {
                case ECssValueTypes.AUTO:
                case ECssValueTypes.INHERIT:
                case ECssValueTypes.INITIAL:
                case ECssValueTypes.DEFAULT:
                case ECssValueTypes.UNSET:
                case ECssValueTypes.NONE:
                    {
                        return Lookup.Keyword(Type);
                    }
                case ECssValueTypes.NULL:
                    {
                        return string.Empty;
                    }
                case ECssValueTypes.KEYWORD:
                    {
                        return (string)value;
                    }
                case ECssValueTypes.STRING:
                    {
                        return string.Concat(UnicodeCommon.CHAR_QUOTATION_MARK, (string)value, UnicodeCommon.CHAR_QUOTATION_MARK);
                    }
                case ECssValueTypes.COLOR:
                    {
                        return AsColor().Serialize();
                    }
                case ECssValueTypes.DIMENSION:
                    {
                        if (Unit == ECssUnit.None)
                        {
                            return AsDecimal().ToString(CultureInfo.InvariantCulture);
                        }
                        else
                        {
                            if (!Lookup.TryKeyword(Unit, out string unitStr))
                            {
                                throw new CssException($"Unable to find enum value {Unit} in CSS enum table");
                            }

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

}
