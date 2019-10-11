

namespace CssUI.Platform
{
    /// <summary>
    /// Implements IPlatformFactory functionality that is common among all platforms.
    /// IPlatformFactory implementations should inherit from this class.
    /// </summary>
    abstract class PlatformFactoryBase : IPlatformFactoryMethods
    {
        /// <summary>
        /// Provides access to important system dependant configuration information such as double click time, drag drop distance, and screen DPI
        /// </summary>
        public ISystemMetricsHandler SystemMetrics { get; protected set; }

        /// <summary>
        /// Provides access to handling screens/monitors
        /// </summary>
        public ISystemScreensHandler SystemScreens { get; protected set; }

        /// <summary>
        /// Provides access to handling application windows
        /// </summary>
        public ISystemWindowHandler SystemWindows { get; protected set; }
    }
}
