using CssUI.DOM;
using CssUI.DOM.Nodes;
using CssUI.CSS.Internal;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System;

namespace CssUI.CSS.Selectors
{
    /// <summary>
    /// A relative selector consists of a combinator and a compound selector
    /// </summary>
    public class RelativeSelector : CompoundSelector
    {/* Docs: https://drafts.csswg.org/selectors-4/#typedef-relative-selector */

        #region Properties
        /// <summary>
        /// The combinator following the selector sequence.
        /// </summary>
        public readonly ESelectorCombinator Combinator;
        #endregion

        #region Constructors
        /// <summary>
        /// Creates a new selector sequence
        /// </summary>
        /// <param name="Chain"></param>
        /// <param name="Combinator">The combinator that comes AFTER the selector sequence.</param>
        public RelativeSelector(ESelectorCombinator Combinator, CompoundSelector Compound) : base(Compound)
        {
            this.Combinator = Combinator;
        }
        #endregion



        /// <summary>
        /// Takes in a list of elements and performs matching on them, modifying and outputting a new list of elements to be matched by the next Complex selector in the chain.
        /// </summary>
        /// <param name="element"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Match(IEnumerable<Element> MatchList, out LinkedList<Element> outMatchList, params Node[] scopeElements)
        {/* Docs: https://drafts.csswg.org/selectors-4/#match-a-complex-selector-against-an-element */
            if (Count <= 0)
            {
                outMatchList = (LinkedList<Element>)MatchList;
                return false;
            }
            /*
             * Takes in a list of elements to match against.
             * Performs matching on all given elements, putting any that match into a new list.
             * Applies the selectors combinator logic to all items in new list
             * Returns the new list of elements that should be matched against the next Complex selector
            */
            /* 1) Perform matching on all elements in MatchList, putting any matches in a new list */
            base.Match(MatchList, out LinkedList<Element> matches, ESelectorMatchingOrder.RTL, scopeElements);
            /* 2) For all matched elements, apply to them the combinator and append the transformed element list to the hash-set */
            var matchSet = new HashSet<Element>();
            foreach (Element element in matches)
            {
                var modList = Apply_Combinator(element, ESelectorMatchingOrder.RTL);
                matchSet.UnionWith(modList);
            }
            /* 3) Return the transformed results */
            outMatchList = new LinkedList<Element>(matchSet);
            return matchSet.Count > 0;
        }

        /// <summary>
        /// Applies the Complex selectors combinator to a single element
        /// </summary>
        /// <param name="element"></param>
        /// <returns></returns>
        public IEnumerable<Element> Apply_Combinator(Element element, ESelectorMatchingOrder Dir)
        {

            switch (Combinator)
            {
                case ESelectorCombinator.Child:
                    {
                        switch (Dir)
                        {
                            case ESelectorMatchingOrder.LTR:
                                return new LinkedList<Element>(element.children);
                            default:
                                return new Element[] { element.parentElement };
                        }
                    }
                case ESelectorCombinator.Sibling_Adjacent:
                    {
                        switch (Dir)
                        {
                            case ESelectorMatchingOrder.LTR:
                                return new Element[] { element.nextElementSibling };
                            default:
                                return new Element[] { element.previousElementSibling };
                        }

                    }
                case ESelectorCombinator.Sibling_Subsequent:
                    {
                        switch (Dir)
                        {
                            case ESelectorMatchingOrder.LTR:
                                return (LinkedList<Element>)DOMCommon.Get_Following(element, FilterElements.Instance);
                            default:
                                return (LinkedList<Element>)DOMCommon.Get_Preceeding(element, FilterElements.Instance);
                        }
                    }
                case ESelectorCombinator.Descendant:
                    {
                        switch (Dir)
                        {
                            case ESelectorMatchingOrder.LTR:
                                return (LinkedList<Element>)DOMCommon.Get_Descendents(element, FilterElements.Instance);
                            default:
                                return (LinkedList<Element>)DOMCommon.Get_Ancestors(element, FilterElements.Instance);
                        }
                    }
                case ESelectorCombinator.None:
                    {
                        return new Element[] { element };
                    }
                default:
                    throw new NotImplementedException($"[CSS][Selector] Unhandled selector-combinator({Enum.GetName(typeof(ESelectorCombinator), Combinator)})!");
            }



        }


    }
}
