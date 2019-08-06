using System;
using System.Collections;
using System.Collections.Generic;
using CssUI.Enums;
using System.Linq;
using CssUI.Internal;
using CssUI.CSS;
using System.Diagnostics;
using CssUI.CSS.Enums;
using CssUI.DOM;
using CssUI.DOM.Nodes;
using CssUI.CSS.Formatting;
using CssUI.CSS.Internal;
using CssUI.DOM.Geometry;

namespace CssUI
{
    /// <summary>
    /// A box as defined in the CSS level 3 box model standards, containing all of the 4 areas: content, padding, border, and margin
    /// It is actually acontainer for multiple box fragments
    /// Its origin is Top, Left.
    /// </summary>
    public partial class CssPrincipalBox : FragmentationRoot
    {
        /* 
         * Docs: https://www.w3.org/TR/CSS22/visuren.html#box-gen
         * Docs: https://www.w3.org/TR/css-box-3/#box-model
         * 
         * Docs: https://www.w3.org/TR/css-break-3/
         */

        #region Flags
        public EBoxFlags Flags = 0x0;
        #endregion

        #region Dirty Flags
        public EBoxInvalidationReason Dirt { get; private set; } = EBoxInvalidationReason.NotInvalid;
        public EPropertyDirtFlags PropertyDirt { get; private set; } = 0x0;
        /// <summary>
        /// Adds a flag to the dirty bit
        /// </summary>
        internal void Flag(EBoxInvalidationReason flag) { Dirt |= flag; }
        /// <summary>
        /// Removes a flag for the dirty bit
        /// </summary>
        internal void Unflag(EBoxInvalidationReason flag) { Dirt &= ~flag; }

        /// <summary>
        /// Adds a flag to the property-dirty bit
        /// </summary>
        internal void FlagProperty(EPropertyDirtFlags PropertyFlags) { PropertyDirt |= PropertyDirt; }
        /// <summary>
        /// Removes a flag to the property-dirty bit
        /// </summary>
        internal void UnflagProperty(EPropertyDirtFlags PropertyFlags) { PropertyDirt &= ~PropertyDirt; }

        public bool IsDirty { get => (Dirt != EBoxInvalidationReason.NotInvalid); }
        #endregion

        #region Box Areas
        /* XXX: Should we switch to using DOMRects for tracking these? */
        /* YES WE SHOULD */

        /// <summary>
        /// The edge positions of the Repalced-Content-Area 
        /// </summary>
        public CssBoxArea Replaced { get; private set; } = null;
        /// <summary>
        /// The edge positions of the Content-Area 
        /// </summary>
        public readonly CssBoxArea Content;
        /// <summary>
        /// The edge positions of the Padding-Area 
        /// </summary>
        public readonly CssBoxArea Padding;
        /// <summary>
        /// The edge positions of the Border-Area 
        /// </summary>
        public readonly CssBoxArea Border;
        /// <summary>
        /// The edge positions of the Margin-Area 
        /// </summary>
        public readonly CssBoxArea Margin;

        /// <summary>
        /// The block which represents the hitbox for mouse related input events
        /// </summary>
        public CssBoxArea ClickArea { get => this.Padding; }
        #endregion

        #region Properties
        private readonly cssElement Owner;
        internal IFormattingContext FormattingContext { get; private set; } = null;

        /// <summary>
        /// The 'Inner Display Type'
        /// Defines the *-level of this box, whether it is block-level, inline-level, or other.
        /// Dictates how the principal box itself participates in flow layout.
        /// </summary>
        public EOuterDisplayType OuterDisplayType { get; private set; } = 0x0;
        /// <summary>
        /// The 'Inner Display Type'
        /// Defines (if it is a non-replaced element) the kind of formatting context it generates, dictating how its descendant boxes are laid out. (The inner display of a replaced element is outside the scope of CSS.)
        /// </summary>
        public EInnerDisplayType InnerDisplayType { get; private set; } = 0x0;
        /// <summary>
        /// Determines how the boxes boundaries are calculated
        /// </summary>
        internal EBoxDisplayGroup DisplayGroup { get; private set; } = 0x0;

        /// <summary>
        /// Does this box's size depend on the size of it's children?
        /// </summary>
        public bool DependsOnChildren { get; private set; } = false;
        /// <summary>
        /// Does this box's size depend on the size of it's containing-block?
        /// </summary>
        public bool DependsOnContainer { get; private set; } = false;

