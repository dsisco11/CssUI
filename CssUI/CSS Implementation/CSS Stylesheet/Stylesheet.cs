
namespace CssUI.CSS
{
    public abstract class Stylesheet
    {
        #region Properties
        public readonly string type;
        public readonly string href;
        public readonly object ownerNode;
        public readonly Stylesheet parentStylesheet;
        public readonly string title;
        public bool disabled;
        #endregion
    }
}
