namespace CssUI.CSS.Internal;

/// <summary>
/// CSS rule type constants per CSSOM §6.4.2.
/// </summary>
/// <seealso href="https://www.w3.org/TR/cssom-1/#the-cssrule-interface"/>
public enum ECssRuleType : ushort
{
    STYLE_RULE = 1,
    //CHARSET_RULE = 2, // historical
    IMPORT_RULE = 3,
    MEDIA_RULE = 4,
    FONT_FACE_RULE = 5,
    PAGE_RULE = 6,
    KEYFRAMES_RULE = 7,
    KEYFRAME_RULE = 8,
    MARGIN_RULE = 9,
    NAMESPACE_RULE = 10,
    COUNTER_STYLE_RULE = 11,
    SUPPORTS_RULE = 12,
    FONT_FEATURE_VALUES_RULE = 14,
    LAYER_BLOCK_RULE = 16,
    LAYER_STATEMENT_RULE = 17
}

