using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CssUI.CSS.Selector;

namespace CssUI.CSS
{
    // TODO: Alter the selector matching code to perform matching from right-to-left (way more efficient)

    /// <summary>
    /// Describes the basis for all selector filtering classes
    /// <para>
    /// The way we organize our selector hierarchy:
    /// Rather then being forced to wrap a compound-selector within a complex-selector even when the complex one has no combinator,
    /// We instead define compound and complex selectors as "SelectorFilters" because each of those types is fundamentally just altering a list of <see cref="uiElement"/>s passed to them, removing any items which they don't match.
    /// By doing this we can instead just store a list of those filter objects and simplify things a little bit more.
    /// </para>
    /// </summary>
    public abstract class SelectorFilter
    {

        /// <summary>
        /// Querys a list of uiElements, modifying the list by removing all elements that dont match this filter and then returning whether there were any matches.
        /// </summary>
        /// <param name="MatchList">A linked-list of all the elements we want to query, we use a linked list because non-matches are removed mid-iteration</param>
        /// <param name="Dir">The order in which matching will be executed</param>
        /// <returns>Matches</returns>
        public abstract bool Query(LinkedList<uiElement> MatchList, ESelectorMatchingOrder Dir);

        public abstract List<CssSimpleSelector> Get_Selectors();
    }

    /// <summary>
    /// A Compound selector is one that consists of multiple simple selectors
    /// In addition a Compound selector can contain at most ONE type-selector and if it does, that must be the first selector within it.
    /// </summary>
    public class CssCompoundSelector : SelectorFilter
    {// SEE:  https://drafts.csswg.org/selectors-4/#typedef-compound-selector

        public readonly List<CssSimpleSelector> Selectors = null;
        #region Constructors
        public CssCompoundSelector()
        {
            this.Selectors = new List<CssSimpleSelector>();
        }

        public CssCompoundSelector(IEnumerable<CssSimpleSelector> Collection)
        {
            this.Selectors = new List<CssSimpleSelector>(Collection);
            CssSimpleSelector ts = Selectors.FirstOrDefault(o => o is CssTypeSelector);
            if (ts != null && ts != Selectors[0] || Selectors.Count(o => o is CssTypeSelector) > 1)
                throw new CssSyntaxError("Compound selectors can only contain a single type-selector and it MUST be the first selector in the list!");
        }

        public CssCompoundSelector(CssCompoundSelector Compound) : this(Compound.Selectors)
        {
        }
        #endregion

        public override List<CssSimpleSelector> Get_Selectors() { return Selectors; }
        public override bool Query(LinkedList<uiElement> MatchList, ESelectorMatchingOrder Order)
        {
            // Filter the matchlist with our simple selectors first
            LinkedListNode<uiElement> node = MatchList.First;
            do
            {
                bool bad = false;// if True then we remove the node from our list
                switch (Order)
                {
                    case ESelectorMatchingOrder.LTR:
                        {
                            for (int i = 0; i < Selectors.Count; i++)// progressing forwards
                            {
                                CssSimpleSelector Selector = Selectors[i];
                                if (!Selectors[i].Matches(node.Value))
                                {
                                    bad = true;
                                    break;
                                }
                            }
                        }
                        break;
                    case ESelectorMatchingOrder.RTL:
                        {
                            for (int i = Selectors.Count - 1; i >= 0; i--)// progressing backwards
                            {
                                CssSimpleSelector Selector = Selectors[i];
                                if (!Selectors[i].Matches(node.Value))
                                {
                                    bad = true;
                                    break;
                                }
                            }
                        }
                        break;
                }
                            
                if (bad)
                {
                    var curr = node;// Track current node just incase next is null or something
                    node = node.Next;// Progress
                    MatchList.Remove(curr);// Remove this element
                }
                else
                {
                    node = node.Next;
                }
            }
            while (node.Next != null);

            return true;
        }
    }

    /// <summary>
    /// A Complex selector consists of a Compound selector followed by a Combinator
    /// </summary>
    public class CssComplexSelector : CssCompoundSelector
    {// SEE:  https://drafts.csswg.org/selectors-4/#ref-for-typedef-complex-selector-1
        /// <summary>
        /// The combinator following the selector sequence.
        /// </summary>
        public readonly ESelectorCombinator Combinator;

        /// <summary>
        /// Creates a new selector sequence
        /// </summary>
        /// <param name="Chain"></param>
        /// <param name="Combinator">The combinator that comes AFTER the selector sequence.</param>
        public CssComplexSelector(ESelectorCombinator Combinator, CssCompoundSelector Compound) : base(Compound)
        {
            this.Combinator = Combinator;
        }

        public override bool Query(LinkedList<uiElement> MatchList, ESelectorMatchingOrder Order)
        {
            switch (Order)
            {
                case ESelectorMatchingOrder.LTR:
                    {
                        base.Query(MatchList, Order);
                        Apply_Combinator(MatchList, Order);
                    }
                    break;
                case ESelectorMatchingOrder.RTL:
                    {
                        Apply_Combinator(MatchList, Order);
                        base.Query(MatchList, Order);
                    }
                    break;
            }

            return true;
        }

