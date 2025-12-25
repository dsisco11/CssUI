using EnumRecords;

namespace CssUI.Devices;

[EnumRecord<KeywordProperties>]
public enum EKeyboardCode : int
{/* Docs: https://www.w3.org/TR/uievents-code/#code-value-tables */

    /// <summary>
    /// This value code should be used when no other value given in this specification is appropriate.
    /// </summary>
    [EnumData("")]
    Invalid = 0x0,

    /// <summary>
    /// `~ on a US keyboard. This is the 半角/全角/漢字 (hankaku/zenkaku/kanji) key on Japanese keyboards
    /// </summary>
    [EnumData("Backquote", UnicodeCommon.CHAR_BACKTICK)]
    Backquote,

    /// <summary>
    /// Used for both the US \| (on the 101-key layout) and also for the key    located between the " and Enter keys on row C of the 102-, 104- and 106-key layouts. Labelled #~ on a UK (102) keyboard.
    /// </summary>
    [EnumData("Backslash", UnicodeCommon.CHAR_REVERSE_SOLIDUS)]
    Backslash,

    /// <summary>
    /// Backspace or ⌫.	Labelled Delete on Apple keyboards.
    /// </summary>
    [EnumData("Backspace", '⌫')]
    Backspace,

    /// <summary>
    /// [{ on a US keyboard.
    /// </summary>
    [EnumData("BracketLeft", UnicodeCommon.CHAR_LEFT_SQUARE_BRACKET)]
    BracketLeft,

    /// <summary>
    /// ]} on a US keyboard.
    /// </summary>
    [EnumData("BracketRight", UnicodeCommon.CHAR_RIGHT_SQUARE_BRACKET)]
    BracketRight,

    /// <summary>
    /// ,< on a US keyboard.
    /// </summary>
    [EnumData("Comma", UnicodeCommon.CHAR_COMMA)]
    Comma,

    /// <summary>
    /// 0 ) on a US keyboard.
    /// </summary>
    [EnumData("Digit0", UnicodeCommon.CHAR_DIGIT_0)]
    Digit0,

    /// <summary>
    /// 1 ! on a US keyboard.
    /// </summary>
    [EnumData("Digit1", UnicodeCommon.CHAR_DIGIT_1)]
    Digit1,

    /// <summary>
    /// 2 @ on a US keyboard.
    /// </summary>
    [EnumData("Digit2", UnicodeCommon.CHAR_DIGIT_2)]
    Digit2,

    /// <summary>
    /// 3 # on a US keyboard.
    /// </summary>
    [EnumData("Digit3", UnicodeCommon.CHAR_DIGIT_3)]
    Digit3,

    /// <summary>
    /// 4 $ on a US keyboard.
    /// </summary>
    [EnumData("Digit4", UnicodeCommon.CHAR_DIGIT_4)]
    Digit4,

    /// <summary>
    /// 5 % on a US keyboard.
    /// </summary>
    [EnumData("Digit5", UnicodeCommon.CHAR_DIGIT_5)]
    Digit5,

    /// <summary>
    /// 6 ^ on a US keyboard.
    /// </summary>
    [EnumData("Digit6", UnicodeCommon.CHAR_DIGIT_6)]
    Digit6,

    /// <summary>
    /// 7 & on a US keyboard.
    /// </summary>
    [EnumData("Digit7", UnicodeCommon.CHAR_DIGIT_7)]
    Digit7,

    /// <summary>
    /// 8 * on a US keyboard.
    /// </summary>
    [EnumData("Digit8", UnicodeCommon.CHAR_DIGIT_8)]
    Digit8,

    /// <summary>
    /// 9 ( on a US keyboard.
    /// </summary>
    [EnumData("Digit9", UnicodeCommon.CHAR_DIGIT_9)]
    Digit9,

    /// <summary>
    /// = + on a US keyboard.
    /// </summary>
    [EnumData("Equal", UnicodeCommon.CHAR_EQUALS)]
    Equal,

    /// <summary>
    /// Located between the left Shift and Z keys.	Labelled \| on a UK keyboard.
    /// </summary>
    [EnumData("IntlBackslash", UnicodeCommon.CHAR_REVERSE_SOLIDUS)]
    IntlBackslash,

    /// <summary>
    /// Located between the / and right Shift keys.	Labelled \ (ro) on a Japanese keyboard.
    /// </summary>
    [EnumData("IntlRo", 'ろ')]
    IntlRo,

