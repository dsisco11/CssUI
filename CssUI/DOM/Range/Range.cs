using CssUI.DOM.Enums;
using CssUI.DOM.Exceptions;
using CssUI.DOM.Geometry;
using CssUI.DOM.Nodes;
using CssUI.DOM.Traversal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using CssUI.Fonts;

namespace CssUI.DOM
{
    /// <summary>
    /// Objects implementing the Range interface are known as live ranges.
    /// </summary>
    public class Range : AbstractRange
    {/* Docs: https://dom.spec.whatwg.org/#interface-range */

#region Properties
#endregion

#region Accessors
        internal BoundaryPoint start { get => new BoundaryPoint() { node = startContainer, offset = startOffset }; }
        internal BoundaryPoint end { get => new BoundaryPoint() { node = endContainer, offset = endOffset }; }
        public Node root { get => startContainer?.getRootNode(); }
        public Node commonAncestorContainer
        {
            get
            {
                var container = startContainer;
                while (!ReferenceEquals(endContainer, container) && container.compareDocumentPosition(endContainer) != EDocumentPosition.CONTAINED_BY)
                {
                    container = container.parentNode;
                }
                return container;
            }
        }
#endregion

#region Constructors
        private Range()
        {
            /* Add self to list of live ranges */
            root.nodeDocument.LIVE_RANGES.AddLast(new WeakReference<Range>(this));
        }
        public Range(Document document) : this()
        {
            /*  */
            startContainer = endContainer = document.body;
            startOffset = endOffset = 0;
        }

        public Range(Node startNode, int startOffset, Node endNode, int endOffset) : this()
        {
            startContainer = startNode;
            endContainer = endNode;
            this.startOffset = startOffset;
            this.endOffset = endOffset;
        }

        ~Range()
        {
            /* Remove self from list of live ranges */
            foreach (WeakReference<Range> weakRef in root.nodeDocument.LIVE_RANGES)
            {
                if (weakRef.TryGetTarget(out Range r))
                {
                    if (ReferenceEquals(this, r))
                    {
                        root.nodeDocument.LIVE_RANGES.Remove(weakRef);
                        break;
                    }
                }
            }
        }
#endregion
        
#region Utility
        /// <summary>
        /// A node is partially contained in a live range if it’s an inclusive ancestor of the live range’s start node but not its end node, or vice versa.
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        public bool PartiallyContains(Node node)
        {
            return (DOMCommon.Is_Inclusive_Ancestor(node, startContainer) ^ DOMCommon.Is_Inclusive_Ancestor(node, endContainer));
        }

        /// <summary>
        /// Returns whether a given node is 'contained' within this range according to the W3C specifications
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Contains(Node node)
        {
            /* A node node is contained in a live range range if node’s root is range’s root, and (node, 0) is after range’s start, and (node, node’s length) is before range’s end. */
            if (!ReferenceEquals(node.getRootNode(), root))
                return false;

            var st = new BoundaryPoint() { node = node, offset = 0 };
            if (Get_Boundary_Position(start, st) != EBoundaryPosition.After)
                return false;

            var en = new BoundaryPoint() { node = node, offset = node.nodeLength };
            if (Get_Boundary_Position(end, en) != EBoundaryPosition.Before)
                return false;

            return true;
        }

        /// <summary>
        /// Returns an enum indicating if <paramref name="B"/> if Before, Equal, or After <paramref name="A"/>
        /// </summary>
        /// <param name="A"></param>
        /// <param name="B"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static EBoundaryPosition Get_Boundary_Position(BoundaryPoint A, BoundaryPoint B)
        {
            Node nodeA = A.node;
            Node nodeB = B.node;
            var offsetA = A.offset;
            var offsetB = B.offset;
            /* The position of a boundary point (nodeA, offsetA) relative to a boundary point (nodeB, offsetB) is before, equal, or after, as returned by these steps: */
            if (!ReferenceEquals(nodeA.getRootNode(), nodeB.getRootNode()))
                throw new DOMException("Boundarys roots do not match!");
            /* 2) If nodeA is nodeB, then return equal if offsetA is offsetB, before if offsetA is less than offsetB, and after if offsetA is greater than offsetB. */
            if (ReferenceEquals(nodeA, nodeB))
            {
                if (offsetA == offsetB) return EBoundaryPosition.Equal;
                else if (offsetA < offsetB) return EBoundaryPosition.Before;
                else return EBoundaryPosition.After;
            }
            /* 3) If nodeA is following nodeB, then if the position of (nodeB, offsetB) relative to (nodeA, offsetA) is before, return after, and if it is after, return before. */
            var docPos = nodeB.compareDocumentPosition(nodeA);
            if (docPos == EDocumentPosition.FOLLOWING)
            {
                EBoundaryPosition pos = Get_Boundary_Position(B, A);
                if (pos == EBoundaryPosition.Before) return EBoundaryPosition.After;
                else if (pos == EBoundaryPosition.After) return EBoundaryPosition.Before;
            }
            /* 4) If nodeA is an ancestor of nodeB: */
            if (docPos == EDocumentPosition.CONTAINED_BY)
            {
                var child = nodeB;
                /* 2) While child is not a child of nodeA, set child to its parent. */
                while (!ReferenceEquals(child.parentNode, nodeA))
                {
                    child = child.parentNode;
                }
                /* 3) If child’s index is less than offsetA, then return after. */
                if (offsetA > child.index) return EBoundaryPosition.After;
            }

            return EBoundaryPosition.Before;
        }
#endregion

