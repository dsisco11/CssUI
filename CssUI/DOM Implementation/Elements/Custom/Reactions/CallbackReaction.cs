using CssUI.DOM.CustomElements;

namespace CssUI.DOM.Internal
{
    internal class CallbackReaction : IElementReaction
    {
        #region Properties
        private readonly CustomReactionCallback Callback = null;
        private readonly object[] Args = null;
        #endregion

        #region Constructor
        public CallbackReaction(CustomReactionCallback Callback, params object[] Args)
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
