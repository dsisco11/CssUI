using CssUI.Internal;

namespace CssUI.DOM.Events
{
    [MetaEnum]
    public enum EKeyboardCode : int
    {/* Docs: https://www.w3.org/TR/uievents-code/#code-value-tables */

        /// <summary>
        /// This value code should be used when no other value given in this specification is appropriate.
        /// </summary>
        [MetaKeyword("")]
        Invalid = 0x0,

        /// <summary>
        /// `~ on a US keyboard. This is the 半角/全角/漢字 (hankaku/zenkaku/kanji) key on Japanese keyboards
        /// </summary>
        [MetaKeyword("Backquote", '`')]
        Backquote,

        /// <summary>
        /// Used for both the US \| (on the 101-key layout) and also for the key    located between the " and Enter keys on row C of the 102-, 104- and 106-key layouts. Labelled #~ on a UK (102) keyboard.
        /// </summary>
        [MetaKeyword("Backslash", UnicodeCommon.CHAR_REVERSE_SOLIDUS)]
        Backslash,
        
        /// <summary>
        /// Backspace or ⌫.	Labelled Delete on Apple keyboards.
        /// </summary>
        [MetaKeyword("Backspace", '⌫')]
        Backspace,

        /// <summary>
        /// [{ on a US keyboard.
        /// </summary>
        [MetaKeyword("BracketLeft", '[')]
        BracketLeft,

        /// <summary>
        /// ]} on a US keyboard.
        /// </summary>
        [MetaKeyword("BracketRight", ']')]
        BracketRight,

        /// <summary>
        /// ,< on a US keyboard.
        /// </summary>
        [MetaKeyword("Comma", ',')]
        Comma,

        /// <summary>
        /// 0 ) on a US keyboard.
        /// </summary>
        [MetaKeyword("Digit0", '0')]
        Digit0,

        /// <summary>
        /// 1 ! on a US keyboard.
        /// </summary>
        [MetaKeyword("Digit1", '1')]
        Digit1,

        /// <summary>
        /// 2 @ on a US keyboard.
        /// </summary>
        [MetaKeyword("Digit2", '2')]
        Digit2,

        /// <summary>
        /// 3 # on a US keyboard.
        /// </summary>
        [MetaKeyword("Digit3", '3')]
        Digit3,

        /// <summary>
        /// 4 $ on a US keyboard.
        /// </summary>
        [MetaKeyword("Digit4", '4')]
        Digit4,

        /// <summary>
        /// 5 % on a US keyboard.
        /// </summary>
        [MetaKeyword("Digit5", '5')]
        Digit5,

        /// <summary>
        /// 6 ^ on a US keyboard.
        /// </summary>
        [MetaKeyword("Digit6", '6')]
        Digit6,

        /// <summary>
        /// 7 & on a US keyboard.
        /// </summary>
        [MetaKeyword("Digit7", '7')]
        Digit7,

        /// <summary>
        /// 8 * on a US keyboard.
        /// </summary>
        [MetaKeyword("Digit8", '8')]
        Digit8,

        /// <summary>
        /// 9 ( on a US keyboard.
        /// </summary>
        [MetaKeyword("Digit9", '9')]
        Digit9,

        /// <summary>
        /// = + on a US keyboard.
        /// </summary>
        [MetaKeyword("Equal", '=')]
        Equal,

        /// <summary>
        /// Located between the left Shift and Z keys.	Labelled \| on a UK keyboard.
        /// </summary>
        [MetaKeyword("IntlBackslash", UnicodeCommon.CHAR_REVERSE_SOLIDUS)]
        IntlBackslash,

        /// <summary>
        /// Located between the / and right Shift keys.	Labelled \ (ro) on a Japanese keyboard.
        /// </summary>
        [MetaKeyword("IntlRo", 'ろ')]
        IntlRo,

        /// <summary>
        /// Located between the = and Backspace keys.	Labelled  (yen) on a Japanese keyboard. \/ on a Russian keyboard.
        /// </summary>
        [MetaKeyword("IntlYen", '¥')]
        IntlYen,

        /// <summary>
        /// a on a US keyboard.Labelled q on an AZERTY (e.g., French) keyboard.
        /// </summary>
        [MetaKeyword("KeyA", 'a')]
        KeyA,

        /// <summary>
        /// b on a US keyboard.
        /// </summary>
        [MetaKeyword("KeyB", 'b')]
        KeyB,

