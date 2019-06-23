using KeyPic.CSSUI.Enums;

namespace CssUI
{
    public class DomKeyboardKeyEventArgs
    {
        public readonly bool IsRepeat;
        public readonly bool Alt;
        public readonly bool Shift;
        public readonly bool Control;
        public readonly EKey Key;

        public DomKeyboardKeyEventArgs(EKey Key, bool IsRepeat, bool Alt, bool Shift, bool Control)
        {
            this.Key = Key;
            this.IsRepeat = IsRepeat;
            this.Alt = Alt;
            this.Shift = Shift;
            this.Control = Control;
        }

    }
}
