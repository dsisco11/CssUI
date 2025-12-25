using EnumRecords;

namespace CssUI.CSS.Media;

[EnumRecord<KeywordProperties>]
public enum EMediaOrientation
{
    /// <summary>
    /// The orientation media feature is portrait when the value of the height media feature is greater than or equal to the value of the width media feature.
    /// </summary>
    [EnumData("portrait")]
    Portrait,
    /// <summary>
    /// The orientation media feature is portrait when the value of the width media feature is greater than or equal to the value of the height media feature.
    /// </summary>
    [EnumData("landscape")]
    Landscape,
}

