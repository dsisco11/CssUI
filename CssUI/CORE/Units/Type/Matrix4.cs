using System;
using System.Runtime.CompilerServices;
using System.Text;

namespace CssUI
{
    public class Matrix4
    {
        #region Static
        const int POW_LUT_SIZE = 2;
        private static double[,] POW_LUT;
        static Matrix4()
        {
            POW_LUT = new double[POW_LUT_SIZE, POW_LUT_SIZE];
            for(int i=0; i<POW_LUT_SIZE; i++)
            {
                for (int j = 0; j < POW_LUT_SIZE; j++)
                {
                    POW_LUT[i, j] = Math.Pow(-1, i + j);
                }
            }
        }
        #endregion

        #region Properties
        public double[] Data = new double[16];
        #endregion

        #region Indexing constants
        internal const int _m11 = 0;
        internal const int _m12 = 1;
        internal const int _m13 = 2;
        internal const int _m14 = 3;

        internal const int _m21 = 4;
        internal const int _m22 = 5;
        internal const int _m23 = 6;
        internal const int _m24 = 7;

        internal const int _m31 = 8;
        internal const int _m32 = 9;
        internal const int _m33 = 10;
        internal const int _m34 = 11;

        internal const int _m41 = 12;
        internal const int _m42 = 13;
        internal const int _m43 = 14;
        internal const int _m44 = 15;
        #endregion

        #region Accessors
        public double m11 { get => Data[_m11]; set => Data[_m11] = value; }
        public double m12 { get => Data[_m12]; set => Data[_m12] = value; }
        public double m13 { get => Data[_m13]; set => Data[_m13] = value; }
        public double m14 { get => Data[_m14]; set => Data[_m14] = value; }

        public double m21 { get => Data[_m21]; set => Data[_m21] = value; }
        public double m22 { get => Data[_m22]; set => Data[_m22] = value; }
        public double m23 { get => Data[_m23]; set => Data[_m23] = value; }
        public double m24 { get => Data[_m24]; set => Data[_m24] = value; }

        public double m31 { get => Data[_m31]; set => Data[_m31] = value; }
        public double m32 { get => Data[_m32]; set => Data[_m32] = value; }
        public double m33 { get => Data[_m33]; set => Data[_m33] = value; }
        public double m34 { get => Data[_m34]; set => Data[_m34] = value; }

        public double m41 { get => Data[_m41]; set => Data[_m41] = value; }
        public double m42 { get => Data[_m42]; set => Data[_m42] = value; }
        public double m43 { get => Data[_m43]; set => Data[_m43] = value; }
        public double m44 { get => Data[_m44]; set => Data[_m44] = value; }
        #endregion

        #region Constructors
        public Matrix4()
        {
            Identity();
        }

        public Matrix4(Matrix4 m)
        {
            Identity();
            CopyAll(m);
        }

        public Matrix4(float[] src)
        {
            CopyAll(src);
        }
        #endregion

        #region Identity
        public void Identity()
        {
            Data[0] = 1.0f; Data[1] = 0.0f; Data[2] = 0.0f; Data[3] = 0.0f;
            Data[4] = 0.0f; Data[5] = 1.0f; Data[6] = 0.0f; Data[7] = 0.0f;
            Data[8] = 0.0f; Data[9] = 0.0f; Data[10] = 1.0f; Data[11] = 0.0f;
            Data[12] = 0.0f; Data[13] = 0.0f; Data[14] = 0.0f; Data[15] = 1.0f;
        }
        #endregion

        #region COPYING FUNCTIONS
        public Matrix4 CopyRot(Matrix4 M)
        {
            Data[0] = M.Data[0]; Data[1] = M.Data[1]; Data[2] = M.Data[2];
            Data[4] = M.Data[4]; Data[5] = M.Data[5]; Data[6] = M.Data[6];
            Data[8] = M.Data[8]; Data[9] = M.Data[9]; Data[10] = M.Data[10];

            return this;
        }

