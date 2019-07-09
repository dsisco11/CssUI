using CssUI.DOM;
using CssUI.DOM.Nodes;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace CssUI.CSS.Selectors
{
    
    public class PseudoClassSelectorFunction : PseudoClassSelector
    {
        protected readonly List<CssToken> Args;

        public PseudoClassSelectorFunction(string Name, List<CssToken> Args = null) : base(Name)
        {
            this.Args = Args;
        }

        /// <summary>
        /// Returns whether the selector matches a specified element or index
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        override public bool Matches(Element E, params Node[] scopeElements)
        {
            switch (Name)
            {
                case "drop":
                    return PseudoClassFunctions.Drop(E, Args);
                default:
                    throw new CssSelectorException("[CSS] Selector pseudo-class (", Name, ") logic not implemented!");
            }
        }
    }

    public class PseudoClassSelectorAnBFunction : PseudoClassSelector
    {
        protected readonly CssAnBMatcher AnB;
        protected readonly IEnumerable<ComplexSelector> Selectors;

        public PseudoClassSelectorAnBFunction(string Name, CssTokenStream Stream) : base(Name)
        {
            AnB = CssAnBMatcher.Consume(Stream);
            if (Stream.Next.Type == ECssTokenType.Ident && string.Compare("or", (Stream.Next as IdentToken).Value)==0)
            {
                Stream.Consume();// Consume the 'or' string token
                Selectors = SelectorParser.Consume_Selector_List(Stream);
            }
        }

        /// <summary>
        /// Returns whether the selector matches a specified element or index
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        override public bool Matches(Element E, params Node[] scopeElements)
        {
            switch (Name)
            {
                case "nth-child":
                    {// SEE: https://www.w3.org/TR/2011/REC-css3-selectors-20110929/#nth-child-pseudo
                        // TODO: IMPLEMENT THIS!

                        return false;
                    }
                default:
                    throw new CssSelectorException("Selector pseudo-class function (", Name, ") logic not implemented!");
            }
        }
    }

    public class PseudoClassSelectorNegationFunction : PseudoClassSelector
    {
        protected readonly ComplexSelector Selector;

        public PseudoClassSelectorNegationFunction(string Name, CssTokenStream Stream) : base(Name)
        {
            Selector = SelectorParser.Consume_Single_Selector(Stream);
        }

        /// <summary>
        /// Returns whether the selector matches a specified element or index
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        override public bool Matches(Element E, params Node[] scopeElements)
        {
            return false == Selector.Match(E, scopeElements);
        }
    }

    static class PseudoClassFunctions
    {
        /// <summary>
        /// Performs matching against the 'drop' function
        /// </summary>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool Drop(Element E, List<CssToken> Args)
        {
            if (Args == null || Args.Count <= 0)
            {// :drop
                return E.AcceptsDragDrop;
            }
            else
            {// drop(...)
                if (!E.AcceptsDragDrop) return false;

                // TODO: test the ":drop(active|valid|invalid) pseudo-class
                foreach (CssToken token in Args)
                {
                    if (token.Type != ECssTokenType.Ident) throw new CssSyntaxErrorException("Expected Ident token!");
                    IdentToken tok = Args[0] as IdentToken;
                    switch (tok.Value.ToLower())
                    {
                        case "active":
                            if (!E.IsDropTarget) return false;
                            break;
                        case "valid":// Matches if the drop-target CAN accept the type of object being dragged 
                            if (!E.Accepts_Current_DragItem()) return false;
                            break;
                        case "invalid":// Matches if the drop-target CAN'T accept the type of object being dragged 
                            if (E.Accepts_Current_DragItem()) return false;
                            break;
                    }
                }

                return true;
            }
        }
    }
}
