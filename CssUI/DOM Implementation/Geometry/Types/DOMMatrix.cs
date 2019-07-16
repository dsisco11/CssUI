using System;

namespace CssUI.DOM.Geometry
{
    /// <summary>
    /// Represent a mathematical matrix
    /// </summary>
    public class DOMMatrix : DOMMatrixReadOnly
    {
        #region Accessors
        public new double m11 { get => Data[_m11]; set => Data[_m11] = value; }
        public new double m12 { get => Data[_m12]; set => Data[_m12] = value; }
        public new double m13
        {
            get => Data[_m13];
            set
            {
                Data[_m13] = value;
                if (!MathExt.floatEq(value, 0.0))
                    is2D = false;
            }
        }
        public new double m14
        {
            get => Data[3];
            set
            {
                Data[3] = value;
                if (!MathExt.floatEq(value, 0.0))
                    is2D = false;
            }
        }
        public new double m21 { get => Data[_m21]; set => Data[_m21] = value; }
        public new double m22 { get => Data[_m22]; set => Data[_m22] = value; }
        public new double m23
        {
            get => Data[_m23];
            set
            {
                Data[_m23] = value;
                if (!MathExt.floatEq(value, 0.0))
                    is2D = false;
            }
        }
        public new double m24
        {
            get => Data[_m24];
            set
            {
                Data[_m24] = value;
                if (!MathExt.floatEq(value, 0.0))
                    is2D = false;
            }
        }
        public new double m31
        {
            get => Data[_m31];
            set
            {
                Data[_m31] = value;
                if (!MathExt.floatEq(value, 1.0))
                    is2D = false;
            }
        }
        public new double m32
        {
            get => Data[_m32];
            set
            {
                Data[_m32] = value;
                if (!MathExt.floatEq(value, 0.0))
                    is2D = false;
            }
        }
        public new double m33
        {
            get => Data[_m33];
            set
            {
                Data[_m33] = value;
                if (!MathExt.floatEq(value, 1.0))
                    is2D = false;
            }
        }
        public new double m34
        {
            get => Data[_m34];
            set
            {
                Data[_m34] = value;
                if (!MathExt.floatEq(value, 0.0))
                    is2D = false;
            }
        }
        public new double m41 { get => Data[_m41]; set => Data[_m41] = value; }
        public new double m42 { get => Data[_m42]; set => Data[_m42] = value; }
        public new double m43
        {
            get => Data[_m43];
            set
            {
                Data[_m43] = value;
                if (!MathExt.floatEq(value, 0.0))
                    is2D = false;
            }
        }
        public new double m44
        {
            get => Data[_m44];
            set
            {
                Data[_m44] = value;
                if (!MathExt.floatEq(value, 1.0))
                    is2D = false;
            }
        }
        #endregion

        #region Constructors
        /// <summary>
        /// Creates a new identity matrix
        /// </summary>
        internal DOMMatrix()
        {
            this.identitySelf();
        }

        public DOMMatrix(params double[] init) : base(init)
        {
        }

        public DOMMatrix(DOMMatrixInit init) : base(init)
        {
        }

        public DOMMatrix(DOMMatrix2DInit init) : base(init)
        {
        }
        #endregion

        #region Instantiation
        public static new DOMMatrix fromMatrix(DOMMatrixInit other)
        {
            return new DOMMatrix(other);
        }

        public static new DOMMatrix fromFloat32Array(params float[] array32)
        {
            double[] array = new double[array32.Length];
            for (int i = 0; i < array32.Length; i++) { array[i] = array32[i]; }
            return new DOMMatrix(array);
        }

        public static new DOMMatrix fromFloat64Array(params double[] array64)
        {
            return new DOMMatrix(array64);
        }
        #endregion

        #region COPYING FUNCTIONS
        public DOMMatrix CopyRot(DOMMatrix M)
        {
            Data[0] = M.Data[0]; Data[1] = M.Data[1]; Data[2] = M.Data[2];
            Data[4] = M.Data[4]; Data[5] = M.Data[5]; Data[6] = M.Data[6];
            Data[8] = M.Data[8]; Data[9] = M.Data[9]; Data[10] = M.Data[10];

            return this;
        }

        internal DOMMatrix CopyValues(DOMMatrix M)
        {
            return CopyValues(M.Data);
        }

