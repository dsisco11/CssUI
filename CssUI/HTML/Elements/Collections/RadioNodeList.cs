using CssUI.DOM;
using CssUI.DOM.Nodes;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CssUI.HTML
{
    public class RadioNodeList : List<Node>
    {/* Docs: https://html.spec.whatwg.org/multipage/common-dom-interfaces.html#radionodelist */
        #region Properties
        #endregion

        #region Constructors
        public RadioNodeList(IEnumerable<Node> collection) : base(collection)
        {
        }
        #endregion

        public string value
        {/* Docs: https://html.spec.whatwg.org/multipage/common-dom-interfaces.html#dom-radionodelist-value */
            get
            {
                var element = (HTMLInputElement)this.FirstOrDefault(n => n is HTMLInputElement input && input.type == EInputType.Radio && input.Checked);
                if (element == null) return string.Empty;

                if (!element.hasAttribute(EAttributeName.Value, out Attr attr))
                {
                    return "on";
                }

                return attr.Value.AsString();
            }

            set
            {
                HTMLInputElement element = null;
                if (StringCommon.StrEq("on", value))
                {
                    element = (HTMLInputElement)this.FirstOrDefault(n => n is HTMLInputElement input && input.type == EInputType.Radio && (!input.hasAttribute(EAttributeName.Value, out Attr attr) || StringCommon.StrEq(value.AsSpan(), attr.Value.AsString().AsSpan())));
                }
                else
                {
                    element = (HTMLInputElement)this.FirstOrDefault(n => n is HTMLInputElement input && input.type == EInputType.Radio && input.hasAttribute(EAttributeName.Value, out Attr attr) && StringCommon.StrEq(value.AsSpan(), attr.Value.AsString().AsSpan()));
                }

                if (element != null)
                {
                    element.Checked = true;
                }
            }
        }

    }
}
