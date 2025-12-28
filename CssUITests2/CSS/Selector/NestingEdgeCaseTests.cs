using System.Linq;
using CssUI.CSS;
using CssUI.CSS.Selectors;
using CssUI.DOM;
using Xunit;

namespace CssUITests.CSS.Selector.Tests;

/// <summary>
/// Edge case tests for CSS Nesting features.
/// Covers deeply nested rules, multiple &amp; usage, and complex nesting patterns.
/// </summary>
/// <seealso href="https://www.w3.org/TR/css-nesting-1/"/>
[Trait("Category", "CSS Selector")]
[Trait("Spec", "CSS Nesting 1")]
public class NestingEdgeCaseTests
{
    #region Test Infrastructure
    private static Document CreateTestDocument()
    {
        var dom = new DOMImplementation();
        return dom.createDocument("CssUI", "cssui");
    }

    private static Element CreateTestElement(Document doc, string tagName)
    {
        return doc.createElement(tagName, new ElementCreationOptions(string.Empty));
    }

    private static Element CreateElementWithClass(Document doc, string tagName, string className)
    {
        var element = CreateTestElement(doc, tagName);
        element.className = className;
        return element;
    }
    #endregion

    #region Deeply Nested Rules (3+ levels)

    [Fact]
    public void DeeplyNested_ThreeLevels_ParsesCorrectly()
    {
        // Level 1: .a
        // Level 2: &.b (becomes .a.b)
        // Level 3: &.c (becomes .a.b.c)
        var level1 = new CssSelector(".a");
        var level2 = new CssSelector("&.b");
        var level3 = new CssSelector("&.c");

        // Set up nesting chain
        SetParentOnNestingSelectors(level2, level1);
        SetParentOnNestingSelectors(level3, level2);

        // All should parse without error
        Assert.Single(level1);
        Assert.Single(level2);
        Assert.Single(level3);
    }

    [Fact]
    public void DeeplyNested_ThreeLevels_SpecificityAccumulates()
    {
        // Level 1: .a           → (0,1,0)
        // Level 2: &.b          → (0,2,0) = .a + .b
        // Level 3: &.c          → (0,3,0) = .a.b + .c
        var level1 = new CssSelector(".a");
        var level2 = new CssSelector("&.b");
        var level3 = new CssSelector("&.c");

        SetParentOnNestingSelectors(level2, level1);
        SetParentOnNestingSelectors(level3, level2);

        // Act
        long spec1 = level1[0].Get_Specificity();
        long spec2 = level2[0].Get_Specificity();
        long spec3 = level3[0].Get_Specificity();

        // Assert - Each level adds one class
        Assert.Equal(1L << 16, spec1); // (0,1,0)
        Assert.Equal(2L << 16, spec2); // (0,2,0)
        Assert.Equal(3L << 16, spec3); // (0,3,0)
    }

    [Fact]
    public void DeeplyNested_FourLevels_ParsesCorrectly()
    {
        var level1 = new CssSelector(".container");
        var level2 = new CssSelector("& .item");
        var level3 = new CssSelector("&:hover");
        var level4 = new CssSelector("& span");

        SetParentOnNestingSelectors(level2, level1);
        SetParentOnNestingSelectors(level3, level2);
        SetParentOnNestingSelectors(level4, level3);

        // All parse correctly
        Assert.NotEmpty(level1);
        Assert.NotEmpty(level2);
        Assert.NotEmpty(level3);
        Assert.NotEmpty(level4);
    }

    [Fact]
    public void DeeplyNested_WithMixedSelectors_ParsesCorrectly()
    {
        // Complex nesting with different selector types
        var level1 = new CssSelector("#main");          // ID
        var level2 = new CssSelector("&.active");       // & + class
        var level3 = new CssSelector("&[data-state]");  // & + attribute
        var level4 = new CssSelector("&::before");      // & + pseudo-element

        SetParentOnNestingSelectors(level2, level1);
        SetParentOnNestingSelectors(level3, level2);
        SetParentOnNestingSelectors(level4, level3);

        Assert.NotEmpty(level1);
        Assert.NotEmpty(level2);
        Assert.NotEmpty(level3);
        Assert.NotEmpty(level4);
    }

