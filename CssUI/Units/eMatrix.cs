
using System;

namespace CssUI
{
    public class eMatrix
    {
        public static readonly eMatrix Identity = new eMatrix();
        // Data
        public double[] Data = new double[16];

        // Functions
        public eMatrix()
        {// Set to identity matrix
            Data[ 0] = 1.0f; Data[ 1] = 0.0f; Data[ 2] = 0.0f; Data[ 3] = 0.0f;
            Data[ 4] = 0.0f; Data[ 5] = 1.0f; Data[ 6] = 0.0f; Data[ 7] = 0.0f;
            Data[ 8] = 0.0f; Data[ 9] = 0.0f; Data[10] = 1.0f; Data[11] = 0.0f;
            Data[12] = 0.0f; Data[13] = 0.0f; Data[14] = 0.0f; Data[15] = 1.0f;
        }

        public eMatrix(eMatrix m) : this()
        {
            this.CopyAll(m);
        }

        public eMatrix(double[] src)
        {
            CopyAll(src);
        }

        public void Set_Identity()
        {
            Data[ 0] = 1.0f; Data[ 1] = 0.0f; Data[ 2] = 0.0f; Data[ 3] = 0.0f;
            Data[ 4] = 0.0f; Data[ 5] = 1.0f; Data[ 6] = 0.0f; Data[ 7] = 0.0f;
            Data[ 8] = 0.0f; Data[ 9] = 0.0f; Data[10] = 1.0f; Data[11] = 0.0f;
            Data[12] = 0.0f; Data[13] = 0.0f; Data[14] = 0.0f; Data[15] = 1.0f;
        }

        #region COPYING FUNCTIONS
        public eMatrix CopyRot(eMatrix M)
        {
            Data[0] = M.Data[0]; Data[1] = M.Data[1]; Data[2] = M.Data[2];
            Data[4] = M.Data[4]; Data[5] = M.Data[5]; Data[6] = M.Data[6];
            Data[8] = M.Data[8]; Data[9] = M.Data[9]; Data[10] = M.Data[10];

            return this;
        }

        internal eMatrix CopyAll(eMatrix M)
        {
            return CopyAll(M.Data);
        }

        internal eMatrix CopyAll(double[] src)
        {
            Data[ 0] = src[ 0]; Data[ 1] = src[ 1]; Data[ 2] = src[ 2]; Data[ 3] = src[ 3];
            Data[ 4] = src[ 4]; Data[ 5] = src[ 5]; Data[ 6] = src[ 6]; Data[ 7] = src[ 7];
            Data[ 8] = src[ 8]; Data[ 9] = src[ 9]; Data[10] = src[10]; Data[11] = src[11];
            Data[12] = src[12]; Data[13] = src[13]; Data[14] = src[14]; Data[15] = src[15];

            return this;
        }
        #endregion

        public void Scale(double sx, double sy, double sz)
        {
            int x;
            for (x = 0; x <  4; x++) Data[x] *= sx;
            for (x = 4; x <  8; x++) Data[x] *= sy;
            for (x = 8; x < 12; x++) Data[x] *= sz;
        }

        public void Translate(eVector3 Point)
        {
            for (int j = 0; j < 4; j++)
            {
                Data[12 + j] += Point.X * Data[j] + Point.Y * Data[4 + j] + Point.Z * Data[8 + j];
            }
        }

        public void Set_Translation(eVector3 p)
        {
            Data[12] = p.X;
            Data[13] = p.Y;
            Data[14] = p.Z;
        }

        public void Set_Translation(double X, double Y, double Z)
        {
            Data[12] = X;
            Data[13] = Y;
            Data[14] = Z;
        }



        public eVector3 getViewVec()
        {
            return new eVector3(Data[2], Data[6], Data[10]);
        }

        public eVector3 getUpVec()
        {
            return new eVector3(Data[1], Data[5], Data[9]);
        }

        public eVector3 getRightVec()
        {
            return new eVector3(Data[0], Data[4], Data[8]);
        }

        public eVector3 getTranslate()
        {
            return new eVector3(Data[12], Data[13], Data[14]);
        }

        // Zero out the translation part of the matrix
        public eMatrix getRotMatrix()
        {
            var Temp = new eMatrix(this);
            Temp.Data[12] = 0;
            Temp.Data[13] = 0;
            Temp.Data[14] = 0;
            return Temp;
        }

        // Simple but not robust matrix inversion. (Doesn't work properly if there is a scaling or skewing transformation.)
        public eMatrix InvertSimple()
        {
            eMatrix R = new eMatrix();
            R.Data[0] = Data[0]; R.Data[1] = Data[4]; R.Data[2] = Data[8]; R.Data[3] = 0.0f;
            R.Data[4] = Data[1]; R.Data[5] = Data[5]; R.Data[6] = Data[9]; R.Data[7] = 0.0f;
            R.Data[8] = Data[2]; R.Data[9] = Data[6]; R.Data[10] = Data[10]; R.Data[11] = 0.0f;
            R.Data[12] = -(Data[12] * Data[0]) - (Data[13] * Data[1]) - (Data[14] * Data[2]);
            R.Data[13] = -(Data[12] * Data[4]) - (Data[13] * Data[5]) - (Data[14] * Data[6]);
            R.Data[14] = -(Data[12] * Data[8]) - (Data[13] * Data[9]) - (Data[14] * Data[10]);
            R.Data[15] = 1.0f;
            return R;
        }

