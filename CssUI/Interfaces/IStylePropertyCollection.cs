﻿using System;

namespace CssUI.CSS
{
    /// <summary>
    /// Interface for styling property types which consist of multiple components
    /// EG: Position, and Color property types
    /// </summary>
    public interface IStylePropertyCollection
    {
        Action<NamedProperty> onChange { get; }
        /// <summary>
        /// Returns TRUE if any values have the <see cref="StyleValueFlags.Depends"/> flag
        /// </summary>
        bool IsDependent { get; }
        /// <summary>
        /// Return TRUE if any values are set to <see cref="CSSValue.Auto"/>
        /// </summary>
        bool IsAuto { get; }
    }
}