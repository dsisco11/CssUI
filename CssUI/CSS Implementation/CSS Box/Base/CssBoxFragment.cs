namespace CssUI.CSS
{
    public class CssBoxFragment
    {
        #region Box Areas
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

        #region Constructor
        public CssBoxFragment()
        {
            this.Content = new CssBoxArea(this);
            this.Padding = new CssBoxArea(this);
            this.Border = new CssBoxArea(this);
            this.Margin = new CssBoxArea(this);
        }
        #endregion
    }
}
