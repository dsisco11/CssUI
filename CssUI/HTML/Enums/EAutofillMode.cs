using EnumRecords;

namespace CssUI.HTML
{
    [EnumRecord<KeywordProperties>]
    public enum EAutofillMode
    {

        [EnumData("shipping")]
        Shipping,

        [EnumData("billing")]
        Billing,
    }
}

