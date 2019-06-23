using System;
using System.Collections.Generic;
using System.Linq;
using CssUI.CSS;
using Logging;

namespace CssUI
{
    /// <summary>
    /// Describes the basis for an element that contains other elements. This is an abstract class
    /// PLEASE USE <see cref="ScrollableElement"/> INSTEAD
    /// </summary>
    public abstract class CompoundElement : uiElement, ICompoundElement
    {
        #region Display
        protected override void Handle_Display_Changed()
        {
            if (Style.Display == EDisplayMode.INLINE)
            {
                Log.Error(new Exception("Complex elements containing other elements cannot use Display: inline, use Inline-Block instead."));
                Style.Default.Display.Set(EDisplayMode.INLINE_BLOCK);
            }
        }
        #endregion

        #region Blocks
        public eSize Get_Layout_Area()
        {
            // We need to account for if MaxSize is set to a real value and if so return it instead of this...
            eSize retVal = new eSize(Block_Containing.Get_Size());
            // We need to act like Block_Containing is our margin-block and find the size of our content-block
            Block_Resize_Border_To_Content(ref retVal.Width, ref retVal.Height);

            if (Style.Max_Width.HasValue) retVal.Width = Style.Max_Width.Value;
            if (Style.Max_Height.HasValue) retVal.Height = Style.Max_Height.Value;

            return retVal;
        }
        
        #endregion

        #region Layout
        /// <summary>
        /// Manages positioning controls automatically according to predetermined rules based on which director type is assigned
        /// </summary>
        public ELayoutMode Layout
        {
            get { return layoutMode; }
            set
            {
                layoutMode = value;
                LayoutBit |= ELayoutBit.Dirty;
                layoutDirector = null;
            }
        }
        ELayoutMode layoutMode = ELayoutMode.None;
        ILayoutDirector layoutDirector = null;

        protected ILayoutDirector Get_Layout()
        {
            if (layoutDirector == null)
            {
                switch (layoutMode)
                {
                    case ELayoutMode.Default:
                        layoutDirector = new Layout_BoxModel();
                        break;
                    case ELayoutMode.Stack:
                        layoutDirector = new Layout_StackModel();
                        break;
                }
            }

            return layoutDirector;
        }
        #endregion

        #region Set Root
        public override void Set_Root(RootElement root)
        {
            base.Set_Root(root);
            foreach (var C in Children)
            {
                C.Set_Root(root);
            }
        }
        #endregion

        #region Constructors
        public CompoundElement(string ID) : base(ID)
        {
            Layout = ELayoutMode.Default;
        }
        #endregion

        #region Destructors
        public override void Dispose()
        {
            Children.Clear();
            base.Dispose();
        }
        #endregion

        #region Updating
        /// <summary>
        /// Updates the Block and Layout if needed and returns True if any updates occured
        /// </summary>
        /// <returns>True/False updates occured</returns>
        public override bool Update()
        {
            bool retVal = false;
            if (base.Update()) retVal = true;

            for (int i = 0; i < Children.Count; i++)
            {
                var C = Children[i];
                if (C.Update()) retVal = true;
            }
            
            return retVal;
        }
        #endregion

        #region Rendering
        /// <summary>
        /// The number of elements with positioning: fixed
        /// </summary>
        protected int Fixed_Element_Count = 0;
        protected override void Draw()
        {
            Render_Children();
        }

        protected virtual void Render_Children()
        {
            for (int i = 0; i < Children.Count; i++)
            {
                var C = Children[i];
                C.Render();
            }
        }

        #endregion

        #region Event Handlers
        public event Action<uiElement, uiElement> onControl_Added;
        public event Action<uiElement, uiElement> onControl_Removed;
        #endregion

        #region Handle Layout

