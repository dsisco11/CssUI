using CssUI.DOM.Internal;

namespace CssUI.DOM.Events
{
    [DomEnum]
    public enum EKeyboardCode : short
    {/* Docs: https://www.w3.org/TR/uievents-code/#code-value-tables */

        /// <summary>
        /// This value code should be used when no other value given in this specification is appropriate.
        /// </summary>
        [DomKeyword("")]
        Invalid = 0x0,

        /// <summary>
        /// `~ on a US keyboard. This is the 半角/全角/漢字 (hankaku/zenkaku/kanji) key on Japanese keyboards
        /// </summary>
        [DomKeyword("Backquote")]
        Backquote,

        /// <summary>
        /// Used for both the US \| (on the 101-key layout) and also for the key    located between the " and Enter keys on row C of the 102-, 104- and 106-key layouts. Labelled #~ on a UK (102) keyboard.
        /// </summary>
        [DomKeyword("Backslash")]
        Backslash,
        
        /// <summary>
        /// Backspace or ⌫.	Labelled Delete on Apple keyboards.
        /// </summary>
        [DomKeyword("Backspace")]
        Backspace,

        /// <summary>
        /// [{ on a US keyboard.
        /// </summary>
        [DomKeyword("BracketLeft")]
        BracketLeft,

        /// <summary>
        /// ]} on a US keyboard.
        /// </summary>
        [DomKeyword("BracketRight")]
        BracketRight,

        /// <summary>
        /// ,< on a US keyboard.
        /// </summary>
        [DomKeyword("Comma")]
        Comma,

        /// <summary>
        /// 0 ) on a US keyboard.
        /// </summary>
        [DomKeyword("Digit0")]
        Digit0,

        /// <summary>
        /// 1 ! on a US keyboard.
        /// </summary>
        [DomKeyword("Digit1")]
        Digit1,

        /// <summary>
        /// 2 @ on a US keyboard.
        /// </summary>
        [DomKeyword("Digit2")]
        Digit2,

        /// <summary>
        /// 3 # on a US keyboard.
        /// </summary>
        [DomKeyword("Digit3")]
        Digit3,

        /// <summary>
        /// 4 $ on a US keyboard.
        /// </summary>
        [DomKeyword("Digit4")]
        Digit4,

        /// <summary>
        /// 5 % on a US keyboard.
        /// </summary>
        [DomKeyword("Digit5")]
        Digit5,

        /// <summary>
        /// 6 ^ on a US keyboard.
        /// </summary>
        [DomKeyword("Digit6")]
        Digit6,

        /// <summary>
        /// 7 & on a US keyboard.
        /// </summary>
        [DomKeyword("Digit7")]
        Digit7,

        /// <summary>
        /// 8 * on a US keyboard.
        /// </summary>
        [DomKeyword("Digit8")]
        Digit8,

        /// <summary>
        /// 9 ( on a US keyboard.
        /// </summary>
        [DomKeyword("Digit9")]
        Digit9,

        /// <summary>
        /// = + on a US keyboard.
        /// </summary>
        [DomKeyword("Equal")]
        Equal,

        /// <summary>
        /// Located between the left Shift and Z keys.	Labelled \| on a UK keyboard.
        /// </summary>
        [DomKeyword("IntlBackslash")]
        IntlBackslash,

        /// <summary>
        /// Located between the / and right Shift keys.	Labelled \ろ (ro) on a Japanese keyboard.
        /// </summary>
        [DomKeyword("IntlRo")]
        IntlRo,

        /// <summary>
        /// Located between the = and Backspace keys.	Labelled ¥ (yen) on a Japanese keyboard. \/ on a Russian keyboard.
        /// </summary>
        [DomKeyword("IntlYen")]
        IntlYen,

        /// <summary>
        /// a on a US keyboard.Labelled q on an AZERTY (e.g., French) keyboard.
        /// </summary>
        [DomKeyword("KeyA")]
        KeyA,

        /// <summary>
        /// b on a US keyboard.
        /// </summary>
        [DomKeyword("KeyB")]
        KeyB,

