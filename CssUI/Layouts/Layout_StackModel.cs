
namespace CssUI
{
    /// <summary>
    /// Layout model that treats every item as if it were block-level
    /// </summary>
    public sealed class Layout_StackModel : LayoutDirectorBase
    {
        #region Constructors
        public Layout_StackModel()
        {
        }
        #endregion


        public override cssBoxArea Handle(IParentElement Owner, cssElement[] controls)
        {
            Reset();

            for (int i = 0; i < controls.Length; i++)
            {
                var E = controls[i];
                if (!E.Affects_Layout) continue;

                // Get the elements bounds
                var cSize = E.Box.Content.Get_Dimensions();

                // Add the current element to our line
                Add_To_Line(E, E.Box.Content, cSize);
                Start_New_Line();
            }

            if (Line.Count > 0) Start_New_Line();// Make sure our line in progress is added

            Perform_Alignment((Owner as cssElement).Style.TextAlign);
            Finalize_Positions();

            return layoutBlock;
        }
    }
}