    /// <summary>
    /// Located between the = and Backspace keys.	Labelled  (yen) on a Japanese keyboard. \/ on a Russian keyboard.
    /// </summary>
    [EnumData("IntlYen", '¥')]
    IntlYen,

    /// <summary>
    /// a on a US keyboard.Labelled q on an AZERTY (e.g., French) keyboard.
    /// </summary>
    [EnumData("KeyA", 'a')]
    KeyA,

    /// <summary>
    /// b on a US keyboard.
    /// </summary>
    [EnumData("KeyB", 'b')]
    KeyB,

    /// <summary>
    /// c on a US keyboard.
    /// </summary>
    [EnumData("KeyC", 'c')]
    KeyC,

    /// <summary>
    /// d on a US keyboard.
    /// </summary>
    [EnumData("KeyD", 'd')]
    KeyD,

    /// <summary>
    /// e on a US keyboard.
    /// </summary>
    [EnumData("KeyE", 'e')]
    KeyE,

    /// <summary>
    /// f on a US keyboard.
    /// </summary>
    [EnumData("KeyF", 'f')]
    KeyF,

    /// <summary>
    /// g on a US keyboard.
    /// </summary>
    [EnumData("KeyG", 'g')]
    KeyG,

    /// <summary>
    /// h on a US keyboard.
    /// </summary>
    [EnumData("KeyH", 'h')]
    KeyH,

    /// <summary>
    /// i on a US keyboard.
    /// </summary>
    [EnumData("KeyI", 'i')]
    KeyI,

    /// <summary>
    /// j on a US keyboard.
    /// </summary>
    [EnumData("KeyJ", 'j')]
    KeyJ,

    /// <summary>
    /// k on a US keyboard.
    /// </summary>
    [EnumData("KeyK", 'k')]
    KeyK,

    /// <summary>
    /// l on a US keyboard.
    /// </summary>
    [EnumData("KeyL", 'l')]
    KeyL,

    /// <summary>
    /// m on a US keyboard.
    /// </summary>
    [EnumData("KeyM", 'm')]
    KeyM,

    /// <summary>
    /// n on a US keyboard.
    /// </summary>
    [EnumData("KeyN", 'n')]
    KeyN,

    /// <summary>
    /// o on a US keyboard.
    /// </summary>
    [EnumData("KeyO", 'o')]
    KeyO,

    /// <summary>
    /// p on a US keyboard.
    /// </summary>
    [EnumData("KeyP", 'p')]
    KeyP,

    /// <summary>
    /// q on a US keyboard.Labelled a on an AZERTY (e.g., French) keyboard.
    /// </summary>
    [EnumData("KeyQ", 'q')]
    KeyQ,

    /// <summary>
    /// r on a US keyboard.
    /// </summary>
    [EnumData("KeyR", 'r')]
    KeyR,

    /// <summary>
    /// s on a US keyboard.
    /// </summary>
    [EnumData("KeyS", 's')]
    KeyS,

    /// <summary>
    /// t on a US keyboard.
    /// </summary>
    [EnumData("KeyT", 't')]
    KeyT,

    /// <summary>
    /// u on a US keyboard.
    /// </summary>
    [EnumData("KeyU", 'u')]
    KeyU,

    /// <summary>
    /// v on a US keyboard.
    /// </summary>
    [EnumData("KeyV", 'v')]
    KeyV,

    /// <summary>
    /// w on a US keyboard.Labelled z on an AZERTY (e.g., French) keyboard.
    /// </summary>
    [EnumData("KeyW", 'w')]
    KeyW,

    /// <summary>
    /// x on a US keyboard.
    /// </summary>
    [EnumData("KeyX", 'x')]
    KeyX,

    /// <summary>
    /// y on a US keyboard.Labelled z on a QWERTZ (e.g., German) keyboard.
    /// </summary>
    [EnumData("KeyY", 'y')]
    KeyY,

    /// <summary>
    /// z on a US keyboard.Labelled w on an AZERTY (e.g., French) keyboard, and y on a QWERTZ (e.g., German) keyboard.
    /// </summary>
    [EnumData("KeyZ", 'z')]
    KeyZ,

    /// <summary>
    /// -_ on a US keyboard.
    /// </summary>
    [EnumData("Minus", UnicodeCommon.CHAR_HYPHEN_MINUS)]
    Minus,

    /// <summary>
    /// .> on a US keyboard.
    /// </summary>
    [EnumData("Period", UnicodeCommon.CHAR_FULL_STOP)]
    Period,

