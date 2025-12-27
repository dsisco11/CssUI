using System.Collections.Generic;
using System.Linq;
using CssUI;
using CssUI.DOM;
using CssUI.DOM.Enums;
using CssUI.DOM.Exceptions;
using CssUI.DOM.Mutation;
using CssUI.DOM.Nodes;
using Xunit;

namespace CssUITests.DOM.Mutation;

/// <summary>
/// Unit tests specifically for MutationObserver class methods.
/// These tests verify each method works correctly in isolation.
/// </summary>
public class MutationObserverMethodTests
{
    #region Test Infrastructure

    private static TestWindow CreateTestWindow() => new TestWindow();
    private static Document CreateTestDocument(TestWindow window) => window.document;
    private static Element CreateTestElement(Document doc, string tagName)
        => doc.createElement(tagName, new ElementCreationOptions(string.Empty));

    #endregion

    #region Constructor Tests

    [Fact]
    public void Constructor_SetsCallback()
    {
        var window = CreateTestWindow();
        MutationCallback callback = (records, observer) => { };

        var mo = new MutationObserver(window, callback);

        Assert.Same(callback, mo.callback);
    }

    [Fact]
    public void Constructor_AddsObserverToWindowObservers()
    {
        var window = CreateTestWindow();
        var initialCount = window.Observers.Count;

        var mo = new MutationObserver(window, (r, o) => { });

        Assert.Equal(initialCount + 1, window.Observers.Count);
        Assert.Contains(mo, window.Observers);
    }

    #endregion

    #region Observe() Method Tests

    [Fact]
    public void Observe_WithAttributesTrue_AddsToRegisteredObservers()
    {
        var window = CreateTestWindow();
        var doc = CreateTestDocument(window);
        var target = CreateTestElement(doc, "div");
        var mo = new MutationObserver(window, (r, o) => { });

        mo.Observe(target, new MutationObserverInit { attributes = true });

        Assert.Single(target.RegisteredObservers);
        Assert.Same(mo, target.RegisteredObservers[0].observer);
    }

    [Fact]
    public void Observe_WithChildListTrue_AddsToRegisteredObservers()
    {
        var window = CreateTestWindow();
        var doc = CreateTestDocument(window);
        var target = CreateTestElement(doc, "div");
        var mo = new MutationObserver(window, (r, o) => { });

        mo.Observe(target, new MutationObserverInit { childList = true });

        Assert.Single(target.RegisteredObservers);
        Assert.True(target.RegisteredObservers[0].options!.childList);
    }

    [Fact]
    public void Observe_WithCharacterDataTrue_AddsToRegisteredObservers()
    {
        var window = CreateTestWindow();
        var doc = CreateTestDocument(window);
        var target = CreateTestElement(doc, "div");
        var mo = new MutationObserver(window, (r, o) => { });

        mo.Observe(target, new MutationObserverInit { characterData = true });

        Assert.Single(target.RegisteredObservers);
        Assert.True(target.RegisteredObservers[0].options!.characterData);
    }

    [Fact]
    public void Observe_AddsTargetToNodesList()
    {
        var window = CreateTestWindow();
        var doc = CreateTestDocument(window);
        var target = CreateTestElement(doc, "div");
        var mo = new MutationObserver(window, (r, o) => { });

        mo.Observe(target, new MutationObserverInit { attributes = true });

        Assert.Single(mo.Nodes);
        Assert.Same(target, mo.Nodes[0]);
    }

    [Fact]
    public void Observe_WithNullOptions_DoesNotAddObserver()
    {
        var window = CreateTestWindow();
        var doc = CreateTestDocument(window);
        var target = CreateTestElement(doc, "div");
        var mo = new MutationObserver(window, (r, o) => { });

        mo.Observe(target, null);

        Assert.Empty(target.RegisteredObservers);
        Assert.Empty(mo.Nodes);
    }

    [Fact]
    public void Observe_WithNoObservationFlags_ThrowsTypeError()
    {
        var window = CreateTestWindow();
        var doc = CreateTestDocument(window);
        var target = CreateTestElement(doc, "div");
        var mo = new MutationObserver(window, (r, o) => { });

        Assert.Throws<TypeError>(() =>
            mo.Observe(target, new MutationObserverInit()));
    }

    [Fact]
    public void Observe_WithAttributeOldValueButNoAttributes_SetsAttributesTrue()
    {
        var window = CreateTestWindow();
        var doc = CreateTestDocument(window);
        var target = CreateTestElement(doc, "div");
        var mo = new MutationObserver(window, (r, o) => { });

        mo.Observe(target, new MutationObserverInit { attributeOldValue = true });

        Assert.True(target.RegisteredObservers[0].options!.attributes);
        Assert.True(target.RegisteredObservers[0].options!.attributeOldValue);
    }

