using Xunit;
using CssUI.DOM;
using System;
using System.Collections.Generic;
using System.Text;
using CssUI.DOM.Enums;

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
            string AttrName = "test";
            AttributeDefinition def;

            //
            def = new AttributeDefinition(AttrName, EAttributeType.Boolean);
            def.CheckAndThrow("1");
            def.CheckAndThrow("true");
            //
            def = new AttributeDefinition(AttrName, EAttributeType.Color);
            def.CheckAndThrow("#FFFFFF");
            Assert.Throws<Exceptions.DomSyntaxError>(() => def.CheckAndThrow("ABC"));
            Assert.Throws<Exceptions.DomSyntaxError>(() => def.CheckAndThrow("#FFFFFFFF"));
            //
            def = new AttributeDefinition(AttrName, EAttributeType.Date);
            def.CheckAndThrow("1992-07-03");
            Assert.Throws<Exceptions.DomSyntaxError>(() => def.CheckAndThrow("ABC"));
            Assert.Throws<Exceptions.DomSyntaxError>(() => def.CheckAndThrow("0"));
            Assert.Throws<Exceptions.DomSyntaxError>(() => def.CheckAndThrow("0-"));
            Assert.Throws<Exceptions.DomSyntaxError>(() => def.CheckAndThrow("1992-00"));
            Assert.Throws<Exceptions.DomSyntaxError>(() => def.CheckAndThrow("1992-13"));
            Assert.Throws<Exceptions.DomSyntaxError>(() => def.CheckAndThrow("1992-07-00"));
            Assert.Throws<Exceptions.DomSyntaxError>(() => def.CheckAndThrow("1992-07-69"));
            //
            def = new AttributeDefinition(AttrName, EAttributeType.Time);
            def.CheckAndThrow("12:59");
            def.CheckAndThrow("12:59:59.999");
            Assert.Throws<Exceptions.DomSyntaxError>(() => def.CheckAndThrow("ABC"));
            Assert.Throws<Exceptions.DomSyntaxError>(() => def.CheckAndThrow("-1"));
            Assert.Throws<Exceptions.DomSyntaxError>(() => def.CheckAndThrow("13"));
            Assert.Throws<Exceptions.DomSyntaxError>(() => def.CheckAndThrow("12:60"));
            Assert.Throws<Exceptions.DomSyntaxError>(() => def.CheckAndThrow("12:59:60"));
            Assert.Throws<Exceptions.DomSyntaxError>(() => def.CheckAndThrow("12:59:60.9"));
            //
            def = new AttributeDefinition(AttrName, EAttributeType.Duration);
            def.CheckAndThrow("0.0s");
            Assert.Throws<Exceptions.DomSyntaxError>(() => def.CheckAndThrow("ABC"));
            Assert.Throws<Exceptions.DomSyntaxError>(() => def.CheckAndThrow("0%"));
            //
            def = new AttributeDefinition(AttrName, EAttributeType.Enumerated, null, null, EAttributeFlags.None, new string[] { "one", "two", "three" });
            def.CheckAndThrow("two");
            Assert.Throws<Exceptions.DomSyntaxError>(() => def.CheckAndThrow("ABC"));
            //
            def = new AttributeDefinition(AttrName, EAttributeType.Integer);
            def.CheckAndThrow("0");
            Assert.Throws<Exceptions.DomSyntaxError>(() => def.CheckAndThrow("ABC"));
            Assert.Throws<Exceptions.DomSyntaxError>(() => def.CheckAndThrow(".0"));
            //
            def = new AttributeDefinition(AttrName, EAttributeType.NonNegative_Integer);
            def.CheckAndThrow("0");
            Assert.Throws<Exceptions.DomSyntaxError>(() => def.CheckAndThrow("ABC"));
            Assert.Throws<Exceptions.DomSyntaxError>(() => def.CheckAndThrow("-1"));
            //
            def = new AttributeDefinition(AttrName, EAttributeType.FloatingPoint);
            def.CheckAndThrow("-1.0");
            def.CheckAndThrow("0.0");
            Assert.Throws<Exceptions.DomSyntaxError>(() => def.CheckAndThrow("ABC"));
            //
            def = new AttributeDefinition(AttrName, EAttributeType.Length);
            def.CheckAndThrow("0");
            Assert.Throws<Exceptions.DomSyntaxError>(() => def.CheckAndThrow("ABC"));
            Assert.Throws<Exceptions.DomSyntaxError>(() => def.CheckAndThrow("-1.0"));
            //
            def = new AttributeDefinition(AttrName, EAttributeType.NonZero_Length);
            def.CheckAndThrow("1");
            Assert.Throws<Exceptions.DomSyntaxError>(() => def.CheckAndThrow("ABC"));
            Assert.Throws<Exceptions.DomSyntaxError>(() => def.CheckAndThrow("-1.0"));
            Assert.Throws<Exceptions.DomSyntaxError>(() => def.CheckAndThrow("0.0"));
            //
            def = new AttributeDefinition(AttrName, EAttributeType.Percentage);
            def.CheckAndThrow("1.0%");
            Assert.Throws<Exceptions.DomSyntaxError>(() => def.CheckAndThrow("ABC"));
            //
            def = new AttributeDefinition(AttrName, EAttributeType.NonZero_Percentage);
            def.CheckAndThrow("1.0%");
            Assert.Throws<Exceptions.DomSyntaxError>(() => def.CheckAndThrow("ABC"));
            Assert.Throws<Exceptions.DomSyntaxError>(() => def.CheckAndThrow("-1%"));

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