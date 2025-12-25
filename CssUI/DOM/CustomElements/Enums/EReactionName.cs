using EnumRecords;

namespace CssUI.DOM.CustomElements;

/// <summary>
/// Lists all of the possible custom element reactions
/// </summary>
[EnumRecord<KeywordProperties>]
public enum EReactionName : int
{
    [EnumData("attributeChangedCallback")]
    AttributeChanged,

    [EnumData("connectedCallback")]
    Connected,

    [EnumData("disconnectedCallback")]
    Disconnected,

    [EnumData("adoptedCallback")]
    Adopted,

    [EnumData("formAssociatedCallback")]
    FormAssociated,

    [EnumData("formDisabledCallback")]
    FormDisabled,

    [EnumData("formResetCallback")]
    FormReset,

    [EnumData("formStateRestoreCallback")]
    FormStateRestore,

}

