using CssUI.Internal;

namespace CssUI.DOM
{
    [MetaEnum]
    public enum ESelectionType : int
    {
        [MetaKeyword("None")]
        None,
        [MetaKeyword("Caret")]
        Caret,
        [MetaKeyword("Range")]
        Range,
    }
}
