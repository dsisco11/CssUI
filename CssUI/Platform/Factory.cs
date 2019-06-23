
namespace CssUI.Platform
{
    public static class Factory
    {
        static IPlatformMethods self;
        /// <summary>
        /// Creates the applicable IPlatformMethods implementation for the current operating system
        /// </summary>
        /// <returns></returns>
        static Factory()
        {
            self = new Windows.WinFactory();
        }

        #region Accessors

        public static SystemMetricsBase SystemMetrics { get { return self.SystemMetrics; } }
        #endregion
    }
}
