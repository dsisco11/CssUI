using System;

namespace CssUI.CSS
{
    /// <summary>
    /// Interface for styling property types which consist of multiple components
    /// EG: Position, and Color property types
    /// </summary>
    public interface ICssPropertyCollection
    {
        Action<CssProperty> onChange { get; }
        /// <summary>
        /// Returns TRUE if any values have the <see cref="StyleValueFlags.Depends"/> flag
        /// </summary>
        bool IsDependent { get; }
        /// <summary>
        /// Return TRUE if any values are set to <see cref="CssValue.Auto"/>
        /// </summary>
        bool IsAuto { get; }
    }
}
