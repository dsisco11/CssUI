using Microsoft.VisualStudio.TestTools.UnitTesting;
using CssUI.CSS;
using CssUI.CSS.Formatting;
using CssUI.CSS.BoxTree;

namespace CssUITests.CSS.Formatting
{
    /// <summary>
    /// Unit tests for GridFormattingContext.
    /// Tests the CSS Grid Layout algorithm (https://www.w3.org/TR/css-grid-1/).
    /// </summary>
    [TestClass]
    public class GridFormattingContextTests
    {
        #region Basic Tests

        [TestMethod]
        [TestCategory("Grid")]
        public void GridFormattingContext_CanBeInstantiated()
        {
            var context = new GridFormattingContext();
            Assert.IsNotNull(context);
        }

        [TestMethod]
        [TestCategory("Grid")]
        public void GridFormattingContext_ImplementsIFormattingContext()
        {
            var context = new GridFormattingContext();
            Assert.IsInstanceOfType(context, typeof(IFormattingContext));
        }

        [TestMethod]
        [TestCategory("Grid")]
        public void GridFormattingContext_Flow_HandlesNullGracefully()
        {
            var context = new GridFormattingContext();
            Assert.ThrowsException<System.ArgumentNullException>(() => context.Flow(null));
        }

        #endregion

        #region Property Tests

        [TestMethod]
        [TestCategory("Grid")]
        public void EGridAutoFlow_HasExpectedValues()
        {
            Assert.AreEqual(0, (int)EGridAutoFlow.Row);
            Assert.AreEqual(1, (int)EGridAutoFlow.Column);
            Assert.AreEqual(2, (int)EGridAutoFlow.Dense);
        }

        [TestMethod]
        [TestCategory("Grid")]
        public void EGridAutoFlow_DenseCanCombineWithRow()
        {
            var flow = EGridAutoFlow.Row | EGridAutoFlow.Dense;
            Assert.IsTrue((flow & EGridAutoFlow.Dense) != 0);
            Assert.IsTrue((flow & EGridAutoFlow.Column) == 0);
        }

        [TestMethod]
        [TestCategory("Grid")]
        public void EGridAutoFlow_DenseCanCombineWithColumn()
        {
            var flow = EGridAutoFlow.Column | EGridAutoFlow.Dense;
            Assert.IsTrue((flow & EGridAutoFlow.Dense) != 0);
            Assert.IsTrue((flow & EGridAutoFlow.Column) != 0);
        }

        #endregion

        #region Display Mode Integration Tests

        [TestMethod]
        [TestCategory("Grid")]
        public void EDisplayMode_HasGridValues()
        {
            // Verify grid display modes exist
            Assert.IsTrue(System.Enum.IsDefined(typeof(EDisplayMode), "GRID"));
            Assert.IsTrue(System.Enum.IsDefined(typeof(EDisplayMode), "INLINE_GRID"));
        }

        [TestMethod]
        [TestCategory("Grid")]
        public void EInnerDisplayType_HasGridValue()
        {
            Assert.IsTrue(System.Enum.IsDefined(typeof(CssUI.CSS.Enums.EInnerDisplayType), "Grid"));
        }

        #endregion

        #region Property ID Tests

        [TestMethod]
        [TestCategory("Grid")]
        public void ECssPropertyID_HasGridProperties()
        {
            Assert.IsTrue(System.Enum.IsDefined(typeof(ECssPropertyID), "GridTemplateColumns"));
            Assert.IsTrue(System.Enum.IsDefined(typeof(ECssPropertyID), "GridTemplateRows"));
            Assert.IsTrue(System.Enum.IsDefined(typeof(ECssPropertyID), "GridAutoColumns"));
            Assert.IsTrue(System.Enum.IsDefined(typeof(ECssPropertyID), "GridAutoRows"));
            Assert.IsTrue(System.Enum.IsDefined(typeof(ECssPropertyID), "GridAutoFlow"));
            Assert.IsTrue(System.Enum.IsDefined(typeof(ECssPropertyID), "GridColumnStart"));
            Assert.IsTrue(System.Enum.IsDefined(typeof(ECssPropertyID), "GridColumnEnd"));
            Assert.IsTrue(System.Enum.IsDefined(typeof(ECssPropertyID), "GridRowStart"));
            Assert.IsTrue(System.Enum.IsDefined(typeof(ECssPropertyID), "GridRowEnd"));
        }

        #endregion
    }
}