        /// <summary>
        /// Causes the controls to perform layout logic.
        /// </summary>
        protected override void Handle_Layout()
        {
            eBlock Content_Bounds = null;
            // Layout all of our controls
            Content_Bounds = Get_Layout()?.Handle(this, Children.ToArray());

            Fixed_Element_Count = 0;
            // Let our controls update to their new positions
            for (int i = 0; i < Children.Count; i++)
            {
                var C = Children[i];
                if (C.Style.Positioning == EPositioning.Fixed) Fixed_Element_Count++;
                C.PerformLayout();
            }

            // In the case where we had no layout object, find the area our controls occupy manually
            if (Content_Bounds == null) this.Get_Contents_Occupied_Area(out Content_Bounds);

            Style.Set_Content_Width(Content_Bounds.Width);
            Style.Set_Content_Height(Content_Bounds.Height);
            /*
            switch (Style.Display)
            {
                // Block elements size to fill up the entire width of their containing-block
                case EDisplayMode.BLOCK:
                    Style.Set_Content_Height(Content_Bounds.Height);
                    break;
                case EDisplayMode.INLINE_BLOCK:
                    Style.Set_Content_Width(Content_Bounds.Width);
                    Style.Set_Content_Height(Content_Bounds.Height);
                    break;
                // Inline elements size to their exact content dimensions by default, but not to the size of elements they contain...
                case EDisplayMode.INLINE:
                    //Set_Content_Block_Size(Content_Bounds.Width, Content_Bounds.Height);
                    break;
            }
            */
            // Always try to update our block after this because we might have changed size due to our child-elements being positioned
            if (Block.IsDirty) Update_Block();
        }

        /// <summary>
        /// Returns the area which our controls actually inhabit
        /// </summary>
        /// <param name="Contents_Block"></param>
        protected void Get_Contents_Occupied_Area(out eBlock Contents_Block)
        {
            int? Left = null, Top = null;
            int? Right = null, Bottom = null;
            foreach (uiElement C in Children)
            {
                if (!Left.HasValue) Left = C.Block.Left;
                if (!Top.HasValue) Top = C.Block.Top;
                if (!Right.HasValue) Right = C.Block.Right;
                if (!Bottom.HasValue) Bottom = C.Block.Bottom;

                Left = MathExt.Min(C.Block.Left, Left.Value);
                Top = MathExt.Min(C.Block.Top, Top.Value);
                Right = MathExt.Max(C.Block.Right, Right.Value);
                Bottom = MathExt.Max(C.Block.Bottom, Bottom.Value);
            }

            int T = MathExt.Max(0, Top.HasValue ? Top.Value : 0);
            int R = MathExt.Max(0, Right.HasValue ? Right.Value : 0);
            int B = MathExt.Max(0, Bottom.HasValue ? Bottom.Value : 0);
            int L = MathExt.Max(0, Left.HasValue ? Left.Value : 0);
            Contents_Block = eBlock.FromTRBL(T, R, B, L);
        }
        #endregion

        #region Children
        public override bool IsEmpty { get { return (Children.Count == 0); } }
        protected List<uiElement> Children = new List<uiElement>(0);
        public IEnumerable<uiElement> Items { get { return Children.ToArray(); } }

        /// <summary>
        /// Appends every element within this element's hierarchy to the given list
        /// </summary>
        /// <returns></returns>
        internal void Get_All_Descendants(LinkedList<uiElement> List)
        {
            // Queue of elements we need to itterate on
            LinkedList<CompoundElement> Queue = new LinkedList<CompoundElement>();// new LinkedList<CompoundElement>(Children.Where(o => o is CompoundElement) as IEnumerable<CompoundElement>);

            for(int i=0; i<Children.Count; i++)
            {
                uiElement child = Children[i];
                if (child is CompoundElement) Queue.AddLast(child as CompoundElement);

                List.AddLast(child);
            }


            LinkedListNode<CompoundElement> node = Queue.First;
            do
            {
                IEnumerable<uiElement> items = node.Value.Items;
                foreach (uiElement o in items)
                {
                    List.AddLast(o);

                    if (o is CompoundElement) Queue.AddLast(o as CompoundElement);
                }
                node = node.Next;
            }
            while (node != null);
        }