        internal bool HasBlockLevelChildren { get; private set; } = false;
        #endregion

        #region Events
        /// <summary>
        /// One of the areas from this box has changed
        /// </summary>
        public event Action<ECssBoxType> onChange;

        #endregion

        #region Accessors
        public ElementPropertySystem Style { get => Owner.Style; }
        public bool IsReplacedElement => (0 != (Flags & EBoxFlags.REPLACED_ELEMENT));
        public bool IsFloating => false;
        /// <summary>
        /// Is this box absolutely positioned? (eg. <see cref="EPositioning.Absolute"/> or <see cref="EPositioning.Fixed"/>)
        /// </summary>
        public bool IsAbsolutelyPositioned => (Style.Positioning == EPositioning.Absolute | Style.Positioning == EPositioning.Fixed);
        /// <summary>
        /// Returns <c>True</c> if the Width was given an explicit value (i.e. it doesn't depend on content size)
        /// </summary>
        public bool IsWidthExplicit => (0 != (Style.Cascaded.Width.Specified.Type & (ECssValueType.DIMENSION | ECssValueType.PERCENT)) && !this.DependsOnChildren);
        /// <summary>
        /// Returns <c>True</c> if the Height was given an explicit value (i.e. it doesn't depend on content size)
        /// </summary>
        public bool IsHeightExplicit => (0 != (Style.Cascaded.Height.Specified.Type & (ECssValueType.DIMENSION | ECssValueType.PERCENT)) && !this.DependsOnChildren);

        /* XXX: Scrollbar : replace with the native DOM element code for scrollboxes */
        /// <summary>
        /// The size of our Elements scrollbar (if any)
        /// </summary>
        internal Size2D Scrollbar_Size { get; private set; } = new Size2D();


        /// <summary>
        /// Returns the Width of this boxes Margin-area
        /// </summary>
        public int Width { get => this.Margin.Width; }
        /// <summary>
        /// Returns the Height of this boxes Margin-area
        /// </summary>
        public int Height { get => this.Margin.Height; }

        /// <summary>
        /// This boxes X-axis position (relative to our container)
        /// </summary>
        public int X { get => this.Margin.X; }
        /// <summary>
        /// This boxes Y-axis position (relative to our container)
        /// </summary>
        public int Y { get => this.Margin.Y; }


        /// <summary>
        /// The margin area edge
        /// </summary>
        public int Top { get => this.Margin.Top; }
        /// <summary>
        /// The margin area edge
        /// </summary>
        public int Right { get => this.Margin.Right; }
        /// <summary>
        /// The margin area edge
        /// </summary>
        public int Bottom { get => this.Margin.Bottom; }
        /// <summary>
        /// The margin area edge
        /// </summary>
        public int Left { get => this.Margin.Left; }


        /// <summary>
        /// Backing value for <see cref="Containing_Box"/>
        /// </summary>
        private CssBoxArea _containing_box = null;
        /// <summary>
        /// The containing block of this element
        /// <para>If the control has an ancestor this will be said ancestors content-area block</para>
        /// <para>Otherwise, if the element is a root element, this should have the dimensions of the viewport</para>
        /// </summary>
        public CssBoxArea Containing_Box
        {
            get
            {
                if (_containing_box == null)
                {
                    _containing_box = Find_Containing_Block();
                }
                return _containing_box;
            }
        }

        /// <summary>
        /// Returns whether or not our containing block depends on our size
        /// </summary>
        internal bool Containing_Box_Dependent
        {
            get
            {
                switch (Owner.Style.Positioning)
                {
                    case EPositioning.Fixed:
                        return false;
                    default:
                        {
                            if (ReferenceEquals(Owner.Parent, null))
                                return false;
                            else
                                return Owner.Parent.Box.DependsOnChildren;
                        }
                }
            }
        }

        /// <summary>
        /// Returns whether or not our containing boxes Width depends on our size
        /// </summary>
        internal bool Containing_Box_Explicit_Width
        {
            get
            {
                if (ReferenceEquals(Owner.Parent, null))
                    return true;
                else
                    return Owner.Parent.Box.IsWidthExplicit;
            }
        }