    [Fact]
    public void DeeplyNested_FiveLevels_SpecificityCorrect()
    {
        // 5 levels of nesting, each adding a class
        var level1 = new CssSelector(".l1");
        var level2 = new CssSelector("&.l2");
        var level3 = new CssSelector("&.l3");
        var level4 = new CssSelector("&.l4");
        var level5 = new CssSelector("&.l5");

        SetParentOnNestingSelectors(level2, level1);
        SetParentOnNestingSelectors(level3, level2);
        SetParentOnNestingSelectors(level4, level3);
        SetParentOnNestingSelectors(level5, level4);

        long spec5 = level5[0].Get_Specificity();

        // 5 classes = (0,5,0)
        Assert.Equal(5L << 16, spec5);
    }

    #endregion

    #region Multiple & Usage in Same Selector

    [Fact]
    public void MultipleAmpersand_TwoInSameCompound_Parses()
    {
        // "&&" - two nesting selectors in same compound
        // Per spec, this is valid and each & refers to the parent
        var selector = new CssSelector("&&");

        Assert.NotEmpty(selector);
        var compound = selector[0][0];

        // Should have two nesting selectors
        int nestingCount = compound.Count(s => s is NestingSelector);
        Assert.Equal(2, nestingCount);
    }

    [Fact]
    public void MultipleAmpersand_TwoWithCombinator_Parses()
    {
        // "& + &" - sibling of self
        var selector = new CssSelector("& + &");

        Assert.NotEmpty(selector);
        Assert.Equal(2, selector[0].Count); // Two RelativeSelectors
        Assert.IsType<NestingSelector>(selector[0][0][0]);
        Assert.IsType<NestingSelector>(selector[0][1][0]);
    }

    [Fact]
    public void MultipleAmpersand_ThreeWithCombinators_Parses()
    {
        // "& > & > &" - nested within nested within parent
        var selector = new CssSelector("& > & > &");

        Assert.NotEmpty(selector);
        Assert.Equal(3, selector[0].Count); // Three RelativeSelectors
    }

    [Fact]
    public void MultipleAmpersand_TwoWithDescendant_Parses()
    {
        // "& &" - descendant of self
        var selector = new CssSelector("& &");

        Assert.NotEmpty(selector);
        Assert.Equal(2, selector[0].Count);
    }

    [Fact]
    public void MultipleAmpersand_Specificity_StacksCorrectly()
    {
        // Parent is ".foo" with specificity (0,1,0)
        // "& &" should have specificity (0,2,0) since each & gets parent's specificity
        var parent = new CssSelector(".foo");
        var selector = new CssSelector("& &");

        SetParentOnNestingSelectors(selector, parent);

        long specificity = selector[0].Get_Specificity();

        // Two & references, each contributing (0,1,0)
        Assert.Equal(2L << 16, specificity);
    }

    [Fact]
    public void MultipleAmpersand_ThreeReferences_SpecificityTriples()
    {
        // Parent is ".bar" with specificity (0,1,0)
        // "& + & + &" should have specificity (0,3,0)
        var parent = new CssSelector(".bar");
        var selector = new CssSelector("& + & + &");

        SetParentOnNestingSelectors(selector, parent);

        long specificity = selector[0].Get_Specificity();

        // Three & references
        Assert.Equal(3L << 16, specificity);
    }

    [Fact]
    public void MultipleAmpersand_WithIdParent_HighSpecificity()
    {
        // Parent is "#id" with specificity (1,0,0)
        // "& &" should have specificity (2,0,0)
        var parent = new CssSelector("#id");
        var selector = new CssSelector("& &");

        SetParentOnNestingSelectors(selector, parent);

        long specificity = selector[0].Get_Specificity();

        // Two IDs worth = (2,0,0)
        Assert.Equal(2L << 32, specificity);
    }

    #endregion

    #region Multiple & in Same Compound (&&)

