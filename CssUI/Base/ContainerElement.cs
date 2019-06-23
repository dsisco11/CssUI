using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CssUI
{
    /// <summary>
    /// Implements a UI element which allows external code to add elements to it
    /// </summary>
    public abstract class ContainerElement : ScrollableElement
    {
        #region Constructors
        public ContainerElement(string ID) : base(ID)
        {
        }
        #endregion

        #region Child Management
        /// <summary>
        /// Adds an element to this one
        /// </summary>
        new public void Add(uiElement element)
        {
            base.Add(element);
        }

        /// <summary>
        /// Removes a given element from this one
        /// </summary>
        /// <param name="element">The element to be removed</param>
        /// <param name="preserve">If TRUE then the element will not be disposed of immediately</param>
        /// <returns>Success</returns>
        new public bool Remove(uiElement element, bool preserve = false)
        {
            return base.Remove(element);
        }

        /// <summary>
        /// Removes a given element, specified by it's ID, from this one 
        /// </summary>
        /// <param name="ID">ID of the element to be removed</param>
        /// <param name="preserve">If TRUE then the element will not be disposed of immediately</param>
        /// <returns>Success</returns>
        new public bool Remove(string ID, bool preserve = false)
        {
            return Remove(Get(ID), preserve);
        }

        /// <summary>
        /// Clears and disposes of all controls
        /// </summary>
        new public void Clear()
        {
            base.Clear();
        }

        /// <summary>
        /// Fetches the first child-element matching a given CSS selector
        /// </summary>
        new public uiElement Get(string Selector)
        {
            return Find(Selector).SingleOrDefault();
        }

        /// <summary>
        /// Fetches the first child-element matching a given CSS selector
        /// </summary>
        new public Ty Get<Ty>(string Selector) where Ty : uiElement
        {
            return (Ty)Find(Selector).SingleOrDefault();
        }
        #endregion

    }
}
