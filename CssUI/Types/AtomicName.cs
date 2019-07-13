using System;
using CssUI.Internal;

namespace CssUI
{
    /// <summary>
    /// Implements an efficient way of referring to common string names without constantly performing costly string comparisons.
    /// Allows identifying common names via an enum which maps to a lookup table
    /// This is used for things like DOM element attributes, and CSS property names.
    /// <para>AtomicNames can effectively be thought of as self indexing strings, Maps and lists of AtomicNames cease being indexed by strings and instead are effectively indexed by their self assigned integer values</para>
    /// </summary>
    public class AtomicName<Ty> where Ty : struct, IConvertible
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
                if (!ReferenceEquals(null, _name))
                {/* Resolve the backing value */
                    if (IsCustom)
                        throw new System.Exception("Name backing-value for custom event type is not set!");

                    string name = Value_To_Name( Value );
                    if (ReferenceEquals(null, name))
                        throw new Exception($"Unable to convert \"{typeof(Ty).Name}\" value to string");

                    _name = name;
                }

                return _name;
            }
            private set => _name = value;
        }
        /// <summary>
        /// The value of this name for comparison purposes
        /// </summary>
        internal readonly int Value;
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
                throw new Exception($"Unable to convert \"{typeof(Ty).Name}\" value to integer");
            Value = eV.Value;
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
        }
        #endregion

        /// <summary>
        /// Converts our integer value into its string name, supposedly through an enum lookup table
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        protected virtual string Value_To_Name(int value)
        {
            if (!IsCustom)
            {
                /* /!\ These conversions will fucking EXPLODE if the given generic type does not have an integer backing type /!\ */
                Ty enumValue = CastTo<Ty>.From<int>(value);
                string domLUT = DomLookup.Keyword_From_Enum<Ty>(enumValue);
                if (!ReferenceEquals(null, domLUT)) return domLUT;
                string cssLUT = CssLookup.Keyword_From_Enum<Ty>(enumValue);
                if (!ReferenceEquals(null, cssLUT)) return cssLUT;
            }

            /* Alright buster, we will do things the slow way */
            if (NameRegistry.TryGetKey(value, out AtomicString name))
                return name;

            /* Well we tried */
            return null;
        }

        /// <summary>
        /// Converts a string name into a positive integer value, preferrably via enum lookup.
        /// A return value of <c>null</c> will cause the name to be registered as a custom one.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        protected virtual int? Name_To_Value(string name)
        {
            return null;// name.GetHashCode();
        }

        /// <summary>
        /// Converts an enum value into a positive integer value, preferrably via LUT lookup or else this class wont be performant.
        /// </summary>
        /// <param name="enumValue"></param>
        /// <returns></returns>
        protected virtual int? Enum_To_Value(Ty enumValue)
        {
            return Convert.ToInt32(enumValue);
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
            if (obj is AtomicName<Ty> name && name.Value == this.Value)
                return true;

            if (obj is Ty enumValue && Convert.ToInt32(enumValue) == this.Value)
                return true;

            return false;
        }
        #endregion

        #region Overrides
        public override int GetHashCode() => this.Value;

        public override string ToString() => this.Name;
        #endregion
    }
}
