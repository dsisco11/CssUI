namespace CssUI.DOM
{
    /// <summary>
    /// Represents a single entry within a forms data
    /// </summary>
    public class FormDataEntryValue
    {
        #region Properties
        public readonly EFormDataValueType Type;
        public readonly string Name;
        public readonly dynamic Value;
        #endregion

        #region Constructor
        public FormDataEntryValue(EFormDataValueType type, string name, dynamic value)
        {
            Type = type;
            Name = name;
            Value = value;
        }
        #endregion
    }
}
