using CssUI.CSS.Serialization;
using CssUI.DOM;
using System.Collections.Generic;
using System.Text;

namespace CssUI.CSS.Media
{
    public class MediaQuery : ICssSerializeable
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
            if (MediaType != EMediaType.All && MediaType != document.defaultView.screen.MediaType)
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

        public string Serialize()
        {
            StringBuilder sb = new StringBuilder();

            if (Modifier == EMediaQueryModifier.Not)
            {
                sb.Append("not");
                sb.Append(UnicodeCommon.CHAR_SPACE);
            }


            string type = Serializer.Identifier<EMediaType>(MediaType);
            /* 3) If the media query does not contain media features append type, to s, then return s. */
            if (Conditions.Count <= 0)
            {
                sb.Append(type);
                return sb.ToString();
            }

            /* 4) If type is not "all" or if the media query is negated append type, followed by a single SPACE (U+0020), followed by "and", followed by a single SPACE (U+0020), to s. */
            if (MediaType != EMediaType.All || Modifier == EMediaQueryModifier.Not)
            {
                sb.Append(type);
                sb.Append(UnicodeCommon.CHAR_SPACE);
                sb.Append("and");
                sb.Append(UnicodeCommon.CHAR_SPACE);
            }

            /* No clue why we would need to do this but its not possible with our system (atleast not easily) */
            /* 5) Sort the media features in lexicographical order. */
            //List<MediaFeature> features = query.Conditions.Where(cond => cond is MediaFeature feature && feature.IsValid).OrderBy(feature => CssLookup.Keyword_From_Enum(feature.Name)).ToList();

            bool first = true;
            foreach(IMediaCondition Condition in Conditions)
            {
                /* 4) If this is not the last media feature append a single SPACE (U+0020), followed by "and", followed by a single SPACE (U+0020), to s. */
                if (!first)
                {
                    sb.Append(UnicodeCommon.CHAR_SPACE);
                    sb.Append("and");
                    sb.Append(UnicodeCommon.CHAR_SPACE);
                }

                sb.Append(Condition.Serialize());
                first = false;
            }

            return sb.ToString();
        }
    }
}