        /// <summary>
        /// Returns whether or not our containing boxes Height depends on our size
        /// </summary>
        internal bool Containing_Box_Explicit_Height
        {
            get
            {
                if (ReferenceEquals(Owner.Parent, null))
                    return true;
                else
                    return Owner.Parent.Box.IsHeightExplicit;
            }
        }
        #endregion

        #region Getters
        public Size2D Get_Dimensions() { return new Size2D(this.Width, this.Height); }
        public Vec2i Get_Position() { return new Vec2i(this.X, this.Y); }
        #endregion

        #region Abstract Units
        /* Docs: https://www.w3.org/TR/css-writing-modes-4/#logical-to-physical */

        #endregion

        #region Parent Assigned Values
        /// <summary>
        /// The x-axis layout offset given to us by our parent
        /// </summary>
        internal int Layout_Pos_X { get; private set; } = 0;
        /// <summary>
        /// The y-axis layout offset given to us by our parent
        /// </summary>
        internal int Layout_Pos_Y { get; private set; } = 0;
        #endregion

        #region Intrinsic Values
        /* These are values which*/
        /* Docs: https://www.w3.org/TR/css-sizing-3/#intrinsic-sizes */

        internal int Preferred_Width => (Intrinsic_Width.HasValue ? Intrinsic_Width.Value : 0);
        internal int Preferred_Height => (Intrinsic_Height.HasValue ? Intrinsic_Height.Value : 0);

        internal int? Content_Width { get; private set; } = null;
        internal int? Content_Height { get; private set; } = null;


        internal int? Intrinsic_Width { get; private set; } = null;
        internal int? Intrinsic_Height { get; private set; } = null;
        /// <summary>
        /// The intrinsic ratio of Height/Width
        /// </summary>
        internal float? Intrinsic_Ratio { get; private set; } = null;

        /// <summary>
        /// The smallest size a box could take that doesn’t lead to overflow that could be avoided by choosing a larger size. (See §4 Intrinsic Size Determination.)
        /// </summary>
        internal Size2D Min_Content = new Size2D();
        /// <summary>
        /// The narrowest inline size a box could take that doesn’t lead to inline-dimension overflow that could be avoided by choosing a larger inline size. Roughly, the inline size that would fit around its contents if all soft wrap opportunities within the box were taken.
        /// </summary>
        internal Size2D Min_Content_Inline = new Size2D();
        /// <summary>
        /// In general, and definitely for block-level and inline-level boxes, this is equivalent to the max-content block size.
        /// </summary>
        internal Size2D Min_Content_Block = new Size2D();


        /// <summary>
        /// A box’s “ideal” size in a given axis when given infinite available space. Usually this is the smallest size the box could take in that axis while still fitting around its contents, i.e. minimizing unfilled space while avoiding overflow.
        /// </summary>
        internal Size2D Max_Content = new Size2D();
        /// <summary>
        /// The box’s “ideal” size in the inline axis. Usually the narrowest inline size it could take while fitting around its contents if none of the soft wrap opportunities within the box were taken. (See §4 Intrinsic Size Determination.)
        /// </summary>
        internal Size2D Max_Content_Inline = new Size2D();
        /// <summary>
        /// The box’s “ideal” size in the block axis. Usually the block size of the content after layout.
        /// </summary>
        internal Size2D Max_Content_Block = new Size2D();
        #endregion

        #region Constructor
        public CssPrincipalBox(cssElement Owner)
        {
            this.Owner = Owner;
        }
        #endregion