        /// <summary>
        /// c on a US keyboard.
        /// </summary>
        [MetaKeyword("KeyC", 'c')]
        KeyC,

        /// <summary>
        /// d on a US keyboard.
        /// </summary>
        [MetaKeyword("KeyD", 'd')]
        KeyD,

        /// <summary>
        /// e on a US keyboard.
        /// </summary>
        [MetaKeyword("KeyE", 'e')]
        KeyE,

        /// <summary>
        /// f on a US keyboard.
        /// </summary>
        [MetaKeyword("KeyF", 'f')]
        KeyF,

        /// <summary>
        /// g on a US keyboard.
        /// </summary>
        [MetaKeyword("KeyG", 'g')]
        KeyG,

        /// <summary>
        /// h on a US keyboard.
        /// </summary>
        [MetaKeyword("KeyH", 'h')]
        KeyH,

        /// <summary>
        /// i on a US keyboard.
        /// </summary>
        [MetaKeyword("KeyI", 'i')]
        KeyI,

        /// <summary>
        /// j on a US keyboard.
        /// </summary>
        [MetaKeyword("KeyJ", 'j')]
        KeyJ,

        /// <summary>
        /// k on a US keyboard.
        /// </summary>
        [MetaKeyword("KeyK", 'k')]
        KeyK,

        /// <summary>
        /// l on a US keyboard.
        /// </summary>
        [MetaKeyword("KeyL", 'l')]
        KeyL,

        /// <summary>
        /// m on a US keyboard.
        /// </summary>
        [MetaKeyword("KeyM", 'm')]
        KeyM,

        /// <summary>
        /// n on a US keyboard.
        /// </summary>
        [MetaKeyword("KeyN", 'n')]
        KeyN,

        /// <summary>
        /// o on a US keyboard.
        /// </summary>
        [MetaKeyword("KeyO", 'o')]
        KeyO,

        /// <summary>
        /// p on a US keyboard.
        /// </summary>
        [MetaKeyword("KeyP", 'p')]
        KeyP,

        /// <summary>
        /// q on a US keyboard.Labelled a on an AZERTY (e.g., French) keyboard.
        /// </summary>
        [MetaKeyword("KeyQ", 'q')]
        KeyQ,

        /// <summary>
        /// r on a US keyboard.
        /// </summary>
        [MetaKeyword("KeyR", 'r')]
        KeyR,

        /// <summary>
        /// s on a US keyboard.
        /// </summary>
        [MetaKeyword("KeyS", 's')]
        KeyS,

        /// <summary>
        /// t on a US keyboard.
        /// </summary>
        [MetaKeyword("KeyT", 't')]
        KeyT,

        /// <summary>
        /// u on a US keyboard.
        /// </summary>
        [MetaKeyword("KeyU", 'u')]
        KeyU,

        /// <summary>
        /// v on a US keyboard.
        /// </summary>
        [MetaKeyword("KeyV", 'v')]
        KeyV,

        /// <summary>
        /// w on a US keyboard.Labelled z on an AZERTY (e.g., French) keyboard.
        /// </summary>
        [MetaKeyword("KeyW", 'w')]
        KeyW,

        /// <summary>
        /// x on a US keyboard.
        /// </summary>
        [MetaKeyword("KeyX", 'x')]
        KeyX,

        /// <summary>
        /// y on a US keyboard.Labelled z on a QWERTZ (e.g., German) keyboard.
        /// </summary>
        [MetaKeyword("KeyY", 'y')]
        KeyY,

        /// <summary>
        /// z on a US keyboard.Labelled w on an AZERTY (e.g., French) keyboard, and y on a QWERTZ (e.g., German) keyboard.
        /// </summary>
        [MetaKeyword("KeyZ", 'z')]
        KeyZ,

        /// <summary>
        /// -_ on a US keyboard.
        /// </summary>
        [MetaKeyword("Minus", '-')]
        Minus,

        /// <summary>
        /// .> on a US keyboard.
        /// </summary>
        [MetaKeyword("Period", '.')]
        Period,

        /// <summary>
        /// '" on a US keyboard.
        /// </summary>
        [MetaKeyword("Quote", UnicodeCommon.CHAR_APOSTRAPHE)]
        Quote,

        /// <summary>
        /// ;: on a US keyboard.
        /// </summary>
        [MetaKeyword("Semicolon", ';')]
        Semicolon,

