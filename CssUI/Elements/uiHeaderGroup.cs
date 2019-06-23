using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CssUI
{
    /// <summary>
    /// A block level container which displays a title text at the top of itself.
    /// </summary>
    public class uiHeaderGroup : cssContainerElement
    {
        public override string Default_CSS_TypeName { get { return "GroupBox"; } }

        #region Components
        cssTextElement text;
        #endregion

        #region Accessors
        public string Text
        {
            get { return text.Text; }
            set { text.Text = value; }
        }
        #endregion

        #region Constructors
        public uiHeaderGroup(string ID = null) : base(ID)
        {
            Style.User.Display.Set(EDisplayMode.BLOCK);
            Layout = ELayoutMode.Default;

            text = new cssTextElement("Title");
            text.Style.User.Display.Set(EDisplayMode.BLOCK);
            //text.TextFont = new System.Drawing.Font(text.TextFont.FontFamily, 20);
            Add(text);
        }
        #endregion
    }
}