        #region Bounds Updating
        /// <summary>
        /// Update the box's boundaries, areas, and edges
        /// </summary>
        /// <param name="X"></param>
        /// <param name="Y"></param>
        /// <param name="Width"></param>
        /// <param name="Height"></param>
        private void Update_Bounds()
        {
            /* Update all of our padding, border-size, and margins */
            this.Padding.Size_Top = Style.Padding_Top;
            this.Padding.Size_Right = Style.Padding_Right;
            this.Padding.Size_Bottom = Style.Padding_Bottom;
            this.Padding.Size_Left = Style.Padding_Left;

            this.Border.Size_Top = Style.Border_Top_Width;
            this.Border.Size_Right = Style.Border_Right_Width;
            this.Border.Size_Bottom = Style.Border_Bottom_Width;
            this.Border.Size_Left = Style.Border_Left_Width;

            this.Margin.Size_Top = Style.Margin_Top;
            this.Margin.Size_Right = Style.Margin_Right;
            this.Margin.Size_Bottom = Style.Margin_Bottom;
            this.Margin.Size_Left = Style.Margin_Left;

            /* First if we are relatively positioned, set our margin edges up */
            switch (Style.Positioning)
            {
                case EPositioning.Static:
                    {
                        Update_Area_Bounds(this.X, this.Y);
                    }
                    break;
                case EPositioning.Relative:
                    {
                        int off_x = this.X + Style.Left + Style.Right;
                        int off_y = this.Y + Style.Top + Style.Bottom;

                        Update_Area_Bounds(off_x, off_y);
                    }
                    break;
                case EPositioning.Absolute:
                    {/* For an absolute element all of the TRBL values will have a resolved value */
                        Margin.Set_TRBL(Style.Top, Style.Right, Style.Bottom, Style.Left);
                        Border.Fit(Margin);
                        Padding.Fit(Border);
                        Content.Fit(Padding);
                    }
                    break;
                case EPositioning.Fixed:
                    {/* For a fixed element all of the TRBL values will have a resolved value */
                        Margin.Set_TRBL(Style.Top, Style.Right, Style.Bottom, Style.Left);
                        Border.Fit(Margin);
                        Padding.Fit(Border);
                        Content.Fit(Padding);
                    }
                    break;
                default:
                    throw new NotImplementedException($"Unhandled Css positioning type: {Enum.GetName(typeof(EPositioning), Style.Positioning)}");
            }

            Update_Replaced_Bounds();
        }

        /// <summary>
        /// Recalculates the coordinates for all sides of the Margin, Border, Padding, and Content areas of this box
        /// </summary>
        /// <param name="off_x"></param>
        /// <param name="off_y"></param>
        private void Update_Area_Bounds(int off_x, int off_y)
        {
            /* Content area */
            int cTop = off_y+(Margin.Size_Top + Border.Size_Top + Padding.Size_Top);
            int cLeft = off_x+(Margin.Size_Left + Border.Size_Left + Padding.Size_Left);

            int cRight = (cLeft + Style.Width);// - (Margin.Size_Right + Border.Size_Right + Padding.Size_Right);
            int cBottom = (cTop + Style.Height);// - (Margin.Size_Bottom + Border.Size_Bottom + Padding.Size_Bottom);

            Content.Set_TRBL(cTop, cRight, cBottom, cLeft);
            Padding.Encapsulate(Content);
            Border.Encapsulate(Padding);
            Margin.Encapsulate(Border);

            Debug.Assert(Content.Get_Dimensions().Width == Style.Width);
            Debug.Assert(Content.Get_Dimensions().Height == Style.Height);
        }

        /// <summary>
        /// Recalculates the coordinates for all sides of the replaced-content area
        /// </summary>
        private void Update_Replaced_Bounds()
        {
            Replaced = null;
            if (!this.IsReplacedElement)
                return;
            
            /* Recalculate object position */
            Style.Cascaded.ObjectPosition_X.UpdateDependent(true);
            Style.Cascaded.ObjectPosition_Y.UpdateDependent(true);

            /* Update replaced area */
            Replaced = new CssBoxArea(this);
            Replaced.Fit(this.Content, new Vec2i(Style.ObjectPosition_X, Style.ObjectPosition_Y), Get_Replaced_Block_Size());
        }
        #endregion

