using EnumRecords;

namespace CssUI.Devices;

[EnumRecord<KeywordProperties>]
public enum EKeyboardCode : int
{/* Docs: https://www.w3.org/TR/uievents-code/#code-value-tables */

    /// <summary>
    /// This value code should be used when no other value given in this specification is appropriate.
    /// </summary>
    [EnumRecordProperties("")]
    Invalid = 0x0,

    /// <summary>
    /// `~ on a US keyboard. This is the 半角/全角/漢字 (hankaku/zenkaku/kanji) key on Japanese keyboards
    /// </summary>
    [EnumRecordProperties("Backquote", UnicodeCommon.CHAR_BACKTICK)]
    Backquote,

    /// <summary>
    /// Used for both the US \| (on the 101-key layout) and also for the key    located between the " and Enter keys on row C of the 102-, 104- and 106-key layouts. Labelled #~ on a UK (102) keyboard.
    /// </summary>
    [EnumRecordProperties("Backslash", UnicodeCommon.CHAR_REVERSE_SOLIDUS)]
    Backslash,

    /// <summary>
    /// Backspace or ⌫.	Labelled Delete on Apple keyboards.
    /// </summary>
    [EnumRecordProperties("Backspace", '⌫')]
    Backspace,

    /// <summary>
    /// [{ on a US keyboard.
    /// </summary>
    [EnumRecordProperties("BracketLeft", UnicodeCommon.CHAR_LEFT_SQUARE_BRACKET)]
    BracketLeft,

    /// <summary>
    /// ]} on a US keyboard.
    /// </summary>
    [EnumRecordProperties("BracketRight", UnicodeCommon.CHAR_RIGHT_SQUARE_BRACKET)]
    BracketRight,

    /// <summary>
    /// ,< on a US keyboard.
    /// </summary>
    [EnumRecordProperties("Comma", UnicodeCommon.CHAR_COMMA)]
    Comma,

    /// <summary>
    /// 0 ) on a US keyboard.
    /// </summary>
    [EnumRecordProperties("Digit0", UnicodeCommon.CHAR_DIGIT_0)]
    Digit0,

    /// <summary>
    /// 1 ! on a US keyboard.
    /// </summary>
    [EnumRecordProperties("Digit1", UnicodeCommon.CHAR_DIGIT_1)]
    Digit1,

    /// <summary>
    /// 2 @ on a US keyboard.
    /// </summary>
    [EnumRecordProperties("Digit2", UnicodeCommon.CHAR_DIGIT_2)]
    Digit2,

    /// <summary>
    /// 3 # on a US keyboard.
    /// </summary>
    [EnumRecordProperties("Digit3", UnicodeCommon.CHAR_DIGIT_3)]
    Digit3,

    /// <summary>
    /// 4 $ on a US keyboard.
    /// </summary>
    [EnumRecordProperties("Digit4", UnicodeCommon.CHAR_DIGIT_4)]
    Digit4,

    /// <summary>
    /// 5 % on a US keyboard.
    /// </summary>
    [EnumRecordProperties("Digit5", UnicodeCommon.CHAR_DIGIT_5)]
    Digit5,

    /// <summary>
    /// 6 ^ on a US keyboard.
    /// </summary>
    [EnumRecordProperties("Digit6", UnicodeCommon.CHAR_DIGIT_6)]
    Digit6,

    /// <summary>
    /// 7 & on a US keyboard.
    /// </summary>
    [EnumRecordProperties("Digit7", UnicodeCommon.CHAR_DIGIT_7)]
    Digit7,

    /// <summary>
    /// 8 * on a US keyboard.
    /// </summary>
    [EnumRecordProperties("Digit8", UnicodeCommon.CHAR_DIGIT_8)]
    Digit8,

    /// <summary>
    /// 9 ( on a US keyboard.
    /// </summary>
    [EnumRecordProperties("Digit9", UnicodeCommon.CHAR_DIGIT_9)]
    Digit9,

    /// <summary>
    /// = + on a US keyboard.
    /// </summary>
    [EnumRecordProperties("Equal", UnicodeCommon.CHAR_EQUALS)]
    Equal,

