using System.Collections.Concurrent;

namespace CssUI.DOM.Events
{
    /// <summary>
    /// Implements an efficient way of specifying events within the DOM without performing costly string analysis literally ALL THE TIME.
    /// </summary>
    public class EventName
    {
        #region Static
        private static int CUSTOM_VALUE = -1;
        private static ConcurrentDictionary<AtomicString, int> CustomRegistry = new ConcurrentDictionary<AtomicString, int>();
        #endregion

        #region Instances
        public static EventName Empty = new EventName(EEventName.None);
        #endregion

        #region Properties
        /* Backing type for Name */
        private string _name;
        public string Name
        {
            get
            {
                if (!ReferenceEquals(null, _name))
                {/* Resolve the backing value */
                    if (IsCustom) throw new System.Exception("Name backing-value for custom event type is not set!");
                    string keyword = Lookup.Keyword((EEventName)Value);
                    _name = keyword;
                }

                return _name;
            }
            private set => _name = value;
        }
        /// <summary>
        /// The value for this object which gets compared against
        /// </summary>
        internal readonly int Value;// { get; private set; }
        internal EEventName EnumValue => Value < 0 ? EEventName.CUSTOM : (EEventName)Value;
        public readonly bool IsCustom;
        #endregion

        #region Constructors
        /// <summary>
        /// Creates an event specifier from the given <see cref="EEventName"/> value.
        /// </summary>
        /// <param name="name"></param>
        public EventName(EEventName name)
        {
            Value = (int)name;
            // this.Name = DomLookup.Enum<EEventName>(name);
        }

        /// <summary>
        /// Creates an event specifier from a custom event name.
        /// </summary>
        /// <param name="CustomName"></param>
        public EventName(string CustomName)
        {
            this.IsCustom = true;
            this.Name = CustomName;
            this.Value = Get_Or_Register_Name(CustomName);
        }
        #endregion

        #region Custom Registry
        private int Get_Or_Register_Name(AtomicString Name)
        {
            if (DomLookup.TryEnum(Name, out EEventName enumValue))
            {
                return (int)enumValue;
            }

            if (!CustomRegistry.TryGetValue(Name, out int Value))
            {/* Register new custom name */
                Value = CUSTOM_VALUE++;
                CustomRegistry.TryAdd(Name, Value);
            }

            return Value;
        }
        #endregion

        #region Implicit
        public static implicit operator EventName(EEventName Name) => new EventName(Name);
        public static implicit operator EventName(string Name) => new EventName(Name);
        #endregion

        #region Equality
        public static bool operator ==(EventName A, EventName B)
        {
            return (A.Value == B.Value);
        }

        public static bool operator !=(EventName A, EventName B)
        {
            return (A.Value != B.Value);
        }

        public static bool operator ==(EventName A, EEventName B)
        {
            return (A.Value == (int)B);
        }

        public static bool operator !=(EventName A, EEventName B)
        {
            return (A.Value != (int)B);
        }

        public override bool Equals(object obj)
        {
            if (obj is EventName name && name.Value == this.Value)
                return true;

            if (obj is EEventName enumValue && (int)enumValue == this.Value)
                return true;

            return false;
        }
        #endregion

        #region Overrides
        public override int GetHashCode() => Value;

        public override string ToString() => Name;
        #endregion
    }
}
