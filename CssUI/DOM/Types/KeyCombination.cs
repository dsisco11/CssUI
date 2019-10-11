using CssUI.Devices;
using System.Collections.Generic;
using System.Linq;

namespace CssUI.DOM
{
    /// <summary>
    /// Holds a set of keys for reference as a key combination for keyboard commands
    /// </summary>
    public class KeyCombination
    {
        #region Properties
        /// <summary>
        /// The combination of keys
        /// </summary>
        public readonly EKeyboardCode[] Keys;

        /// <summary>
        /// Control (control) key modifier.
        /// </summary>
        public bool ctrlKey { get; private set; } = false;
        /// <summary>
        /// shift (Shift) key modifier.
        /// </summary>
        public bool shiftKey { get; private set; } = false;
        /// <summary>
        /// Alt (alternative) (or "Option") key modifier.
        /// </summary>
        public bool altKey { get; private set; } = false;
        /// <summary>
        /// meta (Meta) key modifier.
        /// </summary>
        public bool metaKey { get; private set; } = false;
        #endregion

        #region Constructors
        public KeyCombination(bool ctrlKey = false, bool shiftKey = false, bool altKey = false, bool metaKey = false, params EKeyboardCode[] keys)
        {
            this.ctrlKey = ctrlKey;
            this.shiftKey = shiftKey;
            this.altKey = altKey;
            this.metaKey = metaKey;

            LinkedList <EKeyboardCode> keyList = new LinkedList<EKeyboardCode>();
            foreach (EKeyboardCode key in keys)
            {
                switch (key)
                {
                    case EKeyboardCode.AltLeft:
                    case EKeyboardCode.AltRight:
                        this.altKey = true;
                        break;
                    case EKeyboardCode.ControlLeft:
                    case EKeyboardCode.ControlRight:
                        this.ctrlKey = true;
                        break;
                    case EKeyboardCode.ShiftLeft:
                    case EKeyboardCode.ShiftRight:
                        this.shiftKey = true;
                        break;
                    case EKeyboardCode.MetaLeft:
                    case EKeyboardCode.MetaRight:
                        this.metaKey = true;
                        break;

                    default:
                        keyList.AddLast(key);
                        break;
                }

            }

            Keys = keyList.OrderBy(k => (int)k).ToArray();
        }
        #endregion

        #region Checks
        #endregion

        #region Hashing
        public override int GetHashCode()
        {
            int hash = 17;
            foreach (EKeyboardCode key in Keys)
            {
                hash += (int)key * 31;
            }

            /* Shift hash by 4 bits and use those bits to signify our modifier key states */
            hash = (hash << 4);
            /* Unset the first 4 bits */
            hash = hash & ~0xF;
            if (ctrlKey) hash = (hash | 0x1);
            if (shiftKey) hash = (hash | 0x2);
            if (altKey) hash = (hash | 0x4);
            if (metaKey) hash = (hash | 0x8);

            return hash;
        }
        #endregion
    }
}
