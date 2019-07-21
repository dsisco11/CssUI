using System;
using System.Collections.Generic;

namespace CssUI.DOM.CustomElements
{
    public delegate void ReactionHandler(params object[] Args);

    public class CustomElementDefinition
    {/* Docs: https://html.spec.whatwg.org/multipage/custom-elements.html#custom-element-definition */

        #region Properties
        /// <summary>
        /// A valid custom element name
        /// </summary>
        public readonly string name;
        /// <summary>
        /// A local name
        /// </summary>
        public readonly string localName;
        public readonly Action constructor;
        public readonly HashSet<AtomicName<EAttributeName>> observedAttributes;
        public readonly Dictionary<AtomicName<EReactionName>, ReactionHandler> lifecycleCallbacks = new Dictionary<AtomicName<EReactionName>, ReactionHandler>() { { EReactionName.Connected, null }, { EReactionName.Disconnected, null }, { EReactionName.Adopted, null }, { EReactionName.AttributeChanged, null }, { EReactionName.FormAssociated, null }, { EReactionName.FormDisabled, null }, { EReactionName.FormReset, null }, { EReactionName.FormStateRestore, null } };
        public readonly Stack<Element> constructionStack = new Stack<Element>();

        /// <summary>
        /// If this is true, user agent treats elements associated to this custom element definition as form-associated custom elements.
        /// </summary>
        public readonly bool bFormAssociated = false;

        /// <summary>
        /// Controls attachInternals()
        /// </summary>
        public readonly bool bDisableInternals = false;

        /// <summary>
        /// Controls attachShadow()
        /// </summary>
        public readonly bool bDisableShadow = false;
        #endregion

        #region Constructor
        public CustomElementDefinition(string name, string localName, Action constructor, HashSet<AtomicName<EAttributeName>> observedAttributes, Dictionary<AtomicName<EReactionName>, ReactionHandler> lifecycleCallbacks, Stack<Element> constructionStack, bool bFormAssociated, bool bDisableInternals, bool bDisableShadow)
        {
            this.name = name;
            this.localName = localName;
            this.constructor = constructor;
            this.observedAttributes = observedAttributes;
            this.lifecycleCallbacks = lifecycleCallbacks;
            this.constructionStack = constructionStack;
            this.bFormAssociated = bFormAssociated;
            this.bDisableInternals = bDisableInternals;
            this.bDisableShadow = bDisableShadow;
        }
        #endregion
    }
}
