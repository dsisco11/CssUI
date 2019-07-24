using System;
using System.Collections.Generic;

namespace CssUI.DOM.CustomElements
{
    public delegate void ReactionHandler(params object[] Args);

    public class CustomElementDefinition
    {/* Docs: https://html.spec.whatwg.org/multipage/custom-elements.html#custom-element-definition */
        /// <summary>
        /// returns a new dictionary for custom element lifecycle callbacks
        /// </summary>
        /// <returns></returns>
        public static Dictionary<AtomicName<EReactionName>, ReactionHandler> New_LifecycleCallbacks_Dictionary()
        {
            return new Dictionary<AtomicName<EReactionName>, ReactionHandler>() { { EReactionName.Connected, null }, { EReactionName.Disconnected, null }, { EReactionName.Adopted, null }, { EReactionName.AttributeChanged, null }, { EReactionName.FormAssociated, null }, { EReactionName.FormDisabled, null }, { EReactionName.FormReset, null }, { EReactionName.FormStateRestore, null } };
        }


        #region Properties
        /// <summary>
        /// A valid custom element name
        /// </summary>
        public readonly string name;
        /// <summary>
        /// A local name
        /// </summary>
        public readonly string localName;
        public readonly CustomElementConstructor constructor;
        public readonly bool observeAllAttributes = false;
        public readonly HashSet<AtomicName<EAttributeName>> observedAttributes;
        public readonly Dictionary<AtomicName<EReactionName>, ReactionHandler> lifecycleCallbacks = New_LifecycleCallbacks_Dictionary();
        public readonly Stack<WeakReference<Element>> constructionStack = new Stack<WeakReference<Element>>();

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
        public CustomElementDefinition(string name, string localName, CustomElementConstructor constructor, HashSet<AtomicName<EAttributeName>> observedAttributes, Dictionary<AtomicName<EReactionName>, ReactionHandler> lifecycleCallbacks, bool observeAllAttributes, bool bFormAssociated, bool bDisableInternals, bool bDisableShadow)
        {
            this.name = name;
            this.localName = localName;
            this.constructor = constructor;
            this.observedAttributes = observedAttributes;
            this.lifecycleCallbacks = lifecycleCallbacks;
            this.observeAllAttributes = observeAllAttributes;
            this.bFormAssociated = bFormAssociated;
            this.bDisableInternals = bDisableInternals;
            this.bDisableShadow = bDisableShadow;
        }
        #endregion
    }
}
