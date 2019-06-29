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
        public readonly int Weight = 400;
        public readonly EFontStyle Style = EFontStyle.Normal;

        #region Constructors
        public FontOptions(string Family, double Size, int Weight = 400, EFontStyle Style = EFontStyle.Normal)
        {
            this.Families = new string[1]{ Family };
            this.Size = Size;
            this.Weight = Weight;
            this.Style = Style;
        }

        public FontOptions(ICollection<string> Families, double Size, int Weight = 400, EFontStyle Style = EFontStyle.Normal)
        {
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
