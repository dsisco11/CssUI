using CssUI.DOM.Exceptions;
using CssUI.DOM.Nodes;
using System;
using System.Collections.Generic;

namespace CssUI.DOM
{
    public class Text : CharacterData, ISlottable
    {
        #region Node Overrides
        public override string nodeName => "#text";
        #endregion

        #region Slottable
        private string _name = string.Empty;
        public string Name
        {/* Docs: https://dom.spec.whatwg.org/#slotable-name */
            get => _name;

            set
            {
                var oldValue = _name;
                if (0 == string.Compare(value, oldValue)) return;
                if (value == null && oldValue.Length <= 0) return;
                if (value.Length <= 0 && oldValue == null) return;
                if (string.IsNullOrEmpty(value))
                {
                    _name = string.Empty;
                }
                else
                {
                    _name = value;
                }
                /* 6) If element is assigned, then run assign slotables for element’s assigned slot. */
                if (this.isAssigned)
                {
                    DOMCommon.assign_slottables(assignedSlot);
                }
                /* 7) Run assign a slot for element. */
                DOMCommon.assign_a_slot(this);
            }
        }

        /* Docs: https://dom.spec.whatwg.org/#slotable-assigned-slot */
        public ISlot assignedSlot { get; set; } = null;
        #endregion

        #region Constructors
        public Text(Document ownerDocument, string data) : base(ownerDocument, data)
        {
        }
        #endregion

        public Text splitText(int offset)
        {/* Docs: https://dom.spec.whatwg.org/#concept-text-split */
            if (offset > Length) throw new IndexSizeError();

            var count = Length - offset;
            var newData = this.substringData(offset, count);
            /* 5) Let new node be a new Text node, with the same node document as node. Set new node’s data to new data. */
            var newNode = new Text(ownerDocument, newData);
            var parent = this.parentNode;
            if (!ReferenceEquals(null, parent))
            {
                /* 1) Insert new node into parent before node’s next sibling. */
                Node._insert_node_into_parent_before(newNode, parent, this.nextSibling);
                foreach (WeakReference<Range> weakRef in Range.LIVE_RANGES)
                {
                    if (weakRef.TryGetTarget(out Range liveRange))
                    {
                        /* 2) For each live range whose start node is node and start offset is greater than offset, set its start node to new node and decrease its start offset by offset. */
                        if (ReferenceEquals(this, liveRange.startContainer) && liveRange.startOffset > offset)
                        {
                            liveRange.startContainer = newNode;
                            liveRange.startOffset -= offset;
                        }
                        /* 3) For each live range whose end node is node and end offset is greater than offset, set its end node to new node and decrease its end offset by offset. */
                        if (ReferenceEquals(this, liveRange.endContainer) && liveRange.endOffset > offset)
                        {
                            liveRange.endContainer = newNode;
                            liveRange.endOffset = offset;
                        }
                        /* 4) For each live range whose start node is parent and start offset is equal to the index of node plus 1, increase its start offset by 1. */
                        if (ReferenceEquals(parent, liveRange.startContainer) && liveRange.startOffset == (this.index + 1))
                        {
                            liveRange.startOffset++;
                        }
                        /* 5) For each live range whose end node is parent and end offset is equal to the index of node plus 1, increase its end offset by 1. */
                        if (ReferenceEquals(this, liveRange.endContainer) && liveRange.endOffset == (this.index + 1))
                        {
                            liveRange.endOffset++;
                        }
                    }
                }
            }

            /* 8) Replace data with node node, offset offset, count count, and data the empty string. */
            this.replaceData(offset, count, string.Empty);
            return newNode;
        }

        public override bool Equals(object obj)
        {/* Docs: https://dom.spec.whatwg.org/#concept-node-equals */
            if (!base.Equals(obj))
                return false;

            if (!(obj is Text B))
                return false;

            return 0 == string.Compare(this.data, B.data);
        }

        public override int GetHashCode()
        {
            int hash = base.GetHashCode();
            hash = hash * 31 + (int)data.GetHashCode();
            return hash;
        }

        /// <summary>
        /// Returns the combined data of all direct Text node siblings.
        /// </summary>
        public string wholeText
        {
            get
            {
                var nodes = Text.get_contiguous_text_nodes(this);
                string whole = string.Empty;
                foreach(Text t in nodes)
                {
                    whole = string.Concat(whole, t.data);
                }

                return whole;
            }
        }

        #region Internal Utility
        /// <summary>
        /// Returns all of the directly adjacent <see cref="Text"/> nodes to the specified node
        /// </summary>
        /// <param name="node">The text-node to start from</param>
        /// <param name="exclude_self">If true the specified node will be left out of the returned list</param>
        /// <returns></returns>
        internal static IEnumerable<Text> get_contiguous_text_nodes(Text node, bool exclude_self = false)
        {
            /* The contiguous Text nodes of a node node are node, node’s previous sibling Text node, if any, and its contiguous Text nodes, and node’s next sibling Text node, if any, and its contiguous Text nodes, avoiding any duplicates. */
            var nodeList = new LinkedList<Text>();
            Node currentNode = node;
            do
            {
                currentNode = currentNode.previousSibling;
                if (!ReferenceEquals(currentNode, null) && currentNode is Text)
                {
                    nodeList.AddFirst((Text)currentNode);
                }
            }
            while (currentNode != null);
            /* Add our self to the lsit too (maybe) */
            if (!exclude_self)
                nodeList.AddLast((Text)node);

            currentNode = node;
            do
            {
                currentNode = currentNode.nextSibling;
                if (!ReferenceEquals(currentNode, null) && currentNode is Text)
                {
                    nodeList.AddLast((Text)currentNode);
                }
            }
            while (currentNode != null);

            return nodeList;
        }
        #endregion
    }
}
