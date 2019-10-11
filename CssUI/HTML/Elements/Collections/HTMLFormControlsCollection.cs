using CssUI.DOM;
using CssUI.DOM.Enums;
using System.Linq;

namespace CssUI.HTML
{
    public class HTMLFormControlsCollection : HTMLCollection<HTMLElement>
    {/* Docs: https://html.spec.whatwg.org/multipage/common-dom-interfaces.html#htmlformcontrolscollection */

        #region Constructors
        public HTMLFormControlsCollection(HTMLFormElement root) : base(root, new FilterFormOwner(root))
        {
        }
        #endregion

        /// <summary>
        /// Returns the item with ID or name name from the collection.
        /// If there are multiple matching items, then a RadioNodeList object containing all those elements is returned.
        /// </summary>
        public new dynamic this[string name]
        {
            get
            {
                if (ReferenceEquals(null, name) || name.Length <= 0)
                {
                    return null;
                }

                var matched = DOMCommon.Get_Descendents<HTMLElement>(root, new FilterNamedElement(name), ENodeFilterMask.SHOW_ELEMENT);
                if (matched.Count <= 0)
                {
                    return null;
                }
                else if (matched.Count == 1)
                {
                    return matched.First();
                }
                else
                {
                    return new RadioNodeList(matched.ToArray());
                }
            }
        }
    }
}
