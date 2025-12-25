using CssUI.DOM;
using EnumRecords;

namespace CssUI.HTML
{
    // Category values: 0=Off, 1=Automatic, 2=Normal, 3=Contact (matches EAutofillCategory)
    [EnumRecord<AutofillProperties>]
    public enum EAutofill : int
    {
        [EnumRecordProperties("", 0, (int)EAutofillCategory.Off)]
        EMPTY,

        [EnumRecordProperties("off", 1, (int)EAutofillCategory.Off)]
        Off,

        [EnumRecordProperties("on", 1, (int)EAutofillCategory.Automatic)]
        On,

        [EnumRecordProperties("name", 3, (int)EAutofillCategory.Normal)]
        Name,

        [EnumRecordProperties("honorific-prefix", 3, (int)EAutofillCategory.Normal)]
        Honorific_Prefix,

        [EnumRecordProperties("given-name", 3, (int)EAutofillCategory.Normal)]
        Given_Name,

        [EnumRecordProperties("additional-name", 3, (int)EAutofillCategory.Normal)]
        Additional_Name,

        [EnumRecordProperties("family-name", 3, (int)EAutofillCategory.Normal)]
        Family_Name,

        [EnumRecordProperties("honorific-suffix", 3, (int)EAutofillCategory.Normal)]
        Honorific_Suffix,

        [EnumRecordProperties("nickname", 3, (int)EAutofillCategory.Normal)]
        Nickname,

        [EnumRecordProperties("organization-title", 3, (int)EAutofillCategory.Normal)]
        Organization_Title,

        [EnumRecordProperties("username", 3, (int)EAutofillCategory.Normal)]
        Username,

        [EnumRecordProperties("new-password", 3, (int)EAutofillCategory.Normal)]
        New_Password,

        [EnumRecordProperties("current-password", 3, (int)EAutofillCategory.Normal)]
        Current_Password,

        [EnumRecordProperties("one-time-code", 3, (int)EAutofillCategory.Normal)]
        One_Time_Code,

        [EnumRecordProperties("organization", 3, (int)EAutofillCategory.Normal)]
        Organization,

        [EnumRecordProperties("street-address", 3, (int)EAutofillCategory.Normal)]
        Street_Address,

        [EnumRecordProperties("address-line1", 3, (int)EAutofillCategory.Normal)]
        Address_Line1,

        [EnumRecordProperties("address-line2", 3, (int)EAutofillCategory.Normal)]
        Address_Line2,

        [EnumRecordProperties("address-line3", 3, (int)EAutofillCategory.Normal)]
        Address_Line3,

        [EnumRecordProperties("address-level4", 3, (int)EAutofillCategory.Normal)]
        Address_Level4,

        [EnumRecordProperties("address-level3", 3, (int)EAutofillCategory.Normal)]
        Address_Level3,

        [EnumRecordProperties("address-level2", 3, (int)EAutofillCategory.Normal)]
        Address_Level2,

        [EnumRecordProperties("address-level1", 3, (int)EAutofillCategory.Normal)]
        Address_Level1,

        [EnumRecordProperties("country", 3, (int)EAutofillCategory.Normal)]
        Country,

        [EnumRecordProperties("country-name", 3, (int)EAutofillCategory.Normal)]
        Country_Name,

        [EnumRecordProperties("postal-code", 3, (int)EAutofillCategory.Normal)]
        Postal_Code,

        [EnumRecordProperties("cc-name", 3, (int)EAutofillCategory.Normal)]
        CC_Name,

        [EnumRecordProperties("cc-given-name", 3, (int)EAutofillCategory.Normal)]
        CC_Given_Name,

        [EnumRecordProperties("cc-additional-name", 3, (int)EAutofillCategory.Normal)]
        CC_Additional_Name,

        [EnumRecordProperties("cc-family-name", 3, (int)EAutofillCategory.Normal)]
        CC_Family_Name,

        [EnumRecordProperties("cc-number", 3, (int)EAutofillCategory.Normal)]
        CC_Number,

        [EnumRecordProperties("cc-exp", 3, (int)EAutofillCategory.Normal)]
        CC_Exp,

        [EnumRecordProperties("cc-exp-month", 3, (int)EAutofillCategory.Normal)]
        CC_Exp_Month,

        [EnumRecordProperties("cc-exp-year", 3, (int)EAutofillCategory.Normal)]
        CC_Exp_Year,

        [EnumRecordProperties("cc-csc", 3, (int)EAutofillCategory.Normal)]
        CC_Csc,

        [EnumRecordProperties("cc-type", 3, (int)EAutofillCategory.Normal)]
        CC_Type,

        [EnumRecordProperties("transaction-currency", 3, (int)EAutofillCategory.Normal)]
        Transaction_Currency,

        [EnumRecordProperties("transaction-amount", 3, (int)EAutofillCategory.Normal)]
        Transaction_Amount,

        [EnumRecordProperties("language", 3, (int)EAutofillCategory.Normal)]
        Language,

        [EnumRecordProperties("bday", 3, (int)EAutofillCategory.Normal)]
        Bday,

        [EnumRecordProperties("bday-day", 3, (int)EAutofillCategory.Normal)]
        Bday_Day,

        [EnumRecordProperties("bday-month", 3, (int)EAutofillCategory.Normal)]
        Bday_Month,

        [EnumRecordProperties("bday-year", 3, (int)EAutofillCategory.Normal)]
        Bday_Year,

        [EnumRecordProperties("sex", 3, (int)EAutofillCategory.Normal)]
        Sex,

        [EnumRecordProperties("url", 3, (int)EAutofillCategory.Normal)]
        Url,

        [EnumRecordProperties("photo", 3, (int)EAutofillCategory.Normal)]
        Photo,

        [EnumRecordProperties("tel", 4, (int)EAutofillCategory.Contact)]
        Tel,

        [EnumRecordProperties("tel-country-code", 4, (int)EAutofillCategory.Contact)]
        Tel_Country_Code,

        [EnumRecordProperties("tel-national", 4, (int)EAutofillCategory.Contact)]
        Tel_National,

        [EnumRecordProperties("tel-area-code", 4, (int)EAutofillCategory.Contact)]
        Tel_Area_Code,

        [EnumRecordProperties("tel-local", 4, (int)EAutofillCategory.Contact)]
        Tel_Local,

        [EnumRecordProperties("tel-local-prefix", 4, (int)EAutofillCategory.Contact)]
        Tel_Local_Prefix,

        [EnumRecordProperties("tel-local-suffix", 4, (int)EAutofillCategory.Contact)]
        Tel_Local_Suffix,

        [EnumRecordProperties("tel-extension", 4, (int)EAutofillCategory.Contact)]
        Tel_Extension,

        [EnumRecordProperties("email", 4, (int)EAutofillCategory.Contact)]
        Email,

        [EnumRecordProperties("impp", 4, (int)EAutofillCategory.Contact)]
        Impp,
    }
}

