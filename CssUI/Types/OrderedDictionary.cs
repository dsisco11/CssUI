using System.Collections;
using System.Collections.Generic;

namespace CssUI
{
    /// <summary>
    /// Implementation of an ordered/indexed dictionary backed by a list.
    /// </summary>
    /// <typeparam name="keyTy"></typeparam>
    /// <typeparam name="valueTy"></typeparam>
    public class OrderedDictionary<keyTy, valueTy> : IEnumerable<valueTy>
    {
        #region Properties
        private ReversableDictionary<keyTy, int> KeyIndex = new ReversableDictionary<keyTy, int>();
        private List<valueTy> Items = new List<valueTy>();
        #endregion

        #region Accessors
        public int Count => ((IList<valueTy>)Items).Count;
        public bool IsReadOnly => ((IList<valueTy>)Items).IsReadOnly;
        public valueTy this[int index]
        {
            get => ((IList<valueTy>)Items)[index];
            set => ((IList<valueTy>)Items)[index] = value;
        }

        public valueTy this[keyTy key]
        {
            get => ((IList<valueTy>)Items)[KeyIndex[key]];
            set => ((IList<valueTy>)Items)[KeyIndex[key]] = value;
        }
        #endregion

        #region Constructor
        public OrderedDictionary()
        {
        }
        #endregion

        #region Indexing
        public int IndexOfKey(keyTy key) => KeyIndex[key];
        public int IndexOfValue(valueTy item) => Items.IndexOf(item);
        #endregion

        #region Add / Insert
        public void Add(keyTy key, valueTy item)
        {
            int index = Items.Count;
            Items.Add(item);
            KeyIndex.Add(key, index);
        }

        public void Insert(int index, keyTy key, valueTy item)
        {
            /* Find the item we will displace */
            var displaced = Items[index];
            Items.Insert(index, item);

            /* Find the key of the item we are displacing by looking for it by its value */
            if (!KeyIndex.TryGetKey(index, out keyTy movedKey))
            {
                throw new System.Exception($"Unable to reverse lookup key by value!");
            }

            KeyIndex.Add(key, index);
            KeyIndex.Update(movedKey, index + 1, index);
        }
        #endregion

        #region Removals
        /// <summary>
        /// Removes the key-value pair specified by the given key
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public bool Remove(keyTy key)
        {
            /* 1) Find the index */
            if (!KeyIndex.TryGetValue(key, out int index))
                return false;

            /* 2) Remove the key */
            if (!KeyIndex.Remove(key))
                return false;

            /* 3) Remove value */
            Items.RemoveAt(index);

            return true;
        }

        /// <summary>
        /// Removes the key-value pair specified by the given value
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public bool RemoveValue(valueTy item)
        {
            int index = Items.IndexOf(item);
            /* 1) Remove the value */
            if (!Items.Remove(item))
                return false;

            /* 2) Remove the key */
            if (!KeyIndex.RemoveInverse(index))
                return false;

            return true;
        }

        /// <summary>
        /// Removes the key-value pair specified by the given index
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public bool RemoveAt(int index)
        {
            /* 1) Remove the value */
            Items.RemoveAt(index);
            /* 2) Remove the key */
            if (!KeyIndex.RemoveInverse(index))
                return false;

            return true;
        }

        public void Clear()
        {
            KeyIndex.Clear();
            Items.Clear();
        }
        #endregion

        #region Updates
        public void Update(int index, keyTy newKey, valueTy newValue)
        {
            /* 1) Check if the oldValue and newValue are the same*/
            valueTy oldValue = Items[index];
            if (!oldValue.Equals(newValue))
            {
                /* Update the value */
                Items[index] = newValue;
            }

            /* 2) Check if the oldKey and newKey are the same */
            KeyIndex.TryGetKey(index, out keyTy oldKey);
            if (!oldKey.Equals(newKey))
            {
                /* Update our key map */
                KeyIndex.Remove(oldKey);
                KeyIndex.Add(newKey, index);
            }
        }

        public void Update(keyTy key, valueTy newValue)
        {
            /* 1) Find the values index */
            KeyIndex.TryGetValue(key, out int index);
            /* 2) Update the value */
            Items[index] = newValue;
        }
        #endregion

        #region Querys
        public bool ContainsKey(keyTy key)
        {
            return KeyIndex.ContainsKey(key);
        }

        public bool ContainsValue(valueTy item)
        {
            return ((IList<valueTy>)Items).Contains(item);
        }

        public bool TryGetKey(valueTy value, out keyTy outKey)
        {
            int index = Items.IndexOf(value);
            if (!KeyIndex.TryGetKey(index, out keyTy key))
            {
                outKey = default(keyTy);
                return false;
            }

            outKey = key;
            return true;
        }

        public bool TryGetValue(keyTy key, out valueTy outValue)
        {
            if (!KeyIndex.TryGetValue(key, out int index))
            {
                outValue = default(valueTy);
                return false;
            }

            if (index >= Items.Count)
                throw new System.Exception("Logic error, KeyIndex specified index exceeding the number of object in backing list!");

            outValue = Items[index];
            return true;
        }
        #endregion

        #region Enumeration
        public IEnumerator<valueTy> GetEnumerator()
        {
            return ((IList<valueTy>)Items).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IList<valueTy>)Items).GetEnumerator();
        }
        #endregion

    }
}
