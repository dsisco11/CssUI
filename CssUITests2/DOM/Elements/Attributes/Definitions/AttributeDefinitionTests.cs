using Xunit;
using CssUI.DOM;
using System;
using System.Collections.Generic;
using System.Text;

namespace CssUI.DOM.Tests
{
    public class AttributeDefinitionTests
    {
        [Fact()]
        public void AttributeDefinitionTest()
        {
            throw new NotImplementedException();
        }

        [Fact()]
        public void ParseTest()
        {
            throw new NotImplementedException();
        }

        [Fact()]
        public void CheckAndThrowTest()
        {
            throw new NotImplementedException();
        }

        [Fact()]
        public void LookupTest()
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