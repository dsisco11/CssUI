using CssUI.DOM.CustomElements;
using CssUI.DOM.Nodes;
using System.Collections.Generic;

namespace CssUI.DOM
{
    public class HTMLInputElement : HTMLElement, IFormAssociatedElement
    {
        #region Properties
        public bool bParserInserted { get; set; }

        #endregion

        #region Constructors
        public HTMLInputElement(Document document) : base(document, "input")
        {
        }

        public HTMLInputElement(Document document, string localName) : base(document, localName)
        {
        }
        #endregion


        [CEReactions] public string accept;
        [CEReactions] public string alt;
        [CEReactions] public string autocomplete;
        [CEReactions] public bool autofocus;
        [CEReactions] public bool defaultChecked;
        public bool Checked;
        [CEReactions] public string dirName;
        [CEReactions] public bool disabled;
        public HTMLFormElement form { get; private set; }
        public FileList files;
        [CEReactions] public string formAction;
        [CEReactions] public string formEnctype;
        [CEReactions] public string formMethod;
        [CEReactions] public bool formNoValidate;
        [CEReactions] public string formTarget;
        [CEReactions] public uint height;
        public bool indeterminate;
        public HTMLElement list { get; private set; }
        [CEReactions] public string max;
        [CEReactions] public long maxLength;
        [CEReactions] public string min;
        [CEReactions] public long minLength;
        [CEReactions] public bool multiple;
        [CEReactions] public string name;
        [CEReactions] public string pattern;
        [CEReactions] public string placeholder;
        [CEReactions] public bool readOnly;
        [CEReactions] public bool required;
        [CEReactions] public uint size;
        [CEReactions] public string src;
        [CEReactions] public string step;

        [CEReactions]
        public EInputType type
        {/* Docs: https://html.spec.whatwg.org/multipage/input.html#dom-input-type */
            get => getAttribute(EAttributeName.Type).Get_Enum<EInputType>();
            set => CEReactions.Wrap_CEReaction(this, () => setAttribute(EAttributeName.Type, AttributeValue.From_Enum(value)));
        }

        [CEReactions] public string defaultValue;
        [CEReactions] public string value;
        public object valueAsDate;
        public double valueAsNumber;
        [CEReactions] public uint width;

        public void stepUp(long n = 1);
        public void stepDown(long n = 1);

        public readonly bool willValidate;
        public readonly EValidityState validity;
        public readonly string validationMessage;

        public bool checkValidity();
        public bool reportValidity();
        public void setCustomValidity(string error);

        public IReadOnlyCollection<Node> labels;

        public void select();
        public uint selectionStart;
        public uint selectionEnd;
        public string selectionDirection;

        /* Docs: https://html.spec.whatwg.org/multipage/input.html#concept-input-type-image-coordinate */
        public int selected_coordinate_x { get; private set; } = 0;
        public int selected_coordinate_y { get; private set; } = 0;

        public void setRangeText(string replacement);
        public void setRangeText(string replacement, uint start, uint end, SelectionMode selectionMode = "preserve");
        public void setSelectionRange(uint start, uint end, string direction);

    }
}
