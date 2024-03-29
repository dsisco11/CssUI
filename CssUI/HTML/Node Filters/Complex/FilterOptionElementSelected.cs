﻿using CssUI.DOM.Enums;
using CssUI.DOM.Nodes;
using CssUI.HTML;

namespace CssUI.DOM
{
    /// <summary>
    /// Accepts only <see cref="HTMLOptionElement"/> whose selectedness is true
    /// </summary>
    public class FilterOptionElementSelected : NodeFilter
    {
        public static readonly NodeFilter Instance = new FilterOptionElementSelected();

        public override ENodeFilterResult acceptNode(Node node)
        {
            if (node is HTMLOptionElement opt && opt.selected)
            {
                return ENodeFilterResult.FILTER_ACCEPT;
            }

            return ENodeFilterResult.FILTER_SKIP;
        }
    }
}
