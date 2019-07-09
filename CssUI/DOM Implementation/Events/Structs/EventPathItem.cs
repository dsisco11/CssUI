using System.Collections.Generic;

namespace CssUI.DOM.Events
{
    public struct EventPathItem
    {
        public IEventTarget invocationTarget;
        public bool invocation_target_in_shadow_tree;
        public IEventTarget shadow_adjusted_target;
        public IEventTarget relatedTarget;
        public List<IEventTarget> touch_target_list;
        public bool root_of_closed_tree;
        public bool slot_in_closed_tree;
    }
}