    /// <summary>
    /// '" on a US keyboard.
    /// </summary>
    [EnumData("Quote", UnicodeCommon.CHAR_APOSTRAPHE)]
    Quote,

    /// <summary>
    /// ;: on a US keyboard.
    /// </summary>
    [EnumData("Semicolon", UnicodeCommon.CHAR_SEMICOLON)]
    Semicolon,

    /// <summary>
    /// /? on a US keyboard.
    /// </summary>
    [EnumData("Slash", UnicodeCommon.CHAR_SOLIDUS)]
    Slash,

    /// <summary>
    /// Alt, Option or ⌥.
    /// </summary>
    [EnumData("AltLeft", UnicodeCommon.KEY_ALT_MODIFIER)]
    AltLeft,

    /// <summary>
    /// Alt, Option or ⌥.	This is labelled AltGr key on many keyboard layouts.
    /// </summary>
    [EnumData("AltRight", UnicodeCommon.KEY_ALT_MODIFIER)]
    AltRight,

    /// <summary>
    /// CapsLock or ⇪
    /// </summary>
    [EnumData("CapsLock", UnicodeCommon.KEY_CAPSLOCK)]
    CapsLock,

    /// <summary>
    /// The application context menu key, which is typically found between the right Meta key and the right Control key.
    /// </summary>
    [EnumData("ContextMenu")]
    ContextMenu,

    /// <summary>
    /// Control or ⌃
    /// </summary>
    [EnumData("ControlLeft", UnicodeCommon.KEY_CTRL_MODIFIER)]
    ControlLeft,

    /// <summary>
    /// Control or ⌃
    /// </summary>
    [EnumData("ControlRight", UnicodeCommon.KEY_CTRL_MODIFIER)]
    ControlRight,

    /// <summary>
    /// Enter or ↵. Labelled Return on Apple keyboards.
    /// </summary>
    [EnumData("Enter", UnicodeCommon.KEY_ENTER)]
    Enter,

    /// <summary>
    /// The Windows, ⌘, Command or other OS symbol key.
    /// </summary>
    [EnumData("MetaLeft", UnicodeCommon.KEY_META_MODIFIER)]
    MetaLeft,

    /// <summary>
    /// The Windows, ⌘, Command or other OS symbol key.
    /// </summary>
    [EnumData("MetaRight", UnicodeCommon.KEY_META_MODIFIER)]
    MetaRight,

    /// <summary>
    /// Shift or ⇧
    /// </summary>
    [EnumData("ShiftLeft", UnicodeCommon.KEY_SHIFT_MODIFIER)]
    ShiftLeft,

    /// <summary>
    /// Shift or ⇧
    /// </summary>
    [EnumData("ShiftRight", UnicodeCommon.KEY_SHIFT_MODIFIER)]
    ShiftRight,

    /// <summary>
    /// (space)
    /// </summary>
    [EnumData("Space", UnicodeCommon.KEY_SPACE)]
    Space,

    /// <summary>
    /// Tab or ⇥
    /// </summary>
    [EnumData("Tab", UnicodeCommon.KEY_TAB)]
    Tab,

    /// <summary>
    /// Japanese: 変換 (henkan)
    /// </summary>
    [EnumData("Convert")]
    Convert,

    /// <summary>
    /// Japanese: カタカナ/ひらがな/ローマ字 (katakana/hiragana/romaji)
    /// </summary>
    [EnumData("KanaMode")]
    KanaMode,

    /// <summary>
    /// Korean: HangulMode 한/영 (han/yeong) Japanese (Mac keyboard): かな (kana)
    /// </summary>
    [EnumData("Lang1", '한', '영')]
    Lang1,

    /// <summary>
    /// Korean: Hanja 한자 (hanja) Japanese (Mac keyboard): 英数 (eisu)
    /// </summary>
    [EnumData("Lang2")]
    Lang2,

    /// <summary>
    /// Japanese (word-processing keyboard): Katakana
    /// </summary>
    [EnumData("Lang3")]
    Lang3,

    /// <summary>
    /// Japanese (word-processing keyboard): Hiragana
    /// </summary>
    [EnumData("Lang4")]
    Lang4,

    /// <summary>
    /// Japanese (word-processing keyboard): Zenkaku/Hankaku
    /// </summary>
    [EnumData("Lang5")]
    Lang5,

    /// <summary>
    /// Japanese: 無変換 (muhenkan)
    /// </summary>
    [EnumData("NonConvert")]
    NonConvert,


