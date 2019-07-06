using CssUI.Internal;

namespace CssUI.Enums
{
    /// <summary>
    /// 
    /// </summary>
    /// Docs: https://www.w3.org/TR/css-writing-modes-4/#logical-to-physical
    [CssEnum]
    public enum EWritingMode : int
    {
        [CssKeyword("horizontal-tb")]
        Horizontal_TB = 1,
        [CssKeyword("vertical-rl")]
        Vertical_RL,
        [CssKeyword("vertical-lr")]
        Vertical_LR,
        [CssKeyword("sideways-rl")]
        Sideways_RL,
        [CssKeyword("sideways-lr")]
        Sideways_LR,
    }
}
