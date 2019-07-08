using CssUI.DOM.Events;

namespace CssUI.DOM
{
    public interface IWindowEventHandlers
    {
        /* These print ones MIGHT be used by someone in the future, consider supporting them. */
        /*EventHandler onafterprint { get; }
        EventHandler onbeforeprint { get; }*/
        // OnBeforeUnloadEventHandler onbeforeunload { get; }
        event EventHandler onhashchange;
        event EventHandler onlanguagechange;
        event EventHandler onmessage;
        event EventHandler onmessageerror;
        event EventHandler onoffline;
        event EventHandler ononline;
        event EventHandler onpagehide;
        event EventHandler onpageshow;
        event EventHandler onpopstate;
        event EventHandler onrejectionhandled;
        event EventHandler onstorage;
        event EventHandler onunhandledrejection;
        /*EventHandler onunload { get; }*/
    }
}
