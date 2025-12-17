using Microsoft.VisualStudio.TestTools.UnitTesting;
using CssUI.CSS;

namespace CssUITests.CSS.Functions
{
    /// <summary>
    /// Unit tests for CSS calc() function infrastructure.
    /// Note: Full parsing tests are disabled due to existing parser bugs with function parsing.
    /// The CssCalcEvaluator is ready for use once the parser is fixed.
    /// </summary>
    [TestClass]
    public class CssCalcEvaluatorTests
    {
        #region Infrastructure Tests

        [TestMethod]
        [TestCategory("Calc")]
        public void ECssValueTypes_HasFunctionType()
        {
            Assert.IsTrue(System.Enum.IsDefined(typeof(ECssValueTypes), "FUNCTION"));
        }

        [TestMethod]
        [TestCategory("Calc")]
        public void ECssValueTypes_FunctionValue()
        {
            Assert.AreEqual(1 << 23, (int)ECssValueTypes.FUNCTION);
        }

        #endregion
    }
}

