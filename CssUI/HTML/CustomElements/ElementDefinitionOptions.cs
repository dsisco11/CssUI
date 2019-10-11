using CssUI.DOM.CustomElements;
using CssUI.HTML.CustomElements;
using System.Collections.Generic;

namespace CssUI.DOM.Internal
{
    public class ElementDefinitionOptions
    {
        #region Properties
        public readonly string extends;
        public readonly HashSet<AtomicName<EAttributeName>> observedAttributes = null;
        public readonly Dictionary<AtomicName<EReactionName>, ReactionHandler> lifecycleCallbacks = new Dictionary<AtomicName<EReactionName>, ReactionHandler>() { { EReactionName.Connected, null }, { EReactionName.Disconnected, null }, { EReactionName.Adopted, null }, { EReactionName.AttributeChanged, null }, { EReactionName.FormAssociated, null }, { EReactionName.FormDisabled, null }, { EReactionName.FormReset, null }, { EReactionName.FormStateRestore, null } };
        public readonly bool bDisableShadow = false;
        public readonly bool bDisableInternals = false;
        public readonly bool bFormAssociated = false;
        public readonly bool observeAllAttributes = false;

        #endregion

        #region Constructor
        public ElementDefinitionOptions(string extends, Dictionary<AtomicName<EReactionName>, ReactionHandler> lifecycleCallbacks, bool bDisableShadow, bool bDisableInternals, bool bFormAssociated, HashSet<AtomicName<EAttributeName>> observedAttributes = null)
        {
            this.extends = extends;
            this.lifecycleCallbacks = lifecycleCallbacks;
            this.bDisableShadow = bDisableShadow;
            this.bDisableInternals = bDisableInternals;
            this.bFormAssociated = bFormAssociated;
            this.observedAttributes = observedAttributes;
        }
        
        public ElementDefinitionOptions(string extends, Dictionary<AtomicName<EReactionName>, ReactionHandler> lifecycleCallbacks, bool bDisableShadow, bool bDisableInternals, bool bFormAssociated, bool observeAllAttributes)
        {
            this.extends = extends;
            this.lifecycleCallbacks = lifecycleCallbacks;
            this.bDisableShadow = bDisableShadow;
            this.bDisableInternals = bDisableInternals;
            this.bFormAssociated = bFormAssociated;
            this.observeAllAttributes = observeAllAttributes;
        }

        #endregion
    }
}