    [Fact]
    public void DoubleAmpersand_Specificity_Doubles()
    {
        // "&&" - two & in same compound, parent is ".x"
        var parent = new CssSelector(".x");
        var selector = new CssSelector("&&");

        SetParentOnNestingSelectors(selector, parent);

        long specificity = selector[0].Get_Specificity();

        // Two & in same compound = 2 × (0,1,0) = (0,2,0)
        Assert.Equal(2L << 16, specificity);
    }

    [Fact]
    public void DoubleAmpersand_WithClass_Parses()
    {
        // "&&.active" - two & plus a class
        var selector = new CssSelector("&&.active");

        Assert.NotEmpty(selector);
        var compound = selector[0][0];

        int nestingCount = compound.Count(s => s is NestingSelector);
        int classCount = compound.Count(s => s is ClassSelector);

        Assert.Equal(2, nestingCount);
        Assert.Equal(1, classCount);
    }

    [Fact]
    public void TripleAmpersand_Parses()
    {
        // "&&&" - three nesting selectors
        var selector = new CssSelector("&&&");

        Assert.NotEmpty(selector);
        var compound = selector[0][0];

        int nestingCount = compound.Count(s => s is NestingSelector);
        Assert.Equal(3, nestingCount);
    }

    #endregion

    #region Complex Nesting Patterns

    [Fact]
    public void ComplexPattern_AmpersandAfterClass_Parses()
    {
        // ".class&" - class followed by &
        var selector = new CssSelector(".class&");

        Assert.NotEmpty(selector);
        var compound = selector[0][0];

        Assert.Contains(compound, s => s is ClassSelector);
        Assert.Contains(compound, s => s is NestingSelector);
    }

    [Fact]
    public void ComplexPattern_AmpersandBetweenClasses_Parses()
    {
        // ".a&.b" - class, &, class
        var selector = new CssSelector(".a&.b");

        Assert.NotEmpty(selector);
        var compound = selector[0][0];

        int classCount = compound.Count(s => s is ClassSelector);
        int nestingCount = compound.Count(s => s is NestingSelector);

        Assert.Equal(2, classCount);
        Assert.Equal(1, nestingCount);
    }

    [Fact]
    public void ComplexPattern_AmpersandWithPseudoElement_Parses()
    {
        // "&::after" - nesting selector with pseudo-element
        var selector = new CssSelector("&::after");

        Assert.NotEmpty(selector);
        Assert.Contains(selector[0][0], s => s is NestingSelector);
        Assert.Contains(selector[0][0], s => s is PseudoElementSelector);
    }

    [Fact]
    public void ComplexPattern_AmpersandWithAttribute_Parses()
    {
        // "&[disabled]" - nesting selector with attribute
        var selector = new CssSelector("&[disabled]");

        Assert.NotEmpty(selector);
        Assert.Contains(selector[0][0], s => s is NestingSelector);
        Assert.Contains(selector[0][0], s => s is AttributeSelector);
    }

    [Fact]
    public void ComplexPattern_MixedCombinators_Parses()
    {
        // "& > .child + & ~ .sibling" - complex combinator chain
        var selector = new CssSelector("& > .child + .sibling");

        Assert.NotEmpty(selector);
        Assert.True(selector[0].Count >= 3);
    }

    [Fact]
    public void ComplexPattern_SelectorList_Parses()
    {
        // "&.a, &.b" - comma-separated list with nesting
        var selector = new CssSelector("&.a, &.b");

        Assert.Equal(2, selector.Count); // Two complex selectors
        Assert.Contains(selector[0][0], s => s is NestingSelector);
        Assert.Contains(selector[1][0], s => s is NestingSelector);
    }

    [Fact]
    public void ComplexPattern_SelectorList_MultipleAmpersands()
    {
        // "& &, & > &" - list with multiple & in each
        var selector = new CssSelector("& &, & > &");

        Assert.Equal(2, selector.Count);

        // First selector: "& &" has 2 RelativeSelectors
        Assert.Equal(2, selector[0].Count);

        // Second selector: "& > &" also has 2
        Assert.Equal(2, selector[1].Count);
    }

    #endregion

    #region CSSStyleRule Nesting Edge Cases

