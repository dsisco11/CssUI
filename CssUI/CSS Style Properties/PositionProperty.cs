using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CssUI.CSS;

namespace CssUI
{
    /// <summary>
    /// Specifies the position of a object area (e.g. background image) inside a positioning area (e.g. background positioning area).
    /// SEE:  https://www.w3.org/TR/2012/WD-css3-values-20120308/#position
    /// </summary>
    public class PositionProperty : StyleProperty
    {
        #region Propertys
        CssValue Computed_X = CssValue.Null;// Stores our currently computed X coordinate value
        CssValue Computed_Y = CssValue.Null;// Stores our currently computed Y coordinate value
        #endregion

        #region Accessors
        /// <summary>
        /// Returns TRUE if any values have the <see cref="StyleValueFlags.Depends"/> flag
        /// </summary>
        public override bool IsDependent { get { return (Computed_X.Has_Flags(StyleValueFlags.Depends) || Computed_Y.Has_Flags(StyleValueFlags.Depends)); } }
        /// <summary>
        /// Return TRUE if any values are set to <see cref="CssValue.Auto"/>
        /// </summary>
        public override bool IsAuto { get { return (Computed_X.Type == EStyleDataType.AUTO || Computed_Y.Type == EStyleDataType.AUTO); } }
        /// <summary>
        /// Return TRUE if any values are set to <see cref="CssValue.Auto"/>
        /// Returns TRUE if any values have the <see cref="StyleValueFlags.Depends"/> flag
        /// </summary>
        public override bool IsDependentOrAuto { get { return (Computed_X.Type == EStyleDataType.AUTO || Computed_X.Has_Flags(StyleValueFlags.Depends) || Computed_Y.Type == EStyleDataType.AUTO || Computed_Y.Has_Flags(StyleValueFlags.Depends)); } }

        /// <summary>
        /// The currently resolved X value
        /// </summary>
        public int X { get; private set; }
        /// <summary>
        /// The currently resolved Y value
        /// </summary>
        public int Y { get; private set; }
        #endregion

        #region Constructors
        public PositionProperty() : base(CssValue.Null, null)
        {
            base.onChanged += this.Update;
        }

        public PositionProperty(string CssName, cssElement Owner, bool Locked, bool Unset, PropertyOptions Options) : base(CssName, Locked, Unset, Owner, Options)
        {
            base.onChanged += this.Update;
        }
        #endregion

        /// <summary>
        /// Whenever this propertys value changes this is called so we can update our <see cref="Computed_X"/> and <see cref="Computed_Y"/> values
        /// </summary>
        void Update(IStyleProperty Sender)
        {
        }

        #region Translations
        /// <summary>
        /// Translates the given X and Y style values to a valid string representation for this property type
        /// </summary>
        string Translate_From_XY(CssValue X, CssValue Y)
        {
            string strX = "center";
            string strY = "center";
            if (X != CssValue.Null)
            {
            }

            return string.Empty;
        }

        /// <summary>
        /// Translates the given string to X and Y style values
        /// </summary>
        void Translate_From_String(string str, out CssValue X, out CssValue Y)
        {
            CssTokenizer Parser = new CssTokenizer(Specified.AsString());

            X = null;
            Y = null;
        }
        #endregion


        #region Setters
        /// <summary>
        /// Set the explicit values
        /// </summary>
        public void Set(string str)
        {
            base.Assigned = CssValue.From_String(str);
        }
        /// <summary>
        /// Set the explicit values
        /// </summary>
        public void Set(CssValue x, CssValue y)
        {
            base.Assigned = CssValue.From_String(Translate_From_XY(x, y));
        }

        /// <summary>
        /// Set the explicit values
        /// </summary>
        public void Set(int? x, int? y)
        {
            var X = CssValue.From_Int(x, CssValue.Null);
            var Y = CssValue.From_Int(y, CssValue.Null);
            base.Assigned = CssValue.From_String(Translate_From_XY(X, Y));
        }

        /// <summary>
        /// Set the explicit values
        /// </summary>
        /// <param name="x">Percentage in the range [0 - 100]</param>
        /// <param name="y">Percentage in the range [0 - 100]</param>
        public void Set(float? x, float? y)
        {
            var X = CssValue.From_Int((int?)x, CssValue.Null);
            var Y = CssValue.From_Int((int?)y, CssValue.Null);
            base.Assigned = CssValue.From_String(Translate_From_XY(X, Y));
        }
        #endregion

        #region Operators
        public static bool operator ==(PositionProperty A, PositionProperty B)
        {
            if (object.ReferenceEquals(A, null) || object.ReferenceEquals(B, null)) return (object.ReferenceEquals(A, null) && object.ReferenceEquals(B, null));
            return (A.X == B.X && A.Y == B.Y);
        }

        public static bool operator !=(PositionProperty A, PositionProperty B)
        {
            if (object.ReferenceEquals(A, null) || object.ReferenceEquals(B, null)) return !(object.ReferenceEquals(A, null) && object.ReferenceEquals(B, null));
            return (A.X != B.X || A.Y != B.Y);
        }

        public override string ToString() { return string.Concat("[", nameof(PositionProperty), "]<", Computed_X, ", ", Computed_Y, ">"); }


        public override bool Equals(object o)
        {

            if (o is PositionProperty)
            {
                return this == (PositionProperty)o;
            }

            return false;
        }
        public override int GetHashCode()
        {
            return (int)Computed_X.Value ^ (int)Computed_Y.Value;
        }

        #endregion

        #region Resolvers
        /*
        /// <summary>
        /// Resolves the values to absolute integers
        /// </summary>
        /// <param name="Container">Size of the containing area</param>
        /// <param name="Size">Size of the object being positioned</param>
        public void Resolve_As_Position(ref ePos Pos, eSize Container, eSize Size)
        {// SEE: https://www.w3.org/TR/2011/CR-css3-background-20110215/#background-position

            int? x = Computed_X.AsInt(delegate (float pct)
            {
                return (((float)Container.Width * pct) - ((float)Size.Width * pct));
            });

            int? y = Computed_Y.AsInt(delegate (float pct)
            {
                return (((float)Container.Height * pct) - ((float)Size.Height * pct));
            });

            Pos.X = (x.HasValue ? x.Value : 0);
            Pos.Y = (y.HasValue ? y.Value : 0);
        }
        */
        #endregion

    }
}