        /// <summary>
        /// c on a US keyboard.
        /// </summary>
        [DomKeyword("KeyC")]
        KeyC,

        /// <summary>
        /// d on a US keyboard.
        /// </summary>
        [DomKeyword("KeyD")]
        KeyD,

        /// <summary>
        /// e on a US keyboard.
        /// </summary>
        [DomKeyword("KeyE")]
        KeyE,

        /// <summary>
        /// f on a US keyboard.
        /// </summary>
        [DomKeyword("KeyF")]
        KeyF,

        /// <summary>
        /// g on a US keyboard.
        /// </summary>
        [DomKeyword("KeyG")]
        KeyG,

        /// <summary>
        /// h on a US keyboard.
        /// </summary>
        [DomKeyword("KeyH")]
        KeyH,

        /// <summary>
        /// i on a US keyboard.
        /// </summary>
        [DomKeyword("KeyI")]
        KeyI,

        /// <summary>
        /// j on a US keyboard.
        /// </summary>
        [DomKeyword("KeyJ")]
        KeyJ,

        /// <summary>
        /// k on a US keyboard.
        /// </summary>
        [DomKeyword("KeyK")]
        KeyK,

        /// <summary>
        /// l on a US keyboard.
        /// </summary>
        [DomKeyword("KeyL")]
        KeyL,

        /// <summary>
        /// m on a US keyboard.
        /// </summary>
        [DomKeyword("KeyM")]
        KeyM,

        /// <summary>
        /// n on a US keyboard.
        /// </summary>
        [DomKeyword("KeyN")]
        KeyN,

        /// <summary>
        /// o on a US keyboard.
        /// </summary>
        [DomKeyword("KeyO")]
        KeyO,

        /// <summary>
        /// p on a US keyboard.
        /// </summary>
        [DomKeyword("KeyP")]
        KeyP,

        /// <summary>
        /// q on a US keyboard.Labelled a on an AZERTY (e.g., French) keyboard.
        /// </summary>
        [DomKeyword("KeyQ")]
        KeyQ,

        /// <summary>
        /// r on a US keyboard.
        /// </summary>
        [DomKeyword("KeyR")]
        KeyR,

        /// <summary>
        /// s on a US keyboard.
        /// </summary>
        [DomKeyword("KeyS")]
        KeyS,

        /// <summary>
        /// t on a US keyboard.
        /// </summary>
        [DomKeyword("KeyT")]
        KeyT,

        /// <summary>
        /// u on a US keyboard.
        /// </summary>
        [DomKeyword("KeyU")]
        KeyU,

        /// <summary>
        /// v on a US keyboard.
        /// </summary>
        [DomKeyword("KeyV")]
        KeyV,

        /// <summary>
        /// w on a US keyboard.Labelled z on an AZERTY (e.g., French) keyboard.
        /// </summary>
        [DomKeyword("KeyW")]
        KeyW,

        /// <summary>
        /// x on a US keyboard.
        /// </summary>
        [DomKeyword("KeyX")]
        KeyX,

        /// <summary>
        /// y on a US keyboard.Labelled z on a QWERTZ (e.g., German) keyboard.
        /// </summary>
        [DomKeyword("KeyY")]
        KeyY,

        /// <summary>
        /// z on a US keyboard.Labelled w on an AZERTY (e.g., French) keyboard, and y on a QWERTZ (e.g., German) keyboard.
        /// </summary>
        [DomKeyword("KeyZ")]
        KeyZ,

        /// <summary>
        /// -_ on a US keyboard.
        /// </summary>
        [DomKeyword("Minus")]
        Minus,

        /// <summary>
        /// .> on a US keyboard.
        /// </summary>
        [DomKeyword("Period")]
        Period,

        /// <summary>
        /// '" on a US keyboard.
        /// </summary>
        [DomKeyword("Quote")]
        Quote,

        /// <summary>
        /// ;: on a US keyboard.
        /// </summary>
        [DomKeyword("Semicolon")]
        Semicolon,