    /// <summary>
    /// Located between the left Shift and Z keys.	Labelled \| on a UK keyboard.
    /// </summary>
    [EnumRecordProperties("IntlBackslash", UnicodeCommon.CHAR_REVERSE_SOLIDUS)]
    IntlBackslash,

    /// <summary>
    /// Located between the / and right Shift keys.	Labelled \ (ro) on a Japanese keyboard.
    /// </summary>
    [EnumRecordProperties("IntlRo", 'ろ')]
    IntlRo,

    /// <summary>
    /// Located between the = and Backspace keys.	Labelled  (yen) on a Japanese keyboard. \/ on a Russian keyboard.
    /// </summary>
    [EnumRecordProperties("IntlYen", '¥')]
    IntlYen,

    /// <summary>
    /// a on a US keyboard.Labelled q on an AZERTY (e.g., French) keyboard.
    /// </summary>
    [EnumRecordProperties("KeyA", 'a')]
    KeyA,

    /// <summary>
    /// b on a US keyboard.
    /// </summary>
    [EnumRecordProperties("KeyB", 'b')]
    KeyB,

    /// <summary>
    /// c on a US keyboard.
    /// </summary>
    [EnumRecordProperties("KeyC", 'c')]
    KeyC,

    /// <summary>
    /// d on a US keyboard.
    /// </summary>
    [EnumRecordProperties("KeyD", 'd')]
    KeyD,

    /// <summary>
    /// e on a US keyboard.
    /// </summary>
    [EnumRecordProperties("KeyE", 'e')]
    KeyE,

    /// <summary>
    /// f on a US keyboard.
    /// </summary>
    [EnumRecordProperties("KeyF", 'f')]
    KeyF,

    /// <summary>
    /// g on a US keyboard.
    /// </summary>
    [EnumRecordProperties("KeyG", 'g')]
    KeyG,

    /// <summary>
    /// h on a US keyboard.
    /// </summary>
    [EnumRecordProperties("KeyH", 'h')]
    KeyH,

    /// <summary>
    /// i on a US keyboard.
    /// </summary>
    [EnumRecordProperties("KeyI", 'i')]
    KeyI,

    /// <summary>
    /// j on a US keyboard.
    /// </summary>
    [EnumRecordProperties("KeyJ", 'j')]
    KeyJ,

    /// <summary>
    /// k on a US keyboard.
    /// </summary>
    [EnumRecordProperties("KeyK", 'k')]
    KeyK,

    /// <summary>
    /// l on a US keyboard.
    /// </summary>
    [EnumRecordProperties("KeyL", 'l')]
    KeyL,

    /// <summary>
    /// m on a US keyboard.
    /// </summary>
    [EnumRecordProperties("KeyM", 'm')]
    KeyM,

    /// <summary>
    /// n on a US keyboard.
    /// </summary>
    [EnumRecordProperties("KeyN", 'n')]
    KeyN,

    /// <summary>
    /// o on a US keyboard.
    /// </summary>
    [EnumRecordProperties("KeyO", 'o')]
    KeyO,

    /// <summary>
    /// p on a US keyboard.
    /// </summary>
    [EnumRecordProperties("KeyP", 'p')]
    KeyP,

    /// <summary>
    /// q on a US keyboard.Labelled a on an AZERTY (e.g., French) keyboard.
    /// </summary>
    [EnumRecordProperties("KeyQ", 'q')]
    KeyQ,

    /// <summary>
    /// r on a US keyboard.
    /// </summary>
    [EnumRecordProperties("KeyR", 'r')]
    KeyR,

    /// <summary>
    /// s on a US keyboard.
    /// </summary>
    [EnumRecordProperties("KeyS", 's')]
    KeyS,

    /// <summary>
    /// t on a US keyboard.
    /// </summary>
    [EnumRecordProperties("KeyT", 't')]
    KeyT,

    /// <summary>
    /// u on a US keyboard.
    /// </summary>
    [EnumRecordProperties("KeyU", 'u')]
    KeyU,

