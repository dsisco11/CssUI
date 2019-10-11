using System;
using System.Collections.Generic;
using System.Text;
using CssUI.CSS;
using CssUI.CSS.BoxTree;
using CssUI.DOM.Exceptions;
using CssUI.DOM.Nodes;

namespace CssUI.DOM
{
    /* 
     * Docs: https://www.w3.org/TR/DOM-Level-3-Core/core.html#ID-1312295772
     */

    public class Text : CharacterData, ISlottable
    {
        #region Properties
        #region Node Overrides
        public override string nodeName => "#text";
        #endregion

        #region Slottable
#if ENABLE_HTML
        private string _name = string.Empty;
        public string Slot_Name
        {/* Docs: https://dom.spec.whatwg.org/#slotable-name */
            get => _name;

            set
            {
                var oldValue = _name;
                if (oldValue.Equals(value)) return;
                if (value is null && oldValue.Length <= 0) return;
                if (value.Length <= 0 && oldValue is null) return;
                if (string.IsNullOrEmpty(value))
                {
                    _name = string.Empty;
                }
                else
                {
                    _name = value;
                }
                /* 6) If element is assigned, then run assign slotables for element’s assigned slot. */
                if (isAssigned)
                {
                    DOMCommon.Assign_Slottables(assignedSlot);
                }
                /* 7) Run assign a slot for element. */
                DOMCommon.Assign_A_Slot(this);
            }
        }

        /* Docs: https://dom.spec.whatwg.org/#slotable-assigned-slot */
        public ISlot assignedSlot { get; set; } = null;
#endif
        #endregion

        #region CSS
        public new CssTextRun Box
        {
            get
            {
                return (CssTextRun)base.Box;
            }
            set
            {
                base.Box = value;
            }
        }
        #endregion
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
            var newData = substringData(offset, count);
            /* 5) Let new node be a new Text node, with the same node document as node. Set new node’s data to new data. */
            var newNode = new Text(ownerDocument, newData);
            var parent = parentNode;
            if (parent != null)
            {
                /* 1) Insert new node into parent before node’s next sibling. */
                parent.insertBefore(newNode, nextSibling);
                foreach (WeakReference<Range> weakRef in nodeDocument.LIVE_RANGES)
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

        /// <summary>
        /// Returns the combined data of all direct Text node siblings.
        /// </summary>
        public string wholeText
        {
            get
            {
                var nodes = Text.get_contiguous_text_nodes(this);
                StringBuilder sb = new StringBuilder();
                foreach (Text t in nodes)
                {
                    sb.Append(t.data);
                }

                return sb.ToString();
            }
        }


        #region Internal States
        /// <summary>
        /// A node (in particular elements and text nodes) can be marked as inert. 
        /// When a node is inert, then the user agent must act as if the node was absent for the purposes of targeting user interaction events, 
        /// may ignore the node for the purposes of text search user interfaces (commonly known as "find in page"), 
        /// and may prevent the user from selecting text in that node. User agents should allow the user to override the restrictions on search and text selection, however.
        /// </summary>
        internal new bool is_expressly_inert => (base.isExpresslyInert || parentElement.isInert);
        //internal bool is_expressly_inert => (this.inert && !(parentNode is Element E && E.inert) && !(parentNode is Text T && T.inert));
        #endregion

        #region Internal Utility
        /// <summary>
        /// Returns all of the directly adjacent <see cref="Text"/> nodes to the specified node
        /// </summary>
        /// <param name="node">The text-node to start from</param>
        /// <param name="exclude_self">If true the specified node will be left out of the returned list</param>
        /// <returns></returns>
        internal static LinkedList<Text> get_contiguous_text_nodes(in Text node, bool exclude_self = false)
        {
            /* The contiguous Text nodes of a node node are node, node’s previous sibling Text node, if any, and its contiguous Text nodes, and node’s next sibling Text node, if any, and its contiguous Text nodes, avoiding any duplicates. */
            var RetList = new LinkedList<Text>();
            Node currentNode = node;
            // 1) Add all previous text-node siblings to the list
            do
            {
                currentNode = currentNode.previousSibling;
                if (currentNode is Text)
                {
                    RetList.AddFirst((Text)currentNode);
                }
            }
            while (currentNode is object);
            // 2) Add our self to the list too (maybe)
            if (!exclude_self)
                RetList.AddLast(node);

            // 3) Add all following text-node siblings to the list
            currentNode = node;
            do
            {
                currentNode = currentNode.nextSibling;
                if (currentNode is Text)
                {
                    RetList.AddLast((Text)currentNode);
                }
            }
            while (currentNode is object);

            return RetList;
        }
        #endregion

        #region Object Overrides
        public override bool Equals(object obj)
        {/* Docs: https://dom.spec.whatwg.org/#concept-node-equals */
            if (!base.Equals(obj))
                return false;

            if (!(obj is Text B))
                return false;

            return data.Equals(B.data);
        }

        public override int GetHashCode()
        {
            int hash = base.GetHashCode();
            hash = hash * 31 + (int)data.GetHashCode();
            return hash;
        }
        #endregion
    }
}
