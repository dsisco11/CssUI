using System;
using System.Collections.Generic;
using CssUI.CSS;
using System.Threading.Tasks;

namespace CssUI
{
    // TO BE CLEAR: I've kept the implementation of child-controls in a seperate class from the base control class so it is more clear which controls actually support having child-controls
    // For instance; it wouldnt make sense if a TextArea element could have children

    /// <summary>
    /// Provides the basis for elements that can contain other elements and CAN handle scrolling
    /// <para>For elements which allow external sources to add child-elements to them use <see cref="cssContainerElement"/> instead</para>
    /// </summary>
    public abstract class cssScrollableElement : cssCompoundElement
    {
        #region Blocks
        void Update_ScrollViewport_X()
        {
            Block_ScrollViewport.X = (Block_Padding.X + SB_Horizontal.Value);
            Update_RenderList();
        }

        void Update_ScrollViewport_Y()
        {
            Block_ScrollViewport.Y = (Block_Padding.Y + SB_Vertical.Value);
            Update_RenderList();
        }

        protected override void Update_Cached_Blocks()
        {
            base.Update_Cached_Blocks();

            if (!IsScrollContainer)
            {
                Block_Clipping = null;
                Block_Scroll = null;
                Block_ScrollViewport = null;

                Update_Vertical_Scrollbar(false);
                Update_Horizontal_Scrollbar(false);
            }
            else
            {
                Block_Clipping = new eBlock(Block_Padding);

                int cX = Block_Padding.Width;
                int cY = Block_Padding.Height;
                if (Style.Content_Width.HasValue || Style.Content_Height.HasValue)
                {
                    if (Style.Content_Width.HasValue) cX = Style.Content_Width.Value;
                    if (Style.Content_Height.HasValue) cY = Style.Content_Height.Value;
                    Block_Resize_Padding_To_Content(ref cX, ref cY);
                    if (Style.Content_Width.HasValue) cX += Style.Padding_Horizontal;// (Style.Padding_Left + Style.Padding_Right);
                    if (Style.Content_Height.HasValue) cY += Style.Padding_Vertical;// (Style.Padding_Top + Style.Padding_Bottom);
                }

                Block_Scroll = new eBlock(Block_Padding.Get_Pos(), new eSize(cX, cY));
                if (Block_ScrollViewport == null)
                    Block_ScrollViewport = new eBlock(new ePos(Block_Padding.Get_Pos()), new eSize(Block_Padding.Get_Size()));
                else
                {
                    Block_ScrollViewport.X = Block_Padding.X;
                    Block_ScrollViewport.Y = Block_Padding.Y;
                    Block_ScrollViewport.Width = Block_Padding.Width;
                    Block_ScrollViewport.Height = Block_Padding.Height;
                }
                // Update our scrollbars
                bool need_vertical = (Style.Overflow_Y != EOverflowMode.Visible && Block_Padding.Height < Block_Scroll.Height);
                bool need_horizontal = (Style.Overflow_X != EOverflowMode.Visible && Block_Padding.Width < Block_Scroll.Width);

                Update_Vertical_Scrollbar(need_vertical);
                Update_Horizontal_Scrollbar(need_horizontal);
            }
        }
        
        #endregion

        #region Constructors
        public cssScrollableElement(IParentElement Parent, string ID) : base(Parent, ID)
        {
        }
        #endregion

        #region Destructors
        public override void Dispose()
        {
            Children.Clear();
            base.Dispose();
        }
        #endregion

        #region Events
        /// <summary>
        /// Occurs when the element is being scrolled.
        /// </summary>
        public event Action<cssScrollableElement, cssScrollBarElement> onScroll;
        #endregion

        #region Scroll Translations
        /// <summary>
        /// Matrix used to apply scrollbar translations to the elements contents during rendering.
        /// </summary>
        private eMatrix scrollMatrix = new eMatrix();
        /// <summary>
        /// Matrix used when rendering the actual scrollbard
        /// </summary>
        private eMatrix scrollbarMatrix = new eMatrix();

        /// <summary>
        /// Fires whenever this elements horizontal or vertical scrollbar values changes.
        /// </summary>
        public void Handle_Scrolled()
        {
            var p = Get_Scroll_Offset();
            Scroll.X = p.X;
            Scroll.Y = p.Y;
            scrollMatrix.Set_Translation(-p.X, -p.Y, 0);
            scrollbarMatrix.Set_Translation(p.X, p.Y, 0);
            // Simulate a mouse movement so elements underneath the mouse update their states
            Root?.Fire_Dummy_MouseMove();
        }

