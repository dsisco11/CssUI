using System;
using System.Runtime.CompilerServices;

namespace CssUI.CSS
{
    /// <summary>
    /// Holds various commonly used CSS specification defined algorithms
    /// </summary>
    public static class CssAlgorithms
    {
        /// <summary>
        /// Applies (Along a single axis) the CSS specification formula for resolving object relative positions using the values given
        /// </summary>
        /// <param name="Pos">Axis-Position to resolve</param>
        /// <param name="ObjectArea">Axis-Size of the area the object resides in</param>
        /// <param name="ObjectSize">Axis-Size of the object itsself</param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int Solve_Object_Axis_Position(float Percent, int ObjectArea, int ObjectSize)
        {/* https://www.w3.org/TR/css-backgrounds-3/#the-background-position */
            return (int)((Percent * (float)ObjectArea) - (Percent * (float)ObjectSize));
        }
        /// <summary>
        /// Applies (Along a single axis) the CSS specification formula for resolving object relative positions using the values given
        /// </summary>
        /// <param name="Pos">Axis-Position to resolve</param>
        /// <param name="ObjectArea">Axis-Size of the area the object resides in</param>
        /// <param name="ObjectSize">Axis-Size of the object itsself</param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int Solve_Object_Axis_Position(CssValue Pos, int ObjectArea, int ObjectSize)
        {/* https://www.w3.org/TR/css-backgrounds-3/#the-background-position */
            if (Pos.Type != ECssValueType.PERCENT)
                throw new ArgumentException("Pos must be a Css percent-type value!");

            float P = (float)Pos.Value;
            return (int)((P * (float)ObjectArea) - (P * (float)ObjectSize));
        }