        internal Matrix4 CopyAll(Matrix4 M)
        {
            return CopyAll(M.Data);
        }

        internal Matrix4 CopyAll(float[] src)
        {
            Data[0] = src[0]; Data[1] = src[1]; Data[2] = src[2]; Data[3] = src[3];
            Data[4] = src[4]; Data[5] = src[5]; Data[6] = src[6]; Data[7] = src[7];
            Data[8] = src[8]; Data[9] = src[9]; Data[10] = src[10]; Data[11] = src[11];
            Data[12] = src[12]; Data[13] = src[13]; Data[14] = src[14]; Data[15] = src[15];

            return this;
        }

        internal Matrix4 CopyAll(double[] src)
        {
            Data[0] = src[0]; Data[1] = src[1]; Data[2] = src[2]; Data[3] = src[3];
            Data[4] = src[4]; Data[5] = src[5]; Data[6] = src[6]; Data[7] = src[7];
            Data[8] = src[8]; Data[9] = src[9]; Data[10] = src[10]; Data[11] = src[11];
            Data[12] = src[12]; Data[13] = src[13]; Data[14] = src[14]; Data[15] = src[15];

            return this;
        }
        #endregion

        #region Scaling
        /// <summary>
        /// Applys axis scaling to the matrix
        /// </summary>
        /// <param name="sx">Scaling factor for X axis</param>
        /// <param name="sy">Scaling factor for Y axis</param>
        /// <param name="sz">Scaling factor for Z axis</param>
        public void Scale(float sx, float sy, float sz)
        {
            int x;
            for (x = 0; x < 4; x++) Data[x] *= sx;
            for (x = 4; x < 8; x++) Data[x] *= sy;
            for (x = 8; x < 12; x++) Data[x] *= sz;
        }
        #endregion

        #region Translation
        public void Translate(Point3f Point)
        {
            for (int j = 0; j < 4; j++)
            {
                Data[12 + j] += Point.X * Data[j] + Point.Y * Data[4 + j] + Point.Z * Data[8 + j];
            }
        }
        public void SetTranslate(Point3f p)
        {
            Data[12] = p.X;
            Data[13] = p.Y;
            Data[14] = p.Z;
        }

        public void Set_Translation(float X, float Y, float Z)
        {
            Data[12] = X;
            Data[13] = Y;
            Data[14] = Z;
        }
        #endregion

        #region Getters
        public Point3f getViewVec()
        {
            return new Point3f(Data[2], Data[6], Data[10]);
        }

        public Point3f getUpVec()
        {
            return new Point3f(Data[1], Data[5], Data[9]);
        }

        public Point3f getRightVec()
        {
            return new Point3f(Data[0], Data[4], Data[8]);
        }

        public Point3f getTranslate()
        {
            return new Point3f(Data[12], Data[13], Data[14]);
        }

        // Zero out the translation part of the matrix
        public Matrix4 getRotMatrix()
        {
            Matrix4 Temp = new Matrix4(this);
            Temp.Data[12] = 0;
            Temp.Data[13] = 0;
            Temp.Data[14] = 0;
            return Temp;
        }
        #endregion

        #region Internal
        /// <summary>
        /// Calculates the data index into a square matrix of a row <paramref name="i"/> and column <paramref name="j"/> without allowing <paramref name="i"/> or <paramref name="j"/> to overflow the square matricies bounds.
        /// </summary>
        /// <param name="size">Matrix Width</param>
        /// <param name="i">Row</param>
        /// <param name="j">Column</param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static int get_index(int size, int i, int j) => ((i % size) * size) + (j % size);