        /// <summary>
        /// Builds and returns a complete list of every element which descends from this one.
        /// </summary>
        /// <returns></returns>
        internal LinkedList<uiElement> Get_All_Descendants()
        {
            LinkedList<uiElement> List = new LinkedList<uiElement>();
            // Queue of elements we need to itterate on
            LinkedList<CompoundElement> Queue = new LinkedList<CompoundElement>();// new LinkedList<CompoundElement>(Children.Where(o => o is CompoundElement) as IEnumerable<CompoundElement>);

            for (int i = 0; i < Children.Count; i++)
            {
                uiElement child = Children[i];
                if (child is CompoundElement) Queue.AddLast(child as CompoundElement);

                List.AddLast(child);
            }


            LinkedListNode<CompoundElement> node = Queue.First;
            do
            {
                IEnumerable<uiElement> items = node.Value.Items;
                foreach (uiElement o in items)
                {
                    List.AddLast(o);

                    if (o is CompoundElement) Queue.AddLast(o as CompoundElement);
                }
                node = node.Next;
            }
            while (node != null);

            return List;
        }

        /// <summary>
        /// Finds all elements in our hierarchy that match the given selector
        /// </summary>
        /// <param name="Selector_String">CSS selector string</param>
        /// <returns><see cref="uiElement"/> or <c>NULL</c></returns>
        public IEnumerable<uiElement> Find(string Selector_String)
        {
            if (string.IsNullOrEmpty(Selector_String)) throw new ArgumentNullException("Selector cannot be NULL!");
            // TODO: Implement a full CSS selector parsing system
            return Find(new CssSelector(Selector_String));
        }

        /// <summary>
        /// Finds all elements in our hierarchy that match the given selector
        /// </summary>
        /// <returns><see cref="uiElement"/> or <c>NULL</c></returns>
        public IEnumerable<uiElement> Find(CssSelector Selector)
        {
            if (Selector == null) throw new ArgumentNullException("Selector cannot be NULL!");
            List<uiElement> list = new List<uiElement>();

            foreach (var C in Children)
            {
                if (Selector.QuerySingle(C)) list.Add(C);
                if (C is CompoundElement)
                {
                    list.AddRange(((CompoundElement)C).Find(Selector));
                }
            }

            return list;
        }

        /// <summary>
        /// Adds an element to this one
        /// </summary>
        protected void Add(uiElement element)
        {
            if (Children.Contains(element)) throw new Exception("Cannot add element which is already present!");
            if (Root != null && !string.IsNullOrEmpty(element.ID) && Root.Find_ID(element.ID) != null) throw new Exception("Cannot add element, another element with the same ID already exists!");

            Children.Add(element);
            element.Set_Parent(this, Children.Count-1);
            onControl_Added?.Invoke(this, element);
        }

        /// <summary>
        /// Removes a given element from this one
        /// </summary>
        /// <param name="element">The element to be removed</param>
        /// <param name="preserve">If TRUE then the element will not be disposed of immediately</param>
        /// <returns>Success</returns>
        protected bool Remove(uiElement element, bool preserve = false)
        {
            if (element == null) return false;
            if (!Children.Contains(element)) return false;
            if (Children.Remove(element))
            {
                element.Set_Root(null);
                element.Set_Parent(null, 0);
                onControl_Removed?.Invoke(this, element);

                if (!preserve)
                {
                    element.Dispose();
                    element = null;
                }
                return true;
            }

            return false;
        }

        /// <summary>
        /// Removes a given element, specified by it's ID, from this one 
        /// </summary>
        /// <param name="ID">ID of the element to be removed</param>
        /// <param name="preserve">If TRUE then the element will not be disposed of immediately</param>
        /// <returns>Success</returns>
        protected bool Remove(string ID, bool preserve = false)
        {
            return Remove(Get(ID), preserve);
        }

        /// <summary>
        /// Clears and disposes of all controls
        /// </summary>
        protected void Clear()
        {
            foreach (var C in Children)
            {
                C.Set_Root(null);
                C.Set_Parent(null, 0);
                C.Dispose();
            }
            this.Children.Clear();
        }
        
