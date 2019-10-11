using System;
using System.Threading;

namespace CssUI
{
    public class ScheduledFunction
    {
        #region Properties
        public TimeSpan Delay { get; set; }
        Action func;
        bool bStarted;
        Timer timer;
        #endregion

        #region Constructors
        public ScheduledFunction(Action func)
            : this(func, TimeSpan.FromMilliseconds(-1))
        {
        }

        public ScheduledFunction(Action func, TimeSpan delay) 
            : this(delay, func)
        {
        }

        public ScheduledFunction(TimeSpan delay, Action func)
        {
            this.func = func;
            this.Delay = delay;

            timer = new Timer(Tick, null, TimeSpan.FromMilliseconds(-1), delay);
        }

        ~ScheduledFunction()
        {
            Stop();
        }
        #endregion


        public void Start()
        {
            if (bStarted)
                return;

            bStarted = timer.Change(TimeSpan.Zero, Delay);
        }

        public void Stop()
        {
            if (!bStarted)
                return;

            bStarted = !timer.Change(TimeSpan.FromMilliseconds(-1), Delay);
        }


        void Tick(object state)
        {
            func?.Invoke();
        }
    }

}