        /// <summary>
        /// Compiles a submatrix from the given square matrix by deleting row i and column j
        /// </summary>
        /// <param name="data">Matrix Values</param>
        /// <param name="size">Matrix Width</param>
        /// <param name="i"></param>
        /// <param name="j"></param>
        /// <param name="subData"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static void get_submatrix(double[] data, int size, int i, int j, out double[] subData)
        {
            int subSize = (size - 1) * (size - 1);
            double[] subMatrix = new double[subSize];
            int index = 0;

            for (int _i = 0; _i < size; _i++)
            {
                if (i == _i) continue; // skip row i
                for (int _j = 0; _j < size; _j++)
                {
                    if (j == _j) continue; // skip column j
                    subMatrix[index++] = data[get_index(size, _i, _j)];
                }
            }

            subData = subMatrix;
        }

        /// <summary>
        /// Calculates the determinant of a square matrix
        /// </summary>
        /// <param name="data">Matrix data</param>
        /// <param name="size">Matrix size</param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static double calc_determinant(double[] data, int size)
        {
            double Sum = 0;

            /* For each item in the row multiply it by its wrapped diagonal values (count = size) and add this to Sum */
            for (int j=0; j<size; j++)
            {/* j = the offset into our horizontal row */
                double value = 1;
                for (int c=0; c<size; c++)
                {/* c = our diagonal index */
                    //value *= data[rowIndex + j + ((c * size) + j)];
                    value *= data[get_index(size, c, j+c)];
                }
                Sum += value;
            }

            /* For each item in the row multiply it by its wrapped REVERSE diagonal values (count = size) and subtract this from Sum */
            for (int j = 0; j < size; j++)
            {/* j = the offset into our horizontal row */
                double value = 1;
                for (int c = 0; c < size; c++)
                {/* c = our diagonal index */
                    value *= data[get_index(size, c, j + c)];
                }
                Sum -= value;
            }

            return Sum;
        }

        /// <summary>
        /// Calculates the cofactor of a given item <paramref name="i"/>, <paramref name="j"/> within a square matrix
        /// </summary>
        /// <param name="data">Matrix Values</param>
        /// <param name="size">Matrix Width</param>
        /// <param name="i">Row</param>
        /// <param name="j">Column</param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static double calc_cofactor(double[] data, int size, int i, int j)
        {
            /* The cofactor of a given item i,j in a square matrix is the determinant of a sub-matrix consisting of all values except those in row i and column j */
            int subSize = (size - 1);
            get_submatrix(data, size, i, j, out double[] submatrix);

            // the real formula is -1^(i+j) but this pattern is repeated so im pretty sure this wrapped table will work and be faster then calculating a TON of powers
            return POW_LUT[i % 1, j % 1] * calc_determinant(submatrix, subSize);
        }

        /// <summary>
        /// Calculates the cofactor expansion of a given matrix, which is the expanded determinant.
        /// </summary>
        /// <param name="data">Matrix Values</param>
        /// <param name="size">Matrix Width</param>
        /// <param name="i">Row</param>
        /// <param name="j">Column</param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static double calc_cofactor_expansion(double[] data, int size)
        {
            /* The cofactor expansion of a given square matrix is the sum of the cofactors of each of its items */
            double Result = 0;

            for (int i = 0; i < size; i++)
            {
                //for (int j = 0; j < size; j++)
                //{
                    Result += data[get_index(size, i, 0)] * calc_cofactor(data, size, i, 0);
                //}
            }

            return Result;
        }

        /// <summary>
        /// Calculates the adjugant matrix of a given square matrix of size denoted by <paramref name="size"/>
        /// </summary>
        /// <param name="data">Matrix Values</param>
        /// <param name="size">Matrix Width</param>
        internal static double[] calc_adjugant_matrix(double[] data, int size)
        {/* https://en.wikipedia.org/wiki/Determinant */
            /* The adjugant matrix of a given matrix consists of the cofactor of each value in said matrix */
            /* A[i,j] = -1^(i+j)|M[j,i]| */
            double[] matrix = new double[size * size];
            for (int i=0; i<size; i++)
            {
                for (int j = 0; j < size; j++)
                {
                    matrix[get_index(size, i, j)] = calc_cofactor(data, size, j, i);
                }
            }

            return matrix;
        }