        /// <summary>
        /// /? on a US keyboard.
        /// </summary>
        [DomKeyword("Slash")]
        Slash,

        /// <summary>
        /// Alt, Option or ⌥.
        /// </summary>
        [DomKeyword("AltLeft")]
        AltLeft,

        /// <summary>
        /// Alt, Option or ⌥.	This is labelled AltGr key on many keyboard layouts.
        /// </summary>
        [DomKeyword("AltRight")]
        AltRight,

        /// <summary>
        /// CapsLock or ⇪
        /// </summary>
        [DomKeyword("CapsLock")]
        CapsLock,

        /// <summary>
        /// The application context menu key, which is typically found between the right Meta key and the right Control key.
        /// </summary>
        [DomKeyword("ContextMenu")]
        ContextMenu,

        /// <summary>
        /// Control or ⌃
        /// </summary>
        [DomKeyword("ControlLeft")]
        ControlLeft,

        /// <summary>
        /// Control or ⌃
        /// </summary>
        [DomKeyword("ControlRight")]
        ControlRight,

        /// <summary>
        /// Enter or ↵. Labelled Return on Apple keyboards.
        /// </summary>
        [DomKeyword("Enter")]
        Enter,

        /// <summary>
        /// The Windows, ⌘, Command or other OS symbol key.
        /// </summary>
        [DomKeyword("MetaLeft")]
        MetaLeft,

        /// <summary>
        /// The Windows, ⌘, Command or other OS symbol key.
        /// </summary>
        [DomKeyword("MetaRight")]
        MetaRight,

        /// <summary>
        /// Shift or ⇧
        /// </summary>
        [DomKeyword("ShiftLeft")]
        ShiftLeft,

        /// <summary>
        /// Shift or ⇧
        /// </summary>
        [DomKeyword("ShiftRight")]
        ShiftRight,

        /// <summary>
        /// (space)
        /// </summary>
        [DomKeyword("Space")]
        Space,

        /// <summary>
        /// Tab or ⇥
        /// </summary>
        [DomKeyword("Tab")]
        Tab,

        /// <summary>
        /// Japanese: 変換 (henkan)
        /// </summary>
        [DomKeyword("Convert")]
        Convert,

        /// <summary>
        /// Japanese: カタカナ/ひらがな/ローマ字 (katakana/hiragana/romaji)
        /// </summary>
        [DomKeyword("KanaMode")]
        KanaMode,

        /// <summary>
        /// Korean: HangulMode 한/영 (han/yeong) Japanese (Mac keyboard): かな (kana)
        /// </summary>
        [DomKeyword("Lang1")]
        Lang1,

        /// <summary>
        /// Korean: Hanja 한자 (hanja) Japanese (Mac keyboard): 英数 (eisu)
        /// </summary>
        [DomKeyword("Lang2")]
        Lang2,

        /// <summary>
        /// Japanese (word-processing keyboard): Katakana
        /// </summary>
        [DomKeyword("Lang3")]
        Lang3,

        /// <summary>
        /// Japanese (word-processing keyboard): Hiragana
        /// </summary>
        [DomKeyword("Lang4")]
        Lang4,

        /// <summary>
        /// Japanese (word-processing keyboard): Zenkaku/Hankaku
        /// </summary>
        [DomKeyword("Lang5")]
        Lang5,

        /// <summary>
        /// Japanese: 無変換 (muhenkan)
        /// </summary>
        [DomKeyword("NonConvert")]
        NonConvert,


        /* Control Pad */
        /// <summary>
        /// ⌦. The forward delete key.	Note that on Apple keyboards, the key labelled Delete on the main part of the keyboard should be encoded as "Backspace".
        /// </summary>
        [DomKeyword("Delete")]
        Delete,

        /// <summary>
        /// Page Down, End or ↘
        /// </summary>
        [DomKeyword("End")]
        End,

        /// <summary>
        /// Help. Not present on standard PC keyboards.
        /// </summary>
        [DomKeyword("Help")]
        Help,

