using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CssUI
{
    /// <summary>
    /// Defines the basic functionality which all properties defined in an <see cref="StyleRuleData"/> instance should have
    /// </summary>
    public interface IElementProperty
    {
        /// <summary>
        /// Overwrites the explicit values of this instance with any set explicit values of another
        /// </summary>
        bool Cascade(IElementProperty obj);
        /// <summary>
        /// Overwrites the explicit values of this instance with any set explicit values from another if they are different
        /// </summary>
        bool Overwrite(IElementProperty obj);
    }
}