    /// <summary>
    /// v on a US keyboard.
    /// </summary>
    [EnumRecordProperties("KeyV", 'v')]
    KeyV,

    /// <summary>
    /// w on a US keyboard.Labelled z on an AZERTY (e.g., French) keyboard.
    /// </summary>
    [EnumRecordProperties("KeyW", 'w')]
    KeyW,

    /// <summary>
    /// x on a US keyboard.
    /// </summary>
    [EnumRecordProperties("KeyX", 'x')]
    KeyX,

    /// <summary>
    /// y on a US keyboard.Labelled z on a QWERTZ (e.g., German) keyboard.
    /// </summary>
    [EnumRecordProperties("KeyY", 'y')]
    KeyY,

    /// <summary>
    /// z on a US keyboard.Labelled w on an AZERTY (e.g., French) keyboard, and y on a QWERTZ (e.g., German) keyboard.
    /// </summary>
    [EnumRecordProperties("KeyZ", 'z')]
    KeyZ,

    /// <summary>
    /// -_ on a US keyboard.
    /// </summary>
    [EnumRecordProperties("Minus", UnicodeCommon.CHAR_HYPHEN_MINUS)]
    Minus,

    /// <summary>
    /// .> on a US keyboard.
    /// </summary>
    [EnumRecordProperties("Period", UnicodeCommon.CHAR_FULL_STOP)]
    Period,

    /// <summary>
    /// '" on a US keyboard.
    /// </summary>
    [EnumRecordProperties("Quote", UnicodeCommon.CHAR_APOSTRAPHE)]
    Quote,

    /// <summary>
    /// ;: on a US keyboard.
    /// </summary>
    [EnumRecordProperties("Semicolon", UnicodeCommon.CHAR_SEMICOLON)]
    Semicolon,

    /// <summary>
    /// /? on a US keyboard.
    /// </summary>
    [EnumRecordProperties("Slash", UnicodeCommon.CHAR_SOLIDUS)]
    Slash,

    /// <summary>
    /// Alt, Option or ⌥.
    /// </summary>
    [EnumRecordProperties("AltLeft", UnicodeCommon.KEY_ALT_MODIFIER)]
    AltLeft,

    /// <summary>
    /// Alt, Option or ⌥.	This is labelled AltGr key on many keyboard layouts.
    /// </summary>
    [EnumRecordProperties("AltRight", UnicodeCommon.KEY_ALT_MODIFIER)]
    AltRight,

    /// <summary>
    /// CapsLock or ⇪
    /// </summary>
    [EnumRecordProperties("CapsLock", UnicodeCommon.KEY_CAPSLOCK)]
    CapsLock,

    /// <summary>
    /// The application context menu key, which is typically found between the right Meta key and the right Control key.
    /// </summary>
    [EnumRecordProperties("ContextMenu")]
    ContextMenu,

    /// <summary>
    /// Control or ⌃
    /// </summary>
    [EnumRecordProperties("ControlLeft", UnicodeCommon.KEY_CTRL_MODIFIER)]
    ControlLeft,

    /// <summary>
    /// Control or ⌃
    /// </summary>
    [EnumRecordProperties("ControlRight", UnicodeCommon.KEY_CTRL_MODIFIER)]
    ControlRight,

    /// <summary>
    /// Enter or ↵. Labelled Return on Apple keyboards.
    /// </summary>
    [EnumRecordProperties("Enter", UnicodeCommon.KEY_ENTER)]
    Enter,

    /// <summary>
    /// The Windows, ⌘, Command or other OS symbol key.
    /// </summary>
    [EnumRecordProperties("MetaLeft", UnicodeCommon.KEY_META_MODIFIER)]
    MetaLeft,

    /// <summary>
    /// The Windows, ⌘, Command or other OS symbol key.
    /// </summary>
    [EnumRecordProperties("MetaRight", UnicodeCommon.KEY_META_MODIFIER)]
    MetaRight,

    /// <summary>
    /// Shift or ⇧
    /// </summary>
    [EnumRecordProperties("ShiftLeft", UnicodeCommon.KEY_SHIFT_MODIFIER)]
    ShiftLeft,