        ePos Get_Scroll_Offset()
        {
            int x = 0, y = 0;
            if (SB_Horizontal != null) x = SB_Horizontal.Value;
            if (SB_Vertical != null) y = SB_Vertical.Value;
            return new ePos(x, y);
        }
        #endregion

        #region Scrollbars
        /// <summary>
        /// The total area that our scrollbars represent
        /// AKA: the "Scrollbable Overflow Region"
        /// </summary>
        private eBlock Block_Scroll = null;
        /// <summary>
        /// Stores the current absolute-space area of this elements contents which is visible due to scrolling.
        /// </summary>
        private eBlock Block_ScrollViewport = null;
        private uiVScrollBar SB_Vertical = null;
        private uiHScrollBar SB_Horizontal = null;


        private void Update_Vertical_Scrollbar(bool is_needed)
        {
            switch (Style.Overflow_Y)
            {
                case EOverflowMode.Visible:
                case EOverflowMode.Clip:
                    {
                        Remove_Vertical_Scrollbar();
                        return;
                    }
                case EOverflowMode.Auto:// Scrollbar only present when content extends beyond clipping area
                    {
                        if (is_needed) Ensure_Vertical_Scrollbar();
                        else
                        {
                            Remove_Vertical_Scrollbar();
                            return;
                        }
                    }
                    break;
                case EOverflowMode.Scroll:// Scrollbar always present
                case EOverflowMode.Hidden:// Scrollbar hidden but can still scroll
                    {
                        Ensure_Vertical_Scrollbar();
                    }
                    break;
            }

            SB_Vertical.ValueMax = (Block_Scroll.Height - Viewport.Block.Height) + 1;// Set how many pixels we can scroll
            SB_Vertical.StepSize = Style.LineHeight;
            // We sit the vertical scrollbar at the padding block's right edge because the ScrollBar_Offset is set to it's width and is accounted for when calculating the padding block's size. So there is already room for us between the padding and border blocks!
            /*
            SB_Vertical.Style.Default.X.Set(Block_Padding.Right);
            SB_Vertical.Style.Default.Y.Set(Block_Padding.Top);
            SB_Vertical.Style.Default.Height.Set(Block_Padding.Height);
            */

            Update_ScrollViewport_Y();

            if (Style.Overflow_Y == EOverflowMode.Scroll || (Style.Overflow_Y == EOverflowMode.Auto && is_needed))
                Scrollbar_Offset.Right = SB_Vertical.Block.Width;
            else
                Scrollbar_Offset.Right = 0;
        }

        private void Update_Horizontal_Scrollbar(bool is_needed)
        {
            switch (Style.Overflow_X)
            {
                case EOverflowMode.Visible:
                case EOverflowMode.Clip:
                    {
                        Remove_Horizontal_Scrollbar();
                        return;
                    }
                case EOverflowMode.Auto:// Scrollbar only present when content extends beyond clipping area
                    {
                        if (is_needed) Ensure_Horizontal_Scrollbar();
                        else
                        {
                            Remove_Horizontal_Scrollbar();
                            return;
                        }
                    }
                    break;
                case EOverflowMode.Scroll:// Scrollbar always present
                case EOverflowMode.Hidden:// Scrollbar hidden but can still scroll
                    {
                        Ensure_Horizontal_Scrollbar();
                    }
                    break;
            }

            SB_Horizontal.ValueMax = (Block_Scroll.Width - Viewport.Block.Width) + 1;// Set how many pixels we can scroll
            SB_Horizontal.StepSize = Style.LineHeight;
            /*
            SB_Horizontal.Style.Default.X.Set(Block_Padding.Left);
            SB_Horizontal.Style.Default.Y.Set(Block_Padding.Bottom);
            SB_Horizontal.Style.Default.Width.Set(Block_Padding.Width);
            */
            Update_ScrollViewport_X();

            if (Style.Overflow_X == EOverflowMode.Scroll || (Style.Overflow_X == EOverflowMode.Auto && is_needed))
                Scrollbar_Offset.Bottom = SB_Horizontal.Block.Height;
            else
                Scrollbar_Offset.Bottom = 0;
        }

