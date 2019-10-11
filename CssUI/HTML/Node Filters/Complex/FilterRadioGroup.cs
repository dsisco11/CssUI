using CssUI.DOM.Enums;
using CssUI.DOM.Nodes;
using CssUI.HTML;
using System;

namespace CssUI.DOM
{
    /// <summary>
    /// Rejects any node that isnt an instance of <see cref="Element"/>
    /// </summary>
    public class FilterRadioGroup : NodeFilter
    {
        public readonly HTMLInputElement Subject = null;
        private readonly HTMLFormElement subjectForm = null;
        private readonly string subjectName = null;
        private readonly bool Subject_Is_Valid = false;

        public FilterRadioGroup(HTMLInputElement Subject)
        {
            this.Subject = Subject;
            this.subjectForm = Subject.form;/* We cache the subjects form here so we dont have to keep looking it up */

            if (Subject.hasAttribute(EAttributeName.Name, out Attr outAttr))
            {
                string nameStr = outAttr?.Value?.AsString();
                if (!ReferenceEquals(null, nameStr) && nameStr.Length > 0)
                {
                    Subject_Is_Valid = true;
                    subjectName = nameStr;
                }
            }
        }

        public override ENodeFilterResult acceptNode(Node node)
        {/* Docs: https://html.spec.whatwg.org/multipage/input.html#radio-button-state-(type=radio):radio-button-group */

            if (!Subject_Is_Valid)
                return ENodeFilterResult.FILTER_REJECT;

            if (node.nodeType != ENodeType.ELEMENT_NODE)
                return ENodeFilterResult.FILTER_REJECT;

            /* Dont accept our own subject or else theyll end up in their own grouplist which doesnt make sense and will also screw up the logic of input element functions */
            if (ReferenceEquals(Subject, node))
                return ENodeFilterResult.FILTER_SKIP;

            /* The input element b's type attribute is in the Radio Button state. */
            if (node is HTMLInputElement inputElement && inputElement.type == EInputType.Radio)
            {/* Either a and b have the same form owner, or they both have no form owner. */
                if (ReferenceEquals(subjectForm, inputElement.form))
                {/* Both a and b are in the same tree. */
                    if (ReferenceEquals(Subject.ownerDocument, inputElement.ownerDocument))
                    {/* They both have a name attribute, their name attributes are not empty, and the value of a's name attribute equals the value of b's name attribute. */
                        string nameStr = inputElement.name;
                        if (!ReferenceEquals(null, nameStr) && nameStr.Length > 0)
                        {
                            if (StringCommon.StrEq(subjectName, nameStr))
                            {
                                return ENodeFilterResult.FILTER_ACCEPT;
                            }
                        }
                    }
                }
            }

            return ENodeFilterResult.FILTER_SKIP;
        }
    }
}
