using System;
using System.Numerics;

namespace CssUI
{
    public class Matrix4
    {
        #region Properties
        public float[] Data = new float[16];
        #endregion

        #region Constructors
        public Matrix4()
        {
            Identity();
        }

        public Matrix4(Matrix4 m)
        {
            this.Identity();
            this.CopyAll(m);
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
        public void Translate(Vec3 Point)
        {
            for (int j = 0; j < 4; j++)
            {
                Data[12 + j] += Point.X * Data[j] + Point.Y * Data[4 + j] + Point.Z * Data[8 + j];
            }
        }
        public void SetTranslate(Vec3 p)
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
        public Vec3 getViewVec()
        {
            return new Vec3(Data[2], Data[6], Data[10]);
        }

        public Vec3 getUpVec()
        {
            return new Vec3(Data[1], Data[5], Data[9]);
        }

        public Vec3 getRightVec()
        {
            return new Vec3(Data[0], Data[4], Data[8]);
        }

        public Vec3 getTranslate()
        {
            return new Vec3(Data[12], Data[13], Data[14]);
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

        #region Inversion
        // Simple but not robust matrix inversion. (Doesn't work properly if there is a scaling or skewing transformation.)
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
            R.Data[0] = Data[8]; R.Data[1] = Data[4]; R.Data[2] = Data[0];
            R.Data[4] = Data[9]; R.Data[5] = Data[5]; R.Data[6] = Data[1];
            R.Data[8] = Data[10]; R.Data[9] = Data[6]; R.Data[10] = Data[2];

            R.Data[11] = R.Data[7] = R.Data[3] = R.Data[12] = R.Data[13] = R.Data[14] = 0.0f;
            R.Data[15] = 1.0f;
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

        // helpers for Rotate
        public Matrix4 RotX(float deg)
        {
            float angle = MathExt.DegreesToRadians(deg);
            Data[5] = (float)Math.Cos(angle);
            Data[6] = -(float)Math.Sin(angle);
            Data[9] = (float)Math.Sin(angle);
            Data[10] = (float)Math.Cos(angle);
            return this;
        }

        public Matrix4 RotY(float deg)
        {
            float angle = MathExt.DegreesToRadians(deg);
            Data[0] = (float)Math.Cos(angle);
            Data[2] = (float)Math.Sin(angle);
            Data[8] = -(float)Math.Sin(angle);
            Data[10] = (float)Math.Cos(angle);
            return this;
        }

        public Matrix4 RotZ(float deg)
        {
            float angle = MathExt.DegreesToRadians(deg);
            Data[0] = (float)Math.Cos(angle);
            Data[1] = -(float)Math.Sin(angle);
            Data[4] = (float)Math.Sin(angle);
            Data[5] = (float)Math.Cos(angle);
            return this;
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
        public static Vec3 operator *(Matrix4 m, Vec3 Point)
        {
            float x = (Point.X * m.Data[0] + Point.Y * m.Data[4] + Point.Z * m.Data[8]) + m.Data[12];
            float y = (Point.X * m.Data[1] + Point.Y * m.Data[5] + Point.Z * m.Data[9]) + m.Data[13];
            float z = (Point.X * m.Data[2] + Point.Y * m.Data[6] + Point.Z * m.Data[10]) + m.Data[14];
            return new Vec3(x, y, z);
        }
        #endregion

        #region Explicit Casts
        public static explicit operator Matrix3x2(Matrix4 mat)
        {
            return new Matrix3x2(mat.Data[0], mat.Data[4], 
                                 mat.Data[1], mat.Data[5],
                                 mat.Data[2], mat.Data[6]);
        }

        public static explicit operator string(Matrix4 mat)
        {
            String str = "[";
            string[] arr = new string[16];
            int longest = 0;
            //first convert each value to a string and store in an array
            for (int i = 0; i < 16; i++)
            {
                arr[i] = String.Format("{0:0.##}", mat.Data[i]);
                longest = Math.Max(longest, arr[i].Length);
            }

            for (int i = 0; i < 16; i++)
            {
                str += arr[i].PadLeft(longest);
                if ((i % 4) < 3) str += ',';
                if ((i % 4) == 3)
                {
                    str += "]\n";
                    if (i < 13) str += '[';
                }
            }

            return str;
        }
        #endregion
    }
}