        public void setStart(Node node, int offset)
        {
            if (node is DocumentType) throw new InvalidNodeTypeError();
            if (offset > node.nodeLength) throw new IndexSizeError();

            var bp = new BoundaryPoint(node, offset);
            /* 1) If bp is after the range’s end, or if range’s root is not equal to node’s root, set range’s end to bp. */
            var bpPos = Get_Boundary_Position(bp, new BoundaryPoint() { node = endContainer, offset = endOffset });
            if (bpPos == EBoundaryPosition.After || !ReferenceEquals(node.getRootNode(), root))
            {
                endContainer = bp.node;
                endOffset = bp.offset;
            }
            else /* 2) Set range’s start to bp. */
            {
                startContainer = bp.node;
                startOffset = bp.offset;
            }
        }

        public void setEnd(Node node, int offset)
        {
            if (node is DocumentType) throw new InvalidNodeTypeError();
            if (offset > node.nodeLength) throw new IndexSizeError();

            var bp = new BoundaryPoint() { node = node, offset = offset };
            /* 1) If bp is before the range’s start, or if range’s root is not equal to node’s root, set range’s start to bp. */
            var bpPos = Get_Boundary_Position(bp, new BoundaryPoint() { node = startContainer, offset = startOffset });
            if (bpPos == EBoundaryPosition.Before || !ReferenceEquals(node.getRootNode(), root))
            {
                startContainer = bp.node;
                startOffset = bp.offset;
            }
            else /* 2) Set range’s end to bp. */
            {
                endContainer = bp.node;
                endOffset = bp.offset;
            }
        }

        public void setStartBefore(Node node)
        {
            var parent = node.parentNode;
            if (ReferenceEquals(parent, null)) throw new InvalidNodeTypeError();
            /* 3) Set the start of the context object to boundary point (parent, node’s index). */
            startContainer = parent;
            startOffset = node.index;
        }

        public void setStartAfter(Node node)
        {
            var parent = node.parentNode;
            if (ReferenceEquals(parent, null)) throw new InvalidNodeTypeError();
            /* 3) Set the start of the context object to boundary point (parent, node’s index plus 1). */
            startContainer = parent;
            startOffset = node.index + 1;
        }

        public void setEndBefore(Node node)
        {
            var parent = node.parentNode;
            if (ReferenceEquals(parent, null)) throw new InvalidNodeTypeError();
            /* 3) Set the end of the context object to boundary point (parent, node’s index). */
            endContainer = parent;
            endOffset = node.index;
        }

        public void setEndAfter(Node node)
        {
            var parent = node.parentNode;
            if (ReferenceEquals(parent, null)) throw new InvalidNodeTypeError();
            /* 3) Set the end of the context object to boundary point (parent, node’s index plus 1). */
            endContainer = parent;
            endOffset = node.index + 1;
        }

        /// <summary>
        /// Collapses the range so it's start and end points are equal.
        /// </summary>
        /// <param name="toStart"></param>
        public void collapse(bool toStart = false)
        {
            /* The collapse(toStart) method, when invoked, must if toStart is true, set end to start, and set start to end otherwise. */
            if (toStart)
            {
                endContainer = startContainer;
                endOffset = startOffset;
            }
            else
            {
                startContainer = endContainer;
                startOffset = endOffset;
            }
        }

        public void selectNode(Node node)
        {/* Docs: https://dom.spec.whatwg.org/#concept-range-select */
            /* To select a node node within a range range, run these steps: */
            /* 1) Let parent be node’s parent. */
            var parent = node.parentNode;
            /* 2) If parent is null, then throw an "InvalidNodeTypeError" DOMException. */
            if (ReferenceEquals(parent, null)) throw new InvalidNodeTypeError();
            /* 3) Let index be node’s index. */
            var index = node.index;
            /* 4) Set range’s start to boundary point (parent, index). */
            startContainer = parent;
            startOffset = index;
            /* 5) Set range’s end to boundary point (parent, index plus 1). */
            endContainer = parent;
            endOffset = index + 1;
        }

