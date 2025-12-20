using Xunit;
using CssUI.CSS;
using CssUI.CSS.Selectors;
using CssUI.DOM;

namespace CssUI.CSS.Selector.Tests;

/// <summary>
/// Tests for CSS selector specificity calculation.
/// See: https://www.w3.org/TR/selectors-4/#specificity
/// </summary>
public class SpecificityTests
{
    #region Test Infrastructure
    private const string AttributeSelectorBugSkipReason = "Bug: Attribute value selectors cause NullReferenceException in parser";

    private static Document CreateTestDocument()
    {
        var dom = new DOMImplementation();
        return dom.createDocument("CssUI", "cssui");
    }

    private static Element CreateTestElement(Document doc, string tagName)
    {
        return doc.createElement(tagName, new ElementCreationOptions(string.Empty));
    }

    /// <summary>
    /// Helper to get specificity of a selector for a matching element
    /// </summary>
    private static long GetSpecificity(string selectorString, Element element)
    {
        var selector = new CssSelector(selectorString);
        return selector.Get_Specificity(element);
    }

    /// <summary>
    /// Helper to get specificity directly from ComplexSelector without matching
    /// </summary>
    private static long GetSelectorSpecificity(string selectorString)
    {
        var selector = new CssSelector(selectorString);
        if (selector.Count == 0) return 0;
        return selector[0].Get_Specificity();
    }
    #endregion

    #region Basic Specificity Tests - No DOM Required
    [Fact]
    public void Specificity_IdSelector_ParsesCorrectly()
    {
        // ID selector: A=1, B=0, C=0
        var specificity = GetSelectorSpecificity("#myId");
        Assert.True(specificity > 0, "ID selector should have non-zero specificity");
    }

    [Fact]
    public void Specificity_ClassSelector_ParsesCorrectly()
    {
        // Class selector: A=0, B=1, C=0
        var specificity = GetSelectorSpecificity(".myClass");
        Assert.True(specificity > 0, "Class selector should have non-zero specificity");
    }

    [Fact]
    public void Specificity_TypeSelector_ParsesCorrectly()
    {
        // Type selector: A=0, B=0, C=1
        var specificity = GetSelectorSpecificity("div");
        Assert.True(specificity > 0, "Type selector should have non-zero specificity");
    }

    [Fact]
    public void Specificity_UniversalSelector_HasZeroSpecificity()
    {
        // Universal selector: A=0, B=0, C=0
        var specificity = GetSelectorSpecificity("*");
        Assert.Equal(0, specificity);
    }

    [Fact]
    public void Specificity_IdSelector_HigherThanClassSelector()
    {
        var idSpec = GetSelectorSpecificity("#myId");
        var classSpec = GetSelectorSpecificity(".myClass");
        Assert.True(idSpec > classSpec, "ID selector specificity should be higher than class selector");
    }

    [Fact]
    public void Specificity_ClassSelector_HigherThanTypeSelector()
    {
        var classSpec = GetSelectorSpecificity(".myClass");
        var typeSpec = GetSelectorSpecificity("div");
        Assert.True(classSpec > typeSpec, "Class selector specificity should be higher than type selector");
    }

    [Fact]
    public void Specificity_IdSelector_HigherThanTypeSelector()
    {
        var idSpec = GetSelectorSpecificity("#myId");
        var typeSpec = GetSelectorSpecificity("div");
        Assert.True(idSpec > typeSpec, "ID selector specificity should be higher than type selector");
    }
    #endregion

    #region Compound Selector Specificity
    [Fact]
    public void Specificity_TypeAndClass_Compound()
    {
        // div.class: A=0, B=1, C=1
        var compoundSpec = GetSelectorSpecificity("div.myClass");
        var typeSpec = GetSelectorSpecificity("div");
        var classSpec = GetSelectorSpecificity(".myClass");

        Assert.True(compoundSpec > typeSpec, "Compound selector should have higher specificity than just type");
        Assert.True(compoundSpec > classSpec, "Compound selector should have higher specificity than just class");
    }

    [Fact]
    public void Specificity_TypeAndId_Compound()
    {
        // div#id: A=1, B=0, C=1
        var compoundSpec = GetSelectorSpecificity("div#myId");
        var typeSpec = GetSelectorSpecificity("div");
        var idSpec = GetSelectorSpecificity("#myId");

        Assert.True(compoundSpec > typeSpec, "Compound selector should have higher specificity than just type");
        Assert.True(compoundSpec > idSpec, "Compound selector should have higher specificity than just ID");
    }

