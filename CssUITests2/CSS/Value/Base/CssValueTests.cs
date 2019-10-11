using Xunit;
using CssUI.CSS;
using System;
using System.Collections.Generic;
using System.Text;

namespace CssUI.CSS.Tests
{
    public class CssValueTests
    {
        [Fact()]
        public void CloneTest()
        {
            throw new NotImplementedException();
        }

        [Fact()]
        public void FromTest()
        {
            throw new NotImplementedException();
        }

        [Fact()]
        public void FromTest1()
        {
            throw new NotImplementedException();
        }

        [Fact()]
        public void FromTest2()
        {
            throw new NotImplementedException();
        }

        [Fact()]
        public void FromTest3()
        {
            throw new NotImplementedException();
        }

        [Fact()]
        public void FromTest4()
        {
            throw new NotImplementedException();
        }

        [Fact()]
        public void From_PercentTest()
        {
            throw new NotImplementedException();
        }

        [Fact()]
        public void FromTest5()
        {
            throw new NotImplementedException();
        }

        [Fact()]
        public void FromTest6()
        {
            throw new NotImplementedException();
        }

        [Fact()]
        public void FromTest7()
        {
            throw new NotImplementedException();
        }

        [Fact()]
        public void FromTest8()
        {
            throw new NotImplementedException();
        }

        [Fact()]
        public void From_StringTest()
        {
            throw new NotImplementedException();
        }

        [Fact()]
        public void From_CSSTest()
        {
            CssValue Value;


            /* Ensure that null values are handled correctly */

            Value = CssValue.From_CSS("");
            Assert.True(!Value.HasValue);
            Assert.Equal(ECssValueTypes.NULL, Value.Type);

            Value = CssValue.From_CSS(" ");
            Assert.True(!Value.HasValue);
            Assert.Equal(ECssValueTypes.NULL, Value.Type);


            /* Test dimension parsing */

            Value = CssValue.From_CSS("100px");
            Assert.True(Value.HasValue);
            Assert.Equal(ECssUnit.PX, Value.Unit);
            Assert.Equal(100, Value.AsInteger());
            Assert.Equal(ECssValueTypes.DIMENSION, Value.Type);

            Value = CssValue.From_CSS("1ch");
            Assert.True(Value.HasValue);
            Assert.Equal(ECssUnit.CH, Value.Unit);
            Assert.Equal(1, Value.AsInteger());
            Assert.Equal(ECssValueTypes.DIMENSION, Value.Type);

            Value = CssValue.From_CSS("1em");
            Assert.True(Value.HasValue);
            Assert.Equal(ECssUnit.EM, Value.Unit);
            Assert.Equal(1, Value.AsInteger());
            Assert.Equal(ECssValueTypes.DIMENSION, Value.Type);

            Value = CssValue.From_CSS("96dpi");
            Assert.True(Value.HasValue);
            Assert.Equal(ECssUnit.DPI, Value.Unit);
            Assert.Equal(96, Value.AsInteger());
            Assert.Equal(ECssValueTypes.RESOLUTION, Value.Type);

            Value = CssValue.From_CSS("1dppx");
            Assert.True(Value.HasValue);
            Assert.Equal(ECssUnit.DPPX, Value.Unit);
            Assert.Equal(1, Value.AsInteger());
            Assert.Equal(ECssValueTypes.RESOLUTION, Value.Type);


            /* Check some global keywords to make sure those are being converted to the correct type flag */

            Value = CssValue.From_CSS("auto");
            Assert.True(Value.HasValue);
            Assert.True(Value.IsAuto);
            Assert.Equal(ECssValueTypes.AUTO, Value.Type);

            Value = CssValue.From_CSS("none");
            Assert.True(Value.HasValue);
            Assert.Equal(ECssValueTypes.NONE, Value.Type);

            Value = CssValue.From_CSS("initial");
            Assert.True(Value.HasValue);
            Assert.Equal(ECssValueTypes.INITIAL, Value.Type);

            Value = CssValue.From_CSS("inherit");
            Assert.True(Value.HasValue);
            Assert.Equal(ECssValueTypes.INHERIT, Value.Type);

            Value = CssValue.From_CSS("unset");
            Assert.True(Value.HasValue);
            Assert.Equal(ECssValueTypes.UNSET, Value.Type);

            Value = CssValue.From_CSS("default");
            Assert.True(Value.HasValue);
            Assert.Equal(ECssValueTypes.DEFAULT, Value.Type);


            /* Check custom keywords */

            Value = CssValue.From_CSS("hidden");
            Assert.True(Value.HasValue);
            Assert.Equal(ECssValueTypes.KEYWORD, Value.Type);
            Assert.Equal("hidden", Value.AsString());

            Value = CssValue.From_CSS("sans-serif");
            Assert.True(Value.HasValue);
            Assert.Equal(ECssValueTypes.KEYWORD, Value.Type);
            Assert.Equal(EGenericFontFamily.SansSerif, Value.AsEnum<EGenericFontFamily>());
            Assert.Equal("sans-serif", Value.AsString());


            /* Check strings */

            Value = CssValue.From_CSS("\"Hello World!\"");
            Assert.True(Value.HasValue);
            Assert.Equal(ECssValueTypes.STRING, Value.Type);
            Assert.Equal("Hello World!", Value.AsString());


            /* Check percentages */

            Value = CssValue.From_CSS("100%");
            Assert.True(Value.HasValue);
            Assert.Equal(ECssValueTypes.PERCENT, Value.Type);
            Assert.Equal(100, Value.AsDecimal());

            Value = CssValue.From_CSS("1%");
            Assert.True(Value.HasValue);
            Assert.Equal(ECssValueTypes.PERCENT, Value.Type);
            Assert.Equal(1, Value.AsDecimal());


            /* Check numbers */

            Value = CssValue.From_CSS("100.5");
            Assert.True(Value.HasValue);
            Assert.Equal(ECssValueTypes.NUMBER, Value.Type);
            Assert.Equal(100.5, Value.AsDecimal());

            Value = CssValue.From_CSS("1.25");
            Assert.True(Value.HasValue);
            Assert.Equal(ECssValueTypes.NUMBER, Value.Type);
            Assert.Equal(1.25, Value.AsDecimal());


            /* Check integers */

            Value = CssValue.From_CSS("100");
            Assert.True(Value.HasValue);
            Assert.Equal(ECssValueTypes.INTEGER, Value.Type);
            Assert.Equal(100, Value.AsInteger());

            Value = CssValue.From_CSS("1");
            Assert.True(Value.HasValue);
            Assert.Equal(ECssValueTypes.INTEGER, Value.Type);
            Assert.Equal(1, Value.AsInteger());


            /* Check functions */

            Value = CssValue.From_CSS("calc(1 / 100 + 5)");
            Assert.True(Value.HasValue);
            Assert.Equal(ECssValueTypes.FUNCTION, Value.Type);
            Assert.Equal(100.0, Value.AsDecimal());


            /* Check positions */

            Value = CssValue.From_CSS(" left 50% ");
            Assert.True(Value.HasValue);
            Assert.Equal(ECssValueTypes.POSITION, Value.Type);
            Assert.Equal(100.0, Value.AsDecimal());

        }