        internal DOMMatrix CopyValues(double[] src)
        {
            /* Just replace the data structure when copying from an array */
            Data = src;
            /*
            Data[0] = src[0]; Data[1] = src[1]; Data[2] = src[2]; Data[3] = src[3];
            Data[4] = src[4]; Data[5] = src[5]; Data[6] = src[6]; Data[7] = src[7];
            Data[8] = src[8]; Data[9] = src[9]; Data[10] = src[10]; Data[11] = src[11];
            Data[12] = src[12]; Data[13] = src[13]; Data[14] = src[14]; Data[15] = src[15];
            */
            return this;
        }
        #endregion
        
        #region Rotation Transforms
        // helpers for Rotate
        public static DOMMatrix get_X_Rotation_Transform(double deg)
        {
            var matrix = new DOMMatrix();
            double rads = deg * MathExt.Radians;
            matrix.Data[5] = Math.Cos(rads);
            matrix.Data[6] =  -Math.Sin(rads);
            matrix.Data[9] = Math.Sin(rads);
            matrix.Data[10] = Math.Cos(rads);
            return matrix;
        }

        public static DOMMatrix get_Y_Rotation_Transform(double deg)
        {
            var matrix = new DOMMatrix();
            double rads = deg * MathExt.Radians;
            matrix.Data[0] = Math.Cos(rads);
            matrix.Data[2] = Math.Sin(rads);
            matrix.Data[8] = -Math.Sin(rads);
            matrix.Data[10] = Math.Cos(rads);
            return matrix;
        }

        public static DOMMatrix get_Z_Rotation_Transform(double deg)
        {
            var matrix = new DOMMatrix();
            double rads = deg * MathExt.Radians;
            matrix.Data[0] = Math.Cos(rads);
            matrix.Data[1] = -Math.Sin(rads);
            matrix.Data[4] = Math.Sin(rads);
            matrix.Data[5] = Math.Cos(rads);
            return matrix;
        }

        public static DOMMatrix get_Rotation_Transform(double axisX, double axisY, double axisZ, double alpha)
        {
            DOMMatrix matrix = new DOMMatrix();
            double angle = MathExt.Radians * alpha;
            double cosA = Math.Cos(angle);
            double sinA = -Math.Sin(angle);
            double m = 1.0f - cosA;
            matrix.Data[0] = cosA + axisX * axisX * m;
            matrix.Data[5] = cosA + axisY * axisY * m;
            matrix.Data[10] = cosA + axisZ * axisZ * m;

            double tmp1 = axisX * axisY * m;
            double tmp2 = axisZ * sinA;
            matrix.Data[4] = tmp1 + tmp2;
            matrix.Data[1] = tmp1 - tmp2;

            tmp1 = axisX * axisZ * m;
            tmp2 = axisY * sinA;
            matrix.Data[8] = tmp1 - tmp2;
            matrix.Data[2] = tmp1 + tmp2;

            tmp1 = axisY * axisZ * m;
            tmp2 = axisX * sinA;
            matrix.Data[9] = tmp1 + tmp2;
            matrix.Data[6] = tmp1 - tmp2;

            return matrix;
        }
        #endregion

        #region Internal Utility
        /// <summary>
        /// Sets this matrix to the identity matrix and returns it
        /// </summary>
        /// <returns></returns>
        internal DOMMatrix identitySelf()
        {
            Data[0] = 1.0f; Data[1] = 0.0f; Data[2] = 0.0f; Data[3] = 0.0f;
            Data[4] = 0.0f; Data[5] = 1.0f; Data[6] = 0.0f; Data[7] = 0.0f;
            Data[8] = 0.0f; Data[9] = 0.0f; Data[10] = 1.0f; Data[11] = 0.0f;
            Data[12] = 0.0f; Data[13] = 0.0f; Data[14] = 0.0f; Data[15] = 1.0f;

            return this;
        }
        #endregion