        /// <summary>
        /// Fetches the first child-element matching a given CSS selector
        /// </summary>
        protected uiElement Get(string Selector)
        {
            return Find(Selector).SingleOrDefault();
        }

        /// <summary>
        /// Fetches the first child-element matching a given CSS selector
        /// </summary>
        protected Ty Get<Ty>(string Selector) where Ty : uiElement
        {
            return (Ty)Find(Selector).SingleOrDefault();
        }

        #endregion

        #region Propagation

        public override void Element_Hierarchy_Changed(int Depth)
        {
            base.Element_Hierarchy_Changed(Depth);

            int dist = Depth + 1;
            for (int i = 0; i < Children.Count; i++)
            {
                var C = Children[i];
                C.Element_Hierarchy_Changed(dist);
            }
        }

        /// <summary>
        /// Flags the blocks of all children who are dependent on us, as dirty
        /// </summary>
        protected void Flag_All_Children(EUIInvalidationReason Reason)
        {
            for (int i = 0; i < Children.Count; i++)
            {
                var C = Children[i];
                C.Flag_Block_Dirty(Reason);
            }
        }

        /// <summary>
        /// Flags the blocks of all children who are dependent on us, as dirty
        /// </summary>
        protected void Flag_Dependent_Children()
        {
            for (int i = 0; i < Children.Count; i++)
            {
                var C = Children[i];
                C.Flag_Containing_Block_Dirty();
            }
        }

        /*
         * // Just because our containing block changed doesnt mean our children need to change. (Thats wasteful)
        public override void Flag_Containing_Block_Dirty()
        {
            base.Flag_Containing_Block_Dirty();
            Flag_Dependent_Children();
        }
        */

        public override void Flag_Block_Dirty(EUIInvalidationReason Reason)
        {
            base.Flag_Block_Dirty(Reason);
            Flag_Dependent_Children();
        }

        /// <summary>
        /// Occurs after <see cref="Update_Cached_Blocks"/>
        /// </summary>
        protected override void Handle_Moved(ePos oldPos, ePos newPos)
        {
            base.Handle_Moved(oldPos, newPos);
            //Flag_All_Children(); // Redacted on 06-19-2017 "Why would child elements need to change their block because the parent moved?"
        }

        /// <summary>
        /// Occurs after <see cref="Update_Cached_Blocks"/>
        /// </summary>
        protected override void Handle_Resized(eSize oldSize, eSize newSize)
        {
            base.Handle_Resized(oldSize, newSize);
            Flag_Dependent_Children();
        }

        protected override void Handle_Content_Block_Change()
        {
            base.Handle_Content_Block_Change();
            //Flag_All_Children(); // Redacted on 06-19-2017 "Why would child elements need to change their block because the parent's content-block changed?"
        }
        #endregion

        #region Hit Testing

        /// <summary>
        /// Returns the element which intersects the given screen-space point or NULL if none
        /// </summary>
        /// <param name="pos">Screen-Space point to test for intersection with</param>
        public override uiElement Get_Hit_Element(ePos pos)
        {
            for (int i = Children.Count; i > 0; i--)// Traverse backwards to obey drawing order
            {
                var C = Children[i - 1];
                if (C.HitTest_ScreenPos(pos))
                {
                    return C;
                }
            }

            return base.Get_Hit_Element(pos);
        }

        /// <summary>
        /// Takes a local-space point and returns all the child elements that it intersects
        /// </summary>
        /// <param name="pos"></param>
        /// <returns></returns>
        List<uiElement> Get_Children_Hit(ePos pos)
        {
            List<uiElement> list = new List<uiElement>();
            for (int i = Children.Count; i > 0; i--)// Traverse backwards to obey drawing order
            {
                var C = Children[i - 1];
                if (C.HitTest_ScreenPos(pos))
                {
                    list.Add(C);
                }
            }
            return list;
        }

        #endregion
        

        #region Keyboard Event Handlers