        /// <summary>
        /// Home or ↖
        /// </summary>
        [DomKeyword("Home")]
        Home,

        /// <summary>
        /// Insert or Ins. Not present on Apple keyboards.
        /// </summary>
        [DomKeyword("Insert")]
        Insert,

        /// <summary>
        /// Page Down, PgDn or ⇟
        /// </summary>
        [DomKeyword("PageDown")]
        PageDown,

        /// <summary>
        /// Page Up, PgUp or ⇞
        /// </summary>
        [DomKeyword("PageUp")]
        PageUp,



        /* Arrow Pad */

        /// <summary>
        /// ↓
        /// </summary>
        [DomKeyword("ArrowDown")]
        ArrowDown,

        /// <summary>
        /// ←
        /// </summary>
        [DomKeyword("ArrowLeft")]
        ArrowLeft,

        /// <summary>
        /// →
        /// </summary>
        [DomKeyword("ArrowRight")]
        ArrowRight,

        /// <summary>
        /// ↑
        /// </summary>
        [DomKeyword("ArrowUp")]
        ArrowUp,



        /* Numpad Section */


        /// <summary>
        /// On the Mac, the "NumLock" code should be used for the numpad Clear key.
        /// </summary>
        [DomKeyword("NumLock")]
        NumLock,

        /// <summary>
        /// 0 Ins on a keyboard 0 on a phone or remote control
        /// </summary>
        [DomKeyword("Numpad0")]
        Numpad0,

        /// <summary>
        /// 1 End on a keyboard 1 or 1 QZ on a phone or remote control
        /// </summary>
        [DomKeyword("Numpad1")]
        Numpad1,

        /// <summary>
        /// 2 ↓ on a keyboard 2 ABC on a phone or remote control
        /// </summary>
        [DomKeyword("Numpad2")]
        Numpad2,

        /// <summary>
        /// 3 PgDn on a keyboard 3 DEF on a phone or remote control
        /// </summary>
        [DomKeyword("Numpad3")]
        Numpad3,

        /// <summary>
        /// 4 ← on a keyboard 4 GHI on a phone or remote control
        /// </summary>
        [DomKeyword("Numpad4")]
        Numpad4,

        /// <summary>
        /// 5 on a keyboard 5 JKL on a phone or remote control
        /// </summary>
        [DomKeyword("Numpad5")]
        Numpad5,

        /// <summary>
        /// 6 → on a keyboard 6 MNO on a phone or remote control
        /// </summary>
        [DomKeyword("Numpad6")]
        Numpad6,

        /// <summary>
        /// 7 Home on a keyboard 7 PQRS or 7 PRS on a phone  or remote control
        /// </summary>
        [DomKeyword("Numpad7")]
        Numpad7,

        /// <summary>
        /// 8 ↑ on a keyboard 8 TUV on a phone or remote control
        /// </summary>
        [DomKeyword("Numpad8")]
        Numpad8,

        /// <summary>
        /// 9 PgUp on a keyboard 9 WXYZ or 9 WXY on a phone  or remote control
        /// </summary>
        [DomKeyword("Numpad9")]
        Numpad9,

        /// <summary>
        /// +
        /// </summary>
        [DomKeyword("NumpadAdd")]
        NumpadAdd,

        /// <summary>
        /// Found on the Microsoft Natural Keyboard.
        /// </summary>
        [DomKeyword("NumpadBackspace")]
        NumpadBackspace,

        /// <summary>
        /// C or AC (All Clear). Also for use with numpads that have a Clear key that is separate from the NumLock key. On the Mac, the numpad Clear key should always be encoded as "NumLock".
        /// </summary>
        [DomKeyword("NumpadClear")]
        NumpadClear,

        /// <summary>
        /// CE (Clear Entry)
        /// </summary>
        [DomKeyword("NumpadClearEntry")]
        NumpadClearEntry,

        /// <summary>
        /// , (thousands separator). For locales where the thousands separator	is a "." (e.g., Brazil), this key may generate a ..
        /// </summary>
        [DomKeyword("NumpadComma")]
        NumpadComma,