    /// <summary>
    /// Shift or ⇧
    /// </summary>
    [EnumRecordProperties("ShiftRight", UnicodeCommon.KEY_SHIFT_MODIFIER)]
    ShiftRight,

    /// <summary>
    /// (space)
    /// </summary>
    [EnumRecordProperties("Space", UnicodeCommon.KEY_SPACE)]
    Space,

    /// <summary>
    /// Tab or ⇥
    /// </summary>
    [EnumRecordProperties("Tab", UnicodeCommon.KEY_TAB)]
    Tab,

    /// <summary>
    /// Japanese: 変換 (henkan)
    /// </summary>
    [EnumRecordProperties("Convert")]
    Convert,

    /// <summary>
    /// Japanese: カタカナ/ひらがな/ローマ字 (katakana/hiragana/romaji)
    /// </summary>
    [EnumRecordProperties("KanaMode")]
    KanaMode,

    /// <summary>
    /// Korean: HangulMode 한/영 (han/yeong) Japanese (Mac keyboard): かな (kana)
    /// </summary>
    [EnumRecordProperties("Lang1", '한', '영')]
    Lang1,

    /// <summary>
    /// Korean: Hanja 한자 (hanja) Japanese (Mac keyboard): 英数 (eisu)
    /// </summary>
    [EnumRecordProperties("Lang2")]
    Lang2,

    /// <summary>
    /// Japanese (word-processing keyboard): Katakana
    /// </summary>
    [EnumRecordProperties("Lang3")]
    Lang3,

    /// <summary>
    /// Japanese (word-processing keyboard): Hiragana
    /// </summary>
    [EnumRecordProperties("Lang4")]
    Lang4,

    /// <summary>
    /// Japanese (word-processing keyboard): Zenkaku/Hankaku
    /// </summary>
    [EnumRecordProperties("Lang5")]
    Lang5,

    /// <summary>
    /// Japanese: 無変換 (muhenkan)
    /// </summary>
    [EnumRecordProperties("NonConvert")]
    NonConvert,


    /* Control Pad */
    /// <summary>
    /// ⌦. The forward delete key.	Note that on Apple keyboards, the key labelled Delete on the main part of the keyboard should be encoded as "Backspace".
    /// </summary>
    [EnumRecordProperties("Delete", UnicodeCommon.KEY_DELETE)]
    Delete,

    /// <summary>
    /// Page Down, End or ↘
    /// </summary>
    [EnumRecordProperties("End", UnicodeCommon.KEY_END)]
    End,

    /// <summary>
    /// Help. Not present on standard PC keyboards.
    /// </summary>
    [EnumRecordProperties("Help")]
    Help,

    /// <summary>
    /// Home or ↖
    /// </summary>
    [EnumRecordProperties("Home", UnicodeCommon.KEY_HOME)]
    Home,

    /// <summary>
    /// Insert or Ins. Not present on Apple keyboards.
    /// </summary>
    [EnumRecordProperties("Insert")]
    Insert,

    /// <summary>
    /// Page Down, PgDn or ⇟
    /// </summary>
    [EnumRecordProperties("PageDown", UnicodeCommon.KEY_PGDOWN)]
    PageDown,

    /// <summary>
    /// Page Up, PgUp or ⇞
    /// </summary>
    [EnumRecordProperties("PageUp", UnicodeCommon.KEY_PGUP)]
    PageUp,



    /* Arrow Pad */

    /// <summary>
    /// ↓
    /// </summary>
    [EnumRecordProperties("ArrowDown", UnicodeCommon.KEY_DOWN)]
    ArrowDown,

    /// <summary>
    /// ←
    /// </summary>
    [EnumRecordProperties("ArrowLeft", UnicodeCommon.KEY_LEFT)]
    ArrowLeft,

    /// <summary>
    /// →
    /// </summary>
    [EnumRecordProperties("ArrowRight", UnicodeCommon.KEY_RIGHT)]
    ArrowRight,

    /// <summary>
    /// ↑
    /// </summary>
    [EnumRecordProperties("ArrowUp", UnicodeCommon.KEY_UP)]
    ArrowUp,



