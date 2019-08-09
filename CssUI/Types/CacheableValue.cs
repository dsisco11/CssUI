using System;

namespace CssUI
{
    /// <summary>
    /// Simplifies the process of resolving a value once and caching it for future references
    /// </summary>
    public class CacheableValue<Ty>
    {
        #region Properties
        public bool IsCached { get; private set; } = false;
        private Ty value = default(Ty);
        private readonly Func<Ty> Resolver = null;
        #endregion

        #region Constructors
        public CacheableValue()
        {
        }

        public CacheableValue(Func<Ty> Resolver)
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
    }
}
