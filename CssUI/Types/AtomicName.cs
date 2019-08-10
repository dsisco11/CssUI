using System;

namespace CssUI
{
    /// <summary>
    /// Implements an efficient way of referring to common string names without constantly performing costly string comparisons.
    /// Allows identifying common names via an enum which maps to a lookup table
    /// This is used for things like DOM element attributes, and CSS property names.
    /// <para>AtomicNames can effectively be thought of as self indexing strings, Maps and lists of AtomicNames cease being indexed by strings and instead are effectively indexed by their self assigned integer values</para>
    /// </summary>
    public class AtomicName<Ty> : IConvertible, IComparable<Int32>, IEquatable<Int32> where Ty : struct
    {
        #region Static
        private static int CUSTOM_VALUE = -1;
        private static ConcurrentReversableDictionary<AtomicString, int> NameRegistry = new ConcurrentReversableDictionary<AtomicString, int>();
        #endregion

        #region Properties
        /* Backing type for Name */
        private string _name = null;
        /* AtomicName instances wont always need their string names to be referenced, so we dont resolve it until we are asked. */
        public string Name
        {
            get
            {
                if (ReferenceEquals(null, _name))
                {/* Resolve the backing value */
                    if (IsCustom)
                        throw new Exception("Name backing-value for custom event type is not set!");

                    string name = Value_To_Name( Value );
                    if (ReferenceEquals(null, name))
                        throw new Exception($"Unable to convert \"{typeof(Ty).Name}\" value to string");

                    _name = name;
                    NameLower = name.ToLowerInvariant();
                }

                return _name;
            }
            private set
            {
                _name = value;
                NameLower = value.ToLowerInvariant();
            }
        }

        public string NameLower { get; private set; } = null;

        /// <summary>
        /// The value of this name for comparison purposes
        /// </summary>
        internal readonly int Value;
        public readonly Ty? EnumValue = null;
        public readonly bool IsCustom;
        #endregion

        #region Constructors
        /// <summary>
        /// Creates an event specifier from the given <see cref="EEventName"/> value.
        /// </summary>
        /// <param name="name"></param>
        public AtomicName(Ty enumValue)
        {
            var eV = Enum_To_Value(enumValue);
            if (!eV.HasValue)
            {
                throw new Exception($"Unable to convert \"{typeof(Ty).Name}\" value to integer");
            }

            Value = eV.Value;
            EnumValue = enumValue;
        }

        /// <summary>
        /// Creates an name specifier from a string
        /// </summary>
        /// <param name="Name"></param>
        public AtomicName(string Name)
        {
            this.Name = Name;
            this.Value = Get_Or_Register_Name(Name, out bool outIsCustom);
            this.IsCustom = outIsCustom;
            if (!IsCustom)
            {
                EnumValue = Name_To_Enum(Name);
            }
        }
        #endregion

        /// <summary>
        /// Converts our integer value into its string name, supposedly through an enum lookup table
        /// </summary>
        /// <param name="value"></param>
        protected virtual string Value_To_Name(int value)
        {
            if (!IsCustom)
            {
                /* /!\ These conversions will fucking EXPLODE if the given generic type does not have an integer backing type /!\ */
                Ty enumValue = CastTo<Ty>.From<int>(value);

                if (Lookup.TryKeyword<Ty>(enumValue, out string LUT) && !ReferenceEquals(null, LUT))
                {
                    return LUT;
                }
            }

            /* Alright buster, we gotta do things the slow way */
            if (NameRegistry.TryGetKey(value, out AtomicString name))
                return name;

            /* Well we tried */
            return null;
        }

        /// <summary>
        /// Converts a string name into a positive integer value, preferrably via enum lookup.
        /// A return value of <c>null</c> will cause the name to be registered as a custom one.
        /// </summary>
        /// <param name="Name"></param>
        protected virtual int? Name_To_Value(string Name)
        {
            if (Lookup.TryEnum<Ty>(Name, out Ty outEnum))
            {
                return CastTo<Int32>.From<Ty>(outEnum);
            }
            return null;// name.GetHashCode();
        }

        /// <summary>
        /// Converts an enum value into a positive integer value, preferrably via LUT lookup or else this class wont be performant.
        /// </summary>
        /// <param name="enumValue"></param>
        protected virtual int? Enum_To_Value(Ty enumValue)
        {
            //return Convert.ToInt32(enumValue);
            return CastTo<Int32>.From<Ty>(enumValue);
        }

        /// <summary>
        /// Converts a string name into an enum value, preferrably via LUT lookup or else this class wont be performant.
        /// </summary>
        /// <param name="Name"></param>
        protected virtual Ty? Name_To_Enum(string Name)
        {
            if (Lookup.TryEnum<Ty>(Name, out Ty outEnum))
            {
                return outEnum;
            }
            return null;
        }

