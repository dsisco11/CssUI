using CssUI.DOM;
using CssUI.HTML;
using CssUI.HTML.CustomElements;

namespace CssUI.HTML.Internal
{
    internal class UpgradeReaction : IElementReaction
    {
        #region Properties
        public readonly CustomElementDefinition definition = null;
        #endregion

        #region Constructor
        public UpgradeReaction(CustomElementDefinition definition)
        {
            this.definition = definition;
        }
        #endregion
        /// <summary>
        /// Executes the logic for this reactions
        /// </summary>
        public void Handle(Element element)
        {
            throw new System.NotImplementedException();
        }
    }
}
