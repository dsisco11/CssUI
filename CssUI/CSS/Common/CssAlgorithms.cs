using CssUI.CSS.BoxTree;
using System;
using System.Diagnostics.Contracts;
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
        /// <param name="Percent">Percentage along the axis to resolve</param>
        /// <param name="ObjectArea">Axis-Size of the area the object resides in</param>
        /// <param name="ObjectSize">Axis-Size of the object itsself</param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double Solve_Object_Axis_Position(double Percent, double ObjectArea, double ObjectSize)
        {/* https://www.w3.org/TR/css-backgrounds-3/#the-background-position */
            return ((Percent * ObjectArea) - (Percent * ObjectSize));
        }
        /// <summary>
        /// Applies (Along a single axis) the CSS specification formula for resolving object relative positions using the values given
        /// </summary>
        /// <param name="Pos">Axis-Position to resolve</param>
        /// <param name="ObjectArea">Axis-Size of the area the object resides in</param>
        /// <param name="ObjectSize">Axis-Size of the object itsself</param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double Solve_Object_Axis_Position(CssValue Pos, double ObjectArea, double ObjectSize)
        {/* https://www.w3.org/TR/css-backgrounds-3/#the-background-position */
            if (Pos is null) throw new ArgumentNullException(nameof(Pos));
            if (Pos.Type != ECssValueTypes.PERCENT) throw new ArgumentException("Pos must be a CSS percent-type value!");
            Contract.EndContractBlock();

            var P = Pos.AsDecimal();
            return ((P * ObjectArea) - (P * ObjectSize));
        }

        /// <summary>
        /// Applies the CSS specification formula for resolving object relative positions using the values given
        /// </summary>
        /// <param name="xPos">Positioning (along x-axis) to resolve </param>
        /// <param name="yPos">Positioning (along y-axis) to resolve </param>
        /// <param name="ObjectArea">Size of the area the object resides in</param>
        /// <param name="ObjectSize">Size of the object itsself</param>
        public static Point2f Solve_Object_Position(CssValue xPos, CssValue yPos, Rect2f ObjectArea, Rect2f ObjectSize)
        {/* https://www.w3.org/TR/css-backgrounds-3/#the-background-position */
            if (ObjectArea is null) throw new ArgumentNullException(nameof(ObjectArea));
            if (ObjectSize is null) throw new ArgumentNullException(nameof(ObjectSize));
            Contract.EndContractBlock();

            var retPos = new Point2f
            {
                X = Solve_Object_Axis_Position(xPos, ObjectArea.Width, ObjectSize.Width),
                Y = Solve_Object_Axis_Position(yPos, ObjectArea.Height, ObjectSize.Height)
            };
            return retPos;
        }
        /// <summary>
        /// The default sizing algorithm is a set of rules commonly used to find an object's concrete object size. 
        /// It resolves the simultaneous constraints presented by the object's intrinsic dimensions and either an unconstrained specified size or one consisting of only a definite width and/or height.
        /// </summary>
        public static Rect2f Default_Sizing_Algorithm(CssPrincipalBox Box, CssValue Specified_Width, CssValue Specified_Height, double Default_Width, double Default_Height)
        {/* Docs: https://www.w3.org/TR/css3-images/#default-object-size */
            if (Box is null) throw new ArgumentNullException(nameof(Box));
            if (Specified_Width is null) throw new ArgumentNullException(nameof(Specified_Width));
            if (Specified_Height is null) throw new ArgumentNullException(nameof(Specified_Height));
            Contract.EndContractBlock();

            /* The default sizing algorithm is defined as follows: */

            /* If the specified size is a definite width and height, the concrete object size is given that width and height. */
            if (Specified_Width.IsDefinite && Specified_Height.IsDefinite)
            {
                return new Rect2f(Specified_Width.AsDecimal(), Specified_Height.AsDecimal());
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
                        return new Rect2f(Specified_Width.AsDecimal(), (Specified_Width.AsDecimal() / Box.Intrinsic_Ratio.Value));
                    }
                    else
                    {
                        return new Rect2f((Specified_Height.AsDecimal() * Box.Intrinsic_Ratio.Value), Specified_Height.AsDecimal());
                    }
                }

                /* 2) Otherwise, if the missing dimension is present in the object's intrinsic dimensions, the missing dimension is taken from the object's intrinsic dimensions. */
                if (!Specified_Width.HasValue && Box.Intrinsic_Width.HasValue)
                {
                    return new Rect2f(Box.Intrinsic_Width.Value, Specified_Height.AsDecimal());
                }
                else if (!Specified_Height.HasValue && Box.Intrinsic_Height.HasValue)
                {
                    return new Rect2f(Specified_Width.AsDecimal(), Box.Intrinsic_Height.Value);
                }

                /* 3) Otherwise, the missing dimension of the concrete object size is taken from the default object size. */
                if (Specified_Width.HasValue)
                {
                    return new Rect2f(Specified_Width.AsDecimal(), Default_Height);
                }
                else
                {
                    return new Rect2f(Default_Width, Specified_Height.AsDecimal());
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
                    return new Rect2f(Box.Intrinsic_Width.Value, Box.Intrinsic_Height.Value);
                }
            }
            else if(Box.Intrinsic_Width.HasValue ^ Box.Intrinsic_Height.HasValue)
            {
                /* Step 1 */
                if (Box.Intrinsic_Ratio.HasValue)
                {/* 1) If the object has an intrinsic aspect ratio, the missing dimension of the concrete object size is calculated using the intrinsic aspect ratio and the present dimension. */
                    if (Box.Intrinsic_Width.HasValue)
                    {
                        return new Rect2f(Box.Intrinsic_Width.Value, (Box.Intrinsic_Width.Value / Box.Intrinsic_Ratio.Value));
                    }
                    else
                    {
                        return new Rect2f((Box.Intrinsic_Height.Value * Box.Intrinsic_Ratio.Value), Box.Intrinsic_Height.Value);
                    }
                }

                /* Step 3 */
                if (Box.Intrinsic_Width.HasValue)
                {
                    return new Rect2f(Box.Intrinsic_Width.Value, Default_Height);
                }
                else
                {
                    return new Rect2f(Default_Width, Box.Intrinsic_Height.Value);
                }
            }

            /* 2) Otherwise, its size is resolved as a contain constraint against the default object size. */
            return Contain_Constraint_Algorithm(Box, Default_Width, Default_Height);
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Rect2f Contain_Constraint_Algorithm(CssPrincipalBox Box, double Width, double Height)
        {/* Docs: https://www.w3.org/TR/css3-images/#contain-constraint */
            if (Box is null) throw new ArgumentNullException(nameof(Box));
            Contract.EndContractBlock();

            /* A contain constraint is resolved by setting the concrete object size to the largest rectangle that has the object's intrinsic aspect ratio and additionally has neither width nor height larger than the constraint rectangle's width and height, respectively. */

            /*
             * Essentially whats going on is that we are setting the largest axis of the box to match the same axis of the container 
             * And then we are calculating the opposite axis of the box using the aspect ratio while respecting its minimum bounds
             * 
             * Essentially whats happening is that we want to match the axis on which the container and box are closest.
             * This relationship is a bit obtuse at first glance so i'll simplify it down into a single point.
             * Heres 2 examples to drive that point home.
             * IF box W>H AND container W>H   then you match one axis, BUT
             * IF box W>H AND container W<H   then you match the opposite axis
             * SO the box wants to match its longest axis and the container wants to match it's own smallest.
             * 
             * The logical competition between the two indicates we need to ask a different question!
             * The question here is: "Which of these rectangles edges are going to touch first?"
             * 
             * So we want to know which axis is the longest for the inner rectangle and which is the shortest for the outer rectangle
             * For both rectangles the relationship between their sides is describes by their aspect ratio!
             * 
             * So by subtracting their ratios we can determine which axis is closest between them.
             * If (outter - inner) < 0 then their X axis is the closer point of collision
             * Otherwise its the Y axis!
             * 
             * Note: There's absolutely a way to do this without divisions which would be fewer cycles on the processor,
             * But any situation wherein this is making a difference, you probably shouldnt be using C# (an JIT intermediate language)
             * In the aforementioned case you should be using something closer to machine language like C++ or, heck, go for Assembly if your a masochist!
             */


            var innerRatio = (Box.Intrinsic_Ratio ?? 1.0);
            var outterRatio = Width / Height;

            if (outterRatio < innerRatio)// this is equivalent to (outter - inner) < 0
            {// Lock width, scale height
                Height = (int)(Width / innerRatio);
            }
            else
            {// Lock height, scale width
                Width = (int)(Height * innerRatio);
            }

            //var Ratio = (Box.Intrinsic_Ratio ?? 1f);
            ///* Formula: Height = Width / Min(ratio, (Width/minHeight)) */
            //if (Width > Height)
            //{
            //    Height = (Width / MathExt.Min(Ratio, Width / Height));
            //}
            //else
            //{
            //    Width = (Height * MathExt.Min(1f/Ratio, Height / Width));
            //}

            return new Rect2f(Width, Height);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Rect2f Cover_Constraint_Algorithm(CssPrincipalBox Box, double Width, double Height)
        {/* Docs: https://www.w3.org/TR/css3-images/#cover-constraint */
            if (Box is null) throw new ArgumentNullException(nameof(Box));
            Contract.EndContractBlock();

            /* A cover constraint is resolved by setting the concrete object size to the smallest rectangle that has the object's intrinsic aspect ratio and additionally has neither width nor height smaller than the constraint rectangle's width and height, respectively. */
            /*
             * THIS IS THE EXPLINATION OF THE PROCESS FOR THE "Contain" CONSTRAINT METHOD
             * THE "Cover" CONSTRAINT METHOD IS MERELY THE INVERSE OF THE OTHER ONE!
             * 
             * Essentially whats going on is that we are setting the largest axis of the box to match the same axis of the container 
             * And then we are calculating the opposite axis of the box using the aspect ratio while respecting its minimum bounds
             * 
             * Essentially whats happening is that we want to match the axis on which the container and box are closest.
             * This relationship is a bit obtuse at first glance so i'll simplify it down into a single point.
             * Heres 2 examples to drive that point home.
             * IF box W>H AND container W>H   then you match one axis, BUT
             * IF box W>H AND container W<H   then you match the opposite axis
             * SO the box wants to match its longest axis and the container wants to match it's own smallest.
             * 
             * The logical competition between the two indicates we need to ask a different question!
             * The question here is: "Which of these rectangles edges are going to touch FIRST?"
             * 
             * So we want to know which axis is the longest for the inner rectangle and which is the shortest for the outer rectangle
             * For both rectangles the relationship between their sides is describes by their aspect ratio!
             * 
             * So by subtracting their ratios we can determine which axis is closest between them.
             * If (outter - inner) < 0 then their X axis is the closer point of collision
             * Otherwise its the Y axis!
             * 
             * Note: There's absolutely a way to do this without divisions which would be fewer cycles on the processor,
             * But any situation wherein this is making a difference, you probably shouldnt be using C# (an JIT intermediate language)
             * In the aforementioned case you should be using something closer to machine language like C++ or, heck, go for Assembly if your a masochist!
             */

            var innerRatio = (Box.Intrinsic_Ratio ?? 1.0);
            var outterRatio = Width / Height;

            // HERES THE DIFFERENCE BETWEEN "Contain" AND "Cover"
            //             V
            if (innerRatio < outterRatio)
            {// Lock width, scale height
                Height = (int)(Width / innerRatio);
            }
            else
            {// Lock height, scale width
                Width = (int)(Height * innerRatio);
            }

            //var Ratio = (Box.Intrinsic_Ratio ?? 1f);

            //if (Width > Height)
            //{/* Formula: Height = Width / Max(ratio, (Width/minHeight)) */
            //    Height = (Width / MathExt.Max(Ratio, Width / Height));
            //    return new Rect2f(Width, Height);
            //}
            //else
            //{/* Formula: Width = Height * Max(1/ratio, (minHeight/Width)) */
            //    Width = (Height * MathExt.Max(1f/Ratio, Height / Width));
            //    return new Rect2f(Width, Height);
            //}

            return new Rect2f(Width, Height);
        }
    }
}