        #region Name Registry
        protected int Get_Or_Register_Name(AtomicString Name, out bool outIsCustom)
        {
            var nameValue = Name_To_Value(Name);
            if (nameValue.HasValue)
            {
                outIsCustom = false;
                return (int)nameValue.Value;
            }

            /* If we couldnt resolve this string name from our enum then it is a custom name */
            outIsCustom = true;

            if (!NameRegistry.TryGetValue(Name, out int Value))
            {/* Register new custom name */
                Value = CUSTOM_VALUE--;
                NameRegistry.TryAdd(Name, Value);
            }

            return Value;
        }
        #endregion

        #region Implicit
        public static implicit operator AtomicName<Ty>(Ty Name) => new AtomicName<Ty>(Name);
        public static implicit operator AtomicName<Ty>(string Name) => new AtomicName<Ty>(Name);
        #endregion

        #region Equality
        public static bool operator ==(AtomicName<Ty> A, AtomicName<Ty> B)
        {
            return (A.Value == B.Value);
        }

        public static bool operator !=(AtomicName<Ty> A, AtomicName<Ty> B)
        {
            return (A.Value != B.Value);
        }

        public static bool operator ==(AtomicName<Ty> A, Ty B)
        {
            return (A.Value == Convert.ToInt32(B));
        }

        public static bool operator !=(AtomicName<Ty> A, Ty B)
        {
            return (A.Value != Convert.ToInt32(B));
        }

        public override bool Equals(object obj)
        {
            if (obj is AtomicName<Ty> name && Value.Equals(name.Value))
                return true;

            if (obj is Ty enumValue && Value.Equals(Convert.ToInt32(enumValue)))
                return true;

            return false;
        }

        public bool Equals(AtomicName<Ty> Name)
        {
            if (Name == null)
                return false;

            return Value.Equals(Name.Value);
        }

        public bool Equals(int other)
        {
            return Value.Equals(other);
        }

        #endregion

        #region Overrides
        public override int GetHashCode() => this.Value;

        public override string ToString() => this.Name;
        #endregion

        #region Comparable
        public int CompareTo(object obj)
        {
            if (obj == null)
                return 1;

            if (obj is AtomicName<Ty> objName)
            {
                return Value.CompareTo(objName.Value);
            }
            else if (obj is int oInt)
            {
                return Value.CompareTo(oInt);
            }
            else if (obj is uint oUInt)
            {
                return Value.CompareTo(oUInt);
            }
            else
            {
                throw new ArgumentException($"Object is not an {nameof(AtomicName<Ty>)} or Integer");
            }
        }

        public int CompareTo(AtomicName<Ty> Name)
        {
            if (Name == null)
                return 1;

            return Value.CompareTo(Name.Value);
        }

        public int CompareTo(int other)
        {
            return Value.CompareTo(other);
        }
        #endregion

        #region Convertable
        bool IConvertible.ToBoolean(IFormatProvider provider) => Convert.ToBoolean(Value, provider);

        byte IConvertible.ToByte(IFormatProvider provider) => Convert.ToByte(Value, provider);

        UInt16 IConvertible.ToUInt16(IFormatProvider provider) => Convert.ToUInt16(Value, provider);

        Int16 IConvertible.ToInt16(IFormatProvider provider) => Convert.ToInt16(Value, provider);

        UInt32 IConvertible.ToUInt32(IFormatProvider provider) => Convert.ToUInt32(Value, provider);

        Int32 IConvertible.ToInt32(IFormatProvider provider) => Convert.ToInt32(Value, provider);

        UInt64 IConvertible.ToUInt64(IFormatProvider provider) => Convert.ToUInt64(Value, provider);

        Int64 IConvertible.ToInt64(IFormatProvider provider) => Convert.ToInt64(Value, provider);

        public TypeCode GetTypeCode() => Value.GetTypeCode();

        public char ToChar(IFormatProvider provider) => ((IConvertible)Value).ToChar(provider);

        public DateTime ToDateTime(IFormatProvider provider) => ((IConvertible)Value).ToDateTime(provider);

        public decimal ToDecimal(IFormatProvider provider) => ((IConvertible)Value).ToDecimal(provider);

        public double ToDouble(IFormatProvider provider) => ((IConvertible)Value).ToDouble(provider);

        public sbyte ToSByte(IFormatProvider provider) => ((IConvertible)Value).ToSByte(provider);

        public float ToSingle(IFormatProvider provider) => ((IConvertible)Value).ToSingle(provider);

        public string ToString(IFormatProvider provider) => Value.ToString(provider);

        public object ToType(Type conversionType, IFormatProvider provider) => ((IConvertible)Value).ToType(conversionType, provider);
        #endregion

    }
}
