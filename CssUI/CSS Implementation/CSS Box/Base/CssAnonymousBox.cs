
using System.Collections;
using System.Collections.Generic;

namespace CssUI.CSS
{
    public class CssAnonymousBox : CssBoxFragment, IList<CssBoxFragment>
    {
        #region Backing List
        private List<CssBoxFragment> Children = new List<CssBoxFragment>();
        #endregion

        #region Constructor
        public CssAnonymousBox()
        {
        }
        #endregion

        #region List Implementation
        public CssBoxFragment this[int index] { get => ((IList<CssBoxFragment>)Children)[index]; set => ((IList<CssBoxFragment>)Children)[index] = value; }

        public int Count => ((IList<CssBoxFragment>)Children).Count;

        public bool IsReadOnly => ((IList<CssBoxFragment>)Children).IsReadOnly;

        public void Add(CssBoxFragment item)
        {
            ((IList<CssBoxFragment>)Children).Add(item);
        }

        public void Clear()
        {
            ((IList<CssBoxFragment>)Children).Clear();
        }

        public bool Contains(CssBoxFragment item)
        {
            return ((IList<CssBoxFragment>)Children).Contains(item);
        }

        public void CopyTo(CssBoxFragment[] array, int arrayIndex)
        {
            ((IList<CssBoxFragment>)Children).CopyTo(array, arrayIndex);
        }

        public IEnumerator<CssBoxFragment> GetEnumerator()
        {
            return ((IEnumerable<CssBoxFragment>)Children).GetEnumerator();
        }

        public int IndexOf(CssBoxFragment item)
        {
            return ((IList<CssBoxFragment>)Children).IndexOf(item);
        }

        public void Insert(int index, CssBoxFragment item)
        {
            ((IList<CssBoxFragment>)Children).Insert(index, item);
        }

        public bool Remove(CssBoxFragment item)
        {
            return ((IList<CssBoxFragment>)Children).Remove(item);
        }

        public void RemoveAt(int index)
        {
            ((IList<CssBoxFragment>)Children).RemoveAt(index);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable<CssBoxFragment>)Children).GetEnumerator();
        }
        #endregion


    }
}
