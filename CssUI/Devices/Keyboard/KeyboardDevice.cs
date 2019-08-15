namespace CssUI.Devices
{
    /// <summary>
    /// Represents the systems keyboard device
    /// </summary>
    public abstract class KeyboardDevice
    {
        /// <summary>
        /// Returns true if the given <paramref name="key"/> on the systems current keyboard device is currently in the 'down' state
        /// </summary>
        /// <param name="key">They key to check for</param>
        public abstract bool IsDown(EKeyboardCode key);

        /// <summary>
        /// Returns true if the systems current keyboard device has the given <paramref name="key"/> on it
        /// </summary>
        /// <param name="key">They key to check for</param>
        public abstract bool Has_Key(EKeyboardCode key);

        /// <summary>
        /// Returns true if the systems current keyboard device has the given <paramref name="key"/> on it
        /// </summary>
        /// <param name="key">They key to check for</param>
        public abstract bool Has_Key(char key);


        /// <summary>
        /// Translates a given keyboard key into a key character
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public abstract EKeyboardCode TranslateKey(char key);
    }
}
