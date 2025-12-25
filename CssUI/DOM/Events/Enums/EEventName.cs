using EnumRecords;

namespace CssUI.DOM.Events;

[EnumRecord<KeywordProperties>]
public enum EEventName : int
{
    /// <summary>
    /// Represents all custom (end-user specified) event values
    /// </summary>
    CUSTOM = -1,
    [EnumRecordProperties("")]
    None = 0x0,
    [EnumRecordProperties("abort")]
    Abort,
    [EnumRecordProperties("auxclick")]
    AuxClick,
    [EnumRecordProperties("blur")]
    Blur,
    [EnumRecordProperties("cancel")]
    Cancel,
    [EnumRecordProperties("canplay")]
    CanPlay,
    [EnumRecordProperties("canplaythrough")]
    CanPlayThrough,
    [EnumRecordProperties("change")]
    Change,
    [EnumRecordProperties("click")]
    Click,
    [EnumRecordProperties("close")]
    Close,
    [EnumRecordProperties("contextmenu")]
    ContextMenu,
    [EnumRecordProperties("cuechange")]
    CueChange,
    [EnumRecordProperties("dblclick")]
    DoubleClick,

    /* Drag drop */
    [EnumRecordProperties("drag")]
    Drag,
    [EnumRecordProperties("dragend")]
    DragEnd,
    [EnumRecordProperties("dragenter")]
    DragEnter,
    [EnumRecordProperties("dragexit")]
    DragExit,
    [EnumRecordProperties("dragleave")]
    DragLeave,
    [EnumRecordProperties("dragover")]
    DragOver,
    [EnumRecordProperties("dragstart")]
    DragStart,
    [EnumRecordProperties("drop")]
    Drop,

    [EnumRecordProperties("durationchange")]
    DurationChange,
    [EnumRecordProperties("emptied")]
    Emptied,
    [EnumRecordProperties("ended")]
    Ended,

    [EnumRecordProperties("error")]
    Error,

    [EnumRecordProperties("focus")]
    Focus,
    [EnumRecordProperties("formdata")]
    FormData,
    [EnumRecordProperties("input")]
    Input,
    [EnumRecordProperties("invalid")]
    Invalid,
    [EnumRecordProperties("keydown")]
    KeyDown,
    [EnumRecordProperties("keypress")]
    KeyPress,
    [EnumRecordProperties("keyup")]
    KeyUp,
    [EnumRecordProperties("load")]
    Load,
    [EnumRecordProperties("loadeddata")]
    LoadedData,
    [EnumRecordProperties("loadedmetadata")]
    LoadedMetadata,
    [EnumRecordProperties("loadend")]
    LoadEnd,
    [EnumRecordProperties("loadstart")]
    LoadStart,

    /* Mouse input */
    [EnumRecordProperties("mousedown")]
    MouseDown,
    [EnumRecordProperties("mouseenter")]
    MouseEnter,
    [EnumRecordProperties("mouseleave")]
    MouseLeave,
    [EnumRecordProperties("mousemove")]
    MouseMove,
    [EnumRecordProperties("mouseout")]
    MouseOut,
    [EnumRecordProperties("mouseover")]
    MouseOver,
    [EnumRecordProperties("mouseup")]
    MouseUp,
    [EnumRecordProperties("wheel")]
    Wheel,

    /* Pointer input */
    /* Docs: https://w3c.github.io/pointerevents/#intro */
    [EnumRecordProperties("pointerover")]
    PointerOver,
    [EnumRecordProperties("pointerdown")]
    PointerDown,
    [EnumRecordProperties("pointermove")]
    PointerMove,
    [EnumRecordProperties("pointerup")]
    PointerUp,
    [EnumRecordProperties("pointercancel")]
    PointerCancel,
    [EnumRecordProperties("pointerout")]
    PointerOut,
    [EnumRecordProperties("pointerleave")]
    PointerLeave,
    [EnumRecordProperties("gotpointercapture")]
    GotPointerCapture,
    [EnumRecordProperties("lostpointercapture")]
    LostPointerCapture,

    /* Touch input */
    /* Docs: https://w3c.github.io/touch-events/#introduction */
    [EnumRecordProperties("touchstart")]
    TouchStart,
    [EnumRecordProperties("touchend")]
    TouchEnd,
    [EnumRecordProperties("touchmove")]
    TouchMove,
    [EnumRecordProperties("touchcancel")]
    TouchCancel,


    /* Media */
    [EnumRecordProperties("pause")]
    Pause,
    [EnumRecordProperties("play")]
    Play,
    [EnumRecordProperties("playing")]
    Playing,
    [EnumRecordProperties("progress")]
    Progress,
    [EnumRecordProperties("ratechange")]
    RateChange,
    [EnumRecordProperties("reset")]
    Reset,
    [EnumRecordProperties("resize")]
    Resize,
    [EnumRecordProperties("scroll")]
    Scroll,
    [EnumRecordProperties("securitypolicyviolation")]
    SecurityPolicyViolation,
    [EnumRecordProperties("seeked")]
    Seeked,
    [EnumRecordProperties("seeking")]
    Seeking,
    [EnumRecordProperties("select")]
    Select,
    [EnumRecordProperties("stalled")]
    Stalled,
    [EnumRecordProperties("submit")]
    Submit,
    [EnumRecordProperties("suspend")]
    Suspend,
    [EnumRecordProperties("timeupdate")]
    TimeUpdate,
    [EnumRecordProperties("toggle")]
    Toggle,
    [EnumRecordProperties("volumechange")]
    VolumeChange,
    [EnumRecordProperties("waiting")]
    Waiting,

    [EnumRecordProperties("selectstart")]
    SelectStart,
    [EnumRecordProperties("selectionchange")]
    SelectionChange,

    [EnumRecordProperties("copy")]
    Copy,
    [EnumRecordProperties("cut")]
    Cut,
    [EnumRecordProperties("paste")]
    Paste,

    /* Window events */
    [EnumRecordProperties("afterprint")]
    AfterPrint,
    [EnumRecordProperties("beforeprint")]
    BeforePrint,
    [EnumRecordProperties("beforeunload")]
    BeforeUnload,
    [EnumRecordProperties("hashchange")]
    HashChange,
    [EnumRecordProperties("languagechange")]
    LanguageChange,
    [EnumRecordProperties("message")]
    Message,
    [EnumRecordProperties("messageerror")]
    MessageError,
    [EnumRecordProperties("offline")]
    Offline,
    [EnumRecordProperties("online")]
    Online,
    [EnumRecordProperties("pagehide")]
    PageHide,
    [EnumRecordProperties("pageshow")]
    PageShow,
    [EnumRecordProperties("popstate")]
    PopState,
    [EnumRecordProperties("rejectionhandled")]
    RejectionHandled,
    [EnumRecordProperties("storage")]
    Storage,
    [EnumRecordProperties("unhandledrejection")]
    UnhandledRejection,
    [EnumRecordProperties("unload")]
    Unload,


    [EnumRecordProperties("slotchange")]
    SlotChange,

}