        /// <summary>
        /// Called whenever the user presses a character key while the element has input-focus
        /// </summary>
        public override bool Handle_KeyPress(uiElement Sender, DomKeyboardKeyEventArgs Args)
        {
            if (!base.Handle_KeyPress(Sender, Args)) return false;// Abort
            // Pass the event to all applicable child-elements
            for (int i = 0; i < Children.Count; i++)
            {
                var C = Children[i];
                C.Handle_KeyPress(this, Args);
            }

            return true;
        }

        /// <summary>
        /// Called whenever a keyboard key is depressed while the element has input-focus
        /// </summary>
        public override bool Handle_KeyUp(uiElement Sender, DomKeyboardKeyEventArgs Args)
        {
            if (!base.Handle_KeyUp(Sender, Args)) return false;// Abort
            // Pass the event to all applicable child-elements
            for (int i = 0; i < Children.Count; i++)
            {
                var C = Children[i];
                C.Handle_KeyUp(this, Args);
            }

            return true;
        }

        /// <summary>
        /// Called whenever a keyboard key is pressed while the element has input-focus
        /// </summary>
        public override bool Handle_KeyDown(uiElement Sender, DomKeyboardKeyEventArgs Args)
        {
            if (!base.Handle_KeyDown(Sender, Args)) return false;// Abort
            // Pass the event to all applicable child-elements
            for (int i = 0; i < Children.Count; i++)
            {
                var C = Children[i];
                C.Handle_KeyDown(this, Args);
            }

            return true;
        }
        #endregion
        

        #region Tunneling Events

