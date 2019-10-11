using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using CssUI.CSS.BoxTree;
using CssUI.CSS.Enums;
using CssUI.DOM;
using CssUI.DOM.Nodes;
using CssUI.NodeTree;

namespace CssUI.CSS
{
    public static class CssBoxTree
    {
        /*
         * Docs: https://www.w3.org/TR/css-display-3/#intro
         * Docs: https://www.w3.org/TR/CSS22/visuren.html#box-gen
         */

        /// <summary>
        /// Generates an appropriate CSS principal box object for the given element, populated with any appropriate child boxes
        /// </summary>
        /// <param name="E"></param>
        /// <returns></returns>
        public static void Generate_Tree(Node StartNode, Node EndNode = null)
        {
            if (StartNode is null)
            {
                throw new ArgumentNullException(nameof(StartNode));
            }

            if (!StartNode.GetFlag(ENodeFlags.NeedsBoxUpdate | ENodeFlags.ChildNeedsBoxUpdate))
            {
                throw new ArgumentException($"Neither {nameof(StartNode)} nor its children are flagged for box updates.");
            }

            Contract.EndContractBlock();
            /* This is the method we use to populate the tree from a given point onwards:
             * 1) While queue is not empty, Pop next node.
             * 2) If next node == End then break;
             * 3) Delete all box-nodes in chain to the nodes real principal-box parent.
             * 4) Generate a new principal-box or text-run.
             * 5) Insert the new box-node into tree.
             * 6) Queue all children of element.
             */

            var Queue = new Queue<Node>();
            Queue.Enqueue(StartNode);
            while (Queue.Count > 0)
            {
                Node node = Queue.Dequeue();
                if (ReferenceEquals(node, EndNode))
                    break;

                if (node.GetFlag(ENodeFlags.NeedsBoxUpdate))
                {
                    CssBoxTreeNode Box = node.Box;
                    Element nearestAncestor = Get_Closest_Box_Generating_Ancestor(node);

                    // 3) Delete the nodes current box
                    // 3) Unlink all box-nodes in the chain leading to the nodes real principal-box parent
                    // The current box might be wrapped in an anonymous box. in which case the index we WANT is actually THAT boxs'
                    int index = -1;
                    if (Box is object)
                    {
                        var ChainRoot = Box.Unlink(nearestAncestor.Box);
                        index = ChainRoot.index;
                    }

                    CssBoxTreeNode nextBox = null;
                    // 4) Generate a new principal-box or text-run
                    switch (node.nodeType)
                    {
                        case DOM.Enums.ENodeType.TEXT_NODE:
                            {
                                // Texts runs encompass all of the contiguous sibling text-nodes, skip those contiguous nodes in the queue
                                var TextNodes = new List<Text>(node.parentNode.childNodes.Count);
                                while (Queue.Peek().nodeType == DOM.Enums.ENodeType.TEXT_NODE && ReferenceEquals(Queue.Peek().parentNode, node.parentNode))
                                {
                                    TextNodes.Add((Text)Queue.Dequeue());
                                }

                                nextBox = new CssTextRun(TextNodes.ToArray());
                            }
                            break;
                        case DOM.Enums.ENodeType.ELEMENT_NODE:
                            {
                                nextBox = Generate_Box((Element)node);
                            }
                            break;
                    }

                    // Transfer child nodes from old box to the new box (they will remove themselves if needed)
                    ITreeNode current = Box.firstChild;
                    while (current is object)
                    {
                        current.parentNode = null;
                        nextBox.childNodes.Add(current);
                        current = current.nextSibling;
                    }

                    // 5) Insert the new box-node into tree
                    if (index > -1)
                        nearestAncestor.Box.Insert(index, nextBox);
                    else
                        nearestAncestor.Box.Add(nextBox);

                    // Notify the tree that we need to be reflowed
                    node.Propagate_Flag(ENodeFlags.ChildNeedsReflow, exclude_self: true);
                }

                node.ClearFlag(ENodeFlags.NeedsBoxUpdate | ENodeFlags.ChildNeedsBoxUpdate);
                // 6) Queue all children of node.
                foreach (var n in node.childNodes)
                {
                    // Only add items which we KNOW will need an update
                    if (n.GetFlag(ENodeFlags.NeedsBoxUpdate | ENodeFlags.ChildNeedsBoxUpdate))
                        Queue.Enqueue(n);
                }
            }
        }

