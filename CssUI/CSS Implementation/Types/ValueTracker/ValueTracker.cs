
namespace CssUI.CSS.Internal
{

    /// <summary>
    /// Stores the hash for an generic object such that changes to it may be detected.
    /// </summary>
    public class ValueTracker<Ty>
    {
        #region Properties
        /// <summary>
        /// Tracks if this hash has been set
        /// </summary>
        public bool HasValue { get; protected set; } = false;

        public int Hash { get; protected set; } = 0;

        /// <summary>
        /// How many times this hash has been changed
        /// </summary>
        public int ChangeCount { get; protected set; } = 0;
        #endregion

        #region Events
        /// <summary>
        /// Fired whenever the value hash changes
        /// </summary>
        public event ValueTrackerEventHandler onChange;
        #endregion

        #region Constructors
        public ValueTracker()
        {
        }
        public ValueTracker(Ty value)
        {
            Hash = value.GetHashCode();
        }
        #endregion

        #region Casts
        /// <summary>
        /// Updates the value and optionally suppresses the change event.
        /// </summary>
        /// <param name="newValue">New Value</param>
        /// <param name="suppress">If <c>True</c> the change event will not be fired</param>
        public void Update(Ty newValue, bool suppress = false)
        {
            if (ReferenceEquals(newValue, null))
            {
                if (Hash != 0)
                {
                    if (!suppress)
                    {
                        this.onChange?.Invoke(Hash, 0, ChangeCount, HasValue);
                    }

                    Hash = 0;
                    ChangeCount++;
                    HasValue = false;
                }
            }
            else
            {
                int newHash = newValue.GetHashCode();
                if (Hash != newHash)
                {
                    if (!suppress)
                    {
                        this.onChange?.Invoke(Hash, newHash, ChangeCount, HasValue);
                    }

                    Hash = newValue.GetHashCode();
                    ChangeCount++;
                    HasValue = true;
                }
            }
        }
        #endregion

        #region Operators
        public static bool operator ==(ValueTracker<Ty> Hash, object o)
        {
            return !ReferenceEquals(o, null) && Hash.Hash == o.GetHashCode();
        }

        public static bool operator !=(ValueTracker<Ty> Hash, object o)
        {
            return ReferenceEquals(o, null) || Hash.Hash != o.GetHashCode();
        }

        public override int GetHashCode()
        {
            return Hash;
        }

        public override bool Equals(object obj)
        {
            if (obj is ValueTracker<Ty> hash && this.Hash == hash.Hash)
                return true;

            return (obj is Ty typeValue && this.Hash == typeValue.GetHashCode());
        }
        #endregion
    }
}
