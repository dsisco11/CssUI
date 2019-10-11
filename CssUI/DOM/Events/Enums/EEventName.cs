using CssUI.Internal;

namespace CssUI.DOM.Events
{
    [MetaEnum]
    public enum EEventName : int
    {
        /// <summary>
        /// Represents all custom (end-user specified) event values
        /// </summary>
        CUSTOM = -1,
        [MetaKeyword("")]
        None = 0x0,
        [MetaKeyword("abort")]
        Abort,
        [MetaKeyword("auxclick")]
        AuxClick,
        [MetaKeyword("blur")]
        Blur,
        [MetaKeyword("cancel")]
        Cancel,
        [MetaKeyword("canplay")]
        CanPlay,
        [MetaKeyword("canplaythrough")]
        CanPlayThrough,
        [MetaKeyword("change")]
        Change,
        [MetaKeyword("click")]
        Click,
        [MetaKeyword("close")]
        Close,
        [MetaKeyword("contextmenu")]
        ContextMenu,
        [MetaKeyword("cuechange")]
        CueChange,
        [MetaKeyword("dblclick")]
        DoubleClick,

    /* Drag drop */
        [MetaKeyword("drag")]
        Drag,
        [MetaKeyword("dragend")]
        DragEnd,
        [MetaKeyword("dragenter")]
        DragEnter,
        [MetaKeyword("dragexit")]
        DragExit,
        [MetaKeyword("dragleave")]
        DragLeave,
        [MetaKeyword("dragover")]
        DragOver,
        [MetaKeyword("dragstart")]
        DragStart,
        [MetaKeyword("drop")]
        Drop,

        [MetaKeyword("durationchange")]
        DurationChange,
        [MetaKeyword("emptied")]
        Emptied,
        [MetaKeyword("ended")]
        Ended,

        [MetaKeyword("error")]
        Error,

        [MetaKeyword("focus")]
        Focus,
        [MetaKeyword("formdata")]
        FormData,
        [MetaKeyword("input")]
        Input,
        [MetaKeyword("invalid")]
        Invalid,
        [MetaKeyword("keydown")]
        KeyDown,
        [MetaKeyword("keypress")]
        KeyPress,
        [MetaKeyword("keyup")]
        KeyUp,
        [MetaKeyword("load")]
        Load,
        [MetaKeyword("loadeddata")]
        LoadedData,
        [MetaKeyword("loadedmetadata")]
        LoadedMetadata,
        [MetaKeyword("loadend")]
        LoadEnd,
        [MetaKeyword("loadstart")]
        LoadStart,

/* Mouse input */
        [MetaKeyword("mousedown")]
        MouseDown,
        [MetaKeyword("mouseenter")]
        MouseEnter,
        [MetaKeyword("mouseleave")]
        MouseLeave,
        [MetaKeyword("mousemove")]
        MouseMove,
        [MetaKeyword("mouseout")]
        MouseOut,
        [MetaKeyword("mouseover")]
        MouseOver,
        [MetaKeyword("mouseup")]
        MouseUp,
        [MetaKeyword("wheel")]
        Wheel,

/* Pointer input */
/* Docs: https://w3c.github.io/pointerevents/#intro */
        [MetaKeyword("pointerover")]
        PointerOver,
        [MetaKeyword("pointerdown")]
        PointerDown,
        [MetaKeyword("pointermove")]
        PointerMove,
        [MetaKeyword("pointerup")]
        PointerUp,
        [MetaKeyword("pointercancel")]
        PointerCancel,
        [MetaKeyword("pointerout")]
        PointerOut,
        [MetaKeyword("pointerleave")]
        PointerLeave,
        [MetaKeyword("gotpointercapture")]
        GotPointerCapture,
        [MetaKeyword("lostpointercapture")]
        LostPointerCapture,

        /* Touch input */
        /* Docs: https://w3c.github.io/touch-events/#introduction */
        [MetaKeyword("touchstart")]
        TouchStart,
        [MetaKeyword("touchend")]
        TouchEnd,
        [MetaKeyword("touchmove")]
        TouchMove,
        [MetaKeyword("touchcancel")]
        TouchCancel,


        /* Media */
        [MetaKeyword("pause")]
        Pause,
        [MetaKeyword("play")]
        Play,
        [MetaKeyword("playing")]
        Playing,
        [MetaKeyword("progress")]
        Progress,
        [MetaKeyword("ratechange")]
        RateChange,
        [MetaKeyword("reset")]
        Reset,
        [MetaKeyword("resize")]
        Resize,
        [MetaKeyword("scroll")]
        Scroll,
        [MetaKeyword("securitypolicyviolation")]
        SecurityPolicyViolation,
        [MetaKeyword("seeked")]
        Seeked,
        [MetaKeyword("seeking")]
        Seeking,
        [MetaKeyword("select")]
        Select,
        [MetaKeyword("stalled")]
        Stalled,
        [MetaKeyword("submit")]
        Submit,
        [MetaKeyword("suspend")]
        Suspend,
        [MetaKeyword("timeupdate")]
        TimeUpdate,
        [MetaKeyword("toggle")]
        Toggle,
        [MetaKeyword("volumechange")]
        VolumeChange,
        [MetaKeyword("waiting")]
        Waiting,

        [MetaKeyword("selectstart")]
        SelectStart,
        [MetaKeyword("selectionchange")]
        SelectionChange,
        
        [MetaKeyword("copy")]
        Copy,
        [MetaKeyword("cut")]
        Cut,
        [MetaKeyword("paste")]
        Paste,

/* Window events */
        [MetaKeyword("afterprint")]
        AfterPrint,
        [MetaKeyword("beforeprint")]
        BeforePrint,
        [MetaKeyword("beforeunload")]
        BeforeUnload,
        [MetaKeyword("hashchange")]
        HashChange,
        [MetaKeyword("languagechange")]
        LanguageChange,
        [MetaKeyword("message")]
        Message,
        [MetaKeyword("messageerror")]
        MessageError,
        [MetaKeyword("offline")]
        Offline,
        [MetaKeyword("online")]
        Online,
        [MetaKeyword("pagehide")]
        PageHide,
        [MetaKeyword("pageshow")]
        PageShow,
        [MetaKeyword("popstate")]
        PopState,
        [MetaKeyword("rejectionhandled")]
        RejectionHandled,
        [MetaKeyword("storage")]
        Storage,
        [MetaKeyword("unhandledrejection")]
        UnhandledRejection,
        [MetaKeyword("unload")]
        Unload,


        [MetaKeyword("slotchange")]
        SlotChange,

    }
}
