
namespace CssUI.Platform
{
    public static class Factory
    {
        static IPlatformFactoryMethods self;
        /// <summary>
        /// Creates the applicable IPlatformMethods implementation for the current operating system
        /// </summary>
        /// <returns></returns>
        static Factory()
        {
#if WINDOWS
            self = new Windows.WinFactory();
#endif
#if LINUX_DEBIAN
            // XXX: implement basic debian support
#endif
        }

        #region Accessors

        public static ISystemMetricsHandler SystemMetrics { get { return self.SystemMetrics; } }
        public static ISystemScreensHandler SystemScreens { get { return self.SystemScreens; } }
        public static ISystemWindowHandler  SystemWindows { get { return self.SystemWindows; } }
        #endregion
    }
}
