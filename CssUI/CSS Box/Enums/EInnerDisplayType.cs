
namespace CssUI.Enums
{
    /// <summary>
    /// the inner display type, which defines (if it is a non-replaced element) the kind of formatting context it generates, 
    /// dictating how its descendant boxes are laid out. (The inner display of a replaced element is outside the scope of CSS.)
    /// </summary>
    public enum EInnerDisplayType : int
    {
        Flow = 0,
        Flow_Root,
        Table,
        Flex,
        Grid,
        Ruby
    }
}
