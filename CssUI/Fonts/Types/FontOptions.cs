using SixLabors.Fonts;
using System.Collections.Generic;

namespace CssUI.Fonts
{
    /// <summary>
    /// Specifies all options for a font
    /// </summary>
    public class FontOptions
    {
        public readonly ICollection<string> Families = null;
        public readonly double Size = 0;
        public readonly double Weight = 400;
        public readonly EFontStyle Style = EFontStyle.Normal;

        #region Constructors
        public FontOptions(string Family, double Size, double Weight = 400, EFontStyle Style = EFontStyle.Normal)
        {
            // Per the specifications we should not use any fonts below 9px in size
            if (Size < 9.0)
                Size = 9.0;

            this.Families = new string[1]{ Family };
            this.Size = Size;
            this.Weight = Weight;
            this.Style = Style;
        }

        public FontOptions(ICollection<string> Families, double Size, double Weight = 400, EFontStyle Style = EFontStyle.Normal)
        {
            // Per the specifications we should not use any fonts below 9px in size
            if (Size < 9.0)
                Size = 9.0;

            this.Families = Families;
            this.Size = Size;
            this.Weight = Weight;
            this.Style = Style;
        }

        #endregion



        #region Overrides
        public override int GetHashCode()
        {
            int hashCode = Families.GetHashCode();
            hashCode = hashCode * -152113495 + Size.GetHashCode();
            hashCode = hashCode * -152113495 + Weight.GetHashCode();
            hashCode = hashCode * -152113495 + Style.GetHashCode();

            return hashCode;
        }
        #endregion
    }
}
