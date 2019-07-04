using System.Linq;
using CssUI.CSS;
using System.Collections;
using System.Collections.Generic;
using System;

namespace CssUI.Internal
{
    /// <summary>
    /// Used to contain and track multiple <see cref="CssValue"/>s
    /// </summary>
    public class CssValueList : IEnumerable<CssValue>
    {
        #region Proprties
        private int? hash_cache = null;
        private List<CssValue> Items = new List<CssValue>();

        public readonly bool IsReadOnly = true;
        #endregion


        #region Constructor
        public CssValueList()
        {
        }

        public CssValueList(CssValue singleValue)
        {
            this.Items.Add(singleValue);
        }

        /// <summary>
        /// Creates a new <see cref="CssValueList"/> as a deep copy of another
        /// </summary>
        /// <param name="cssValues"></param>
        public CssValueList(CssValueList cssValues)
        {
            foreach (CssValue cssValue in cssValues)
            {
                this.Items.Add(new CssValue(cssValue));
            }
        }

        /// <summary>
        /// Creates a new <see cref="CssValueList"/> populated by a given set of <see cref="CssValue"/>s
        /// </summary>
        /// <param name="cssValues"></param>
        public CssValueList(IEnumerable<CssValue> cssValues)
        {
            foreach (CssValue cssValue in cssValues)
            {
                this.Items.Add(cssValue);
            }
        }
        #endregion

        #region IList Implementation

        public CssValue this[int index]
        {
            get => ((IList<CssValue>)Items)[index];
        }

        public int Count => ((IList<CssValue>)Items).Count;

        public bool Contains(CssValue item)
        {
            return ((IList<CssValue>)Items).Contains(item);
        }

        public int IndexOf(CssValue item)
        {
            return ((IList<CssValue>)Items).IndexOf(item);
        }

        public IEnumerator<CssValue> GetEnumerator()
        {
            return ((IEnumerable<CssValue>)Items).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable<CssValue>)Items).GetEnumerator();
        }

        #endregion

        #region LINQ Implementation
        public IEnumerable<T> Select<T>(Func<CssValue, T> predicate)
        {
            foreach(CssValue cssValue in this)
            {
                yield return predicate(cssValue);
            }
        }
        #endregion

        #region Operators
        public static bool operator ==(CssValueList A, CssValueList B)
        {
            // If either object is null return whether they are BOTH null
            if (object.ReferenceEquals(A, null) || object.ReferenceEquals(B, null))
                return (object.ReferenceEquals(A, null) && object.ReferenceEquals(B, null));

            return A.GetHashCode() == B.GetHashCode();
        }
        public static bool operator !=(CssValueList A, CssValueList B)
        {
            // If either object is null return whether they are BOTH null
            if (object.ReferenceEquals(A, null) || object.ReferenceEquals(B, null))
                return !(object.ReferenceEquals(A, null) && object.ReferenceEquals(B, null));

            return A.GetHashCode() != B.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            return obj is CssValueList list && (GetHashCode() == list.GetHashCode());
        }
        #endregion

        #region Hashing
        public override int GetHashCode()
        {
            if (hash_cache != null)
                return hash_cache.Value;

            int hash = 17;//magic number
            foreach(CssValue obj in Items)
            {
                hash = hash * 31 + obj.GetHashCode();
            }

            hash_cache = hash;
            return hash;
        }
        #endregion

    }
}
