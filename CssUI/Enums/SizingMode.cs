using System;

namespace KeyPic.OpenGL
{
    /// <summary>
    /// Describes how a control 
    /// </summary>
    public enum SizingConstraint
    {
        /// <summary>
        /// Maintain a set size.
        /// </summary>
        STATIC = 0,
        /// <summary>
        /// Allow size growth if child controls exceed the current bounds
        /// Any user specified size is treated as the MINIMUM size instead of the constant one.
        /// </summary>
        GROW,
        /// <summary>
        /// Allow the control to shrink so it only takes up as much space as it needs.
        /// The User-Specified size is treated as the MAXIMUM size instead of the constant one.
        /// </summary>
        SHRINK
    }
}