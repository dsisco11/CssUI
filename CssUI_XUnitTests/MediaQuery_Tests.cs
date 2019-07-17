using CssUI;
using CssUI.CSS.Serialization;
using CssUI.DOM;
using CssUI.DOM.Media;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace Tests
{
    public class MediaQuery_Tests
    {
        static Document document;
        static DOMImplementation DOM;

        static MediaQuery_Tests()
        {
            DOM = new DOMImplementation();
            document = DOM.createDocument("CssUI", "cssui");
        }

        [Fact]
        public void Test_Media_Query_Parsing()
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
