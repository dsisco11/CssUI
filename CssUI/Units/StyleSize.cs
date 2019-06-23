using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CssUI.CSS
{
    public class StyleSize
    {
        public CSSValue Width = CSSValue.Null;
        public CSSValue Height = CSSValue.Null;

        #region Constructors
        public StyleSize()
        {
        }
        public StyleSize(StyleSize size)
        {
            Width = size.Width;
            Height = size.Height;
        }

        public StyleSize(CSSValue Width, CSSValue Height)
        {
            this.Width = Width;
            this.Height = Height;
        }
        
        #endregion

        #region Operators
        public static bool operator ==(StyleSize A, StyleSize B)
        {
            if (object.ReferenceEquals(A, null) || object.ReferenceEquals(B, null)) return (object.ReferenceEquals(A, null) && object.ReferenceEquals(B, null));
            return (A.Width == B.Width && A.Height == B.Height);
        }
        public static bool operator !=(StyleSize A, StyleSize B)
        {
            if (object.ReferenceEquals(A, null) || object.ReferenceEquals(B, null)) return (!object.ReferenceEquals(A, null) || !object.ReferenceEquals(B, null));
            return (A.Width != B.Width || A.Height != B.Height);
        }
        public override string ToString() { return ("[" + nameof(StyleSize) + "]<" + Width + ", " + Height + ">"); }

        public override bool Equals(object o)
        {

            if (o is StyleSize)
            {
                return this == (StyleSize)o;
            }

            return false;
        }
        public override int GetHashCode()
        {
            return (int)Width.Value ^ (int)Height.Value;
        }

        #endregion

    }
}
