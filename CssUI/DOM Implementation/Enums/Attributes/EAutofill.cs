using CssUI.Internal;

namespace CssUI.DOM
{

    [MetaEnum]
    public enum EAutofill : int
    {
        [MetaKeyword("", 0, "")]
        EMPTY,

        [MetaKeyword("off", 1, "Off")]
        Off,

        [MetaKeyword("on", 1, "Automatic")]
        On,

        [MetaKeyword("name", 3, "Normal")]
        Name,

        [MetaKeyword("honorific-prefix", 3, "Normal")]
        Honorific_Prefix,

        [MetaKeyword("given-name", 3, "Normal")]
        Given_Name,

        [MetaKeyword("additional-name", 3, "Normal")]
        Additional_Name,

        [MetaKeyword("family-name", 3, "Normal")]
        Family_Name,

        [MetaKeyword("honorific-suffix", 3, "Normal")]
        Honorific_Suffix,

        [MetaKeyword("nickname", 3, "Normal")]
        Nickname,

        [MetaKeyword("organization-title", 3, "Normal")]
        Organization_Title,

        [MetaKeyword("username", 3, "Normal")]
        Username,

        [MetaKeyword("new-password", 3, "Normal")]
        New_Password,

        [MetaKeyword("current-password", 3, "Normal")]
        Current_Password,

        [MetaKeyword("one-time-code", 3, "Normal")]
        One_Time_Code,

        [MetaKeyword("organization", 3, "Normal")]
        Organization,

        [MetaKeyword("street-address", 3, "Normal")]
        Street_Address,

        [MetaKeyword("address-line1", 3, "Normal")]
        Address_Line1,

        [MetaKeyword("address-line2", 3, "Normal")]
        Address_Line2,

        [MetaKeyword("address-line3", 3, "Normal")]
        Address_Line3,

        [MetaKeyword("address-level4", 3, "Normal")]
        Address_Level4,

        [MetaKeyword("address-level3", 3, "Normal")]
        Address_Level3,

        [MetaKeyword("address-level2", 3, "Normal")]
        Address_Level2,

        [MetaKeyword("address-level1", 3, "Normal")]
        Address_Level1,

        [MetaKeyword("country", 3, "Normal")]
        Country,

        [MetaKeyword("country-name", 3, "Normal")]
        Country_Name,

        [MetaKeyword("postal-code", 3, "Normal")]
        Postal_Code,

        [MetaKeyword("cc-name", 3, "Normal")]
        CC_Name,

        [MetaKeyword("cc-given-name", 3, "Normal")]
        CC_Given_Name,

        [MetaKeyword("cc-additional-name", 3, "Normal")]
        CC_Additional_Name,

        [MetaKeyword("cc-family-name", 3, "Normal")]
        CC_Family_Name,

        [MetaKeyword("cc-number", 3, "Normal")]
        CC_Number,

        [MetaKeyword("cc-exp", 3, "Normal")]
        CC_Exp,

        [MetaKeyword("cc-exp-month", 3, "Normal")]
        CC_Exp_Month,

        [MetaKeyword("cc-exp-year", 3, "Normal")]
        CC_Exp_Year,

        [MetaKeyword("cc-csc", 3, "Normal")]
        CC_Csc,

        [MetaKeyword("cc-type", 3, "Normal")]
        CC_Type,

        [MetaKeyword("transaction-currency", 3, "Normal")]
        Transaction_Currency,

        [MetaKeyword("transaction-amount", 3, "Normal")]
        Transaction_Amount,

        [MetaKeyword("language", 3, "Normal")]
        Language,

        [MetaKeyword("bday", 3, "Normal")]
        Bday,

        [MetaKeyword("bday-day", 3, "Normal")]
        Bday_Day,

        [MetaKeyword("bday-month", 3, "Normal")]
        Bday_Month,

        [MetaKeyword("bday-year", 3, "Normal")]
        Bday_Year,

        [MetaKeyword("sex", 3, "Normal")]
        Sex,

        [MetaKeyword("url", 3, "Normal")]
        Url,

        [MetaKeyword("photo", 3, "Normal")]
        Photo,

        [MetaKeyword("tel", 4, "Contact")]
        Tel,

        [MetaKeyword("tel-country-code", 4, "Contact")]
        Tel_Country_Code,

        [MetaKeyword("tel-national", 4, "Contact")]
        Tel_National,

        [MetaKeyword("tel-area-code", 4, "Contact")]
        Tel_Area_Code,

        [MetaKeyword("tel-local", 4, "Contact")]
        Tel_Local,

        [MetaKeyword("tel-local-prefix", 4, "Contact")]
        Tel_Local_Prefix,

        [MetaKeyword("tel-local-suffix", 4, "Contact")]
        Tel_Local_Suffix,

        [MetaKeyword("tel-extension", 4, "Contact")]
        Tel_Extension,

        [MetaKeyword("email", 4, "Contact")]
        Email,

        [MetaKeyword("impp", 4, "Contact")]
        Impp,
    }
}
