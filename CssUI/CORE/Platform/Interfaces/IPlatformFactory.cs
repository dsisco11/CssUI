
namespace CssUI.Platform
{
    interface IPlatformFactoryMethods
    {
        ISystemMetricsHandler SystemMetrics { get; }
        ISystemScreensHandler SystemScreens { get; }
        ISystemWindowHandler  SystemWindows { get; }
    }
}