        #region Mutable Transformations
        /// <summary>
        /// Performs a post-multiplication of <paramref name="other"/> against this matrix and returns it.
        /// </summary>
        /// <returns>This matrix after <paramref name="other"/> if post-multiplied against it</returns>
        public DOMMatrix multiplySelf(DOMMatrix other)
        {/* Docs: https://www.w3.org/TR/geometry-1/#dom-dommatrix-multiplyself */
            for (int i = 0; i < 16; i += 4)
            {
                for (int j = 0; j < 4; j++)
                {
                    this.Data[i + j] = other.Data[i + 0] * this.Data[0 + j] + other.Data[i + 1] * this.Data[4 + j]
                        + other.Data[i + 2] * this.Data[8 + j] + other.Data[i + 3] * this.Data[12 + j];
                }
            }

            if (!other.is2D) is2D = false;
            return this;
        }

        /// <summary>
        /// Performs a pre-multiplication of <paramref name="other"/> against this matrix and returns it.
        /// </summary>
        /// <returns>This matrix after <paramref name="other"/> if pre-multiplied against it</returns>
        public DOMMatrix preMultiplySelf(DOMMatrix other)
        {/* Docs: https://www.w3.org/TR/geometry-1/#dom-dommatrix-premultiplyself */
            for (int i = 0; i < 16; i += 4)
            {
                for (int j = 0; j < 4; j++)
                {
                    this.Data[i + j] = this.Data[i + 0] * other.Data[0 + j] + this.Data[i + 1] * other.Data[4 + j]
                        + this.Data[i + 2] * other.Data[8 + j] + this.Data[i + 3] * other.Data[12 + j];
                }
            }

            if (!other.is2D) is2D = false;
            return this;
        }

        /// <summary>
        /// Translates this matrix and returns it.
        /// </summary>
        /// <param name="tx"></param>
        /// <param name="ty"></param>
        /// <param name="tz"></param>
        /// <returns></returns>
        public DOMMatrix translateSelf(double tx, double ty, double tz = 0.0)
        {/* Docs: https://www.w3.org/TR/geometry-1/#dom-dommatrix-translateself */
            if (!MathExt.floatEq(tz, 0.0)) is2D = false;

            for (int j = 0; j < 4; j++)
            {
                Data[12 + j] += tx * Data[j] + ty * Data[4 + j] + tz * Data[8 + j];
            }

            return this;
        }

        /// <summary>
        /// Scales this matrix and returns it.
        /// </summary>
        /// <param name="scaleX">X-Axis scaling factor</param>
        /// <param name="scaleY">Y-Axis scaling factor</param>
        /// <param name="scaleZ">Z-Axis scaling factor</param>
        /// <param name="originX">X-Axis position to scale around</param>
        /// <param name="originY">Y-Axis position to scale around</param>
        /// <param name="originZ">Z-Axis position to scale around</param>
        /// <returns>This matrix with scaling applied to it</returns>
        public DOMMatrix scaleSelf(double scaleX, double scaleY, double scaleZ, double originX, double originY, double originZ)
        {/* Docs: https://www.w3.org/TR/geometry-1/#dom-dommatrix-scaleself */
            if (!MathExt.floatEq(scaleZ, 1.0) || !MathExt.floatEq(originZ, 0.0)) is2D = false;
            translateSelf(originX, originY, originZ);

            int x;
            for (x = 0; x < 4; x++) Data[x] *= scaleX;
            for (x = 4; x < 8; x++) Data[x] *= scaleY;
            for (x = 8; x < 12; x++) Data[x] *= scaleZ;

            translateSelf(-originX, -originY, -originZ);
            return this;
        }

        /// <summary>
        /// Scales this matrix and returns it.
        /// </summary>
        /// <param name="scale">Scaling factor for all axi</param>
        /// <param name="originX">X-Axis position to scale around</param>
        /// <param name="originY">Y-Axis position to scale around</param>
        /// <param name="originZ">Z-Axis position to scale around</param>
        /// <returns>This matrix with scaling applied to it</returns>
        public DOMMatrix scale3DSelf(double scale, double originX, double originY, double originZ)
        {/* Docs: https://www.w3.org/TR/geometry-1/#dom-dommatrix-scale3dself */
            if (!MathExt.floatEq(scale, 1.0)) is2D = false;
            translateSelf(originX, originY, originZ);

            int x;
            for (x = 0; x < 4; x++) Data[x] *= scale;
            for (x = 4; x < 8; x++) Data[x] *= scale;
            for (x = 8; x < 12; x++) Data[x] *= scale;

            translateSelf(-originX, -originY, -originZ);
            return this;
        }

