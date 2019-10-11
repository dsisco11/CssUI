using CssUI.DOM.Enums;
using CssUI.DOM.Nodes;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace CssUI.DOM
{
    [Obsolete("USE DOMCommon.Get_Descendents_OfType<NodeType>(root, filter) it's the same thing", true)]
    public class NodeList<NodeType> : IEnumerable<NodeType> where NodeType : Node
    {
        #region Properties
        public readonly Node Root;
        private readonly NodeFilter CollectionFilter = null;
        #endregion

        #region Constructors
        public NodeList(Node root)
        {
            Root = root;
        }

        public NodeList(Node root, NodeFilter filter)
        {
            Root = root;
            CollectionFilter = filter;
        }
        #endregion

        #region Accessors
        protected IReadOnlyList<NodeType> Collection => DOMCommon.Get_Descendents<NodeType>(Root, CollectionFilter, ENodeFilterMask.SHOW_ELEMENT).ToArray();


        /// <summary>
        /// Returns the number of nodes in the collection.
        /// </summary>
        public virtual int length
        {
            get => Collection.Count;
            set { }
        }

        /// <summary>
        /// Returns the node with index index from the collection. The nodes are sorted in tree order.
        /// </summary>
        public virtual NodeType this[int index]
        {
            get => Collection[index];
            set { }
        }

        #endregion


        #region IEnumerable Implementation
        public IEnumerator<NodeType> GetEnumerator()
        {
            return Collection.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return Collection.GetEnumerator();
        }
        #endregion
    }
}
