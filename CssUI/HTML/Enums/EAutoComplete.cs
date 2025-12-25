using EnumRecords;

namespace CssUI.HTML
{
    [EnumRecord<KeywordProperties>]
    public enum EAutoComplete : int
    {

        [EnumData("on")]
        On,

        [EnumData("off")]
        Off,
    }
}

