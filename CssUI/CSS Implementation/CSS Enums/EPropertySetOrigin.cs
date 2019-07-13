
namespace CssUI.CSS.Internal
{
    public enum EPropertySetOrigin : uint
    {
        /// <summary>
        /// Applys to Properties which would normally be set by a browser, default properties
        /// </summary>
        UserAgent = 0,
        User = 1,
        /// <summary>
        /// Applys to proerties which are specified within a stylesheet
        /// </summary>
        Author = 2
    }
}