        /// <summary>
        /// /? on a US keyboard.
        /// </summary>
        [MetaKeyword("Slash", '/')]
        Slash,

        /// <summary>
        /// Alt, Option or ⌥.
        /// </summary>
        [MetaKeyword("AltLeft", UnicodeCommon.KEY_ALT_MODIFIER)]
        AltLeft,

        /// <summary>
        /// Alt, Option or ⌥.	This is labelled AltGr key on many keyboard layouts.
        /// </summary>
        [MetaKeyword("AltRight", UnicodeCommon.KEY_ALT_MODIFIER)]
        AltRight,

        /// <summary>
        /// CapsLock or ⇪
        /// </summary>
        [MetaKeyword("CapsLock", UnicodeCommon.KEY_CAPSLOCK)]
        CapsLock,

        /// <summary>
        /// The application context menu key, which is typically found between the right Meta key and the right Control key.
        /// </summary>
        [MetaKeyword("ContextMenu")]
        ContextMenu,

        /// <summary>
        /// Control or ⌃
        /// </summary>
        [MetaKeyword("ControlLeft", UnicodeCommon.KEY_CTRL_MODIFIER)]
        ControlLeft,

        /// <summary>
        /// Control or ⌃
        /// </summary>
        [MetaKeyword("ControlRight", UnicodeCommon.KEY_CTRL_MODIFIER)]
        ControlRight,

        /// <summary>
        /// Enter or ↵. Labelled Return on Apple keyboards.
        /// </summary>
        [MetaKeyword("Enter", UnicodeCommon.KEY_ENTER)]
        Enter,

        /// <summary>
        /// The Windows, ⌘, Command or other OS symbol key.
        /// </summary>
        [MetaKeyword("MetaLeft", UnicodeCommon.KEY_META_MODIFIER)]
        MetaLeft,

        /// <summary>
        /// The Windows, ⌘, Command or other OS symbol key.
        /// </summary>
        [MetaKeyword("MetaRight", UnicodeCommon.KEY_META_MODIFIER)]
        MetaRight,

        /// <summary>
        /// Shift or ⇧
        /// </summary>
        [MetaKeyword("ShiftLeft", UnicodeCommon.KEY_SHIFT_MODIFIER)]
        ShiftLeft,

        /// <summary>
        /// Shift or ⇧
        /// </summary>
        [MetaKeyword("ShiftRight", UnicodeCommon.KEY_SHIFT_MODIFIER)]
        ShiftRight,

        /// <summary>
        /// (space)
        /// </summary>
        [MetaKeyword("Space", UnicodeCommon.KEY_SPACE)]
        Space,

        /// <summary>
        /// Tab or ⇥
        /// </summary>
        [MetaKeyword("Tab", UnicodeCommon.KEY_TAB)]
        Tab,

        /// <summary>
        /// Japanese: 変換 (henkan)
        /// </summary>
        [MetaKeyword("Convert")]
        Convert,

        /// <summary>
        /// Japanese: カタカナ/ひらがな/ローマ字 (katakana/hiragana/romaji)
        /// </summary>
        [MetaKeyword("KanaMode")]
        KanaMode,

        /// <summary>
        /// Korean: HangulMode 한/영 (han/yeong) Japanese (Mac keyboard): かな (kana)
        /// </summary>
        [MetaKeyword("Lang1", '한', '영')]
        Lang1,

        /// <summary>
        /// Korean: Hanja 한자 (hanja) Japanese (Mac keyboard): 英数 (eisu)
        /// </summary>
        [MetaKeyword("Lang2")]
        Lang2,

        /// <summary>
        /// Japanese (word-processing keyboard): Katakana
        /// </summary>
        [MetaKeyword("Lang3")]
        Lang3,

        /// <summary>
        /// Japanese (word-processing keyboard): Hiragana
        /// </summary>
        [MetaKeyword("Lang4")]
        Lang4,

        /// <summary>
        /// Japanese (word-processing keyboard): Zenkaku/Hankaku
        /// </summary>
        [MetaKeyword("Lang5")]
        Lang5,

        /// <summary>
        /// Japanese: 無変換 (muhenkan)
        /// </summary>
        [MetaKeyword("NonConvert")]
        NonConvert,


        /* Control Pad */
        /// <summary>
        /// ⌦. The forward delete key.	Note that on Apple keyboards, the key labelled Delete on the main part of the keyboard should be encoded as "Backspace".
        /// </summary>
        [MetaKeyword("Delete", UnicodeCommon.KEY_DELETE)]
        Delete,

