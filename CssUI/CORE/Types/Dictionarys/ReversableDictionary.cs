using System;
using System.Collections.Generic;

namespace CssUI
{
    public class ReversableDictionary<KeyTy, ValueTy> : Dictionary<KeyTy, ValueTy>
    {
        #region Properties
        Dictionary<ValueTy, KeyTy> MapInverse = new Dictionary<ValueTy, KeyTy>();
        #endregion

        #region Constructors
        public ReversableDictionary()
        {
        }
        #endregion

        public new void Add(KeyTy key, ValueTy value)
        {
            base.Add(key, value);
            MapInverse.Add(value, key);
        }

        public new void Remove(KeyTy key, out ValueTy outValue)
        {
            if (base.TryGetValue(key, out ValueTy value))
            {
                throw new Exception($"Unable to find key-value entry from dictionary! ValueType: {typeof(ValueTy).FullName}");
            }

            base.Remove(key);
            MapInverse.Remove(value);
            outValue = value;
        }


        public bool RemoveInverse(ValueTy value)
        {
            if (MapInverse.TryGetValue(value, out KeyTy key))
            {
                throw new Exception($"Unable to find value-key entry from inverse dictionary! ValueType: {typeof(ValueTy).FullName}");
            }

            if (!MapInverse.Remove(value))
                return false;
            if (!base.Remove(key))
                return false;

            return true;
        }

        public bool RemoveInverse(ValueTy value, out KeyTy outKey)
        {
            if (MapInverse.TryGetValue(value, out KeyTy key))
            {
                throw new Exception($"Unable to find value-key entry from inverse dictionary! ValueType: {typeof(ValueTy).FullName}");
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
        public new bool Update(KeyTy key, ValueTy newValue, ValueTy comparisonValue)
        {
            var oldValue = base[key];
            if (!ReferenceEquals(oldValue, comparisonValue) && !oldValue.Equals(comparisonValue))
                return false;

            base[key] = newValue;
            MapInverse.Remove(oldValue);
            MapInverse.Add(newValue, key);

            return true;
        }

        public bool TryGetKey(ValueTy value, out KeyTy key) => MapInverse.TryGetValue(value, out key);

    }
}