    /* Control Pad */
    /// <summary>
    /// ⌦. The forward delete key.	Note that on Apple keyboards, the key labelled Delete on the main part of the keyboard should be encoded as "Backspace".
    /// </summary>
    [EnumData("Delete", UnicodeCommon.KEY_DELETE)]
    Delete,

    /// <summary>
    /// Page Down, End or ↘
    /// </summary>
    [EnumData("End", UnicodeCommon.KEY_END)]
    End,

    /// <summary>
    /// Help. Not present on standard PC keyboards.
    /// </summary>
    [EnumData("Help")]
    Help,

    /// <summary>
    /// Home or ↖
    /// </summary>
    [EnumData("Home", UnicodeCommon.KEY_HOME)]
    Home,

    /// <summary>
    /// Insert or Ins. Not present on Apple keyboards.
    /// </summary>
    [EnumData("Insert")]
    Insert,

    /// <summary>
    /// Page Down, PgDn or ⇟
    /// </summary>
    [EnumData("PageDown", UnicodeCommon.KEY_PGDOWN)]
    PageDown,

    /// <summary>
    /// Page Up, PgUp or ⇞
    /// </summary>
    [EnumData("PageUp", UnicodeCommon.KEY_PGUP)]
    PageUp,



    /* Arrow Pad */

    /// <summary>
    /// ↓
    /// </summary>
    [EnumData("ArrowDown", UnicodeCommon.KEY_DOWN)]
    ArrowDown,

    /// <summary>
    /// ←
    /// </summary>
    [EnumData("ArrowLeft", UnicodeCommon.KEY_LEFT)]
    ArrowLeft,

    /// <summary>
    /// →
    /// </summary>
    [EnumData("ArrowRight", UnicodeCommon.KEY_RIGHT)]
    ArrowRight,

    /// <summary>
    /// ↑
    /// </summary>
    [EnumData("ArrowUp", UnicodeCommon.KEY_UP)]
    ArrowUp,



    /* Numpad Section */


    /// <summary>
    /// On the Mac, the "NumLock" code should be used for the numpad Clear key.
    /// </summary>
    [EnumData("NumLock")]
    NumLock,

    /// <summary>
    /// 0 Ins on a keyboard 0 on a phone or remote control
    /// </summary>
    [EnumData("Numpad0", UnicodeCommon.CHAR_DIGIT_0)]
    Numpad0,

    /// <summary>
    /// 1 End on a keyboard 1 or 1 QZ on a phone or remote control
    /// </summary>
    [EnumData("Numpad1", UnicodeCommon.CHAR_DIGIT_1)]
    Numpad1,

    /// <summary>
    /// 2 ↓ on a keyboard 2 ABC on a phone or remote control
    /// </summary>
    [EnumData("Numpad2", UnicodeCommon.CHAR_DIGIT_2)]
    Numpad2,

    /// <summary>
    /// 3 PgDn on a keyboard 3 DEF on a phone or remote control
    /// </summary>
    [EnumData("Numpad3", UnicodeCommon.CHAR_DIGIT_3)]
    Numpad3,

    /// <summary>
    /// 4 ← on a keyboard 4 GHI on a phone or remote control
    /// </summary>
    [EnumData("Numpad4", UnicodeCommon.CHAR_DIGIT_4)]
    Numpad4,

    /// <summary>
    /// 5 on a keyboard 5 JKL on a phone or remote control
    /// </summary>
    [EnumData("Numpad5", UnicodeCommon.CHAR_DIGIT_5)]
    Numpad5,

    /// <summary>
    /// 6 → on a keyboard 6 MNO on a phone or remote control
    /// </summary>
    [EnumData("Numpad6", UnicodeCommon.CHAR_DIGIT_6)]
    Numpad6,

    /// <summary>
    /// 7 Home on a keyboard 7 PQRS or 7 PRS on a phone  or remote control
    /// </summary>
    [EnumData("Numpad7", UnicodeCommon.CHAR_DIGIT_7)]
    Numpad7,

    /// <summary>
    /// 8 ↑ on a keyboard 8 TUV on a phone or remote control
    /// </summary>
    [EnumData("Numpad8", UnicodeCommon.CHAR_DIGIT_8)]
    Numpad8,

    /// <summary>
    /// 9 PgUp on a keyboard 9 WXYZ or 9 WXY on a phone  or remote control
    /// </summary>
    [EnumData("Numpad9", UnicodeCommon.CHAR_DIGIT_9)]
    Numpad9,

