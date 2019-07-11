using CssUI.DOM.Internal;

namespace CssUI.DOM.Events
{
    [DomEnum]
    public enum EEventName : int
    {
        /// <summary>
        /// Represents all custom (end-user specified) event values
        /// </summary>
        CUSTOM = -1,
        [DomKeyword("")]
        None = 0x0,
        [DomKeyword("abort")]
        abort,
        [DomKeyword("auxclick")]
        AuxClick,
        [DomKeyword("blur")]
        Blur,
        [DomKeyword("cancel")]
        Cancel,
        [DomKeyword("canplay")]
        CanPlay,
        [DomKeyword("canplaythrough")]
        CanPlayThrough,
        [DomKeyword("change")]
        Change,
        [DomKeyword("click")]
        Click,
        [DomKeyword("close")]
        Close,
        [DomKeyword("contextmenu")]
        ContextMenu,
        [DomKeyword("cuechange")]
        CueChange,
        [DomKeyword("dblclick")]
        DoubleClick,
    /* Drag drop */
        [DomKeyword("drag")]
        Drag,
        [DomKeyword("dragend")]
        DragEnd,
        [DomKeyword("dragenter")]
        DragEnter,
        [DomKeyword("dragexit")]
        DragExit,
        [DomKeyword("dragleave")]
        DragLeave,
        [DomKeyword("dragover")]
        DragOver,
        [DomKeyword("dragstart")]
        DragStart,
        [DomKeyword("drop")]
        Drop,

        [DomKeyword("durationchange")]
        DurationChange,
        [DomKeyword("emptied")]
        Emptied,
        [DomKeyword("ended")]
        Ended,

        [DomKeyword("error")]
        Error,

        [DomKeyword("focus")]
        Focus,
        [DomKeyword("formdata")]
        FormData,
        [DomKeyword("input")]
        Input,
        [DomKeyword("invalid")]
        Invalid,
        [DomKeyword("keydown")]
        KeyDown,
        [DomKeyword("keypress")]
        KeyPress,
        [DomKeyword("keyup")]
        KeyUp,
        [DomKeyword("load")]
        Load,
        [DomKeyword("loadeddata")]
        LoadedData,
        [DomKeyword("loadedmetadata")]
        LoadedMetadata,
        [DomKeyword("loadend")]
        LoadEnd,
        [DomKeyword("loadstart")]
        LoadStart,
/* Mouse input */
        [DomKeyword("mousedown")]
        MouseDown,
        [DomKeyword("mouseenter")]
        MouseEnter,
        [DomKeyword("mouseleave")]
        MouseLeave,
        [DomKeyword("mousemove")]
        MouseMove,
        [DomKeyword("mouseout")]
        MouseOut,
        [DomKeyword("mouseover")]
        MouseOver,
        [DomKeyword("mouseup")]
        MouseUp,
        [DomKeyword("wheel")]
        Wheel,
/* Media */
        [DomKeyword("pause")]
        Pause,
        [DomKeyword("play")]
        Play,
        [DomKeyword("playing")]
        Playing,
        [DomKeyword("progress")]
        Progress,
        [DomKeyword("ratechange")]
        RateChange,
        [DomKeyword("reset")]
        Reset,
        [DomKeyword("resize")]
        Resize,
        [DomKeyword("scroll")]
        Scroll,
        [DomKeyword("securitypolicyviolation")]
        SecurityPolicyViolation,
        [DomKeyword("seeked")]
        Seeked,
        [DomKeyword("seeking")]
        Seeking,
        [DomKeyword("select")]
        Select,
        [DomKeyword("stalled")]
        Stalled,
        [DomKeyword("submit")]
        Submit,
        [DomKeyword("suspend")]
        Suspend,
        [DomKeyword("timeupdate")]
        Timeupdate,
        [DomKeyword("toggle")]
        Toggle,
        [DomKeyword("volumechange")]
        Volumechange,
        [DomKeyword("waiting")]
        Waiting,

        [DomKeyword("selectstart")]
        SelectStart,
        [DomKeyword("selectionchange")]
        SelectionChange,
        
        [DomKeyword("copy")]
        Copy,
        [DomKeyword("cut")]
        Cut,
        [DomKeyword("paste")]
        Paste,

/* Window events */
        [DomKeyword("afterprint")]
        AfterPrint,
        [DomKeyword("beforeprint")]
        BeforePrint,
        [DomKeyword("beforeunload")]
        BeforeUnload,
        [DomKeyword("hashchange")]
        HashChange,
        [DomKeyword("languagechange")]
        LanguageChange,
        [DomKeyword("message")]
        Message,
        [DomKeyword("messageerror")]
        MessageError,
        [DomKeyword("offline")]
        Offline,
        [DomKeyword("online")]
        Online,
        [DomKeyword("pagehide")]
        PageHide,
        [DomKeyword("pageshow")]
        PageShow,
        [DomKeyword("popstate")]
        PopState,
        [DomKeyword("rejectionhandled")]
        RejectionHandled,
        [DomKeyword("storage")]
        Storage,
        [DomKeyword("unhandledrejection")]
        UnhandledRejection,
        [DomKeyword("unload")]
        Unload,


        [DomKeyword("slotchange")]
        SlotChange,
    }
}
