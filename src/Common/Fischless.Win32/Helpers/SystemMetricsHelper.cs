using System.Diagnostics.CodeAnalysis;
using Vanara.PInvoke;

namespace Fischless.Win32;

public static class SystemMetricsHelper
{
    [SuppressMessage("Interoperability", "CA1416:Validate platform compatibility")]
    public static (int, int) GetSystemMetrics(User32.SystemMetric metric)
    {
        if (Environment.OSVersion.Platform == PlatformID.Win32NT)
        {
            int cxMinBtn = User32.GetSystemMetrics(User32.SystemMetric.SM_CXMIN);
            int cyMinBtn = User32.GetSystemMetrics(User32.SystemMetric.SM_CYMIN);
            return (cxMinBtn, cyMinBtn);
        }
        return (default, default);
    }
}