        /// <summary>
        /// Calculates the adjugant matrix of a given square matrix of size denoted by <paramref name="size"/>
        /// </summary>
        /// <param name="data">Matrix Values</param>
        /// <param name="size">Matrix Width</param>
        internal static double[] calc_inverse_matrix(double[] data, int size)
        {/* https://en.wikipedia.org/wiki/Determinant */
            /* The adjugant matrix of a given matrix consists of the cofactor of each value in said matrix */
            /* A-1 = (1/|A|)*~A */

            double determinant = calc_cofactor_expansion(data, size);
            double inverse = 1.0 / determinant;
            double[] adjunct = calc_adjugant_matrix(data, size);

            double[] matrix = new double[size * size];
            for (int i=0; i<size; i++)
            {
                for (int j = 0; j < size; j++)
                {
                    matrix[get_index(size, i, j)] = inverse * adjunct[get_index(size, i, j)];
                }
            }

            return matrix;
        }
        #endregion

        #region Determinant
        public double Determinant()
        {/* https://semath.info/src/inverse-cofactor-ex4.html */
            return (m11 * m22 * m33 * m44) + (m11 * m23 * m34 * m42) + (m11 * m24 * m32 * m43)
                 - (m11 * m24 * m33 * m42) - (m11 * m23 * m32 * m44) - (m11 * m22 * m34 * m43)
                 - (m12 * m21 * m33 * m44) - (m13 * m21 * m34 * m42) - (m14 * m21 * m32 * m43)
                 + (m14 * m21 * m33 * m42) + (m13 * m21 * m32 * m44) + (m12 * m21 * m34 * m43)
                 + (m12 * m23 * m31 * m44) + (m13 * m24 * m31 * m42) + (m14 * m22 * m31 * m43)
                 - (m14 * m23 * m31 * m42) - (m13 * m22 * m31 * m44) - (m12 * m24 * m31 * m43)
                 - (m12 * m23 * m34 * m41) - (m13 * m24 * m32 * m41) - (m14 * m22 * m33 * m41)
                 + (m14 * m23 * m32 * m41) + (m13 * m22 * m34 * m41) + (m12 * m24 * m33 * m41);
        }
        #endregion

        #region Inversion
        /// <summary>
        /// </summary>
        /// <returns></returns>
        public Matrix4 Invert()
        {/* https://en.wikipedia.org/wiki/Minor_(linear_algebra)#Inverse_of_a_matrix */
            Matrix4 R = new Matrix4();
            var invData = calc_inverse_matrix(this.Data, 4);
            R.CopyAll(invData);
            return R;
        }

        /// <summary>
        /// Simple but not robust matrix inversion. (Doesn't work properly if there is a scaling or skewing transformation.)
        /// </summary>
        /// <returns></returns>
        public Matrix4 InvertSimple()
        {
            Matrix4 R = new Matrix4();
            R.Data[0] = Data[0]; R.Data[1] = Data[4]; R.Data[2] = Data[8]; R.Data[3] = 0.0f;
            R.Data[4] = Data[1]; R.Data[5] = Data[5]; R.Data[6] = Data[9]; R.Data[7] = 0.0f;
            R.Data[8] = Data[2]; R.Data[9] = Data[6]; R.Data[10] = Data[10]; R.Data[11] = 0.0f;
            R.Data[12] = -(Data[12] * Data[0]) - (Data[13] * Data[1]) - (Data[14] * Data[2]);
            R.Data[13] = -(Data[12] * Data[4]) - (Data[13] * Data[5]) - (Data[14] * Data[6]);
            R.Data[14] = -(Data[12] * Data[8]) - (Data[13] * Data[9]) - (Data[14] * Data[10]);
            R.Data[15] = 1.0f;
            return R;
        }

