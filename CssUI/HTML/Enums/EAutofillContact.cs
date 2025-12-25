using EnumRecords;

namespace CssUI.HTML
{
    [EnumRecord<KeywordProperties>]
    public enum EAutofillContact
    {
        [EnumRecordProperties("home")]
        Home,

        [EnumRecordProperties("work")]
        Work,

        [EnumRecordProperties("mobile")]
        Mobile,

        [EnumRecordProperties("fax")]
        Fax,

        [EnumRecordProperties("pager")]
        Pager,
    }
}