        #region Box Rebuilding
        /// <summary>
        /// Rebuilds this box's boundaries from the style information given
        /// </summary>
        /// <param name="Style"></param>
        public bool Rebuild(bool Force=false)
        {
            if (!this.IsDirty && !Force)
                return false;

            this.Resolve();

            /* change detection */
            var oldPos = new Vec2i(this.X, this.Y);
            var oldSize = new Size2D(this.Width, this.Height);
            int oldReplaced = this.Replaced?.GetHashCode() ?? 0;
            int oldContent = this.Content.GetHashCode();
            int oldPadding = this.Padding.GetHashCode();
            int oldBorder = this.Border.GetHashCode();
            int oldMargin = this.Margin.GetHashCode();
            /* Account for scrollbars */
            if (Owner is cssScrollableElement scrollable)
            {
                /* XXX: Create a CSSCommon class to help with the LogicalWidth & LogicalHeight determination */
                /* XXX: Replace this old scrollable element crap with the new native DOM element scrollboxes */
                this.Scrollbar_Size.Width = scrollable.SB_Vertical?.Box.Margin.LogicalWidth ?? 0;
                this.Scrollbar_Size.Height = scrollable.SB_Horizontal?.Box.Margin.LogicalHeight ?? 0;
            }
            else
            {
                this.Scrollbar_Size.Width = this.Scrollbar_Size.Height = 0;
            }

            Update_Display_Group();
            Update_Display_Types();
            Update_Depends_Flag();

            // Figure out if we have any block-level children
            // this.HasBlockLevelChildren = (null != this.SingleOrDefault((CssLayoutBox b) => (b.OuterDisplayType == EOuterDisplayType.Block)));
            this.HasBlockLevelChildren = false;
            Element node = Owner.firstElementChild;
            while (node != null)
            {
                if (node.Box.OuterDisplayType == EOuterDisplayType.Block)
                {
                    this.HasBlockLevelChildren = true;
                    break;
                }

                node = node.nextElementSibling;
            }

            /* Update our bounds */
            switch(this.DisplayGroup)
            {
                case EBoxDisplayGroup.INVALID:
                    {
                        this.Replaced = null;

                        this.Content.Size_Top = 0;
                        this.Content.Size_Right = 0;
                        this.Content.Size_Bottom = 0;
                        this.Content.Size_Left = 0;

                        this.Padding.Size_Top = 0;
                        this.Padding.Size_Right = 0;
                        this.Padding.Size_Bottom = 0;
                        this.Padding.Size_Left = 0;

                        this.Border.Size_Top = 0;
                        this.Border.Size_Right = 0;
                        this.Border.Size_Bottom = 0;
                        this.Border.Size_Left = 0;

                        this.Margin.Size_Top = 0;
                        this.Margin.Size_Right = 0;
                        this.Margin.Size_Bottom = 0;
                        this.Margin.Size_Left = 0;
                    }
                    break;
                default:
                    Update_Bounds();
                    break;
            }

            /* Reset dirt flags */
            Dirt = EBoxInvalidationReason.NotInvalid;
            PropertyDirt = 0x0;

            /* Check if this box has changed from the oustides perspective */
            var newPos = new Vec2i(this.X, this.Y);
            var newSize = new Size2D(this.Width, this.Height);

            bool HasChanged = false;
            bool eqReplaced = (oldReplaced != (this.Replaced?.GetHashCode() ?? 0));
            bool eqContent = (oldContent != this.Content.GetHashCode());
            bool eqPadding = (oldPadding != this.Padding.GetHashCode());
            bool eqBorder = (oldBorder != this.Border.GetHashCode());
            bool eqMargin = (oldMargin != this.Margin.GetHashCode());

            HasChanged = (oldPos != newPos || oldSize != newSize);
            HasChanged = HasChanged | eqReplaced;
            HasChanged = HasChanged | eqContent;
            HasChanged = HasChanged | eqPadding;
            HasChanged = HasChanged | eqBorder;
            HasChanged = HasChanged | eqMargin;

            if (HasChanged)
            {
                Owner.Invalidate_Layout(EBoxInvalidationReason.Block_Changed);

                ECssBoxType ChangeFlags = 0x0;
                if (eqReplaced) ChangeFlags |= ECssBoxType.Replaced;
                if (eqContent) ChangeFlags |= ECssBoxType.Content;
                if (eqPadding) ChangeFlags |= ECssBoxType.Padding;
                if (eqBorder) ChangeFlags |= ECssBoxType.Border;
                if (eqMargin) ChangeFlags |= ECssBoxType.Margin;

                this.onChange?.Invoke(ChangeFlags);
            }

            return true;
        }

        private CssBoxArea Find_Containing_Block()
        {
            return new CssBoxArea(CssCommon.Find_Containing_Block(Owner));
        }

