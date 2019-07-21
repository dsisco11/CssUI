
namespace CssUI.DOM.CustomElements
{
    /// <summary>
    /// Implements a Custom element reaction callback containing a function handler to call and the arguments to pass to it.
    /// </summary>
    internal class ReactionCallback : IElementReaction
    {
        #region Properties
        private readonly ReactionHandler Callback = null;
        private readonly object[] Args = null;
        #endregion

        #region Constructor
        public ReactionCallback(ReactionHandler Callback, params object[] Args)
        {
            this.Callback = Callback;
            this.Args = Args;
        }
        #endregion


        /// <summary>
        /// Executes the logic for this reactions
        /// </summary>
        public void Handle(Element element)
        {
            Callback.Invoke(Args);
        }
    }
}
