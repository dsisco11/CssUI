using System;
using System.Collections.Generic;
using System.Text;

namespace CssUI.CSS
{
    public class CSSStyleDeclaration : List<CssDecleration>
    {
        public readonly CSSRule parentRule;
        public string cssFloat;

        public string cssText {
            get {
                StringBuilder sb = new StringBuilder();
                foreach(CssDecleration dec in this)
                {
                    sb.AppendLine(dec.ToString());
                }

                return sb.ToString();
            }

            set
            {
                /*
                 * 1) If the readonly flag is set, throw a NoModificationAllowedError exception and terminate these steps.
                 * 2) Empty the declarations.
                 * 3) Parse the given value and, if the return value is not the empty list, insert the items in the list into the declarations, in specified order.
                 */
                throw new NotImplementedException();
            }
        }
        //public getter string item(ulong index) { }
        public string getPropertyValue(string property) { throw new NotImplementedException(); }
        public string getPropertyPriority(string property) { throw new NotImplementedException(); }
        public void setProperty(string property, string value, string priority = "") { throw new NotImplementedException(); }
        public void setPropertyValue(string property, string value) { throw new NotImplementedException(); }
        public void setPropertyPriority(string property, string priority) { throw new NotImplementedException(); }
        public string removeProperty(string property) { throw new NotImplementedException(); }

        public CSSStyleDeclaration(CSSRule parentRule)
        {
            this.parentRule = parentRule;
        }
    }
}
