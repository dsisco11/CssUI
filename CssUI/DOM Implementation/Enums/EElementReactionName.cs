﻿using CssUI.DOM.Internal;

namespace CssUI.DOM.Enums
{
    /// <summary>
    /// Lists all of the possible custom element reactions
    /// </summary>
    [DomEnum]
    public enum EElementReactionName : int
    {
        [DomKeyword("attributeChangedCallback")]
        AttributeChanged,

        [DomKeyword("connectedCallback")]
        Connected,

        [DomKeyword("disconnectedCallback")]
        Disconnected,

        [DomKeyword("adoptedCallback")]
        Adopted,

        [DomKeyword("formAssociatedCallback")]
        FormAssociated,

        [DomKeyword("formDisabledCallback")]
        FormDisabled,

        [DomKeyword("formResetCallback")]
        FormReset,

        [DomKeyword("formStateRestoreCallback")]
        FormStateRestore,

    }
}