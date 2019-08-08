using System;

namespace CssUI.HTML
{
    public class MetaElementAttribute : Attribute
    {
        #region Properties
        public readonly string Name;
        #endregion

        #region Constructor
        public MetaElementAttribute(string name)
        {
            Name = name;
        }
        #endregion
    }
}