    [Fact]
    public void Observe_WithAttributeFilterButNoAttributes_SetsAttributesTrue()
    {
        var window = CreateTestWindow();
        var doc = CreateTestDocument(window);
        var target = CreateTestElement(doc, "div");
        var mo = new MutationObserver(window, (r, o) => { });

        mo.Observe(target, new MutationObserverInit { attributeFilter = new List<string> { "class" } });

        Assert.True(target.RegisteredObservers[0].options!.attributes);
    }

    [Fact]
    public void Observe_WithCharacterDataOldValueButNoCharacterData_SetsCharacterDataTrue()
    {
        var window = CreateTestWindow();
        var doc = CreateTestDocument(window);
        var target = CreateTestElement(doc, "div");
        var mo = new MutationObserver(window, (r, o) => { });

        mo.Observe(target, new MutationObserverInit { characterDataOldValue = true });

        Assert.True(target.RegisteredObservers[0].options!.characterData);
    }

    [Fact]
    public void Observe_CalledTwiceOnSameTarget_UpdatesOptions()
    {
        var window = CreateTestWindow();
        var doc = CreateTestDocument(window);
        var target = CreateTestElement(doc, "div");
        var mo = new MutationObserver(window, (r, o) => { });

        mo.Observe(target, new MutationObserverInit { attributes = true });
        mo.Observe(target, new MutationObserverInit { childList = true });

        // Should still have only one registered observer, but with updated options
        Assert.Single(target.RegisteredObservers);
        Assert.True(target.RegisteredObservers[0].options!.childList);
    }

    [Fact]
    public void Observe_OnMultipleTargets_AddsToAllTargets()
    {
        var window = CreateTestWindow();
        var doc = CreateTestDocument(window);
        var target1 = CreateTestElement(doc, "div");
        var target2 = CreateTestElement(doc, "span");
        var mo = new MutationObserver(window, (r, o) => { });

        mo.Observe(target1, new MutationObserverInit { attributes = true });
        mo.Observe(target2, new MutationObserverInit { attributes = true });

        Assert.Single(target1.RegisteredObservers);
        Assert.Single(target2.RegisteredObservers);
        Assert.Equal(2, mo.Nodes.Count);
    }

    #endregion

    #region Disconnect() Method Tests

    [Fact]
    public void Disconnect_RemovesObserverFromAllTargets()
    {
        var window = CreateTestWindow();
        var doc = CreateTestDocument(window);
        var target1 = CreateTestElement(doc, "div");
        var target2 = CreateTestElement(doc, "span");
        var mo = new MutationObserver(window, (r, o) => { });

        mo.Observe(target1, new MutationObserverInit { attributes = true });
        mo.Observe(target2, new MutationObserverInit { attributes = true });

        mo.Disconnect();

        Assert.Empty(target1.RegisteredObservers);
        Assert.Empty(target2.RegisteredObservers);
    }

    [Fact]
    public void Disconnect_ClearsRecordQueue()
    {
        var window = CreateTestWindow();
        var doc = CreateTestDocument(window);
        var target = CreateTestElement(doc, "div");
        doc.documentElement!.appendChild(target);
        var mo = new MutationObserver(window, (r, o) => { });
        mo.Observe(target, new MutationObserverInit { attributes = true });

        // Generate some records
        target.setAttribute("test", AttributeValue.From("value"));

        mo.Disconnect();

        var records = mo.TakeRecords();
        Assert.Empty(records);
    }

    [Fact]
    public void Disconnect_AllowsReobserving()
    {
        var window = CreateTestWindow();
        var doc = CreateTestDocument(window);
        var target = CreateTestElement(doc, "div");
        var mo = new MutationObserver(window, (r, o) => { });

        mo.Observe(target, new MutationObserverInit { attributes = true });
        mo.Disconnect();
        mo.Observe(target, new MutationObserverInit { childList = true });

        Assert.Single(target.RegisteredObservers);
        Assert.True(target.RegisteredObservers[0].options!.childList);
    }

    #endregion

    #region Attribute Mutation Recording Tests

