using Xunit;
using CssUI.DOM.Media;
using System;
using System.Collections.Generic;
using System.Text;
using CssUI.CSS.Serialization;

namespace CssUI.DOM.Media.Tests
{
    public class MediaQueryListTests
    {
        static Document document;
        static DOMImplementation DOM;

        static MediaQueryListTests()
        {
            DOM = new DOMImplementation();
            document = DOM.createDocument("CssUI", "cssui");
        }

        [Fact()]
        public void MediaQueryListTest()
        {
            throw new NotImplementedException();
        }

        [Fact()]
        public void addEventListenerTest()
        {
            throw new NotImplementedException();
        }

        [Fact()]
        public void removeEventListenerTest()
        {
            throw new NotImplementedException();
        }

        [Fact()]
        public void SerializeTest()
        {
            throw new NotImplementedException();
        }
    }
}