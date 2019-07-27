using CssUI.DOM;
using System;
using System.Collections.Generic;
using Xunit;

namespace CssUI_XUnitTests
{
    public class DOMAttributeDefinition_Tests
    {
        [Fact]
        public void Check_All_Attributes_Defined()
        {
            var attribNames = Enum.GetValues(typeof(EAttributeName));

            foreach (EAttributeName Name in attribNames)
            {
                AttributeDefinition def = AttributeDefinition.Lookup(Name);
                Assert.NotNull(def);
            }
        }

    }
}
