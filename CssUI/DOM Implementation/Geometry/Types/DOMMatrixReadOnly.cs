using CssUI.DOM.Exceptions;

namespace CssUI.DOM.Geometry
{
    /// <summary>
    /// Represents a mathematical matrix.
    /// </summary>
    public class DOMMatrixReadOnly
    {
        #region Backing Values
        protected double[] Data = new double[16];
        #endregion

        #region Indexing constants
        internal const int _m11 = 0;
        internal const int _m12 = 4;
        internal const int _m13 = 8;
        internal const int _m14 = 12;
        
        internal const int _m21 = 1;
        internal const int _m22 = 5;
        internal const int _m23 = 9;
        internal const int _m24 = 13;
        
        internal const int _m31 = 2;
        internal const int _m32 = 6;
        internal const int _m33 = 10;
        internal const int _m34 = 14;
        
        internal const int _m41 = 3;
        internal const int _m42 = 7;
        internal const int _m43 = 11;
        internal const int _m44 = 15;
        #endregion

        #region Accessors
        public double m11 { get => Data[_m11]; protected set => Data[_m11] = value; }
        public double m12 { get => Data[_m12]; protected set => Data[_m12] = value; }
        public double m13
        {
            get => Data[_m13];
            protected set
            {
                Data[_m13] = value;
                if (!MathExt.floatEq(value, 0.0))
                    is2D = false;
            }
        }
        public double m14
        {
            get => Data[3];
            protected set
            {
                Data[3] = value;
                if (!MathExt.floatEq(value, 0.0))
                    is2D = false;
            }
        }
        public double m21 { get => Data[_m21]; protected set => Data[_m21] = value; }
        public double m22 { get => Data[_m22]; protected set => Data[_m22] = value; }
        public double m23
        {
            get => Data[_m23];
            protected set
            {
                Data[_m23] = value;
                if (!MathExt.floatEq(value, 0.0))
                    is2D = false;
            }
        }
        public double m24
        {
            get => Data[_m24];
            protected set
            {
                Data[_m24] = value;
                if (!MathExt.floatEq(value, 0.0))
                    is2D = false;
            }
        }
        public double m31
        {
            get => Data[_m31];
            protected set
            {
                Data[_m31] = value;
                if (!MathExt.floatEq(value, 1.0))
                    is2D = false;
            }
        }
        public double m32
        {
            get => Data[_m32];
            protected set
            {
                Data[_m32] = value;
                if (!MathExt.floatEq(value, 0.0))
                    is2D = false;
            }
        }
        public double m33
        {
            get => Data[_m33];
            protected set
            {
                Data[_m33] = value;
                if (!MathExt.floatEq(value, 1.0))
                    is2D = false;
            }
        }
        public double m34
        {
            get => Data[_m34];
            protected set
            {
                Data[_m34] = value;
                if (!MathExt.floatEq(value, 0.0))
                    is2D = false;
            }
        }
        public double m41 { get => Data[_m41]; protected set => Data[_m41] = value; }
        public double m42 { get => Data[_m42]; protected set => Data[_m42] = value; }
        public double m43
        {
            get => Data[_m43];
            protected set
            {
                Data[_m43] = value;
                if (!MathExt.floatEq(value, 0.0))
                    is2D = false;
            }
        }
        public double m44
        {
            get => Data[_m44];
            protected set
            {
                Data[_m44] = value;
                if (!MathExt.floatEq(value, 1.0))
                    is2D = false;
            }
        }
        #endregion

        #region Accessors
        // These attributes are simple aliases for certain elements of the 4x4 matrix
        public double a { get => m11; set => m11 = value; }
        public double b { get => m12; set => m12 = value; }
        public double c { get => m21; set => m21 = value; }
        public double d { get => m22; set => m22 = value; }
        public double e { get => m41; set => m41 = value; }
        public double f { get => m42; set => m42 = value; }

        public bool is2D { get; protected set; }
        public bool isIdentity
        {/* Docs: https://www.w3.org/TR/geometry-1/#dom-dommatrixreadonly-isidentity */
            get
            {
                if (!MathExt.floatEq(1.0, m11)) return false;
                if (!MathExt.floatEq(1.0, m22)) return false;
                if (!MathExt.floatEq(1.0, m33)) return false;
                if (!MathExt.floatEq(1.0, m44)) return false;

                if (!MathExt.floatEq(0.0, m12)) return false;
                if (!MathExt.floatEq(0.0, m13)) return false;
                if (!MathExt.floatEq(0.0, m14)) return false;
                if (!MathExt.floatEq(0.0, m21)) return false;
                if (!MathExt.floatEq(0.0, m23)) return false;
                if (!MathExt.floatEq(0.0, m24)) return false;
                if (!MathExt.floatEq(0.0, m31)) return false;
                if (!MathExt.floatEq(0.0, m32)) return false;
                if (!MathExt.floatEq(0.0, m34)) return false;
                if (!MathExt.floatEq(0.0, m41)) return false;
                if (!MathExt.floatEq(0.0, m42)) return false;
                if (!MathExt.floatEq(0.0, m43)) return false;

                return true;
            }
        }
        #endregion

