using System;

namespace CssUI
{
    /// <summary>
    /// Simplifies the process of resolving a value once and caching it for future references
    /// </summary>
    public class CacheableValue<Ty>
    {
        public bool IsCached { get; private set; } = false;
        Ty value = default(Ty);

        public CacheableValue()
        {
        }

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
