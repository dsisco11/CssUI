using EnumRecords;

namespace CssUI.HTML
{
    [EnumRecord<KeywordProperties>]
    public enum EAutofillContact
    {
        [EnumData("home")]
        Home,

        [EnumData("work")]
        Work,

        [EnumData("mobile")]
        Mobile,

        [EnumData("fax")]
        Fax,

        [EnumData("pager")]
        Pager,
    }
}

