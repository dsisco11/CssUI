using CssUI.CSS.Selectors;
using CssUI.DOM;
using CssUI.DOM.Nodes;
using CssUI.DOM.Traversal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using xLog;

namespace CssUI.CSS
{
    /// <summary>
    /// Represents a CSS selector that can perform matching,
    /// consisting of one or more whole (complex) selectors parsed from a single definition.
    /// </summary>
    public class CssSelector : List<ComplexSelector>
    {/* Docs: https://www.w3.org/TR/selectors-4 */
        #region Properties
        private static ILogger Log = LogFactory.GetLogger(nameof(CssSelector));
        /// <summary>
        /// Whether this selector is coming from a stylesheet we loaded or from some internal styling rule defined directly in code.
        /// </summary>
        private readonly bool IsFromStylesheet = false;

        #endregion

        #region Constructors
        public CssSelector(string Selector) : base(CssSelector.Parse(Selector))
        {
            this.IsFromStylesheet = false;
        }
        public CssSelector(bool IsFromStylesheet, string Selector) : base(CssSelector.Parse(Selector))
        {
            this.IsFromStylesheet = IsFromStylesheet;
        }

        private CssSelector(IEnumerable<ComplexSelector> Collection) : base(Collection)
        {
        }
        #endregion

        #region Parsing
        /// <summary>
        /// </summary>
        /// <param name="SelectorString"></param>
        /// <returns></returns>
        private static IEnumerable<ComplexSelector> Parse(string SelectorString)
        {// DOCS: https://www.w3.org/TR/selectors-3/#selectors
            try
            {
                SelectorParser Parser = new SelectorParser(SelectorString);
                return Parser.Parse_Selector_List();
            }
            catch (CssException ex)
            {
                Log.Error(ex);
            }
            return new ComplexSelector[] { };
        }
        #endregion

        /// <summary>
        /// Returns the highest selector specificity
        /// </summary>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public long Get_Specificity(Element E)
        {
            var retVal = new List<long>();

            foreach (ComplexSelector Complex in this)
            {
                if (Complex.Match(E))
                {
                    retVal.Add(Complex.Get_Specificity());
                }
            }

            return retVal.Max();
        }

        /// <summary>
        /// Returns a list of all selector specificitys that match the given element (<paramref name="E"/>)
        /// </summary>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public IReadOnlyList<long> Get_Specificitys(Element E)
        {
            var retVal = new List<long>();

            foreach (ComplexSelector Complex in this)
            {
                if (Complex.Match(E))
                {
                    retVal.Add(Complex.Get_Specificity());
                }
            }

            return retVal.ToArray();
        }

        
        /// <summary>
        /// Performs matching against a single element
        /// </summary>
        /// <param name="E"></param>
        /// <param name="scopeElements"></param>
        /// <returns></returns>
        public bool Match(Element E, params Node[] scopeElements)
        {/* Docs: https://drafts.csswg.org/selectors-4/#match-a-selector-against-an-element */
            
            /* We are a group of one or more complete(complex) selectors, so we only need this element to match ONE of those selectors */
            foreach (ComplexSelector Complex in this)
            {
                if (Complex.Match(E, scopeElements))
                    return true;
            }

            return false;
        }
        
        /// <summary>
        /// Performs matching against a single element
        /// </summary>
        /// <param name="E"></param>
        /// <param name="scopeElements"></param>
        /// <param name="scopingRoot">Element to limit the search to, only this element and its descendents will be matched against</param>
        /// <returns>All matched elements</returns>
        public IEnumerable<Element> Match_Against_Tree(IEnumerable<Node> rootElements, Node scopingRoot, params Node[] scopeElements)
        {/* Docs: https://drafts.csswg.org/selectors-4/#match-a-selector-against-a-tree */
            if (rootElements.Count() <= 0)
                return new Element[] { };

            /* "All of the root elements must share the same root, or else calling this algorithm is invalid." */
            Node treeRoot = rootElements.First().getRootNode();
            foreach(var element in rootElements)
            {
                if (!ReferenceEquals(treeRoot, element.getRootNode()))
                    throw new CssSelectorException("When matching a selector against a tree, all rootElements must share the same root!");
            }

            /* Resolve scoping */
            if (scopeElements.Count() <= 0)
            {
                if (scopingRoot != null)
                {
                    scopeElements = new Node[] { scopingRoot };
                }
                else
                {
                    scopeElements = new List<Node>(rootElements).ToArray();
                }
            }

            /*
             * This algorithm returns a (possibly empty) list of elements.
             * Start with a list of candidate elements, which are the the root elements and all of their descendant elements, sorted in shadow-including tree order, unless otherwise specified.
             * If an optional scoping root was provided, then remove from the candidate elements any elements that are not descendants of the scoping root.
             * Initialize the selector match list to empty.
             * For each element in the set of candidate elements:
             * If the result of match a selector against an element for element and selector is success, add element to the selector match list.
             * For each possible pseudo-element associated with element that is one of the pseudo-elements allowed to show up in the match list, if the result of match a selector against a pseudo-element for the pseudo-element and selector is success, add the pseudo-element to the selector match list.
            */
            NodeFilter Filter = null;
            if (scopingRoot != null)
                Filter = new FilterDescendantOf(scopingRoot);

            LinkedList<Node> candidateElements = new LinkedList<Node>();
            foreach (Node root in rootElements)
            {
                var rootList = DOMCommon.Get_Shadow_Including_Inclusive_Descendents(root, Filter, DOM.Enums.ENodeFilterMask.SHOW_ELEMENT);
                LinkedListNode<Node> firstNode = ((LinkedList<Node>)rootList).First;
                candidateElements.AddLast(firstNode);
            }

            LinkedList<Element> matchList = new LinkedList<Element>();
            foreach(Element candidate in candidateElements)
            {
                if (Match(candidate, scopeElements))
                    matchList.AddLast(candidate);
            }
            /* For each possible pseudo-element associated with element that is one of the pseudo-elements allowed to show up in the match list, if the result of match a selector against a pseudo-element for the pseudo-element and selector is success, add the pseudo-element to the selector match list. */
            /* XXX: pseudo-element Need to implement this stuff */

            return matchList.ToArray();
        }


    }
    
}