        #region Constructors
        public DOMMatrixReadOnly(params double[] init)
        {
            if (init.Length == 6)
            {
                this.a = init[0];
                this.b = init[1];
                this.c = init[2];
                this.d = init[3];
                this.e = init[4];
                this.f = init[5];
                return;
            }
            else if (init.Length == 16)
            {
                this.m11 = init[0];
                this.m12 = init[1];
                this.m13 = init[2];
                this.m14 = init[3];

                this.m21 = init[4];
                this.m22 = init[5];
                this.m23 = init[6];
                this.m24 = init[7];

                this.m31 = init[8];
                this.m32 = init[9];
                this.m33 = init[10];
                this.m34 = init[11];

                this.m41 = init[12];
                this.m42 = init[13];
                this.m43 = init[14];
                this.m44 = init[15];
                return;
            }

            throw new TypeError("Matrix initialization did not receive a valid amount of numbers");
        }

        public DOMMatrixReadOnly(DOMMatrixInit init)
        {
            this.m11 = init.m11;
            this.m12 = init.m12;
            this.m21 = init.m21;
            this.m22 = init.m22;
            this.m41 = init.m41;
            this.m42 = init.m42;

            this.m13  = init.m13;
            this.m14  = init.m14;
            this.m23  = init.m23;
            this.m24  = init.m24;
            this.m31  = init.m31;
            this.m32  = init.m32;
            this.m33  = init.m33;
            this.m34  = init.m34;
            this.m43  = init.m43;
            this.m44  = init.m44;
        }

        public DOMMatrixReadOnly(DOMMatrix2DInit init)
        {
            this.m11 = init.m11;
            this.m12 = init.m12;
            this.m21 = init.m21;
            this.m22 = init.m22;
            this.m41 = init.m41;
            this.m42 = init.m42;
        }
        #endregion

        #region Instantiation
        public static DOMMatrixReadOnly fromMatrix(DOMMatrixInit other)
        {
            return new DOMMatrixReadOnly(other);
        }

        public static DOMMatrixReadOnly fromFloat32Array(params float[] array32)
        {
            double[] array = new double[array32.Length];
            for (int i = 0; i < array32.Length; i++) { array[i] = array32[i]; }
            return new DOMMatrixReadOnly(array);
        }

        public static DOMMatrixReadOnly fromFloat64Array(params double[] array64)
        {
            return new DOMMatrixReadOnly(array64);
        }


        /// <summary>
        /// Returns a copy of the matrix.
        /// </summary>
        /// <returns></returns>
        public DOMMatrix Clone()
        {
            return new DOMMatrix(m11, m12, m13, m14,
                                 m21, m22, m23, m24,
                                 m31, m32, m33, m34,
                                 m41, m42, m43, m44);
        }
        #endregion



        #region Immutable Transformations
        /// <summary>
        /// Applies a translation to a copy of this matrix and returns it.
        /// </summary>
        /// <param name="tx"></param>
        /// <param name="ty"></param>
        /// <param name="tz"></para
        public DOMMatrix Translate(double tx = 0, double ty = 0, double tz = 0.0)
        {
            return this.Clone().translateSelf(tx, ty, tz);
        }

        /// <summary>
        /// Scales a copy of this matrix and returns it.
        /// </summary>
        /// <param name="scaleX">X-Axis scaling factor</param>
        /// <param name="scaleY">Y-Axis scaling factor</param>
        /// <param name="scaleZ">Z-Axis scaling factor</param>
        /// <param name="originX">X-Axis position to scale around</param>
        /// <param name="originY">Y-Axis position to scale around</param>
        /// <param name="originZ">Z-Axis position to scale around</param>
        /// <returns>Copy of this matrix with scaling applied to it</returns>
        public DOMMatrix Scale(double scaleX = 1, double scaleY = 1, double scaleZ = 1, double originX = 0, double originY = 0, double originZ = 0)
        {
            return this.Clone().scaleSelf(scaleX, scaleY, scaleZ, originX, originY, originZ);
        }

        /// <summary>
        /// Scales a copy of this matrix (non-uniformly) and returns it.
        /// <para>Supported for legacy SVG reasons, use <see cref="Scale(double, double, double, double, double, double)" instead./></para>
        /// </summary>
        /// <param name="scaleX">X-Axis scaling factor</param>
        /// <param name="scaleY">Y-Axis scaling factor</param>
        /// <returns>Copy of this matrix with scaling applied to it</returns>
        public DOMMatrix ScaleNonUniform(double scaleX = 1, double scaleY = 1)
        {
            return this.Clone().scaleSelf(scaleX, scaleY, 1, 0, 0, 0);
        }

        /// <summary>
        /// Scales a copy of this matrix and returns it.
        /// </summary>
        /// <param name="scaleX">Scaling factor for ALL axi</param>
        /// <param name="originX">X-Axis position to scale around</param>
        /// <param name="originY">Y-Axis position to scale around</param>
        /// <param name="originZ">Z-Axis position to scale around</param>
        /// <returns>Copy of this matrix with scaling applied to it</returns>
        public DOMMatrix Scale3D(double scale = 1, double originX = 0, double originY = 0, double originZ = 0)
        {
            return this.Clone().scale3DSelf(scale, originX, originY, originZ);
        }