        private void Update_Display_Types()
        {
            switch(Style.Display)
            {
                case EDisplayMode.BLOCK:
                    OuterDisplayType = EOuterDisplayType.Block;
                    InnerDisplayType = EInnerDisplayType.Flow;
                    break;
                case EDisplayMode.FLOW_ROOT:
                    OuterDisplayType = EOuterDisplayType.Block;
                    InnerDisplayType = EInnerDisplayType.Flow_Root;
                    break;
                case EDisplayMode.INLINE:
                    OuterDisplayType = EOuterDisplayType.Inline;
                    InnerDisplayType = EInnerDisplayType.Flow;
                    break;
                case EDisplayMode.INLINE_BLOCK:
                    OuterDisplayType = EOuterDisplayType.Inline;
                    InnerDisplayType = EInnerDisplayType.Flow_Root;
                    break;
                case EDisplayMode.RUN_IN:
                    OuterDisplayType = EOuterDisplayType.Run_In;
                    InnerDisplayType = EInnerDisplayType.Flow;
                    break;
                case EDisplayMode.LIST_ITEM:
                    OuterDisplayType = EOuterDisplayType.Block;
                    InnerDisplayType = EInnerDisplayType.Flow_Root;
                    break;
                case EDisplayMode.FLEX:
                    OuterDisplayType = EOuterDisplayType.Block;
                    InnerDisplayType = EInnerDisplayType.Flex;
                    break;
                case EDisplayMode.INLINE_FLEX:
                    OuterDisplayType = EOuterDisplayType.Inline;
                    InnerDisplayType = EInnerDisplayType.Flex;
                    break;
                case EDisplayMode.GRID:
                    OuterDisplayType = EOuterDisplayType.Block;
                    InnerDisplayType = EInnerDisplayType.Grid;
                    break;
                case EDisplayMode.INLINE_GRID:
                    OuterDisplayType = EOuterDisplayType.Inline;
                    InnerDisplayType = EInnerDisplayType.Grid;
                    break;
                case EDisplayMode.TABLE:
                    OuterDisplayType = EOuterDisplayType.Block;
                    InnerDisplayType = EInnerDisplayType.Table;
                    break;
                case EDisplayMode.INLINE_TABLE:
                    OuterDisplayType = EOuterDisplayType.Inline;
                    InnerDisplayType = EInnerDisplayType.Table;
                    break;
                default:
                    throw new NotImplementedException("Display type has not been implemented yet");
            }
        }

        private void Update_Display_Group()
        {
            switch (Style.Display)
            {
                case EDisplayMode.BLOCK:
                    DisplayGroup = EBoxDisplayGroup.BLOCK;
                    break;
                case EDisplayMode.INLINE_BLOCK:
                    DisplayGroup = EBoxDisplayGroup.INLINE_BLOCK;
                    break;
                case EDisplayMode.INLINE:
                case EDisplayMode.INLINE_FLEX:
                case EDisplayMode.INLINE_GRID:
                case EDisplayMode.INLINE_TABLE:
                    DisplayGroup = EBoxDisplayGroup.INLINE;
                    break;
                default:
                    DisplayGroup = EBoxDisplayGroup.INVALID;
                    break;
            }

            if (IsAbsolutelyPositioned)
            {
                DisplayGroup = EBoxDisplayGroup.ABSOLUTELY_POSITIONED;
            }
        }

        /// <summary>
        /// Checks if any of properties used to calculate the block will depend on our parent block
        /// </summary>
        private void Update_Depends_Flag()
        {
            DependsOnChildren = (this.DisplayGroup == EBoxDisplayGroup.INLINE);
            DependsOnContainer = false;
            // Width / Height
            if (Style.Cascaded.Width.IsPercentageOrAuto || Style.Cascaded.Height.IsPercentageOrAuto) DependsOnContainer = true;
            // Size-Max
            else if (Style.Cascaded.Max_Width.IsPercentageOrAuto || Style.Cascaded.Max_Height.IsPercentageOrAuto) DependsOnContainer = true;
            // Size-Min
            else if (Style.Cascaded.Min_Width.IsPercentageOrAuto || Style.Cascaded.Min_Height.IsPercentageOrAuto) DependsOnContainer = true;
            // Margin
            else if (Style.Cascaded.Margin_Top.IsPercentageOrAuto) DependsOnContainer = true;
            else if (Style.Cascaded.Margin_Right.IsPercentageOrAuto) DependsOnContainer = true;
            else if (Style.Cascaded.Margin_Bottom.IsPercentageOrAuto) DependsOnContainer = true;
            else if (Style.Cascaded.Margin_Left.IsPercentageOrAuto) DependsOnContainer = true;
            // Padding
            else if (Style.Cascaded.Padding_Top.IsPercentageOrAuto) DependsOnContainer = true;
            else if (Style.Cascaded.Padding_Right.IsPercentageOrAuto) DependsOnContainer = true;
            else if (Style.Cascaded.Padding_Bottom.IsPercentageOrAuto) DependsOnContainer = true;
            else if (Style.Cascaded.Padding_Left.IsPercentageOrAuto) DependsOnContainer = true;
            // Positioning
            else if (Style.Cascaded.Top.IsPercentageOrAuto) DependsOnContainer = true;
            else if (Style.Cascaded.Right.IsPercentageOrAuto) DependsOnContainer = true;
            else if (Style.Cascaded.Bottom.IsPercentageOrAuto) DependsOnContainer = true;
            else if (Style.Cascaded.Left.IsPercentageOrAuto) DependsOnContainer = true;

        }
        #endregion