        /*
        /// <summary>
        /// Called whenever the mouse releases a button whilst over the element
        /// </summary>
        /// <param name="Sender">The UI element sending us this event, or NULL if we are the first element to receive it.</param>
        public override void Handle_PreviewMouseUp(uiElement Sender, PreviewMouseButtonEventArgs Args)
        {
            base.Handle_PreviewMouseUp(this, Args);
            if (Args.Handled) return;
            // Pass the event to all applicable child-elements
            List<uiElement> Hits = Get_Children_Hit(Args.Position);
            foreach (var C in Hits)
            {
                C.Handle_PreviewMouseUp(this, Args);
                if (Args.Handled) return;
            }
        }
        /// <summary>
        /// Called whenever the mouse presses a button whilst over the element
        /// </summary>
        /// <param name="Sender">The UI element sending us this event, or NULL if we are the first element to receive it.</param>
        public override void Handle_PreviewMouseDown(uiElement Sender, PreviewMouseButtonEventArgs Args)
        {
            base.Handle_PreviewMouseDown(this, Args);
            if (Args.Handled) return;
            // Pass the event to all applicable child-elements
            List<uiElement> Hits = Get_Children_Hit(Args.Position);
            foreach (var C in Hits)
            {
                C.Handle_PreviewMouseDown(this, Args);
                if (Args.Handled) return;
            }

        }

        /// <summary>
        /// Called whenever the mouse wheel scrolls whilst over the element
        /// </summary>
        /// <param name="Sender">The UI element sending us this event, or NULL if we are the first element to receive it.</param>
        public override void Handle_PreviewMouseWheel(uiElement Sender, PreviewMouseWheelEventArgs Args)
        {
            base.Handle_PreviewMouseWheel(this, Args);
            if (Args.Handled) return;
            // Pass the event to all applicable child-elements
            List<uiElement> Hits = Get_Children_Hit(Args.Position);
            foreach (var C in Hits)
            {
                C.Handle_PreviewMouseWheel(this, Args);
                if (Args.Handled) return;
            }

        }
        /// <summary>
        /// Called whenever the mouse moves whilst over the element.
        /// <para>Fires after MouseEnter</para>
        /// </summary>
        /// <param name="Sender">The UI element sending us this event, or NULL if we are the first element to receive it.</param>
        public override void Handle_PreviewMouseMove(uiElement Sender, PreviewMouseMoveEventArgs Args)
        {
            base.Handle_PreviewMouseMove(this, Args);
            if (Args.Handled) return;
            // Pass the event to all applicable child-elements
            List<uiElement> Hits = Get_Children_Hit(Args.Position);
            foreach (var C in Hits)
            {
                C.Handle_PreviewMouseMove(this, Args);
                if (Args.Handled) return;
            }

        }

        /// <summary>
        /// Called whenever the mouse moves onto the element.
        /// <para>Fires before MouseMove</para>
        /// </summary>
        /// <param name="Sender">The UI element sending us this event, or NULL if we are the first element to receive it.</param>
        public override void Handle_PreviewMouseEnter(uiElement Sender, PreviewMouseEventArgs Args)
        {
            base.Handle_PreviewMouseEnter(this, Args);
            if (Args.Handled) return;
            // Pass the event to all applicable child-elements
            List<uiElement> Hits = Get_Children_Hit(Args.Position);
            foreach (var C in Hits)
            {
                C.Handle_PreviewMouseEnter(this, Args);
                if (Args.Handled) return;
            }

        }
        /// <summary>
        /// Called whenever the mouse moves out of the element
        /// </summary>
        /// <param name="Sender">The UI element sending us this event, or NULL if we are the first element to receive it.</param>
        public override void Handle_PreviewMouseLeave(uiElement Sender, PreviewMouseEventArgs Args)
        {
            base.Handle_PreviewMouseLeave(this, Args);
            if (Args.Handled) return;
            // Pass event to all applicable children
            foreach(var C in Children)
            {
                if (C.IsMousedOver) C.Handle_PreviewMouseLeave(this, Args);
                if (Args.Handled) return;
            }
        }

        /// <summary>
        /// Called whenever the element is 'clicked' by mouse input or otherwise
        /// </summary>
        /// <param name="Sender">The UI element sending us this event, or NULL if we are the first element to receive it.</param>
        public override void Handle_PreviewClick(uiElement Sender, PreviewEventArgs Args)
        {
            base.Handle_PreviewClick(this, Args);
            if (Args.Handled) return;
            // Pass event to all children
            foreach (var C in Children)
            {
                C.Handle_PreviewClick(this, Args);
                if (Args.Handled) return;
            }
        }
        /// <summary>
        /// Called whenever the element is 'double clicked' by mouse input or otherwise
        /// </summary>
        /// <param name="Sender">The UI element sending us this event, or NULL if we are the first element to receive it.</param>
        public override void Handle_PreviewDoubleClick(uiElement Sender, PreviewEventArgs Args)
        {
            base.Handle_PreviewDoubleClick(this, Args);
            if (Args.Handled) return;
            // Pass event to all children
            foreach (var C in Children)
            {
                C.Handle_PreviewDoubleClick(this, Args);
                if (Args.Handled) return;
            }
        }

        /// <summary>
        /// Called whenever the mouse clicks the element.
        /// Two single clicks that occur close enough in time, as determined by the mouse settings of the user's operating system, will generate a MouseDoubleClick event instead of the second MouseClick event.
        /// </summary>
        /// <param name="Sender">The UI element sending us this event, or NULL if we are the first element to receive it.</param>
        public override void Handle_PreviewMouseClick(uiElement Sender, PreviewMouseButtonEventArgs Args)
        {
            base.Handle_PreviewMouseClick(this, Args);
            if (Args.Handled) return;
            // Pass the event to all applicable child-elements
            List<uiElement> Hits = Get_Children_Hit(Args.Position);
            foreach (var C in Hits)
            {
                C.Handle_PreviewMouseClick(this, Args);
                if (Args.Handled) return;
            }

        }
        /// <summary>
        /// Called whenever the mouse double clicks the element
        /// </summary>
        /// <param name="Sender">The UI element sending us this event, or NULL if we are the first element to receive it.</param>
        public override void Handle_PreviewMouseDoubleClick(uiElement Sender, PreviewMouseButtonEventArgs Args)
        {
            base.Handle_PreviewMouseDoubleClick(this, Args);
            if (Args.Handled) return;
            // Pass the event to all applicable child-elements
            List<uiElement> Hits = Get_Children_Hit(Args.Position);
            foreach (var C in Hits)
            {
                C.Handle_PreviewMouseDoubleClick(this, Args);
                if (Args.Handled) return;
            }

        }
        */

        #endregion

    }
}
