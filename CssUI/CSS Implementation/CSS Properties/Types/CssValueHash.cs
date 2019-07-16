
namespace CssUI.CSS.Internal
{
    /// <summary>
    /// Stores the hash for a <see cref="CssValue"/> for later comparison
    /// </summary>
    public class CssValueHash
    {
        #region Properties
        /// <summary>
        /// Tracks if this hash has been set
        /// </summary>
        public bool HasValue { get; private set; } = false;

        public int Hash { get; private set; } = 0;

        /// <summary>
        /// How many times this hash has been changed
        /// </summary>
        public int ChangeCount { get; private set; } = 0;
        #endregion


        #region Constructors
        public CssValueHash()
        {
        }
        public CssValueHash(CssValue Value)
        {
            Hash = Value.GetHashCode();
        }
        #endregion

        #region Casts
        public void Set(object Value)
        {
            if (ReferenceEquals(Value, null))
            {
                Hash = 0;
                HasValue = false;
            }
            else
            {
                HasValue = true;
                Hash = Value.GetHashCode();
                ChangeCount++;
            }
        }
        #endregion

        #region Operators
        public static bool operator ==(CssValueHash Hash, object o)
        {
            return !ReferenceEquals(o, null) && Hash.Hash == o.GetHashCode();
        }

        public static bool operator !=(CssValueHash Hash, object o)
        {
            return ReferenceEquals(o, null) || Hash.Hash != o.GetHashCode();
        }

        public override int GetHashCode()
        {
            return Hash;
        }

        public override bool Equals(object obj)
        {
            return obj is CssValueHash hash &&
                   Hash == hash.Hash;
        }
        #endregion
    }
}
