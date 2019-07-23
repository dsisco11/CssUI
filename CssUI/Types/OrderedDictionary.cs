using System;
using System.Collections;
using System.Collections.Generic;

namespace CssUI
{
    /// <summary>
    /// Implementation of an ordered/indexed dictionary backed by a list.
    /// </summary>
    /// <typeparam name="KeyTy"></typeparam>
    /// <typeparam name="ValueTy"></typeparam>
    public class OrderedDictionary<KeyTy, ValueTy> : IEnumerable<ValueTy>
    {
        #region Properties
        private ReversableDictionary<KeyTy, int> KeyIndex = new ReversableDictionary<KeyTy, int>();
        private List<ValueTy> Items = new List<ValueTy>();
        #endregion

        #region Accessors
        public int Count => ((IList<ValueTy>)Items).Count;
        public bool IsReadOnly => ((IList<ValueTy>)Items).IsReadOnly;
        public ValueTy this[int index]
        {
            get => ((IList<ValueTy>)Items)[index];
            set => ((IList<ValueTy>)Items)[index] = value;
        }

        public ValueTy this[KeyTy key]
        {
            get => ((IList<ValueTy>)Items)[KeyIndex[key]];
            set => ((IList<ValueTy>)Items)[KeyIndex[key]] = value;
        }
        #endregion

        #region Constructor
        public OrderedDictionary()
        {
        }
        #endregion

        #region Indexing
        public int IndexOfKey(KeyTy key) => KeyIndex[key];
        public int IndexOfValue(ValueTy item) => Items.IndexOf(item);
        #endregion

        #region Add / Insert
        public void Add(KeyTy key, ValueTy item)
        {
            int index = Items.Count;
            Items.Add(item);
            KeyIndex.Add(key, index);
        }

        public void Insert(int index, KeyTy key, ValueTy item)
        {
            /* Find the item we will displace */
            var displaced = Items[index];
            Items.Insert(index, item);

            /* Find the key of the item we are displacing by looking for it by its value */
            if (!KeyIndex.TryGetKey(index, out KeyTy movedKey))
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
        public bool Remove(KeyTy key)
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
        public bool RemoveValue(ValueTy item)
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
        public void Update(int index, KeyTy newKey, ValueTy newValue)
        {
            /* 1) Check if the oldValue and newValue are the same*/
            ValueTy oldValue = Items[index];
            if (!oldValue.Equals(newValue))
            {
                /* Update the value */
                Items[index] = newValue;
            }

            /* 2) Check if the oldKey and newKey are the same */
            KeyIndex.TryGetKey(index, out KeyTy oldKey);
            if (!oldKey.Equals(newKey))
            {
                /* Update our key map */
                KeyIndex.Remove(oldKey);
                KeyIndex.Add(newKey, index);
            }
        }

        public void Update(KeyTy key, ValueTy newValue)
        {
            /* 1) Find the values index */
            KeyIndex.TryGetValue(key, out int index);
            /* 2) Update the value */
            Items[index] = newValue;
        }
        #endregion

        #region Querys
        public bool ContainsKey(KeyTy key)
        {
            return KeyIndex.ContainsKey(key);
        }

        public bool ContainsValue(ValueTy item)
        {
            return ((IList<ValueTy>)Items).Contains(item);
        }

        public bool TryGetKey(ValueTy value, out KeyTy outKey)
        {
            int index = Items.IndexOf(value);
            if (!KeyIndex.TryGetKey(index, out KeyTy key))
            {
                outKey = default(KeyTy);
                return false;
            }

            outKey = key;
            return true;
        }

        public bool TryGetValue(KeyTy key, out ValueTy outValue)
        {
            if (!KeyIndex.TryGetValue(key, out int index))
            {
                outValue = default(ValueTy);
                return false;
            }

            if (index >= Items.Count)
                throw new System.Exception("Logic error, KeyIndex specified index exceeding the number of object in backing list!");

            outValue = Items[index];
            return true;
        }
        #endregion

        #region Enumeration
        public IEnumerator<ValueTy> GetEnumerator()
        {
            return ((IList<ValueTy>)Items).GetEnumerator();
        }

        IEnumerator<ValueTy> IEnumerable<ValueTy>.GetEnumerator()
        {
            return ((IEnumerable<ValueTy>)Items).GetEnumerator();
        }
        /*
       /// <summary>
       /// Enumerates the elements of an ordered dictionary
       /// </summary>
       public struct Enumerator : IEnumerator<KeyValuePair<KeyTy, ValueTy>>, IEnumerator, IDisposable, IDictionaryEnumerator
       {
           /// <summary>
           /// Gets the element at the current position of the enumerator.
           /// </summary>
           /// <returns>The element in the <see cref="OrderedDictionary{keyTy, valueTy}"/> at the current position of the enumerator</returns>
           public KeyValuePair<KeyTy, ValueTy> Current { get => new KeyValuePair<KeyTy, ValueTy>((KeyTy)Key, (ValueTy)Value); }

           public DictionaryEntry Entry;

           public object Key => Entry.Key;

           public object Value => Entry.Value;

           object IEnumerator.Current => Current;

           private IEnumerator<KeyValuePair<KeyTy, uint>> enumerator;

           /// <summary>
           /// Releases all resources used by the <see cref="OrderedDictionary{keyTy, valueTy}.Enumerator"/>
           /// </summary>
           public void Dispose()
           {
           }

           /// <summary>
           /// Advances the enumerator to the next element of the <see cref="OrderedDictionary{KeyTy, ValueTy}"/>
           /// </summary>
           /// <returns><c>True</c> if the enumerator was successfully advanced to the next element; <c>False</c> if the enumerator has passed the end of the collection.</returns>
           /// <exception cref="InvalidOperationException">The collection was modified after the enumerator was created.</exception>
           public bool MoveNext()
           {
           }

           public void Reset()
           {
           }
       }
*/
        #endregion

    }
}
