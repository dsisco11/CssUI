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

        /* XXX: Replace these with something more efficient. perhapse statically assigning and tracking these values within the ChildNodesList */

#if USE_FUNCTIONS_FOR_NODE_RELATIONSHIP_LINKS
        public IEnumerable<Element> children => childNodes.Where(c => c is Element).Cast<Element>();
        public int childElementCount => childNodes.Count(c => c is Element);
        public Element firstElementChild => (Element)childNodes.FirstOrDefault(c => c is Element);
        public Element lastElementChild => (Element)childNodes.LastOrDefault(c => c is Element);
#else
        public LinkedList<Element> children => childNodes.ChildElements;
        public int childElementCount => childNodes.ChildElements?.Count ?? 0;
        public Element firstElementChild => childNodes.ChildElements?.First?.Value;
        public Element lastElementChild => childNodes.ChildElements?.Last?.Value;
#endif
        #endregion

        #region Accessors
        #endregion

        #region Constructors
        public ParentNode() : base()
        {
        }
        #endregion

        [CEReactions]
        public void prepend(params object[] nodes)
        {
            CEReactions.Wrap_CEReaction(nodeDocument.defaultView, () =>
            {
                if (nodes.Any(c => !(c is Node) && !(c is string)))
                    throw new TypeError("Only Node and string types are accepted types.");

                var node = Dom_convert_nodes_into_node(ownerDocument, nodes);
                Dom_pre_insert_node(node, this, firstChild);
            });
        }

        [CEReactions]
        public void append(params object[] nodes)
        {
            CEReactions.Wrap_CEReaction(nodeDocument.defaultView, () =>
            {
                if (nodes.Any(c => !(c is Node) && !(c is string)))
                    throw new TypeError("Only Node and string types are accepted types.");

                var node = Dom_convert_nodes_into_node(ownerDocument, nodes);
                appendChild(node);
            });
        }

        /// <summary>
        /// Returns the first element that is a descendant of node that matches selectors.
        /// </summary>
        /// <param name="selectors"></param>
        /// <returns></returns>
        public Element querySelector(string selector)
        {
            var results = DOMCommon.Scope_Match_Selector_String(this, selector);
            if (results.Any())
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