        #region Layout
        internal void Set_Layout_Pos(int X, int Y)
        {
            Layout_Pos_X = X;
            Layout_Pos_Y = Y;

            Owner.Style.Handle_Layout_Position_Change();
            // These 'Layout_Pos_' vars have the same effect as any other styling property in that WHENEVER they change it will effect how the owning uiElement's BLOCK placement.
            //Property_Changed?.Invoke(null, EPropertyFlags.Block | EPropertyFlags.Flow, Stack);// Replaced with the lines below on 06-19-2017
            Owner.Flag_Box_Dirty(EBoxInvalidationReason.Layout_Pos_Changed);

            // SHOULD we be invalidating Flow(layout) whenever an element's layout pos changes?
            Owner.Invalidate_Layout(EBoxInvalidationReason.Layout_Pos_Changed);
        }
        #endregion

        #region Content Sizing

        public void Set_Content_Size(int? Width, int? Height)
        {
            bool changed = (Width != this.Content_Width || Height != this.Content_Height);
            if (!changed) return;

            this.Content_Width = Width;
            this.Content_Height = Height;
            this.Flag(EBoxInvalidationReason.Content_Changed);
        }

        public void Set_Content_Width(int? Width)
        {
            bool changed = (Width != this.Content_Width);
            if (!changed) return;

            this.Content_Width = Width;

            this.Flag(EBoxInvalidationReason.Content_Changed);
        }

        public void Set_Content_Height(int? Height)
        {
            bool changed = (Height != this.Content_Height);
            if (!changed) return;

            this.Content_Height = Height;
            this.Flag(EBoxInvalidationReason.Content_Changed);
        }
        #endregion

        #region Intrinsic Sizing
        public void Set_Intrinsic_Size(int? Width, int? Height)
        {
            bool changed = (Width != this.Intrinsic_Width || Height != this.Intrinsic_Height);
            if (!changed) return;

            this.Intrinsic_Width = Width;
            this.Intrinsic_Height = Height;

            if (Width.HasValue && Height.HasValue)
                this.Intrinsic_Ratio = ((float)Height.Value / (float)Width.Value);
            else
                this.Intrinsic_Ratio = null;

            Flag(EBoxInvalidationReason.Content_Changed);
        }

        public void Set_Intrinsic_Width(int? Width)
        {
            bool changed = (Width != this.Intrinsic_Width);
            if (!changed) return;

            this.Intrinsic_Width = Width;
            this.Flag(EBoxInvalidationReason.Content_Changed);
        }

        public void Set_Intrinsic_Height(int? Height)
        {
            bool changed = (Height != this.Intrinsic_Height);
            if (!changed) return;

            this.Intrinsic_Height = Height;
            this.Flag(EBoxInvalidationReason.Content_Changed);
        }

        public void Set_Intrinsic_Ratio(float? Ratio)
        {
            bool changed = (Ratio != this.Intrinsic_Ratio);
            if (!changed) return;

            this.Intrinsic_Ratio = Ratio;
            this.Flag(EBoxInvalidationReason.Content_Changed);
        }

        #endregion

        #region Hit Testing
        /// <summary>
        /// Returns <c>True</c> if the given x/y coordinates lie within the click area of this box
        /// </summary>
        public bool HitTest(double x, double y)
        {
            DOMRect rect = ClickArea.getBoundingClientRect();
            return (x >= rect.left && x <= rect.right && y >= rect.top && y <= rect.bottom);
        }
        #endregion
    }
}