        /// <summary>
        /// Generates an appropriate CSS text-run object for the given node
        /// </summary>
        /// <param name="Node"></param>
        /// <returns></returns>
        private static CssTextRun Generate_TextRun(in Text Node)
        {
            return new CssTextRun(Text.get_contiguous_text_nodes(Node, false).ToArray());
        }

        /// <summary>
        /// Generates an appropriate CSS principal-box object for the given element
        /// </summary>
        /// <param name="Node"></param>
        /// <returns></returns>
        private static CssBoxTreeNode Generate_Box(in Element Node)
        {
            if (Node is null || Node?.Box?.DisplayType.Outer == EOuterDisplayType.None)
            {
                return null;
            }

            if (Node.isRoot || Node.parentElement is null)
            {// Root-nodes always generate block-level boxes
                Node.Style.ImplicitRules.Display.Set(EDisplayMode.BLOCK);
                CssPrincipalBox box = new CssPrincipalBox(Node.parentElement, null);
                return box;
            }
            else
            {
                CssBox parentBox = Node.parentElement.Box;
                CssBox box = new CssPrincipalBox(Node, parentBox);

                if (!Is_Compatable_Parent_Box(Node, Node.parentElement))
                {
                    // If a block container box has a block-level box inside it, then force it to ONLY have block-level boxes inside it
                    if (HasBlockLevelChildren(Node.parentElement))
                    {
                        CssAnonymousBox wrapper = CssAnonymousBox.Create_Block(parentBox);
                        wrapper.Add(box);
                        box = wrapper;
                    }
                    else if (HasInlineLevelChildren(Node.parentElement))
                    {
                        CssAnonymousBox wrapper = CssAnonymousBox.Create_Inline(parentBox);
                        wrapper.Add(box);
                        box = wrapper;
                    }
                }

                return box;
            }
        }

        /// <summary>
        /// Populates a given principal box with all of the appropriate boxes of it's owning element's children
        /// </summary>
        public static LinkedList<CssBoxTreeNode> Populate(ref CssPrincipalBox Box)
        {
            var RetList = new LinkedList<CssBoxTreeNode>();

            return RetList;
        }



        public static bool HasBlockLevelChildren(in Element Node)
        {
            Element current = Node.firstElementChild;
            while (current != null)
            {
                if (DisplayType.Get_Outer(current.Style.Display) == EOuterDisplayType.Block)
                {
                    return true;
                }

                current = current.nextElementSibling;
            }

            return false;
        }

        public static bool HasInlineLevelChildren(in Element Node)
        {
            Element current = Node.firstElementChild;
            while (current != null)
            {
                if (DisplayType.From(current.Style.Display).IsInlineLevel)
                {
                    return true;
                }

                current = current.nextElementSibling;
            }

            return false;
        }

        /// <summary>
        /// In constructing the box tree, boxes generated by an element are descendants of the principal box of any ancestor elements.
        /// In the general case, the direct parent box of an element’s principal box is the principal box of its nearest ancestor element that generates a box; 
        /// however, there are some exceptions, such as for run-in boxes, display types(like tables) that generate multiple container boxes, and intervening anonymous boxes.
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        private static Element Get_Closest_Box_Generating_Ancestor(in Node node)
        {
            var current = node.parentNode;
            while (current is object)
            {// Find the nearest ancestor element which has a box
                if (current is Element element && element.Box is object)
                {
                    return element;
                }

                current = current.parentElement;
            }

            return null;
        }



        /// <summary>
        /// Returns <c>True</c> if the parent box is a valid container for the given child
        /// </summary>
        /// <param name="child"></param>
        /// <param name="parent"></param>
        /// <returns></returns>
        private static bool Is_Compatable_Parent_Box(in Element child, in Element parent)
        {
            if (parent is null)
                return false;

            DisplayType parentDisplay = DisplayType.From(parent.Style.Display);
            DisplayType childDisplay = DisplayType.From(child.Style.Display);

            switch (parentDisplay.Outer)
            {
                case EOuterDisplayType.Block:
                    {
                        if (childDisplay.IsBlockLevel)
                        {
                            return !HasInlineLevelChildren(parent);
                        }
                        else if (childDisplay.IsInlineLevel)
                        {
                            return !HasBlockLevelChildren(parent);
                        }
                    }
                    break;
                case EOuterDisplayType.Inline:
                    {
                        if (childDisplay.IsInlineLevel)
                        {
                            return true;
                        }
                    }
                    break;
                case EOuterDisplayType.Run_In:
                    {
                    }
                    break;
                case EOuterDisplayType.None:
                default:
                    {
                        return true;
                    }
            }

            return false;
        }

    }
}