        /// <summary>
        /// Ensures that the element has a vertical scrollbar instantiated
        /// </summary>
        private void Ensure_Vertical_Scrollbar()
        {
            if (SB_Vertical == null)
            {
                SB_Vertical = new uiVScrollBar(this);
                SB_Vertical.Style.ImplicitRules.Height.Set(CssValue.From_Percent(100f));
                
                SB_Vertical.Style.ImplicitRules.Top.Set(0);
                SB_Vertical.Style.ImplicitRules.Bottom.Set(0);
                SB_Vertical.Style.ImplicitRules.Right.Set(0);
                
                SB_Vertical.IsVisible = false;
                SB_Vertical.ValueChanged += delegate (cssScrollBarElement Sender) 
                {
                    Update_ScrollViewport_Y();
                    Handle_Scrolled();
                    onScroll?.Invoke(this, SB_Vertical);
                };
            }
        }
        /// <summary>
        /// Removes the elements vertical scrollbar
        /// </summary>
        private void Remove_Vertical_Scrollbar()
        {
            if (SB_Vertical != null)
            {
                Remove(SB_Vertical);
                Scrollbar_Offset.Right = 0;
                SB_Vertical = null;
            }
        }
        /// <summary>
        /// Ensures that the element has a horizontal scrollbar instantiated
        /// </summary>
        private void Ensure_Horizontal_Scrollbar()
        {
            if (SB_Horizontal == null)
            {
                SB_Horizontal = new uiHScrollBar(this);
                SB_Horizontal.Style.ImplicitRules.Width.Set(CssValue.From_Percent(100f));

                SB_Horizontal.Style.ImplicitRules.Left.Set(0);
                SB_Horizontal.Style.ImplicitRules.Right.Set(0);
                SB_Horizontal.Style.ImplicitRules.Bottom.Set(0);

                SB_Horizontal.IsVisible = false;
                SB_Horizontal.ValueChanged += delegate (cssScrollBarElement Sender) 
                {
                    Update_ScrollViewport_X();
                    Handle_Scrolled();
                    onScroll?.Invoke(this, SB_Horizontal);
                };
            }
        }
        /// <summary>
        /// Removes the elements horizontal scrollbar
        /// </summary>
        private void Remove_Horizontal_Scrollbar()
        {
            if (SB_Horizontal != null)
            {
                Remove(SB_Horizontal);
                Scrollbar_Offset.Bottom = 0;
                SB_Horizontal = null;
            }
        }
        #endregion

        /// <summary>
        /// Scrolls the viewport so the specified element is within it.
        /// </summary>
        /// <param name="E"></param>
        public void ScrollIntoView(cssElement E)
        {// TODO: FINISH ScrollIntoView(uiElement) function!
            if (!E.IsChildOf(this)) return;


        }
        

        #region Updating
        /// <summary>
        /// Updates the Block and Layout if needed and returns True if any updates occured
        /// </summary>
        /// <returns>True/False updates occured</returns>
        public override async Task<bool> Update()
        {
            bool retVal = false;
            if (await base.Update()) retVal = true;
            // Our base Update() function already handles calling Update() on our child-elements
            /*
            for (int i = 0; i < Children.Count; i++)
            {
                var C = Children[i];
                if (C.Update()) retVal = true;
            }
            */
            if (retVal) Update_RenderList();
            return retVal;
        }

        protected void Update_RenderList()
        {
            if (!IsScrollContainer)
            {
                RenderList = null;// No render list
                return;
            }

            List<int> list = new List<int>(0);
            for (int i = 0; i < Children.Count; i++)
            {
                var C = Children[i];
                if (!C.ShouldRender) continue;
                if (Block_ScrollViewport.Intersects(C.Block))
                    list.Add(i);
            }
            RenderList = list.ToArray();
        }
        #endregion

        #region Rendering
        /// <summary>
        /// List of elements which currently fall within visible bounds and should attempt to be rendered.
        /// If NULL then render all elements
        /// </summary>
        int[] RenderList = null;

        /// <summary>
        /// Attempts to render the element
        /// </summary>
        /// <param name="force">Forces rendering even if the element would not normally be rendered</param>
        /// <returns>Success</returns>
        public override bool Render(bool force=false)
        {
            if (base.Render(force) == false) return false;
            
            if (Debug.Draw_Child_Bounds)
            {
                // Draw all of our child controls
                Root.Engine.Push();
                Root.Engine.Set_Matrix(scrollMatrix);
                for (int i = 0; i < Children.Count; i++)
                {
                    var C = Children[i];
                    C.Draw_Debug_Bounds();
                }
                Root.Engine.Pop();
            }

            return true;
        }

