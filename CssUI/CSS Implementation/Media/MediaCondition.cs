using System.Collections.Generic;
using CssUI.DOM;

namespace CssUI.CSS.Media
{
    public class MediaCondition : IMediaCondition
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

    }
}