        /// <summary>
        /// Page Down, End or ↘
        /// </summary>
        [MetaKeyword("End", UnicodeCommon.KEY_END)]
        End,

        /// <summary>
        /// Help. Not present on standard PC keyboards.
        /// </summary>
        [MetaKeyword("Help")]
        Help,

        /// <summary>
        /// Home or ↖
        /// </summary>
        [MetaKeyword("Home", UnicodeCommon.KEY_HOME)]
        Home,

        /// <summary>
        /// Insert or Ins. Not present on Apple keyboards.
        /// </summary>
        [MetaKeyword("Insert")]
        Insert,

        /// <summary>
        /// Page Down, PgDn or ⇟
        /// </summary>
        [MetaKeyword("PageDown", UnicodeCommon.KEY_PGDOWN)]
        PageDown,

        /// <summary>
        /// Page Up, PgUp or ⇞
        /// </summary>
        [MetaKeyword("PageUp", UnicodeCommon.KEY_PGUP)]
        PageUp,



        /* Arrow Pad */

        /// <summary>
        /// ↓
        /// </summary>
        [MetaKeyword("ArrowDown", UnicodeCommon.KEY_DOWN)]
        ArrowDown,

        /// <summary>
        /// ←
        /// </summary>
        [MetaKeyword("ArrowLeft", UnicodeCommon.KEY_LEFT)]
        ArrowLeft,

        /// <summary>
        /// →
        /// </summary>
        [MetaKeyword("ArrowRight", UnicodeCommon.KEY_RIGHT)]
        ArrowRight,

        /// <summary>
        /// ↑
        /// </summary>
        [MetaKeyword("ArrowUp", UnicodeCommon.KEY_UP)]
        ArrowUp,



        /* Numpad Section */


        /// <summary>
        /// On the Mac, the "NumLock" code should be used for the numpad Clear key.
        /// </summary>
        [MetaKeyword("NumLock")]
        NumLock,

        /// <summary>
        /// 0 Ins on a keyboard 0 on a phone or remote control
        /// </summary>
        [MetaKeyword("Numpad0")]
        Numpad0,

        /// <summary>
        /// 1 End on a keyboard 1 or 1 QZ on a phone or remote control
        /// </summary>
        [MetaKeyword("Numpad1")]
        Numpad1,

        /// <summary>
        /// 2 ↓ on a keyboard 2 ABC on a phone or remote control
        /// </summary>
        [MetaKeyword("Numpad2")]
        Numpad2,

        /// <summary>
        /// 3 PgDn on a keyboard 3 DEF on a phone or remote control
        /// </summary>
        [MetaKeyword("Numpad3")]
        Numpad3,

        /// <summary>
        /// 4 ← on a keyboard 4 GHI on a phone or remote control
        /// </summary>
        [MetaKeyword("Numpad4")]
        Numpad4,

        /// <summary>
        /// 5 on a keyboard 5 JKL on a phone or remote control
        /// </summary>
        [MetaKeyword("Numpad5")]
        Numpad5,

        /// <summary>
        /// 6 → on a keyboard 6 MNO on a phone or remote control
        /// </summary>
        [MetaKeyword("Numpad6")]
        Numpad6,

        /// <summary>
        /// 7 Home on a keyboard 7 PQRS or 7 PRS on a phone  or remote control
        /// </summary>
        [MetaKeyword("Numpad7")]
        Numpad7,

        /// <summary>
        /// 8 ↑ on a keyboard 8 TUV on a phone or remote control
        /// </summary>
        [MetaKeyword("Numpad8")]
        Numpad8,

        /// <summary>
        /// 9 PgUp on a keyboard 9 WXYZ or 9 WXY on a phone  or remote control
        /// </summary>
        [MetaKeyword("Numpad9")]
        Numpad9,

        /// <summary>
        /// +
        /// </summary>
        [MetaKeyword("NumpadAdd", '+')]
        NumpadAdd,

        /// <summary>
        /// Found on the Microsoft Natural Keyboard.
        /// </summary>
        [MetaKeyword("NumpadBackspace")]
        NumpadBackspace,

        /// <summary>
        /// C or AC (All Clear). Also for use with numpads that have a Clear key that is separate from the NumLock key. On the Mac, the numpad Clear key should always be encoded as "NumLock".
        /// </summary>
        [MetaKeyword("NumpadClear")]
        NumpadClear,

