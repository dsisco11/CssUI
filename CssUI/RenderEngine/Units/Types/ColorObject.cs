using System;
using System.Diagnostics.Contracts;
using System.Globalization;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace CssUI.Rendering
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public abstract class ColorObject<T> : IColorObject
    {
        const float fbyteMax = byte.MaxValue;

        public abstract T From(uint n);
        public abstract T From(Vector4 vector);
               

        #region Scalars
        public abstract Vector4 GetVector();
        public abstract void SetVector(Vector4 RGBA);
        #endregion

        #region Conversion
        public abstract uint AsInteger();
        #endregion


        #region Operators
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T operator *(ColorObject<T> left, IColorObject right)
        {
            if (left is null) throw new ArgumentNullException(nameof(left));
            if (right is null) throw new ArgumentNullException(nameof(right));
            Contract.EndContractBlock();

            return left.From((left.GetVector() * right.GetVector()));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T operator /(ColorObject<T> left, IColorObject right)
        {
            if (left is null) throw new ArgumentNullException(nameof(left));
            if (right is null) throw new ArgumentNullException(nameof(right));
            Contract.EndContractBlock();

            return left.From(left.GetVector() / right.GetVector());
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T operator +(ColorObject<T> left, IColorObject right)
        {
            if (left is null) throw new ArgumentNullException(nameof(left));
            if (right is null) throw new ArgumentNullException(nameof(right));
            Contract.EndContractBlock();

            return left.From(left.GetVector() + right.GetVector());
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T operator -(ColorObject<T> left, IColorObject right)
        {
            if (left is null) throw new ArgumentNullException(nameof(left));
            if (right is null) throw new ArgumentNullException(nameof(right));
            Contract.EndContractBlock();

            return left.From(left.GetVector() - right.GetVector());
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public T Multiply(ColorObject<T> left, IColorObject right)
        {
            return left * right;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public T Divide(ColorObject<T> left, IColorObject right)
        {
            return left / right;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public T Add(ColorObject<T> left, IColorObject right)
        {
            return left + right;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public T Subtract(ColorObject<T> left, IColorObject right)
        {
            return left - right;
        }
        #endregion

        #region Mixing
        /// <summary>
        /// Linearly interpolates between this color and another using the given <paramref name="blendFactor"/>.
        /// </summary>
        /// <param name="right">The color to interpolate toward</param>
        /// <param name="blendFactor">[0-1] range factor for interpolation</param>
        public T Mix(IColorObject right, float blendFactor)
        {// Linear Interp:  (x * (1.0 - i) + y * i);
            if (right is null) throw new ArgumentNullException(nameof(right));
            //if (blendFactor < 0 || blendFactor > 1f) throw new ArgumentOutOfRangeException(nameof(blendFactor));// No need to constrain this, let them have freedom to abuse it
            Contract.EndContractBlock();

            var RetVal = (GetVector() * (1f - blendFactor)) + (right.GetVector() * blendFactor);
            RetVal *= fbyteMax;
            return From(RetVal);
        }

        /// <summary>
        /// Returns a clone of this color with the alpha scaled by the given amount.
        /// </summary>
        /// <param name="Factor">Factor to multiply the alpha by</param>
        /// <returns></returns>
        public T MixAlpha(float Factor)
        {
            var RetVal = GetVector();
            RetVal.W *= Factor;
            return From(RetVal);
        }
        #endregion

        #region Equality
        public override bool Equals(object obj)
        {
            if (obj is IColorObject C)
            {
                return C.AsInteger() == AsInteger();
            }
            return false;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public static bool operator ==(ColorObject<T> left, IColorObject right)
        {
            return left?.Equals(right) ?? false;
        }

        public static bool operator !=(ColorObject<T> left, IColorObject right)
        {
            return !(left == right);
        }
        #endregion

    }
}
