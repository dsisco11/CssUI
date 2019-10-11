using System;

namespace CssUI
{
    /// <summary>
    /// Simplifies the process of resolving a value once and caching it for future references
    /// </summary>
    public class CachedValue<Ty>
    {
        #region Properties
        public bool IsCached { get; private set; } = false;
        private Ty value = default(Ty);
        private readonly Func<Ty> Resolver = null;
        #endregion

        #region Constructors
        public CachedValue(Func<Ty> Resolver)
        {
            this.Resolver = Resolver;
        }
        #endregion

        /// <summary>
        /// Clears the cached value(if any)
        /// </summary>
        public void Clear()
        {
            IsCached = false;
            value = default(Ty);
        }

        /// <summary>
        /// Resolves and caches the value if unset, otherwise returns the cached value
        /// </summary>
        /// <param name="Resolver"></param>
        /// <returns></returns>
        public Ty Get()
        {
            if (!IsCached)
            {
                value = Resolver();
                IsCached = true;
            }

            return value;
        }

        /// <summary>
        /// Resolves and caches the value if unset, otherwise returns the cached value
        /// </summary>
        /// <param name="Resolver"></param>
        /// <returns></returns>
        public Ty Get(Func<Ty> Resolver)
        {
            if (!IsCached)
            {
                value = Resolver();
                IsCached = true;
            }

            return value;
        }

        #region Operators

        public static bool operator ==(CachedValue<Ty> A, CachedValue<Ty> B)
        {
            // If both objects are NULL they match
            if (ReferenceEquals(null, A) && ReferenceEquals(null, B)) return true;
            // If one object is null and not the other they do not match
            if (ReferenceEquals(null, A) ^ ReferenceEquals(null, B)) return false;
            // Check if values match
            return A.Get().Equals(B.Get());
        }

        public static bool operator !=(CachedValue<Ty> A, CachedValue<Ty> B)
        {
            // If both objects are null they do not match
            if (ReferenceEquals(null, A) && ReferenceEquals(null, B)) return false;
            // If one object is null and not the other they do match
            if (ReferenceEquals(null, A) ^ ReferenceEquals(null, B)) return true;
            // Check if values match
            return !A.Get().Equals(B.Get());
        }

        public bool Equals(CachedValue<Ty> other)
        {
            if (other == null) return false;

            return Get().Equals(other.Get());
        }

        public override bool Equals(object other)
        {
            if (other == null) return false;
            if (other is CachedValue<Ty> cache)
            {
                return Get().Equals(cache.Get());
            }
            return false;
        }

        public override int GetHashCode()
        {
            return Get().GetHashCode();
        }
        public override string ToString()
        {
            return Get().ToString();
        }
        #endregion
    }
}