        public void selectNodeContents(Node node)
        {
            if (node is DocumentType) throw new InvalidNodeTypeError();
            var length = node.nodeLength;
            /* 3) Set start to the boundary point (node, 0). */
            startContainer = node;
            startOffset = 0;
            /* 4) Set end to the boundary point (node, length). */
            endContainer = node;
            endOffset = length;
        }

        public EBoundaryPosition compareBoundaryPoints(EBoundaryComparison how, Range sourceRange)
        {
            /* 2) If context object’s root is not the same as sourceRange’s root, then throw a "WrongDocumentError" DOMException. */
            if (!ReferenceEquals(root, sourceRange.root))
                throw new WrongDocumentError();
            /* 3) If how is: */
            BoundaryPoint thisPoint, otherPoint;
            EBoundaryPosition pos;
            switch (how)
            {
                case EBoundaryComparison.START_TO_START:
                    {/* Let this point be the context object’s start. Let other point be sourceRange’s start. */
                        thisPoint = start;
                        otherPoint = sourceRange.start;
                        return Get_Boundary_Position(thisPoint, otherPoint);
                    }
                    break;
                case EBoundaryComparison.START_TO_END:
                    {/* Let this point be the context object’s end. Let other point be sourceRange’s start. */
                        thisPoint = end;
                        otherPoint = sourceRange.start;
                        return Get_Boundary_Position(thisPoint, otherPoint);
                    }
                    break;
                case EBoundaryComparison.END_TO_END:
                    {/* Let this point be the context object’s end. Let other point be sourceRange’s end. */
                        thisPoint = end;
                        otherPoint = sourceRange.end;
                        return Get_Boundary_Position(thisPoint, otherPoint);
                    }
                    break;
                case EBoundaryComparison.END_TO_START:
                    {/* Let this point be the context object’s start. Let other point be sourceRange’s end. */
                        thisPoint = start;
                        otherPoint = sourceRange.end;
                        return Get_Boundary_Position(thisPoint, otherPoint);
                    }
                    break;
            }

            throw new DOMException("Uncaught boundary comparison method");
        }

        [CEReactions]
        public void deleteContents()
        {
            CEReactions.Wrap_CEReaction(root.nodeDocument.defaultView, () =>
            {
                if (collapsed) return;
                /* 2) Let original start node, original start offset, original end node, and original end offset be the context object’s start node, start offset, end node, and end offset, respectively. */
                /* 3) If original start node and original end node are the same, and they are a Text, ProcessingInstruction, or Comment node... */
                if (ReferenceEquals(startContainer, endContainer) && startContainer is CharacterData characterData)
                {
                    /* ...replace data with node original start node, offset original start offset, count original end offset minus original start offset, and data the empty string, and then return. */
                    characterData.replaceData(startOffset, endOffset, string.Empty);
                    return;
                }
                /* 4) Let nodes to remove be a list of all the nodes that are contained in the context object, in tree order, omitting any node whose parent is also contained in the context object. */
                var nodes_to_remove = new LinkedList<Node>();
                var tree = new TreeWalker(startContainer, ENodeFilterMask.SHOW_ALL, new RangeDeleteFilter(this));
                // first look through the start node and all of its descendants
                Node node = tree.nextNode();
                while (!ReferenceEquals(node, null))
                {
                    nodes_to_remove.AddLast(node);
                    node = tree.nextNode();
                }
                // Set the TreeWalkers position back to the start node
                tree = new TreeWalker(startContainer, ENodeFilterMask.SHOW_ALL, new RangeDeleteFilter(this));
                // Now look through all the start nodes siblings & their descendants

                node = tree.nextSibling();
                while (!ReferenceEquals(node, null))
                {
                    nodes_to_remove.AddLast(node);
                    node = tree.nextSibling();
                }
                /* 5) If original start node is an inclusive ancestor of original end node, set new node to original start node and new offset to original start offset. */
                Node newNode = null;
                int newOffset = 0;
                if (DOMCommon.Is_Inclusive_Ancestor(startContainer, endContainer))
                {
                    newNode = startContainer;
                    newOffset = startOffset;
                }
                /* 6) Otherwise */
                else
                {
                    /* 1) Let reference node equal original start node. */
                    Node refNode = startContainer;
                    /* 2) While reference node’s parent is not null and is not an inclusive ancestor of original end node, set reference node to its parent. */
                    /* (W3C NOTE) If reference node’s parent were null, it would be the root of the context object, so would be an inclusive ancestor of original end node, and we could not reach this point. */
                    while (!ReferenceEquals(refNode.parentNode, null) && !DOMCommon.Is_Inclusive_Ancestor(refNode, endContainer))
                    {
                        refNode = refNode.parentNode;
                    }
                    /* 3) Set new node to the parent of reference node, and new offset to one plus the index of reference node. */
                    newNode = refNode.parentNode;
                    newOffset = refNode.index + 1;
                }
                /* 7) If original start node is a Text, ProcessingInstruction, or Comment node, replace data with node original start node, offset original start offset, count original start node’s length minus original start offset, data the empty string. */
                if (startContainer is CharacterData characterData1)
                {
                    characterData1.replaceData(startOffset, startContainer.nodeLength - startOffset, string.Empty);
                }
                /* 8) For each node in nodes to remove, in tree order, remove node from its parent. */
                foreach (Node rmvNode in nodes_to_remove)
                {
                    rmvNode.parentNode.removeChild(rmvNode);
                }
                /* 9) If original end node is a Text, ProcessingInstruction, or Comment node, replace data with node original end node, offset 0, count original end offset and data the empty string. */
                if (endContainer is CharacterData endCD)
                {
                    endCD.replaceData(0, endOffset, string.Empty);
                }
                /* 10) Set start and end to (new node, new offset). */
                startContainer = endContainer = newNode;
                startOffset = endOffset = newOffset;
            });
        }

