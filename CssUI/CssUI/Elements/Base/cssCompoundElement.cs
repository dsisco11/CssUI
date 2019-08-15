using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using CssUI.CSS;
using CssUI.DOM;
using CssUI.Enums;
using xLog;

namespace CssUI
{
    /// <summary>
    /// Describes the basis for an element that contains other elements. This is an abstract class
    /// PLEASE USE <see cref="cssScrollableElement"/> INSTEAD
    /// </summary>
    public abstract class cssCompoundElement : cssElement, IParentElement, ICollection<cssElement>
    {
        #region Accessors
        new public ILogger Logs { get; }
        #endregion

        #region Display
        protected override void Handle_Display_Changed()
        {
            if (Style.Display == EDisplayMode.INLINE)
            {
                Log.Error(new Exception("Complex elements containing other elements cannot use Display: inline, use Inline-Block instead."));
                Style.ImplicitRules.Display.Set(EDisplayMode.INLINE_BLOCK);
            }
        }
        #endregion

        #region Blocks
        public Size2D Get_Layout_Area()
        {
            return Box.Content.Get_Dimensions();
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
                LayoutDirt |= ELayoutDirt.Dirty;
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
        public override void Set_Root(cssRootElement root)
        {
            base.Set_Root(root);
            foreach (var C in Children)
            {
                C.Set_Root(root);
            }
        }
        #endregion

        #region Constructors
        public cssCompoundElement(Document document, IParentElement Parent, string className, string ID) : base(document, Parent, className, ID)
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
        /// <returns>True/False whether updates occured</returns>
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
        public event Action<cssElement, cssElement> onControl_Added;
        public event Action<cssElement, cssElement> onControl_Removed;

        protected override async void Handle_Style_Property_Change(ICssProperty Property, EPropertyDirtFlags Flags, StackTrace Source)
        {
            base.Handle_Style_Property_Change(Property, Flags, Source);

            // If this property inherits by default then Let all of our children know
            if (Property.Definition.Inherited)
            {
                if (!this.IsEmpty)
                {
                    AsyncCountdownEvent ctdn = new AsyncCountdownEvent(this.Count);
                    // I dont think it will matter in what order the children update their own blocks or anything
                    Parallel.For(0, this.Count, async (int i) =>
                    {
                        await this[i].Style.Handle_Inherited_Property_Change_In_Hierarchy(this, Property);
                        ctdn.Signal();
                    });

                    await ctdn.WaitAsync();
                }
            }
        }
        #endregion

        #region Handle Layout

        /// <summary>
        /// Causes the controls to perform layout logic.
        /// </summary>
        protected override void Handle_Layout()
        {
            CssBoxArea Content_Bounds = null;
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
            Box.Min_Content = new Size2D(Content_Bounds.Width, Content_Bounds.Height);
            // Always try to update our block after this because we might have changed size due to our child-elements being positioned
            Box.Rebuild();
        }

        /// <summary>
        /// Returns the area which our controls actually inhabit
        /// </summary>
        /// <param name="Contents_Block"></param>
        protected void Get_Contents_Occupied_Area(out CssBoxArea Contents_Block)
        {
            int? Left = null, Top = null;
            int? Right = null, Bottom = null;
            foreach (cssElement C in Children)
            {
                if (!Top.HasValue) Top = C.Box.Top;
                if (!Right.HasValue) Right = C.Box.Right;
                if (!Bottom.HasValue) Bottom = C.Box.Bottom;
                if (!Left.HasValue) Left = C.Box.Left;

                Top = MathExt.Min(C.Box.Top, Top.Value);
                Right = MathExt.Max(C.Box.Right, Right.Value);
                Bottom = MathExt.Max(C.Box.Bottom, Bottom.Value);
                Left = MathExt.Min(C.Box.Left, Left.Value);
            }

            int T = MathExt.Max(0, Top.HasValue ? Top.Value : 0);
            int R = MathExt.Max(0, Right.HasValue ? Right.Value : 0);
            int B = MathExt.Max(0, Bottom.HasValue ? Bottom.Value : 0);
            int L = MathExt.Max(0, Left.HasValue ? Left.Value : 0);
            Contents_Block = new CssBoxArea(T, R, B, L);
        }
        #endregion

        #region Children
        /// <summary>
        /// Returns True/False if this element has children
        /// </summary>
        public override bool IsEmpty { get { return (Children.Count == 0); } }
        protected List<cssElement> Children = new List<cssElement>(0);
        public IEnumerable<cssElement> Items { get { return Children.ToList(); } }

        public int Count => ((IList<cssElement>)Children).Count;

        public bool IsReadOnly => ((IList<cssElement>)Children).IsReadOnly;

        public cssElement this[int index] { get => ((IList<cssElement>)Children)[index]; }

        /// <summary>
        /// Appends every element within this element's hierarchy to the given list
        /// </summary>
        /// <returns></returns>
        internal void Get_All_Descendants(LinkedList<cssElement> List)
        {
            // Queue of elements we need to itterate on
            LinkedList<cssCompoundElement> Queue = new LinkedList<cssCompoundElement>();// new LinkedList<CompoundElement>(Children.Where(o => o is CompoundElement) as IEnumerable<CompoundElement>);

            for(int i=0; i<Children.Count; i++)
            {
                cssElement child = Children[i];
                if (child is cssCompoundElement) Queue.AddLast(child as cssCompoundElement);

                List.AddLast(child);
            }


            LinkedListNode<cssCompoundElement> node = Queue.First;
            do
            {
                IEnumerable<cssElement> items = node.Value.Items;
                foreach (cssElement o in items)
                {
                    List.AddLast(o);

                    if (o is cssCompoundElement) Queue.AddLast(o as cssCompoundElement);
                }
                node = node.Next;
            }
            while (node != null);
        }

        /// <summary>
        /// Builds and returns a complete list of every element which descends from this one.
        /// </summary>
        /// <returns></returns>
        internal LinkedList<cssElement> Get_All_Descendants()
        {
            LinkedList<cssElement> List = new LinkedList<cssElement>();
            // Queue of elements we need to itterate on
            LinkedList<cssCompoundElement> Queue = new LinkedList<cssCompoundElement>();// new LinkedList<CompoundElement>(Children.Where(o => o is CompoundElement) as IEnumerable<CompoundElement>);

            for (int i = 0; i < Children.Count; i++)
            {
                cssElement child = Children[i];
                if (child is cssCompoundElement) Queue.AddLast(child as cssCompoundElement);

                List.AddLast(child);
            }


            LinkedListNode<cssCompoundElement> node = Queue.First;
            do
            {
                IEnumerable<cssElement> items = node.Value.Items;
                foreach (cssElement o in items)
                {
                    List.AddLast(o);

                    if (o is cssCompoundElement) Queue.AddLast(o as cssCompoundElement);
                }
                node = node.Next;
            }
            while (node != null);

            return List;
        }


        /// <summary>
        /// Adds an element to this one
        /// </summary>
        public void Insert(int index, cssElement element)
        {
            if (Children.Contains(element))
                throw new Exception("Cannot add element which is already present!");
            if (Root != null && !string.IsNullOrEmpty(element.id) && Root.Find_ID(element.id) != null)
                throw new Exception("Cannot add element, another element with the same ID already exists!");

            ((IList<cssElement>)Children).Insert(index, element);

            element.Set_Parent(this, this.Count - 1);
            onControl_Added?.Invoke(this, element);
        }

        /// <summary>
        /// Adds an element to this one as a child
        /// </summary>
        public void Add(cssElement element)
        {
            Insert(Count, element);
        }

        /// <summary>
        /// Adds an element to this one as a child
        /// </summary>
        void ICollection<cssElement>.Add(cssElement element)
        {
            Insert(Count, element);
        }


        /// <summary>
        /// Removes a given element from this one and destroys it.
        /// </summary>
        /// <param name="index">The indice of the element to be removed</param>
        /// <returns>Success</returns>
        public void RemoveAt(int index)
        {
            Remove(this[index]);
        }

        /// <summary>
        /// Removes a given element from this one and destroys it.
        /// </summary>
        /// <param name="element">The element to be removed</param>
        /// <returns>Success</returns>
        public bool Remove(cssElement item)
        {
            return Remove(item, false);
        }

        /// <summary>
        /// Removes a given element from this one, optionally preserving it.
        /// </summary>
        /// <param name="element">The element to be removed</param>
        /// <param name="preserve">If TRUE then the element will not be disposed of immediately</param>
        /// <returns>Success</returns>
        public bool Remove(cssElement element, bool preserve = false)
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
        /// Clears and disposes of all child elements
        /// </summary>
        public void Clear()
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
        /// Clears and disposes of all child elements
        /// </summary>
        void ICollection<cssElement>.Clear()
        {
            (this as cssCompoundElement).Clear();
        }
        
        public int IndexOf(cssElement item)
        {
            return ((IList<cssElement>)Children).IndexOf(item);
        }
        
        public bool Contains(cssElement item)
        {
            return ((IList<cssElement>)Children).Contains(item);
        }

        public void CopyTo(cssElement[] array, int arrayIndex)
        {
            ((IList<cssElement>)Children).CopyTo(array, arrayIndex);
        }

        public IEnumerator<cssElement> GetEnumerator()
        {
            return ((IList<cssElement>)Children).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator() => ((IList<cssElement>)Children).GetEnumerator();
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
        protected void Flag_All_Children(EBoxInvalidationReason Reason)
        {
            for (int i = 0; i < Children.Count; i++)
            {
                var C = Children[i];
                C.Flag_Box_Dirty(Reason);
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
                C.Handle_Containing_Block_Dirty();
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

        public override void Flag_Box_Dirty(EBoxInvalidationReason Reason)
        {
            base.Flag_Box_Dirty(Reason);
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

        #endregion

        #region Hit Testing

        /// <summary>
        /// Returns the element which intersects the given screen-space point or NULL if none
        /// </summary>
        /// <param name="pos">Screen-Space point to test for intersection with</param>
        public override cssElement Get_Hit_Element(Vec2i pos)
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
        List<cssElement> Get_Children_Hit(Vec2i pos)
        {
            List<cssElement> list = new List<cssElement>();
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
        


    }
}
