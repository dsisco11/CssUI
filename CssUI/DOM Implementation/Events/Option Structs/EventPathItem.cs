using System.Collections.Generic;

namespace CssUI.DOM.Events
{
    public struct EventPathItem
    {
        public EventTarget invocationTarget;
        public bool invocation_target_in_shadow_tree;
        public EventTarget shadow_adjusted_target;
        public EventTarget relatedTarget;
        public List<EventTarget> touch_target_list;
        public bool root_of_closed_tree;
        public bool slot_in_closed_tree;
    }
}
