
namespace CssUI.Platform.Windows
{
    /// <summary>
    /// Provides access to generic functions that change depending on the current builds targeted OS
    /// </summary>
    class WinFactory : PlatformFactoryBase
    {

        public WinFactory()
        {
            SystemMetrics = new WinMetrics();
        }

    }
}