        [CEReactions]
        public DocumentFragment extractContents()
        {/* Docs: https://dom.spec.whatwg.org/#dom-range-extractcontents */

            return CEReactions.Wrap_CEReaction(root.nodeDocument.defaultView, () =>
            {
                var fragment = new DocumentFragment(null, startContainer.ownerDocument);
                if (collapsed)
                    return fragment;

                /* 4) If original start node is original end node, and they are a Text, ProcessingInstruction, or Comment node: */
                if (ReferenceEquals(startContainer, endContainer) && DOMCommon.Is_CommonTextNode(startContainer))
                {
                    CharacterData clone = (CharacterData)startContainer.cloneNode();
                    /* 2) Set the data of clone to the result of substringing data with node original start node, offset original start offset, and count original end offset minus original start offset. */
                    clone.data = (startContainer as CharacterData).substringData(startOffset, endOffset - startOffset);
                    fragment.appendChild(clone);
                    /* 4) Replace data with node original start node, offset original start offset, count original end offset minus original start offset, and data the empty string. */
                    (startContainer as CharacterData).replaceData(startOffset, endOffset - startOffset, string.Empty);
                    return fragment;
                }
                /* 5) Let common ancestor be original start node. */
                var commonAncestor = startContainer;
                while (!DOMCommon.Is_Inclusive_Ancestor(commonAncestor, endContainer))
                {
                    commonAncestor = commonAncestor.parentNode;
                }
                /* 7) Let first partially contained child be null. */
                Node firstPartiallyContainedChild = null;
                var partialFilter = new FilterRangePartiallyContains(this);
                /* 8) If original start node is not an inclusive ancestor of original end node, set first partially contained child to the first child of common ancestor that is partially contained in range. */
                if (!DOMCommon.Is_Inclusive_Ancestor(startContainer, endContainer))
                {
                    var tree = new TreeWalker(commonAncestor, ENodeFilterMask.SHOW_ALL, partialFilter);
                    firstPartiallyContainedChild = tree.firstChild();
                }
                /* 9) Let last partially contained child be null. */
                Node lastPartiallyContainedChild = null;
                /* 10) If original end node is not an inclusive ancestor of original start node, set last partially contained child to the last child of common ancestor that is partially contained in range. */
                if (!DOMCommon.Is_Inclusive_Ancestor(endContainer, startContainer))
                {
                    var tree = new TreeWalker(commonAncestor, ENodeFilterMask.SHOW_ALL, partialFilter);
                    lastPartiallyContainedChild = tree.lastChild();
                }
                /* 11) Let contained children be a list of all children of common ancestor that are contained in range, in tree order. */
                var containedChildren = DOMCommon.Get_Descendents(commonAncestor, new FilterRangeContains(this));
                /* 12) If any member of contained children is a doctype, then throw a "HierarchyRequestError" DOMException. */
                if (containedChildren.Any(c => c is DocumentType))
                    throw new HierarchyRequestError();
                /* 13) If original start node is an inclusive ancestor of original end node, set new node to original start node and new offset to original start offset. */
                Node newNode = null;
                int newOffset = 0;
                if (DOMCommon.Is_Inclusive_Ancestor(startContainer, endContainer))
                {
                    newNode = startContainer;
                    newOffset = startOffset;
                }
                /* 14) Otherwise: */
                else
                {
                    Node referenceNode = startContainer;
                    /* 2) While reference node’s parent is not null and is not an inclusive ancestor of original end node, set reference node to its parent. */
                    while (referenceNode.parentNode != null && !DOMCommon.Is_Inclusive_Ancestor(referenceNode.parentNode, endContainer))
                    {
                        referenceNode = referenceNode.parentNode;
                    }
                    /* 3) Set new node to the parent of reference node, and new offset to one plus reference node’s index. */
                    newNode = referenceNode.parentNode;
                    newOffset = 1 + referenceNode.index;
                }
                /* 15) If first partially contained child is a Text, ProcessingInstruction, or Comment node: */
                if (DOMCommon.Is_CommonTextNode(firstPartiallyContainedChild))
                {
                    CharacterData clone = (CharacterData)startContainer.cloneNode();
                    clone.data = ((CharacterData)startContainer).substringData(startOffset, startContainer.nodeLength - startOffset);
                    fragment.appendChild(clone);
                    /* 4) Replace data with node original start node, offset original start offset, count original start node’s length minus original start offset, and data the empty string. */
                    ((CharacterData)startContainer).replaceData(startOffset, startContainer.nodeLength - startOffset, string.Empty);
                }
                /* 16) Otherwise, if first partially contained child is not null: */
                else if (firstPartiallyContainedChild != null)
                {
                    Node clone = firstPartiallyContainedChild.cloneNode();
                    fragment.appendChild(clone);
                    /* 3) Let subrange be a new live range whose start is (original start node, original start offset) and whose end is (first partially contained child, first partially contained child’s length). */
                    var subrange = new Range(startContainer, startOffset, firstPartiallyContainedChild, firstPartiallyContainedChild.nodeLength);
                    var subfragment = subrange.extractContents();
                    clone.appendChild(subfragment);
                }
                /* 17) For each contained child in contained children, append contained child to fragment. */
                foreach (Node containedChild in containedChildren)
                {
                    fragment.appendChild(containedChild);
                }
                /* 18) If last partially contained child is a Text, ProcessingInstruction, or Comment node: */
                if (DOMCommon.Is_CommonTextNode(lastPartiallyContainedChild))
                {
                    CharacterData clone = (CharacterData)endContainer.cloneNode();
                    clone.data = ((CharacterData)endContainer).substringData(0, endOffset);
                    fragment.appendChild(clone);
                    ((CharacterData)endContainer).replaceData(0, endOffset, string.Empty);
                }
                /* 19) Otherwise, if last partially contained child is not null: */
                else if (lastPartiallyContainedChild != null)
                {
                    Node clone = lastPartiallyContainedChild.cloneNode();
                    fragment.appendChild(clone);
                    /* 3) Let subrange be a new live range whose start is (last partially contained child, 0) and whose end is (original end node, original end offset). */
                    var subrange = new Range(lastPartiallyContainedChild, 0, endContainer, endOffset);
                    var subfragment = subrange.extractContents();
                    clone.appendChild(subfragment);
                }
                /* 20) Set range’s start and end to (new node, new offset). */
                startContainer = endContainer = newNode;
                startOffset = endOffset = newOffset;

                return fragment;
            });
        }

