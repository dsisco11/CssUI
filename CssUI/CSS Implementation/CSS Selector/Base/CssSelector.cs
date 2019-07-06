using System;
using System.Collections.Generic;
using System.Linq;

namespace CssUI.CSS
{
    // XXX: Alter the selector matching code to perform matching from right-to-left (way more efficient)
    // DOCS: https://www.w3.org/TR/selectors-4

    /// <summary>
    /// Represents a CSS selector that can perform matching
    /// </summary>
    public class CssSelector
    {
        #region Properties
        /// <summary>
        /// Whether this selector is coming from a stylesheet we loaded or from some internal styling rule defined directly in code.
        /// </summary>
        private readonly bool IsFromStylesheet = false;
        /// <summary>
        /// List of all selector instances for this selector
        /// </summary>
        readonly CssSelectorList Selectors;

        #endregion

        #region Constructors
        public CssSelector(string SelectorString)
        {
            this.IsFromStylesheet = false;
            this.Selectors = Parse_Selector(SelectorString);
        }
        public CssSelector(bool IsFromStylesheet, string SelectorString)
        {
            this.IsFromStylesheet = IsFromStylesheet;
            this.Selectors = Parse_Selector(SelectorString);
        }
        #endregion

        #region Parsing
        /// <summary>
        /// </summary>
        /// <param name="SelectorString"></param>
        /// <returns></returns>
        public static CssSelectorList Parse_Selector(string SelectorString)
        {// DOCS: https://www.w3.org/TR/selectors-3/#selectors
            CssSelectorParser Parser = new CssSelectorParser(SelectorString);
            return Parser.Parse_Selector_List();
        }
        #endregion
        
        /// <summary>
        /// Returns the greatest specificity value that matches the given element
        /// </summary>
        /// <returns></returns>
        public long Get_Specificity(cssElement E)
        {
            if (Selectors == null)
                return 0;

            return Selectors.Get_Specificity(E, IsFromStylesheet).Max();
        }

        /// <summary>
        /// Returns a list of all selector specificitys that match the given element
        /// </summary>
        /// <returns></returns>
        public IList<long> Get_Specificity_List(cssElement E)
        {
            if (Selectors == null)
                return null;

            return Selectors.Get_Specificity(E, IsFromStylesheet);
        }

        /// <summary>
        /// Querys the selector against a single <see cref="cssElement"/> to determine if it matches it
        /// </summary>
        /// <returns>True/False Match</returns>
        public bool QuerySingle(cssElement E)
        {
            return Selectors.Query(E);
        }

        /// <summary>
        /// Querys the selector against all of the elements within the given root elements tree to determine which ones it matches
        /// </summary>
        public List<cssElement> Query(cssCompoundElement Root)
        {
            LinkedList<cssElement> MatchList = Root.Get_All_Descendants();
            return Selectors.Query(MatchList, ESelectorMatchingOrder.LTR).ToList();
        }

        /// <summary>
        /// Querys the selector against all of the elements within all of the given root elements trees to determijne which ones it matches
        /// </summary>
        public List<cssElement> Query(List<cssCompoundElement> Roots)
        {// DOCS: https://www.w3.org/TR/selectors-4/#match-a-selector-against-a-tree
            LinkedList<cssElement> MatchList = new LinkedList<cssElement>();
            foreach(cssRootElement Root in Roots)
            {
                if (Root.Root != Roots[0].Root) throw new Exception("All element trees must belong to the same root element!");
                Root.Get_All_Descendants(MatchList);
            }

            return Selectors.Query(MatchList, ESelectorMatchingOrder.LTR).ToList();
            /*
            if (Selectors.Count == 1)// Most selectors will just have a single instance
            {
                Selectors[0].Match(MatchList, ESelectorMatchDir.LTR);
                return MatchList;
            }
            else if (Selectors.Count > 1)
            {
                HashSet<uiElement> retVal = new HashSet<uiElement>();
                foreach (CssComplexSelectorList Selector in Selectors)
                {
                    var tmpMatchList = new List<uiElement>(MatchList);
                    Selector.Match(tmpMatchList, ESelectorMatchDir.LTR);
                    foreach (uiElement E in tmpMatchList)
                    {
                        retVal.Add(E);
                    }
                }
                return retVal.ToList();
            }

            return new List<uiElement>(0);
            */
        }

    }
    
}
