using CssUI.DOM;
using System.Linq;

namespace CssUI
{
    /// <summary>
    /// Implements a UI element which allows external code to add elements to it
    /// </summary>
    public abstract class cssContainerElement : cssScrollableElement
    {
        #region Constructors
        public cssContainerElement(Document document, IParentElement Parent, string className = null, string ID = null) : base(document, Parent, className, ID)
        {
        }
        #endregion
        
    }
}
