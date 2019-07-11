using CssUI.DOM.Events;

namespace CssUI.DOM
{
    public interface IWindowEventCallbacks
    {
        /* These print ones MIGHT be used by someone in the future, consider supporting them. */
        /*EventCallback onAfterPrint { get; }
        EventCallback onBeforePrint { get; }*/
        // OnBeforeUnloadEventCallback onbeforeunload { get; }
        event EventCallback onHashChange;
        event EventCallback onLanguageChange;
        event EventCallback onMessage;
        event EventCallback onMessagEerror;
        event EventCallback onOffline;
        event EventCallback onOnline;
        event EventCallback onPageHide;
        event EventCallback onPageShow;
        event EventCallback onPopState;
        event EventCallback onRejectionHandled;
        event EventCallback onStorage;
        event EventCallback onUnhandledRejection;
        /*EventCallback onunload { get; }*/
    }
}
