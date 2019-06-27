
namespace CssUI
{
    public enum EPropertySystemDirtFlags : byte
    {
        /// <summary>
        /// Not dirty
        /// </summary>
        Clean = 0,
        /// <summary>
        /// The property system needs to re-cascade its properties
        /// </summary>
        Cascade = (1 << 0),
        /// <summary>
        /// The property system needs to resolve its block property values
        /// </summary>
        Block = (1 << 1),
        /// <summary>
        /// The property system needs to resolve its font values
        /// </summary>
        Font = (1 << 2)
    }
}
