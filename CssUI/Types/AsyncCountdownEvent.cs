using System;
using System.Threading;
using System.Threading.Tasks;

namespace CssUI
{
    public class AsyncCountdownEvent
    {
        #region Properties
        private readonly AsyncManualResetEvent m_amre = new AsyncManualResetEvent();
        private int m_count;
        #endregion

        #region Accessors
        public int CurrentCount => m_count;
        #endregion

        #region Constructors
        public AsyncCountdownEvent(int initialCount)
        {
            if (initialCount <= 0) throw new ArgumentOutOfRangeException("initialCount");
            m_count = initialCount;
        }
        #endregion
        
        public Task WaitAsync() => m_amre.WaitAsync();

        public Task WaitAsync(TimeSpan Timeout) => m_amre.WaitAsync( Timeout );

        public void Signal()
        {
            if (m_count <= 0)
                throw new InvalidOperationException();

            int newCount = Interlocked.Decrement(ref m_count);
            if (newCount == 0)
                m_amre.Set();
            else if (newCount < 0)
                throw new InvalidOperationException();
        }

        public Task SignalAndWait()
        {
            Signal();
            return WaitAsync();
        }

        /// <summary>
        /// Increments the total count by one
        /// </summary>
        public void AddCount()
        {
            m_count += 1;
        }
    }
}
