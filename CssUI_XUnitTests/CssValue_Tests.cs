using CssUI.CSS;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace CssUI_XUnitTests
{
    public class CssValue_Tests
    {
        [Fact]
        public void CSS_Parsing()
        {
            CssValue Value;


            /* Ensure that null values are handled correctly */

            Value = CssValue.From_CSS("");
            Assert.True(!Value.HasValue);
            Assert.Equal(ECssValueType.NULL, Value.Type);

            Value = CssValue.From_CSS(" ");
            Assert.True(!Value.HasValue);
            Assert.Equal(ECssValueType.NULL, Value.Type);


            /* Test dimension parsing */

            Value = CssValue.From_CSS("100px");
            Assert.True(Value.HasValue);
            Assert.True(Value.Unit == EUnit.PX);
            Assert.True((int)Value.Value == 100);
            Assert.Equal(ECssValueType.DIMENSION, Value.Type);

            Value = CssValue.From_CSS("1ch");
            Assert.True(Value.HasValue);
            Assert.True(Value.Unit == EUnit.CH);
            Assert.True((int)Value.Value == 1);
            Assert.Equal(ECssValueType.DIMENSION, Value.Type);

            Value = CssValue.From_CSS("1em");
            Assert.True(Value.HasValue);
            Assert.True(Value.Unit == EUnit.EM);
            Assert.True((int)Value.Value == 1);
            Assert.Equal(ECssValueType.DIMENSION, Value.Type);

            Value = CssValue.From_CSS("96dpi");
            Assert.True(Value.HasValue);
            Assert.True(Value.Unit == EUnit.DPI);
            Assert.True((int)Value.Value == 96);
            Assert.Equal(ECssValueType.RESOLUTION, Value.Type);

            Value = CssValue.From_CSS("1dppx");
            Assert.True(Value.HasValue);
            Assert.True(Value.Unit == EUnit.DPPX);
            Assert.True((int)Value.Value == 1);
            Assert.Equal(ECssValueType.RESOLUTION, Value.Type);


            /* Check some global keywords to make sure those are being converted to the correct type flag */

            Value = CssValue.From_CSS("auto");
            Assert.True(Value.HasValue);
            Assert.True(Value.IsAuto);
            Assert.Equal(ECssValueType.AUTO, Value.Type);

            Value = CssValue.From_CSS("none");
            Assert.True(Value.HasValue);
            Assert.Equal(ECssValueType.NONE, Value.Type);

            Value = CssValue.From_CSS("initial");
            Assert.True(Value.HasValue);
            Assert.Equal(ECssValueType.INITIAL, Value.Type);

            Value = CssValue.From_CSS("inherit");
            Assert.True(Value.HasValue);
            Assert.Equal(ECssValueType.INHERIT, Value.Type);
            
            Value = CssValue.From_CSS("unset");
            Assert.True(Value.HasValue);
            Assert.Equal(ECssValueType.UNSET, Value.Type);
            
            Value = CssValue.From_CSS("default");
            Assert.True(Value.HasValue);
            Assert.Equal(ECssValueType.DEFAULT, Value.Type);


            /* Check custom keywords */

            Value = CssValue.From_CSS("hidden");
            Assert.True(Value.HasValue);
            Assert.Equal(ECssValueType.KEYWORD, Value.Type);
            Assert.Equal("hidden", Value.Value);
            
            Value = CssValue.From_CSS("sans-serif");
            Assert.True(Value.HasValue);
            Assert.Equal(ECssValueType.KEYWORD, Value.Type);
            Assert.Equal("sans-serif", Value.Value);


            /* Check strings */

            Value = CssValue.From_CSS("\"Hello World!\"");
            Assert.True(Value.HasValue);
            Assert.Equal(ECssValueType.STRING, Value.Type);
            Assert.Equal("Hello World!", Value.Value);


            /* Check percentages */

            Value = CssValue.From_CSS("100%");
            Assert.True(Value.HasValue);
            Assert.Equal(ECssValueType.PERCENT, Value.Type);
            Assert.Equal(100, Value.Value);

            Value = CssValue.From_CSS("1%");
            Assert.True(Value.HasValue);
            Assert.Equal(ECssValueType.PERCENT, Value.Type);
            Assert.Equal(1, Value.Value);


            /* Check numbers */

            Value = CssValue.From_CSS("100.5");
            Assert.True(Value.HasValue);
            Assert.Equal(ECssValueType.NUMBER, Value.Type);
            Assert.Equal(100.5, Value.Value);

            Value = CssValue.From_CSS("1.25");
            Assert.True(Value.HasValue);
            Assert.Equal(ECssValueType.NUMBER, Value.Type);
            Assert.Equal(1.25, Value.Value);


            /* Check integers */

            Value = CssValue.From_CSS("100");
            Assert.True(Value.HasValue);
            Assert.Equal(ECssValueType.INTEGER, Value.Type);
            Assert.Equal(100, Value.Value);

            Value = CssValue.From_CSS("1");
            Assert.True(Value.HasValue);
            Assert.Equal(ECssValueType.INTEGER, Value.Type);
            Assert.Equal(1, Value.Value);


            /* Check functions */

            Value = CssValue.From_CSS("calc(1 / 100 + 5)");
            Assert.True(Value.HasValue);
            Assert.Equal(ECssValueType.FUNCTION, Value.Type);
            Assert.Equal(100, Value.Value);


            /* Check positions */

            Value = CssValue.From_CSS(" left 50% ");
            Assert.True(Value.HasValue);
            Assert.Equal(ECssValueType.POSITION, Value.Type);
            Assert.Equal(100, Value.Value);

        }

    }
}
