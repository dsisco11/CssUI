using System;

namespace CssUI.Enums
{
    public enum ECssRuleType : UInt16
    {
        STYLE_RULE = 1,
        //CHARSET_RULE = 2, // historical
        IMPORT_RULE = 3,
        MEDIA_RULE = 4,
        FONT_FACE_RULE = 5,
        PAGE_RULE = 6,
        MARGIN_RULE = 9,
        NAMESPACE_RULE = 10
    }
}