    /// <summary>
    /// +
    /// </summary>
    [EnumData("NumpadAdd", UnicodeCommon.CHAR_PLUS_SIGN)]
    NumpadAdd,

    /// <summary>
    /// Found on the Microsoft Natural Keyboard.
    /// </summary>
    [EnumData("NumpadBackspace")]
    NumpadBackspace,

    /// <summary>
    /// C or AC (All Clear). Also for use with numpads that have a Clear key that is separate from the NumLock key. On the Mac, the numpad Clear key should always be encoded as "NumLock".
    /// </summary>
    [EnumData("NumpadClear")]
    NumpadClear,

    /// <summary>
    /// CE (Clear Entry)
    /// </summary>
    [EnumData("NumpadClearEntry")]
    NumpadClearEntry,

    /// <summary>
    /// , (thousands separator). For locales where the thousands separator	is a "." (e.g., Brazil), this key may generate a ..
    /// </summary>
    [EnumData("NumpadComma")]
    NumpadComma,

    /// <summary>
    /// . Del. For locales where the decimal separator is "," (e.g., Brazil), this key may generate a,.
    /// </summary>
    [EnumData("NumpadDecimal")]
    NumpadDecimal,

    /// <summary>
    /// /
    /// </summary>
    [EnumData("NumpadDivide")]
    NumpadDivide,
    /// <summary>
    /// Newline
    /// </summary>
    [EnumData("NumpadEnter")]
    NumpadEnter,

    /// <summary>
    /// =
    /// </summary>
    [EnumData("NumpadEqual")]
    NumpadEqual,

    /// <summary>
    /// # on a phone or remote control device. This key is typically found	below the 9 key and to the right of the 0 key.
    /// </summary>
    [EnumData("NumpadHash")]
    NumpadHash,

    /// <summary>
    /// M+ Add current entry to the value stored in memory.
    /// </summary>
    [EnumData("NumpadMemoryAdd")]
    NumpadMemoryAdd,

    /// <summary>
    /// MC Clear the value stored in memory.
    /// </summary>
    [EnumData("NumpadMemoryClear")]
    NumpadMemoryClear,

    /// <summary>
    /// MR Replace the current entry with the value stored in memory.
    /// </summary>
    [EnumData("NumpadMemoryRecall")]
    NumpadMemoryRecall,

    /// <summary>
    /// MS Replace the value stored in memory with the current entry.
    /// </summary>
    [EnumData("NumpadMemoryStore")]
    NumpadMemoryStore,

    /// <summary>
    /// M- Subtract current entry from the value stored in memory.
    /// </summary>
    [EnumData("NumpadMemorySubtract")]
    NumpadMemorySubtract,

    /// <summary>
    /// * on a keyboard. For use with numpads that provide mathematical operations (+, -, * and /). Use "NumpadStar" for the * key on phones and remote controls.
    /// </summary>
    [EnumData("NumpadMultiply")]
    NumpadMultiply,

    /// <summary>
    /// ( Found on the Microsoft Natural Keyboard.
    /// </summary>
    [EnumData("NumpadParenLeft")]
    NumpadParenLeft,

    /// <summary>
    /// ) Found on the Microsoft Natural Keyboard.
    /// </summary>
    [EnumData("NumpadParenRight")]
    NumpadParenRight,

    /// <summary>
    /// * on a phone or remote control device.	This key is typically found below the 7 key and to the left of the 0 key. Use "NumpadMultiply" for the * key on numeric keypads.
    /// </summary>
    [EnumData("NumpadStar")]
    NumpadStar,

    /// <summary>
    /// -
    /// </summary>
    [EnumData("NumpadSubtract")]
    NumpadSubtract,



    /* Function Section */


    /// <summary>
    /// Esc or ⎋
    /// </summary>
    [EnumData("Escape", UnicodeCommon.KEY_ESCAPE)]
    Escape,

    /// <summary>
    /// F1
    /// </summary>
    [EnumData("F1")]
    F1,

    /// <summary>
    /// F2
    /// </summary>
    [EnumData("F2")]
    F2,

    /// <summary>
    /// F3
    /// </summary>
    [EnumData("F3")]
    F3,

    /// <summary>
    /// F4
    /// </summary>
    [EnumData("F4")]
    F4,

    /// <summary>
    /// F5
    /// </summary>
    [EnumData("F5")]
    F5,

