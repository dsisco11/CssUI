using CssUI.Internal;

namespace CssUI.CSS
{
    /// <summary>
    /// 
    /// </summary>
    /// Docs: https://www.w3.org/TR/css-writing-modes-4/#logical-to-physical
    [MetaEnum]
    public enum EWritingMode : int
    {
        [MetaKeyword("horizontal-tb")]
        Horizontal_TB = 1,
        [MetaKeyword("vertical-rl")]
        Vertical_RL,
        [MetaKeyword("vertical-lr")]
        Vertical_LR,
        [MetaKeyword("sideways-rl")]
        Sideways_RL,
        [MetaKeyword("sideways-lr")]
        Sideways_LR,
    }
}