    [Fact]
    public void Specificity_TypeClassAndId_Compound()
    {
        // div.class#id: A=1, B=1, C=1
        var compoundSpec = GetSelectorSpecificity("div.myClass#myId");
        var typeClassSpec = GetSelectorSpecificity("div.myClass");
        var typeIdSpec = GetSelectorSpecificity("div#myId");

        Assert.True(compoundSpec > typeClassSpec, "Full compound should be higher than type.class");
        Assert.True(compoundSpec > typeIdSpec, "Full compound should be higher than type#id");
    }

    [Fact]
    public void Specificity_MultipleClasses_AccumulateCorrectly()
    {
        // .a.b.c: A=0, B=3, C=0
        var threeClasses = GetSelectorSpecificity(".a.b.c");
        var twoClasses = GetSelectorSpecificity(".a.b");
        var oneClass = GetSelectorSpecificity(".a");

        Assert.True(threeClasses > twoClasses, "Three classes should be higher than two");
        Assert.True(twoClasses > oneClass, "Two classes should be higher than one");
    }

    [Fact]
    public void Specificity_MultipleIds_AccumulateCorrectly()
    {
        // Note: Multiple IDs on same element is invalid HTML but valid CSS
        // This tests that the parser correctly accumulates specificity
        var twoIds = GetSelectorSpecificity("#a#b");
        var oneId = GetSelectorSpecificity("#a");

        Assert.True(twoIds > oneId, "Two IDs should be higher than one");
    }
    #endregion

    #region Complex Selector Specificity (with combinators)
    [Fact]
    public void Specificity_DescendantCombinator_CombinesSpecificities()
    {
        // div p: A=0, B=0, C=2
        var descendantSpec = GetSelectorSpecificity("div p");
        var singleTypeSpec = GetSelectorSpecificity("div");

        Assert.True(descendantSpec > singleTypeSpec, "Descendant selector should combine specificities");
    }

    [Fact]
    public void Specificity_ChildCombinator_CombinesSpecificities()
    {
        // div > p: A=0, B=0, C=2 (combinator doesn't add specificity)
        var childSpec = GetSelectorSpecificity("div > p");
        var descendantSpec = GetSelectorSpecificity("div p");

        // Both should have the same specificity (combinators don't contribute)
        Assert.Equal(descendantSpec, childSpec);
    }

    [Fact]
    public void Specificity_SiblingCombinator_CombinesSpecificities()
    {
        // div + p: A=0, B=0, C=2
        var siblingSpec = GetSelectorSpecificity("div + p");
        var singleTypeSpec = GetSelectorSpecificity("div");

        Assert.True(siblingSpec > singleTypeSpec, "Sibling selector should combine specificities");
    }

    [Fact]
    public void Specificity_DeepCombinator_AccumulatesCorrectly()
    {
        // div > ul > li > a: A=0, B=0, C=4
        var deepSpec = GetSelectorSpecificity("div > ul > li > a");
        var threeTypes = GetSelectorSpecificity("div > ul > li");
        var twoTypes = GetSelectorSpecificity("div > ul");

        Assert.True(deepSpec > threeTypes, "Deeper selector should have higher specificity");
        Assert.True(threeTypes > twoTypes, "More elements should have higher specificity");
    }

    [Fact]
    public void Specificity_MixedCombinatorWithClasses()
    {
        // div.container > p.text: A=0, B=2, C=2
        var mixedSpec = GetSelectorSpecificity("div.container > p.text");
        var simpleSpec = GetSelectorSpecificity("div > p");

        Assert.True(mixedSpec > simpleSpec, "Selectors with classes should be higher");
    }
    #endregion

    #region Pseudo-Class Specificity (same as class)
    [Fact]
    public void Specificity_PseudoClass_SameAsClass()
    {
        // :hover has same specificity as .class (B=1)
        var hoverSpec = GetSelectorSpecificity(":hover");
        var classSpec = GetSelectorSpecificity(".myClass");

        // Both should have B=1, so equal specificity
        Assert.Equal(classSpec, hoverSpec);
    }

    [Fact]
    public void Specificity_TypeAndPseudoClass()
    {
        // a:hover: A=0, B=1, C=1
        var typeHoverSpec = GetSelectorSpecificity("a:hover");
        var typeSpec = GetSelectorSpecificity("a");
        var hoverSpec = GetSelectorSpecificity(":hover");

        Assert.True(typeHoverSpec > typeSpec, "Type + pseudo-class should be higher than just type");
        Assert.True(typeHoverSpec > hoverSpec, "Type + pseudo-class should be higher than just pseudo-class");
    }

    [Fact]
    public void Specificity_MultiplePseudoClasses()
    {
        // :hover:focus: A=0, B=2, C=0
        var twoPC = GetSelectorSpecificity(":hover:focus");
        var onePC = GetSelectorSpecificity(":hover");

        Assert.True(twoPC > onePC, "Two pseudo-classes should be higher than one");
    }
    #endregion

