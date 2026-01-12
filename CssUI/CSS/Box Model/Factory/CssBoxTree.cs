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
     * Docs: https://www.w3.org/TR/CSS22/tables.html#anonymous-boxes (Table anonymous boxes)
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
         * 8) After processing children, fix up table-internal boxes per CSS 2.2 §17.2.1.
         */

        var Queue = new Queue<Node>();
        // Track elements that need anonymous box normalization after their children are processed
        var BlockContainersToNormalize = new HashSet<Element>();
        // Track boxes that may need table internal fixup
        var BoxesRequiringTableFixup = new HashSet<CssBox>();
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
                            // Per CSS Display 3 §1: "each contiguous sequence of sibling text nodes generates a text sequence
                            // containing their text contents... If the sequence contains no text, however, it does not generate a text sequence."
                            //
                            // Note: Whitespace-only text nodes DO generate text sequences because whitespace IS text content.
                            // The white-space property (CSS Text 3 §4) controls whitespace collapsing during layout,
                            // which is handled separately during inline formatting context layout (phase 18.3).
                            // @todo Phase 18.3: Implement white-space property to handle whitespace collapsing/trimming.
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

                            // Only generate text run if there's actual text content (including whitespace)
                            // Per spec: EMPTY text nodes (zero characters) do not generate text sequences
                            // Whitespace-only nodes DO generate text sequences (whitespace is content)
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
                // Special handling for table wrapper boxes: don't transfer the grid box
                if (Box is not null)
                {
                    ITreeNode? current = Box.firstChild;
                    while (current is object)
                    {
                        var next = current.nextSibling; // Save next before we modify the tree

                        // Skip the grid box for table wrappers - it's already set up
                        bool skipTransfer = false;
                        if (nextBox is CssTableWrapperBox nextWrapperBox && current is CssTableGridBox)
                        {
                            skipTransfer = true;
                        }

                        if (!skipTransfer)
                        {
                            current.parentNode = null;
                            // Get the actual container (grid box for tables, else the box itself)
                            var containerBox = Get_Container_Box(nextBox!);
                            containerBox.childNodes.Add(current);
                        }

                        current = next;
                    }
                }

                // Assign the new box to the node (or clear it if no box generated)
                node.Box = nextBox;

                // 5) Insert the new box-node into tree (skip for root elements which have no parent)
                // Also skip if the box already has a parent set (from constructor)
                if (nearestAncestor?.Box is not null && nextBox is not null && nextBox.parentNode is null)
                {
                    // Get the actual container box - for table wrappers, this is the grid box
                    var containerBox = Get_Container_Box(nearestAncestor.Box);

                    // Only use the saved index if it's valid for the current child count
                    // The index might be stale if the parent's children were modified (e.g., by normalization)
                    if (index > -1 && index < containerBox.childNodes.Count)
                        containerBox.Insert(index, nextBox);
                    else
                        containerBox.Add(nextBox);

                    // Mark parent block container for normalization after children are processed
                    if (containerBox is CssBox containerCssBox && containerCssBox.IsBlockContainer)
                    {
                        BlockContainersToNormalize.Add(nearestAncestor);
                    }

                    // Track boxes that may need table internal fixup
                    // Per CSS 2.2 §17.2.1: table-internal boxes generate anonymous wrappers if misparented
                    if (nextBox is CssBox childBox && node is Element nodeElement)
                    {
                        var childDisplay = nodeElement.Style.Display;
                        if (TableInternalDisplayType.IsTableInternal(childDisplay))
                        {
                            BoxesRequiringTableFixup.Add((CssBox)containerBox);
                        }
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

        // 8) Fix up table-internal boxes per CSS 2.2 §17.2.1
        // Misparented table-internal boxes generate anonymous wrapper boxes
        foreach (var parentBox in BoxesRequiringTableFixup)
        {
            FixupTableInternalBoxes(parentBox);
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

    #region Table Internal Box Fixup (CSS 2.2 §17.2.1)
    /*
     * Docs: https://www.w3.org/TR/CSS22/tables.html#anonymous-boxes
     * Docs: https://www.w3.org/TR/css-display-3/#layout-specific-display
     *
     * Per CSS 2.2 §17.2.1: Any table element will automatically generate necessary
     * anonymous table objects around itself, consisting of at least three nested
     * objects corresponding to a 'table'/'inline-table' element, a 'table-row'
     * element, and a 'table-cell' element.
     *
     * The fixup algorithm has three stages:
     * 1. Remove irrelevant boxes (whitespace between table elements)
     * 2. Generate missing child wrappers
     * 3. Generate missing parents (wrap misparented table-internal boxes)
     */

    /// <summary>
    /// Fixes up table-internal boxes that are misparented.
    /// Per CSS 2.2 §17.2.1: Generates anonymous wrapper boxes when table-internal
    /// boxes don't have the required parent type.
    /// </summary>
    /// <remarks>
    /// Example transformations:
    /// - table-cell in block → anonymous table-row → anonymous table-row-group → anonymous table → table-cell
    /// - table-row in block → anonymous table-row-group → anonymous table → table-row
    /// - table-row in table (not row-group) → anonymous table-row-group → table-row
    /// </remarks>
    private static void FixupTableInternalBoxes(CssBox parentBox)
    {
        if (parentBox.childNodes.Count == 0)
            return;

        // Get the parent's display mode
        EDisplayMode parentDisplay = GetBoxDisplayMode(parentBox);

        // Take a snapshot of children to avoid modification during iteration
        var children = new List<ITreeNode>(parentBox.childNodes);

        // Track indices of children that need fixup and their wrapper chains
        var replacements = new List<(int Index, ITreeNode OriginalChild, CssAnonymousBox OutermostWrapper, CssAnonymousBox InnermostWrapper)>();

        // Process each child that needs fixup
        for (int i = 0; i < children.Count; i++)
        {
            var child = children[i];

            if (child is not CssPrincipalBox childPrincipalBox)
                continue;

            EDisplayMode childDisplay = GetBoxDisplayMode(childPrincipalBox);

            // Check if this child is a misparented table-internal box
            if (!TableInternalDisplayType.IsMisparented(parentDisplay, childDisplay))
                continue;

            // Generate the necessary anonymous wrapper boxes
            var (outermost, innermost) = GenerateTableWrapperChain(parentBox, childPrincipalBox, parentDisplay, childDisplay);
            if (outermost is not null && innermost is not null)
            {
                replacements.Add((i, child, outermost, innermost));
            }
        }

        // Apply replacements in reverse order to maintain correct indices
        for (int i = replacements.Count - 1; i >= 0; i--)
        {
            var (index, originalChild, outermostWrapper, innermostWrapper) = replacements[i];

            // Remove the original child at its index (this also unparents it)
            parentBox.childNodes.RemoveAt(index);

            // Add the original child to the innermost wrapper
            innermostWrapper.Add(originalChild);

            // Insert the outermost wrapper at the same position
            parentBox.Insert(index, outermostWrapper);
        }

        // If we made changes, recursively check the new structure
        // (newly created anonymous boxes may themselves need fixup)
        if (replacements.Count > 0)
        {
            foreach (var child in parentBox.childNodes)
            {
                if (child is CssBox childBox)
                {
                    FixupTableInternalBoxes(childBox);
                }
            }
        }
    }

    /// <summary>
    /// Generates a chain of anonymous table wrapper boxes to properly parent a misparented table-internal box.
    /// </summary>
    /// <param name="parentBox">The current (incompatible) parent box.</param>
    /// <param name="childBox">The misparented table-internal child box.</param>
    /// <param name="parentDisplay">The display mode of the parent.</param>
    /// <param name="childDisplay">The display mode of the child.</param>
    /// <returns>A tuple of (outermost wrapper, innermost wrapper) where the child should be added to innermost.</returns>
    private static (CssAnonymousBox? Outermost, CssAnonymousBox? Innermost) GenerateTableWrapperChain(
        CssBox parentBox,
        CssPrincipalBox childBox,
        EDisplayMode parentDisplay,
        EDisplayMode childDisplay)
    {
        /*
         * Per CSS 2.2 §17.2.1, the wrapper chain depends on the child type:
         *
         * table-cell → needs table-row → needs table-row-group → needs table
         * table-row → needs table-row-group → needs table
         * table-row-group/header-group/footer-group → needs table
         * table-column → needs table-column-group → needs table
         * table-column-group → needs table
         * table-caption → needs table
         *
         * The chain stops when we reach a valid parent type.
         */

        // Determine if we should use inline-table based on parent context
        // Per CSS 2.2 §17.2.1: "If C's parent is an 'inline' box, then T must be an 'inline-table' box"
        bool useInlineTable = parentBox.IsInlineLevel;

        // Build the wrapper chain from outside (table) to inside (immediate parent of child)
        CssAnonymousBox? outermostWrapper = null;
        CssAnonymousBox? innermostWrapper = null;

        // Helper to add a wrapper layer
        void AddWrapper(CssAnonymousBox wrapper)
        {
            if (outermostWrapper is null)
            {
                outermostWrapper = wrapper;
                innermostWrapper = wrapper;
            }
            else
            {
                innermostWrapper!.Add(wrapper);
                innermostWrapper = wrapper;
            }
        }

        // Generate wrappers based on what's needed
        switch (childDisplay)
        {
            case EDisplayMode.TABLE_CELL:
                // table-cell needs: table-row → table-row-group → table
                if (!TableInternalDisplayType.IsTableBox(parentDisplay))
                {
                    AddWrapper(useInlineTable
                        ? CssAnonymousBox.Create_InlineTable(null!)
                        : CssAnonymousBox.Create_Table(null!));
                }
                if (!TableInternalDisplayType.IsRowGroupBox(parentDisplay) && !TableInternalDisplayType.IsTableBox(parentDisplay))
                {
                    AddWrapper(CssAnonymousBox.Create_TableRowGroup(null!));
                }
                if (parentDisplay != EDisplayMode.TABLE_ROW)
                {
                    AddWrapper(CssAnonymousBox.Create_TableRow(null!));
                }
                break;

            case EDisplayMode.TABLE_ROW:
                // table-row needs: table-row-group → table
                if (!TableInternalDisplayType.IsTableBox(parentDisplay))
                {
                    AddWrapper(useInlineTable
                        ? CssAnonymousBox.Create_InlineTable(null!)
                        : CssAnonymousBox.Create_Table(null!));
                }
                if (!TableInternalDisplayType.IsRowGroupBox(parentDisplay) && !TableInternalDisplayType.IsTableBox(parentDisplay))
                {
                    AddWrapper(CssAnonymousBox.Create_TableRowGroup(null!));
                }
                break;

            case EDisplayMode.TABLE_ROW_GROUP:
            case EDisplayMode.TABLE_HEADER_GROUP:
            case EDisplayMode.TABLE_FOOTER_GROUP:
                // row groups need: table
                if (!TableInternalDisplayType.IsTableBox(parentDisplay))
                {
                    AddWrapper(useInlineTable
                        ? CssAnonymousBox.Create_InlineTable(null!)
                        : CssAnonymousBox.Create_Table(null!));
                }
                break;

            case EDisplayMode.TABLE_COLUMN:
                // table-column needs: table-column-group → table
                if (!TableInternalDisplayType.IsTableBox(parentDisplay))
                {
                    AddWrapper(useInlineTable
                        ? CssAnonymousBox.Create_InlineTable(null!)
                        : CssAnonymousBox.Create_Table(null!));
                }
                if (parentDisplay != EDisplayMode.TABLE_COLUMN_GROUP && !TableInternalDisplayType.IsTableBox(parentDisplay))
                {
                    AddWrapper(CssAnonymousBox.Create_TableColumnGroup(null!));
                }
                break;

            case EDisplayMode.TABLE_COLUMN_GROUP:
            case EDisplayMode.TABLE_CAPTION:
                // column groups and captions need: table
                if (!TableInternalDisplayType.IsTableBox(parentDisplay))
                {
                    AddWrapper(useInlineTable
                        ? CssAnonymousBox.Create_InlineTable(null!)
                        : CssAnonymousBox.Create_Table(null!));
                }
                break;
        }

        // The innermost wrapper is where the child should be added
        // NOTE: We do NOT add the child here - that's done by the caller
        // after removing the child from its current parent

        return (outermostWrapper, innermostWrapper);
    }

    /// <summary>
    /// Gets the display mode for a box. For principal boxes, returns the owning element's display.
    /// For anonymous boxes, returns the SourceDisplayMode property.
    /// </summary>
    private static EDisplayMode GetBoxDisplayMode(CssBox box)
    {
        if (box is CssPrincipalBox principalBox && principalBox.Owner is Element element)
        {
            return element.Style.Display;
        }

        if (box is CssAnonymousBox anonymousBox)
        {
            return anonymousBox.SourceDisplayMode;
        }

        // For other cases, infer from DisplayType
        var displayType = box.DisplayType;

        if (displayType.Inner == EInnerDisplayType.Table)
        {
            return displayType.Outer == EOuterDisplayType.Inline
                ? EDisplayMode.INLINE_TABLE
                : EDisplayMode.TABLE;
        }

        // Default based on outer/inner display
        if (displayType.Outer == EOuterDisplayType.Block && displayType.Inner == EInnerDisplayType.Flow_Root)
            return EDisplayMode.BLOCK;
        if (displayType.Outer == EOuterDisplayType.Inline && displayType.Inner == EInnerDisplayType.Flow)
            return EDisplayMode.INLINE;

        return EDisplayMode.BLOCK; // Default fallback
    }

    #endregion

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

        // Check for display: table - generates TWO boxes (wrapper + grid)
        // Per CSS Display 3 §2.2: "The element generates a principal table wrapper box that
        // establishes a block formatting context, and which contains an additionally-generated
        // table grid box that establishes a table formatting context."
        if (displayType.Inner == EInnerDisplayType.Table)
        {
            return Generate_Table_Boxes(Node);
        }

        // Create box without parent - Generate_Tree() will add it to the parent's children
        // This avoids the parentNode != null check in Generate_Tree that would skip tree insertion
        CssPrincipalBox box = new CssPrincipalBox(Node, null!);

        // Per CSS Display 3 §2.3: list-item generates a ::marker pseudo-element
        // The marker box is the list item's first child, before ::before pseudo-element
        if (Node.Style.Display == EDisplayMode.LIST_ITEM)
        {
            Generate_Marker_Box(Node, box);
        }

        // Note: Anonymous box normalization is handled in Generate_Tree() after all
        // children of block containers are processed (per CSS 2.2 §9.2.1.1)

        return box;
    }

    /// <summary>
    /// Generates a marker box for a list item element.
    /// Per CSS Display 3 §2.3 and CSS Lists 3 §3.1.
    /// </summary>
    /// <param name="listItem">The list item element.</param>
    /// <param name="principalBox">The principal box of the list item.</param>
    private static void Generate_Marker_Box(Element listItem, CssPrincipalBox principalBox)
    {
        // Only generate marker if list-style-type is not 'none' (and no list-style-image)
        // Per CSS Lists 3 §3.2: if no marker content, ::marker does not generate a box
        var markerType = listItem.Style.ListStyleType;
        if (markerType == Enums.EListStyleType.None)
        {
            return;
        }

        // Create the marker box without a parent (will be inserted at position 0)
        var markerBox = new CssMarkerBox(listItem);

        // Calculate counter value based on position among siblings
        int counterValue = Calculate_List_Item_Counter(listItem);
        markerBox.SetCounterValue(counterValue);

        // Insert marker box as first child of the principal box
        principalBox.Insert(0, markerBox);
    }

    /// <summary>
    /// Calculates the list-item counter value for a list item element.
    /// Per CSS Lists 3 §4.6: list items automatically increment a special 'list-item' counter.
    /// </summary>
    /// <param name="listItem">The list item element.</param>
    /// <returns>The 1-based counter value for this list item.</returns>
    private static int Calculate_List_Item_Counter(Element listItem)
    {
        // Find the parent list container and count preceding list item siblings
        var parent = listItem.parentElement;
        if (parent is null)
        {
            return 1;
        }

        int counter = 0;
        Element? current = parent.firstElementChild;
        while (current is not null)
        {
            // Count list items (elements with display: list-item)
            if (current.Style.Display == EDisplayMode.LIST_ITEM)
            {
                counter++;
            }

            if (ReferenceEquals(current, listItem))
            {
                break;
            }

            current = current.nextElementSibling;
        }

        return counter > 0 ? counter : 1;
    }

    /// <summary>
    /// Generates both the table wrapper box and table grid box for a table element.
    /// Per CSS Display 3 §2.2 and CSS Tables 3 §4.
    /// </summary>
    /// <param name="tableElement">The element with display: table.</param>
    /// <returns>The table wrapper box (principal box) with grid box as its child.</returns>
    /// <remarks>
    /// Per CSS Display 3 §2.2:
    /// "The element generates a principal table wrapper box that establishes a block formatting
    /// context, and which contains an additionally-generated table grid box that establishes a
    /// table formatting context."
    ///
    /// The table wrapper box is the principal box that:
    /// - Is returned from this method (becomes the element's box)
    /// - Establishes a block formatting context
    /// - Contains the table grid box as a child
    /// - Receives inherited properties from the element
    ///
    /// The table grid box:
    /// - Is an additionally-generated box (not the principal)
    /// - Establishes a table formatting context
    /// - Contains the table's internal structure (rows, cells, etc.)
    /// - Receives certain non-inherited properties (border, padding, etc.)
    ///
    /// Note: For simplicity, both boxes reference the same originating element.
    /// Property application is handled by the layout and rendering algorithms.
    /// </remarks>
    private static CssTableWrapperBox Generate_Table_Boxes(Element tableElement)
    {
        // Create the table wrapper box (principal box) without parent
        // Generate_Tree() will handle adding it to the parent
        var wrapperBox = new CssTableWrapperBox(tableElement, parent: null);

        // Create the table grid box WITHOUT parent first
        // We'll add it to the wrapper in the next step
        var gridBox = new CssTableGridBox(tableElement, parent: null);

        // Establish the bidirectional relationship
        wrapperBox.SetGridBox(gridBox);

        // Add the grid box as a child of the wrapper
        // This will set gridBox.parentNode = wrapperBox
        wrapperBox.Add(gridBox);

        // The table element's box tree children (table-row-groups, table-rows, etc.)
        // will be added to the grid box during Generate_Tree()'s child processing

        return wrapperBox;
    }

    /// <summary>
    /// Applies automatic box type transformations (blockification/inlinification) per CSS Display 3 §2.7.
    /// </summary>
    /// <remarks>
    /// As of Phase 14.6.6, blockification is handled during style cascade via the Display_Computed resolver.
    /// This method now serves as a passthrough that returns the computed display type directly,
    /// which already has blockification applied for:
    /// - Absolutely/fixed positioned elements
    /// - Children of flex/grid containers
    /// - Root elements (per CSS Display 3 §2.8)
    /// - Floated elements (when Float property is implemented in Phase 15)
    ///
    /// Inlinification (for ruby containers) is not yet implemented.
    /// </remarks>
    private static DisplayType ApplyBoxTypeTransformations(in Element Node, DisplayType displayType)
    {
        // Blockification is now handled in the Display computed value resolver (CssPropertyResolver.Display_Computed)
        // The displayType parameter already reflects the computed display value with blockification applied.
        // This method is kept for potential future inlinification logic (e.g., ruby containers).

        // Skip if already has no box or contents
        if (displayType.Outer == EOuterDisplayType.None || displayType.Outer == EOuterDisplayType.Contents)
        {
            return displayType;
        }

        // @todo: Add inlinification for ruby container children when ruby is implemented

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
    /// Gets the appropriate container box for inserting children.
    /// For table wrapper boxes, returns the grid box.
    /// For all other boxes, returns the box itself.
    /// </summary>
    /// <param name="box">The parent element's box.</param>
    /// <returns>The box that should receive children.</returns>
    /// <remarks>
    /// Per CSS Display 3 §2.2 and CSS Tables 3 §4:
    /// Table elements generate two boxes - a wrapper and a grid.
    /// The wrapper is the principal box, but children are added to the grid.
    /// </remarks>
    private static CssBoxTreeNode Get_Container_Box(CssBoxTreeNode box)
    {
        // If this is a table wrapper box, children go into the grid box
        if (box is CssTableWrapperBox wrapperBox && wrapperBox.GridBox is not null)
        {
            return wrapperBox.GridBox;
        }

        // For all other boxes, children are added directly
        return box;
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
                    // Run-in boxes are DEFERRED per Phase 14.6.7
                    // CSS Display Level 3 marks 'display: run-in' as "at-risk" and may be dropped from the spec.
                    // No modern browsers support run-in (Chrome/Firefox/Safari all removed support).
                    // Complex implementation requires post-generation tree manipulation for box merging/reparenting.
                    // For now, treat run-in as inline-level (fallback behavior).
                    // @todo Future: Implement run-in sequence detection and merging if spec stabilizes
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

