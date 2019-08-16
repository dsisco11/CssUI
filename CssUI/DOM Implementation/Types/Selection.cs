using CssUI.DOM.CustomElements;
using CssUI.DOM.Exceptions;
using CssUI.DOM.Nodes;
using System;

namespace CssUI.DOM
{
    public class Selection
    {/* Docs: https://www.w3.org/TR/selection-api/#selection-interface */
        #region Properties
        Range _range = null;
        Document document = null;
        ESelectionDirection direction = ESelectionDirection.None;
        #endregion

        #region Constructors
        public Selection(Document document)
        {
            this.document = document;
        }
        #endregion

        #region Accessors

        BoundaryPoint? Anchor => (direction == ESelectionDirection.Forward) ? _range?.start : _range?.end;
        BoundaryPoint? Focus => (direction == ESelectionDirection.Forward) ? _range?.end : _range?.start;
        public Node AnchorNode => Anchor?.node;
        public int AnchorOffset => Anchor?.offset ?? 0;
        public Node FocusNode => Focus?.node;
        public int FocusOffset => Focus?.offset ?? 0;
        public bool IsCollapsed => (Anchor == Focus);
        public int RangeCount => (_range == null) ? 0 : 1;
        public ESelectionType Type
        {
            get
            {
                if (_range == null) return ESelectionType.None;
                return _range.collapsed ? ESelectionType.Caret : ESelectionType.Range;
            }
        }
        #endregion


        public Range GetRangeAt(int index)
        {
            if (index != 0) throw new IndexSizeError();
            return _range;
        }

        public void AddRange(Range range)
        {
            if (!ReferenceEquals(document, _range.startContainer.getRootNode()) || !ReferenceEquals(document, _range.endContainer.getRootNode())) return;
            if (RangeCount != 0) return;
            _range = range;
        }

        public void RemoveRange(Range range)
        {
            if (!ReferenceEquals(_range, range)) throw new NotFoundError($"This selection does not contain the given {nameof(range)}");
            _range = null;
        }

        public void RemoveAllRanges()
        {
            _range = null;
        }

        public void Empty()
        {
            _range = null;
        }

        public void Collapse(Node node, int offset = 0)
        {
            if (node == null)
            {
                _range = null;
                return;
            }
            if (offset > node.nodeLength) throw new IndexSizeError($"offset cannot be greater than node length");
            if (!ReferenceEquals(document, node.getRootNode())) return;

            _range = new Range(node, offset, node, offset);
        }

        /// <summary>
        /// Alias for <see cref="Collapse(Node, int)"/>
        /// </summary>
        /// <param name="node"></param>
        /// <param name="offset"></param>
        public void SetPosition(Node node, int offset = 0) => Collapse(node, offset);

        public void CollapseToStart()
        {
            if (_range == null) throw new InvalidStateError($"Selection has no set range");
            var newRange = new Range(_range.startContainer, _range.startOffset, _range.startContainer, _range.startOffset);
            _range = newRange;
        }

        public void CollapseToEnd()
        {
            if (_range == null) throw new InvalidStateError($"Selection has no set range");
            var newRange = new Range(_range.endContainer, _range.endOffset, _range.endContainer, _range.endOffset);
            _range = newRange;
        }

        public void Extend(Node node, int offset = 0)
        {
            if (_range == null) throw new InvalidStateError($"Selection has no set range");
            if (!ReferenceEquals(document, node.getRootNode())) return;

            var oldAnchor = Anchor.Value;
            var oldFocus = Focus.Value;
            var newFocus = new BoundaryPoint(node, offset);
            var newRange = new Range(document);

            if (!ReferenceEquals(_range.root, node.getRootNode()))
            {
                newRange.setStart(node, offset);
                newRange.setEnd(node, offset);
            }
            else if (Range.Get_Boundary_Position(oldAnchor, newFocus) != EBoundaryPosition.After)
            {
                newRange.setStart(oldAnchor.node, oldAnchor.offset);
                newRange.setEnd(newFocus.node, newFocus.offset);
            }
            else
            {
                newRange.setStart(newFocus.node, newFocus.offset);
                newRange.setEnd(oldAnchor.node, oldAnchor.offset);
            }

            _range = newRange;
            direction = Range.Get_Boundary_Position(newFocus, oldAnchor) == EBoundaryPosition.Before ? ESelectionDirection.Backward : ESelectionDirection.Forward;
        }

        public void SetBaseAndExtent(Node anchorNode, int anchorOffset, Node focusNode, int focusOffset)
        {
            if (anchorOffset > anchorNode.nodeLength) throw new IndexSizeError($"{nameof(anchorOffset)} exceeds {nameof(anchorNode.nodeLength)}");
            if (focusOffset > focusNode.nodeLength) throw new IndexSizeError($"{nameof(focusOffset)} exceeds {nameof(focusNode.nodeLength)}");
            if (!ReferenceEquals(document, anchorNode.getRootNode()) || !ReferenceEquals(document, focusNode.getRootNode())) return;

            var anchor = new BoundaryPoint(anchorNode, anchorOffset);
            var focus = new BoundaryPoint(focusNode, focusOffset);
            var newRange = new Range(document);
            if (Range.Get_Boundary_Position(anchor, focus) == EBoundaryPosition.Before)
            {
                newRange.setStart(anchor.node, anchor.offset);
                newRange.setEnd(focus.node, focus.offset);
            }
            else
            {
                newRange.setStart(focus.node, focus.offset);
                newRange.setEnd(anchor.node, anchor.offset);
            }

            _range = newRange;
            direction = (Range.Get_Boundary_Position(focus, anchor) == EBoundaryPosition.Before) ? ESelectionDirection.Backward : ESelectionDirection.Forward;
        }

        public void SelectAllChildren(Node node)
        {
            if (!ReferenceEquals(document, node.getRootNode())) return;

            var newRange = new Range(document);
            var nodeLength = node.nodeLength;
            newRange.setStart(node, 0);
            newRange.setEnd(node, nodeLength);
            _range = newRange;
            direction = ESelectionDirection.Forward;
        }


        [CEReactions]
        public void DeleteFromDocument()
        {
            CEReactions.Wrap_CEReaction(document.defaultView, () => _range?.deleteContents());
        }

        public bool ContainsNode(Node node, bool allowPartialContainment = false)
        {
            if (node == null) throw new ArgumentNullException(nameof(node));
            if (_range == null || !ReferenceEquals(document, node.getRootNode())) return false;

            var first = new BoundaryPoint(node, 0);
            var last = new BoundaryPoint(node, node.nodeLength);

            var firstPos = Range.Get_Boundary_Position(_range.start, first);
            var lastPos = Range.Get_Boundary_Position(_range.end, last);

            if (!allowPartialContainment)
            {
                return (firstPos == EBoundaryPosition.Before || firstPos == EBoundaryPosition.Equal) && (lastPos == EBoundaryPosition.After || lastPos == EBoundaryPosition.Equal);
            }
            else
            {
                return (firstPos == EBoundaryPosition.Before || firstPos == EBoundaryPosition.Equal) || (lastPos == EBoundaryPosition.After || lastPos == EBoundaryPosition.Equal);
            }
        }

        public override string ToString()
        {
            return _range?.ToString();
        }
    }
}
