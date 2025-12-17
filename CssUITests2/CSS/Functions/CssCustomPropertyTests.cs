using Microsoft.VisualStudio.TestTools.UnitTesting;
using CssUI.CSS;
using CssUI.CSS.Functions;

namespace CssUITests.CSS.Functions
{
    /// <summary>
    /// Unit tests for CSS Custom Properties (var()) support.
    /// Spec: https://www.w3.org/TR/css-variables-1/
    /// </summary>
    [TestClass]
    public class CssCustomPropertyTests
    {
        #region Registry Basic Tests

        [TestMethod]
        [TestCategory("Var")]
        public void CssCustomPropertyRegistry_CanBeInstantiated()
        {
            var registry = new CssCustomPropertyRegistry();
            Assert.IsNotNull(registry);
        }

        [TestMethod]
        [TestCategory("Var")]
        public void CssCustomPropertyRegistry_CanSetAndGetProperty()
        {
            var registry = new CssCustomPropertyRegistry();
            var value = CssValue.From(42);
            
            registry.Set("--my-value", value);
            var retrieved = registry.Get("--my-value");
            
            Assert.IsNotNull(retrieved);
            Assert.AreEqual(42, retrieved.AsInteger());
        }

        [TestMethod]
        [TestCategory("Var")]
        public void CssCustomPropertyRegistry_ReturnsNullForUndefined()
        {
            var registry = new CssCustomPropertyRegistry();
            var retrieved = registry.Get("--undefined-property");
            
            Assert.IsNull(retrieved);
        }

        [TestMethod]
        [TestCategory("Var")]
        public void CssCustomPropertyRegistry_ContainsReturnsTrueForDefined()
        {
            var registry = new CssCustomPropertyRegistry();
            registry.Set("--my-prop", CssValue.From(100));
            
            Assert.IsTrue(registry.Contains("--my-prop"));
        }

        [TestMethod]
        [TestCategory("Var")]
        public void CssCustomPropertyRegistry_ContainsReturnsFalseForUndefined()
        {
            var registry = new CssCustomPropertyRegistry();
            
            Assert.IsFalse(registry.Contains("--undefined"));
        }

        #endregion

        #region Inheritance Tests

        [TestMethod]
        [TestCategory("Var")]
        public void CssCustomPropertyRegistry_InheritsFromParent()
        {
            var parent = new CssCustomPropertyRegistry();
            parent.Set("--inherited-color", CssValue.From(0xFF0000));
            
            var child = new CssCustomPropertyRegistry(parent);
            var retrieved = child.Get("--inherited-color");
            
            Assert.IsNotNull(retrieved);
            Assert.AreEqual(0xFF0000, retrieved.AsInteger());
        }

        [TestMethod]
        [TestCategory("Var")]
        public void CssCustomPropertyRegistry_LocalOverridesInherited()
        {
            var parent = new CssCustomPropertyRegistry();
            parent.Set("--color", CssValue.From(100));
            
            var child = new CssCustomPropertyRegistry(parent);
            child.Set("--color", CssValue.From(200));
            
            var retrieved = child.Get("--color");
            
            Assert.IsNotNull(retrieved);
            Assert.AreEqual(200, retrieved.AsInteger());
        }

        [TestMethod]
        [TestCategory("Var")]
        public void CssCustomPropertyRegistry_ParentNotAffectedByChild()
        {
            var parent = new CssCustomPropertyRegistry();
            parent.Set("--color", CssValue.From(100));
            
            var child = new CssCustomPropertyRegistry(parent);
            child.Set("--color", CssValue.From(200));
            
            var parentValue = parent.Get("--color");
            
            Assert.IsNotNull(parentValue);
            Assert.AreEqual(100, parentValue.AsInteger());
        }

        #endregion

        #region Validation Tests

        [TestMethod]
        [TestCategory("Var")]
        public void CssCustomPropertyRegistry_IsValidCustomPropertyName_Valid()
        {
            Assert.IsTrue(CssCustomPropertyRegistry.IsValidCustomPropertyName("--my-prop"));
            Assert.IsTrue(CssCustomPropertyRegistry.IsValidCustomPropertyName("--a"));
            Assert.IsTrue(CssCustomPropertyRegistry.IsValidCustomPropertyName("--my-long-property-name"));
        }

        [TestMethod]
        [TestCategory("Var")]
        public void CssCustomPropertyRegistry_IsValidCustomPropertyName_Invalid()
        {
            Assert.IsFalse(CssCustomPropertyRegistry.IsValidCustomPropertyName("my-prop"));
            Assert.IsFalse(CssCustomPropertyRegistry.IsValidCustomPropertyName("-my-prop"));
            Assert.IsFalse(CssCustomPropertyRegistry.IsValidCustomPropertyName(""));
            Assert.IsFalse(CssCustomPropertyRegistry.IsValidCustomPropertyName(null));
        }

        [TestMethod]
        [TestCategory("Var")]
        [ExpectedException(typeof(System.ArgumentException))]
        public void CssCustomPropertyRegistry_ThrowsOnInvalidName()
        {
            var registry = new CssCustomPropertyRegistry();
            registry.Set("invalid-name", CssValue.From(42));
        }

        #endregion

        #region Remove and Clear Tests

        [TestMethod]
        [TestCategory("Var")]
        public void CssCustomPropertyRegistry_Remove()
        {
            var registry = new CssCustomPropertyRegistry();
            registry.Set("--to-remove", CssValue.From(42));
            
            Assert.IsTrue(registry.Contains("--to-remove"));
            
            registry.Remove("--to-remove");
            
            Assert.IsFalse(registry.Contains("--to-remove"));
        }

        [TestMethod]
        [TestCategory("Var")]
        public void CssCustomPropertyRegistry_Clear()
        {
            var registry = new CssCustomPropertyRegistry();
            registry.Set("--prop1", CssValue.From(1));
            registry.Set("--prop2", CssValue.From(2));
            
            registry.Clear();
            
            Assert.IsFalse(registry.Contains("--prop1"));
            Assert.IsFalse(registry.Contains("--prop2"));
        }

        #endregion
    }
}
