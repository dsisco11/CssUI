namespace CssUI.DOM.Events
{
    public enum EKeyLocation : ulong
    {/* Docs: https://w3c.github.io/uievents/#events-keyboard-key-location */
        /// <summary>
        /// The key activation MUST NOT be distinguished as the left or right version of the key, and (other than the NumLock key) 
        /// did not originate from the numeric keypad (or did not originate with a virtual key corresponding to the numeric keypad).
        /// </summary>
        STANDARD = 0x0,

        /// <summary>
        /// The key activated originated from the left key location (when there is more than one possible location for this key).
        /// </summary>
        LEFT = 0x01,

        /// <summary>
        /// The key activation originated from the right key location (when there is more than one possible location for this key).
        /// </summary>
        RIGHT = 0x02,

        /// <summary>
        /// The key activation originated on the numeric keypad or with a virtual key corresponding to the numeric keypad (when there is more than one possible location for this key). 
        /// Note that the NumLock key should always be encoded with a location of DOM_KEY_LOCATION_STANDARD.
        /// </summary>
        NUMPAD = 0x03
    }
}
