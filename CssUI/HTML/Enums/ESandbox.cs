using EnumRecords;

namespace CssUI.HTML
{
    [EnumRecord<KeywordProperties>]
    public enum ESandbox : int
    {/* Docs: https://html.spec.whatwg.org/multipage/iframe-embed-object.html#attr-iframe-sandbox */

        /// <summary>
        ///
        /// </summary>
        [EnumData("allow-forms")]
        Allow_Forms,

        /// <summary>
        ///
        /// </summary>
        [EnumData("allow-modals")]
        Allow_Modals,

        /// <summary>
        ///
        /// </summary>
        [EnumData("allow-orientation-lock")]
        Allow_Orientation_Lock,

        /// <summary>
        ///
        /// </summary>
        [EnumData("allow-pointer-lock")]
        Allow_Pointer_Lock,

        /// <summary>
        ///
        /// </summary>
        [EnumData("allow-popups")]
        Allow_Popups,

        /// <summary>
        ///
        /// </summary>
        [EnumData("allow-popups-to-escape-sandbox")]
        Allow_Popups_To_Escape_Sandbox,

        /// <summary>
        ///
        /// </summary>
        [EnumData("allow-presentation")]
        Allow_Presentation,

        /// <summary>
        ///
        /// </summary>
        [EnumData("allow-same-origin")]
        Allow_Same_Origin,

        /// <summary>
        ///
        /// </summary>
        [EnumData("allow-scripts")]
        Allow_Scripts,

        /// <summary>
        ///
        /// </summary>
        [EnumData("allow-top-navigation")]
        Allow_To_Navigation,

        /// <summary>
        ///
        /// </summary>
        [EnumData("allow-top-navigation-by-user-activation")]
        Allow_Top_Navigation_By_User_Activation,



    }
}