        public eMatrix Transpose()
        {
            eMatrix R = new eMatrix();
            R.Data[0] = Data[8]; R.Data[1] = Data[4]; R.Data[2] = Data[0];
            R.Data[4] = Data[9]; R.Data[5] = Data[5]; R.Data[6] = Data[1];
            R.Data[8] = Data[10]; R.Data[9] = Data[6]; R.Data[10] = Data[2];

            R.Data[11] = R.Data[7] = R.Data[3] = R.Data[12] = R.Data[13] = R.Data[14] = 0.0f;
            R.Data[15] = 1.0f;
            return R;
        }


        #region ROTATION

        // Rotate the *this matrix fDegrees clockwise around a single axis( either x, y, or z )
        public void Rotate(double fDegrees, int x, int y, int z)
        {
            var M = new eMatrix();
            M.RotateMatrix(fDegrees, ((double)x), ((double)y), ((double)z));
            M *= this;
            this.CopyAll(M);
        }
        public void RotateXYZ(double rX, double rY, double rZ)
        {
            this.Rotate(rX, 1, 0, 0);
            this.Rotate(rY, 0, 1, 0);
            this.Rotate(rZ, 0, 0, 1);
        }
        public void RotateZYX(double rX, double rY, double rZ)
        {
            this.Rotate(rZ, 0, 0, 1);
            this.Rotate(rY, 0, 1, 0);
            this.Rotate(rX, 1, 0, 0);
        }

        const double PiOver180 = (Math.PI / 180.0);
        public double DegreesToRadians(double Degrees)
        {
            return Degrees * PiOver180;
        }

        // Create a rotation matrix for a counter-clockwise rotation of fDegrees around an arbitrary axis(x, y, z)
        public void RotateMatrix(double fDegrees, double x, double y, double z)
        {
            Set_Identity();
            double angle = DegreesToRadians(fDegrees);
            double cosA = Math.Cos(angle);
            double sinA = -Math.Sin(angle);
            double m = 1.0f - cosA;
            Data[0] = cosA + x * x * m;
            Data[5] = cosA + y * y * m;
            Data[10] = cosA + z * z * m;

            double tmp1 = x * y * m;
            double tmp2 = z * sinA;
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
        public eMatrix InvertRot()
        {
            var R = new eMatrix();
            R.Data[ 0] = Data[0]; R.Data[ 1] = Data[4]; R.Data[ 2] = Data[8];   R.Data[ 3] = 0.0f;
            R.Data[ 4] = Data[1]; R.Data[ 5] = Data[5]; R.Data[ 6] = Data[9];   R.Data[ 7] = 0.0f;
            R.Data[ 8] = Data[2]; R.Data[ 9] = Data[6]; R.Data[10] = Data[10];  R.Data[11] = 0.0f;
            R.Data[12] = 0;     R.Data[13] = 0;     R.Data[14] = 0;       R.Data[15] = 1.0f;
            return R;
        }

        // helpers for Rotate
        public eMatrix RotX(double deg)
        {
            double angle = DegreesToRadians(deg);
            Data[5] = Math.Cos(angle);
            Data[6] = -Math.Sin(angle);
            Data[9] = Math.Sin(angle);
            Data[10] = Math.Cos(angle);
            return this;
        }

        public eMatrix RotY(double deg)
        {
            double angle = DegreesToRadians(deg);
            Data[0] = Math.Cos(angle);
            Data[2] = Math.Sin(angle);
            Data[8] = -Math.Sin(angle);
            Data[10] = Math.Cos(angle);
            return this;
        }

        public eMatrix RotZ(double deg)
        {
            double angle = DegreesToRadians(deg);
            Data[0] = Math.Cos(angle);
            Data[1] = -Math.Sin(angle);
            Data[4] = Math.Sin(angle);
            Data[5] = Math.Cos(angle);
            return this;
        }
        #endregion

        #region OPERATORS
        // Concatenate 2 matrices with the * operator
        public static eMatrix operator *(eMatrix m, eMatrix InM)
        {
            var Result = new eMatrix();
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
        public static eVector3 operator *(eMatrix m, eVector3 Point)
        {
            double x = (Point.X * m.Data[0] + Point.Y * m.Data[4] + Point.Z * m.Data[8]) + m.Data[12];
            double y = (Point.X * m.Data[1] + Point.Y * m.Data[5] + Point.Z * m.Data[9]) + m.Data[13];
            double z = (Point.X * m.Data[2] + Point.Y * m.Data[6] + Point.Z * m.Data[10]) + m.Data[14];
            return new eVector3(x, y, z);
        }

        public static explicit operator string(eMatrix mat)
        {
            string str = "[";
            string[] arr = new string[16];
            int longest = 0;
            //first convert each value to a string and store in an array
            for (int i = 0; i < 16; i++)
            {
                arr[i] = string.Format("{0:0.##}", mat.Data[i]);
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
    };
}
