//#define PRINT_BLOCK_CHANGE_SOURCE
using Logging;
using SixLabors.Fonts;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CssUI
{
    // UI elements will follow the W3C standard box model.
    // https://www.w3schools.com/css/css_boxmodel.asp

    /// <summary>
    /// The basis for all OpenGL based UI elements.
    /// <para>NOTE: All OpenGL UI elements inheriting from this base class makes the assumption that texturing is disabled by default when drawing.</para>
    /// </summary>
    public abstract class cssElement : IDisposable, UIEvents
    {
        /// <summary>
        /// Current UID tracker for UI elements
        /// </summary>
        static uint CUID = 0;
        #region Identity
        /// <summary>
        /// An ID number, unique among all UI elements, assigned to this element.
        /// </summary>
        public readonly uint UID;

        /// <summary>
        /// The DEFAULT type-name used when referencing this element from within a StyleSheet, this is set by all element classes which inherit from the uiElement type (they have to, so all of them)
        /// </summary>
        public abstract string Default_CSS_TypeName { get; }
        /// <summary>
        /// If not NULL then this type-name is used when referencing this element from within a StyleSheet
        /// </summary>
        string TypeName_Override = null;
        /// <summary>
        /// The type-name used when referencing this element from within a StyleSheet
        /// </summary>
        public string TypeName { get { return (TypeName_Override != null ? TypeName_Override : Default_CSS_TypeName); } }
        
        /// <summary>
        /// Allows us to resolve the fullpath just once.
        /// </summary>
        CacheableValue<string> Cached_FullPath = new CacheableValue<string>();

        protected LogModule Logs
        {
            get
            {
                if (Root != null) return Root.Logs;
                else return new LogModule("-");
            }
        }

        public string FullPath
        {
            get
            {
                return Cached_FullPath.Get(() => 
                {
                    string str = string.Empty;
                    if (Parent?.FullPath != null) str += Parent?.FullPath + "/";
                    str += (ID != null ? ID : TypeName);
                    return str;
                });
            }
        }

        public override string ToString()
        {
            return FullPath;
        }
        #endregion

        #region Flags
        private EElementFlags Flags = EElementFlags.Clickable | EElementFlags.DoubleClickable;// By default all elements will process click and doubleclick events

        /// <summary>
        /// Add flags to the element
        /// </summary>
        /// <param name="flags">Set of flags to add</param>
        public void Flags_Add(EElementFlags flags) { Flags = (Flags | flags); }
        /// <summary>
        /// Remove flags from the element
        /// </summary>
        /// <param name="flags">Set of flags to remove</param>
        public void Flags_Remove(EElementFlags flags) { Flags = (Flags ^ flags); }
        /// <summary>
        /// Check if element has a set of flags
        /// </summary>
        /// <param name="flags">Set of flags to check for</param>
        /// <returns>Flags present</returns>
        public bool HasFlags(EElementFlags flags) { return ((Flags & flags) != 0); }

        /// <summary>
        /// Whether or not this element can be dragged around by the user
        /// </summary>
        public bool IsDraggable
        {
            get { return HasFlags(EElementFlags.Draggable); }
            set
            {
                if (value == true) Flags_Add(EElementFlags.Draggable);
                else Flags_Remove(EElementFlags.Draggable);
            }
        }
        #endregion
        
        #region Attributes
        private Dictionary<string, object> Attributes = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);

        /// <summary>
        /// Attempts to set the value of a specified attribute, returning whether or not the operation succeeded
        /// </summary>
        /// <param name="Attrib">Name of the attribute to set</param>
        /// <param name="Value">Value to set</param>
        /// <returns>Success</returns>
        public bool Set_Attribute(string Attrib, object Value)
        {
            bool result = false;
            object old;
            if (Attributes.TryGetValue(Attrib, out old))
            {
                if (!object.Equals(old, Value))
                    result = true;
            }
            else
            {
                result = true;
            }
            Attributes[Attrib] = Value;

            if (result)
            {
                // TODO: recascade rules from the stylesheet and update our Style accordingly
            }
            return true;
        }

        /// <summary>
        /// Returns the value of a specified attribute
        /// </summary>
        /// <param name="Attrib">Name of the attribute to get</param>
        public object Get_Attribute(string Attrib)
        {
            object Value;
            if (Attributes.TryGetValue(Attrib, out Value))
            {
                return Value;
            }

            return null;
        }

        /// <summary>
        /// Returns the value of a specified attribute
        /// </summary>
        /// <param name="Attrib">Name of the attribute to get</param>
        public Ty Get_Attribute<Ty>(string Attrib)
        {
            object Value;
            if (Attributes.TryGetValue(Attrib, out Value))
            {
                return (Ty)Value;
            }

            return default(Ty);
        }

        /// <summary>
        /// Returns whether or not a specified attribute is present
        /// </summary>
        /// <param name="Attrib">Name of the attribute to check</param>
        public bool Has_Attribute(string Attrib)
        {
            return Attributes.ContainsKey(Attrib);
        }

        /// <summary>
        /// Attempts to remove a specified attribute, returning whether or not the operation succeeded
        /// </summary>
        /// <param name="Attrib">Name of the attribute to clear</param>
        /// <returns>Success</returns>
        public bool Clear_Attribute(string Attrib)
        {
            bool result = Attributes.Remove(Attrib);
            if (result)
            {// Attribute changed (was removed)
                // TODO: recascade rules from the stylesheet and update our Style accordingly
            }

            return result;
        }

        #endregion

        #region ID
        /// <summary>
        /// functionally identical to an elements "ID" in HTML, a UNIQUE identifier for the element
        /// </summary>
        public string ID { get { return Get_Attribute<string>("id"); } private set { Set_Attribute("id", value); } }
        #endregion

        #region Classes
        /// <summary>
        /// Adds a styling class to the element
        /// </summary>
        /// <returns>Success</returns>
        public bool Add_Class(string ClassName)
        {
            if (string.IsNullOrEmpty(ClassName)) return false;
            var Classes = new List<string>(Get_Attribute<string>("class")?.Split(' '));
            Classes.Add(ClassName);
            return Set_Attribute("class", string.Join(" ", Classes));
        }

        /// <summary>
        /// Removes a styling class from the element
        /// </summary>
        /// <returns>Success</returns>
        public bool Remove_Class(string ClassName)
        {
            if (string.IsNullOrEmpty(ClassName)) return false;
            var Classes = new List<string>(Get_Attribute<string>("class")?.Split(' '));
            Classes.Remove(ClassName);
            return Set_Attribute("class", string.Join(" ", Classes));
        }

        /// <summary>
        /// Returns whether the element is assigned the specified styling class
        /// </summary>
        /// <returns></returns>
        public bool Has_Class(string ClassName)
        {
            if (string.IsNullOrEmpty(ClassName)) return false;
            return new List<string>(Get_Attribute<string>("class")?.Split(' ')).Contains(ClassName);
        }
        #endregion

        #region Layout Position
        /// <summary>
        /// This elements position according to the parent elements layout
        /// </summary>
        private ePos LayoutPos = new ePos(0, 0);

        /// <summary>
        /// Sets the layout position for this element
        /// </summary>
        public void Set_Layout_Pos(int? X, int? Y)
        {
            LayoutPos.X = (X.HasValue ? X.Value : 0);
            LayoutPos.Y = (Y.HasValue ? Y.Value : 0);
            Flag_Block_Dirty();
        }
        #endregion

        #region Style
        public ElementPropertySystem Style { get; private set; }
        #endregion

        #region Display
        public DebugOpts Debug = new DebugOpts();
        public Viewport Viewport { get; protected set; } = null;
        
        /// <summary>
        /// Returns whether or not LayoutDirectors ignore this element, so it's position will not be altered by them.
        /// </summary>
        public bool Affects_Layout { get { return (Style.Display != EDisplayMode.NONE && Style.PositioningScheme != EPositioningScheme.Absolute); } }

        #endregion

        #region Overflow

        /// <summary>
        /// Returns True if this element can scroll it's contents on the X axis
        /// </summary>
        protected bool Can_Scroll_X { get { return (Style.Overflow_X == EOverflowMode.Scroll || Style.Overflow_X == EOverflowMode.Auto); } }
        /// <summary>
        /// Returns True if this element can scroll it's contents on the Y axis
        /// </summary>
        protected bool Can_Scroll_Y { get { return (Style.Overflow_Y == EOverflowMode.Scroll || Style.Overflow_Y == EOverflowMode.Auto); } }

        /// <summary>
        /// Returns whether the element can be scrolled, either programmatically or via user input
        /// </summary>
        protected bool IsScrollContainer
        {
            get
            {
                switch (Style.Overflow_X)
                {
                    case EOverflowMode.Visible:
                    case EOverflowMode.Clip:
                        return false;
                }

                switch (Style.Overflow_Y)
                {
                    case EOverflowMode.Visible:
                    case EOverflowMode.Clip:
                        return false;
                }

                return true;
            }
        }
        /// <summary>
        /// Returns True if content will be clipped to the <see cref="Block_Padding"/> area
        /// </summary>
        protected bool Has_ScrollClipping { get { return (Style.Overflow_X != EOverflowMode.Visible || Style.Overflow_Y != EOverflowMode.Visible); } }
        /// <summary>
        /// Returns True/False whether or not the element has any clipping bounds at all
        /// </summary>
        protected bool HasClipping { get { return (Block_Clipping != null); } }
        #endregion

        #region Events
        protected virtual void Handle_Display_Changed() { }
        /// <summary>
        /// Called each time the control is performing its layout logic.
        /// </summary>
        public event Action<cssElement> onLayout;
        /// <summary>
        /// Called after the control has performed its layout logic.
        /// </summary>
        public event Action<cssElement> onLayoutPost;
        /// <summary>
        /// Called each time the control's block size changes.
        /// </summary>
        public event Action<cssElement, eSize, eSize> onResized;
        /// <summary>
        /// Fires each time the controls Block position changes
        /// </summary>
        public event Action<cssElement, ePos, ePos> onMoved;
        
        /// <summary>
        /// Occurs after <see cref="Update_Cached_Blocks"/>
        /// </summary>
        protected virtual void Handle_Resized(eSize oldSize, eSize newSize)
        {
            if (Parent != null) Parent.Flag_Layout(ELayoutBit.Dirty_Children);
            if (dbg_size != null) dbg_size = cssTextElement.From_Text_String(string.Format("{0}x{1}", Block.Width, Block.Height), null);
            onResized?.Invoke(this, oldSize, newSize);
        }
        
        /// <summary>
        /// Occurs after <see cref="Update_Cached_Blocks"/>
        /// </summary>
        protected virtual void Handle_Moved(ePos oldPos, ePos newPos)
        {
            if (Parent != null) Parent.Flag_Layout(ELayoutBit.Dirty_Children);
            onMoved?.Invoke(this, oldPos, newPos);
        }

        protected virtual void Handle_Content_Block_Change()
        {
        }
        #endregion
        
                
        #region Aligners Position
        public uiAligner xAligner;
        public uiAligner yAligner;
        #endregion
        

        #region Content
        public virtual bool IsEmpty { get { return true; } }
        #endregion

        #region Blocks
        /// <summary>
        /// The margin-edge block for an element
        /// <para>‘margin-left’ + ‘border-left-width’ + ‘padding-left’ + ‘width’ + ‘padding-right’ + scrollbar width (if any) + ‘border-right-width’ + ‘margin-right’ = width of containing block</para>
        /// </summary>
        public eBlock Block = new eBlock();
        /// <summary>
        /// The block for an element extending up to the edge of it's borders
        /// <para>‘border-left-width’ + ‘padding-left’ + ‘width’ + ‘padding-right’ + scrollbar width (if any) + ‘border-right-width’ = width of border block</para>
        /// </summary>
        public eBlock Block_Border = new eBlock();
        /// <summary>
        /// Offset applied to the containing-block when calculating <see cref="Block_Content"/> to account for the presence of a scrollbar (if any)
        /// </summary>
        public readonly eBlockOffset Scrollbar_Offset = new eBlockOffset();//new OffsetProperty(null, StyleValue.Zero, StyleValue.Zero, StyleValue.Zero, StyleValue.Zero) { AutoResolve=true };
        /// <summary>
        /// The block for an element extending up to the edge of it's padding.
        /// An elements background is contained within this block.
        /// <para>‘padding-left’ + ‘width’ + ‘padding-right’ = width of padding block</para>
        /// </summary>
        public eBlock Block_Padding = new eBlock();
        /// <summary>
        /// The block for an element extending up to the edge of it's content-area
        /// </summary>
        public eBlock Block_Content = new eBlock();
        /// <summary>
        /// The block for an element that dictates the area it's content may be drawn in, or NULL if no clipping should be used
        /// </summary>
        public eBlock Block_Clipping = null;
        /// <summary>
        /// The block which represents the hitbox for mouse related input events
        /// </summary>
        public eBlock Block_ClickArea { get { return Block_Padding; } }
        /// <summary>
        /// The containing block of this element
        /// <para>If the control has an ancestor this will be said ancestors content-area block</para>
        /// <para>Otherwise, if the element is a root element, this should have the dimensions of the viewport</para>
        /// </summary>
        public virtual eBlock Block_Containing
        {
            get
            {
                switch (Style.Positioning)
                {
                    case EPositioning.Fixed:
                        return Viewport?.Block;
                    default:
                        return Parent?.Block_Content;
                }
            }
        }

        /// <summary>
        /// Notifys this element that its containing block has changed, meaning it's style should be set to dirty
        /// </summary>
        public virtual void Flag_Containing_Block_Dirty()
        {
            if (Style.Depends_On_ContainingBlock)
            {
                Flag_Block_Dirty(EUIInvalidationReason.Containing_Block_Changed);
                Style.Flag();
            }
        }

        /// <summary>
        /// Flags this elements block as dirty, meaning it needs to be updated when next convenient
        /// </summary>
        public virtual void Flag_Block_Dirty(EUIInvalidationReason Reason = EUIInvalidationReason.Unknown)
        {
            Block.Flag_Dirty();
#if DEBUG
            if (Debug.Log_Block_Changes)
            {
                if (Reason == EUIInvalidationReason.Unknown)
                    Logs.Info(Logging.XTERM.magenta("[Block Change] {0} (UNKNOWN SOURCE!)"), this);
                else
                    Logs.Info(Logging.XTERM.magenta("[Block Change] {0} {1}"), this, Enum.GetName(typeof(EUIInvalidationReason), Reason).ToUpper());
            }
#endif
        }
        /// <summary>
        /// Flags this elements block as dirty, meaning it needs to be updated when next convenient
        /// </summary>
        protected void Flag_Block_Dirty(IStyleProperty sender)
        {
            Block.Flag_Dirty();
#if DEBUG
            if (Debug.Log_Block_Changes) Logs.Info(Logging.XTERM.magenta("[Block Change] {0}.{1} => {2}"), this, sender.CssName, sender);
#endif
        }

        
        /// <summary>
        /// Updates the controls block boundaries
        /// </summary>
        public void Update_Block()
        {
            if (Block.IsLocked) return;
            var block = Peek_Block(true);
            bool CHANGED = Block.IsDirty;
            if (!CHANGED)
                CHANGED = (block != Block);// Last resort safety check

            if (CHANGED || Block.IsDirty)// A change occured
            {
                if (!Block.IsDirty)// Apparently our computed block and our current one dont match, but for some reason the block wasnt flagged as dirty?
                    Log.Warn("[{0}] Untracked block change, find the source and fix it! IsDependent: {1}", this, Style.Depends_On_ContainingBlock);

                bool was_moved = (Block.Get_Pos() != block.Get_Pos());
                bool was_resized = (Block.Get_Size() != block.Get_Size());
                ePos oPos = (was_moved? new ePos(block.Get_Pos()) : null), nPos = (was_moved ? new ePos(Block.Get_Pos()) : null);
                eSize oSize = (was_resized ? new eSize(block.Get_Size()) : null), nSize = (was_resized ? new eSize(Block.Get_Size()) : null);

                Block = block;
                Block.Flag_Clean();
                Update_Cached_Blocks();
                if (Block.IsDirty)
                {
                    Log.Warn("{0} Update_Cached_Blocks() Changed the block!", this);
                }
                // Moved to the Handle_Resized and Handle_Moved functions
                // if (was_moved || was_resized) Parent?.Flag_Layout_Dirty();
                Block.Flag_Clean();
#if DEBUG
                if (Debug.Log_Block_Changes) Logs.Info(Logging.XTERM.magentaBright("[Block Resolved] {0} <{1}>"), this, Block);
#endif

                // Handle resizing and moving events before handling the layout as they might further change it...
                if (was_moved) Handle_Moved(oPos, nPos);
                if (was_resized) Handle_Resized(oSize, nSize);

                Invalidate_Layout(EUIInvalidationReason.Block_Changed);
            }
        }

        /// <summary>
        /// Returns a newly calculated Block for the element without altering the one it has set currently
        /// </summary>
        public eBlock Peek_Block(bool Force=false)
        {
            if (!Force && !Block.IsDirty)
            {// Minor optimization
                return this.Block;
            }

            Style.Resolve();// 06-18-2017  WAS => Style.Resolve(this);
            if (Style.Display == EDisplayMode.NONE) return eBlock.FromTRBL(0, 0, 0, 0);

            ePos cPos = Style.Get_Offset() + Block_Containing.Get_Pos();
            eBlock block;
            switch (Style.BoxSizing)
            {// SEE: https://drafts.csswg.org/css-ui-3/#box-sizing
                case EBoxSizingMode.BORDER:
                    {
                        int W = Style.Width;
                        int H = Style.Height;
                        Block_Resize_Border_To_Margin(ref W, ref H);

                        block = new eBlock(cPos, new eSize(W, H));
                    }
                    break;
                case EBoxSizingMode.CONTENT:
                default:
                    {
                        int W = Style.Width;
                        int H = Style.Height;
                        Block_Resize_Content_To_Margin(ref W, ref H);

                        block = new eBlock(cPos, new eSize(W, H));
                    }
                    break;
            }
            
            return block;
        }

        /// <summary>
        /// Returns the GUARANTED size of the element with all available non-dependent values resolved, that is all of its padding, borders, and margins plus it's minimum size or its computed non-dependent absolute size.
        /// </summary>
        public eSize Get_Layout_Size()
        {
            eSize size = new eSize();
            if (Style.Display == EDisplayMode.NONE) return size;

            Block_Resize_Content_To_Margin_Only_Absolute(ref size.Width, ref size.Height);

            if (Style.Final.Width.Has_Flags(StyleValueFlags.Absolute)) size.Width += Style.Resolve_Size_Width(this, Style.Final.Width.Computed);
            else if (Style.Final.Min_Width.Has_Flags(StyleValueFlags.Absolute)) size.Width += Style.Resolve_MinSize_Width(this, Style.Final.Min_Width.Computed);

            if (Style.Final.Height.Has_Flags(StyleValueFlags.Absolute)) size.Height += Style.Resolve_Size_Height(this, Style.Final.Height.Computed);
            else if (Style.Final.Min_Height.Has_Flags(StyleValueFlags.Absolute)) size.Height += Style.Resolve_MinSize_Height(this, Style.Final.Min_Height.Computed);

            return size;
        }
        

        /// <summary>
        /// Updates the values for <see cref="Block_Content"/>, <see cref="Block_Padding"/>, <see cref="Block_Border"/>, and <see cref="Block_Margin"/>
        /// </summary>
        protected virtual void Update_Cached_Blocks()
        {
            Block_Border = Block_Margin_To_Border(Block);
            Block_Padding = Block_Margin_To_Padding(Block);

            var old = new eBlock(Block_Content);
            Block_Content = Block_Margin_To_Content(Block);
            if (old != Block_Content) Handle_Content_Block_Change();
        }

#region Block Calculations

        /// <summary>
        /// Returns a margin-edge block when given a location and content size for an element by adding to it
        /// the total Horizontal & Vertical sizes for all of the Padding, Margins, and Borders
        /// as well as the width of the scrollbar (if any)
        /// </summary>
        public eBlock Block_Content_To_Margin(int X, int Y, int Width, int Height)
        {
            //return block.Add(Padding.Value).Add(Scrollbar_Offset).Add(Border.Get_Offsets()).Add(Margin.Value);
            int Top = Y;
            int Right = ((X+Width) + Style.Padding_Horizontal + Scrollbar_Offset.Horizontal + (Border.Left.Size + Border.Right.Size) + Style.Margin_Horizontal);
            int Bottom = ((Y+Height) + Style.Padding_Vertical + Scrollbar_Offset.Vertical + (Border.Top.Size + Border.Bottom.Size) + Style.Margin_Vertical);
            int Left = X;
            return eBlock.FromTRBL(Top, Right, Bottom, Left);
        }
        /// <summary>
        /// Returns the margin-edge block size when given a border-block size for an element by adding to it
        /// the total Horizontal & Vertical sizes for the Margins
        /// </summary>
        public void Block_Resize_Border_To_Margin(ref int Width, ref int Height)
        {
            Width += Style.Margin_Horizontal;
            Height += Style.Margin_Vertical;
        }
        /// <summary>
        /// Returns the margin-edge block size when given a padding-block size for an element by adding to it
        /// the total Horizontal & Vertical sizes for all of the Borders and Margins aswell as the width of the scrollbar(if any)
        /// </summary>
        public void Block_Resize_Padding_To_Margin(ref int Width, ref int Height)
        {
            Width += (Scrollbar_Offset.Horizontal + (Border.Left.Size + Border.Right.Size) + Style.Margin_Horizontal);
            Height += (Scrollbar_Offset.Vertical + (Border.Left.Size + Border.Right.Size) + Style.Margin_Vertical);
        }
        /// <summary>
        /// Returns the margin-edge block size when given a content-block size for an element by adding to the given Width/Height
        /// the total Horizontal & Vertical sizes for all of the Padding, Borders, and Margins aswell as the width of the scrollbar(if any)
        /// </summary>
        public void Block_Resize_Content_To_Margin(ref int Width, ref int Height)
        {
            Width += (Style.Padding_Horizontal + Scrollbar_Offset.Horizontal + (Border.Left.Size + Border.Right.Size) + Style.Margin_Horizontal);
            Height += (Style.Padding_Vertical + Scrollbar_Offset.Vertical + (Border.Left.Size + Border.Right.Size) + Style.Margin_Vertical);
        }

        /// <summary>
        /// Returns the margin-edge block size when given a content-block size for an element by adding to the given Width/Height
        /// the total Horizontal & Vertical sizes for all of the Padding, Borders, and Margins aswell as the width of the scrollbar(if any)
        /// <para>In addition any 'auto' values for Padding, Borders, and Margins are replaced with the given default value</para>
        /// </summary>
        //TODO: Scrap this, it wont be needed soon.
        public void Block_Resize_Content_To_Margin_NoAuto(ref int Width, ref int Height, int autoValue)
        {
            eBlockOffset margin = Style.Get_Margin_NoAuto(autoValue);
            Width += (Style.Padding_Horizontal + Scrollbar_Offset.Horizontal + (Border.Left.Size + Border.Right.Size) + margin.Horizontal);
            Height += (Style.Padding_Vertical + Scrollbar_Offset.Vertical + (Border.Left.Size + Border.Right.Size) + margin.Vertical);
        }

        /// <summary>
        /// Returns the margin-edge block size when given a content-block size for an element by adding to the given Width/Height
        /// the total Horizontal & Vertical sizes for all of the Padding, Borders, and Margins aswell as the width of the scrollbar(if any)
        /// <para>In addition only values with absolute definitions are used, meaning no percentage or Auto values</para>
        /// </summary>
        public void Block_Resize_Content_To_Margin_Only_Absolute(ref int Width, ref int Height)
        {
            eBlockOffset margin = Style.Get_Margin_OnlyAbsolute(0);
            eBlockOffset padding = Style.Get_Padding_OnlyAbsolute(0);

            Width += (padding.Horizontal + Scrollbar_Offset.Horizontal + (Border.Left.Size + Border.Right.Size) + margin.Horizontal);
            Height += (padding.Vertical + Scrollbar_Offset.Vertical + (Border.Left.Size + Border.Right.Size) + margin.Vertical);
        }

        /// <summary>
        /// Returns the content-edge block size when given a margin-block size for an element by subtracting from the given Width/Height
        /// the total Horizontal & Vertical sizes for all of the Padding, Borders, and Margins aswell as the width of the scrollbar(if any)
        /// </summary>
        public void Block_Resize_Margin_To_Content(ref int Width, ref int Height)
        {
            Width -= (Style.Padding_Horizontal + Scrollbar_Offset.Horizontal + (Border.Left.Size + Border.Right.Size) + Style.Margin_Horizontal);
            Height -= (Style.Padding_Vertical + Scrollbar_Offset.Vertical + (Border.Left.Size + Border.Right.Size) + Style.Margin_Vertical);
        }

        /// <summary>
        /// Returns the content-edge block size when given a border-block size for an element by subtracting from the given Width/Height
        /// the total Horizontal & Vertical sizes for all of the Padding, Borders, and Margins aswell as the width of the scrollbar(if any)
        /// </summary>
        public void Block_Resize_Border_To_Content(ref int Width, ref int Height)
        {
            Width -= (Style.Padding_Horizontal + Scrollbar_Offset.Horizontal + (Border.Left.Size + Border.Right.Size));
            Height -= (Style.Padding_Vertical + Scrollbar_Offset.Vertical + (Border.Left.Size + Border.Right.Size));
        }

        /// <summary>
        /// Returns the border-edge block size when given a content-block size for an element by adding to the given Width/Height
        /// the total Horizontal & Vertical sizes for all of the Padding, Borders, and Margins aswell as the width of the scrollbar(if any)
        /// </summary>
        public void Block_Resize_Content_To_Border(ref int Width, ref int Height)
        {
            Width += (Style.Padding_Horizontal + Scrollbar_Offset.Horizontal + (Border.Left.Size + Border.Right.Size));
            Height += (Style.Padding_Vertical + Scrollbar_Offset.Vertical + (Border.Left.Size + Border.Right.Size));
        }

        /// <summary>
        /// Returns the content-edge block size when given a padding-block size for an element by adding to the given Width/Height
        /// the total Horizontal & Vertical sizes for all of the Padding and Scrollbar offsets(if any)
        /// </summary>
        public void Block_Resize_Padding_To_Content(ref int Width, ref int Height)
        {
            Width -= (Style.Padding_Horizontal + Scrollbar_Offset.Horizontal);
            Height -= (Style.Padding_Vertical + Scrollbar_Offset.Vertical);
        }

        /// <summary>
        /// Collapses a given margin block so it represents the content-area block for a control.
        /// </summary>
        public eBlock Block_Margin_To_Content(eBlock b)
        {
            //return (((block - Padding.Value) - Scrollbar_Offset - Border.Get_Offsets()) - Margin.Value);
            int Top = (b.Top + Style.Padding_Top + Scrollbar_Offset.Top + Border.Top.Size + Style.Margin_Top);
            int Right = (b.Right - Style.Padding_Right - Scrollbar_Offset.Right - Border.Right.Size - Style.Margin_Right);
            int Bottom = (b.Bottom - Style.Padding_Bottom - Scrollbar_Offset.Bottom - Border.Bottom.Size - Style.Margin_Bottom);
            int Left = (b.Left + Style.Padding_Left + Scrollbar_Offset.Left + Border.Left.Size + Style.Margin_Left);
            return eBlock.FromTRBL(Top, Right, Bottom, Left);
        }
        /// <summary>
        /// Collapses a given margin-block so it represents the padding-area block for a control.
        /// </summary>
        public eBlock Block_Margin_To_Padding(eBlock b)
        {
            //return ((block - Scrollbar_Offset - Border.Get_Offsets()) - Margin.Value);
            int Top = (b.Top + Scrollbar_Offset.Top + Border.Top.Size + Style.Margin_Top);
            int Right = (b.Right - Scrollbar_Offset.Right - Border.Right.Size - Style.Margin_Right);
            int Bottom = (b.Bottom - Scrollbar_Offset.Bottom - Border.Bottom.Size - Style.Margin_Bottom);
            int Left = (b.Left + Scrollbar_Offset.Left + Border.Left.Size + Style.Margin_Left);
            return eBlock.FromTRBL(Top, Right, Bottom, Left);
        }
        /// <summary>
        /// Collapses a given containing block so it represents the border-area block for a control.
        /// </summary>
        public eBlock Block_Margin_To_Border(eBlock b)
        {
            //return (block - Margin.Value);
            int Top = (b.Top + Style.Margin_Top);
            int Right = (b.Right - Style.Margin_Right);
            int Bottom = (b.Bottom - Style.Margin_Bottom);
            int Left = (b.Left + Style.Margin_Left);
            return eBlock.FromTRBL(Top, Right, Bottom, Left);
        }
#endregion

#endregion

#region Setters
        /// <summary>
        /// Sets the explicit Width and Height properties for this elements default style state
        /// </summary>
        public void Set_Size(int? Width, int? Height)
        {
            Style.User.Width.Set(Width);
            Style.User.Height.Set(Height);
        }

        /// <summary>
        /// Sets the explicit X(Left) and Y(Top) properties for this elements default style state
        /// </summary>
        public void Set_Pos(int? X, int? Y)
        {
            Style.User.Left.Set(X);
            Style.User.Top.Set(Y);
        }

        /// <summary>
        /// Sets the explicit Top, Right, Bottom, and Left margin properties for this elements default style state
        /// </summary>
        public void Set_Margin(int? T, int? R, int? B, int? L)
        {
            Style.User.Margin_Top.Set(T);
            Style.User.Margin_Right.Set(R);
            Style.User.Margin_Bottom.Set(B);
            Style.User.Margin_Left.Set(L);
        }

        /// <summary>
        /// Sets the explicit Top, Right, Bottom, and Left padding properties for this elements default style state
        /// </summary>
        public void Set_Padding(int? T, int? R, int? B, int? L)
        {
            Style.User.Padding_Top.Set(T);
            Style.User.Padding_Right.Set(R);
            Style.User.Padding_Bottom.Set(B);
            Style.User.Padding_Left.Set(L);
        }
#endregion

#region Layout
        /// <summary>
        /// Specifys something that affects the controls boundaries has changed, be it the pos, size, border, contents, etc.
        /// </summary>
        protected ELayoutBit LayoutBit = ELayoutBit.Dirty;
        public void Flag_Layout(ELayoutBit Flags) { LayoutBit |= Flags; }

        public void Invalidate_Layout(EUIInvalidationReason Reason = EUIInvalidationReason.Unknown)
        {
            Flag_Layout(ELayoutBit.Dirty);
#if DEBUG
            if (Debug.Log_Layout_Changes)
            {
                if (Reason == EUIInvalidationReason.Unknown)
                    Logs.Info(Logging.XTERM.cyan("[Layout Flagged] {0} (UNKNOWN SOURCE!)"), this);
                else
                    Logs.Info(Logging.XTERM.cyan("[Layout Flagged] {0} {1}"), this, Enum.GetName(typeof(EUIInvalidationReason), Reason).ToUpper());
                     
            }
#endif
        }

        public void Invalidate_Layout(IStyleProperty sender)
        {
            Flag_Layout(ELayoutBit.Dirty);
#if DEBUG
            if (Debug.Log_Layout_Changes) Logs.Info(Logging.XTERM.cyan("[Layout Flagged] {0}.{1} => {2}"), this, sender.CssName, sender);
#endif
        }

        /// <summary>
        /// Forces the element to apply layout logic to all of it's child elements.
        /// </summary>
        public void PerformLayout()
        {
            Style.Resolve();
            //Guid TMR = Timing.Start("PerformLayout()");
            // We still need to apply the positioners ourselves here just incase they arent linked to a target control and are a "relative" positioner.
            xAligner?.Apply_Relative();
            yAligner?.Apply_Relative();
            
            if (Block_Containing.X != last_containerPos.X || Block_Containing.Y != last_containerPos.Y)
            {
                last_containerPos.X = Block_Containing.X;
                last_containerPos.Y = Block_Containing.Y;
            }

            // If our block is dirty then we need to update it NOW before we update the layout and give useless block positions to our child-elements
            Update_Block();
            int cycles = 0;
            const int MAX_LAYOUT_CYCLES = 100;
            do
            {
                LayoutBit = ELayoutBit.Clean;
                Handle_Layout();
            }
            while (LayoutBit!=ELayoutBit.Clean && ++cycles < MAX_LAYOUT_CYCLES);
            if (cycles >= MAX_LAYOUT_CYCLES) throw new Exception(string.Format("Aborted Handle_Layout() cycle loop after {0} passes!", cycles));

            onLayout?.Invoke(this);
            if (Block.IsDirty) Update_Block();
            
            onLayoutPost?.Invoke(this);

#if DEBUG
            if (Debug.Log_Layout_Changes) Logs.Info(Logging.XTERM.cyanBright("[Layout Resolved] {0}"), this);
#endif
            //Timing.Stop(TMR);
        }

        /// <summary>
        /// Does actual layout logic
        /// </summary>
        protected virtual void Handle_Layout() { }
#endregion
                
#region Parent
        /// <summary>
        /// The element that this one resides within, or NULL if this element is independent
        /// </summary>
        public cssCompoundElement Parent { get; private set; } = null;
        /// <summary>
        /// The elements indice within it's parent, or 0
        /// </summary>
        public int Indice { get; protected set; } = 0;

        public void Set_Parent(cssCompoundElement parent, int index)
        {
            Parent = parent;
            Indice = (parent!=null ? index : 0);
            if (parent != null) Set_Root(parent.Root);// Passed down from generation to generation!
            Element_Hierarchy_Changed(0);
            Flag_Containing_Block_Dirty();// Our parent element has been changed, so logically our containing-block is not different from what it was.
        }

        private ePos last_containerPos = new ePos();
        public bool isChild { get { return (Parent != null); } }
        public bool IsChildOf(cssCompoundElement Element)
        {
            cssElement E = this.Parent;
            while (E != null)
            {
                if (object.ReferenceEquals(E, Element)) return true;
            }
            return false;
        }

        /// <summary>
        /// Called whenever the hierarchy an element resides in changes.
        /// </summary>
        /// <param name="Depth">Distance of the current element to the one from which the hierachy change originated</param>
        public virtual void Element_Hierarchy_Changed(int Depth)
        {
            // Clear the cached string value for the elements FullPath
            Cached_FullPath.Clear();
            // Invalidate the elements layout
            //Invalidate_Layout();
            if (Depth == 0)// We are the originator and must invalidate our own block.
            {
                // Invalidate the elements block
                //Flag_Block_Dirty();
            }
        }
#endregion

#region Scrolling
        
        /// <summary>
        /// Coordinate offsets for this elements own scroll values.
        /// </summary>
        protected readonly ePos Scroll = new ePos(0, 0);
        /// <summary>
        /// Returns the total scroll translations for this element, including translations due to it's own scrolling
        /// </summary>
        public virtual ePos Get_Scroll_Total()
        {
            ePos Total = new ePos();
            cssElement E = this;
            do
            {
                Total += E.Scroll;
                if (E.Style.IsAbsolutelyPositioned) break;
                E = E.Parent;// Traverse upwards
            }
            while (E != null);

            return Total;
        }

#endregion

#region Root

        /// <summary>
        /// The Root UI Element
        /// </summary>
        public cssRootElement Root { get; private set; } = null;
        public virtual void Set_Root(cssRootElement root)
        {
            if (Root != null) Root.Unregister_Element(this);

            Root = root;

            if (Root != null)
            {
                Viewport = Root.Viewport;
                if (!string.IsNullOrEmpty(ID)) Root.Register_Element_ID(ID, this);
            }
        }
#endregion
        
#region Font
        protected bool Dirty_Font = false;
        Font font = null;
        public Font Font
        {
            get
            {
                if (font == null) return Parent?.Font;
                return font;
            }
            set
            {
                font = value;
                Style.User.FontFamily.Set(font.Family.Name);
                Style.User.FontSize.Set((double?)font.Size);

                if(font.Italic)
                {
                    Style.User.FontStyle.Set(EFontStyle.Italic);
                }
                else
                {
                    Style.User.FontStyle.Set(EFontStyle.Normal);
                }
            }
        }

        void Update_Font()
        {
            font = null;

            string fontFamily = Style.FontFamily;
            if (fontFamily == null) return;

            double fontSize = Style.FontSize;
            if (fontSize <= 0) return;

            var fontStyle = FontStyle.Regular;
            switch (Style.FontStyle)
            {
                case EFontStyle.Normal:
                    fontStyle = FontStyle.Regular;
                    break;
                case EFontStyle.Italic:
                case EFontStyle.Oblique:
                    fontStyle = FontStyle.Italic;
                    break;
            }

            if (Style.FontWeight >= 600) fontStyle = FontStyle.Bold;

            font = SystemFonts.CreateFont(fontFamily, (float)fontSize, fontStyle);

            Dirty_Font = false;
        }
#endregion

#region Colors
        /// <summary>
        /// Forecolor of the control
        /// </summary>
        public cssColor Color = cssColor.White;
        /// <summary>
        /// Background color of the control, if <c>Null</c> then no background will be drawn
        /// </summary>
        public cssColor ColorBackground = null;
#endregion

#region Borders
        /// <summary>
        /// Styling information for the controls borders
        /// </summary>
        public uiBorderStyle Border = uiBorderStyle.Default;
#endregion
        
#region PSEUDO STATES
        /// <summary>
        /// If FALSE the element will not respond to input events
        /// </summary>
        public bool IsEnabled
        {
            get { return (Has_Attribute("enabled") && (bool)Get_Attribute("enabled")); }
            set { Set_Attribute("enabled", value); }
        }// = true;

        /// <summary>
        /// Indicates whether the mouse is currently overtop the element
        /// </summary>
        public bool IsMouseOver
        {
            get { return (Has_Attribute("hovered") && (bool)Get_Attribute("hovered")); }
            protected set { Set_Attribute("hovered", value); }
        }// = false;

        /// <summary>
        /// Indicates whether the primary mouse button is currently pressed on the element
        /// </summary>
        public bool IsActive
        {
            get { return (Has_Attribute("active") && (bool)Get_Attribute("active")); }
            protected set { Set_Attribute("active", value); }
        }// = false;

        /// <summary>
        /// Indicates whether the element can accept drag-drop operations
        /// </summary>
        public bool AcceptsDragDrop
        {
            get { return (Has_Attribute("dropzone") && (bool)Get_Attribute("dropzone")); }
            protected set { Set_Attribute("dropzone", value); }
        }// = false;


        /// <summary>
        /// Returns all of the drag-drop object types that this element can accept
        /// </summary>
        public string[] AcceptedDropTypes
        {
            get
            {
                if (Has_Attribute("dropzone"))
                {
                    string[] spl = Get_Attribute<string>("dropzone")?.Replace(" ", "").Split(',');
                    if (spl == null) spl = new string[] { "*" };
                    return spl;
                }
                return new string[] { "*" };
            }
            protected set { Set_Attribute("dropzone", string.Join(",", value)); }
        }

        /// <summary>
        /// Returns whether the element is the current drop-target of a drag-drop operation
        /// </summary>
        public bool IsDropTarget { get { return (Root.Mouse.IsDragging && IsMouseOver); } }
        internal bool Accepts_Current_DragItem()
        { 
            if (!Root.Mouse.IsDragging) return false;

            return (AcceptsDragDrop);
        }
        
#endregion

#region Constructors
        /// <summary>
        /// Initializes a new UI element
        /// </summary>
        /// <param name="Name">A unique ID or classname, ID's must be prefixed with '#' or else the string is considered a classname</param>
        public cssElement(string Name)
        {
            UID = ++CUID;
            if (this is cssRootElement) Root = (cssRootElement)this;
            if (!string.IsNullOrEmpty(Name))
            {
                if (Name.StartsWith("#"))
                {
                    if (Name.Length > 1) this.ID = Name.Remove(0, 1);
                }
                else TypeName_Override = Name;
            }
            // Set default pseudo-state values
            IsEnabled = true;
            IsActive = false;
            IsMouseOver = false;
            AcceptsDragDrop = false;

            Scrollbar_Offset.onChanged += () => { Flag_Block_Dirty(EUIInvalidationReason.Scroll_Offset_Change); };

            Style = new ElementPropertySystem(this);
            Style.Property_Changed += Style_Property_Changed;


            // TESTING
            //var A = new CSS.CssSelector("#Root");
            //var B = new CSS.CssSelector("*:hover");
            //var C = new CSS.CssSelector("*:not(#Root)");
        }
#endregion

#region Property Change Handlers

        private void Style_Property_Changed(IStyleProperty Sender, EPropertyFlags Flags, System.Diagnostics.StackTrace Source)
        {

#if DEBUG
            if (Debug.Log_Property_Changes)
            {
                if (Sender != null)
                {
                    if (Flags.HasFlag(EPropertyFlags.Visual) || Flags.HasFlag(EPropertyFlags.Font))
                    {
                        Logs.Info("[Property Change] {0}.{1} => {2}", this.FullPath, Sender.CssName, Sender);
                    }
                }
            }
#endif

            if ((Flags & EPropertyFlags.Visual) != 0)
            {
                Dirty_Visuals = true;
            }

            if ((Flags & EPropertyFlags.Block) != 0)
            {
                if (Sender == null) Flag_Block_Dirty();
                else Flag_Block_Dirty(Sender);
            }

            if ((Flags & EPropertyFlags.Font) != 0)
            {
                Dirty_Font = true;
                // Q: Why update the font immediately? Why not wait until next frame when our parent will do it?
                // A: When a font property changes it means the user is most likely about to change other properties which depend on the font's current values (character dimensions and whatnot)
                //   In the future immediate font updates can be avoided because the system will naturally handle propogating updates to properties expressed in font based units(ex/ex/ch)
                Update_Font();
            }

            if ((Flags & EPropertyFlags.Flow) != 0)
            {
                if (Sender == null) Invalidate_Layout();
                else Invalidate_Layout(Sender);
            }
        }
#endregion

#region Destructors
        ~cssElement()
        {
            if (Root != null) Root.Unregister_Element(this);
        }
        public virtual void Dispose() { }
#endregion

#region Updating
        /// <summary>
        /// Updates the Block and Layout if needed and returns True if any updates occured
        /// </summary>
        /// <returns>True/False updates occured</returns>
        public virtual bool Update()
        {
            bool retVal = false;

            if (Dirty_Font)
            {
                retVal = true;
                Update_Font();
            }

            if (Block.IsDirty)
            {
                retVal = true;
                Update_Block();
            }

            if (LayoutBit > 0)
            {
                retVal = true;
                PerformLayout();
            }

            return retVal;
        }
#endregion

#region Drawing
        /// <summary>
        /// If FALSE the element will not draw
        /// </summary>
        public bool IsVisible = true;
        /// <summary>
        /// Returns True/False whether or not this ui element should render itself
        /// </summary>
        public bool ShouldRender { get { return (IsVisible && Style.Display != EDisplayMode.NONE); } }


        /// <summary>
        /// Do our visuals need to be updated? (if this element renders to an image buffer instead of directly to the screen)
        /// </summary>
        public bool Dirty_Visuals { get; protected set; } = true;
        /// <summary>
        /// Attempts to render the element
        /// </summary>
        /// <returns>False if control should not be drawn</returns>
        public virtual bool Render(bool force=false)
        {
            if (!force && !ShouldRender) return false;
            Root.Engine.Push();
            if (Style.Blend_Color != null) Root.Engine.Set_Blending_Color(Style.Blend_Color);
            if (Style.TransformMatrix != null) Root.Engine.Set_Matrix(Style.TransformMatrix);
            //Root.Engine.Set_Blend(new uiColor(1, 1, 1, 0.5));

            Dirty_Visuals = false;
            Draw_Background();
            Draw_Borders();
            // ReplacedElements always clip their rendered content to the area of their content block
            //if (Block_Clipping != null) Viewport.Push_Scissor(Block_Clipping);
            Draw();// Call the overridable drawing logic
            //if (Block_Clipping != null) Viewport.Pop_Scissor();
            Root.Engine.Pop();

            if (Debug.Draw_Bounds) Draw_Debug_Bounds();
            if (Debug.Draw_Size) Draw_Debug_Size();

            return true;
        }

        /// <summary>
        /// Draws the actual visuals of the control, can be overriden for different control types.
        /// </summary>
        /// <returns></returns>
        protected virtual void Draw()
        {
        }

        protected virtual void Draw_Background()
        {
            // Draw the background
            if (ColorBackground != null)
            {
                Root.Engine.Set_Color(ColorBackground);
                Root.Engine.Fill_Rect(Block_Padding);
            }
        }

        internal void Draw_Borders()
        {
            //if (Border.Dirty) Border.Prepare_Texture(Block_Border.Get_Size());

            // The CSS3 docs dont specify how borders are to be joined at the corners.
            // But chromium joins adjacent corners by splitting the corner quad diagonally from its outtermost to its innermost corner and giving each border the closes triangle
            // Which is pretty fair so we'll do that too!
            
            // Draw the horizontal bars without join areas
            int hLeft = this.Block_Border.Left + Border.Left.Size;// Starting point for horizontal border sides            
            int hWidth = Block_Border.Width - (Border.Left.Size + Border.Right.Size);// Width for horizontal border sides
            if (Border.Top.IsVisible)
            {
                Root.Engine.Set_Color(Border.Top.Color);
                Root.Engine.Set_Color(cssColor.White);
                Root.Engine.Fill_Rect(hLeft, Block_Border.Top, hWidth, Border.Top.Size);
            }
            if (Border.Bottom.IsVisible)
            {
                Root.Engine.Set_Color(Border.Bottom.Color);
                Root.Engine.Fill_Rect(hLeft, Block_Border.Bottom-Border.Bottom.Size, hWidth, Border.Bottom.Size);
            }
            // Draw the vertical bars without join areas
            int vTop = Block_Border.Top + Border.Top.Size;// Starting point for vertical border sides
            int vHeight = Block_Border.Height - (Border.Top.Size + Border.Bottom.Size);// Height for vertical border sides
            if (Border.Left.IsVisible)
            {
                Root.Engine.Set_Color(Border.Left.Color);
                Root.Engine.Fill_Rect(Block_Border.Left, vTop, Border.Left.Size, vHeight);
            }
            if (Border.Right.IsVisible)
            {
                Root.Engine.Set_Color(Border.Right.Color);
                Root.Engine.Fill_Rect(Block_Border.Right - Border.Right.Size, vTop, Border.Right.Size, vHeight);
            }
            
            // Draw the join areas
            if (Border.Top.IsVisible && Border.Left.IsVisible)
            {
                int xo = Block_Border.Left;// x origin
                int yo = Block_Border.Top;// y origin
                int xi = Block_Border.Left + Border.Left.Size;// x inner
                int yi = Block_Border.Top + Border.Top.Size;// y inner

                // Top Triangle
                Root.Engine.Set_Color(Border.Top.Color);
                Root.Engine.Fill_Tri(xi, yi, xi, yo, xo, yo);
                // Left Triangle
                Root.Engine.Set_Color(Border.Left.Color);
                Root.Engine.Fill_Tri(xi, yi, xo, yi, xo, yo);
            }

            if (Border.Top.IsVisible && Border.Right.IsVisible)
            {
                int xo = Block_Border.Right;// x origin
                int yo = Block_Border.Top;// y origin
                int xi = Block_Border.Right - Border.Right.Size;// x inner
                int yi = Block_Border.Top + Border.Top.Size;// y inner

                // Top Triangle
                Root.Engine.Set_Color(Border.Top.Color);
                Root.Engine.Fill_Tri(xi, yi, xi, yo, xo, yo);
                // Right Triangle
                Root.Engine.Set_Color(Border.Right.Color);
                Root.Engine.Fill_Tri(xi, yi, xo, yi, xo, yo);
            }

            if (Border.Bottom.IsVisible && Border.Left.IsVisible)
            {
                int xo = Block_Border.Left;// x origin
                int yo = Block_Border.Bottom;// y origin
                int xi = Block_Border.Left + Border.Left.Size;// x inner
                int yi = Block_Border.Bottom - Border.Bottom.Size;// y inner

                // Bottom Triangle
                Root.Engine.Set_Color(Border.Bottom.Color);
                Root.Engine.Fill_Tri(xi, yi, xi, yo, xo, yo);
                // Left Triangle
                Root.Engine.Set_Color(Border.Left.Color);
                Root.Engine.Fill_Tri(xi, yi, xo, yi, xo, yo);
            }

            if (Border.Bottom.IsVisible && Border.Right.IsVisible)
            {
                int xo = Block_Border.Right;// x origin
                int yo = Block_Border.Bottom;// y origin
                int xi = Block_Border.Right - Border.Right.Size;// x inner
                int yi = Block_Border.Bottom - Border.Bottom.Size;// y inner

                // Bottom Triangle
                Root.Engine.Set_Color(Border.Bottom.Color);
                Root.Engine.Fill_Tri(xi, yi, xi, yo, xo, yo);
                // Right Triangle
                Root.Engine.Set_Color(Border.Right.Color);
                Root.Engine.Fill_Tri(xi, yi, xo, yi, xo, yo);
            }
        }

        private cssTexture dbg_size = null;
        internal void Draw_Debug_Size()
        {
            if (dbg_size == null) dbg_size = cssTextElement.From_Text_String(string.Format("{0}x{1}", Block.Width, Block.Height), null);

            Root.Engine.Set_Color(1f, 1f, 1f, 1f);
            Root.Engine.Set_Texture(dbg_size);
            Root.Engine.Fill_Rect((Block.X + (Block.Width / 2)) - dbg_size.Size.Width, Block.Y, dbg_size.Size.Width, dbg_size.Size.Height);
            Root.Engine.Set_Texture(null);
        }

        internal virtual void Draw_Debug_Bounds()
        {            
            // Draw the content bounds
            Root.Engine.Set_Color(1f, 0f, 0f, 1f);// Red
            Root.Engine.Draw_Rect(1, Block_Content);

            // Draw the padding bounds
            Root.Engine.Set_Color(0f, 0f, 1f, 1f);// Blue
            Root.Engine.Draw_Rect(1, Block_Padding);

            // Draw the border bounds
            Root.Engine.Set_Color(0f, 1f, 0f, 1f);// Green
            Root.Engine.Draw_Rect(1, Block_Border);
            
            // Draw the block bounds
            Root.Engine.Set_Color(1f, 0.5f, 0f, 1f);// Orange            
            Root.Engine.Draw_Rect(1, Block);
        }
        
#endregion

#region Point Transformation

        /// <summary>
        /// Transforms a point from absolute(screen) space to the elements local space relative to it's margin-block.
        /// </summary>
        public ePos PointToLocal(ePos Point)
        {
            return (Point - Block.Get_Pos());
        }

        /// <summary>
        /// Transforms a point from local space relative to it's margin-block to absolute screen-space.
        /// </summary>
        public ePos PointToScreen(ePos Point)
        {
            return (Point + Block.Get_Pos());
        }

        /// <summary>
        /// Transforms a point from absolute(screen) space into local space relative to the specified block.
        /// </summary>
        public ePos PointToLocal(eBlock block, ePos Point)
        {
            return (Point - block.Get_Pos());
        }

        /// <summary>
        /// Transforms a point from local space relative to the specified block into absolute screen-space.
        /// </summary>
        public ePos PointToScreen(eBlock block, ePos Point)
        {
            return (Point + block.Get_Pos());
        }
#endregion

#region Hit Testing
        /// <summary>
        /// Returns true if the given screen-space point intersects this elements click-hitbox
        /// </summary>
        /// <param name="pos"></param>
        /// <returns></returns>
        public bool HitTest_ScreenPos(ePos pos)
        {
            return Block_ClickArea.Intersects(pos + Get_Scroll_Total() - Scroll);
        }

        /// <summary>
        /// Returns the element which intersects the given screen-space point or NULL if none
        /// </summary>
        /// <param name="pos">Screen-Space point to test for intersection with</param>
        public virtual cssElement Get_Hit_Element(ePos pos)
        {
            if (HitTest_ScreenPos(pos)) return this;
            return null;
        }
#endregion

#region Sizing
#endregion

#region Positioning
        /*
        /// <summary>
        /// Clears this elements positioners and sets both it's explicit and implicit positions to zero
        /// </summary>
        public void Clear_Pos()
        {
            Style.Default.Set_Position_Implicit(StyleValue.Unset, StyleValue.Unset);

            xAligner?.Unhook();
            yAligner?.Unhook();

            xAligner = null;
            yAligner = null;
        }
        /// <summary>
        /// Positions the control so it's top edge is yOff away below another given control once.
        /// </summary>
        public void moveBelow(uiElement targ, int yOff = 0)
        {
            if (targ == null) throw new ArgumentNullException(ID + " target cannot be NULL!");
            if (targ.Parent != Parent) throw new ArgumentException(ID + " target control must be parented to the same control!");

            Style.Default.Set_Position(Block.Left, targ.Block.Bottom + yOff);
        }

        /// <summary>
        /// Positions the control so it's bottom edge is yOff above the top edge of another given control once.
        /// </summary>
        public void moveAbove(uiElement targ, int yOff = 0)
        {
            if (targ == null) throw new ArgumentNullException(ID + " target cannot be NULL!");
            if (targ.Parent != Parent) throw new ArgumentException(ID + " target control must be parented to the same control!");

            Style.Default.Set_Position(Block.Left, targ.Block.Top - Block.Height - yOff);
        }

        /// <summary>
        /// Positions the control so it's right edge is xOff away from the right edge of another given control once.
        /// </summary>
        public void moveRightOf(uiElement targ, int xOff = 0)
        {
            if (targ == null) throw new ArgumentNullException(ID + " target cannot be NULL!");
            if (targ.Parent != Parent) throw new ArgumentException(ID + " target control must be parented to the same control!");

            Style.Default.Set_Position(targ.Block.Right + xOff, Block.Top);
            //if (horizontal_positioner == null || !horizontal_positioner.Equals(targ, xOff, cPosDir.RIGHT)) horizontal_positioner = new ControlPositioner(targ, xOff, cPosDir.RIGHT);
        }

        /// <summary>
        /// Positions the control so it's right edge is xOff away from the left edge of another given control once.
        /// </summary>
        public void moveLeftOf(uiElement targ, int xOff = 0)
        {
            if (targ == null) throw new ArgumentNullException(ID + " target cannot be NULL!");
            if (targ.Parent != Parent) throw new ArgumentException(ID + " target control must be parented to the same control!");

            Style.Default.Set_Position(targ.Block.Left - Block.Width - xOff, Block.Top);
            //if (horizontal_positioner == null || !horizontal_positioner.Equals(targ, xOff, cPosDir.LEFT)) horizontal_positioner = new ControlPositioner(targ, xOff, cPosDir.LEFT);
        }

        /// <summary>
        /// Adjusts X & Y position of the control so it sites directly above another given control forever.
        /// </summary>
        public void sitAbove(uiElement targ, int yOff = 0)
        {
            if (targ == null) throw new ArgumentNullException(ID + " target cannot be NULL!");
            if (targ.Parent != Parent) throw new ArgumentException(ID + " target control must be parented to the same control!");

            Style.Default.Set_Position(targ.Block.Left, targ.Block.Y - Block.Height - yOff);
            xAligner = null;
            //if (vertical_positioner == null || !vertical_positioner.Equals(targ, yOff, cPosDir.ABOVE)) vertical_positioner = new ControlPositioner(targ, yOff, cPosDir.SIT_ABOVE);
        }

        /// <summary>
        /// Adjusts X & Y position of the control so it sites directly below another given control forever.
        /// </summary>
        public void sitBelow(uiElement targ, int yOff = 0)
        {
            if (targ == null) throw new ArgumentNullException(ID + " target cannot be NULL!");
            if (targ.Parent != Parent) throw new ArgumentException(ID + " target control must be parented to the same control!");

            Style.Default.Set_Position(targ.Block.Left, targ.Block.Bottom + yOff);
            xAligner = null;
            //if (vertical_positioner == null || !vertical_positioner.Equals(targ, yOff, cPosDir.BELOW)) vertical_positioner = new ControlPositioner(targ, yOff, cPosDir.SIT_BELOW);
        }

        /// <summary>
        /// Adjusts X & Y position of the control so it sites directly to the right of another given control forever.
        /// </summary>
        public void sitRightOf(uiElement targ, int xOff = 0)
        {
            if (targ == null) throw new ArgumentNullException(ID + " target cannot be NULL!");
            if (targ.Parent != Parent) throw new ArgumentException(ID + " target control must be parented to the same control!");

            Style.Default.Set_Position(targ.Block.Right + xOff, targ.Block.Y);
            yAligner = null;
            //if (horizontal_positioner == null || !horizontal_positioner.Equals(targ, xOff, cPosDir.RIGHT)) horizontal_positioner = new ControlPositioner(targ, xOff, cPosDir.SIT_RIGHT_OF);
        }

        /// <summary>
        /// Adjusts X & Y position of the control so it sites directly to the left of another given control forever.
        /// </summary>
        public void sitLeftOf(uiElement targ, int xOff = 0)
        {
            if (targ == null) throw new ArgumentNullException(ID + " target cannot be NULL!");
            if (targ.Parent != Parent) throw new ArgumentException(ID + " target control must be parented to the same control!");

            Style.Default.Set_Position(targ.Block.Left - Block.Width - xOff, targ.Block.Y);
            yAligner = null;
            //if (horizontal_positioner == null || !horizontal_positioner.Equals(targ, xOff, cPosDir.LEFT)) horizontal_positioner = new ControlPositioner(targ, xOff, cPosDir.SIT_LEFT_OF);
        }

        /// <summary>
        /// Repositions the control so it's top edge sits on it's parent's top edge.
        /// </summary>
        /// <param name="yOff">Offset from the edge where we will be positioned</param>
        public void alignTop(int yOff = 0)
        {
            int y = yOff;
            Style.Default.Y.Set(y);
            if (yAligner == null || !yAligner.Equals(null, yOff, cPosDir.TOP_OF)) yAligner = new uiAligner(this, null, yOff, cPosDir.TOP_OF);
        }

        /// <summary>
        /// Repositions the control so it's bottom edge sits on it's parent's bottom edge.
        /// </summary>
        /// <param name="yOff">Offset from the edge where we will be positioned</param>
        public void alignBottom(int yOff = 0)
        {
            if (!isChild) throw new ArgumentException("Must have a parent inorder to call alignBottom()!");
            int val = Block_Containing.Height;// parent.inner_area.height;
            
            int y = (val - Block.Height - yOff);
            Style.Default.Y.Set(y);
            
            if (yAligner == null || !yAligner.Equals(null, yOff, cPosDir.BOTTOM_OF)) yAligner = new uiAligner(this, null, yOff, cPosDir.BOTTOM_OF);
        }

        /// <summary>
        /// Repositions the control so it's left edge sits on it's parent's left edge.
        /// </summary>
        /// <param name="xOff">Offset from the edge where we will be positioned</param>
        public void alignLeftSide(int xOff = 0)
        {
            int x = xOff;
            Style.Default.X.Set(x);

            if (xAligner == null || !xAligner.Equals(null, xOff, cPosDir.LEFT_SIDE_OF)) xAligner = new uiAligner(this, null, xOff, cPosDir.LEFT_SIDE_OF);
        }

        /// <summary>
        /// Repositions the control so it's right edge sits on it's parent's right edge.
        /// </summary>
        /// <param name="xOff">Offset from the edge where we will be positioned</param>
        public void alignRightSide(int xOff = 0)
        {
            if (!isChild) throw new ArgumentException("Must have a parent inorder to call alignRightSide()!");
            int val = Block_Containing.Width;

            int x = (val - Block.Width - xOff);
            Style.Default.X.Set(x);

            if (isChild) return;
            else if (xAligner == null || !xAligner.Equals(null, xOff, cPosDir.RIGHT_SIDE_OF)) xAligner = new uiAligner(this, null, xOff, cPosDir.RIGHT_SIDE_OF);
        }

        /// <summary>
        /// Aligns the control to the middle of the parent area's Vertical axis.
        /// </summary>
        /// <param name="yOff">An offset from the center</param>
        public void CenterVertically(int yOff = 0)
        {
            if (!isChild) throw new ArgumentException("Must have a parent inorder to call CenterVertically()!");
            int val = Block_Containing.Height;
            
            int y = ((val / 2) - (Block.Height / 2) + yOff);
            Style.Default.Y.Set(y);
            
            if (yAligner == null || !yAligner.Equals(null, yOff, cPosDir.CENTER_Y)) yAligner = new uiAligner(this, null, yOff, cPosDir.CENTER_Y);
        }

        /// <summary>
        /// Aligns the control to the middle of the parent area's Horizontal axis.
        /// </summary>
        /// <param name="xOff">An offset from the center</param>
        public void CenterHorizontally(int xOff = 0)
        {
            if (!isChild) throw new ArgumentException("Must have a parent inorder to call CenterHorizontally()!");
            int val = Block_Containing.Width;

            int x = ((val / 2) - (Block.Width / 2) + xOff);
            Style.Default.X.Set(x);
            
            if (xAligner == null || !xAligner.Equals(null, xOff, cPosDir.CENTER_X)) xAligner = new uiAligner(this, null, xOff, cPosDir.CENTER_X);
        }
        */
#endregion

#region Behavior Triggers
        public void Click()
        {
            if (!HasFlags(EElementFlags.Clickable)) return;
            Handle_Click(this, new DomRoutedEventArgs(this));
        }

        public void DoubleClick()
        {
            if (!HasFlags(EElementFlags.DoubleClickable)) return;
            
            Handle_DoubleClick(this, new DomRoutedEventArgs(this));
        }
#endregion
        
#region Dragging Event Delegates
        /// <summary>
        /// Called at the start of the dragging process
        /// </summary>
        public event Action<cssElement, DomItemDragEventArgs> DraggingStart;
        /// <summary>
        /// Called as the element is being dragged
        /// </summary>
        public event Action<cssElement, DomItemDragEventArgs> DraggingUpdate;
        /// <summary>
        /// Called when the dragging operation ends (including being cancelled)
        /// Occurs AFTER <see cref="Handle_DraggingConfirm"/>
        /// </summary>
        public event Action<cssElement, DomItemDragEventArgs> DraggingEnd;
        /// <summary>
        /// Called when the dragging operation is confirmed (ended without cancelling)
        /// </summary>
        public event Action<cssElement, DomItemDragEventArgs> DraggingConfirm;
#endregion

#region Keyboard Event Delegates
        /// <summary>
        /// Called whenever the user presses a character key while the element has input-focus
        /// </summary>
        public event Action<cssElement, uiCancellableEvent<DomKeyboardKeyEventArgs>> KeyPress;
        /// <summary>
        /// Called whenever a keyboard key is depressed while the element has input-focus
        /// </summary>
        public event Action<cssElement, DomKeyboardKeyEventArgs> KeyUp;
        /// <summary>
        /// Called whenever a keyboard key is pressed while the element has input-focus
        /// </summary>
        public event Action<cssElement, DomKeyboardKeyEventArgs> KeyDown;
#endregion

#region Routed Events

#region Mouse Delegates
        /// <summary>
        /// Called whenever the mouse releases a button whilst over the element
        /// </summary>
        /// <param name="Sender">The UI element sending us this event, or NULL if we are the first element to receive it.</param>
        public event Action<cssElement, DomMouseButtonEventArgs> MouseUp;
        /// <summary>
        /// Called whenever the mouse presses a button whilst over the element
        /// </summary>
        /// <param name="Sender">The UI element sending us this event, or NULL if we are the first element to receive it.</param>
        public event Action<cssElement, DomMouseButtonEventArgs> MouseDown;
        /// <summary>
        /// Called whenever the element is 'clicked' by the mouse or by keyboard input (pressing ENTER)
        /// </summary>
        /// <param name="Sender">The UI element sending us this event, or NULL if we are the first element to receive it.</param>
        public event Action<cssElement, DomRoutedEventArgs> Clicked;
        /// <summary>
        /// Called whenever the element is 'double clicked' by the mouse or by keyboard input (pressing ENTER)
        /// </summary>
        /// <param name="Sender">The UI element sending us this event, or NULL if we are the first element to receive it.</param>
        public event Action<cssElement, DomRoutedEventArgs> DoubleClicked;
        /// <summary>
        /// Called whenever the mouse clicks the element
        /// </summary>
        /// <param name="Sender">The UI element sending us this event, or NULL if we are the first element to receive it.</param>
        public event Action<cssElement, DomMouseButtonEventArgs> MouseClick;
        /// <summary>
        /// Called whenever the mouse double clicks the element
        /// </summary>
        /// <param name="Sender">The UI element sending us this event, or NULL if we are the first element to receive it.</param>
        public event Action<cssElement, DomMouseButtonEventArgs> MouseDoubleClick;
        /// <summary>
        /// Called whenever the mouse wheel scrolls whilst over the element
        /// </summary>
        /// <param name="Sender">The UI element sending us this event, or NULL if we are the first element to receive it.</param>
        public event Action<cssElement, DomMouseWheelEventArgs> MouseWheel;
        /// <summary>
        /// Called whenever the mouse moves whilst over the element
        /// </summary>
        /// <param name="Sender">The UI element sending us this event, or NULL if we are the first element to receive it.</param>
        public event Action<cssElement, DomMouseMoveEventArgs> MouseMove;
        /// <summary>
        /// Called whenever the mouse pauses for longer than <see cref="UI_CONSTANTS.HOVER_TIME"/> over the element
        /// </summary>
        /// <param name="Sender">The UI element sending us this event, or NULL if we are the first element to receive it.</param>
        public event Action<cssElement> MouseHover;
        /// <summary>
        /// Called whenever the mouse first moves onto the element, but not one of its children
        /// </summary>
        public event Action<cssElement> MouseEnter;
        /// <summary>
        /// Called whenever the mouse moved out of the element, but not one of its children
        /// </summary>
        public event Action<cssElement> MouseLeave;
#endregion

#region Mouse Handlers
        public bool IsBeingDragged { get { return object.ReferenceEquals(Root.Mouse.Dragging_Target, this); } }

        /// <summary>
        /// Called whenever the mouse releases a button whilst over the element
        /// </summary>
        /// <param name="Sender">The UI element sending us this event, or NULL if we are the first element to receive it.</param>
        public virtual void Handle_MouseUp(cssElement Sender, DomMouseButtonEventArgs Args)
        {
            if (Args.Button == EMouseButton.Left) IsActive = false;
            MouseUp?.Invoke(this, Args);
        }
        /// <summary>
        /// Called whenever the mouse presses a button whilst over the element
        /// </summary>
        /// <param name="Sender">The UI element sending us this event, or NULL if we are the first element to receive it.</param>
        public virtual void Handle_MouseDown(cssElement Sender, DomMouseButtonEventArgs Args)
        {
            if (Args.Button == EMouseButton.Left) IsActive = true;
            MouseDown?.Invoke(this, Args);
        }

        /// <summary>
        /// Called whenever the mouse wheel scrolls whilst over the element
        /// </summary>
        /// <param name="Sender">The UI element sending us this event, or NULL if we are the first element to receive it.</param>
        public virtual void Handle_MouseWheel(cssElement Sender, DomMouseWheelEventArgs Args)
        {
            MouseWheel?.Invoke(this, Args);
        }
        /// <summary>
        /// Called whenever the mouse moves whilst over the element.
        /// <para>Fires after MouseEnter</para>
        /// </summary>
        /// <param name="Sender">The UI element sending us this event, or NULL if we are the first element to receive it.</param>
        public virtual void Handle_MouseMove(cssElement Sender, DomMouseMoveEventArgs Args)
        {
            MouseMove?.Invoke(this, Args);
        }
        
        /// <summary>
        /// Called whenever the mouse rests on the element
        /// <para>Rest delay dictated by <see cref="UI_CONSTANTS.HOVER_TIME"/></para>
        /// </summary>
        /// <param name="Sender">The UI element sending us this event, or NULL if we are the first element to receive it.</param>
        public virtual void Handle_MouseHover(cssElement Sender)
        {
            MouseHover?.Invoke(this);
        }
        /// <summary>
        /// Called whenever the mouse moves onto the element, but not one of its children
        /// <para>Fires before MouseMove</para>
        /// </summary>
        /// <param name="Sender">The UI element sending us this event, or NULL if we are the first element to receive it.</param>
        public virtual void Handle_MouseEnter(cssElement Sender)
        {
            IsMouseOver = true;
            Style.Set_State(ElementPropertySystem.STATE_HOVER, true);
            MouseEnter?.Invoke(this);
        }
        /// <summary>
        /// Called whenever the mouse moves out of the element, but not one of its children
        /// </summary>
        /// <param name="Sender">The UI element sending us this event, or NULL if we are the first element to receive it.</param>
        public virtual void Handle_MouseLeave(cssElement Sender)
        {
            IsMouseOver = false;
            Style.Set_State(ElementPropertySystem.STATE_HOVER, false);
            MouseLeave?.Invoke(this);
        }

        /// <summary>
        /// Called whenever the element is 'clicked' by the mouse or by keyboard input (pressing ENTER)
        /// </summary>
        /// <param name="Sender">The UI element sending us this event, or NULL if we are the first element to receive it.</param>
        public virtual void Handle_Click(cssElement Sender, DomRoutedEventArgs Args)
        {
            Clicked?.Invoke(this, Args);
        }
        /// <summary>
        /// Called whenever the element is 'double clicked' by the mouse or by keyboard input (pressing ENTER)
        /// </summary>
        /// <param name="Sender">The UI element sending us this event, or NULL if we are the first element to receive it.</param>
        public virtual void Handle_DoubleClick(cssElement Sender, DomRoutedEventArgs Args)
        {
            DoubleClicked?.Invoke(this, Args);
        }

        /// <summary>
        /// Called whenever the mouse clicks the element.
        /// Two single clicks that occur close enough in time, as determined by the mouse settings of the user's operating system, will generate a MouseDoubleClick event instead of the second MouseClick event.
        /// </summary>
        /// <param name="Sender">The UI element sending us this event, or NULL if we are the first element to receive it.</param>
        public virtual void Handle_MouseClick(cssElement Sender, DomMouseButtonEventArgs Args)
        {
            MouseClick?.Invoke(this, Args);
        }
        /// <summary>
        /// Called whenever the mouse double clicks the element
        /// </summary>
        /// <param name="Sender">The UI element sending us this event, or NULL if we are the first element to receive it.</param>
        public virtual void Handle_MouseDoubleClick(cssElement Sender, DomMouseButtonEventArgs Args)
        {
            MouseDoubleClick?.Invoke(this, Args);
        }
#endregion

#endregion

#region Drag Events
        /// <summary>
        /// Fires at the start of the dragging process
        /// </summary>
        public virtual void Handle_DraggingStart(cssElement Sender, DomItemDragEventArgs Args)
        {
            DraggingStart?.Invoke(this, Args);
        }
        /// <summary>
        /// Fired as the element is being dragged
        /// </summary>
        public virtual void Handle_DraggingUpdate(cssElement Sender, DomItemDragEventArgs Args)
        {
            DraggingUpdate?.Invoke(this, Args);
        }
        /// <summary>
        /// Fires when the dragging operation is confirmed (ended without cancelling)
        /// </summary>
        public virtual void Handle_DraggingConfirm(cssElement Sender, DomItemDragEventArgs Args)
        {
            DraggingConfirm?.Invoke(this, Args);
        }
        /// <summary>
        /// Fires when the dragging operation ends (including being cancelled)
        /// Called AFTER <see cref="Handle_DraggingConfirm"/>
        /// </summary>
        public virtual void Handle_DraggingEnd(cssElement Sender, DomItemDragEventArgs Args)
        {
            DraggingEnd?.Invoke(this, Args);
        }
#endregion

#region Preview Events

#region Delegates
        /// <summary>
        /// Called whenever the mouse releases a button whilst over the element
        /// </summary>
        /// <param name="Sender">The UI element sending us this event, or NULL if we are the first element to receive it.</param>
        public event Action<cssElement, DomPreviewMouseButtonEventArgs> PreviewMouseUp;
        /// <summary>
        /// Called whenever the mouse presses a button whilst over the element
        /// </summary>
        /// <param name="Sender">The UI element sending us this event, or NULL if we are the first element to receive it.</param>
        public event Action<cssElement, DomPreviewMouseButtonEventArgs> PreviewMouseDown;
        /// <summary>
        /// Called whenever the element is 'clicked' by the mouse or by keyboard input (pressing ENTER)
        /// </summary>
        /// <param name="Sender">The UI element sending us this event, or NULL if we are the first element to receive it.</param>
        public event Action<cssElement, DomPreviewEventArgs> PreviewClicked;
        /// <summary>
        /// Called whenever the element is 'double clicked' by the mouse or by keyboard input (pressing ENTER)
        /// </summary>
        /// <param name="Sender">The UI element sending us this event, or NULL if we are the first element to receive it.</param>
        public event Action<cssElement, DomPreviewEventArgs> PreviewDoubleClicked;
        /// <summary>
        /// Called whenever the mouse clicks the element
        /// </summary>
        /// <param name="Sender">The UI element sending us this event, or NULL if we are the first element to receive it.</param>
        public event Action<cssElement, DomPreviewMouseButtonEventArgs> PreviewMouseClick;
        /// <summary>
        /// Called whenever the mouse double clicks the element
        /// </summary>
        /// <param name="Sender">The UI element sending us this event, or NULL if we are the first element to receive it.</param>
        public event Action<cssElement, DomPreviewMouseButtonEventArgs> PreviewMouseDoubleClick;
        /// <summary>
        /// Called whenever the mouse wheel scrolls whilst over the element
        /// </summary>
        /// <param name="Sender">The UI element sending us this event, or NULL if we are the first element to receive it.</param>
        public event Action<cssElement, DomPreviewMouseWheelEventArgs> PreviewMouseWheel;
        /// <summary>
        /// Called whenever the mouse moves whilst over the element
        /// </summary>
        /// <param name="Sender">The UI element sending us this event, or NULL if we are the first element to receive it.</param>
        public event Action<cssElement, DomPreviewMouseMoveEventArgs> PreviewMouseMove;
#endregion

#region Handlers

        /// <summary>
        /// Called whenever the mouse releases a button whilst over the element
        /// </summary>
        /// <param name="Sender">The UI element sending us this event, or NULL if we are the first element to receive it.</param>
        public virtual void Handle_PreviewMouseUp(cssElement Sender, DomPreviewMouseButtonEventArgs Args)
        {
            Args.Handler = this;
            PreviewMouseUp?.Invoke(this, Args);
        }
        /// <summary>
        /// Called whenever the mouse presses a button whilst over the element
        /// </summary>
        /// <param name="Sender">The UI element sending us this event, or NULL if we are the first element to receive it.</param>
        public virtual void Handle_PreviewMouseDown(cssElement Sender, DomPreviewMouseButtonEventArgs Args)
        {
            Args.Handler = this;
            PreviewMouseDown?.Invoke(this, Args);
        }

        /// <summary>
        /// Called whenever the mouse wheel scrolls whilst over the element
        /// </summary>
        /// <param name="Sender">The UI element sending us this event, or NULL if we are the first element to receive it.</param>
        public virtual void Handle_PreviewMouseWheel(cssElement Sender, DomPreviewMouseWheelEventArgs Args)
        {
            Args.Handler = this;
            PreviewMouseWheel?.Invoke(this, Args);
        }
        /// <summary>
        /// Called whenever the mouse moves whilst over the element.
        /// <para>Fires after MouseEnter</para>
        /// </summary>
        /// <param name="Sender">The UI element sending us this event, or NULL if we are the first element to receive it.</param>
        public virtual void Handle_PreviewMouseMove(cssElement Sender, DomPreviewMouseMoveEventArgs Args)
        {
            Args.Handler = this;
            PreviewMouseMove?.Invoke(this, Args);
        }
        
        /// <summary>
        /// Called whenever the element is 'clicked' by mouse input or otherwise
        /// </summary>
        /// <param name="Sender">The UI element sending us this event, or NULL if we are the first element to receive it.</param>
        public virtual void Handle_PreviewClick(cssElement Sender, DomPreviewEventArgs Args)
        {
            Args.Handler = this;
            PreviewClicked?.Invoke(this, Args);
        }
        /// <summary>
        /// Called whenever the element is 'double clicked' by mouse input or otherwise
        /// </summary>
        /// <param name="Sender">The UI element sending us this event, or NULL if we are the first element to receive it.</param>
        public virtual void Handle_PreviewDoubleClick(cssElement Sender, DomPreviewEventArgs Args)
        {
            Args.Handler = this;
            PreviewDoubleClicked?.Invoke(this, Args);
        }

        /// <summary>
        /// Called whenever the mouse clicks the element.
        /// Two single clicks that occur close enough in time, as determined by the mouse settings of the user's operating system, will generate a MouseDoubleClick event instead of the second MouseClick event.
        /// </summary>
        /// <param name="Sender">The UI element sending us this event, or NULL if we are the first element to receive it.</param>
        public virtual void Handle_PreviewMouseClick(cssElement Sender, DomPreviewMouseButtonEventArgs Args)
        {
            Args.Handler = this;
            PreviewMouseClick?.Invoke(this, Args);
        }
        /// <summary>
        /// Called whenever the mouse double clicks the element
        /// </summary>
        /// <param name="Sender">The UI element sending us this event, or NULL if we are the first element to receive it.</param>
        public virtual void Handle_PreviewMouseDoubleClick(cssElement Sender, DomPreviewMouseButtonEventArgs Args)
        {
            Args.Handler = this;
            PreviewMouseDoubleClick?.Invoke(this, Args);
        }
#endregion

#endregion

#region Keyboard Event Handlers
        /// <summary>
        /// Called whenever the user presses a character key while the element has input-focus
        /// </summary>
        /// <returns>Continue</returns>
        public virtual bool Handle_KeyPress(cssElement Sender, DomKeyboardKeyEventArgs Args)
        {
            var e = new uiCancellableEvent<DomKeyboardKeyEventArgs>(Args);
            KeyPress?.Invoke(this, e);
            if (e.Cancel) return false;

            return true;
        }
        /// <summary>
        /// Called whenever a keyboard key is depressed while the element has input-focus
        /// </summary>
        public virtual bool Handle_KeyUp(cssElement Sender, DomKeyboardKeyEventArgs Args)
        {
            KeyUp?.Invoke(this, Args);

            return true;
        }
        /// <summary>
        /// Called whenever a keyboard key is pressed while the element has input-focus
        /// </summary>
        public virtual bool Handle_KeyDown(cssElement Sender, DomKeyboardKeyEventArgs Args)
        {
            KeyDown?.Invoke(this, Args);

            return true;
        }
#endregion

        

#region Input Focus
        /// <summary>
        /// Sets input focus to the element
        /// </summary>
        /// <returns>Success</returns>
        public bool Focus()
        {
            // Find the nearest FocusScope
            cssElement E = this;
            while (E != null && E.FocusScope == null)
            {
                E = E.Parent;
            }
            if (E == null) return false;
            E.FocusScope.Set(this);// Set ourself to the logical focus element, if this scope has active input focus then it will transfer it to us...
            return true;
        }

        public FocusScope FocusScope { get; protected set; }  = null;
        /// <summary>
        /// Indicates whether the element currently has input focus
        /// </summary>
        public bool HasFocus { get { return object.ReferenceEquals(this, Root.FocusedElement); } }
        public event Action<cssElement> onFocusGain;
        public event Action<cssElement> onFocusLose;

        /// <summary>
        /// Handles logic for an element gaining input focus
        /// </summary>
        internal virtual void Handle_InputFocusGain()
        {
            if (FocusScope != null)// If we are a focus scope containing element then we pass it on.
            {
                FocusScope.GainFocus();
            }
            else
            {
                Root.SetFocus(this);// Let the keyboard manager know that we accepted input focus
                Style.Set_State(ElementPropertySystem.STATE_FOCUS, true);
                onFocusGain?.Invoke(this);
            }
        }
        /// <summary>
        /// Handles logic for an element losing input focus
        /// </summary>
        internal virtual void Handle_InputFocusLose()
        {
            if (FocusScope != null)// If we are a focus scope containing element then we pass it on.
            {
                FocusScope.LoseFocus();
            }
            else
            {
                Style.Set_State(ElementPropertySystem.STATE_FOCUS, false);
                onFocusLose?.Invoke(this);
            }
        }
#endregion
    }
}
