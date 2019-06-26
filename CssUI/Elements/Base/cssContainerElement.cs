using System.Linq;

namespace CssUI
{
    /// <summary>
    /// Implements a UI element which allows external code to add elements to it
    /// </summary>
    public abstract class cssContainerElement : cssScrollableElement
    {
        #region Constructors
        public cssContainerElement(string ID) : base(ID)
        {
        }
        #endregion

        #region Child Management

        /// <summary>
        /// Fetches the first child-element matching a given CSS selector
        /// </summary>
        new public cssElement Get(string Selector)
        {
            return Find(Selector).SingleOrDefault();
        }

        /// <summary>
        /// Fetches the first child-element matching a given CSS selector
        /// </summary>
        new public Ty Get<Ty>(string Selector) where Ty : cssElement
        {
            return (Ty)Find(Selector).SingleOrDefault();
        }
        #endregion

    }
}
