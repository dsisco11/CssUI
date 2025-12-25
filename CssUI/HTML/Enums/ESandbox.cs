using EnumRecords;

namespace CssUI.HTML
{
    [EnumRecord<KeywordProperties>]
    public enum ESandbox : int
    {/* Docs: https://html.spec.whatwg.org/multipage/iframe-embed-object.html#attr-iframe-sandbox */

        /// <summary>
        /// 
        /// </summary>
        [EnumRecordProperties("allow-forms")]
        Allow_Forms, 

        /// <summary>
        /// 
        /// </summary>
        [EnumRecordProperties("allow-modals")]
        Allow_Modals, 

        /// <summary>
        /// 
        /// </summary>
        [EnumRecordProperties("allow-orientation-lock")]
        Allow_Orientation_Lock, 

        /// <summary>
        /// 
        /// </summary>
        [EnumRecordProperties("allow-pointer-lock")]
        Allow_Pointer_Lock, 

        /// <summary>
        /// 
        /// </summary>
        [EnumRecordProperties("allow-popups")]
        Allow_Popups, 

        /// <summary>
        /// 
        /// </summary>
        [EnumRecordProperties("allow-popups-to-escape-sandbox")]
        Allow_Popups_To_Escape_Sandbox, 

        /// <summary>
        /// 
        /// </summary>
        [EnumRecordProperties("allow-presentation")]
        Allow_Presentation, 

        /// <summary>
        /// 
        /// </summary>
        [EnumRecordProperties("allow-same-origin")]
        Allow_Same_Origin, 

        /// <summary>
        /// 
        /// </summary>
        [EnumRecordProperties("allow-scripts")]
        Allow_Scripts, 

        /// <summary>
        /// 
        /// </summary>
        [EnumRecordProperties("allow-top-navigation")]
        Allow_To_Navigation, 

        /// <summary>
        /// 
        /// </summary>
        [EnumRecordProperties("allow-top-navigation-by-user-activation")]
        Allow_Top_Navigation_By_User_Activation,



    }
}