        /// <summary>
        /// Rotates(ZYX) a copy of this matrix and returns it
        /// </summary>
        /// <param name="rotX">X-Axis rotation (in degrees)</param>
        /// <param name="rotY">Y-Axis rotation (in degrees)</param>
        /// <param name="rotZ">Z-Axis rotation (in degrees)</param>
        /// <returns>A rotated copy of this matrix</returns>
        public DOMMatrix Rotate(double rotX = 0, double rotY = 0, double rotZ = 0)
        {
            return this.Clone().rotateSelf(rotX, rotY, rotZ);
        }

        /// <summary>
        /// Rotates(Z-Axis) a copy of this matrix to face the given vector and returns it
        /// </summary>
        /// <param name="x">X-Axis position</param>
        /// <param name="y">Y-Axis position</param>
        /// <returns>A rotated copy of this matrix</returns>
        public DOMMatrix RotateFromVector(double x = 0, double y = 0)
        {
            return this.Clone().rotateFromVectorSelf(x, y);
        }

        /// <summary>
        /// Rotates a copy of this matrix around the given axis by <paramref name="alpha"/> degrees.
        /// </summary>
        /// <param name="axisX">Axis vector X factor</param>
        /// <param name="axisY">Axis vector Y factor</param>
        /// <param name="axisZ">Axis vector Z factor</param>
        /// <param name="alpha">Degrees of rotation</param>
        /// <returns>Rotated copy of this matrix</returns>
        public DOMMatrix RotateAxisAngle(double axisX = 0, double axisY = 0, double axisZ = 0, double alpha = 0)
        {
            return this.Clone().rotateAxisAngleSelf(axisX, axisY, axisZ, alpha);
        }

        /// <summary>
        /// Skews a copy of this matrix on the X axis and returns it
        /// </summary>
        /// <param name="sx">Angle(in degrees) to skew on the x-axis</param>
        /// <returns>Skewed copy of this matrix</returns>
        public DOMMatrix SkewX(double sx)
        {
            return this.Clone().skewXSelf(sx);
        }

        /// <summary>
        /// Skews a copy of this matrix on the Y axis and returns it
        /// </summary>
        /// <param name="sy">Angle(in degrees) to skew on the y-axis</param>
        /// <returns>Skewed copy of this matrix</returns>
        public DOMMatrix SkewY(double sy)
        {
            return this.Clone().skewYSelf(sy);
        }

        /// <summary>
        /// Post-multiplies a copy of this matrix by <paramref name="other"/> and returns it
        /// </summary>
        /// <param name="other">Matrix to multiply by</param>
        /// <returns>Multiplied copy of this matrix</returns>
        public DOMMatrix Multiply(DOMMatrix other)
        {
            return this.Clone().multiplySelf(other);
        }

        /// <summary>
        /// Flips a copy of this matrix on the x-axis
        /// </summary>
        /// <returns>An x-axis flipped copy of this matrix</returns>
        public DOMMatrix FlipX()
        {/* Docs: https://www.w3.org/TR/geometry-1/#dom-dommatrixreadonly-flipx */
            return this.Clone().multiplySelf(new DOMMatrix(new DOMMatrixInit(-1, 0, 0, 1, 0, 0)));
        }

        /// <summary>
        /// Flips a copy of this matrix on the y-axis
        /// </summary>
        /// <returns>An y-axis flipped copy of this matrix</returns>
        public DOMMatrix FlipY()
        {/* Docs: https://www.w3.org/TR/geometry-1/#dom-dommatrixreadonly-flipy */
            return this.Clone().multiplySelf(new DOMMatrix(new DOMMatrixInit(1, 0, 0, -1, 0, 0)));
        }

        /// <summary>
        /// Inverts a copy of this matrix and returns it
        /// </summary>
        /// <returns>Inverse copy of this matrix</returns>
        public DOMMatrix Inverse()
        {/* Docs: https://www.w3.org/TR/geometry-1/#dom-dommatrixreadonly-inverse */
            return this.Clone().invertSelf();
        }

        /// <summary>
        /// Transforms a point by multiplying it against this matrix.
        /// </summary>
        /// <param name="Point">Point to transform</param>
        /// <returns>Transformed point</returns>
        public DOMPoint TransformPoint(DOMPointInit Point)
        {
            var x = (Point.x * Data[0] + Point.y * Data[4] + Point.z * Data[8]) + Data[12];
            var y = (Point.x * Data[1] + Point.y * Data[5] + Point.z * Data[9]) + Data[13];
            var z = (Point.x * Data[2] + Point.y * Data[6] + Point.z * Data[10]) + Data[14];
            var w = (Point.x * Data[3] + Point.y * Data[7] + Point.z * Data[11]) + Data[15];
            return new DOMPoint(x, y, z, w);
        }
        #endregion
    }
}