        /// <summary>
        /// CE (Clear Entry)
        /// </summary>
        [MetaKeyword("NumpadClearEntry")]
        NumpadClearEntry,

        /// <summary>
        /// , (thousands separator). For locales where the thousands separator	is a "." (e.g., Brazil), this key may generate a ..
        /// </summary>
        [MetaKeyword("NumpadComma")]
        NumpadComma,

        /// <summary>
        /// . Del. For locales where the decimal separator is "," (e.g., Brazil), this key may generate a,.
        /// </summary>
        [MetaKeyword("NumpadDecimal")]
        NumpadDecimal,

        /// <summary>
        /// /
        /// </summary>
        [MetaKeyword("NumpadDivide")]
        NumpadDivide,
        /// <summary>
        /// Newline
        /// </summary>
        [MetaKeyword("NumpadEnter")]
        NumpadEnter,

        /// <summary>
        /// =
        /// </summary>
        [MetaKeyword("NumpadEqual")]
        NumpadEqual,

        /// <summary>
        /// # on a phone or remote control device. This key is typically found	below the 9 key and to the right of the 0 key.
        /// </summary>
        [MetaKeyword("NumpadHash")]
        NumpadHash,

        /// <summary>
        /// M+ Add current entry to the value stored in memory.
        /// </summary>
        [MetaKeyword("NumpadMemoryAdd")]
        NumpadMemoryAdd,

        /// <summary>
        /// MC Clear the value stored in memory.
        /// </summary>
        [MetaKeyword("NumpadMemoryClear")]
        NumpadMemoryClear,

        /// <summary>
        /// MR Replace the current entry with the value stored in memory.
        /// </summary>
        [MetaKeyword("NumpadMemoryRecall")]
        NumpadMemoryRecall,

        /// <summary>
        /// MS Replace the value stored in memory with the current entry.
        /// </summary>
        [MetaKeyword("NumpadMemoryStore")]
        NumpadMemoryStore,

        /// <summary>
        /// M- Subtract current entry from the value stored in memory.
        /// </summary>
        [MetaKeyword("NumpadMemorySubtract")]
        NumpadMemorySubtract,

        /// <summary>
        /// * on a keyboard. For use with numpads that provide mathematical operations (+, -, * and /). Use "NumpadStar" for the * key on phones and remote controls.
        /// </summary>
        [MetaKeyword("NumpadMultiply")]
        NumpadMultiply,

        /// <summary>
        /// ( Found on the Microsoft Natural Keyboard.
        /// </summary>
        [MetaKeyword("NumpadParenLeft")]
        NumpadParenLeft,

        /// <summary>
        /// ) Found on the Microsoft Natural Keyboard.
        /// </summary>
        [MetaKeyword("NumpadParenRight")]
        NumpadParenRight,

        /// <summary>
        /// * on a phone or remote control device.	This key is typically found below the 7 key and to the left of the 0 key. Use "NumpadMultiply" for the * key on numeric keypads.
        /// </summary>
        [MetaKeyword("NumpadStar")]
        NumpadStar,

        /// <summary>
        /// -
        /// </summary>
        [MetaKeyword("NumpadSubtract")]
        NumpadSubtract,



        /* Function Section */


        /// <summary>
        /// Esc or ⎋
        /// </summary>
        [MetaKeyword("Escape", UnicodeCommon.KEY_ESCAPE)]
        Escape,

        /// <summary>
        /// F1
        /// </summary>
        [MetaKeyword("F1")]
        F1,

        /// <summary>
        /// F2
        /// </summary>
        [MetaKeyword("F2")]
        F2,

        /// <summary>
        /// F3
        /// </summary>
        [MetaKeyword("F3")]
        F3,

        /// <summary>
        /// F4
        /// </summary>
        [MetaKeyword("F4")]
        F4,

        /// <summary>
        /// F5
        /// </summary>
        [MetaKeyword("F5")]
        F5,

        /// <summary>
        /// F6
        /// </summary>
        [MetaKeyword("F6")]
        F6,

        /// <summary>
        /// F7
        /// </summary>
        [MetaKeyword("F7")]
        F7,

        /// <summary>
        /// F8
        /// </summary>
        [MetaKeyword("F8")]
        F8,