        public Matrix4 Transpose()
        {
            Matrix4 R = new Matrix4();

            /*R.Data[0] = Data[8]; R.Data[1] = Data[4]; R.Data[2] = Data[0];
            R.Data[4] = Data[9]; R.Data[5] = Data[5]; R.Data[6] = Data[1];
            R.Data[8] = Data[10]; R.Data[9] = Data[6]; R.Data[10] = Data[2];

            R.Data[11] = R.Data[7] = R.Data[3] = R.Data[12] = R.Data[13] = R.Data[14] = 0.0f;
            R.Data[15] = 1.0f;*/

            R.m11 = m31; R.m12 = m21; R.m13 = m11;
            R.m21 = m32; R.m22 = m22; R.m23 = m12;
            R.m31 = m33; R.m32 = m23; R.m33 = m13;

            R.m34 = R.m24 = R.m14 = R.m41 = R.m42 = R.m43 = 0.0f;
            R.m44 = 1.0f;

            return R;
        }
        #endregion

        #region Rotation
        // Rotate the *this matrix fDegrees clockwise around a single axis( either x, y, or z )
        public void Rotate(float fDegrees, int x, int y, int z)
        {
            Matrix4 M = new Matrix4();
            M.RotateMatrix(fDegrees, ((float)x), ((float)y), ((float)z));
            M *= this;
            this.CopyAll(M);
        }
        public void RotateXYZ(float rX, float rY, float rZ)
        {
            this.Rotate(rX, 1, 0, 0);
            this.Rotate(rY, 0, 1, 0);
            this.Rotate(rZ, 0, 0, 1);
        }
        public void RotateZYX(float rX, float rY, float rZ)
        {
            this.Rotate(rZ, 0, 0, 1);
            this.Rotate(rY, 0, 1, 0);
            this.Rotate(rX, 1, 0, 0);
        }
        // Create a rotation matrix for a counter-clockwise rotation of fDegrees around an arbitrary axis(x, y, z)
        public void RotateMatrix(float fDegrees, float x, float y, float z)
        {
            Identity();
            float angle = MathExt.DegreesToRadians(fDegrees);
            float cosA = (float)Math.Cos(angle);
            float sinA = (float)-Math.Sin(angle);
            float m = 1.0f - cosA;
            Data[0] = cosA + x * x * m;
            Data[5] = cosA + y * y * m;
            Data[10] = cosA + z * z * m;

            float tmp1 = x * y * m;
            float tmp2 = z * sinA;
            Data[4] = tmp1 + tmp2;
            Data[1] = tmp1 - tmp2;

            tmp1 = x * z * m;
            tmp2 = y * sinA;
            Data[8] = tmp1 - tmp2;
            Data[2] = tmp1 + tmp2;

            tmp1 = y * z * m;
            tmp2 = x * sinA;
            Data[9] = tmp1 + tmp2;
            Data[6] = tmp1 - tmp2;
        }

        // Invert for only a rotation, any translation is zeroed out
        public Matrix4 InvertRot()
        {
            Matrix4 R = new Matrix4();
            R.Data[0] = Data[0]; R.Data[1] = Data[4]; R.Data[2] = Data[8]; R.Data[3] = 0.0f;
            R.Data[4] = Data[1]; R.Data[5] = Data[5]; R.Data[6] = Data[9]; R.Data[7] = 0.0f;
            R.Data[8] = Data[2]; R.Data[9] = Data[6]; R.Data[10] = Data[10]; R.Data[11] = 0.0f;
            R.Data[12] = 0; R.Data[13] = 0; R.Data[14] = 0; R.Data[15] = 1.0f;
            return R;
        }
        #endregion

        #region Rotation Transforms
        // helpers for Rotate
        public static Matrix4 get_X_Rotation_Transform(float deg)
        {
            var matrix = new Matrix4();
            float angle = MathExt.DegreesToRadians(deg);
            matrix.Data[5] = (float)Math.Cos(angle);
            matrix.Data[6] = -(float)Math.Sin(angle);
            matrix.Data[9] = (float)Math.Sin(angle);
            matrix.Data[10] = (float)Math.Cos(angle);
            return matrix;
        }

