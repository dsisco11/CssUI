using EnumRecords;

namespace CssUI.DOM.CustomElements;

/// <summary>
/// Lists all of the possible custom element reactions
/// </summary>
[EnumRecord<KeywordProperties>]
public enum EReactionName : int
{
    [EnumRecordProperties("attributeChangedCallback")]
    AttributeChanged,

    [EnumRecordProperties("connectedCallback")]
    Connected,

    [EnumRecordProperties("disconnectedCallback")]
    Disconnected,

    [EnumRecordProperties("adoptedCallback")]
    Adopted,

    [EnumRecordProperties("formAssociatedCallback")]
    FormAssociated,

    [EnumRecordProperties("formDisabledCallback")]
    FormDisabled,

    [EnumRecordProperties("formResetCallback")]
    FormReset,

    [EnumRecordProperties("formStateRestoreCallback")]
    FormStateRestore,

}

