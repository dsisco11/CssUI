using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using CssUI.CSS.BoxTree;
using CssUI.CSS.Enums;
using CssUI.DOM;
using CssUI.DOM.Nodes;
using CssUI.NodeTree;

namespace CssUI.CSS;

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
    public static void Generate_Tree(Node StartNode, Node? EndNode = null)
    {
        ArgumentNullException.ThrowIfNull(StartNode);

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
         * 7) After processing children, normalize block containers to ensure only block-level children.
         */

        var Queue = new Queue<Node>();
        // Track elements that need anonymous box normalization after their children are processed
        var BlockContainersToNormalize = new HashSet<Element>();
        Queue.Enqueue(StartNode);
        while (Queue.Count > 0)
        {
            Node node = Queue.Dequeue();
            if (ReferenceEquals(node, EndNode))
                break;

            if (node.GetFlag(ENodeFlags.NeedsBoxUpdate))
            {
                CssBoxTreeNode? Box = node.Box;
                Element? nearestAncestor = Get_Closest_Box_Generating_Ancestor(node);

                // 3) Delete the nodes current box
                // 3) Unlink all box-nodes in the chain leading to the nodes real principal-box parent
                // The current box might be wrapped in an anonymous box. in which case the index we WANT is actually THAT boxs'
                int index = -1;
                if (Box is not null)
                {
                    // Only unlink if we have a parent box to unlink from
                    // Root elements have no ancestor, so they don't need unlinking
                    if (nearestAncestor?.Box is not null)
                    {
                        var ChainRoot = Box.Unlink(nearestAncestor.Box);
                        index = ChainRoot.index;
                    }
                    else
                    {
                        // Root element - just detach from any existing parent
                        Box.parentNode = null;
                    }
                }

                CssBoxTreeNode? nextBox = null;
                // 4) Generate a new principal-box or text-run
                switch (node.nodeType)
                {
                    case DOM.Enums.ENodeType.TEXT_NODE:
                        {
                            // Text runs encompass all of the contiguous sibling text-nodes, skip those contiguous nodes in the queue
                            // Per CSS Display 3 §1: "If the sequence contains no text, however, it does not generate a text sequence."
                            var TextNodes = new List<Text>(node.parentNode?.childNodes.Count ?? 4);

                            // Add the current text node first
                            TextNodes.Add((Text)node);

                            // Collect contiguous sibling text nodes (must check Queue.Count before Peek to avoid InvalidOperationException)
                            while (Queue.Count > 0 &&
                                   Queue.Peek().nodeType == DOM.Enums.ENodeType.TEXT_NODE &&
                                   ReferenceEquals(Queue.Peek().parentNode, node.parentNode))
                            {
                                var contiguousTextNode = (Text)Queue.Dequeue();
                                TextNodes.Add(contiguousTextNode);
                                // Clear the update flag since we're processing this node now
                                contiguousTextNode.ClearFlag(ENodeFlags.NeedsBoxUpdate | ENodeFlags.ChildNeedsBoxUpdate);
                            }

                            // Only generate text run if there's actual text content
                            // Per spec: empty text nodes do not generate text sequences
                            bool hasContent = false;
                            foreach (var textNode in TextNodes)
                            {
                                if (!string.IsNullOrEmpty(textNode.data))
                                {
                                    hasContent = true;
                                    break;
                                }
                            }

                            if (hasContent)
                            {
                                var textRun = new CssTextRun(TextNodes.ToArray());
                                nextBox = textRun;
                                // Set the box reference on all text nodes in the run
                                foreach (var textNode in TextNodes)
                                {
                                    textNode.Box = textRun;
                                }
                            }
                            else
                            {
                                // Clear box reference on all text nodes if no content
                                foreach (var textNode in TextNodes)
                                {
                                    textNode.Box = null;
                                }
                            }
                        }
                        break;
                    case DOM.Enums.ENodeType.ELEMENT_NODE:
                        {
                            nextBox = Generate_Box((Element)node);
                        }
                        break;
                }

                // Transfer child nodes from old box to the new box (they will remove themselves if needed)
                if (Box is not null)
                {
                    ITreeNode? current = Box.firstChild;
                    while (current is object)
                    {
                        current.parentNode = null;
                        nextBox!.childNodes.Add(current);
                        current = current.nextSibling;
                    }
                }

                // Assign the new box to the node (or clear it if no box generated)
                node.Box = nextBox;

                // 5) Insert the new box-node into tree (skip for root elements which have no parent)
                // Also skip if the box already has a parent set (from constructor)
                if (nearestAncestor?.Box is not null && nextBox is not null && nextBox.parentNode is null)
                {
                    // Only use the saved index if it's valid for the current child count
                    // The index might be stale if the parent's children were modified (e.g., by normalization)
                    if (index > -1 && index < nearestAncestor.Box.childNodes.Count)
                        nearestAncestor.Box.Insert(index, nextBox);
                    else
                        nearestAncestor.Box.Add(nextBox);

                    // Mark parent block container for normalization after children are processed
                    if (nearestAncestor.Box.IsBlockContainer)
                    {
                        BlockContainersToNormalize.Add(nearestAncestor);
                    }
                }

                // Notify the tree that we need to be reflowed
                node.Propagate_Flag(ENodeFlags.ChildNeedsReflow, exclude_self: true);
            }

            node.ClearFlag(ENodeFlags.NeedsBoxUpdate | ENodeFlags.ChildNeedsBoxUpdate);

            // 6) Queue all children of node.
            // Per CSS Display 3 §2.5: display: none elements and their descendants generate no boxes.
            // Skip queueing children if this element has display: none.
            bool skipChildren = false;
            if (node is Element element)
            {
                var displayType = new DisplayType(element.Style.Display);
                if (displayType.Outer == EOuterDisplayType.None)
                {
                    skipChildren = true;
                    // Clear boxes from all descendants since they won't be processed
                    ClearDescendantBoxes(node);
                }
            }

            if (!skipChildren)
            {
                foreach (var n in node.childNodes)
                {
                    // Only add items which we KNOW will need an update
                    if (n.GetFlag(ENodeFlags.NeedsBoxUpdate | ENodeFlags.ChildNeedsBoxUpdate))
                        Queue.Enqueue(n);
                }
            }
        }

        // 7) Normalize all block containers to ensure they contain only block-level children
        // Per CSS 2.2 §9.2.1.1: A block container either contains only inline-level boxes
        // or only block-level boxes (wrapped in anonymous blocks if mixed)
        foreach (var container in BlockContainersToNormalize)
        {
            if (container.Box is CssBox box)
            {
                NormalizeBlockContainerChildren(box);
            }
        }
    }

    /// <summary>
    /// Normalizes a block container's children per CSS 2.2 §9.2.1.1.
    /// If the container has mixed block-level and inline-level children,
    /// all runs of consecutive inline content are wrapped in anonymous block boxes.
    /// </summary>
    /// <remarks>
    /// Docs: https://www.w3.org/TR/CSS22/visuren.html#anonymous-block-level
    ///
    /// "if a block container box has a block-level box inside it, then we force it
    /// to have only block-level boxes inside it."
    ///
    /// The algorithm:
    /// 1. Check if children are mixed (both block and inline level boxes)
    /// 2. If not mixed, no action needed
    /// 3. If mixed, iterate children and wrap consecutive inline runs in anonymous block boxes
    /// </remarks>
    private static void NormalizeBlockContainerChildren(CssBox container)
    {
        if (container.childNodes.Count == 0)
            return;

        // Take a snapshot of children to avoid modification during iteration
        var children = new List<ITreeNode>(container.childNodes);

        // First pass: determine if we have mixed content
        bool hasBlockLevel = false;
        bool hasInlineLevel = false;

        foreach (var child in children)
        {
            if (child is CssBox childBox)
            {
                if (childBox.IsBlockLevel)
                    hasBlockLevel = true;
                else if (childBox.IsInlineLevel)
                    hasInlineLevel = true;
            }
            else if (child is CssTextRun)
            {
                // Text runs are inline-level
                hasInlineLevel = true;
            }
        }

        // If not mixed, no normalization needed
        if (!(hasBlockLevel && hasInlineLevel))
            return;

        // Clear the container's children FIRST, before building new list
        // This ensures all children have parentNode = null before we re-parent them
        container.childNodes.Clear();

        // Second pass: wrap inline runs in anonymous block boxes
        var currentInlineRun = new List<ITreeNode>();

        void FlushInlineRun()
        {
            if (currentInlineRun.Count == 0)
                return;

            // Create anonymous block box to wrap the inline run
            // Pass null as parent - we'll add it to the container
            var anonymousBlock = CssAnonymousBox.Create_Block(null!);
            foreach (var inlineChild in currentInlineRun)
            {
                // parentNode is already null from Clear() above
                anonymousBlock.Add(inlineChild);
            }
            // Add the anonymous block to the container
            container.Add(anonymousBlock);
            currentInlineRun.Clear();
        }

        foreach (var child in children)
        {
            bool isBlockLevel = false;
            if (child is CssBox childBox)
            {
                isBlockLevel = childBox.IsBlockLevel;
            }

            if (isBlockLevel)
            {
                // Flush any pending inline content
                FlushInlineRun();
                // Add block-level child directly to container
                // parentNode is already null from Clear() above
                container.Add(child);
            }
            else
            {
                // Collect inline-level content
                currentInlineRun.Add(child);
            }
        }

        // Flush any remaining inline content
        FlushInlineRun();
    }

    /// <summary>
    /// Recursively clears boxes from all descendants of a node.
    /// Used when an ancestor has display: none, which means descendants generate no boxes.
    /// </summary>
    private static void ClearDescendantBoxes(Node node)
    {
        foreach (var child in node.childNodes)
        {
            child.Box = null;
            child.ClearFlag(ENodeFlags.NeedsBoxUpdate | ENodeFlags.ChildNeedsBoxUpdate);
            ClearDescendantBoxes(child);
        }
    }

    /// <summary>
    /// Generates an appropriate CSS principal-box object for the given element.
    /// Implements CSS Display 3 box generation rules.
    /// </summary>
    /// <param name="Node">The element to generate a box for.</param>
    /// <returns>The generated box, or null if the element generates no box.</returns>
    /// <remarks>
    /// Docs: https://www.w3.org/TR/css-display-3/#box-generation
    /// </remarks>
    private static CssBoxTreeNode? Generate_Box(in Element Node)
    {
        if (Node is null)
        {
            return null;
        }

        var displayType = new DisplayType(Node.Style.Display);

        // Check if display is none - these elements don't generate boxes
        // Per CSS Display 3 §2.5: "The element and its descendants generate no boxes or text sequences."
        if (displayType.Outer == EOuterDisplayType.None)
        {
            return null;
        }

        // Check for display: contents - element generates no box but children still do
        // Per CSS Display 3 §2.5: "The element itself does not generate any boxes,
        // but its children and pseudo-elements still generate boxes and text sequences as normal."
        if (displayType.Outer == EOuterDisplayType.Contents)
        {
            // Return null for this element's box, but children will still be processed
            // Note: For replaced elements, display: contents computes to display: none (handled at style level)
            return null;
        }

        // Apply automatic box type transformations (blockification/inlinification)
        // Per CSS Display 3 §2.7
        displayType = ApplyBoxTypeTransformations(Node, displayType);

        if (Node.isRoot || Node.parentElement is null)
        {
            // Root-nodes always generate block-level boxes
            // Per CSS Display 3 §2.8: "The root element's display type is always blockified"
            Node.Style!.ImplicitRules.Display.Set(EDisplayMode.BLOCK);
            return new CssPrincipalBox(Node, null!);
        }

        // Create box without parent - Generate_Tree() will add it to the parent's children
        // This avoids the parentNode != null check in Generate_Tree that would skip tree insertion
        CssPrincipalBox box = new CssPrincipalBox(Node, null!);

        // Note: Anonymous box normalization is handled in Generate_Tree() after all
        // children of block containers are processed (per CSS 2.2 §9.2.1.1)

        return box;
    }

    /// <summary>
    /// Applies automatic box type transformations (blockification/inlinification) per CSS Display 3 §2.7.
    /// </summary>
    /// <remarks>
    /// Blockification occurs for:
    /// - Floated elements (float != none)
    /// - Absolutely/fixed positioned elements
    /// - Children of flex/grid containers
    ///
    /// Inlinification occurs for:
    /// - Children of ruby containers (not implemented yet)
    /// </remarks>
    private static DisplayType ApplyBoxTypeTransformations(in Element Node, DisplayType displayType)
    {
        // Skip if already has no box or contents
        if (displayType.Outer == EOuterDisplayType.None || displayType.Outer == EOuterDisplayType.Contents)
        {
            return displayType;
        }

        bool shouldBlockify = false;

        // Check for blockification triggers
        // 1. Absolutely positioned elements (position: absolute or fixed)
        if (Node.Style.Positioning == EBoxPositioning.Absolute || Node.Style.Positioning == EBoxPositioning.Fixed)
        {
            shouldBlockify = true;
        }

        // 2. Floated elements (when float property is implemented)
        // @todo: Check float property when implemented

        // 3. Children of flex/grid containers
        if (Node.parentElement is not null)
        {
            var parentDisplay = Node.parentElement.Style.Display;
            if (parentDisplay == EDisplayMode.FLEX || parentDisplay == EDisplayMode.INLINE_FLEX ||
                parentDisplay == EDisplayMode.GRID || parentDisplay == EDisplayMode.INLINE_GRID)
            {
                shouldBlockify = true;
            }
        }

        // Apply blockification if needed
        if (shouldBlockify && displayType.IsInlineLevel)
        {
            // Per CSS Display 3 §2.7: blockification sets outer display to block
            // For inline flow-root (inline-block), it becomes block (losing flow-root) for legacy reasons
            if (displayType.Inner == EInnerDisplayType.Flow_Root)
            {
                // inline-block → block (per spec, loses flow-root for legacy reasons)
                Node.Style.ImplicitRules.Display.Set(EDisplayMode.BLOCK);
                return new DisplayType(EOuterDisplayType.Block, EInnerDisplayType.Flow_Root);
            }
            else
            {
                // Other inline types → block flow-root
                Node.Style.ImplicitRules.Display.Set(EDisplayMode.BLOCK);
                return new DisplayType(EOuterDisplayType.Block, displayType.Inner);
            }
        }

        return displayType;
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
        Element? current = Node.firstElementChild;
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
        Element? current = Node.firstElementChild;
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
    private static Element? Get_Closest_Box_Generating_Ancestor(in Node node)
    {
        var current = node.parentNode;
        while (current is not null)
        {
            // Find the nearest ancestor element which has a box
            if (current is Element element)
            {
                // Skip elements with display: contents as they don't generate boxes
                var displayType = new DisplayType(element.Style.Display);
                if (displayType.Outer != EOuterDisplayType.Contents && element.Box is not null)
                {
                    return element;
                }
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

