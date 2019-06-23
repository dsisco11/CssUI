
namespace CssUI
{
    /// <summary>
    /// Holds Nullable Sizing dimensions for a <see cref="cssElement"/>
    /// </summary>
    public class eSize_Nullable
    {
        public static eSize_Nullable Zero = new eSize_Nullable() { Width = 0, Height = 0 };
        public int? Width = null;
        public int? Height = null;

        #region Constructors
        public eSize_Nullable() { }
        public eSize_Nullable(int? Width, int? Height)
        {
            this.Width = Width;
            this.Height = Height;
        }
        public eSize_Nullable(System.Drawing.Size sz)
        {
            Width = sz.Width;
            Height = sz.Height;
        }
        public eSize_Nullable(eSize_Nullable sz)
        {
            Width = sz.Width;
            Height = sz.Height;
        }
        #endregion

        /// <summary>
        /// Converts a <see cref="eSize_Nullable"/> instance to an <see cref="eSize"/> instance, replacing null values with the default one specified
        /// </summary>
        public static eSize ToSize(eSize_Nullable sz, int default_value)
        {
            int W = (sz.Width.HasValue ? sz.Width.Value : default_value);
            int H = (sz.Height.HasValue ? sz.Height.Value : default_value);
            return new eSize(W, H);
        }

        #region Operators
        public static bool operator ==(eSize_Nullable x, eSize_Nullable y)
        {
            if (object.ReferenceEquals(x, null) || object.ReferenceEquals(y, null)) return (object.ReferenceEquals(x, null) && object.ReferenceEquals(y, null));
            return (x.Width == y.Width && x.Height == y.Height);
        }
        public static bool operator !=(eSize_Nullable x, eSize_Nullable y) { return !(x == y); }

        public override bool Equals(object o)
        {

            if (o is eSize_Nullable)
            {
                return this == (eSize_Nullable)o;
            }

            return false;
        }

        public override int GetHashCode()
        {
            return (Width ?? 0) ^ (Height ?? 0);
        }

        public override string ToString()
        {
            return string.Concat("[", nameof(eSize_Nullable), "]<", Width, ", ", Height, ">");
        }
        #endregion
    }

}