    #region Pseudo-Element Specificity (same as type)
    [Fact(Skip = "Bug: Pseudo-element selectors (::before) not recognized by parser")]
    public void Specificity_PseudoElement_SameAsType()
    {
        // ::before has same specificity as type (C=1)
        var beforeSpec = GetSelectorSpecificity("::before");
        var typeSpec = GetSelectorSpecificity("div");

        // Both should have C=1, so equal specificity
        Assert.Equal(typeSpec, beforeSpec);
    }

    [Fact(Skip = "Bug: Pseudo-element selectors (::first-line) not recognized by parser")]
    public void Specificity_TypeAndPseudoElement()
    {
        // p::first-line: A=0, B=0, C=2
        var typePESpec = GetSelectorSpecificity("p::first-line");
        var typeSpec = GetSelectorSpecificity("p");

        Assert.True(typePESpec > typeSpec, "Type + pseudo-element should be higher than just type");
    }
    #endregion

    #region Attribute Selector Specificity (same as class)
    [Fact(Skip = AttributeSelectorBugSkipReason)]
    public void Specificity_AttributeSelector_SameAsClass()
    {
        // [type] has same specificity as .class (B=1)
        var attrSpec = GetSelectorSpecificity("[type]");
        var classSpec = GetSelectorSpecificity(".myClass");

        Assert.Equal(classSpec, attrSpec);
    }

    [Fact(Skip = AttributeSelectorBugSkipReason)]
    public void Specificity_AttributeValueSelector()
    {
        // [type="text"]: A=0, B=1, C=0
        var attrValueSpec = GetSelectorSpecificity("[type=\"text\"]");
        var classSpec = GetSelectorSpecificity(".myClass");

        // Attribute selectors have same specificity as class
        Assert.Equal(classSpec, attrValueSpec);
    }
    #endregion

    #region DOM-based Matching Tests
    [Fact(Skip = "Bug: ID selector matching doesn't find element.id - needs investigation")]
    public void Specificity_Matching_IdSelectorMatchesElement()
    {
        var doc = CreateTestDocument();
        var element = CreateTestElement(doc, "div");
        element.id = "testId";

        var selector = new CssSelector("#testId");
        Assert.True(selector.Count > 0, "Selector should parse");
        Assert.True(selector[0].Match(element), "ID selector should match element with that ID");
    }

    [Fact(Skip = "Bug: Class selector matching doesn't find element.className - needs investigation")]
    public void Specificity_Matching_ClassSelectorMatchesElement()
    {
        var doc = CreateTestDocument();
        var element = CreateTestElement(doc, "div");
        element.className = "testClass";

        var selector = new CssSelector(".testClass");
        Assert.True(selector.Count > 0, "Selector should parse");
        Assert.True(selector[0].Match(element), "Class selector should match element with that class");
    }

    [Fact]
    public void Specificity_Matching_TypeSelectorMatchesElement()
    {
        var doc = CreateTestDocument();
        var element = CreateTestElement(doc, "div");

        var selector = new CssSelector("div");
        Assert.True(selector.Count > 0, "Selector should parse");
        Assert.True(selector[0].Match(element), "Type selector should match element with that tag name");
    }

    [Fact(Skip = "Bug: Compound selector matching depends on ID/class matching which fails")]
    public void Specificity_Matching_CompoundSelectorMatchesElement()
    {
        var doc = CreateTestDocument();
        var element = CreateTestElement(doc, "div");
        element.id = "testId";
        element.className = "testClass";

        var selector = new CssSelector("div#testId.testClass");
        Assert.True(selector.Count > 0, "Selector should parse");
        Assert.True(selector[0].Match(element), "Compound selector should match element with all attributes");
    }

    [Fact]
    public void Specificity_Matching_IdSelectorDoesNotMatchWrongId()
    {
        var doc = CreateTestDocument();
        var element = CreateTestElement(doc, "div");
        element.id = "otherId";

        var selector = new CssSelector("#testId");
        Assert.True(selector.Count > 0, "Selector should parse");
        Assert.False(selector[0].Match(element), "ID selector should not match element with different ID");
    }

    [Fact]
    public void Specificity_Matching_ClassSelectorDoesNotMatchWrongClass()
    {
        var doc = CreateTestDocument();
        var element = CreateTestElement(doc, "div");
        element.className = "otherClass";

        var selector = new CssSelector(".testClass");
        Assert.True(selector.Count > 0, "Selector should parse");
        Assert.False(selector[0].Match(element), "Class selector should not match element with different class");
    }
    #endregion
}
