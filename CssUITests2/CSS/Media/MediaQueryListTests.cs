using System;
using Xunit;

namespace CssUI.DOM.Media.Tests;

public class MediaQueryListTests
{
    static Document document;
    static DOMImplementation DOM;

    static MediaQueryListTests()
    {
        DOM = new DOMImplementation();
        document = DOM.createDocument("CssUI", "cssui");
    }

    [Fact(Skip = "Test stub - needs implementation")]
    public void MediaQueryListTest()
    {
        throw new NotImplementedException();
    }

    [Fact(Skip = "Test stub - needs implementation")]
    public void addEventListenerTest()
    {
        throw new NotImplementedException();
    }

    [Fact(Skip = "Test stub - needs implementation")]
    public void removeEventListenerTest()
    {
        throw new NotImplementedException();
    }

    [Fact(Skip = "Test stub - needs implementation")]
    public void SerializeTest()
    {
        throw new NotImplementedException();
    }
}
