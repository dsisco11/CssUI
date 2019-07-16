using System.Collections.Generic;

namespace CssUI.CSS.Media
{
    public class MediaQuery
    {/* https://drafts.csswg.org/mediaqueries-4/#media */
        #region Properties
        public EMediaQueryModifier Modifier { get; private set; }
        public EMediaType MediaType { get; private set; }
        public LinkedList<MediaFeature> Features { get; private set; }
        #endregion

        #region Constructors
        public MediaQuery(EMediaQueryModifier modifier, EMediaType mediaType, LinkedList<MediaFeature> features)
        {
            Modifier = modifier;
            MediaType = mediaType;
            Features = features;
        }
        #endregion
    }
}
