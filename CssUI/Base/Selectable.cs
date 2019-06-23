using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CssUI
{
    /// <summary>
    /// An element which can receive input focus and tracks a 'Selected' state
    /// </summary>
    public abstract class Selectable : cssCompoundElement, ISelectableElement
    {
        public override string Default_CSS_TypeName { get { return "Selectable"; } }

        #region Constructors
        public Selectable(string ID = null) : base(ID)
        {
            Flags_Add(EElementFlags.Focusable);
        }
        #endregion

        #region Selection Events
        public event Action<Selectable> onSelectedChanged;
        #endregion

        #region Selection State
        private bool selected = false;
        /// <summary>
        /// Selected status of the element
        /// </summary>
        public bool Selected
        {
            get { return selected; }
            set
            {
                if (selected != value)
                {
                    selected = value;
                    onSelectedChanged?.Invoke(this);
                }
            }
        }

        /// <summary>
        /// Toggle the selection state
        /// </summary>
        public void Select()
        {
            Selected = !Selected;
        }
        #endregion
    }
}
