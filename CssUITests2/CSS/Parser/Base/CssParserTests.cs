using Xunit;
using CssUI.CSS.Serialization;
using System;
using System.Collections.Generic;
using System.Text;
using CssUI.DOM;

namespace CssUI.CSS.Serialization.Tests
{
    public class CssParserTests
    {
        static Document document;
        static DOMImplementation DOM;

        static CssParserTests()
        {// Many of the parsers functions will require a DOM and document
            DOM = new DOMImplementation();
            document = DOM.createDocument("CssUI", "cssui");
        }


        [Fact()]
        public void CssParserTest()
        {
            throw new NotImplementedException();
        }

        [Fact()]
        public void CssParserTest1()
        {
            throw new NotImplementedException();
        }

        [Fact()]
        public void Parse_Rule_ListTest()
        {
            throw new NotImplementedException();
        }

        [Fact()]
        public void Parse_RuleTest()
        {
            throw new NotImplementedException();
        }

        [Fact()]
        public void Parse_DeclerationTest()
        {
            throw new NotImplementedException();
        }

        [Fact()]
        public void Parse_Decleration_ListTest()
        {
            throw new NotImplementedException();
        }

        [Fact()]
        public void Parse_ComponentValueTest()
        {
            throw new NotImplementedException();
        }

        [Fact()]
        public void Parse_ComponentValue_ListTest()
        {
            throw new NotImplementedException();
        }

        [Fact()]
        public void Parse_CssValueTest()
        {
            throw new NotImplementedException();
        }

        [Fact()]
        public void Consume_CssValueTest()
        {
            throw new NotImplementedException();
        }

        [Fact()]
        public void Parse_Media_Query_ListTest()
        {
            CssParser parser;

            /* Parse a single condition */
            parser = new CssParser("@media (width <= 320px)");
            Assert.NotNull(parser.Parse_Media_Query_List(document));

            /* Parse negator */
            parser = new CssParser("@media not (color)");
            Assert.NotNull(parser.Parse_Media_Query_List(document));


            /* Parse multiple conditions */
            parser = new CssParser("@media (width <= 320px) or (height <= 100px)");
            Assert.NotNull(parser.Parse_Media_Query_List(document));

            /* Parse nested conditions */
            parser = new CssParser("@media (not (color)) or (width <= 320px)");
            Assert.NotNull(parser.Parse_Media_Query_List(document));


            /* Lets test the parsing on some complex media querys  */
            parser = new CssParser("@media (not ((color) or (grid)) or ((width <= 320px) and (height < 1080px))");
            Assert.NotNull(parser.Parse_Media_Query_List(document));


            /* Ensure we get syntax errors for certain things */

            /* Ensure throws on unsupported features*/
            parser = new CssParser("@media (hover)");
            Assert.Throws<CssSyntaxErrorException>(() => parser.Parse_Media_Query_List(document));

            /* Ensure throws on same-level mixed combinators */
            parser = new CssParser("@media not (color) or (grid)");
            Assert.Throws<CssSyntaxErrorException>(() => parser.Parse_Media_Query_List(document));
        }
    }
}