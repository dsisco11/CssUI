using CssUI.CSS;
using CssUI.CSS.Functions;
using Xunit;

namespace CssUITests.CSS.Functions;

/// <summary>
/// Unit tests for CSS Custom Properties (var()) support.
/// Spec: https://www.w3.org/TR/css-variables-1/
/// </summary>
public class CssCustomPropertyTests
{
    #region Registry Basic Tests

    [Fact]
    [Trait("Category", "Var")]
    public void CssCustomPropertyRegistry_CanBeInstantiated()
    {
        var registry = new CssCustomPropertyRegistry();
        Assert.NotNull(registry);
    }

    [Fact]
    [Trait("Category", "Var")]
    public void CssCustomPropertyRegistry_CanSetAndGetProperty()
    {
        var registry = new CssCustomPropertyRegistry();
        var value = CssValue.From(42);

        registry.Set("--my-value", value);
        var retrieved = registry.Get("--my-value");

        Assert.NotNull(retrieved);
        Assert.Equal(42, retrieved.AsInteger());
    }

    [Fact]
    [Trait("Category", "Var")]
    public void CssCustomPropertyRegistry_ReturnsNullForUndefined()
    {
        var registry = new CssCustomPropertyRegistry();
        var retrieved = registry.Get("--undefined-property");

        Assert.Null(retrieved);
    }

    [Fact]
    [Trait("Category", "Var")]
    public void CssCustomPropertyRegistry_ContainsReturnsTrueForDefined()
    {
        var registry = new CssCustomPropertyRegistry();
        registry.Set("--my-prop", CssValue.From(100));

        Assert.True(registry.Contains("--my-prop"));
    }

    [Fact]
    [Trait("Category", "Var")]
    public void CssCustomPropertyRegistry_ContainsReturnsFalseForUndefined()
    {
        var registry = new CssCustomPropertyRegistry();

        Assert.False(registry.Contains("--undefined"));
    }

    #endregion

    #region Inheritance Tests

    [Fact]
    [Trait("Category", "Var")]
    public void CssCustomPropertyRegistry_InheritsFromParent()
    {
        var parent = new CssCustomPropertyRegistry();
        parent.Set("--inherited-color", CssValue.From(0xFF0000));

        var child = new CssCustomPropertyRegistry(parent);
        var retrieved = child.Get("--inherited-color");

        Assert.NotNull(retrieved);
        Assert.Equal(0xFF0000, retrieved.AsInteger());
    }

    [Fact]
    [Trait("Category", "Var")]
    public void CssCustomPropertyRegistry_LocalOverridesInherited()
    {
        var parent = new CssCustomPropertyRegistry();
        parent.Set("--color", CssValue.From(100));

        var child = new CssCustomPropertyRegistry(parent);
        child.Set("--color", CssValue.From(200));

        var retrieved = child.Get("--color");

        Assert.NotNull(retrieved);
        Assert.Equal(200, retrieved.AsInteger());
    }

    [Fact]
    [Trait("Category", "Var")]
    public void CssCustomPropertyRegistry_ParentNotAffectedByChild()
    {
        var parent = new CssCustomPropertyRegistry();
        parent.Set("--color", CssValue.From(100));

        var child = new CssCustomPropertyRegistry(parent);
        child.Set("--color", CssValue.From(200));

        var parentValue = parent.Get("--color");

        Assert.NotNull(parentValue);
        Assert.Equal(100, parentValue.AsInteger());
    }

    #endregion

    #region Validation Tests

    [Fact]
    [Trait("Category", "Var")]
    public void CssCustomPropertyRegistry_IsValidCustomPropertyName_Valid()
    {
        Assert.True(CssCustomPropertyRegistry.IsValidCustomPropertyName("--my-prop"));
        Assert.True(CssCustomPropertyRegistry.IsValidCustomPropertyName("--a"));
        Assert.True(CssCustomPropertyRegistry.IsValidCustomPropertyName("--my-long-property-name"));
    }

    [Fact]
    [Trait("Category", "Var")]
    public void CssCustomPropertyRegistry_IsValidCustomPropertyName_Invalid()
    {
        Assert.False(CssCustomPropertyRegistry.IsValidCustomPropertyName("my-prop"));
        Assert.False(CssCustomPropertyRegistry.IsValidCustomPropertyName("-my-prop"));
        Assert.False(CssCustomPropertyRegistry.IsValidCustomPropertyName(""));
        Assert.False(CssCustomPropertyRegistry.IsValidCustomPropertyName(null));
    }

    [Fact]
    [Trait("Category", "Var")]
    public void CssCustomPropertyRegistry_ThrowsOnInvalidName()
    {
        var registry = new CssCustomPropertyRegistry();
        Assert.Throws<System.ArgumentException>(() => registry.Set("invalid-name", CssValue.From(42)));
    }

    #endregion

    #region Remove and Clear Tests

    [Fact]
    [Trait("Category", "Var")]
    public void CssCustomPropertyRegistry_Remove()
    {
        var registry = new CssCustomPropertyRegistry();
        registry.Set("--to-remove", CssValue.From(42));

        Assert.True(registry.Contains("--to-remove"));

        registry.Remove("--to-remove");

        Assert.False(registry.Contains("--to-remove"));
    }

    [Fact]
    [Trait("Category", "Var")]
    public void CssCustomPropertyRegistry_Clear()
    {
        var registry = new CssCustomPropertyRegistry();
        registry.Set("--prop1", CssValue.From(1));
        registry.Set("--prop2", CssValue.From(2));

        registry.Clear();

        Assert.False(registry.Contains("--prop1"));
        Assert.False(registry.Contains("--prop2"));
    }

    #endregion
}
