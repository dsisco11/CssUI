using CssUI.CSS.BoxTree;

namespace CssUI.CSS
{
    public class CssBoxFragment : ICssBoxFragment
    {
        #region Properties
        public ICssBoxFragment Parent { get; }
        #endregion

        #region Accessors
        public virtual CssBox Owner
        {
            get
            {
                ICssBoxFragment parent = Parent;
                while (!(parent is CssBox))
                {
                    parent = parent.Parent;
                }

                return ((CssBox)parent);
            }
        }
        #endregion

        #region Box Areas
        /// <summary>
        /// The edge positions of the Repalced-Content-Area 
        /// </summary>
        public CssBoxArea Replaced { get; private set; } = null;
        /// <summary>
        /// The edge positions of the Content-Area 
        /// </summary>
        public CssBoxArea Content { get; private set; }
        /// <summary>
        /// The edge positions of the Padding-Area 
        /// </summary>
        public CssBoxArea Padding { get; private set; }
        /// <summary>
        /// The edge positions of the Border-Area 
        /// </summary>
        public CssBoxArea Border { get; private set; }
        /// <summary>
        /// The edge positions of the Margin-Area 
        /// </summary>
        public CssBoxArea Margin { get; private set; }

        /// <summary>
        /// The block which represents the hitbox for mouse related input events
        /// </summary>
        public CssBoxArea ClickArea { get => Padding; }
        #endregion

        #region Constructor
        public CssBoxFragment(ICssBoxFragment parent)
        {
            Content = new CssBoxArea(this);
            Padding = new CssBoxArea(this);
            Border = new CssBoxArea(this);
            Margin = new CssBoxArea(this);
            Parent = parent;
        }
        #endregion
    }
}
