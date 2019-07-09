using System.Linq;
using CssUI.DOM.Nodes;
using System.Collections.Generic;
using CssUI.DOM.Exceptions;

namespace CssUI.DOM
{
    public abstract class ParentNode : Node
    {/* Docs: https://dom.spec.whatwg.org/#interface-parentnode */
        #region Properties
        /* The children attribute’s getter must return an HTMLCollection collection rooted at context object matching only element children. */
        public IEnumerable<Element> children { get => childNodes.Where(c => c is Element).Cast<Element>(); }
        public Element firstElementChild { get => (Element)childNodes.FirstOrDefault(c => c is Element); }
        public Element lastElementChild { get => (Element)childNodes.LastOrDefault(c => c is Element); }
        public int childElementCount { get => childNodes.Count(c => c is Element); }
        #endregion

        #region Accessors
        #endregion


        public void prepend(params object[] nodes)
        {
            if (nodes.Count(c=> !(c is Node) && !(c is string)) > 0)
                throw new TypeError("Only Node and string types are accepted types.");

            var node = Node._convert_nodes_into_node(ownerDocument, nodes);
            Node._pre_insert_node(node, this, firstChild);
        }

        public void append(params object[] nodes)
        {
            if (nodes.Count(c => !(c is Node) && !(c is string)) > 0)
                throw new TypeError("Only Node and string types are accepted types.");

            var node = Node._convert_nodes_into_node(ownerDocument, nodes);
            appendChild(node);
        }

        /// <summary>
        /// Returns the first element that is a descendant of node that matches selectors.
        /// </summary>
        /// <param name="selectors"></param>
        /// <returns></returns>
        public Element querySelector(string selector)
        {
            var results = DOMCommon.Scope_Match_Selector_String(this, selector);
            if (results.Count() > 0)
                return results.First();

            return null;
        }

        /// <summary>
        /// Returns all element descendants of node that match selectors.
        /// </summary>
        /// <param name="selectors"></param>
        /// <returns></returns>
        public IEnumerable<Node> querySelectorAll(string selectors)
        {
            return DOMCommon.Scope_Match_Selector_String(this, selectors);
        }
    }
}