        internal override void Draw_Debug_Bounds()
        {
            base.Draw_Debug_Bounds();

            if (Block_Scroll != null)
            {
                Root.Engine.Set_Color(0f, 1f, 1f, 1f);
                Root.Engine.Draw_Rect(1, Block_Scroll);
            }
        }

        protected override void Draw()
        {
            Render_Children();

            if (SB_Vertical != null || SB_Horizontal != null)
            {
                // Because the clipping region for a control is it's padding block, and our scrollbars are (by the standards) supposed to occupy the space between the padding and border blocks
                // They will normally be clipped and thus not drawn, so we do it manually...
                if (Style.Overflow_Y != EOverflowMode.Hidden && SB_Vertical != null) SB_Vertical.Render(true);
                if (Style.Overflow_X != EOverflowMode.Hidden && SB_Horizontal != null) SB_Horizontal.Render(true);
            }
        }

        protected override void Render_Children()
        {
            if (Fixed_Element_Count > 0) Render_With_Fixed_Elements();
            else Render_Without_Fixed_Elements();
        }

        /// <summary>
        /// Optimized rendering logic for when this element contains no fixed elements
        /// </summary>
        void Render_Without_Fixed_Elements()
        {
            Render_Children_NoCheck();
        }

        /// <summary>
        /// Rendering logic for when the element contains fixed elements
        /// </summary>
        void Render_With_Fixed_Elements()
        {
            // Draw all of our child controls
            Root.Engine.Push();
            Root.Engine.Set_Matrix(scrollMatrix);

            // Begin drawing non-fixed elements
            Render_Children_Check_Positioning_Fixed(false);
            
            // End drawing non-fixed elements
            Root.Engine.Pop();
            // Begin drawing fixed elements
            Render_Children_Check_Positioning_Fixed(true);
        }

        void Render_Children_NoCheck()
        {
            if (RenderList != null)
            {
                for (int i = 0; i < RenderList.Length; i++)
                {
                    int idx = RenderList[i];
                    var C = Children[idx];
                    C.Render();
                }
            }
            else
            {
                for (int i = 0; i < Children.Count; i++)
                {
                    var C = Children[i];
                    C.Render();
                }
            }
        }

        void Render_Children_Check_Positioning_Fixed(bool eq)
        {
            switch (eq)
            {
                case false:
                    {
                        if (RenderList != null)
                        {
                            for (int i = 0; i < RenderList.Length; i++)
                            {
                                int idx = RenderList[i];
                                var C = Children[idx];
                                if (C.Style.Positioning != EPositioning.Fixed)
                                    C.Render();
                            }
                        }
                        else
                        {
                            for (int i = 0; i < Children.Count; i++)
                            {
                                var C = Children[i];
                                if (C.Style.Positioning != EPositioning.Fixed)
                                    C.Render();
                            }
                        }
                    }
                    break;
                default:
                    {
                        if (RenderList != null)
                        {
                            for (int i = 0; i < RenderList.Length; i++)
                            {
                                int idx = RenderList[i];
                                var C = Children[idx];
                                if (C.Style.Positioning == EPositioning.Fixed)
                                    C.Render();
                            }
                        }
                        else
                        {
                            for (int i = 0; i < Children.Count; i++)
                            {
                                var C = Children[i];
                                if (C.Style.Positioning == EPositioning.Fixed)
                                    C.Render();
                            }
                        }
                    }
                    break;
            }
        }

        #endregion

        #region doLayout
        /// <summary>
        /// Causes the controls to perform layout logic.
        /// </summary>
        protected override void Handle_Layout()
        {
            base.Handle_Layout();
        }
        #endregion
        
        #region Mouse Event Handlers        
        /// <summary>
        /// Called whenever the mouse wheel scrolls whilst over the element
        /// </summary>
        /// <param name="Sender">The UI element sending us this event, or NULL if we are the first element to receive it.</param>
        public override void Handle_MouseWheel(cssElement Sender, DomMouseWheelEventArgs Args)
        {
            if (SB_Vertical != null) SB_Vertical.Value = (SB_Vertical.Value - Args.Delta);
            base.Handle_MouseWheel(Sender, Args);
        }
        
        #endregion
    }
}
