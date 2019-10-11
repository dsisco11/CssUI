using CssUI.Internal;

namespace CssUI.DOM.CustomElements
{
    /// <summary>
    /// Lists all of the possible custom element reactions
    /// </summary>
    [MetaEnum]
    public enum EReactionName : int
    {
        [MetaKeyword("attributeChangedCallback")]
        AttributeChanged,

        [MetaKeyword("connectedCallback")]
        Connected,

        [MetaKeyword("disconnectedCallback")]
        Disconnected,

        [MetaKeyword("adoptedCallback")]
        Adopted,

        [MetaKeyword("formAssociatedCallback")]
        FormAssociated,

        [MetaKeyword("formDisabledCallback")]
        FormDisabled,

        [MetaKeyword("formResetCallback")]
        FormReset,

        [MetaKeyword("formStateRestoreCallback")]
        FormStateRestore,

    }
}