    /* Numpad Section */


    /// <summary>
    /// On the Mac, the "NumLock" code should be used for the numpad Clear key.
    /// </summary>
    [EnumRecordProperties("NumLock")]
    NumLock,

    /// <summary>
    /// 0 Ins on a keyboard 0 on a phone or remote control
    /// </summary>
    [EnumRecordProperties("Numpad0", UnicodeCommon.CHAR_DIGIT_0)]
    Numpad0,

    /// <summary>
    /// 1 End on a keyboard 1 or 1 QZ on a phone or remote control
    /// </summary>
    [EnumRecordProperties("Numpad1", UnicodeCommon.CHAR_DIGIT_1)]
    Numpad1,

    /// <summary>
    /// 2 ↓ on a keyboard 2 ABC on a phone or remote control
    /// </summary>
    [EnumRecordProperties("Numpad2", UnicodeCommon.CHAR_DIGIT_2)]
    Numpad2,

    /// <summary>
    /// 3 PgDn on a keyboard 3 DEF on a phone or remote control
    /// </summary>
    [EnumRecordProperties("Numpad3", UnicodeCommon.CHAR_DIGIT_3)]
    Numpad3,

    /// <summary>
    /// 4 ← on a keyboard 4 GHI on a phone or remote control
    /// </summary>
    [EnumRecordProperties("Numpad4", UnicodeCommon.CHAR_DIGIT_4)]
    Numpad4,

    /// <summary>
    /// 5 on a keyboard 5 JKL on a phone or remote control
    /// </summary>
    [EnumRecordProperties("Numpad5", UnicodeCommon.CHAR_DIGIT_5)]
    Numpad5,

    /// <summary>
    /// 6 → on a keyboard 6 MNO on a phone or remote control
    /// </summary>
    [EnumRecordProperties("Numpad6", UnicodeCommon.CHAR_DIGIT_6)]
    Numpad6,

    /// <summary>
    /// 7 Home on a keyboard 7 PQRS or 7 PRS on a phone  or remote control
    /// </summary>
    [EnumRecordProperties("Numpad7", UnicodeCommon.CHAR_DIGIT_7)]
    Numpad7,

    /// <summary>
    /// 8 ↑ on a keyboard 8 TUV on a phone or remote control
    /// </summary>
    [EnumRecordProperties("Numpad8", UnicodeCommon.CHAR_DIGIT_8)]
    Numpad8,

    /// <summary>
    /// 9 PgUp on a keyboard 9 WXYZ or 9 WXY on a phone  or remote control
    /// </summary>
    [EnumRecordProperties("Numpad9", UnicodeCommon.CHAR_DIGIT_9)]
    Numpad9,

    /// <summary>
    /// +
    /// </summary>
    [EnumRecordProperties("NumpadAdd", UnicodeCommon.CHAR_PLUS_SIGN)]
    NumpadAdd,

    /// <summary>
    /// Found on the Microsoft Natural Keyboard.
    /// </summary>
    [EnumRecordProperties("NumpadBackspace")]
    NumpadBackspace,

    /// <summary>
    /// C or AC (All Clear). Also for use with numpads that have a Clear key that is separate from the NumLock key. On the Mac, the numpad Clear key should always be encoded as "NumLock".
    /// </summary>
    [EnumRecordProperties("NumpadClear")]
    NumpadClear,

    /// <summary>
    /// CE (Clear Entry)
    /// </summary>
    [EnumRecordProperties("NumpadClearEntry")]
    NumpadClearEntry,

    /// <summary>
    /// , (thousands separator). For locales where the thousands separator	is a "." (e.g., Brazil), this key may generate a ..
    /// </summary>
    [EnumRecordProperties("NumpadComma")]
    NumpadComma,

    /// <summary>
    /// . Del. For locales where the decimal separator is "," (e.g., Brazil), this key may generate a,.
    /// </summary>
    [EnumRecordProperties("NumpadDecimal")]
    NumpadDecimal,