        /// <summary>
        /// F9
        /// </summary>
        [MetaKeyword("F9")]
        F9,

        /// <summary>
        /// F10
        /// </summary>
        [MetaKeyword("F10")]
        F10,

        /// <summary>
        /// F11
        /// </summary>
        [MetaKeyword("F11")]
        F11,

        /// <summary>
        /// F12
        /// </summary>
        [MetaKeyword("F12")]
        F12,

        /// <summary>
        /// Fn This is typically a hardware key that does not generate a separate   code. Most keyboards do not place this key in the function section, but it is included here to keep it with related keys.
        /// </summary>
        [MetaKeyword("Fn")]
        Fn,

        /// <summary>
        /// FLock or FnLock. Function Lock key. Found on the Microsoft  Natural Keyboard.
        /// </summary>
        [MetaKeyword("FnLock")]
        FnLock,

        /// <summary>
        /// PrtScr SysRq or Print Screen
        /// </summary>
        [MetaKeyword("PrintScreen")]
        PrintScreen,

        /// <summary>
        /// Scroll Lock
        /// </summary>
        [MetaKeyword("ScrollLock")]
        ScrollLock,

        /// <summary>
        /// Pause Break
        /// </summary>
        [MetaKeyword("Pause")]
        Pause,



        /* Media Keys */


        /// <summary>
        /// Some laptops place this key to the left of the ↑ key.
        /// </summary>
        [MetaKeyword("BrowserBack")]
        BrowserBack,
        /// <summary>
        /// No definition
        /// </summary>
        [MetaKeyword("BrowserFavorites")]
        BrowserFavorites,

        /// <summary>
        /// Some laptops place this key to the right of the ↑ key.
        /// </summary>
        [MetaKeyword("BrowserForward")]
        BrowserForward,

        /// <summary>
        ///         
        /// </summary>
        [MetaKeyword("BrowserHome")]
        BrowserHome,

        /// <summary>
        ///         
        /// </summary>
        [MetaKeyword("BrowserRefresh")]
        BrowserRefresh,

        /// <summary>
        ///         
        /// </summary>
        [MetaKeyword("BrowserSearch")]
        BrowserSearch,

        /// <summary>
        ///         
        /// </summary>
        [MetaKeyword("BrowserStop")]
        BrowserStop,

        /// <summary>
        /// Eject or ⏏. This key is placed in the function  section on some Apple keyboards.
        /// </summary>
        [MetaKeyword("Eject")]
        Eject,

        /// <summary>
        /// Sometimes labelled My Computer on the keyboard
        /// </summary>
        [MetaKeyword("LaunchApp1")]
        LaunchApp1,

        /// <summary>
        /// Sometimes labelled Calculator on the keyboard
        /// </summary>
        [MetaKeyword("LaunchApp2")]
        LaunchApp2,

        /// <summary>
        ///         
        /// </summary>
        [MetaKeyword("LaunchMail")]
        LaunchMail,

        /// <summary>
        ///         
        /// </summary>
        [MetaKeyword("MediaPlayPause")]
        MediaPlayPause,

        /// <summary>
        ///         
        /// </summary>
        [MetaKeyword("MediaSelect")]
        MediaSelect,

        /// <summary>
        ///         
        /// </summary>
        [MetaKeyword("MediaStop")]
        MediaStop,

        /// <summary>
        ///         
        /// </summary>
        [MetaKeyword("MediaTrackNext")]
        MediaTrackNext,

        /// <summary>
        ///         
        /// </summary>
        [MetaKeyword("MediaTrackPrevious")]
        MediaTrackPrevious,

        /// <summary>
        /// This key is placed in the function section on some Apple keyboards, replacing the Eject key.
        /// </summary>
        [MetaKeyword("Power")]
        Power,

        /// <summary>
        ///         
        /// </summary>
        [MetaKeyword("Sleep")]
        Sleep,

        /// <summary>
        ///         
        /// </summary>
        [MetaKeyword("AudioVolumeDown")]
        AudioVolumeDown,

        /// <summary>
        ///         
        /// </summary>
        [MetaKeyword("AudioVolumeMute")]
        AudioVolumeMute,

        /// <summary>
        ///         
        /// </summary>
        [MetaKeyword("AudioVolumeUp")]
        AudioVolumeUp,

        /// <summary>
        /// 
        /// </summary>
        [MetaKeyword("WakeUp")]
        WakeUp,


    }

}
