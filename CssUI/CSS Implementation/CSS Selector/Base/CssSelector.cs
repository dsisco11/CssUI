using CssUI.CSS.Selectors;
using CssUI.DOM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using xLog;

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
        private static ILogger Log = LogFactory.GetLogger(nameof(CssSelector));
        /// <summary>
        /// Whether this selector is coming from a stylesheet we loaded or from some internal styling rule defined directly in code.
        /// </summary>
        private readonly bool IsFromStylesheet = false;
        /// <summary>
        /// List of all selector instances for this selector
        /// </summary>
        private readonly SelectorList Selectors;

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
        private static SelectorList Parse_Selector(string SelectorString)
        {// DOCS: https://www.w3.org/TR/selectors-3/#selectors
            try
            {
                CssSelectorParser Parser = new CssSelectorParser(SelectorString);
                return Parser.Parse_Selector_List();
            }
            catch (CssException ex)
            {
                Log.Error(ex);
            }
            return null;
        }
        #endregion
        
        /// <summary>
        /// Returns the greatest specificity value that matches the given element
        /// </summary>
        /// <returns></returns>
        public long Get_Specificity(Element E)
        {
            if (Selectors == null)
                return 0;

            return Selectors.Get_Specificity(E, IsFromStylesheet).Max();
        }

        /// <summary>
        /// Returns a list of all selector specificitys that match the given element
        /// </summary>
        /// <returns></returns>
        public IList<long> Get_Specificity_List(Element E)
        {
            if (Selectors == null)
                return null;

            return Selectors.Get_Specificity(E, IsFromStylesheet);
        }

        /// <summary>
        /// Querys the selector against a single <see cref="Element"/> to determine if it matches it
        /// </summary>
        /// <returns>True/False Match</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool QuerySingle(Element E)
        {
            return Selectors.Query(E, ESelectorMatchingOrder.RTL);
        }

        public bool Match(Element E)
        {

        }


    }
    
}