        /// <summary>
        /// . Del. For locales where the decimal separator is "," (e.g., Brazil), this key may generate a,.
        /// </summary>
        [DomKeyword("NumpadDecimal")]
        NumpadDecimal,

        /// <summary>
        /// /
        /// </summary>
        [DomKeyword("NumpadDivide")]
        NumpadDivide,
        /// <summary>
        /// Newline
        /// </summary>
        [DomKeyword("NumpadEnter")]
        ),

        /// <summary>
        /// =
        /// </summary>
        [DomKeyword("NumpadEqual")]
        NumpadEqual,

        /// <summary>
        /// # on a phone or remote control device. This key is typically found	below the 9 key and to the right of the 0 key.
        /// </summary>
        [DomKeyword("NumpadHash")]
        NumpadHash,

        /// <summary>
        /// M+ Add current entry to the value stored in memory.
        /// </summary>
        [DomKeyword("NumpadMemoryAdd")]
        NumpadMemoryAdd,

        /// <summary>
        /// MC Clear the value stored in memory.
        /// </summary>
        [DomKeyword("NumpadMemoryClear")]
        NumpadMemoryClear,

        /// <summary>
        /// MR Replace the current entry with the value stored in memory.
        /// </summary>
        [DomKeyword("NumpadMemoryRecall")]
        NumpadMemoryRecall,

        /// <summary>
        /// MS Replace the value stored in memory with the current entry.
        /// </summary>
        [DomKeyword("NumpadMemoryStore")]
        NumpadMemoryStore,

        /// <summary>
        /// M- Subtract current entry from the value stored in memory.
        /// </summary>
        [DomKeyword("NumpadMemorySubtract")]
        NumpadMemorySubtract,

        /// <summary>
        /// * on a keyboard. For use with numpads that provide mathematical operations (+, -, * and /). Use "NumpadStar" for the * key on phones and remote controls.
        /// </summary>
        [DomKeyword("NumpadMultiply")]
        NumpadMultiply,

        /// <summary>
        /// ( Found on the Microsoft Natural Keyboard.
        /// </summary>
        [DomKeyword("NumpadParenLeft")]
        NumpadParenLeft,

        /// <summary>
        /// ) Found on the Microsoft Natural Keyboard.
        /// </summary>
        [DomKeyword("NumpadParenRight")]
        NumpadParenRight,

        /// <summary>
        /// * on a phone or remote control device.	This key is typically found below the 7 key and to the left of the 0 key. Use "NumpadMultiply" for the * key on numeric keypads.
        /// </summary>
        [DomKeyword("NumpadStar")]
        NumpadStar,

        /// <summary>
        /// -
        /// </summary>
        [DomKeyword("NumpadSubtract")]
        NumpadSubtract,



        /* Function Section */


        /// <summary>
        /// Esc or ⎋
        /// </summary>
        [DomKeyword("Escape")]
        Escape,

        /// <summary>
        /// F1
        /// </summary>
        [DomKeyword("F1")]
        F1,

        /// <summary>
        /// F2
        /// </summary>
        [DomKeyword("F2")]
        F2,

        /// <summary>
        /// F3
        /// </summary>
        [DomKeyword("F3")]
        F3,

        /// <summary>
        /// F4
        /// </summary>
        [DomKeyword("F4")]
        F4,

        /// <summary>
        /// F5
        /// </summary>
        [DomKeyword("F5")]
        F5,

        /// <summary>
        /// F6
        /// </summary>
        [DomKeyword("F6")]
        F6,

        /// <summary>
        /// F7
        /// </summary>
        [DomKeyword("F7")]
        F7,

        /// <summary>
        /// F8
        /// </summary>
        [DomKeyword("F8")]
        F8,

        /// <summary>
        /// F9
        /// </summary>
        [DomKeyword("F9")]
        F9,

        /// <summary>
        /// F10
        /// </summary>
        [DomKeyword("F10")]
        F10,

        /// <summary>
        /// F11
        /// </summary>
        [DomKeyword("F11")]
        F11,