        [Fact()]
        public void Has_FlagsTest()
        {
            throw new NotImplementedException();
        }

        [Fact()]
        public void ResolveTest()
        {
            throw new NotImplementedException();
        }

        [Fact()]
        public void ResolveTest1()
        {
            throw new NotImplementedException();
        }

        [Fact()]
        public void Resolve_Or_DefaultTest()
        {
            throw new NotImplementedException();
        }

        [Fact()]
        public void ResolveTest2()
        {
            throw new NotImplementedException();
        }

        [Fact()]
        public void Resolve_Or_DefaultTest1()
        {
            throw new NotImplementedException();
        }

        [Fact()]
        public void ResolveTest3()
        {
            throw new NotImplementedException();
        }

        [Fact()]
        public void Resolve_Or_DefaultTest2()
        {
            throw new NotImplementedException();
        }

        [Fact()]
        public void ResolveTest4()
        {
            throw new NotImplementedException();
        }

        [Fact()]
        public void Resolve_Or_DefaultTest3()
        {
            throw new NotImplementedException();
        }

        [Fact()]
        public void Resolve_Or_DefaultTest4()
        {
            throw new NotImplementedException();
        }

        [Fact()]
        public void Resolve_Or_DefaultTest5()
        {
            throw new NotImplementedException();
        }

        [Fact()]
        public void Resolve_Or_DefaultTest6()
        {
            throw new NotImplementedException();
        }

        [Fact()]
        public void ResolveTest5()
        {
            throw new NotImplementedException();
        }

        [Fact()]
        public void ResolveTest6()
        {
            throw new NotImplementedException();
        }

        [Fact()]
        public void Resolve_Or_DefaultTest7()
        {
            throw new NotImplementedException();
        }

        [Fact()]
        public void ResolveTest7()
        {
            throw new NotImplementedException();
        }

        [Fact()]
        public void Resolve_Or_DefaultTest8()
        {
            throw new NotImplementedException();
        }

        [Fact()]
        public void Resolve_Or_DefaultTest9()
        {
            throw new NotImplementedException();
        }

        [Fact()]
        public void ResolveTest8()
        {
            throw new NotImplementedException();
        }

        [Fact()]
        public void Resolve_Or_DefaultTest10()
        {
            throw new NotImplementedException();
        }

        [Fact()]
        public void Resolve_Or_DefaultTest11()
        {
            throw new NotImplementedException();
        }

        [Fact()]
        public void Resolve_Or_DefaultTest12()
        {
            throw new NotImplementedException();
        }

        [Fact()]
        public void Resolve_Or_DefaultTest13()
        {
            throw new NotImplementedException();
        }

        [Fact()]
        public void AsEnumTest()
        {
            throw new NotImplementedException();
        }

        [Fact()]
        public void AsPositionTest()
        {
            throw new NotImplementedException();
        }

        [Fact()]
        public void AsColorTest()
        {
            throw new NotImplementedException();
        }

        [Fact()]
        public void AsCollectionTest()
        {
            throw new NotImplementedException();
        }

        [Fact()]
        public void AsIntegerTest()
        {
            throw new NotImplementedException();
        }

        [Fact()]
        public void AsIntegerNTest()
        {
            throw new NotImplementedException();
        }

        [Fact()]
        public void AsDecimalTest()
        {
            throw new NotImplementedException();
        }

        [Fact()]
        public void AsDecimalNTest()
        {
            throw new NotImplementedException();
        }

        [Fact()]
        public void AsStringTest()
        {
            throw new NotImplementedException();
        }

        [Fact()]
        public void EqualsTest()
        {
            throw new NotImplementedException();
        }

        [Fact()]
        public void GetHashCodeTest()
        {
            throw new NotImplementedException();
        }

        [Fact()]
        public void ToStringTest()
        {
            throw new NotImplementedException();
        }

        [Fact()]
        public void SerializeTest()
        {
            throw new NotImplementedException();
        }
    }
}