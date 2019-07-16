using CssUI.Internal;
using System;

namespace CssUI.CSS
{
    /// <summary>
    /// Stores data for a Css property about what kinds of values it can be set to.
    /// </summary>
    public class CssPropertyOptions
    {
        #region Options
        /// <summary>
        /// Whether or not the property can be set to <see cref="CssValue.Auto"/>
        /// </summary>
        public bool AllowAuto = true;
        /// <summary>
        /// Whether or not the property can be set to <see cref="CssValue.Inherit"/>
        /// </summary>
        public bool AllowInherited = true;
        /// <summary>
        /// Whether or not the property can be set to percentages
        /// </summary>
        public bool AllowPercentage = true;
        /// <summary>
        /// Whether or not both Implicit and Explicit values start out as <see cref="CssValue.Null"/>
        /// </summary>
        public bool UnsetAll = false;
        #endregion

        #region Constructors
        public CssPropertyOptions()
        {
        }

        public CssPropertyOptions(CssPropertyOptions o)
        {
            this.AllowAuto = o.AllowAuto;
            this.AllowInherited = o.AllowInherited;
            this.AllowPercentage = o.AllowPercentage;
            this.UnsetAll = o.UnsetAll;
        }
        #endregion


        /// <summary>
        /// Returns whether the specified value is valid according to the currently set options
        /// </summary>
        /// <param name="Value"></param>
        /// <returns></returns>
        public bool IsValid(CssValue Value)
        {
            switch (Value.Type)
            {
                case ECssValueType.AUTO:
                    return AllowAuto;
                case ECssValueType.INHERIT:
                    return AllowInherited;
                case ECssValueType.PERCENT:
                    return AllowPercentage;
                default:
                    return true;
            }
        }

        /// <summary>
        /// Throws an exception if any of the given values are invalid according to the currently set options
        /// </summary>
        /// <param name="Value"></param>
        /// <returns></returns>
        public void CheckAndThrow(ICssProperty Owner, CssValueList Values)
        {
            foreach (CssValue Value in Values)
            {
                CheckAndThrow(Owner, Value);
            }
        }

        /// <summary>
        /// Throws an exception if the value is invalid according to the currently set options
        /// </summary>
        /// <param name="Value"></param>
        /// <returns></returns>
        public void CheckAndThrow(ICssProperty Owner, CssValue Value)
        {
            switch (Value.Type)
            {
                case ECssValueType.AUTO:
                    if (!AllowAuto) throw new Exception(string.Concat("The property(", Owner.CssName,") cannot be set to AUTO!"));
                    break;
                case ECssValueType.INHERIT:
                    if (!AllowInherited) throw new Exception(string.Concat("The property(", Owner.CssName, ") cannot be set to INHERITED!"));
                    break;
                case ECssValueType.PERCENT:
                    if (!AllowPercentage) throw new Exception(string.Concat("The property(", Owner.CssName, ") cannot be set to Percentages!"));
                    break;
                default:
                    break;
            }
        }
    }
}
