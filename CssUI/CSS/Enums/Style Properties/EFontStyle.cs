using EnumRecords;

namespace CssUI.CSS;

[EnumRecord<KeywordProperties>]
public enum EFontStyle
{/* Docs: https://www.w3.org/TR/2018/REC-css-fonts-3-20180920/#font-style-prop */

    /// <summary>
    /// selects a face that is classified as a normal face, one that is neither italic or obliqued
    /// </summary>
    [EnumRecordProperties("normal")]
    Normal,

    /// <summary>
    /// selects a font that is labeled as an italic face, or an oblique face if one is not
    /// </summary>
    [EnumRecordProperties("italic")]
    Italic,

    /// <summary>
    /// selects a font that is labeled as an oblique face, or an italic face if one is not
    /// </summary>
    [EnumRecordProperties("oblique")]
    Oblique,
}

