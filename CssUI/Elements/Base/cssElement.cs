//#define PRINT_BLOCK_CHANGE_SOURCE
using xLog;
using SixLabors.Fonts;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CssUI.Types;
using CssUI.CSS;
using CssUI.DOM;
using CssUI.Enums;
using CssUI.CSS.Enums;
using System.Linq;

namespace CssUI
{

    /// <summary>
    /// The basis for all OpenGL based UI elements.
    /// <para>NOTE: All OpenGL UI elements inheriting from this base class makes the assumption that texturing is disabled by default when drawing.</para>
    /// </summary>
    public abstract class cssElement : HTMLElement, IDisposable
    {

        /// <summary>
        /// The CSS type-name used when referencing this element from within a StyleSheet, 
        /// this is set by all element classes which inherit from the uiElement type
        /// </summary>
        public static readonly string CssTagName = null;

        #region Identity
        /// <summary>
        /// An ID number, unique among all UI elements, assigned to this element.
        /// </summary>
        public readonly cssElementID UID = new cssElementID();

        /// <summary>
        /// Allows us to resolve the fullpath just once.
        /// </summary>
        CacheableValue<string> Cached_FullPath = new CacheableValue<string>();

        protected ILogger Logs { get; }

        public string FullPath
        {
            get
            {
                return Cached_FullPath.Get(() => 
                {
                    string str = string.Empty;
                    if (Parent?.FullPath != null)
                        str += Parent?.FullPath + "/";
                    //str += (ID != null ? ID : TypeName);
                    str += string.Concat(CssTagName, "-", UID);
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
    /// <summary>
    /// Tracks what factors of this element need to be updated.
    /// </summary>
    protected EElementDirtyFlags Dirt = EElementDirtyFlags.Clean;
    /// <summary>
    /// Adds a flag from the dirty bit
    /// </summary>
    /// <param name="flags">Set of flags to add</param>
    public void Flag_Dirty(EElementDirtyFlags flags) { Dirt |= flags; }
    /// <summary>
    /// Removes a flag from the dirty bit
    /// </summary>
    /// <param name="flags">Set of flags to remove</param>
    public void Unflag_Dirty(EElementDirtyFlags flags) { Dirt &= ~flags; }

    private EElementFlags Flags = EElementFlags.Clickable | EElementFlags.DoubleClickable;// By default all elements will process click and doubleclick events

    /// <summary>
    /// Add flags to the element
    /// </summary>
    /// <param name="flags">Set of flags to add</param>
    public void Flags_Add(EElementFlags flags) { Flags |= flags; }
    /// <summary>
    /// Remove flags from the element
    /// </summary>
    /// <param name="flags">Set of flags to remove</param>
    public void Flags_Remove(EElementFlags flags) { Flags &= ~flags; }
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
        Flag_Box_Dirty(EBoxInvalidationReason.Layout_Pos_Changed);
    }
    #endregion

        #region Style
    public ElementPropertySystem Style { get; private set; }
    #endregion

        #region Display
        public DebugOpts Debug = new DebugOpts() { /*Draw_Bounds = true, Draw_Child_Bounds = true*/  };
        public cssViewport Viewport { get; protected set; } = null;
        
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
        protected bool HasClipping { get { return (ClippingArea != null); } }
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
            if (Parent != null) Parent.Flag_Layout(ELayoutDirt.Dirty_Children);
            update_debug_text();
            onResized?.Invoke(this, oldSize, newSize);
        }
        
        /// <summary>
        /// Occurs after <see cref="Update_Cached_Blocks"/>
        /// </summary>
        protected virtual void Handle_Moved(ePos oldPos, ePos newPos)
        {
            if (Parent != null) Parent.Flag_Layout(ELayoutDirt.Dirty_Children);
            onMoved?.Invoke(this, oldPos, newPos);
        }

        /// <summary>
        /// The content block has changed dimensions
        /// </summary>
        protected virtual void Handle_Content_Block_Change()
        {
        }


        #region Property Change Handlers

        /// <summary>
        /// A property used for our styling has changed its assigned value
        /// </summary>
        /// <param name="Sender"></param>
        /// <param name="Flags"></param>
        /// <param name="Source"></param>
        protected virtual void Handle_Style_Property_Change(ICssProperty Sender, EPropertyDirtFlags Flags, System.Diagnostics.StackTrace Source)
        {

    #if DEBUG
            if (Debug.Log_Property_Changes)
            {
                if (Sender != null)
                {
                    if (Flags.HasFlag(EPropertyDirtFlags.Visual) || Flags.HasFlag(EPropertyDirtFlags.Text))
                    {
                        Logs.Info("[Property Change] {0}.{1} => {2}", this.FullPath, Sender.CssName, Sender);
                    }
                }
            }
    #endif

            if ((Flags & EPropertyDirtFlags.Visual) != 0)
            {
                Dirt |= EElementDirtyFlags.Visuals;
            }

            if ((Flags & EPropertyDirtFlags.Box) != 0)
            {
                Box.Flag(EBoxInvalidationReason.Property_Changed);
                Box.FlagProperty(Flags);
            }

            if ((Flags & EPropertyDirtFlags.Text) != 0)
            {
                Flag_Dirty(EElementDirtyFlags.Font);
            }

            if ((Flags & EPropertyDirtFlags.Flow) != 0)
            {
                if (Sender == null) Invalidate_Layout();
                else Invalidate_Layout(Sender);
            }
        }
        
        #endregion
        #endregion
                
        #region Content
        public virtual bool IsEmpty { get { return true; } }
        #endregion

        #region Box
        public readonly CssPrincipalBox Box;
        #endregion

        #region Blocks
        /// <summary>
        /// The area for an element that dictates the area it's content may be drawn in, or NULL if no clipping should be used
        /// </summary>
        public CssBoxArea ClippingArea = null;


        /// <summary>
        /// Notifies this element that its containing block has changed, meaning it's style should be set to dirty
        /// </summary>
        public virtual void Handle_Containing_Block_Dirty()
        {
            if (Box.DependsOnContainer)
            {
                Flag_Box_Dirty(EBoxInvalidationReason.Containing_Block_Changed);
                Style.Handle_Parent_Block_Change();
            }
        }

        /// <summary>
        /// Flags this elements block as dirty, meaning it needs to be updated when next convenient
        /// </summary>
        public virtual void Flag_Box_Dirty(EBoxInvalidationReason Reason = EBoxInvalidationReason.Unknown)
        {
            Box.Flag(Reason);
    #if DEBUG
            if (Debug.Log_Block_Changes)
            {
                if (Reason == EBoxInvalidationReason.Unknown)
                    Logs.Info(xLog.XTERM.magenta("[Block Change] {0} (UNKNOWN SOURCE!)"), this);
                else
                    Logs.Info(xLog.XTERM.magenta("[Block Change] {0} {1}"), this, Enum.GetName(typeof(EBoxInvalidationReason), Reason).ToUpper());
            }
    #endif
        }


    #endregion

        #region Setters
            /// <summary>
            /// Sets the explicit Width and Height properties for this elements default style state
            /// </summary>
            public void Set_Size(int? Width, int? Height)
            {
                Style.UserRules.Width.Set(Width);
                Style.UserRules.Height.Set(Height);
            }

            /// <summary>
            /// Sets the explicit X(Left) and Y(Top) properties for this elements default style state
            /// </summary>
            public void Set_Pos(int? X, int? Y)
            {
                Style.UserRules.Left.Set(X);
                Style.UserRules.Top.Set(Y);
            }

            /// <summary>
            /// Sets the explicit Top, Right, Bottom, and Left margin properties for this elements default style state
            /// </summary>
            public void Set_Margin(int? T, int? R, int? B, int? L)
            {
                Style.UserRules.Margin_Top.Set(T);
                Style.UserRules.Margin_Right.Set(R);
                Style.UserRules.Margin_Bottom.Set(B);
                Style.UserRules.Margin_Left.Set(L);
            }

            /// <summary>
            /// Sets the explicit Top, Right, Bottom, and Left padding properties for this elements default style state
            /// </summary>
            public void Set_Padding(int? T, int? R, int? B, int? L)
            {
                Style.UserRules.Padding_Top.Set(T);
                Style.UserRules.Padding_Right.Set(R);
                Style.UserRules.Padding_Bottom.Set(B);
                Style.UserRules.Padding_Left.Set(L);
            }
    #endregion

        #region Layout
            /// <summary>
            /// Specifys something that affects the controls boundaries has changed, be it the pos, size, border, contents, etc.
            /// </summary>
            protected ELayoutDirt LayoutDirt = ELayoutDirt.Dirty;
            public void Flag_Layout(ELayoutDirt Flags) { LayoutDirt |= Flags; }

            public void Invalidate_Layout(EBoxInvalidationReason Reason = EBoxInvalidationReason.Unknown)
            {
                Flag_Layout(ELayoutDirt.Dirty);
            #if DEBUG
                if (Debug.Log_Layout_Changes)
                {
                    if (Reason == EBoxInvalidationReason.Unknown)
                        Logs.Info(xLog.XTERM.cyan("[Layout Flagged] {0} (UNKNOWN SOURCE!)"), this);
                    else
                        Logs.Info(xLog.XTERM.cyan("[Layout Flagged] {0} {1}"), this, Enum.GetName(typeof(EBoxInvalidationReason), Reason).ToUpper());
                     
                }
            #endif
            }

            public void Invalidate_Layout(ICssProperty sender)
            {
                Flag_Layout(ELayoutDirt.Dirty);
            #if DEBUG
                if (Debug.Log_Layout_Changes) Logs.Info(xLog.XTERM.cyan("[Layout Flagged] {0}.{1} => {2}"), this, sender.CssName, sender);
            #endif
            }

            /// <summary>
            /// Forces the element to apply layout logic to all of it's child elements.
            /// </summary>
            public async void PerformLayout()
            {
                if (LayoutDirt == ELayoutDirt.Clean)
                    return;

                // await Style.Cascade();
                //Guid TMR = Timing.Start("PerformLayout()");
            
                if (Box.Containing_Box.X != last_containerPos.X || Box.Containing_Box.Y != last_containerPos.Y)
                {
                    last_containerPos.X = Box.Containing_Box.X;
                    last_containerPos.Y = Box.Containing_Box.Y;
                }

                // If our block is dirty then we need to update it NOW before we update the layout and give useless block positions to our child-elements
                if (Box.IsDirty)
                {
                    Box.Rebuild();
                }

                int cycles = 0;
                const int MAX_LAYOUT_CYCLES = 100;
                do
                {
                    LayoutDirt = ELayoutDirt.Clean;
                    Handle_Layout();
                }
                while (LayoutDirt!=ELayoutDirt.Clean && ++cycles < MAX_LAYOUT_CYCLES);
                if (cycles >= MAX_LAYOUT_CYCLES) throw new Exception(string.Format("Aborted Handle_Layout() cycle loop after {0} passes!", cycles));

                onLayout?.Invoke(this);
                if (Box.IsDirty)
                {
                    Box.Rebuild();
                }
            
                onLayoutPost?.Invoke(this);

    #if DEBUG
                if (Debug.Log_Layout_Changes) Logs.Info(xLog.XTERM.cyanBright("[Layout Resolved] {0}"), this);
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

            public async void Set_Parent(cssCompoundElement parent, int index)
            {
                Parent = parent;
                Indice = (parent!=null ? index : 0);
                if (parent != null) Set_Root(parent.Root);// Passed down from generation to generation!
                this.Element_Hierarchy_Changed(0);
                this.Handle_Containing_Block_Dirty();// Our parent element has been changed, so logically our containing-block is now different from what it was.

                this.Style.Flag(EPropertySystemDirtFlags.Cascade);
                // just do it ourselves here and now
                // XXX: if the new element code starts working fine see if we can remove this call, i think its unnecessary
                //await this.Style.Cascade().ConfigureAwait(false);
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
            protected readonly Vec2i Scroll = new Vec2i(0, 0);
            /// <summary>
            /// Returns the total scroll translations for this element, including translations due to it's own scrolling
            /// </summary>
            public virtual Vec2i Get_Scroll_Total()
            {
                Vec2i Total = new Vec2i();
                cssElement E = this;
                do
                {
                    Total += E.Scroll;
                    if (E.Box.IsAbsolutelyPositioned) break;
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
                //if (!string.IsNullOrEmpty(this.id)) Root.Register_Element_ID(this.id, this);
            }
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
                get => hasAttribute("enabled");
                set => toggleAttribute("enabled", value);
            }// = true;

            /// <summary>
            /// Indicates whether the mouse is currently overtop the element
            /// </summary>
            public bool IsMouseOver
            {
                get => hasAttribute("hovered");
                protected set => toggleAttribute("hovered", value);
            }// = false;

            /// <summary>
            /// Indicates whether the primary mouse button is currently pressed on the element
            /// </summary>
            public bool IsActive
            {
                get => hasAttribute("active");
                protected set => toggleAttribute("active", value);
            }// = false;

            /// <summary>
            /// Indicates whether the element can accept drag-drop operations
            /// </summary>
            public bool AcceptsDragDrop
            {
                get => hasAttribute("dropzone");
                protected set => toggleAttribute("dropzone", value);
            }


            /// <summary>
            /// Returns all of the drag-drop object types that this element can accept
            /// </summary>
            public ICollection<string> AcceptedDropTypes
            {
                get
                {
                    if (hasAttribute(EAttributeName.Dropzone, out Attr outAttr))
                    {
                        ICollection<string> spl = DOMCommon.Parse_Ordered_Set(outAttr.Value.Get_String().AsMemory());
                        if (spl == null) spl = new string[] { "*" };
                        return spl;
                    }
                    return new string[] { "*" };
                }
                protected set { setAttribute(EAttributeName.Dropzone, AttributeValue.From_String(DOMCommon.Serialize_Ordered_Set(value.Select(o => o.AsMemory())))); }
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

        private cssElement(Document document) : base(document, CssTagName)
        {
            Box = new CssPrincipalBox(this);

            if (this is cssRootElement)
                Root = (cssRootElement)this;

            // Set default pseudo-state values
            // We do this in the constructor because these are all just accessor for the elements DOM attributes
            IsEnabled = true;
            IsActive = false;
            IsMouseOver = false;
            AcceptsDragDrop = false;

            Box.onChange += Box_onChange;
        }
        /// <summary>
        /// Initializes a new UI element
        /// </summary>
        /// <param name="id">A unique ID</param>
        public cssElement(Document document, IParentElement ParentElement, string className, string id) : this(document)
        {
            if (ReferenceEquals(ParentElement, null) && !(this is cssRootElement))
                throw new ArgumentNullException($"{nameof(ParentElement)} cannot be null!");

            //Setup our logs
            Logs = LogFactory.GetLogger(() => { return id; }, ParentElement?.Logs);

            // Go ahead and assign our ID
            if (string.IsNullOrEmpty(id))
            {
                this.id = UID.ToString();
            }
            else
            {
                if (id.StartsWith("#"))
                {
                    if (id.Length > 1)
                        this.id = id.Remove(0, 1);
                }
                else
                {
                    this.id = id;
                }
            }
            if (!string.IsNullOrEmpty(className))
                this.classList.Add(className);
        
            // We resolve the style once this element is parented, if we do it before, then it cant access it's parents block for layout and causes a null exception
            Style = new ElementPropertySystem(this);
            Style.onProperty_Change += Handle_Style_Property_Change;

            ParentElement?.Add(this);
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

                if (0 != (Style.Dirt & EPropertySystemDirtFlags.Cascade))
                {
                    Style.Cascade();
                }

                if (Box.IsDirty)
                {
                    retVal = true;
                    Box.Rebuild();
                }

                if (LayoutDirt > 0)
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
            public void Set_Visuals_Dirty(bool value)
            {
                if(value)
                {
                    Dirt |= EElementDirtyFlags.Visuals;
                }
                else
                {
                    Dirt &= ~EElementDirtyFlags.Visuals;
                }
            }
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

                Dirt &= ~EElementDirtyFlags.Visuals;
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
                    Root.Engine.Fill_Rect(Box.Padding.Edge);
                }
            }

            internal void Draw_Borders()
            {
                //if (Border.Dirty) Border.Prepare_Texture(Box.Border.Get_Size());

                // The CSS3 docs dont specify how borders are to be joined at the corners.
                // But chromium joins adjacent corners by splitting the corner quad diagonally from its outtermost to its innermost corner and giving each border the closes triangle
                // Which is pretty fair so we'll do that too!
            
                // Draw the horizontal bars without join areas
                int hLeft = this.Box.Border.Left + Border.Left.Size;// Starting point for horizontal border sides            
                int hWidth = Box.Border.Width - (Border.Left.Size + Border.Right.Size);// Width for horizontal border sides
                if (Border.Top.IsVisible)
                {
                    Root.Engine.Set_Color(Border.Top.Color);
                    Root.Engine.Set_Color(cssColor.White);
                    Root.Engine.Fill_Rect(hLeft, Box.Border.Top, hWidth, Border.Top.Size);
                }
                if (Border.Bottom.IsVisible)
                {
                    Root.Engine.Set_Color(Border.Bottom.Color);
                    Root.Engine.Fill_Rect(hLeft, Box.Border.Bottom-Border.Bottom.Size, hWidth, Border.Bottom.Size);
                }
                // Draw the vertical bars without join areas
                int vTop = Box.Border.Top + Border.Top.Size;// Starting point for vertical border sides
                int vHeight = Box.Border.Height - (Border.Top.Size + Border.Bottom.Size);// Height for vertical border sides
                if (Border.Left.IsVisible)
                {
                    Root.Engine.Set_Color(Border.Left.Color);
                    Root.Engine.Fill_Rect(Box.Border.Left, vTop, Border.Left.Size, vHeight);
                }
                if (Border.Right.IsVisible)
                {
                    Root.Engine.Set_Color(Border.Right.Color);
                    Root.Engine.Fill_Rect(Box.Border.Right - Border.Right.Size, vTop, Border.Right.Size, vHeight);
                }
            
                // Draw the join areas
                if (Border.Top.IsVisible && Border.Left.IsVisible)
                {
                    int xo = Box.Border.Left;// x origin
                    int yo = Box.Border.Top;// y origin
                    int xi = Box.Border.Left + Border.Left.Size;// x inner
                    int yi = Box.Border.Top + Border.Top.Size;// y inner

                    // Top Triangle
                    Root.Engine.Set_Color(Border.Top.Color);
                    Root.Engine.Fill_Tri(xi, yi, xi, yo, xo, yo);
                    // Left Triangle
                    Root.Engine.Set_Color(Border.Left.Color);
                    Root.Engine.Fill_Tri(xi, yi, xo, yi, xo, yo);
                }

                if (Border.Top.IsVisible && Border.Right.IsVisible)
                {
                    int xo = Box.Border.Right;// x origin
                    int yo = Box.Border.Top;// y origin
                    int xi = Box.Border.Right - Border.Right.Size;// x inner
                    int yi = Box.Border.Top + Border.Top.Size;// y inner

                    // Top Triangle
                    Root.Engine.Set_Color(Border.Top.Color);
                    Root.Engine.Fill_Tri(xi, yi, xi, yo, xo, yo);
                    // Right Triangle
                    Root.Engine.Set_Color(Border.Right.Color);
                    Root.Engine.Fill_Tri(xi, yi, xo, yi, xo, yo);
                }

                if (Border.Bottom.IsVisible && Border.Left.IsVisible)
                {
                    int xo = Box.Border.Left;// x origin
                    int yo = Box.Border.Bottom;// y origin
                    int xi = Box.Border.Left + Border.Left.Size;// x inner
                    int yi = Box.Border.Bottom - Border.Bottom.Size;// y inner

                    // Bottom Triangle
                    Root.Engine.Set_Color(Border.Bottom.Color);
                    Root.Engine.Fill_Tri(xi, yi, xi, yo, xo, yo);
                    // Left Triangle
                    Root.Engine.Set_Color(Border.Left.Color);
                    Root.Engine.Fill_Tri(xi, yi, xo, yi, xo, yo);
                }

                if (Border.Bottom.IsVisible && Border.Right.IsVisible)
                {
                    int xo = Box.Border.Right;// x origin
                    int yo = Box.Border.Bottom;// y origin
                    int xi = Box.Border.Right - Border.Right.Size;// x inner
                    int yi = Box.Border.Bottom - Border.Bottom.Size;// y inner

                    // Bottom Triangle
                    Root.Engine.Set_Color(Border.Bottom.Color);
                    Root.Engine.Fill_Tri(xi, yi, xi, yo, xo, yo);
                    // Right Triangle
                    Root.Engine.Set_Color(Border.Right.Color);
                    Root.Engine.Fill_Tri(xi, yi, xo, yi, xo, yo);
                }
            }

            private cssTexture dbg_size = null;

            internal void update_debug_text()
            {
                if (dbg_size != null)
                {
                    string dbgTxt = string.Format("{0}x{1}", Box.Width, Box.Height);
                    dbg_size = cssTextElement.Render_Text_String(dbgTxt, "Arial", 18);
                }
            }
            internal void Draw_Debug_Size()
            {
                update_debug_text();

                Root.Engine.Set_Color(1f, 1f, 1f, 1f);
                Root.Engine.Set_Texture(dbg_size);
                Root.Engine.Fill_Rect((Box.X + (Box.Width / 2)) - dbg_size.Size.Width, Box.Y, dbg_size.Size.Width, dbg_size.Size.Height);
                Root.Engine.Set_Texture(null);
            }

            internal virtual void Draw_Debug_Bounds()
            {            
                // Draw the content bounds
                Root.Engine.Set_Color(1f, 0f, 0f, 1f);// Red
                Root.Engine.Draw_Rect(1, Box.Content.Edge);

                // Draw the padding bounds
                Root.Engine.Set_Color(0f, 0f, 1f, 1f);// Blue
                Root.Engine.Draw_Rect(1, Box.Padding.Edge);

                // Draw the border bounds
                Root.Engine.Set_Color(0f, 1f, 0f, 1f);// Green
                Root.Engine.Draw_Rect(1, Box.Border.Edge);
            
                // Draw the block bounds
                Root.Engine.Set_Color(1f, 0.5f, 0f, 1f);// Orange            
                Root.Engine.Draw_Rect(1, Box.Margin.Edge);
            }

            #endregion

        #region Handlers
        private void Box_onChange(ECssBoxType obj)
        {
                this.Invalidate_Layout(EBoxInvalidationReason.Block_Changed);
        }
        #endregion

        #region Point Transformation

            /// <summary>
            /// Transforms a point from absolute(screen) space to the elements local space relative to it's margin-block.
            /// </summary>
            public Vec2i PointToLocal(Vec2i Point)
            {
                return (Point - Box.Get_Position());
            }

            /// <summary>
            /// Transforms a point from local space relative to it's margin-block to absolute screen-space.
            /// </summary>
            public Vec2i PointToScreen(Vec2i Point)
            {
                return (Point + Box.Get_Position());
            }

            /// <summary>
            /// Transforms a point from absolute(screen) space into local space relative to the specified block.
            /// </summary>
            public Vec2i PointToLocal(CssBoxArea area, Vec2i Point)
            {
                return (Point - area.Get_Pos());
            }

            /// <summary>
            /// Transforms a point from local space relative to the specified block into absolute screen-space.
            /// </summary>
            public Vec2i PointToScreen(CssBoxArea area, Vec2i Point)
            {
                return (Point + area.Get_Pos());
            }
        #endregion

        #region Hit Testing
                /// <summary>
                /// Returns true if the given screen-space point intersects this elements click-hitbox
                /// </summary>
                /// <param name="pos"></param>
                /// <returns></returns>
                public bool HitTest_ScreenPos(Vec2i pos)
                {
                    return Box.ClickArea.Intersects(pos + Get_Scroll_Total() - Scroll);
                }

                /// <summary>
                /// Returns the element which intersects the given screen-space point or NULL if none
                /// </summary>
                /// <param name="pos">Screen-Space point to test for intersection with</param>
                public virtual cssElement Get_Hit_Element(Vec2i pos)
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
        public bool HasFocus { get { if (Root == null) return false; else return object.ReferenceEquals(this, Root.FocusedElement); } }
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
