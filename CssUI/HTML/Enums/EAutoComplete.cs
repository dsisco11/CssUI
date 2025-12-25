using EnumRecords;

namespace CssUI.HTML
{
    [EnumRecord<KeywordProperties>]
    public enum EAutoComplete : int
    {

        [EnumRecordProperties("on")]
        On,

        [EnumRecordProperties("off")]
        Off,
    }
}

