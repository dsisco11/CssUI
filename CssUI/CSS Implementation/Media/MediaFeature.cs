namespace CssUI.CSS.Media
{
    /// <summary>
    /// </summary>
    public class MediaFeature
    {/* Docs: https://drafts.csswg.org/mediaqueries-4/#media-feature */
        #region Properties
        public readonly EMediaFeature Name = 0x0;
        public readonly CssValue Value = null;
        public readonly EMediaFeatureType Type = EMediaFeatureType.Plain;
        public readonly EMediaFeatureComparator? Comparator = null;
        #endregion

        #region Constructors
        public MediaFeature(EMediaFeature name, CssValue value)
        {
            Name = name;
            Value = value;
        }
        
        public MediaFeature(EMediaFeature name, EMediaFeatureComparator comparator, CssValue value)
        {
            Name = name;
            Value = value;
            Comparator = comparator;
        }
        #endregion

    }
}
