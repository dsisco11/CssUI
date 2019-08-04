using CssUI.DOM.CustomElements;
using CssUI.DOM.Exceptions;
using CssUI.DOM.Nodes;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace CssUI.DOM
{
    public abstract class HTMLCollection<ElementType> : IEnumerable<ElementType> where ElementType : HTMLElement
    {/* Docs: https://html.spec.whatwg.org/multipage/common-dom-interfaces.html#htmloptionscollection */
        #region Properties
        public readonly HTMLElement root;
        protected readonly NodeFilter CollectionFilter = null;
        #endregion

        #region Constructor
        public HTMLCollection(HTMLElement root)
        {
            this.root = root;
        }

        protected HTMLCollection(HTMLElement root, NodeFilter CollectionFilter) : this(root)
        {
            this.CollectionFilter = CollectionFilter;
        }
        #endregion

        #region Accessors
        protected IReadOnlyList<ElementType> Collection => DOMCommon.Get_Descendents_OfType<ElementType>(root, CollectionFilter, Enums.ENodeFilterMask.SHOW_ELEMENT).ToArray();
        #endregion

        /// <summary>
        /// Returns the number of elements in the collection.
        /// When set to a smaller number, truncates the number of option elements in the corresponding container.
        /// When set to a greater number, adds new blank option elements to that container.
        /// </summary>
        [CEReactions]
        public virtual int length
        {
            get => Collection.Count;
            set { }
        }

        /// <summary>
        /// Returns the item with index index from the collection. The items are sorted in tree order.
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        [CEReactions] public virtual ElementType this[int index]
        {
            get => Collection[index];
            set { }
        }

        /// <summary>
        /// Returns the item with ID or name name from the collection.
        /// If there are multiple matching items, then the first is returned.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public virtual ElementType this[string name]
        {
            get
            {
                if (ReferenceEquals(null, name) || name.Length <= 0)
                {
                    return null;
                }

                return DOMCommon.Get_Nth_Descendant_OfType<ElementType>(root, 1, new FilterNamedElement(name), Enums.ENodeFilterMask.SHOW_ELEMENT);
            }
        }


        #region IEnumerable Implementation
        public IEnumerator<ElementType> GetEnumerator()
        {
            return ((IEnumerable<ElementType>)Collection).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable<ElementType>)Collection).GetEnumerator();
        }
        #endregion

    }
}
