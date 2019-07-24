using System;

namespace CssUI.DOM
{
    /// <summary>
    /// Represents a single entry within a forms data
    /// </summary>
    public class FormDataValue
    {
        #region Properties
        public readonly EFormDataValueType Type;
        public readonly dynamic Value;
        #endregion

        #region Constructor
        public FormDataValue(EFormDataValueType type, dynamic value)
        {
            Type = type;
            Value = value;
        }
        #endregion
    }
}
