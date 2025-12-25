using EnumRecords;

namespace CssUI.HTML
{
    [EnumRecord<KeywordProperties>]
    public enum EAutofillMode
    {

        [EnumRecordProperties("shipping")]
        Shipping,

        [EnumRecordProperties("billing")]
        Billing,
    }
}

