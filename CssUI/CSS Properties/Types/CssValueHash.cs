using CssUI.CSS;

namespace CssUI.Internal
{
    /// <summary>
    /// Stores the hash for a <see cref="CssValue"/> for later comparison
    /// </summary>
    public class CssValueHash
    {
        #region Properties
        public int Hash { get; private set; } = 0;
        
        /// <summary>
        /// How many times this hash has been changed
        /// </summary>
        public int ChangeCount { get; private set; } = 0;
        #endregion


        #region Constructors
        public CssValueHash(CssValue Value)
        {
            this.Hash = Value.GetHashCode();
        }
        #endregion

        #region Casts
        public void Set(object Value)
        {
            this.Hash = Value.GetHashCode();
            ChangeCount++;
        }
        #endregion

        #region Operators
        public static bool operator == (CssValueHash Hash, object o)
        {
            return Hash.Hash == o.GetHashCode();
        }

        public static bool operator !=(CssValueHash Hash, object o)
        {
            return Hash.Hash != o.GetHashCode();
        }

        public override int GetHashCode()
        {
            return this.Hash;
        }

        public override bool Equals(object obj)
        {
            return obj is CssValueHash hash &&
                   Hash == hash.Hash;
        }
        #endregion
    }
}
