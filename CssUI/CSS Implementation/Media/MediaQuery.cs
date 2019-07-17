using CssUI.DOM;
using System.Collections.Generic;

namespace CssUI.CSS.Media
{
    public class MediaQuery
    {/* https://drafts.csswg.org/mediaqueries-4/#media */
        #region Properties
        public EMediaQueryModifier Modifier { get; private set; }
        public EMediaType MediaType { get; private set; }
        public LinkedList<IMediaCondition> Conditions { get; private set; }
        #endregion

        #region Constructors
        public MediaQuery(EMediaQueryModifier modifier, EMediaType mediaType, LinkedList<IMediaCondition> conditions)
        {
            Modifier = modifier;
            MediaType = mediaType;
            Conditions = conditions;
        }
        #endregion

        /// <summary>
        /// Returns <c>true</c> if this query matches the given <see cref="Document"/>
        /// </summary>
        /// <param name="document">The document to test for a match against</param>
        public bool Matches(Document document)
        {
            if (MediaType != EMediaType.All && MediaType != document.window.screen.MediaType)
                return false;

            foreach (IMediaCondition condition in Conditions)
            {
                if (!condition.Matches(document))
                {
                    return false;
                }
            }

            return true;
        }
    }
}
