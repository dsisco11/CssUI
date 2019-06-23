using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CssUI.CSS
{
    /// <summary>
    /// Represents a dummy style-property with a single set value and a name, used in CSS selector matching
    /// </summary>
    public class DummyProperty : NamedProperty
    {
        public readonly CSSValue Value = null;

        public DummyProperty(string Name, CSSValue value) : base(Name)
        {
            Value = value;
        }
        
    }
}
