using System;

namespace CssUI
{
    /// <summary>
    /// Holds ratio values for both Width/Height and Height/Width
    /// <para>Used to cause a <see cref="cssElement"/> to maintain a certain aspect ratio</para>
    /// </summary>
    public class SizeRatio
    {
        /// <summary>
        /// Ratio of Width to Height
        /// <para>Usage: Width = (Height * WH)</para>
        /// </summary>
        public readonly float WH = 1f;
        /// <summary>
        /// Ratio of Height to Width
        /// <para>Usage: Height = (Width * HW)</para>
        /// </summary>
        public readonly float HW = 1f;

        public SizeRatio(int Width, int Height)
        {
            if (Width < 0) throw new ArgumentException("Width must be greater then 0!");
            if (Height < 0) throw new ArgumentException("Height must be greater then 0!");

            WH = ((float)Width / (float)Height);
            HW = ((float)Height / (float)Width);
        }
    }
}
