

namespace CssUI.Platform
{
    /// <summary>
    /// Implements IPlatformFactory functionality that is common among all platforms.
    /// IPlatformFactory implementations should inherit from this class.
    /// </summary>
    abstract class PlatformFactoryBase : IPlatformMethods
    {
        /// <summary>
        /// Provides access to important system dependant, user input related, thresholds
        /// </summary>
        public SystemMetricsBase SystemMetrics { get; protected set; }
        
    }
}
