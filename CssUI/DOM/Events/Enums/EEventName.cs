using EnumRecords;

namespace CssUI.DOM.Events;

[EnumRecord<KeywordProperties>]
public enum EEventName : int
{
    /// <summary>
    /// Represents all custom (end-user specified) event values
    /// </summary>
    CUSTOM = -1,
    [EnumData("")]
    None = 0x0,
    [EnumData("abort")]
    Abort,
    [EnumData("auxclick")]
    AuxClick,
    [EnumData("blur")]
    Blur,
    [EnumData("cancel")]
    Cancel,
    [EnumData("canplay")]
    CanPlay,
    [EnumData("canplaythrough")]
    CanPlayThrough,
    [EnumData("change")]
    Change,
    [EnumData("click")]
    Click,
    [EnumData("close")]
    Close,
    [EnumData("contextmenu")]
    ContextMenu,
    [EnumData("cuechange")]
    CueChange,
    [EnumData("dblclick")]
    DoubleClick,

    /* Drag drop */
    [EnumData("drag")]
    Drag,
    [EnumData("dragend")]
    DragEnd,
    [EnumData("dragenter")]
    DragEnter,
    [EnumData("dragexit")]
    DragExit,
    [EnumData("dragleave")]
    DragLeave,
    [EnumData("dragover")]
    DragOver,
    [EnumData("dragstart")]
    DragStart,
    [EnumData("drop")]
    Drop,

    [EnumData("durationchange")]
    DurationChange,
    [EnumData("emptied")]
    Emptied,
    [EnumData("ended")]
    Ended,

    [EnumData("error")]
    Error,

    [EnumData("focus")]
    Focus,
    [EnumData("formdata")]
    FormData,
    [EnumData("input")]
    Input,
    [EnumData("invalid")]
    Invalid,
    [EnumData("keydown")]
    KeyDown,
    [EnumData("keypress")]
    KeyPress,
    [EnumData("keyup")]
    KeyUp,
    [EnumData("load")]
    Load,
    [EnumData("loadeddata")]
    LoadedData,
    [EnumData("loadedmetadata")]
    LoadedMetadata,
    [EnumData("loadend")]
    LoadEnd,
    [EnumData("loadstart")]
    LoadStart,

    /* Mouse input */
    [EnumData("mousedown")]
    MouseDown,
    [EnumData("mouseenter")]
    MouseEnter,
    [EnumData("mouseleave")]
    MouseLeave,
    [EnumData("mousemove")]
    MouseMove,
    [EnumData("mouseout")]
    MouseOut,
    [EnumData("mouseover")]
    MouseOver,
    [EnumData("mouseup")]
    MouseUp,
    [EnumData("wheel")]
    Wheel,

    /* Pointer input */
    /* Docs: https://w3c.github.io/pointerevents/#intro */
    [EnumData("pointerover")]
    PointerOver,
    [EnumData("pointerdown")]
    PointerDown,
    [EnumData("pointermove")]
    PointerMove,
    [EnumData("pointerup")]
    PointerUp,
    [EnumData("pointercancel")]
    PointerCancel,
    [EnumData("pointerout")]
    PointerOut,
    [EnumData("pointerleave")]
    PointerLeave,
    [EnumData("gotpointercapture")]
    GotPointerCapture,
    [EnumData("lostpointercapture")]
    LostPointerCapture,

    /* Touch input */
    /* Docs: https://w3c.github.io/touch-events/#introduction */
    [EnumData("touchstart")]
    TouchStart,
    [EnumData("touchend")]
    TouchEnd,
    [EnumData("touchmove")]
    TouchMove,
    [EnumData("touchcancel")]
    TouchCancel,


    /* Media */
    [EnumData("pause")]
    Pause,
    [EnumData("play")]
    Play,
    [EnumData("playing")]
    Playing,
    [EnumData("progress")]
    Progress,
    [EnumData("ratechange")]
    RateChange,
    [EnumData("reset")]
    Reset,
    [EnumData("resize")]
    Resize,
    [EnumData("scroll")]
    Scroll,
    [EnumData("securitypolicyviolation")]
    SecurityPolicyViolation,
    [EnumData("seeked")]
    Seeked,
    [EnumData("seeking")]
    Seeking,
    [EnumData("select")]
    Select,
    [EnumData("stalled")]
    Stalled,
    [EnumData("submit")]
    Submit,
    [EnumData("suspend")]
    Suspend,
    [EnumData("timeupdate")]
    TimeUpdate,
    [EnumData("toggle")]
    Toggle,
    [EnumData("volumechange")]
    VolumeChange,
    [EnumData("waiting")]
    Waiting,

    [EnumData("selectstart")]
    SelectStart,
    [EnumData("selectionchange")]
    SelectionChange,

    [EnumData("copy")]
    Copy,
    [EnumData("cut")]
    Cut,
    [EnumData("paste")]
    Paste,

    /* Window events */
    [EnumData("afterprint")]
    AfterPrint,
    [EnumData("beforeprint")]
    BeforePrint,
    [EnumData("beforeunload")]
    BeforeUnload,
    [EnumData("hashchange")]
    HashChange,
    [EnumData("languagechange")]
    LanguageChange,
    [EnumData("message")]
    Message,
    [EnumData("messageerror")]
    MessageError,
    [EnumData("offline")]
    Offline,
    [EnumData("online")]
    Online,
    [EnumData("pagehide")]
    PageHide,
    [EnumData("pageshow")]
    PageShow,
    [EnumData("popstate")]
    PopState,
    [EnumData("rejectionhandled")]
    RejectionHandled,
    [EnumData("storage")]
    Storage,
    [EnumData("unhandledrejection")]
    UnhandledRejection,
    [EnumData("unload")]
    Unload,


    [EnumData("slotchange")]
    SlotChange,

}