    [Fact]
    public void SetAttribute_ThenRemoveAttribute_BothCreateRecords()
    {
        var window = CreateTestWindow();
        var doc = CreateTestDocument(window);
        var target = CreateTestElement(doc, "div");
        doc.documentElement!.appendChild(target);
        var mo = new MutationObserver(window, (r, o) => { });
        mo.Observe(target, new MutationObserverInit { attributes = true });

        // Set attribute
        target.setAttribute("test", AttributeValue.From("value"));
        var setRecords = mo.TakeRecords();

        // Remove attribute
        target.removeAttribute("test");
        var removeRecords = mo.TakeRecords();

        Assert.True(setRecords.Count > 0, $"setAttribute should create records, got {setRecords.Count}");
        Assert.True(removeRecords.Count > 0, $"removeAttribute should create records, got {removeRecords.Count}");
    }

    [Fact]
    public void MultipleSetAttribute_AllCreateRecords()
    {
        var window = CreateTestWindow();
        var doc = CreateTestDocument(window);
        var target = CreateTestElement(doc, "div");
        doc.documentElement!.appendChild(target);
        var mo = new MutationObserver(window, (r, o) => { });
        mo.Observe(target, new MutationObserverInit { attributes = true });

        target.setAttribute("attr1", AttributeValue.From("value1"));
        target.setAttribute("attr2", AttributeValue.From("value2"));
        target.setAttribute("attr3", AttributeValue.From("value3"));
        var records = mo.TakeRecords();

        // Should have records for all 3 (possibly more due to internal change_attribute calls)
        Assert.True(records.Count >= 3, $"Expected at least 3 records, got {records.Count}");
    }

    [Fact]
    public void ModifyExistingAttribute_CreatesRecord()
    {
        var window = CreateTestWindow();
        var doc = CreateTestDocument(window);
        var target = CreateTestElement(doc, "div");
        doc.documentElement!.appendChild(target);

        // Set attribute before observing
        target.setAttribute("test", AttributeValue.From("original"));

        var mo = new MutationObserver(window, (r, o) => { });
        mo.Observe(target, new MutationObserverInit { attributes = true });

        // Modify existing attribute
        target.setAttribute("test", AttributeValue.From("modified"));
        var records = mo.TakeRecords();

        Assert.NotEmpty(records);
        Assert.Equal(EMutationType.Attributes, records.First().type);
        Assert.Equal("test", records.First().attributeName);
    }

    [Fact]
    public void TakeRecords_ReturnsCorrectMutationType_ForAttributes()
    {
        var window = CreateTestWindow();
        var doc = CreateTestDocument(window);
        var target = CreateTestElement(doc, "div");
        doc.documentElement!.appendChild(target);
        var mo = new MutationObserver(window, (r, o) => { });
        mo.Observe(target, new MutationObserverInit { attributes = true });

        target.setAttribute("test", AttributeValue.From("value"));
        var records = mo.TakeRecords();

        Assert.NotEmpty(records);
        Assert.Equal(EMutationType.Attributes, records.First().type);
    }

    #endregion

    #region TakeRecords() Method Tests

    [Fact]
    public void TakeRecords_ReturnsEmptyWhenNoRecords()
    {
        var window = CreateTestWindow();
        var mo = new MutationObserver(window, (r, o) => { });

        var records = mo.TakeRecords();

        Assert.Empty(records);
    }

    [Fact]
    public void TakeRecords_ReturnsRecordsAfterMutation()
    {
        var window = CreateTestWindow();
        var doc = CreateTestDocument(window);
        var target = CreateTestElement(doc, "div");
        doc.documentElement!.appendChild(target);
        var mo = new MutationObserver(window, (r, o) => { });
        mo.Observe(target, new MutationObserverInit { attributes = true });

        target.setAttribute("test", AttributeValue.From("value"));
        var records = mo.TakeRecords();

        Assert.NotEmpty(records);
    }

    [Fact]
    public void TakeRecords_ClearsQueueAfterCall()
    {
        var window = CreateTestWindow();
        var doc = CreateTestDocument(window);
        var target = CreateTestElement(doc, "div");
        doc.documentElement!.appendChild(target);
        var mo = new MutationObserver(window, (r, o) => { });
        mo.Observe(target, new MutationObserverInit { attributes = true });

        target.setAttribute("test", AttributeValue.From("value"));
        var records1 = mo.TakeRecords();
        var records2 = mo.TakeRecords();

        Assert.NotEmpty(records1);
        Assert.Empty(records2);
    }

    [Fact]
    public void TakeRecords_DoesNotUnregisterObserver()
    {
        var window = CreateTestWindow();
        var doc = CreateTestDocument(window);
        var target = CreateTestElement(doc, "div");
        doc.documentElement!.appendChild(target);
        var mo = new MutationObserver(window, (r, o) => { });
        mo.Observe(target, new MutationObserverInit { attributes = true });

        target.setAttribute("test", AttributeValue.From("value"));
        mo.TakeRecords();

        // Observer should still be registered
        Assert.Single(target.RegisteredObservers);
        Assert.Same(mo, target.RegisteredObservers[0].observer);
    }

