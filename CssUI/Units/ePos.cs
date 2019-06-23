
namespace CssUI
{
    /// <summary>
    /// Holds the Positioning coordinates for a <see cref="cssElement"/>
    /// </summary>
    public class ePos
    {
        #region Values
        public static ePos Zero = new ePos() { X = 0, Y = 0 };
        public int X = 0;
        public int Y = 0;
        #endregion

        #region Constructors
        public ePos() { }
        public ePos(ePos pos)
        {
            this.X = pos.X;
            this.Y = pos.Y;
        }
        public ePos(System.Drawing.Point pos)
        {
            this.X = pos.X;
            this.Y = pos.Y;
        }

        public ePos(int X, int Y)
        {
            this.X = X;
            this.Y = Y;
        }
        #endregion

        #region Operators
        public static bool operator ==(ePos a, ePos b)
        {
            if (object.ReferenceEquals(a, null) || object.ReferenceEquals(b, null))
                return object.ReferenceEquals(a, b);

            return (a.X == b.X && a.Y == b.Y);
        }
        public static bool operator !=(ePos a, ePos b)
        {
            if (object.ReferenceEquals(a, null) || object.ReferenceEquals(b, null))
                return !object.ReferenceEquals(a, b);

            return (a.X != b.X || a.Y != b.Y);
        }

        public override bool Equals(object o)
        {
            if (o is ePos)
            {
                return this == (ePos)o;
            }
            return false;
        }

        public override int GetHashCode()
        {
            return X ^ Y;
        }

        public static ePos operator +(ePos A, ePos B)
        {
            return new ePos((A.X + B.X), (A.Y + B.Y));
        }

        public static ePos operator -(ePos A, ePos B)
        {
            return new ePos((A.X - B.X), (A.Y - B.Y));
        }

        public override string ToString()
        {
            return string.Concat("[", nameof(ePos), "]<", X, ", ", Y, ">");
        }
        #endregion
    }
}
