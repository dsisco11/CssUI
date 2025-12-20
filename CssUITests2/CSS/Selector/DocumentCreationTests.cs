using Xunit;
using CssUI.DOM;

namespace CssUI.CSS.Selector.Tests;

/// <summary>
/// Tests to verify that Document creation works after the Get_Location fix.
/// </summary>
public class DocumentCreationTests
{
    [Fact]
    public void DOMImplementation_CreateDocument_ShouldNotThrow()
    {
        // Arrange
        var dom = new DOMImplementation();
        
        // Act & Assert - This should not throw TypeInitializationException
        var document = dom.createDocument("CssUI", "cssui");
        Assert.NotNull(document);
    }
    
    [Fact]
    public void Document_CreateElement_ShouldWork()
    {
        // Arrange
        var dom = new DOMImplementation();
        var document = dom.createDocument("CssUI", "cssui");
        
        // Act
        var element = document.createElement("div", new ElementCreationOptions(string.Empty));
        
        // Assert
        Assert.NotNull(element);
        Assert.Equal("div", element.localName);
    }
    
    [Fact]
    public void Element_SetId_ShouldWork()
    {
        // Arrange
        var dom = new DOMImplementation();
        var document = dom.createDocument("CssUI", "cssui");
        var element = document.createElement("div", new ElementCreationOptions(string.Empty));
        
        // Act
        element.id = "test-id";
        
        // Assert
        Assert.Equal("test-id", element.id);
    }
    
    [Fact]
    public void Element_SetClassName_ShouldWork()
    {
        // Arrange
        var dom = new DOMImplementation();
        var document = dom.createDocument("CssUI", "cssui");
        var element = document.createElement("div", new ElementCreationOptions(string.Empty));
        
        // Act
        element.className = "test-class another-class";
        
        // Assert
        Assert.Equal("test-class another-class", element.className);
    }
    
    [Fact]
    public void Element_AppendChild_ShouldWork()
    {
        // Arrange
        var dom = new DOMImplementation();
        var document = dom.createDocument("CssUI", "cssui");
        var parent = document.createElement("div", new ElementCreationOptions(string.Empty));
        var child = document.createElement("span", new ElementCreationOptions(string.Empty));
        
        // Act
        parent.appendChild(child);
        
        // Assert
        Assert.Single(parent.childNodes);
        Assert.Same(child, parent.firstChild);
    }
}
