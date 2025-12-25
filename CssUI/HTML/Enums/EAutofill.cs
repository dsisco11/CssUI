using EnumRecords;

namespace CssUI.HTML
{

    [EnumRecord<AutofillProperties>]
    public enum EAutofill : int
    {
        [EnumRecordProperties("", 0, "")]
        EMPTY,

        [EnumRecordProperties("off", 1, "Off")]
        Off,

        [EnumRecordProperties("on", 1, "Automatic")]
        On,

        [EnumRecordProperties("name", 3, "Normal")]
        Name,

        [EnumRecordProperties("honorific-prefix", 3, "Normal")]
        Honorific_Prefix,

        [EnumRecordProperties("given-name", 3, "Normal")]
        Given_Name,

        [EnumRecordProperties("additional-name", 3, "Normal")]
        Additional_Name,

        [EnumRecordProperties("family-name", 3, "Normal")]
        Family_Name,

        [EnumRecordProperties("honorific-suffix", 3, "Normal")]
        Honorific_Suffix,

        [EnumRecordProperties("nickname", 3, "Normal")]
        Nickname,

        [EnumRecordProperties("organization-title", 3, "Normal")]
        Organization_Title,

        [EnumRecordProperties("username", 3, "Normal")]
        Username,

        [EnumRecordProperties("new-password", 3, "Normal")]
        New_Password,

        [EnumRecordProperties("current-password", 3, "Normal")]
        Current_Password,

        [EnumRecordProperties("one-time-code", 3, "Normal")]
        One_Time_Code,

        [EnumRecordProperties("organization", 3, "Normal")]
        Organization,

        [EnumRecordProperties("street-address", 3, "Normal")]
        Street_Address,

        [EnumRecordProperties("address-line1", 3, "Normal")]
        Address_Line1,

        [EnumRecordProperties("address-line2", 3, "Normal")]
        Address_Line2,

        [EnumRecordProperties("address-line3", 3, "Normal")]
        Address_Line3,

        [EnumRecordProperties("address-level4", 3, "Normal")]
        Address_Level4,

        [EnumRecordProperties("address-level3", 3, "Normal")]
        Address_Level3,

        [EnumRecordProperties("address-level2", 3, "Normal")]
        Address_Level2,

        [EnumRecordProperties("address-level1", 3, "Normal")]
        Address_Level1,

        [EnumRecordProperties("country", 3, "Normal")]
        Country,

        [EnumRecordProperties("country-name", 3, "Normal")]
        Country_Name,

        [EnumRecordProperties("postal-code", 3, "Normal")]
        Postal_Code,

        [EnumRecordProperties("cc-name", 3, "Normal")]
        CC_Name,

        [EnumRecordProperties("cc-given-name", 3, "Normal")]
        CC_Given_Name,

        [EnumRecordProperties("cc-additional-name", 3, "Normal")]
        CC_Additional_Name,

        [EnumRecordProperties("cc-family-name", 3, "Normal")]
        CC_Family_Name,

        [EnumRecordProperties("cc-number", 3, "Normal")]
        CC_Number,

        [EnumRecordProperties("cc-exp", 3, "Normal")]
        CC_Exp,

        [EnumRecordProperties("cc-exp-month", 3, "Normal")]
        CC_Exp_Month,

        [EnumRecordProperties("cc-exp-year", 3, "Normal")]
        CC_Exp_Year,

        [EnumRecordProperties("cc-csc", 3, "Normal")]
        CC_Csc,

        [EnumRecordProperties("cc-type", 3, "Normal")]
        CC_Type,

        [EnumRecordProperties("transaction-currency", 3, "Normal")]
        Transaction_Currency,

        [EnumRecordProperties("transaction-amount", 3, "Normal")]
        Transaction_Amount,

        [EnumRecordProperties("language", 3, "Normal")]
        Language,

        [EnumRecordProperties("bday", 3, "Normal")]
        Bday,

        [EnumRecordProperties("bday-day", 3, "Normal")]
        Bday_Day,

        [EnumRecordProperties("bday-month", 3, "Normal")]
        Bday_Month,

        [EnumRecordProperties("bday-year", 3, "Normal")]
        Bday_Year,

        [EnumRecordProperties("sex", 3, "Normal")]
        Sex,

        [EnumRecordProperties("url", 3, "Normal")]
        Url,

        [EnumRecordProperties("photo", 3, "Normal")]
        Photo,

        [EnumRecordProperties("tel", 4, "Contact")]
        Tel,

        [EnumRecordProperties("tel-country-code", 4, "Contact")]
        Tel_Country_Code,

        [EnumRecordProperties("tel-national", 4, "Contact")]
        Tel_National,

        [EnumRecordProperties("tel-area-code", 4, "Contact")]
        Tel_Area_Code,

        [EnumRecordProperties("tel-local", 4, "Contact")]
        Tel_Local,

        [EnumRecordProperties("tel-local-prefix", 4, "Contact")]
        Tel_Local_Prefix,

        [EnumRecordProperties("tel-local-suffix", 4, "Contact")]
        Tel_Local_Suffix,

        [EnumRecordProperties("tel-extension", 4, "Contact")]
        Tel_Extension,

        [EnumRecordProperties("email", 4, "Contact")]
        Email,

        [EnumRecordProperties("impp", 4, "Contact")]
        Impp,
    }
}