    /// <summary>
    /// /
    /// </summary>
    [EnumRecordProperties("NumpadDivide")]
    NumpadDivide,
    /// <summary>
    /// Newline
    /// </summary>
    [EnumRecordProperties("NumpadEnter")]
    NumpadEnter,

    /// <summary>
    /// =
    /// </summary>
    [EnumRecordProperties("NumpadEqual")]
    NumpadEqual,

    /// <summary>
    /// # on a phone or remote control device. This key is typically found	below the 9 key and to the right of the 0 key.
    /// </summary>
    [EnumRecordProperties("NumpadHash")]
    NumpadHash,

    /// <summary>
    /// M+ Add current entry to the value stored in memory.
    /// </summary>
    [EnumRecordProperties("NumpadMemoryAdd")]
    NumpadMemoryAdd,

    /// <summary>
    /// MC Clear the value stored in memory.
    /// </summary>
    [EnumRecordProperties("NumpadMemoryClear")]
    NumpadMemoryClear,

    /// <summary>
    /// MR Replace the current entry with the value stored in memory.
    /// </summary>
    [EnumRecordProperties("NumpadMemoryRecall")]
    NumpadMemoryRecall,

    /// <summary>
    /// MS Replace the value stored in memory with the current entry.
    /// </summary>
    [EnumRecordProperties("NumpadMemoryStore")]
    NumpadMemoryStore,

    /// <summary>
    /// M- Subtract current entry from the value stored in memory.
    /// </summary>
    [EnumRecordProperties("NumpadMemorySubtract")]
    NumpadMemorySubtract,

    /// <summary>
    /// * on a keyboard. For use with numpads that provide mathematical operations (+, -, * and /). Use "NumpadStar" for the * key on phones and remote controls.
    /// </summary>
    [EnumRecordProperties("NumpadMultiply")]
    NumpadMultiply,

    /// <summary>
    /// ( Found on the Microsoft Natural Keyboard.
    /// </summary>
    [EnumRecordProperties("NumpadParenLeft")]
    NumpadParenLeft,

    /// <summary>
    /// ) Found on the Microsoft Natural Keyboard.
    /// </summary>
    [EnumRecordProperties("NumpadParenRight")]
    NumpadParenRight,

    /// <summary>
    /// * on a phone or remote control device.	This key is typically found below the 7 key and to the left of the 0 key. Use "NumpadMultiply" for the * key on numeric keypads.
    /// </summary>
    [EnumRecordProperties("NumpadStar")]
    NumpadStar,

    /// <summary>
    /// -
    /// </summary>
    [EnumRecordProperties("NumpadSubtract")]
    NumpadSubtract,



    /* Function Section */


    /// <summary>
    /// Esc or ⎋
    /// </summary>
    [EnumRecordProperties("Escape", UnicodeCommon.KEY_ESCAPE)]
    Escape,

    /// <summary>
    /// F1
    /// </summary>
    [EnumRecordProperties("F1")]
    F1,

    /// <summary>
    /// F2
    /// </summary>
    [EnumRecordProperties("F2")]
    F2,

    /// <summary>
    /// F3
    /// </summary>
    [EnumRecordProperties("F3")]
    F3,

    /// <summary>
    /// F4
    /// </summary>
    [EnumRecordProperties("F4")]
    F4,

    /// <summary>
    /// F5
    /// </summary>
    [EnumRecordProperties("F5")]
    F5,

    /// <summary>
    /// F6
    /// </summary>
    [EnumRecordProperties("F6")]
    F6,

    /// <summary>
    /// F7
    /// </summary>
    [EnumRecordProperties("F7")]
    F7,

    /// <summary>
    /// F8
    /// </summary>
    [EnumRecordProperties("F8")]
    F8,

    /// <summary>
    /// F9
    /// </summary>
    [EnumRecordProperties("F9")]
    F9,

    /// <summary>
    /// F10
    /// </summary>
    [EnumRecordProperties("F10")]
    F10,

    /// <summary>
    /// F11
    /// </summary>
    [EnumRecordProperties("F11")]
    F11,

    /// <summary>
    /// F12
    /// </summary>
    [EnumRecordProperties("F12")]
    F12,

