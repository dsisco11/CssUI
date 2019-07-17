using CssUI.DOM;
using System;

namespace CssUI.CSS.Media
{
    /// <summary>
    /// </summary>
    public class MediaFeature
    {/* Docs: https://www.w3.org/TR/mediaqueries-4/#mq-features */
        #region Properties
        public readonly EMediaFeatureName Name = 0x0;
        public readonly EMediaFeatureType Type = EMediaFeatureType.Plain;
        public readonly EMediaFeatureComparator Comparator = EMediaFeatureComparator.EqualTo;
        private readonly CssValue Value = null;
        #endregion

        #region Constructors
        public MediaFeature(EMediaFeatureName name, CssValue value)
        {
            Name = name;
            Value = value;
        }
        
        public MediaFeature(EMediaFeatureName name, EMediaFeatureComparator comparator, CssValue value)
        {
            Name = name;
            Value = value;
            Comparator = comparator;
        }
        #endregion

        private bool Compare(double other)
        {
            if (0 != (Comparator & EMediaFeatureComparator.EqualTo))
            {
                double value = (double)Value.Value;
                if (MathExt.floatEq(value, other))
                    return true;
            }

            if (0 != (Comparator & EMediaFeatureComparator.LessThan))
            {
                double value = (double)Value.Value;
                if (value < other)
                    return true;
            }

            if (0 != (Comparator & EMediaFeatureComparator.GreaterThan))
            {
                double value = (double)Value.Value;
                if (value > other)
                    return true;
            }

            return false;
        }

        /// <summary>
        /// Returns <c>true</c> if this query matches the given <see cref="Document"/>
        /// </summary>
        /// <param name="document">The document to test for a match against</param>
        public bool Matches(Document document)
        {
            // XXX: Need a definitions class for media list properties because they arent like normal css properties yet we still NEED to restrict their values and whatnot
            Screen screen = document.window.screen;
            switch (Name)
            {
                case EMediaFeatureName.AspectRatio:
                    {
                        double docAspect = (double)screen.width / (double)screen.height;
                        return Compare(docAspect);
                    }
                case EMediaFeatureName.Width:
                case EMediaFeatureName.Min_Width:
                    {
                        return Compare((double)screen.width);
                    }
                case EMediaFeatureName.Height:
                    {
                        return Compare((double)screen.height);
                    }
                case EMediaFeatureName.Orientation:
                    {
                        //return Compare((double)screen.height);
                    }

                default:
                    {
                        throw new NotImplementedException($"Media feature \"{Enum.GetName(typeof(EMediaFeatureName), Name)}\" is not implemented");
                    }
            }
        }

    }
}
