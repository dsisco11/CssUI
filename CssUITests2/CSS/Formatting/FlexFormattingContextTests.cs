using Microsoft.VisualStudio.TestTools.UnitTesting;
using CssUI.CSS;
using CssUI.CSS.Formatting;
using CssUI.CSS.BoxTree;

namespace CssUITests.CSS.Formatting
{
    /// <summary>
    /// Unit tests for FlexFormattingContext.
    /// Tests the CSS Flexible Box Layout algorithm (https://www.w3.org/TR/css-flexbox-1/).
    /// </summary>
    [TestClass]
    public class FlexFormattingContextTests
    {
        #region Basic Tests

        [TestMethod]
        [TestCategory("Flex")]
        public void FlexFormattingContext_CanBeInstantiated()
        {
            var context = new FlexFormattingContext();
            Assert.IsNotNull(context);
        }

        [TestMethod]
        [TestCategory("Flex")]
        public void FlexFormattingContext_ImplementsIFormattingContext()
        {
            var context = new FlexFormattingContext();
            Assert.IsInstanceOfType(context, typeof(IFormattingContext));
        }

        [TestMethod]
        [TestCategory("Flex")]
        public void FlexFormattingContext_Flow_HandlesNullGracefully()
        {
            var context = new FlexFormattingContext();
            Assert.ThrowsException<System.ArgumentNullException>(() => context.Flow(null));
        }

        #endregion

        #region Property Tests

        [TestMethod]
        [TestCategory("Flex")]
        public void EFlexDirection_HasExpectedValues()
        {
            Assert.AreEqual(0, (int)EFlexDirection.Row);
            Assert.AreEqual(1, (int)EFlexDirection.RowReverse);
            Assert.AreEqual(2, (int)EFlexDirection.Column);
            Assert.AreEqual(3, (int)EFlexDirection.ColumnReverse);
        }

        [TestMethod]
        [TestCategory("Flex")]
        public void EFlexWrap_HasExpectedValues()
        {
            Assert.AreEqual(0, (int)EFlexWrap.NoWrap);
            Assert.AreEqual(1, (int)EFlexWrap.Wrap);
            Assert.AreEqual(2, (int)EFlexWrap.WrapReverse);
        }

        [TestMethod]
        [TestCategory("Flex")]
        public void EJustifyContent_HasFlexValues()
        {
            // Verify flex-specific values exist
            Assert.IsTrue(System.Enum.IsDefined(typeof(EJustifyContent), "FlexStart"));
            Assert.IsTrue(System.Enum.IsDefined(typeof(EJustifyContent), "FlexEnd"));
            Assert.IsTrue(System.Enum.IsDefined(typeof(EJustifyContent), "Center"));
            Assert.IsTrue(System.Enum.IsDefined(typeof(EJustifyContent), "SpaceBetween"));
            Assert.IsTrue(System.Enum.IsDefined(typeof(EJustifyContent), "SpaceAround"));
            Assert.IsTrue(System.Enum.IsDefined(typeof(EJustifyContent), "SpaceEvenly"));
        }

        [TestMethod]
        [TestCategory("Flex")]
        public void EAlignItems_HasFlexValues()
        {
            // Verify flex-specific values exist
            Assert.IsTrue(System.Enum.IsDefined(typeof(EAlignItems), "FlexStart"));
            Assert.IsTrue(System.Enum.IsDefined(typeof(EAlignItems), "FlexEnd"));
            Assert.IsTrue(System.Enum.IsDefined(typeof(EAlignItems), "Center"));
            Assert.IsTrue(System.Enum.IsDefined(typeof(EAlignItems), "Stretch"));
            Assert.IsTrue(System.Enum.IsDefined(typeof(EAlignItems), "Baseline"));
        }

        #endregion

        #region Display Mode Integration Tests

        [TestMethod]
        [TestCategory("Flex")]
        public void EDisplayMode_HasFlexValues()
        {
            // Verify flex display modes exist
            Assert.IsTrue(System.Enum.IsDefined(typeof(EDisplayMode), "FLEX"));
            Assert.IsTrue(System.Enum.IsDefined(typeof(EDisplayMode), "INLINE_FLEX"));
        }

        [TestMethod]
        [TestCategory("Flex")]
        public void EInnerDisplayType_HasFlexValue()
        {
            Assert.IsTrue(System.Enum.IsDefined(typeof(CssUI.CSS.Enums.EInnerDisplayType), "Flex"));
        }

        #endregion

        #region Gap Property Tests

        [TestMethod]
        [TestCategory("Flex")]
        [TestCategory("Gap")]
        public void ECssPropertyID_HasGapProperties()
        {
            Assert.IsTrue(System.Enum.IsDefined(typeof(ECssPropertyID), "RowGap"));
            Assert.IsTrue(System.Enum.IsDefined(typeof(ECssPropertyID), "ColumnGap"));
        }

        #endregion
    }
}
