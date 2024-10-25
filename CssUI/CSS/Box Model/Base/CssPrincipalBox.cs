using System;
using System.Diagnostics;
using System.Diagnostics.Contracts;
using CssUI.CSS.Enums;
using CssUI.CSS.Formatting;
using CssUI.CSS.Internal;
using CssUI.DOM;
using CssUI.DOM.Geometry;

namespace CssUI.CSS.BoxTree
{
    /* Docs: https://www.w3.org/TR/2019/CR-css-display-3-20190711/ */

    /* 
    * Docs: https://www.w3.org/TR/CSS22/visuren.html#containing-block
    * Docs: https://www.w3.org/TR/CSS22/visuren.html#box-gen
    * Docs: https://www.w3.org/TR/css-box-3/#box-model
    * 
    * Docs: https://www.w3.org/TR/css-break-3/
    */

    /// <summary>
    /// A box as defined in the CSS level 3 box model standards, containing all of the 4 areas: content, padding, border, and margin.
    /// Origin: Top, Left.
    /// </summary>
    public class CssPrincipalBox : CssBox
    {
        #region Properties

        #region Backing
        private WeakReference<Element> _owner;
        #endregion

        #region Display Types
        /// <summary> Does this box's size depend on the size of it's children? </summary>
        public bool DependsOnChildren { get; protected set; } = false;
        /// <summary> Does this box's size depend on the size of it's containing-block? </summary>
        public bool DependsOnContainer { get; protected set; } = false;
        /// <summary> </summary>
        public bool HasBlockLevelChildren { get; protected set; } = false;

        /// <summary> Determines how the box's boundaries are calculated </summary>
        public EBoxDisplayGroup DisplayGroup
        {
            get
            {
                if (IsAbsolutelyPositioned)
                {
                    return EBoxDisplayGroup.ABSOLUTELY_POSITIONED;
                }

                switch (Style.Display)
                {
                    case EDisplayMode.BLOCK:
                        {
                            return EBoxDisplayGroup.BLOCK;
                        }
                    case EDisplayMode.INLINE_BLOCK:
                        {
                            return EBoxDisplayGroup.INLINE_BLOCK;
                        }
                    case EDisplayMode.INLINE:
                    case EDisplayMode.INLINE_FLEX:
                    case EDisplayMode.INLINE_GRID:
                    case EDisplayMode.INLINE_TABLE:
                        {
                            return EBoxDisplayGroup.INLINE;
                        }
                    default:
                        {
                            return EBoxDisplayGroup.INVALID;
                        }
                }
            }
        }
        #endregion

        #region Formatting Context
        internal IFormattingContext FormattingContext { get; private set; } = null;
        public bool IsParticipatingInFlow => !(FormattingContext is null);
        #endregion

        #region Box Areas
        /// <summary>
        /// The edge positions of the Replaced-Content-Area 
        /// </summary>
        public Rect4f Replaced { get; protected set; } = null;
        /// <summary>
        /// The edge positions of the Content-Area 
        /// </summary>
        public Rect4f Content { get; protected set; } = null;
        /// <summary>
        /// The edge positions of the Padding-Area 
        /// </summary>
        public Rect4f Padding { get; protected set; } = null;
        /// <summary>
        /// The edge positions of the Border-Area 
        /// </summary>
        public Rect4f Border { get; protected set; } = null;
        /// <summary>
        /// The edge positions of the Margin-Area 
        /// </summary>
        public Rect4f Margin { get; protected set; } = null;
        /// <summary>
        /// The block which represents the hitbox for mouse related input events
        /// </summary>
        public Rect4f ClickArea => Padding;
        #endregion


        #region Accessors
        public Element Owner
        {
            get
            {
                if (_owner.TryGetTarget(out Element outValue))
                    return outValue;

                return null;
            }
        }
        public virtual StyleProperties Style => Owner.Style;

        public override DisplayType DisplayType => new DisplayType(Style.Display);