        /// <summary>
        /// F12
        /// </summary>
        [DomKeyword("F12")]
        F12,

        /// <summary>
        /// Fn This is typically a hardware key that does not generate a separate   code. Most keyboards do not place this key in the function section, but it is included here to keep it with related keys.
        /// </summary>
        [DomKeyword("Fn")]
        Fn,

        /// <summary>
        /// FLock or FnLock. Function Lock key. Found on the Microsoft  Natural Keyboard.
        /// </summary>
        [DomKeyword("FnLock")]
        FnLock,

        /// <summary>
        /// PrtScr SysRq or Print Screen
        /// </summary>
        [DomKeyword("PrintScreen")]
        PrintScreen,

        /// <summary>
        /// Scroll Lock
        /// </summary>
        [DomKeyword("ScrollLock")]
        ScrollLock,

        /// <summary>
        /// Pause Break
        /// </summary>
        [DomKeyword("Pause")]
        Pause,



        /* Media Keys */


        /// <summary>
        /// Some laptops place this key to the left of the ↑ key.
        /// </summary>
        [DomKeyword("BrowserBack")]
        BrowserBack,
        /// <summary>
        /// No definition
        /// </summary>
        [DomKeyword("BrowserFavorites")]
        BrowserFavorites,

        /// <summary>
        /// Some laptops place this key to the right of the ↑ key.
        /// </summary>
        [DomKeyword("BrowserForward")]
        BrowserForward,

        /// <summary>
        ///         
        /// </summary>
        [DomKeyword("BrowserHome")]
        BrowserHome,

        /// <summary>
        ///         
        /// </summary>
        [DomKeyword("BrowserRefresh")]
        BrowserRefresh,

        /// <summary>
        ///         
        /// </summary>
        [DomKeyword("BrowserSearch")]
        BrowserSearch,

        /// <summary>
        ///         
        /// </summary>
        [DomKeyword("BrowserStop")]
        BrowserStop,

        /// <summary>
        /// Eject or ⏏. This key is placed in the function  section on some Apple keyboards.
        /// </summary>
        [DomKeyword("Eject")]
        Eject,

        /// <summary>
        /// Sometimes labelled My Computer on the keyboard
        /// </summary>
        [DomKeyword("LaunchApp1")]
        LaunchApp1,

        /// <summary>
        /// Sometimes labelled Calculator on the keyboard
        /// </summary>
        [DomKeyword("LaunchApp2")]
        LaunchApp2,

        /// <summary>
        ///         
        /// </summary>
        [DomKeyword("LaunchMail")]
        LaunchMail,

        /// <summary>
        ///         
        /// </summary>
        [DomKeyword("MediaPlayPause")]
        MediaPlayPause,

        /// <summary>
        ///         
        /// </summary>
        [DomKeyword("MediaSelect")]
        MediaSelect,

        /// <summary>
        ///         
        /// </summary>
        [DomKeyword("MediaStop")]
        MediaStop,

        /// <summary>
        ///         
        /// </summary>
        [DomKeyword("MediaTrackNext")]
        MediaTrackNext,

        /// <summary>
        ///         
        /// </summary>
        [DomKeyword("MediaTrackPrevious")]
        MediaTrackPrevious,

        /// <summary>
        /// This key is placed in the function section on some Apple keyboards, replacing the Eject key.
        /// </summary>
        [DomKeyword("Power")]
        Power,

        /// <summary>
        ///         
        /// </summary>
        [DomKeyword("Sleep")]
        Sleep,

        /// <summary>
        ///         
        /// </summary>
        [DomKeyword("AudioVolumeDown")]
        AudioVolumeDown,

        /// <summary>
        ///         
        /// </summary>
        [DomKeyword("AudioVolumeMute")]
        AudioVolumeMute,

        /// <summary>
        ///         
        /// </summary>
        [DomKeyword("AudioVolumeUp")]
        AudioVolumeUp,

        /// <summary>
        /// 
        /// </summary>
        [DomKeyword("WakeUp")]
        WakeUp,


    }

}
