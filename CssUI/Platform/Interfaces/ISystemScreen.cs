
using System;

namespace CssUI.Platform
{
    /// <summary>
    /// This interface represents the basis for what any monitor object should consist of reguardless of platform
    /// </summary>
    public interface ISystemScreen
    {
        IntPtr Handle { get; }
    }
}