        void Apply_Combinator(LinkedList<uiElement> MatchList, ESelectorMatchingOrder Dir)
        {
            // Transform the matchlist using our combinator
            switch (Combinator)
            {
                default:
                    throw new NotImplementedException(string.Concat("[CSS][Selector] Unhandled selector-combinator(", Enum.GetName(typeof(ESelectorCombinator), Combinator), ")!"));
            }
        }
    }

    /// <summary>
    /// Represents a collection of selector filtering items
    /// </summary>
    public class SelectorFilterSet : List<SelectorFilter>
    {
        /// <summary>
        /// Selector specificity as defined in the CSS 2.1 specification documentation
        /// </summary>
        public long Specificity { get; private set; }

        public SelectorFilterSet()
        {
        }

        public bool Query(LinkedList<uiElement> MatchList, ESelectorMatchingOrder Dir)
        {
            foreach(CssComplexSelector Sequence in this)
            {
                // We are filtering the match list
                Sequence.Query(MatchList, Dir);
            }

            return true;// there were SOME matches
        }

        /// <summary>
        /// Returns the selectors specificity as defined in the CSS 2.1 specification documentation
        /// </summary>
        private long Get_Specificity(bool IsFromStylesheet)
        {// SEE:  https://www.w3.org/TR/2011/REC-css3-selectors-20110929/#specificity
            long A = 0, B = 0, C = 0, D = 0;
            if (IsFromStylesheet) A = 1;
            
            foreach(SelectorFilter Filter in this)
            {
                foreach(CssSimpleSelector Simple in Filter.Get_Selectors())
                {
                    switch (Simple.Type)
                    {
                        case ECssSimpleSelectorType.IDSelector:
                            B += 1;
                            break;
                        case ECssSimpleSelectorType.AttributeSelector:
                        case ECssSimpleSelectorType.ClassSelector:
                        case ECssSimpleSelectorType.PseudoClassSelector:
                            C += 1;
                            break;
                        case ECssSimpleSelectorType.TypeSelector:
                        case ECssSimpleSelectorType.PseudoElementSelector:
                            D += 1;
                            break;
                    }
                }
            }

            return ((A << 00) | (B << 16) | (C << 32) | (D << 48));
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public class CssSelectorList : List<SelectorFilterSet>
    {
        public bool Query(uiElement E)
        {
            LinkedList<uiElement> List = new LinkedList<uiElement>();
            List.AddFirst(E);

            List<uiElement> res = Query(List, ESelectorMatchingOrder.LTR);

            return res.Contains(E);
        }

        public List<uiElement> Query(LinkedList<uiElement> MatchList, ESelectorMatchingOrder Dir)
        {
            if (this.Count == 1)// Most selectors will just have a single instance
            {
                this[0].Query(MatchList, Dir);
                return new List<uiElement>(MatchList);
            }
            else if (this.Count > 1)
            {
                // If we contain more than a single filter set then the list of matching items we return becomes a collection of any items that were matches by any of our filter sets
                HashSet<uiElement> retVal = new HashSet<uiElement>();
                foreach (SelectorFilterSet FilterSet in this)
                {
                    // Create a clone of our initial list that this filter set can alter
                    var tmpMatchList = new LinkedList<uiElement>(MatchList);
                    if (!FilterSet.Query(tmpMatchList, Dir)) continue;
                    foreach (uiElement E in tmpMatchList)
                    {
                        retVal.Add(E);
                    }
                }

                return new List<uiElement>(retVal);
            }

            return null;
        }
    }


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
        public static CssSelectorList Parse_Selector(string SelectorString)
        {// SEE:  https://www.w3.org/TR/2011/REC-css3-selectors-20110929/#selectors
            
            CssSelectorParser Parser = new CssSelectorParser(SelectorString);
            return Parser.Parse_Selector_List();
        }
        #endregion
        
        /// <summary>
        /// Querys the selector against a single <see cref="uiElement"/> to determine if it matches it
        /// </summary>
        /// <returns>True/False Match</returns>
        public bool QuerySingle(uiElement E)
        {
            return Selectors.Query(E);
        }

        /// <summary>
        /// Querys the selector against all of the elements within the given root elements tree to determine which ones it matches
        /// </summary>
        public List<uiElement> Query(CompoundElement Root)
        {
            LinkedList<uiElement> MatchList = Root.Get_All_Descendants();
            return Selectors.Query(MatchList, ESelectorMatchingOrder.LTR).ToList();
        }

        /// <summary>
        /// Querys the selector against all of the elements within all of the given root elements trees to determijne which ones it matches
        /// </summary>
        public List<uiElement> Query(List<CompoundElement> Roots)
        {// SEE:  https://drafts.csswg.org/selectors-4/#evaluating-selectors
            LinkedList<uiElement> MatchList = new LinkedList<uiElement>();
            foreach(RootElement Root in Roots)
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
