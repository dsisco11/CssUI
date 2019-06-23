using System;
using System.Threading;
using System.Threading.Tasks;

namespace Vault
{
    public class AsyncManualResetEvent
    {
        private volatile TaskCompletionSource<bool> m_tcs = new TaskCompletionSource<bool>();
        private Timer timer_timeout = null;

        public Task WaitAsync()
        {
            return m_tcs.Task;
        }

        public Task WaitAsync(TimeSpan timeout)
        {
            timer_timeout = new Timer((object state) =>
            {
                var task = state as TaskCompletionSource<bool>;
                task.TrySetCanceled();
            }, m_tcs, timeout, TimeSpan.FromMilliseconds(-1));

            return m_tcs.Task;
        }

        public void Set()
        {
            // Stop the timeour timer
            if (timer_timeout != null)
            {
                timer_timeout.Change(Timeout.Infinite, Timeout.Infinite);
                timer_timeout.Dispose();
                timer_timeout = null;
            }

            m_tcs.TrySetResult(true);

            //// Dont allow the set function to execute callbacks on the calling thread
            //Task.Factory.StartNew(s => 
            //{
            //    ((TaskCompletionSource<bool>)s).TrySetResult(true);
            //}, m_tcs, CancellationToken.None, TaskCreationOptions.PreferFairness, TaskScheduler.Default);
            //m_tcs.Task.Wait();
        }

        public void Reset()
        {
            while (true)
            {
                var tcs = m_tcs;
                if (!tcs.Task.IsCompleted ||
                    Interlocked.CompareExchange(ref m_tcs, new TaskCompletionSource<bool>(), tcs) == tcs)
                    return;
            }
        }
    }
}
