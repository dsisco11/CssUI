namespace CssUI.CSS.Enums
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
        NeedsToCascade = 1 << 0,
        /// <summary>
        /// The property system needs to resolve its block property values
        /// </summary>
        NeedsToResolveBlock = 1 << 1,
        /// <summary>
        /// The property system needs to resolve its font values
        /// </summary>
        NeedsToResolveFont = 1 << 2
    }
}