        public bool IsFloating => false;
        /// <summary>
        /// Is this box absolutely positioned? (eg. <see cref="EBoxPositioning.Absolute"/> or <see cref="EBoxPositioning.Fixed"/>)
        /// </summary>
        public bool IsAbsolutelyPositioned => Style.Positioning == EBoxPositioning.Absolute | Style.Positioning == EBoxPositioning.Fixed;
        /// <summary>
        /// Returns <c>True</c> if the Width was given an explicit value (i.e. it doesn't depend on content size)
        /// </summary>
        public bool IsWidthExplicit => 0 != (Style.Cascaded.Width.Specified.Type & (ECssValueTypes.DIMENSION | ECssValueTypes.PERCENT)) && !DependsOnChildren;
        /// <summary>
        /// Returns <c>True</c> if the Height was given an explicit value (i.e. it doesn't depend on content size)
        /// </summary>
        public bool IsHeightExplicit => 0 != (Style.Cascaded.Height.Specified.Type & (ECssValueTypes.DIMENSION | ECssValueTypes.PERCENT)) && !DependsOnChildren;

        /// <summary>
        /// <c>True</c> is this box is in a fragmentainer and being laid out in fragmented flow
        /// </summary>
        public bool InFragmentedFlow => false;

        /// <summary>
        /// The margin area edge
        /// </summary>
        public double Top => Margin.Top;
        /// <summary>
        /// The margin area edge
        /// </summary>
        public double Right => Margin.Right;
        /// <summary>
        /// The margin area edge
        /// </summary>
        public double Bottom => Margin.Bottom;
        /// <summary>
        /// The margin area edge
        /// </summary>
        public double Left => Margin.Left;

        /// <inheritdoc/>
        public override ReadOnlyRect2f Size { get => new ReadOnlyRect2f(Margin.Width, Margin.Height); set => throw new NotSupportedException($"The layout size of a {nameof(CssPrincipalBox)} cannot be set directly, it is determined by the box-model!"); }

