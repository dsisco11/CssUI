using CssUI.CSS.Enums;

namespace CssUI.CSS.BoxTree
{
    /* 
     * Docs: https://www.w3.org/TR/css-box-3/#box-model
     */

    /// <summary>
    /// Represents a box-model object which holds other box-model objects as children
    /// </summary>
    public abstract class CssBox : CssBoxTreeNode
    {
        #region Properties

        #region Flags
        public EBoxFlags Flags = 0x0;
        #endregion

        #region Dirty Flags
        public EBoxInvalidationReason Dirt { get; protected set; } = EBoxInvalidationReason.Clean;

        /// <summary>
        /// Adds a flag to the dirty bit
        /// </summary>
        internal void Flag(EBoxInvalidationReason flag) { Dirt |= flag; }
        /// <summary>
        /// Removes a flag for the dirty bit
        /// </summary>
        internal void Unflag(EBoxInvalidationReason flag) { Dirt &= ~flag; }

        public bool IsDirty => Dirt != EBoxInvalidationReason.Clean;
        #endregion


        #region Display Accessors
        public virtual DisplayType DisplayType { get; protected set; }
        public bool IsReplacedElement => 0 != (Flags & EBoxFlags.IsReplaced);
        /// <summary>
        /// A block container either contains only inline-level boxes participating in an inline formatting context, 
        /// or contains only block-level boxes participating in a block formatting context (possibly generating anonymous block boxes to ensure this constraint, as defined in CSS2§9.2.1.1).
        /// </summary>
        public bool IsBlockContainer => DisplayType.IsBlockContainer;

        /// <summary> 
        /// Content that participates in block layout. Specifically, block-level boxes have an Outer display type of 'Block'
        /// </summary>
        public bool IsBlockLevel => (DisplayType.Outer == EOuterDisplayType.Block);
        /// <summary>
        /// Content that participates in inline layout. Specifically, inline-level boxes and text runs.
        /// </summary>
        public bool IsInlineLevel => (DisplayType.Outer == EOuterDisplayType.Inline || DisplayType.Outer == EOuterDisplayType.Run_In);

        /// <summary>
        /// A block-level box that is also a block container.
        /// <para>Note: Not all block container boxes are block-level boxes: non-replaced inline blocks and non-replaced table cells, for example.</para>
        /// </summary>
        public bool IsBlockBox => (IsBlockLevel && IsBlockContainer);
        /// <summary>
        /// A non-replaced inline-level box whose inner display type is flow. The contents of an inline box participate in the same inline formatting context as the inline box itself.
        /// </summary>
        public bool IsInlineBox => (!IsReplacedElement && DisplayType.Inner == EInnerDisplayType.Flow);

        /// <summary>
        /// An inline-level box that is replaced (such as an image) or that establishes a new formatting context (such as an inline-block or inline-table) and cannot split across lines (as inline boxes and ruby containers can).
        /// Any inline-level box whose inner display type is not flow establishes a new formatting context of the specified inner display type.
        /// </summary>
        public bool IsAtomicInline => (IsInlineLevel && (IsReplacedElement || DisplayType.Inner == EInnerDisplayType.Flow_Root));
        #endregion
        #endregion

        #region Constructors
        public CssBox(in CssBoxTreeNode parent) : base(parent)
        {
            childNodes.onAdded += ChildNodes_onAdded;
            childNodes.onRemoved += ChildNodes_onRemoved;
            childNodes.onChanged += ChildNodes_onChanged;
        }
        #endregion


        #region Event Handling
        private void ChildNodes_onAdded(int Index, NodeTree.ITreeNode Source) => Flag(EBoxInvalidationReason.Unknown);
        private void ChildNodes_onRemoved(int Index, NodeTree.ITreeNode Source) => Flag(EBoxInvalidationReason.Unknown);
        private void ChildNodes_onChanged(NodeTree.ITreeNode OldValue, NodeTree.ITreeNode NewValue) => Flag(EBoxInvalidationReason.Unknown);
        #endregion
    }
}