        /// <summary>
        /// Rotates(ZYX) this matrix and returns it.
        /// </summary>
        /// <param name="rotX">X-Axis rotation (in degrees)</param>
        /// <param name="rotY">Y-Axis rotation (in degrees)</param>
        /// <param name="rotZ">Z-Axis rotation (in degrees)</param>
        /// <returns>This matrix with rotation applied to it</returns>
        public DOMMatrix rotateSelf(double rotX, double rotY, double rotZ)
        {/* Docs: https://www.w3.org/TR/geometry-1/#dom-dommatrix-rotateself */
            if (!MathExt.floatEq(rotX, 0.0) || !MathExt.floatEq(rotY, 0.0)) is2D = false;
            multiplySelf(get_Z_Rotation_Transform(rotZ));
            multiplySelf(get_Y_Rotation_Transform(rotY));
            multiplySelf(get_X_Rotation_Transform(rotX));
            return this;
        }

        /// <summary>
        /// Rotates(Z-Axis) this matrix to face the given vector and returns it
        /// </summary>
        /// <param name="x">X-Axis position</param>
        /// <param name="y">Y-Axis position</param>
        /// <returns>This matrix with rotation applied to it</returns>
        public DOMMatrix rotateFromVectorSelf(double x = 0, double y = 0)
        {/* Docs: https://www.w3.org/TR/geometry-1/#dom-dommatrix-rotatefromvectorself */
            /* "The rotation angle is determined by the angle between the vector (1,0)T and (x,y)T in the clockwise direction." */
            double alpha = 0;
            if (!MathExt.floatEq(x, 0) || !MathExt.floatEq(y, 0))
                alpha = Math.Atan((y - 0) / (x - 1));
            /* "The 2D rotation matrix is described in CSS Transforms where alpha is the angle between the vector (1,0)T and (x,y)T in degrees." */
            multiplySelf(get_Z_Rotation_Transform(alpha * (1.0 / MathExt.Radians)));
            return this;
        }

        /// <summary>
        /// Rotates a copy of this matrix around the given axis by <paramref name="alpha"/> degrees.
        /// </summary>
        /// <param name="axisX">Axis vector X factor</param>
        /// <param name="axisY">Axis vector Y factor</param>
        /// <param name="axisZ">Axis vector Z factor</param>
        /// <param name="alpha">Degrees of rotation</param>
        /// <returns>This matrix with rotation applied to it</returns>
        public DOMMatrix rotateAxisAngleSelf(double axisX = 0, double axisY = 0, double axisZ = 0, double alpha = 0)
        {/* Docs: https://www.w3.org/TR/geometry-1/#dom-dommatrix-rotateaxisangleself */

            if (!MathExt.floatEq(axisX, 0) || !MathExt.floatEq(axisY, 0))
                is2D = true;

            multiplySelf(get_Rotation_Transform(axisX, axisY, axisZ, alpha));
            return this;

        }

        /// <summary>
        /// Skews this matrix on the X axis and returns it
        /// </summary>
        /// <param name="sx">Angle(in degrees) to skew on the x-axis</param>
        /// <returns>This matrix skewed along the x-axis</returns>
        public DOMMatrix skewXSelf(double sx)
        {/* Docs: https://www.w3.org/TR/geometry-1/#dom-dommatrix-skewxself */
            var skew = new DOMMatrix();
            skew.m21 = Math.Tan(sx);
            multiplySelf(skew);
            return this;
        }

        /// <summary>
        /// Skews this matrix on the Y axis and returns it
        /// </summary>
        /// <param name="sy">Angle(in degrees) to skew on the y-axis</param>
        /// <returns>This matrix skewed along the y-axis</returns>
        public DOMMatrix skewYSelf(double sy)
        {/* Docs: https://www.w3.org/TR/geometry-1/#dom-dommatrix-skewyself */
            var skew = new DOMMatrix();
            skew.m12 = Math.Tan(sy);
            multiplySelf(skew);
            return this;
        }

        /// <summary>
        /// Inverts this matrix and returns it
        /// </summary>
        /// <returns>This matrix inverted</returns>
        public DOMMatrix invertSelf()
        {/* Docs: https://www.w3.org/TR/geometry-1/#dom-dommatrix-invertself */
            this.CopyValues(Matrix4.calc_inverse_matrix(Data, 4));
            return this;
        }
        #endregion

    }
}