        [CEReactions]
        public DocumentFragment cloneContents()
        {/* Docs: https://dom.spec.whatwg.org/#concept-range-clone */

            return CEReactions.Wrap_CEReaction(root.nodeDocument.defaultView, () =>
            {
                var fragment = new DocumentFragment(null, startContainer.ownerDocument);
                if (collapsed) return fragment;

                /* 4) If original start node is original end node, and they are a Text, ProcessingInstruction, or Comment node: */
                if (ReferenceEquals(startContainer, endContainer) && DOMCommon.Is_CommonTextNode(startContainer))
                {
                    CharacterData clone = (CharacterData)startContainer.cloneNode();
                    clone.data = ((CharacterData)startContainer).substringData(startOffset, endOffset - startOffset);
                    fragment.appendChild(clone);
                    return fragment;
                }
                var commonAncestor = startContainer;
                /* 6) While common ancestor is not an inclusive ancestor of original end node, set common ancestor to its own parent. */
                while (DOMCommon.Is_Inclusive_Ancestor(commonAncestor, endContainer))
                {
                    commonAncestor = commonAncestor.parentNode;
                }
                Node firstPartiallyContainedChild = null;
                var partialFilter = new FilterRangePartiallyContains(this);
                /* 8) If original start node is not an inclusive ancestor of original end node, set first partially contained child to the first child of common ancestor that is partially contained in range. */
                if (!DOMCommon.Is_Inclusive_Ancestor(startContainer, endContainer))
                {
                    var tree = new TreeWalker(commonAncestor, ENodeFilterMask.SHOW_ALL, partialFilter);
                    firstPartiallyContainedChild = tree.firstChild();
                }
                /* 9) Let last partially contained child be null. */
                Node lastPartiallyContainedChild = null;
                /* 10) If original end node is not an inclusive ancestor of original start node, set last partially contained child to the last child of common ancestor that is partially contained in range. */
                if (!DOMCommon.Is_Inclusive_Ancestor(endContainer, startContainer))
                {
                    var tree = new TreeWalker(commonAncestor, ENodeFilterMask.SHOW_ALL, partialFilter);
                    lastPartiallyContainedChild = tree.lastChild();
                }
                /* 11) Let contained children be a list of all children of common ancestor that are contained in range, in tree order. */
                var containedChildren = DOMCommon.Get_Descendents(commonAncestor, new FilterRangeContains(this));
                /* 12) If any member of contained children is a doctype, then throw a "HierarchyRequestError" DOMException. */
                if (containedChildren.Any(c => c is DocumentType))
                    throw new HierarchyRequestError();
                /* 13) If first partially contained child is a Text, ProcessingInstruction, or Comment node: */
                if (DOMCommon.Is_CommonTextNode(firstPartiallyContainedChild))
                {
                    CharacterData clone = (CharacterData)startContainer.cloneNode();
                    /* 2) Set the data of clone to the result of substringing data with node original start node, offset original start offset, and count original start node’s length minus original start offset. */
                    clone.data = ((CharacterData)startContainer).substringData(startOffset, endOffset - startOffset);
                    fragment.appendChild(clone);
                }
                /* 14) Otherwise, if first partially contained child is not null: */
                if (firstPartiallyContainedChild != null)
                {
                    Node clone = firstPartiallyContainedChild.cloneNode();
                    fragment.appendChild(clone);
                    /* 3) Let subrange be a new live range whose start is (original start node, original start offset) and whose end is (first partially contained child, first partially contained child’s length). */
                    Range subrange = new Range(startContainer, startOffset, firstPartiallyContainedChild, firstPartiallyContainedChild.nodeLength);
                    var subfragment = subrange.cloneContents();
                    clone.appendChild(subfragment);
                }
                /* 15) For each contained child in contained children: */
                foreach (Node containedChild in containedChildren)
                {
                    /* 1) Let clone be a clone of contained child with the clone children flag set. */
                    var clone = containedChild.cloneNode(true);
                    fragment.appendChild(clone);
                }
                /* 16) If last partially contained child is a Text, ProcessingInstruction, or Comment node: */
                if (DOMCommon.Is_CommonTextNode(lastPartiallyContainedChild))
                {
                    CharacterData clone = (CharacterData)endContainer.cloneNode();
                    /* 2) Set the data of clone to the result of substringing data with node original end node, offset 0, and count original end offset. */
                    clone.data = ((CharacterData)endContainer).substringData(0, endOffset);
                    fragment.appendChild(clone);
                }
                /* 17) Otherwise, if last partially contained child is not null: */
                if (lastPartiallyContainedChild != null)
                {
                    var clone = lastPartiallyContainedChild.cloneNode();
                    fragment.appendChild(clone);
                    /* 3) Let subrange be a new live range whose start is (last partially contained child, 0) and whose end is (original end node, original end offset). */
                    Range subrange = new Range(lastPartiallyContainedChild, 0, endContainer, endOffset);
                    /* 4) Let subfragment be the result of cloning the contents of subrange. */
                    var subfragment = subrange.cloneContents();
                    clone.appendChild(subfragment);
                }
                /* 18) Return fragment. */
                return fragment;
            });
        }

