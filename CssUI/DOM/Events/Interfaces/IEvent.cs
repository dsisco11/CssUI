using System.Collections.Generic;

namespace CssUI.DOM.Events
{
    public interface IEvent
    {
        System.Boolean cancelBubble { get; set; }
        System.Boolean canceled { get; set; }
        System.Boolean composed { get; }
        EventTarget currentTarget { get; }
        System.Boolean defaultPrevented { get; }
        EEventPhase eventPhase { get; }
        EEventFlags Flags { get; }
        System.Boolean isTrusted { get; }
        List<EventPathItem> Path { get; }
        EventTarget relatedTarget { get; }
        System.Boolean returnValue { get; set; }
        EventTarget target { get; }
        DOMHighResTimeStamp timeStamp { get; }
        LinkedList<IEventTarget> TouchTargetList { get; }

        List<EventTarget> composedPath();
        void preventDefault();
        void stopImmediatePropagation();
        void stopPropagation();
    }
}