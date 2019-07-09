using CssUI.DOM;
using System.Collections.Generic;

namespace CssUI.CSS.Selectors
{
    /// <summary>
    /// Describes the basis for all selector filtering classes
    /// <para>
    /// The way we organize our selector hierarchy:
    /// Rather then being forced to wrap a compound-selector within a complex-selector even when the complex one has no combinator,
    /// We instead define compound and complex selectors as "SelectorFilters" because each of those types is fundamentally just altering a list of <see cref="cssElement"/>s passed to them, removing any items which they don't match.
    /// By doing this we can instead just store a list of those filter objects and simplify things a little bit more.
    /// </para>
    /// </summary>
    public interface ISelectorFilter
    {

        /// <summary>
        /// Querys a list of uiElements, modifying the list by removing all elements that dont match this filter and then returning whether there were any matches.
        /// </summary>
        /// <param name="MatchList">A linked-list of all the elements we want to query, we use a linked list because non-matches are removed mid-iteration</param>
        /// <param name="Dir">The order in which matching will be executed</param>
        /// <returns>Matches</returns>
        bool Query(LinkedList<Element> MatchList, ESelectorMatchingOrder Dir);

        List<SimpleSelector> Get_Selectors();
    }
}
