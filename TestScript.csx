using CssUI.DOM;
using CssUI.DOM.Internal;

var dom = new DOMImplementation();
var doc = dom.createDocument(""CssUI"", ""cssui"");
var root = doc.documentElement;
Console.WriteLine($""root is null: {root is null}"");
Console.WriteLine($""root.Style is null: {root?.Style is null}"");
Console.WriteLine($""root.Style.Cascaded is null: {root?.Style?.Cascaded is null}"");