    /// <summary>
    /// Fn This is typically a hardware key that does not generate a separate   code. Most keyboards do not place this key in the function section, but it is included here to keep it with related keys.
    /// </summary>
    [EnumRecordProperties("Fn")]
    Fn,

    /// <summary>
    /// FLock or FnLock. Function Lock key. Found on the Microsoft  Natural Keyboard.
    /// </summary>
    [EnumRecordProperties("FnLock")]
    FnLock,

    /// <summary>
    /// PrtScr SysRq or Print Screen
    /// </summary>
    [EnumRecordProperties("PrintScreen")]
    PrintScreen,

    /// <summary>
    /// Scroll Lock
    /// </summary>
    [EnumRecordProperties("ScrollLock")]
    ScrollLock,

    /// <summary>
    /// Pause Break
    /// </summary>
    [EnumRecordProperties("Pause")]
    Pause,



    /* Media Keys */


    /// <summary>
    /// Some laptops place this key to the left of the ↑ key.
    /// </summary>
    [EnumRecordProperties("BrowserBack")]
    BrowserBack,
    /// <summary>
    /// No definition
    /// </summary>
    [EnumRecordProperties("BrowserFavorites")]
    BrowserFavorites,

    /// <summary>
    /// Some laptops place this key to the right of the ↑ key.
    /// </summary>
    [EnumRecordProperties("BrowserForward")]
    BrowserForward,

    /// <summary>
    ///
    /// </summary>
    [EnumRecordProperties("BrowserHome")]
    BrowserHome,

    /// <summary>
    ///
    /// </summary>
    [EnumRecordProperties("BrowserRefresh")]
    BrowserRefresh,

    /// <summary>
    ///
    /// </summary>
    [EnumRecordProperties("BrowserSearch")]
    BrowserSearch,

    /// <summary>
    ///
    /// </summary>
    [EnumRecordProperties("BrowserStop")]
    BrowserStop,

    /// <summary>
    /// Eject or ⏏. This key is placed in the function  section on some Apple keyboards.
    /// </summary>
    [EnumRecordProperties("Eject")]
    Eject,

    /// <summary>
    /// Sometimes labelled My Computer on the keyboard
    /// </summary>
    [EnumRecordProperties("LaunchApp1")]
    LaunchApp1,

    /// <summary>
    /// Sometimes labelled Calculator on the keyboard
    /// </summary>
    [EnumRecordProperties("LaunchApp2")]
    LaunchApp2,

    /// <summary>
    ///
    /// </summary>
    [EnumRecordProperties("LaunchMail")]
    LaunchMail,

    /// <summary>
    ///
    /// </summary>
    [EnumRecordProperties("MediaPlayPause")]
    MediaPlayPause,

    /// <summary>
    ///
    /// </summary>
    [EnumRecordProperties("MediaSelect")]
    MediaSelect,

    /// <summary>
    ///
    /// </summary>
    [EnumRecordProperties("MediaStop")]
    MediaStop,

    /// <summary>
    ///
    /// </summary>
    [EnumRecordProperties("MediaTrackNext")]
    MediaTrackNext,

    /// <summary>
    ///
    /// </summary>
    [EnumRecordProperties("MediaTrackPrevious")]
    MediaTrackPrevious,

    /// <summary>
    /// This key is placed in the function section on some Apple keyboards, replacing the Eject key.
    /// </summary>
    [EnumRecordProperties("Power")]
    Power,

    /// <summary>
    ///
    /// </summary>
    [EnumRecordProperties("Sleep")]
    Sleep,

    /// <summary>
    ///
    /// </summary>
    [EnumRecordProperties("AudioVolumeDown")]
    AudioVolumeDown,

    /// <summary>
    ///
    /// </summary>
    [EnumRecordProperties("AudioVolumeMute")]
    AudioVolumeMute,

    /// <summary>
    ///
    /// </summary>
    [EnumRecordProperties("AudioVolumeUp")]
    AudioVolumeUp,

    /// <summary>
    ///
    /// </summary>
    [EnumRecordProperties("WakeUp")]
    WakeUp,


    MAX// Tracks the end of the available enum values
}

