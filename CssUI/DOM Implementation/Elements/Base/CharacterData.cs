using CssUI.DOM.Enums;
using CssUI.DOM.Exceptions;
using CssUI.DOM.Mutation;
using CssUI.DOM.Nodes;
using System.Runtime.CompilerServices;

namespace CssUI.DOM
{
    public abstract class CharacterData : Node, INonDocumentTypeChildNode
    {/* Docs: https://dom.spec.whatwg.org/#interface-characterdata */
        #region Node Implementation
        public override ENodeType nodeType => ENodeType.TEXT_NODE;
        public override string nodeName => "#text";
        public override string nodeValue { get => data; set => replace_data(0, Length, value ?? string.Empty); }
        public override string textContent { get => data; set => replace_data(0, Length, value ?? string.Empty); }

        public override int nodeLength => data.Length;
        #endregion

        #region Properties
        public string data;
        public int Length => data.Length;//{ get; private set; }
        #endregion

        #region Constructors
        public CharacterData(Document ownerDocument, string data)
        {
            this.nodeDocument = ownerDocument;
            this.data = data;
        }
        #endregion

        #region Utility
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void replace_data(int offset, int count, string data)
        {
            /* To replace data of node node with offset offset, count count, and data data, run these steps: */
            if (offset > Length) throw new IndexSizeError();
            /* 3) If offset plus count is greater than length, then set count to length minus offset. */
            if ((offset + count) > Length) count = (Length - offset);
            /* 4) Queue a mutation record of "characterData" for node with null, null, node’s data, « », « », null, and null. */
            MutationRecord.Queue_Text_Mutation_Record(this, this.data);
            /* 5) Insert data into node’s data after offset code units. */
            string newData = this.data.Insert(offset, data);
            /* 6) Let delete offset be offset + data’s length. */
            var delete = offset + nodeLength;
            /* 7) Starting from delete offset code units, remove count code units from node’s data. */
            this.data = newData.Remove(delete, count);
            foreach (var weakRef in nodeDocument.LIVE_RANGES)
            {
                if (weakRef.TryGetTarget(out Range liveRange))
                {
                    /* 8) For each live range whose start node is node and start offset is greater than offset but less than or equal to offset plus count, set its start offset to offset. */
                    if (ReferenceEquals(this, liveRange.startContainer) && liveRange.startOffset > offset && liveRange.startOffset < (offset + count))
                    {
                        liveRange.startOffset = offset;
                    }
                    /* 9) For each live range whose end node is node and end offset is greater than offset but less than or equal to offset plus count, set its end offset to offset. */
                    if (ReferenceEquals(this, liveRange.endContainer) && liveRange.endOffset > offset && liveRange.endOffset < (offset + count))
                    {
                        liveRange.endOffset = offset;
                    }
                    /* 10) For each live range whose start node is node and start offset is greater than offset plus count, increase its start offset by data’s length and decrease it by count. */
                    if (ReferenceEquals(this, liveRange.startContainer) && liveRange.startOffset > (offset + count))
                    {
                        liveRange.startOffset += (nodeLength - count);
                    }
                    /* 11) For each live range whose end node is node and end offset is greater than offset plus count, increase its end offset by data’s length and decrease it by count. */
                    if (ReferenceEquals(this, liveRange.endContainer) && liveRange.endOffset > (offset + count))
                    {
                        liveRange.endOffset += (nodeLength - count);
                    }
                }
                else
                {
                    Log.Error("Found expired reference to Range object in Live Ranges.");
                }
            }
            /* 12) If node is a Text node and its parent is not null, run the child text content change steps for node’s parent. */
            if (this is Text txtNode && parentNode != null)
            {
                parentNode.run_child_text_node_change_steps(txtNode);
            }
        }
        #endregion

        #region INonDocumentTypeChildNode Implementation
        public Element previousElementSibling
        {
            get
            {
                Node n = previousSibling;
                while((n != null) && !(n is Element)) { n = n.previousSibling; }
                return n as Element;
            }
        }

        public Element nextElementSibling
        {
            get
            {
                Node n = nextSibling;
                while ((n != null) && !(n is Element)) { n = n.nextSibling; }
                return n as Element;
            }
        }
        #endregion

        public string substringData(int offset, int count)
        {
            /* To substring data with node node, offset offset, and count count, run these steps: */
            /* 1) Let length be node’s length. */
            /* 2) If offset is greater than length, then throw an "IndexSizeError" DOMException. */
            if (offset > Length) throw new IndexSizeError();
            /* 3) If offset plus count is greater than length, return a string whose value is the code units from the offsetth code unit to the end of node’s data, and then return. */
            if ((offset + count) > Length)
            {
                return data.Substring(offset, (Length - offset));
            }
            /* 4) Return a string whose value is the code units from the offsetth code unit to the offset+countth code unit in node’s data. */
            return data.Substring(offset, count);
        }

        public void appendData(string data)
        {
            replace_data(Length, 0, data);
        }

        public void insertData(int offset, string data)
        {
            replace_data(offset, 0, data);
        }

        public void deleteData(int offset, int count)
        {
            replace_data(offset, count, string.Empty);
        }

        public void replaceData(int offset, int count, string data)
        {
            replace_data(offset, count, data);
        }
    }
}
