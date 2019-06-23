using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CssUI
{
    public class RoutedEventArgs
    {
        #region Properties
        /// <summary>
        /// If set to True then the event handling process will stop at the current element.
        /// </summary>
        public bool Handled = false;
        /// <summary>
        /// The original reporting source for this event (determined by hit testing)
        /// </summary>
        public readonly object OriginSource;
        /// <summary>
        /// Object that is raising the event
        /// </summary>
        public object Source;
        #endregion

        public RoutedEventArgs(object Source)
        {
            this.OriginSource = Source;
            this.Source = Source;
        }
    }
}
