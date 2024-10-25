using System;
using System.Collections.Generic;

namespace CssUI
{
    public class ReversableDictionary<TKey, TValue> : Dictionary<TKey, TValue>, IReversableDictionary<TKey, TValue>
    {
        #region Properties
        Dictionary<TValue, TKey> MapInverse = new Dictionary<TValue, TKey>();
        #endregion

        #region Constructors
        public ReversableDictionary()
        {
        }
        #endregion

        public new void Add(TKey key, TValue value)
        {
            base.Add(key, value);
            MapInverse.Add(value, key);
        }

        public new void Remove(TKey key, out TValue outValue)
        {
            if (base.TryGetValue(key, out TValue value))
            {
                throw new Exception($"Unable to find key-value entry from dictionary! ValueType: {typeof(TValue).FullName}");
            }

            base.Remove(key);
            MapInverse.Remove(value);
            outValue = value;
        }


        public bool RemoveInverse(TValue value)
        {
            if (MapInverse.TryGetValue(value, out TKey key))
            {
                throw new Exception($"Unable to find value-key entry from inverse dictionary! ValueType: {typeof(TValue).FullName}");
            }

            if (!MapInverse.Remove(value))
                return false;
            if (!base.Remove(key))
                return false;

            return true;
        }

        public bool RemoveInverse(TValue value, out TKey outKey)
        {
            if (MapInverse.TryGetValue(value, out TKey key))
            {
                throw new Exception($"Unable to find value-key entry from inverse dictionary! ValueType: {typeof(TValue).FullName}");
            }
            outKey = key;

            if (!MapInverse.Remove(value))
                return false;
            if (!base.Remove(key))
                return false;

            return true;
        }

        /// <summary>
        /// Updates the value for the given <paramref name="key"/> only if the old value matches the <paramref name="comparisonValue"/>
        /// </summary>
        /// <param name="key"></param>
        /// <param name="newValue"></param>
        /// <param name="comparisonValue"></param>
        /// <returns></returns>
        public new bool Update(TKey key, TValue newValue, TValue comparisonValue)
        {
            var oldValue = base[key];
            if (!ReferenceEquals(oldValue, comparisonValue) && !oldValue.Equals(comparisonValue))
                return false;

            base[key] = newValue;
            MapInverse.Remove(oldValue);
            MapInverse.Add(newValue, key);

            return true;
        }

        public bool TryGetKey(TValue value, out TKey key) => MapInverse.TryGetValue(value, out key);

    }
}