        /// <summary>
        /// Applies the CSS specification formula for resolving object relative positions using the values given
        /// </summary>
        /// <param name="xPos">Positioning (along x-axis) to resolve </param>
        /// <param name="yPos">Positioning (along y-axis) to resolve </param>
        /// <param name="ObjectArea">Size of the area the object resides in</param>
        /// <param name="ObjectSize">Size of the object itsself</param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vec2i Solve_Object_Position(CssValue xPos, CssValue yPos, Size2D ObjectArea, Size2D ObjectSize)
        {/* https://www.w3.org/TR/css-backgrounds-3/#the-background-position */
            Vec2i retPos = new Vec2i();
            retPos.X = Solve_Object_Axis_Position(xPos, ObjectArea.Width, ObjectSize.Width);
            retPos.Y = Solve_Object_Axis_Position(yPos, ObjectArea.Height, ObjectSize.Height);
            return retPos;
        }
        /// <summary>
        /// The default sizing algorithm is a set of rules commonly used to find an object's concrete object size. 
        /// It resolves the simultaneous constraints presented by the object's intrinsic dimensions and either an unconstrained specified size or one consisting of only a definite width and/or height.
        /// </summary>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Size2D Default_Sizing_Algorithm(CssLayoutBox Box, CssValue Specified_Width, CssValue Specified_Height, int Default_Width, int Default_Height)
        {/* Docs: https://www.w3.org/TR/css3-images/#default-object-size */
            /* The default sizing algorithm is defined as follows: */

            /* If the specified size is a definite width and height, the concrete object size is given that width and height. */
            if (Specified_Width.IsDefinite && Specified_Height.IsDefinite)
            {
                return new Size2D((int)Specified_Width.Value, (int)Specified_Height.Value);
            }

            /* 
             * If the specified size is only a width or height (but not both) then the concrete object size is given that specified width or height. The other dimension is calculated as follows: 
             * 1) If the object has an intrinsic aspect ratio, the missing dimension of the concrete object size is calculated using the intrinsic aspect ratio and the present dimension.
             * 2) Otherwise, if the missing dimension is present in the object's intrinsic dimensions, the missing dimension is taken from the object's intrinsic dimensions.
             * 3) Otherwise, the missing dimension of the concrete object size is taken from the default object size.
             */
            if (Specified_Width.HasValue ^ Specified_Height.HasValue)
            {
                if (Box.Intrinsic_Ratio.HasValue)
                {/* 1) If the object has an intrinsic aspect ratio, the missing dimension of the concrete object size is calculated using the intrinsic aspect ratio and the present dimension. */
                    if (Specified_Width.HasValue)
                    {
                        return new Size2D((int)Specified_Width.Value, (int)((float)Specified_Width.Value / Box.Intrinsic_Ratio.Value));
                    }
                    else
                    {
                        return new Size2D((int)((float)Specified_Height.Value * Box.Intrinsic_Ratio.Value), (int)Specified_Height.Value);
                    }
                }

                /* 2) Otherwise, if the missing dimension is present in the object's intrinsic dimensions, the missing dimension is taken from the object's intrinsic dimensions. */
                if (!Specified_Width.HasValue && Box.Intrinsic_Width.HasValue)
                {
                    return new Size2D(Box.Intrinsic_Width.Value, (int)Specified_Height.Value);
                }
                else if (!Specified_Height.HasValue && Box.Intrinsic_Height.HasValue)
                {
                    return new Size2D((int)Specified_Width.Value, Box.Intrinsic_Height.Value);
                }

                /* 3) Otherwise, the missing dimension of the concrete object size is taken from the default object size. */
                if (Specified_Width.HasValue)
                {
                    return new Size2D((int)Specified_Width.Value, Default_Height);
                }
                else
                {
                    return new Size2D(Default_Width, (int)Specified_Height.Value);
                }
            }

            /* 
             * If the specified size has no constraints: 
             * 1) If the object has an intrinsic height or width, its size is resolved as if its intrinsic size were given as the specified size.
             * 2) Otherwise, its size is resolved as a contain constraint against the default object size.
             */
            if (Box.Intrinsic_Width.HasValue || Box.Intrinsic_Height.HasValue)
            {
                if (Box.Intrinsic_Width.HasValue && Box.Intrinsic_Height.HasValue)
                {
                    return new Size2D(Box.Intrinsic_Width.Value, Box.Intrinsic_Height.Value);
                }
            }
            else if(Box.Intrinsic_Width.HasValue ^ Box.Intrinsic_Height.HasValue)
            {
                /* Step 1 */
                if (Box.Intrinsic_Ratio.HasValue)
                {/* 1) If the object has an intrinsic aspect ratio, the missing dimension of the concrete object size is calculated using the intrinsic aspect ratio and the present dimension. */
                    if (Box.Intrinsic_Width.HasValue)
                    {
                        return new Size2D((int)Box.Intrinsic_Width.Value, (int)((float)Box.Intrinsic_Width.Value / Box.Intrinsic_Ratio.Value));
                    }
                    else
                    {
                        return new Size2D((int)((float)Box.Intrinsic_Height.Value * Box.Intrinsic_Ratio.Value), (int)Box.Intrinsic_Height.Value);
                    }
                }

                /* Step 3 */
                if (Box.Intrinsic_Width.HasValue)
                {
                    return new Size2D((int)Box.Intrinsic_Width.Value, Default_Height);
                }
                else
                {
                    return new Size2D(Default_Width, (int)Box.Intrinsic_Height.Value);
                }
            }

            /* 2) Otherwise, its size is resolved as a contain constraint against the default object size. */
            return CssAlgorithms.Contain_Constraint_Algorithm(Box, Default_Width, Default_Height);
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Size2D Contain_Constraint_Algorithm(CssLayoutBox Box, int Width, int Height)
        {/* Docs: https://www.w3.org/TR/css3-images/#contain-constraint */
            /* A contain constraint is resolved by setting the concrete object size to the largest rectangle that has the object's intrinsic aspect ratio and additionally has neither width nor height larger than the constraint rectangle's width and height, respectively. */
            float Ratio = (Box.Intrinsic_Ratio.HasValue ? Box.Intrinsic_Ratio.Value : 1f);
            /* Formula: Height = Width / Min(ratio, (Width/minHeight)) */
            if (Width > Height)
            {
                Height = (int)((float)Width / Math.Min(Ratio, (float)Width / (float)Height));
            }
            else
            {
                Width = (int)((float)Height * Math.Min(1f/Ratio, (float)Height / (float)Width));
            }

            return new Size2D(Width, Height);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Size2D Cover_Constraint_Algorithm(CssLayoutBox Box, int Width, int Height)
        {/* Docs: https://www.w3.org/TR/css3-images/#cover-constraint */
            /* A cover constraint is resolved by setting the concrete object size to the smallest rectangle that has the object's intrinsic aspect ratio and additionally has neither width nor height smaller than the constraint rectangle's width and height, respectively. */
            float Ratio = (Box.Intrinsic_Ratio.HasValue ? Box.Intrinsic_Ratio.Value : 1f);

            if (Width > Height)
            {/* Formula: Height = Width / Max(ratio, (Width/minHeight)) */
                Height = (int)((float)Width / Math.Max(Ratio, (float)Width / (float)Height));
                return new Size2D(Width, Height);
            }
            else
            {/* Formula: Width = Height * Max(1/ratio, (minHeight/Width)) */
                Width = (int)((float)Height * Math.Max(1f/Ratio, (float)Height / (float)Width));
                return new Size2D(Width, Height);
            }
        }
    }
}