        [CEReactions]
        public void insertNode(Node node)
        {/* Docs: https://dom.spec.whatwg.org/#concept-range-insert */

            CEReactions.Wrap_CEReaction(root.nodeDocument.defaultView, () =>
            {
                /* 1) If range’s start node is a ProcessingInstruction or Comment node, is a Text node whose parent is null, or is node, then throw a "HierarchyRequestError" DOMException. */
                if (startContainer is ProcessingInstruction || startContainer is Comment || (startContainer is Text startTxt && startTxt.parentNode == null))
                    throw new HierarchyRequestError();

                Node referenceNode = null;
                /* 3) If range’s start node is a Text node, set referenceNode to that Text node. */
                if (startContainer is Text)
                {
                    referenceNode = startContainer;
                }
                /* 4) Otherwise, set referenceNode to the child of start node whose index is start offset, and null if there is no such child. */
                else
                {
                    referenceNode = (startContainer.childNodes.Count < startOffset) ? null : startContainer.childNodes[startOffset];
                }

                /* 5) Let parent be range’s start node if referenceNode is null, and referenceNode’s parent otherwise. */
                Node parent = referenceNode == null ? startContainer : referenceNode.parentNode;

                /* 6) Ensure pre-insertion validity of node into parent before referenceNode. */
                Node.Dom_ensure_pre_insertion_validity(node, parent, referenceNode);

                /* 7) If range’s start node is a Text node, set referenceNode to the result of splitting it with offset range’s start offset. */
                if (startContainer is Text)
                {
                    referenceNode = ((Text)startContainer).splitText(startOffset);
                }

                /* 8) If node is referenceNode, set referenceNode to its next sibling. */
                if (ReferenceEquals(node, referenceNode))
                    referenceNode = referenceNode.nextSibling;

                /* 9) If node’s parent is not null, remove node from its parent. */
                if (node.parentNode != null)
                    node.parentNode.removeChild(node);

                /* 10) Let newOffset be parent’s length if referenceNode is null, and referenceNode’s index otherwise. */
                int newOffset = referenceNode == null ? parent.nodeLength : referenceNode.index;

                /* 11) Increase newOffset by node’s length if node is a DocumentFragment node, and one otherwise. */
                newOffset += (node is DocumentFragment) ? node.nodeLength : 1;

                /* 12) Pre-insert node into parent before referenceNode. */
                Node.Dom_pre_insert_node(node, parent, referenceNode);

                /* 13) If range is collapsed, then set range’s end to (parent, newOffset). */
                if (collapsed)
                {
                    endContainer = parent;
                    endOffset = newOffset;
                }
            });
        }