    [Fact]
    public void CSSStyleRule_FourLevelsDeep_SerializesCorrectly()
    {
        // Create 4 levels of nested rules
        var level1 = new CSSStyleRule(new CssSelector(".l1"), null);
        var level2 = new CSSStyleRule(new CssSelector("&.l2"), null);
        var level3 = new CSSStyleRule(new CssSelector("&.l3"), null);
        var level4 = new CSSStyleRule(new CssSelector("&.l4"), null);

        level3.cssRules.Add(level4);
        level2.cssRules.Add(level3);
        level1.cssRules.Add(level2);

        // Act
        string serialized = level1.cssText;

        // Assert - All levels appear with proper nesting
        Assert.Contains(".l1", serialized);
        Assert.Contains("&.l2", serialized);
        Assert.Contains("&.l3", serialized);
        Assert.Contains("&.l4", serialized);

        // Count braces - should be 4 open, 4 close
        int openBraces = serialized.Count(c => c == '{');
        int closeBraces = serialized.Count(c => c == '}');
        Assert.Equal(4, openBraces);
        Assert.Equal(openBraces, closeBraces);
    }

    [Fact]
    public void CSSStyleRule_MultipleChildrenAtEachLevel_SerializesCorrectly()
    {
        // Level 1 with 2 children, each child with 2 grandchildren
        var parent = new CSSStyleRule(new CssSelector(".parent"), null);

        var child1 = new CSSStyleRule(new CssSelector("&.child1"), null);
        var child2 = new CSSStyleRule(new CssSelector("&.child2"), null);

        var grandchild1a = new CSSStyleRule(new CssSelector("&.gc1a"), null);
        var grandchild1b = new CSSStyleRule(new CssSelector("&.gc1b"), null);
        var grandchild2a = new CSSStyleRule(new CssSelector("&.gc2a"), null);
        var grandchild2b = new CSSStyleRule(new CssSelector("&.gc2b"), null);

        child1.cssRules.Add(grandchild1a);
        child1.cssRules.Add(grandchild1b);
        child2.cssRules.Add(grandchild2a);
        child2.cssRules.Add(grandchild2b);

        parent.cssRules.Add(child1);
        parent.cssRules.Add(child2);

        // Act
        string serialized = parent.cssText;

        // Assert - All rules appear
        Assert.Contains(".parent", serialized);
        Assert.Contains("&.child1", serialized);
        Assert.Contains("&.child2", serialized);
        Assert.Contains("&.gc1a", serialized);
        Assert.Contains("&.gc1b", serialized);
        Assert.Contains("&.gc2a", serialized);
        Assert.Contains("&.gc2b", serialized);

        // 7 rules = 7 brace pairs
        int openBraces = serialized.Count(c => c == '{');
        Assert.Equal(7, openBraces);
    }

    [Fact]
    public void CSSStyleRule_DoubleAmpersandSelector_SerializesCorrectly()
    {
        var parent = new CSSStyleRule(new CssSelector(".item"), null);
        var child = new CSSStyleRule(new CssSelector("&&"), null);

        parent.cssRules.Add(child);

        string serialized = parent.cssText;

        Assert.Contains(".item", serialized);
        Assert.Contains("&&", serialized);
    }

    [Fact]
    public void CSSStyleRule_SiblingAmpersand_SerializesCorrectly()
    {
        var parent = new CSSStyleRule(new CssSelector(".item"), null);
        var child = new CSSStyleRule(new CssSelector("& + &"), null);

        parent.cssRules.Add(child);

        string serialized = parent.cssText;

        Assert.Contains(".item", serialized);
        Assert.Contains("& + &", serialized);
    }

    #endregion

    #region Helper Methods

    private static void SetParentOnNestingSelectors(CssSelector child, CssSelector parent)
    {
        foreach (var complex in child)
        {
            foreach (var relative in complex)
            {
                foreach (var simple in relative)
                {
                    if (simple is NestingSelector nesting)
                    {
                        nesting.SetParentSelector(parent);
                    }
                }
            }
        }
    }

    #endregion
}
