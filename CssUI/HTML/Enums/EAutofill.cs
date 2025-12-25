using CssUI.DOM;
using EnumRecords;

namespace CssUI.HTML
{
    // Category values: 0=Off, 1=Automatic, 2=Normal, 3=Contact (matches EAutofillCategory)
    [EnumRecord<AutofillProperties>]
    public enum EAutofill : int
    {
        [EnumData("", 0, (int)EAutofillCategory.Off)]
        EMPTY,

        [EnumData("off", 1, (int)EAutofillCategory.Off)]
        Off,

        [EnumData("on", 1, (int)EAutofillCategory.Automatic)]
        On,

        [EnumData("name", 3, (int)EAutofillCategory.Normal)]
        Name,

        [EnumData("honorific-prefix", 3, (int)EAutofillCategory.Normal)]
        Honorific_Prefix,

        [EnumData("given-name", 3, (int)EAutofillCategory.Normal)]
        Given_Name,

        [EnumData("additional-name", 3, (int)EAutofillCategory.Normal)]
        Additional_Name,

        [EnumData("family-name", 3, (int)EAutofillCategory.Normal)]
        Family_Name,

        [EnumData("honorific-suffix", 3, (int)EAutofillCategory.Normal)]
        Honorific_Suffix,

        [EnumData("nickname", 3, (int)EAutofillCategory.Normal)]
        Nickname,

        [EnumData("organization-title", 3, (int)EAutofillCategory.Normal)]
        Organization_Title,

        [EnumData("username", 3, (int)EAutofillCategory.Normal)]
        Username,

        [EnumData("new-password", 3, (int)EAutofillCategory.Normal)]
        New_Password,

        [EnumData("current-password", 3, (int)EAutofillCategory.Normal)]
        Current_Password,

        [EnumData("one-time-code", 3, (int)EAutofillCategory.Normal)]
        One_Time_Code,

        [EnumData("organization", 3, (int)EAutofillCategory.Normal)]
        Organization,

        [EnumData("street-address", 3, (int)EAutofillCategory.Normal)]
        Street_Address,

        [EnumData("address-line1", 3, (int)EAutofillCategory.Normal)]
        Address_Line1,

        [EnumData("address-line2", 3, (int)EAutofillCategory.Normal)]
        Address_Line2,

        [EnumData("address-line3", 3, (int)EAutofillCategory.Normal)]
        Address_Line3,

        [EnumData("address-level4", 3, (int)EAutofillCategory.Normal)]
        Address_Level4,

        [EnumData("address-level3", 3, (int)EAutofillCategory.Normal)]
        Address_Level3,

        [EnumData("address-level2", 3, (int)EAutofillCategory.Normal)]
        Address_Level2,

        [EnumData("address-level1", 3, (int)EAutofillCategory.Normal)]
        Address_Level1,

        [EnumData("country", 3, (int)EAutofillCategory.Normal)]
        Country,

        [EnumData("country-name", 3, (int)EAutofillCategory.Normal)]
        Country_Name,

        [EnumData("postal-code", 3, (int)EAutofillCategory.Normal)]
        Postal_Code,

        [EnumData("cc-name", 3, (int)EAutofillCategory.Normal)]
        CC_Name,

        [EnumData("cc-given-name", 3, (int)EAutofillCategory.Normal)]
        CC_Given_Name,

        [EnumData("cc-additional-name", 3, (int)EAutofillCategory.Normal)]
        CC_Additional_Name,

        [EnumData("cc-family-name", 3, (int)EAutofillCategory.Normal)]
        CC_Family_Name,

        [EnumData("cc-number", 3, (int)EAutofillCategory.Normal)]
        CC_Number,

        [EnumData("cc-exp", 3, (int)EAutofillCategory.Normal)]
        CC_Exp,

        [EnumData("cc-exp-month", 3, (int)EAutofillCategory.Normal)]
        CC_Exp_Month,

        [EnumData("cc-exp-year", 3, (int)EAutofillCategory.Normal)]
        CC_Exp_Year,

        [EnumData("cc-csc", 3, (int)EAutofillCategory.Normal)]
        CC_Csc,

        [EnumData("cc-type", 3, (int)EAutofillCategory.Normal)]
        CC_Type,

        [EnumData("transaction-currency", 3, (int)EAutofillCategory.Normal)]
        Transaction_Currency,

        [EnumData("transaction-amount", 3, (int)EAutofillCategory.Normal)]
        Transaction_Amount,

        [EnumData("language", 3, (int)EAutofillCategory.Normal)]
        Language,

        [EnumData("bday", 3, (int)EAutofillCategory.Normal)]
        Bday,

        [EnumData("bday-day", 3, (int)EAutofillCategory.Normal)]
        Bday_Day,

        [EnumData("bday-month", 3, (int)EAutofillCategory.Normal)]
        Bday_Month,

        [EnumData("bday-year", 3, (int)EAutofillCategory.Normal)]
        Bday_Year,

        [EnumData("sex", 3, (int)EAutofillCategory.Normal)]
        Sex,

        [EnumData("url", 3, (int)EAutofillCategory.Normal)]
        Url,

        [EnumData("photo", 3, (int)EAutofillCategory.Normal)]
        Photo,

        [EnumData("tel", 4, (int)EAutofillCategory.Contact)]
        Tel,

        [EnumData("tel-country-code", 4, (int)EAutofillCategory.Contact)]
        Tel_Country_Code,

        [EnumData("tel-national", 4, (int)EAutofillCategory.Contact)]
        Tel_National,

        [EnumData("tel-area-code", 4, (int)EAutofillCategory.Contact)]
        Tel_Area_Code,

        [EnumData("tel-local", 4, (int)EAutofillCategory.Contact)]
        Tel_Local,

        [EnumData("tel-local-prefix", 4, (int)EAutofillCategory.Contact)]
        Tel_Local_Prefix,

        [EnumData("tel-local-suffix", 4, (int)EAutofillCategory.Contact)]
        Tel_Local_Suffix,

        [EnumData("tel-extension", 4, (int)EAutofillCategory.Contact)]
        Tel_Extension,

        [EnumData("email", 4, (int)EAutofillCategory.Contact)]
        Email,

        [EnumData("impp", 4, (int)EAutofillCategory.Contact)]
        Impp,
    }
}