        [CEReactions]
        public void surroundContents(Node newParent)
        {/* Docs: https://dom.spec.whatwg.org/#dom-range-surroundcontents */

            /* XXX: FINISH THIS */
            CEReactions.Wrap_CEReaction(root.nodeDocument.defaultView, () =>
            {
                /* 1) If a non-Text node is partially contained in the context object, then throw an "InvalidStateError" DOMException. */
                bool bPartial = false;
                if (startContainer is Text)
                {
                    if (PartiallyContains(startContainer.parentNode))
                    {
                        bPartial = true;
                    }
                }
                else if (PartiallyContains(startContainer))
                {
                    bPartial = true;
                }

                if (endContainer is Text)
                {
                    if (PartiallyContains(endContainer.parentNode))
                    {
                        bPartial = true;
                    }
                }
                else if (PartiallyContains(endContainer))
                {
                    bPartial = true;
                }

                if (bPartial)
                {
                    throw new InvalidStateError($"Cannot surround selection which partially contains an element");
                }

                /* 2) If newParent is a Document, DocumentType, or DocumentFragment node, then throw an "InvalidNodeTypeError" DOMException. */
                if (newParent is Document || newParent is DocumentType || newParent is DocumentFragment)
                {
                    throw new InvalidNodeTypeError($"{nameof(newParent)} cannot be a Document, DocumentType, or DocumentFragment node");
                }

                /* 3) Let fragment be the result of extracting the context object. */
                var fragment = extractContents();
                /* 4) If newParent has children, then replace all with null within newParent. */
                if (newParent.hasChildNodes()) Node.Dom_replace_all_within_node(null, newParent);
                /* 5) Insert newParent into the context object. */
                insertNode(newParent);
                /* 6) Append fragment to newParent. */
                newParent.appendChild(fragment);
                /* 7) Select newParent within the context object. */
                selectNode(newParent);
            });
        }

        public Range cloneRange()
        {
            return new Range(startContainer, startOffset, endContainer, endOffset);
        }

        public bool isPointInRange(Node node, int offset)
        {/* Docs: https://dom.spec.whatwg.org/#dom-range-ispointinrange */
            if (!ReferenceEquals(node.getRootNode(), root))
                return false;
            if (node is DocumentType)
                throw new InvalidNodeTypeError($"node cannot be a {nameof(DocumentType)}");
            if (offset > node.nodeLength)
                throw new IndexSizeError();

            BoundaryPoint newPoint = new BoundaryPoint() { node = node, offset = offset };
            /* 4) If (node, offset) is before start or after end, return false. */
            if (Get_Boundary_Position(newPoint, start) == EBoundaryPosition.Before || Get_Boundary_Position(newPoint, end) == EBoundaryPosition.After)
                return false;

            return true;
        }

        public short comparePoint(Node node, int offset)
        {/* Docs: https://dom.spec.whatwg.org/#dom-range-comparepoint */
            if (!ReferenceEquals(node.getRootNode(), root))
                throw new WrongDocumentError();
            if (node is DocumentType)
                throw new InvalidNodeTypeError($"node cannot be a {nameof(DocumentType)}");
            if (offset > node.nodeLength)
                throw new IndexSizeError();

            BoundaryPoint newPoint = new BoundaryPoint() { node = node, offset = offset };
            if (Get_Boundary_Position(newPoint, start) == EBoundaryPosition.Before)
                return -1;
            if (Get_Boundary_Position(newPoint, end) == EBoundaryPosition.After)
                return 1;

            return 0;
        }

