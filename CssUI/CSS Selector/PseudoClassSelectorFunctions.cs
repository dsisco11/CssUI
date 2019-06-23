using CssUI.CSS.Selector;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CssUI.CSS
{
    
    public class CssPseudoClassSelectorFunction : CssPseudoClassSelector
    {
        protected readonly List<CssToken> Args;

        public CssPseudoClassSelectorFunction(string Name, List<CssToken> Args = null) : base(Name)
        {
            this.Args = Args;
        }

        /// <summary>
        /// Returns whether the selector matches a specified element or index
        /// </summary>
        override public bool Matches(cssElement E)
        {
            switch (Name)
            {
                case "drop":
                    return PseudoClassFunctions.Drop(E, Args);
                default:
                    throw new CssSelectorError("[CSS] Selector pseudo-class (", Name, ") logic not implemented!");
            }
        }
    }

    public class CssPseudoClassSelectorAnBFunction : CssPseudoClassSelector
    {
        protected readonly CssAnBMatcher AnB;
        protected readonly CssSelectorList Selector;

        public CssPseudoClassSelectorAnBFunction(string Name, CssTokenStream Stream) : base(Name)
        {
            AnB = CssAnBMatcher.Consume(Stream);
            if (Stream.Next.Type == ECssTokenType.Ident && string.Compare("or", (Stream.Next as IdentToken).Value)==0)
            {
                Stream.Consume();// Consume the 'or' string token
                Selector = CssSelectorParser.Consume_Selector_List(Stream);
            }
        }

        /// <summary>
        /// Returns whether the selector matches a specified element or index
        /// </summary>
        override public bool Matches(cssElement E)
        {
            switch (Name)
            {
                case "nth-child":
                    {// SEE: https://www.w3.org/TR/2011/REC-css3-selectors-20110929/#nth-child-pseudo
                        // TODO: IMPLEMENT THIS!

                        return false;
                    }
                default:
                    throw new CssSelectorError("Selector pseudo-class function (", Name, ") logic not implemented!");
            }
        }
    }

    public class CssPseudoClassSelectorNegationFunction : CssPseudoClassSelector
    {
        protected readonly CssSelectorList Selector;

        public CssPseudoClassSelectorNegationFunction(string Name, CssTokenStream Stream) : base(Name)
        {
            Selector = CssSelectorParser.Consume_Selector_List(Stream);
        }

        /// <summary>
        /// Returns whether the selector matches a specified element or index
        /// </summary>
        override public bool Matches(cssElement E)
        {
            return false == Selector.Query(E);
        }
    }

    static class PseudoClassFunctions
    {
        /// <summary>
        /// Performs matching against the 'drop' function
        /// </summary>
        /// <returns></returns>
        public static bool Drop(cssElement E, List<CssToken> Args)
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
                    if (token.Type != ECssTokenType.Ident) throw new CssSyntaxError("Expected Ident token!");
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