    [Fact]
    public void TakeRecords_AllowsCapturingMoreRecordsAfterCall()
    {
        var window = CreateTestWindow();
        var doc = CreateTestDocument(window);
        var target = CreateTestElement(doc, "div");
        doc.documentElement!.appendChild(target);
        var mo = new MutationObserver(window, (r, o) => { });
        mo.Observe(target, new MutationObserverInit { attributes = true });

        target.setAttribute("test1", AttributeValue.From("value1"));
        mo.TakeRecords();

        target.setAttribute("test2", AttributeValue.From("value2"));
        var records = mo.TakeRecords();

        Assert.NotEmpty(records);
        Assert.Contains(records, r => r.attributeName == "test2");
    }

    #endregion

    #region Enqueue() Method Tests

    [Fact]
    public void Enqueue_AddsRecordToQueue()
    {
        var window = CreateTestWindow();
        var doc = CreateTestDocument(window);
        var target = CreateTestElement(doc, "div");
        doc.documentElement!.appendChild(target);
        var mo = new MutationObserver(window, (r, o) => { });
        mo.Observe(target, new MutationObserverInit { attributes = true });

        // Trigger a mutation which internally calls Enqueue
        target.setAttribute("test", AttributeValue.From("value"));

        var records = mo.TakeRecords();
        Assert.NotEmpty(records);
    }

    #endregion

    #region Integration Tests - Multiple Operations

    [Fact]
    public void MultipleObservers_OnSameTarget_AllReceiveRecords()
    {
        var window = CreateTestWindow();
        var doc = CreateTestDocument(window);
        var target = CreateTestElement(doc, "div");
        doc.documentElement!.appendChild(target);
        var mo1 = new MutationObserver(window, (r, o) => { });
        var mo2 = new MutationObserver(window, (r, o) => { });

        mo1.Observe(target, new MutationObserverInit { attributes = true });
        mo2.Observe(target, new MutationObserverInit { attributes = true });

        target.setAttribute("test", AttributeValue.From("value"));

        var records1 = mo1.TakeRecords();
        var records2 = mo2.TakeRecords();

        Assert.NotEmpty(records1);
        Assert.NotEmpty(records2);
    }

    [Fact]
    public void Observer_OnParent_WithSubtree_ReceivesChildRecords()
    {
        var window = CreateTestWindow();
        var doc = CreateTestDocument(window);
        var parent = CreateTestElement(doc, "div");
        var child = CreateTestElement(doc, "span");
        doc.documentElement!.appendChild(parent);
        parent.appendChild(child);
        var mo = new MutationObserver(window, (r, o) => { });

        mo.Observe(parent, new MutationObserverInit { attributes = true, subtree = true });

        child.setAttribute("data-test", AttributeValue.From("value"));

        var records = mo.TakeRecords();
        Assert.NotEmpty(records);
        Assert.Same(child, records.First().target);
    }

    [Fact]
    public void Observer_OnParent_WithoutSubtree_DoesNotReceiveChildRecords()
    {
        var window = CreateTestWindow();
        var doc = CreateTestDocument(window);
        var parent = CreateTestElement(doc, "div");
        var child = CreateTestElement(doc, "span");
        doc.documentElement!.appendChild(parent);
        parent.appendChild(child);
        var mo = new MutationObserver(window, (r, o) => { });

        mo.Observe(parent, new MutationObserverInit { attributes = true, subtree = false });

        child.setAttribute("test", AttributeValue.From("value"));

        var records = mo.TakeRecords();
        Assert.Empty(records);
    }

    [Fact]
    public void Observer_WithAttributeFilter_OnlyReceivesFilteredAttributes()
    {
        var window = CreateTestWindow();
        var doc = CreateTestDocument(window);
        var target = CreateTestElement(doc, "div");
        doc.documentElement!.appendChild(target);
        var mo = new MutationObserver(window, (r, o) => { });

        mo.Observe(target, new MutationObserverInit
        {
            attributes = true,
            attributeFilter = new List<string> { "data-class" }
        });

        target.setAttribute("data-other", AttributeValue.From("test-other"));
        target.setAttribute("data-class", AttributeValue.From("test-class"));

        var records = mo.TakeRecords();

        // Should only have records for "data-class" attribute
        Assert.All(records, r => Assert.Equal("data-class", r.attributeName));
    }

    #endregion
}