        public static Matrix4 get_Y_Rotation_Transform(float deg)
        {
            var matrix = new Matrix4();
            float angle = MathExt.DegreesToRadians(deg);
            matrix.Data[0] = (float)Math.Cos(angle);
            matrix.Data[2] = (float)Math.Sin(angle);
            matrix.Data[8] = -(float)Math.Sin(angle);
            matrix.Data[10] = (float)Math.Cos(angle);
            return matrix;
        }

        public static Matrix4 get_Z_Rotation_Transform(float deg)
        {
            var matrix = new Matrix4();
            float angle = MathExt.DegreesToRadians(deg);
            matrix.Data[0] = (float)Math.Cos(angle);
            matrix.Data[1] = -(float)Math.Sin(angle);
            matrix.Data[4] = (float)Math.Sin(angle);
            matrix.Data[5] = (float)Math.Cos(angle);
            return matrix;
        }
        #endregion

        #region Operators
        // Concatenate 2 matrices with the * operator
        public static Matrix4 operator *(Matrix4 m, Matrix4 InM)
        {
            Matrix4 Result = new Matrix4();
            for (int i = 0; i < 16; i += 4)
            {
                for (int j = 0; j < 4; j++)
                {
                    Result.Data[i + j] = m.Data[i + 0] * InM.Data[0 + j] + m.Data[i + 1] * InM.Data[4 + j]
                        + m.Data[i + 2] * InM.Data[8 + j] + m.Data[i + 3] * InM.Data[12 + j];
                }
            }
            return Result;
        }

        // Use a matrix to transform a 3D point with the * operator
        public static Point3f operator *(Matrix4 m, Point3f Point)
        {
            double x = (Point.X * m.Data[0] + Point.Y * m.Data[4] + Point.Z * m.Data[8]) + m.Data[12];
            double y = (Point.X * m.Data[1] + Point.Y * m.Data[5] + Point.Z * m.Data[9]) + m.Data[13];
            double z = (Point.X * m.Data[2] + Point.Y * m.Data[6] + Point.Z * m.Data[10]) + m.Data[14];
            return new Point3f(x, y, z);
        }


        public override int GetHashCode()
        {
            int hash = 17;
            hash = (hash * 31) + Data[0].GetHashCode();
            hash = (hash * 31) + Data[1].GetHashCode();
            hash = (hash * 31) + Data[2].GetHashCode();
            hash = (hash * 31) + Data[3].GetHashCode();
            hash = (hash * 31) + Data[4].GetHashCode();
            hash = (hash * 31) + Data[5].GetHashCode();
            hash = (hash * 31) + Data[6].GetHashCode();
            hash = (hash * 31) + Data[7].GetHashCode();
            hash = (hash * 31) + Data[8].GetHashCode();
            hash = (hash * 31) + Data[9].GetHashCode();
            hash = (hash * 31) + Data[10].GetHashCode();
            hash = (hash * 31) + Data[11].GetHashCode();
            hash = (hash * 31) + Data[12].GetHashCode();
            hash = (hash * 31) + Data[13].GetHashCode();
            hash = (hash * 31) + Data[14].GetHashCode();
            hash = (hash * 31) + Data[15].GetHashCode();
            return hash;
        }
        #endregion

        #region Explicit Casts
        public static explicit operator string(Matrix4 mat)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("[");
            string[] arr = new string[16];
            int longest = 0;
            //first convert each value to a string and store in an array
            for (int i = 0; i < 16; i++)
            {
                arr[i] = mat.Data[i].ToString("0.##");
                longest = MathExt.Max(longest, arr[i].Length);
            }

            for (int i = 0; i < 16; i++)
            {
                sb.Append( arr[i].PadLeft(longest) );
                if ((i % 4) < 3) sb.Append(',');
                if ((i % 4) == 3)
                {
                    sb.Append("]\n");
                    if (i < 13) sb.Append('[');
                }
            }

            return sb.ToString();
        }
        #endregion
    }
}
