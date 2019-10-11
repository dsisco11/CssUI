using System;
using CssUI.DOM;

namespace CssUI.CSS.BoxTree
{
    /* 
     * Docs: https://www.w3.org/TR/CSS2/visudet.html#leading 
     * Docs: https://www.w3.org/TR/css-text-3/#intro
     */

    public class CssTextRun : CssBoxTreeNode
    {
        #region Properties
        public readonly WeakReference<Text>[] Items;
        #endregion

        #region Constructors
        public CssTextRun(Text[] items) : base(null)
        {
            Items = new WeakReference<Text>[items.Length];
            for (int i = 0; i < items.Length; i++)
            {
                Items[i] = new WeakReference<Text>(items[i]);
            }
        }

        ~CssTextRun()
        {
        }
        #endregion
    }
}