        /// <summary>
        /// Backing value for <see cref="Containing_Box"/>
        /// </summary>
        private Rect4f _containing_box = null;
        /// <summary>
        /// The containing block of this element
        /// <para>If the control has an ancestor this will be said ancestors content-area block</para>
        /// <para>Otherwise, if the element is a root element, this should have the dimensions of the viewport</para>
        /// </summary>
        public Rect4f Containing_Box
        {
            get
            {
                if (_containing_box is null)
                {
                    _containing_box = CssCommon.Find_Containing_Block(Owner);
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
                    case EBoxPositioning.Fixed:
                        return false;
                    default:
                        {
                            if (Owner.parentElement is null)
                                return false;
                            else
                                return Owner.parentElement.Box.DependsOnChildren;
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
                if (Owner.parentElement is null)
                    return true;
                else
                    return Owner.parentElement.Box.IsWidthExplicit;
            }
        }

        /// <summary>
        /// Returns whether or not our containing boxes Height depends on our size
        /// </summary>
        internal bool Containing_Box_Explicit_Height
        {
            get
            {
                if (Owner.parentElement is null)
                    return true;
                else
                    return Owner.parentElement.Box.IsHeightExplicit;
            }
        }
        #endregion

        #region Events
        /// <summary>
        /// One of the areas from this box has changed
        /// </summary>
        public event Action<ECssBoxType> onChange;

        #endregion

        #region Abstract Units
        /* Docs: https://www.w3.org/TR/css-writing-modes-4/#logical-to-physical */
        /* XXX: Implement */
        #endregion

        #region Parent Assigned Values
        /// <summary>
        /// The x-axis layout offset given to us by our parent
        /// </summary>
        internal double Layout_Pos_X { get; private set; } = 0;
        /// <summary>
        /// The y-axis layout offset given to us by our parent
        /// </summary>
        internal double Layout_Pos_Y { get; private set; } = 0;
        #endregion

        #region Intrinsic Values
        /* These are values which */
        /* Docs: https://www.w3.org/TR/css-sizing-3/#intrinsic-sizes */

        internal double Preferred_Width => Intrinsic_Width.HasValue ? Intrinsic_Width.Value : 0;
        internal double Preferred_Height => Intrinsic_Height.HasValue ? Intrinsic_Height.Value : 0;

        internal double? Content_Width { get; private set; } = null;
        internal double? Content_Height { get; private set; } = null;


        internal double? Intrinsic_Width { get; private set; } = null;
        internal double? Intrinsic_Height { get; private set; } = null;
        /// <summary>
        /// The intrinsic ratio of Height/Width
        /// </summary>
        internal double? Intrinsic_Ratio { get; private set; } = null;

        /// <summary>
        /// The smallest size a box could take that doesn’t lead to overflow that could be avoided by choosing a larger size. (See §4 Intrinsic Size Determination.)
        /// </summary>
        internal Rect2f Min_Content = new Rect2f();
        /// <summary>
        /// The narrowest inline size a box could take that doesn’t lead to inline-dimension overflow that could be avoided by choosing a larger inline size. Roughly, the inline size that would fit around its contents if all soft wrap opportunities within the box were taken.
        /// </summary>
        internal Rect2f Min_Content_Inline = new Rect2f();
        /// <summary>
        /// In general, and definitely for block-level and inline-level boxes, this is equivalent to the max-content block size.
        /// </summary>
        internal Rect2f Min_Content_Block = new Rect2f();


        /// <summary>
        /// A box’s “ideal” size in a given axis when given infinite available space. Usually this is the smallest size the box could take in that axis while still fitting around its contents, i.e. minimizing unfilled space while avoiding overflow.
        /// </summary>
        internal Rect2f Max_Content = new Rect2f();
        /// <summary>
        /// The box’s “ideal” size in the inline axis. Usually the narrowest inline size it could take while fitting around its contents if none of the soft wrap opportunities within the box were taken. (See §4 Intrinsic Size Determination.)
        /// </summary>
        internal Rect2f Max_Content_Inline = new Rect2f();
        /// <summary>
        /// The box’s “ideal” size in the block axis. Usually the block size of the content after layout.
        /// </summary>
        internal Rect2f Max_Content_Block = new Rect2f();
        #endregion
        #endregion


        #region Constructors
        public CssPrincipalBox(in Element owner, in CssBoxTreeNode parent) : base(parent)
        {
            _owner = new WeakReference<Element>(owner);
        }

        ~CssPrincipalBox()
        {
            _owner = null;
        }
        #endregion

        #region Bounds Updating
        /// <summary>
        /// Alters the TRBL values of the <paramref name="Left"/> rect such that it fits AROUND the <paramref name="Right"/> rect with the given <paramref name="Left_Offsets"/>.
        /// </summary>
        /// <param name="Right"></param>
        /// <param name="Left"></param>
        /// <param name="Left_Offsets"></param>
        private void Fit_Rect_Around(Rect4f Left, Rect4f Right, in ReadOnlyRect4f Left_Offsets)
        {
            if (Left is null)
            {
                throw new ArgumentNullException(nameof(Left));
            }

            if (Right is null)
            {
                throw new ArgumentNullException(nameof(Right));
            }

            Contract.EndContractBlock();

            Left.Top = Right.Top - Left_Offsets.Top;
            Left.Right = Right.Right + Left_Offsets.Right;
            Left.Bottom = Right.Bottom + Left_Offsets.Bottom;
            Left.Left = Right.Left - Left_Offsets.Left;
        }

        /// <summary>
        /// Alters the TRBL values of the <paramref name="Left"/> rect such that it fits WITHIN the <paramref name="Right"/> rect with the given <paramref name="Right_Offsets"/>.
        /// </summary>
        /// <param name="Left"></param>
        /// <param name="Right"></param>
        /// <param name="Right_Offsets"></param>
        private void Fit_Rect_Within(Rect4f Left, Rect4f Right, in ReadOnlyRect4f Right_Offsets)
        {
            if (Left is null)
            {
                throw new ArgumentNullException(nameof(Left));
            }

            if (Right is null)
            {
                throw new ArgumentNullException(nameof(Right));
            }

            Contract.EndContractBlock();

            Left.Top = Right.Top + Right_Offsets.Top;
            Left.Right = Right.Right - Right_Offsets.Right;
            Left.Bottom = Right.Bottom - Right_Offsets.Bottom;
            Left.Left = Right.Left + Right_Offsets.Left;
        }

        /// <summary>
        /// Update the box's boundaries, areas, and edges
        /// </summary>
        /// <param name="X"></param>
        /// <param name="Y"></param>
        /// <param name="Width"></param>
        /// <param name="Height"></param>
        private void Update_Bounds()
        {
            var Padding_Size = Style.Get_Padding_Size();
            var Border_Size = Style.Get_Border_Size();
            var Margin_Size = Style.Get_Margin_Size();

            /* First if we are relatively positioned, set our margin edges up */
            switch (Style.Positioning)
            {
                case EBoxPositioning.Static:
                    {
                        Update_Area_Bounds(Style.Left, Style.Top);
                    }
                    break;
                case EBoxPositioning.Relative:
                    {
                        var off_x = Style.Left + Style.Right;
                        var off_y = Style.Top + Style.Bottom;

                        Update_Area_Bounds(off_x, off_y);
                    }
                    break;
                case EBoxPositioning.Absolute:
                    {/* For an absolute element all of the TRBL values will have a resolved value */
                        Margin = new Rect4f(Style.Top, Style.Right, Style.Bottom, Style.Left);
                        Fit_Rect_Within(Border, Margin, Margin_Size);
                        Fit_Rect_Within(Padding, Border, Border_Size);
                        Fit_Rect_Within(Content, Padding, Padding_Size);
                    }
                    break;
                case EBoxPositioning.Fixed:
                    {/* For a fixed element all of the TRBL values will have a resolved value */
                        Margin = new Rect4f(Style.Top, Style.Right, Style.Bottom, Style.Left);
                        Fit_Rect_Within(Border, Margin, Margin_Size);
                        Fit_Rect_Within(Padding, Border, Border_Size);
                        Fit_Rect_Within(Content, Padding, Padding_Size);
                    }
                    break;
                default:
                    {
                        throw new NotImplementedException($"Unhandled CSS positioning type: {Style.Positioning}");
                        //throw new NotImplementedException($"Unhandled CSS positioning type: {Enum.GetName(typeof(EPositioning), Style.Positioning)}");
                    }
            }

            Update_Replaced_Bounds();
        }

        /// <summary>
        /// Recalculates the coordinates for all sides of the Margin, Border, Padding, and Content areas of this box
        /// </summary>
        /// <param name="off_x"></param>
        /// <param name="off_y"></param>
        private void Update_Area_Bounds(double off_x, double off_y)
        {
            /* Content area */
            /*int cTop = off_y + Margin.Size_Top + Border.Size_Top + Padding.Size_Top;
            int cLeft = off_x + Margin.Size_Left + Border.Size_Left + Padding.Size_Left;*/
            var cTop = off_y + Style.Margin_Top + Style.Border_Top_Width + Style.Padding_Top;
            var cLeft = off_x + Style.Margin_Left + Style.Border_Left_Width + Style.Padding_Left;

            var cRight = cLeft + Style.Width;// - (Margin.Size_Right + Border.Size_Right + Padding.Size_Right);
            var cBottom = cTop + Style.Height;// - (Margin.Size_Bottom + Border.Size_Bottom + Padding.Size_Bottom);

            Content = new Rect4f(cTop, cRight, cBottom, cLeft);
            Fit_Rect_Around(Padding, Content, Style.Get_Padding_Size());
            Fit_Rect_Around(Border, Padding, Style.Get_Border_Size());
            Fit_Rect_Around(Margin, Border, Style.Get_Margin_Size());

            Debug.Assert((Content.Width ==  Style.Width));
            Debug.Assert((Content.Height ==  Style.Height));
        }

        /// <summary>
        /// Recalculates the coordinates for all sides of the replaced-content area
        /// </summary>
        private void Update_Replaced_Bounds()
        {
            if (!IsReplacedElement)
            {
                Replaced = null;
                return;
            }

            /* Recalculate object position */
            Style.Cascaded.ObjectPosition.UpdateDependent(true);
            var X = Style.ObjectPosition.X;
            var Y = Style.ObjectPosition.Y;
            // Style.Cascaded.ObjectPosition_X.UpdateDependent(true);
            // Style.Cascaded.ObjectPosition_Y.UpdateDependent(true);
            // var X = Style.ObjectPosition_X;
            // var Y = Style.ObjectPosition_Y;
            var cSize = Get_Replaced_Block_Size();

            /* Update replaced area */
            var rTop = Content.Top + Y;
            var rRight = Content.Left + X + cSize.Width;
            var rBottom = Content.Top + Y + cSize.Height;
            var rLeft = Content.Left + X;

            if (Replaced is null)
            {
                Replaced = new Rect4f(rTop, rRight, rBottom, rLeft);
            }
            else
            {
                Replaced.Top = rTop;
                Replaced.Right = rRight;
                Replaced.Bottom = rBottom;
                Replaced.Left = rLeft;
            }
        }
        #endregion

        #region Box Rebuilding
        /// <summary>
        /// Rebuilds this box's boundaries from the style information given
        /// </summary>
        /// <param name="Style"></param>
        public bool Rebuild(bool Force = false)
        {
            if (!IsDirty && !Force)
                return false;

            BoxModel.Resolve(this, Style.Cascaded);

            /* change detection */
            var oldPos = Position;// new Point2f(X, Y);
            var oldSize = Size;// new Rect2f(Width, Height);
            int oldReplaced = Replaced?.GetHashCode() ?? 0;
            int oldContent = Content.GetHashCode();
            int oldPadding = Padding.GetHashCode();
            int oldBorder = Border.GetHashCode();
            int oldMargin = Margin.GetHashCode();
            Update_Depends_Flag();

            // Figure out if we have any block-level children
            HasBlockLevelChildren = false;
            Element node = Owner.firstElementChild;
            while (node is object)
            {
                if (node.Box.DisplayType.Outer == EOuterDisplayType.Block)
                {
                    HasBlockLevelChildren = true;
                    break;
                }

                node = node.nextElementSibling;
            }

            /* Update our bounds */
            switch (DisplayGroup)
            {
                case EBoxDisplayGroup.INVALID:
                    {
                        Replaced = null;

                        Content = null;
                        Padding = null;
                        Border = null;
                        Margin = null;
                    }
                    break;
                default:
                    Update_Bounds();
                    break;
            }

            /* Reset dirt flags */
            Dirt = EBoxInvalidationReason.Clean;

            /* Check if this box has changed from the oustides perspective */
            var newPos = Position;//new Point2f(X, Y);
            var newSize = Size;// new Rect2f(Width, Height);

            bool HasChanged = false;
            bool eqReplaced = oldReplaced != (Replaced?.GetHashCode() ?? 0);
            bool eqContent = oldContent != Content.GetHashCode();
            bool eqPadding = oldPadding != Padding.GetHashCode();
            bool eqBorder = oldBorder != Border.GetHashCode();
            bool eqMargin = oldMargin != Margin.GetHashCode();

            HasChanged = oldPos != newPos || oldSize != newSize;
            HasChanged |= eqReplaced;
            HasChanged |= eqContent;
            HasChanged |= eqPadding;
            HasChanged |= eqBorder;
            HasChanged |= eqMargin;

            if (HasChanged)
            {
                // Owner.Invalidate_Layout(EBoxInvalidationReason.Block_Changed);

                ECssBoxType ChangeFlags = 0x0;
                if (eqReplaced) ChangeFlags |= ECssBoxType.Replaced;
                if (eqContent) ChangeFlags |= ECssBoxType.Content;
                if (eqPadding) ChangeFlags |= ECssBoxType.Padding;
                if (eqBorder) ChangeFlags |= ECssBoxType.Border;
                if (eqMargin) ChangeFlags |= ECssBoxType.Margin;

                onChange?.Invoke(ChangeFlags);
            }

            return true;
        }


        /// <summary>
        /// Checks if any of properties used to calculate the block will depend on our parent block
        /// </summary>
        private void Update_Depends_Flag()
        {
            DependsOnChildren = DisplayGroup == EBoxDisplayGroup.INLINE;
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
            Owner.Box.Flag(EBoxInvalidationReason.Layout_Pos_Changed);

            // SHOULD we be invalidating Flow(layout) whenever an element's layout pos changes?
            //Owner.Invalidate_Layout(EBoxInvalidationReason.Layout_Pos_Changed);
        }
        #endregion

        #region Content Sizing

        public void Set_Content_Size(int? Width, int? Height)
        {
            bool changed = Width != Content_Width || Height != Content_Height;
            if (!changed) return;

            Content_Width = Width;
            Content_Height = Height;
            Flag(EBoxInvalidationReason.Content_Changed);
        }

        public void Set_Content_Width(int? Width)
        {
            bool changed = Width != Content_Width;
            if (!changed) return;

            Content_Width = Width;

            Flag(EBoxInvalidationReason.Content_Changed);
        }

        public void Set_Content_Height(int? Height)
        {
            bool changed = Height != Content_Height;
            if (!changed) return;

            Content_Height = Height;
            Flag(EBoxInvalidationReason.Content_Changed);
        }
        #endregion

        #region Intrinsic Sizing
        public void Set_Intrinsic_Size(int? Width, int? Height)
        {
            bool changed = Width != Intrinsic_Width || Height != Intrinsic_Height;
            if (!changed) return;

            Intrinsic_Width = Width;
            Intrinsic_Height = Height;

            if (Width.HasValue && Height.HasValue)
                Intrinsic_Ratio = Height.Value / (float)Width.Value;
            else
                Intrinsic_Ratio = null;

            Flag(EBoxInvalidationReason.Content_Changed);
        }

        public void Set_Intrinsic_Width(int? Width)
        {
            bool changed = Width != Intrinsic_Width;
            if (!changed) return;

            Intrinsic_Width = Width;
            Flag(EBoxInvalidationReason.Content_Changed);
        }

        public void Set_Intrinsic_Height(int? Height)
        {
            bool changed = Height != Intrinsic_Height;
            if (!changed) return;

            Intrinsic_Height = Height;
            Flag(EBoxInvalidationReason.Content_Changed);
        }

        public void Set_Intrinsic_Ratio(float? Ratio)
        {
            bool changed = Ratio != Intrinsic_Ratio;
            if (!changed) return;

            Intrinsic_Ratio = Ratio;
            Flag(EBoxInvalidationReason.Content_Changed);
        }

        #endregion



        #region Sizing
        /// <summary>
        /// Uses the appropriate sizing rules to find the replaced block size
        /// </summary>
        /// <returns></returns>
        internal Rect2f Get_Replaced_Block_Size()
        {/* Docs: https://www.w3.org/TR/css3-images/#default-sizing */

            switch (Style.ObjectFit)
            {
                case EObjectFit.None:
                    return CssAlgorithms.Default_Sizing_Algorithm(this, CssValue.Null, CssValue.Null, Content.Width, Content.Height);
                case EObjectFit.Fill:
                    return new Rect2f(Content.Width, Content.Height);
                case EObjectFit.Contain:
                    return CssAlgorithms.Contain_Constraint_Algorithm(this, Content.Width, Content.Height);
                case EObjectFit.Cover:
                    return CssAlgorithms.Cover_Constraint_Algorithm(this, Content.Width, Content.Height);
                case EObjectFit.Scale_Down:
                    {
                        var sz = CssAlgorithms.Default_Sizing_Algorithm(this, CssValue.Null, CssValue.Null, Content.Width, Content.Height);
                        if (sz.Width < Content.Width || sz.Height < Content.Height)
                            return new Rect2f(Content.Width, Content.Height);
                        else
                            return sz;
                    }
                default:
                    throw new NotImplementedException($"Handling for object-fit value '{Enum.GetName(typeof(EObjectFit), Style.ObjectFit)}' is not implemented!");
            }
        }

        #endregion

        #region Hit Testing
        /// <summary>
        /// Returns <c>True</c> if the given x/y coordinates lie within the click area of this box
        /// </summary>
        public bool HitTest(double x, double y)
        {
            return Geometry.Intersects(ClickArea, x, y);
        }
        #endregion
    }
}