    /// <summary>
    /// F6
    /// </summary>
    [EnumData("F6")]
    F6,

    /// <summary>
    /// F7
    /// </summary>
    [EnumData("F7")]
    F7,

    /// <summary>
    /// F8
    /// </summary>
    [EnumData("F8")]
    F8,

    /// <summary>
    /// F9
    /// </summary>
    [EnumData("F9")]
    F9,

    /// <summary>
    /// F10
    /// </summary>
    [EnumData("F10")]
    F10,

    /// <summary>
    /// F11
    /// </summary>
    [EnumData("F11")]
    F11,

    /// <summary>
    /// F12
    /// </summary>
    [EnumData("F12")]
    F12,

    /// <summary>
    /// Fn This is typically a hardware key that does not generate a separate   code. Most keyboards do not place this key in the function section, but it is included here to keep it with related keys.
    /// </summary>
    [EnumData("Fn")]
    Fn,

    /// <summary>
    /// FLock or FnLock. Function Lock key. Found on the Microsoft  Natural Keyboard.
    /// </summary>
    [EnumData("FnLock")]
    FnLock,

    /// <summary>
    /// PrtScr SysRq or Print Screen
    /// </summary>
    [EnumData("PrintScreen")]
    PrintScreen,

    /// <summary>
    /// Scroll Lock
    /// </summary>
    [EnumData("ScrollLock")]
    ScrollLock,

    /// <summary>
    /// Pause Break
    /// </summary>
    [EnumData("Pause")]
    Pause,



    /* Media Keys */


    /// <summary>
    /// Some laptops place this key to the left of the ↑ key.
    /// </summary>
    [EnumData("BrowserBack")]
    BrowserBack,
    /// <summary>
    /// No definition
    /// </summary>
    [EnumData("BrowserFavorites")]
    BrowserFavorites,

    /// <summary>
    /// Some laptops place this key to the right of the ↑ key.
    /// </summary>
    [EnumData("BrowserForward")]
    BrowserForward,

    /// <summary>
    ///
    /// </summary>
    [EnumData("BrowserHome")]
    BrowserHome,

    /// <summary>
    ///
    /// </summary>
    [EnumData("BrowserRefresh")]
    BrowserRefresh,

    /// <summary>
    ///
    /// </summary>
    [EnumData("BrowserSearch")]
    BrowserSearch,

    /// <summary>
    ///
    /// </summary>
    [EnumData("BrowserStop")]
    BrowserStop,

    /// <summary>
    /// Eject or ⏏. This key is placed in the function  section on some Apple keyboards.
    /// </summary>
    [EnumData("Eject")]
    Eject,

    /// <summary>
    /// Sometimes labelled My Computer on the keyboard
    /// </summary>
    [EnumData("LaunchApp1")]
    LaunchApp1,

    /// <summary>
    /// Sometimes labelled Calculator on the keyboard
    /// </summary>
    [EnumData("LaunchApp2")]
    LaunchApp2,

    /// <summary>
    ///
    /// </summary>
    [EnumData("LaunchMail")]
    LaunchMail,

    /// <summary>
    ///
    /// </summary>
    [EnumData("MediaPlayPause")]
    MediaPlayPause,

    /// <summary>
    ///
    /// </summary>
    [EnumData("MediaSelect")]
    MediaSelect,

    /// <summary>
    ///
    /// </summary>
    [EnumData("MediaStop")]
    MediaStop,

    /// <summary>
    ///
    /// </summary>
    [EnumData("MediaTrackNext")]
    MediaTrackNext,

    /// <summary>
    ///
    /// </summary>
    [EnumData("MediaTrackPrevious")]
    MediaTrackPrevious,

    /// <summary>
    /// This key is placed in the function section on some Apple keyboards, replacing the Eject key.
    /// </summary>
    [EnumData("Power")]
    Power,

    /// <summary>
    ///
    /// </summary>
    [EnumData("Sleep")]
    Sleep,

    /// <summary>
    ///
    /// </summary>
    [EnumData("AudioVolumeDown")]
    AudioVolumeDown,

    /// <summary>
    ///
    /// </summary>
    [EnumData("AudioVolumeMute")]
    AudioVolumeMute,

    /// <summary>
    ///
    /// </summary>
    [EnumData("AudioVolumeUp")]
    AudioVolumeUp,

    /// <summary>
    ///
    /// </summary>
    [EnumData("WakeUp")]
    WakeUp,


    MAX// Tracks the end of the available enum values
}

