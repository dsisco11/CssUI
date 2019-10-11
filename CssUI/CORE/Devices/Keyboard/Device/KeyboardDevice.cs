using System;
using System.Threading;

namespace CssUI.Devices
{
    /// <summary>
    /// Represents the systems keyboard device
    /// </summary>
    public abstract class KeyboardDevice : IDisposable
    {
        #region Static
        public static KeyboardDevice PrimaryDevice { get; private set; } = null;
        #endregion

        #region Constructors
        public KeyboardDevice()
        {
            PrimaryDevice = this;
        }

        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls
        private int Disposed = 0;

        protected virtual void Dispose(bool userInitiated)
        {
            if (Interlocked.Exchange(ref Disposed, 1) == 0)
            {
                if (userInitiated)
                {
                    PrimaryDevice = null;
                }
            }
        }

        ~KeyboardDevice()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(false);
        }

        // This code added to correctly implement the disposable pattern.
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);
             GC.SuppressFinalize(this);
        }
        #endregion
        #endregion


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