        public bool intersectsNode(Node node)
        {/* Docs: https://dom.spec.whatwg.org/#dom-range-intersectsnode */
            if (!ReferenceEquals(node.getRootNode(), root))
                return false;

            var parent = node.parentNode;
            if (parent == null)
                return true;

            int offset = node.index;
            var point1 = new BoundaryPoint() { node = parent, offset = offset };
            var point2 = new BoundaryPoint() { node = parent, offset = offset+1 };
            /* 5) If (parent, offset) is before end and (parent, offset plus 1) is after start, return true. */
            if (Get_Boundary_Position(point1, end) == EBoundaryPosition.Before && Get_Boundary_Position(point2, start) == EBoundaryPosition.After)
                return true;

            return false;
        }

        public IEnumerable<DOMRect> getClientRects()
        {/* Docs: https://www.w3.org/TR/cssom-view-1/#dom-range-getclientrects */
            LinkedList<DOMRect> rectList = new LinkedList<DOMRect>();
            var common = commonAncestorContainer;
            var tree = new TreeWalker(common, ENodeFilterMask.SHOW_ALL);
            Node node = tree.nextNode();
            while (node != null)
            {
                /* For each element selected by the range, whose parent is not selected by the range, include the border areas returned by invoking getClientRects() on the element. */
                if (node.nodeType == ENodeType.ELEMENT_NODE)
                {
                    if (Contains(node))
                    {
                        if (!Contains(node.parentNode))
                        {
                            var rects = (node as Element).getClientRects();
                            foreach (var rect in rects)
                            {
                                rectList.AddLast(rect);
                            }
                        }
                    }
                    continue;
                }

                /* 
                 * For each Text node selected or partially selected by the range (including when the boundary-points are identical), 
                 * include a DOMRect object (for the part that is selected, not the whole line box). 
                 * The bounds of these DOMRect objects are computed using font metrics; thus, for horizontal writing, the vertical dimension of each box is determined by the font ascent and descent, 
                 * and the horizontal dimension by the text advance width. The transforms that apply to the ancestors are applied. 
                 */
                if (node.nodeType == ENodeType.TEXT_NODE)
                {
                    /* XXX: Transforms logic needed here */
                    if (Contains(node))
                    {
                        var r = FontMetrics.getTextRect(node, 0, node.nodeLength);
                        rectList.AddLast(r);
                    }
                    else if (PartiallyContains(node))
                    {
                        if (comparePoint(node, 0) <= 0)// Node is slightly BEFORE the range
                        {
                            var r = FontMetrics.getTextRect(node, startOffset, startContainer.nodeLength - startOffset);
                            rectList.AddLast(r);
                        }
                        else// Node must be slightly after the range
                        {
                            var r = FontMetrics.getTextRect(node, 0, endOffset);
                            rectList.AddLast(r);
                        }
                    }
                }

                node = tree.nextNode();
            }

            return rectList;
        }

        public DOMRect getBoundingClientRect()
        {/* Docs: https://www.w3.org/TR/cssom-view-1/#dom-range-getboundingclientrect */
            return DOMCommon.getBoundingClientRect(getClientRects());
        }

#region ToString
        public override string ToString()
        {/* Docs: https://dom.spec.whatwg.org/#dom-range-stringifier */
            /* 2) If the context object’s start node is the context object’s end node and it is a Text node, then return the substring of that Text node’s data beginning at the context object’s start offset and ending at the context object’s end offset. */
            if (ReferenceEquals(startContainer, endContainer) && startContainer is Text txt)
                return txt.substringData(startOffset, endOffset - startOffset);

            StringBuilder sb = new StringBuilder();
            /* 3) If the context object’s start node is a Text node, then append the substring of that node’s data from the context object’s start offset until the end to s. */
            if (startContainer is Text startText)
                sb.Append(startText.substringData(startOffset, startText.Length - startOffset));

            /* 4) Append the concatenation of the data of all Text nodes that are contained in the context object, in tree order, to s. */
            var allNodes = DOMCommon.Get_Range_Nodes(this);
            foreach(Node node in allNodes)
            {
                if (node is Text textNode)
                {
                    sb.Append(textNode.data);
                }
            }
            /* 5) If the context object’s end node is a Text node, then append the substring of that node’s data from its start until the context object’s end offset to s. */
            if (endContainer is Text endText)
            {
                sb.Append(endText.substringData(0, endOffset));
            }

            return sb.ToString();
        }
#endregion
    }
}
