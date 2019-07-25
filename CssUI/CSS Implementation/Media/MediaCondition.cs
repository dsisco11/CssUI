using System.Collections.Generic;
using System.Text;
using CssUI.CSS.Serialization;
using CssUI.DOM;

namespace CssUI.CSS.Media
{
    public class MediaCondition : IMediaCondition, ICssSerializeable
    {/* https://www.w3.org/TR/mediaqueries-4/#media-condition */

        #region Properites
        private readonly LinkedList<IMediaCondition> Conditions;
        private readonly EMediaCombinator Op;
        #endregion

        #region Constructor
        public MediaCondition(EMediaCombinator op, IEnumerable<IMediaCondition> conditions)
        {
            Conditions = new LinkedList<IMediaCondition>(conditions);
            Op = op;
        }
        #endregion


        public bool Matches(Document document)
        {
            bool matches = true;
            if (Op == EMediaCombinator.OR) matches = false;

            foreach (IMediaCondition condition in Conditions)
            {
                if (condition.Matches(document))
                {
                    if (Op == EMediaCombinator.NOT)
                    {
                        return false;
                    }
                    else if (Op == EMediaCombinator.OR)
                    {
                        matches = true;
                        return true;
                    }
                }
                else
                {
                    if (Op == EMediaCombinator.AND)
                    {
                        matches = false;
                    }
                }
            }

            return matches;
        }



        public string Serialize()
        {
            if (Conditions.Count <= 0)
            {
                return string.Empty;
            }

            StringBuilder sb = new StringBuilder();

            sb.Append(UnicodeCommon.CHAR_PARENTHESES_OPEN);

            bool first = true;
            foreach (IMediaCondition Condition in Conditions)
            {
                if (!first)
                {
                    sb.Append(UnicodeCommon.CHAR_SPACE);
                    sb.Append(CssLookup.Keyword(Op));
                    sb.Append(UnicodeCommon.CHAR_SPACE);
                }

                sb.Append(Condition.Serialize());
                first = false;
            }

            sb.Append(UnicodeCommon.CHAR_PARENTHESES_CLOSE);
            return sb.ToString();
        }


    }
}
